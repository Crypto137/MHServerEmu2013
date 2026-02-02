using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.DRAG.Generators.Regions;
using MHServerEmu.Games.Entities.Avatars;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Properties;
using MHServerEmu.Games.Regions;

namespace MHServerEmu.Games.Entities
{
    public class Transition : WorldEntity
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private RepVar_string _name = new();
        private RepVar_PrototypeDataRef _regionRef = new();    
        private RepVar_PrototypeDataRef _areaRef = new();
        private LocaleStringId _nameId;

        private PrototypeId _targetRef;
        private ulong _entityId;

        public TransitionPrototype TransitionPrototype { get => Prototype as TransitionPrototype; }

        public Transition(Game game) : base(game)
        {
            SetFlag(EntityFlags.IsNeverAffectedByPowers, true);
        }

        public override bool Initialize(EntitySettings settings)
        {
            base.Initialize(settings);

            FindDestination(settings.Cell, TransitionPrototype);

            return true;
        }

        protected override void BindReplicatedFields()
        {
            base.BindReplicatedFields();

            // TODO: Check channels
            _name.Bind(this, AOINetworkPolicyValues.AllChannels);
            _regionRef.Bind(this, AOINetworkPolicyValues.AllChannels);
            _areaRef.Bind(this, AOINetworkPolicyValues.AllChannels);
        }

        protected override void UnbindReplicatedFields()
        {
            base.UnbindReplicatedFields();

            _name.Unbind();
            _regionRef.Unbind();
            _areaRef.Unbind();
        }

        protected override void OnEnteredWorld(EntitySettings settings)
        {
            var transProto = TransitionPrototype;

            switch (transProto.Type)
            {
                case RegionTransitionType.Transition:
                case RegionTransitionType.TransitionDirectReturn:

                    var area = Area;
                    var entityRef = PrototypeDataRef;
                    var cellRef = Cell.PrototypeDataRef;
                    var region = Region;
                    bool noDest = _targetRef == PrototypeId.Invalid;
                    if (noDest && area.RandomInstances.Count > 0)
                        foreach (var instance in area.RandomInstances)
                        {
                            var instanceCell = GameDatabase.GetDataRefByAsset(instance.OriginCell);
                            if (instanceCell == PrototypeId.Invalid || cellRef != instanceCell) continue;
                            if (instance.OriginEntity != entityRef) continue;

                            if (SetFromTarget(instance.Target))
                                noDest = false;
                        }

                    if (noDest)
                    {
                        // TODO destination from region origin target
                        var targets = region.Targets;
                        if (targets.Count == 1 && SetFromTarget(targets[0].TargetId))
                            noDest = false;
                    }

                    // Get default region
                    if (noDest)
                        SetFromTarget(GameDatabase.GlobalsPrototype.DefaultStartTarget);

                    break;

                case RegionTransitionType.TransitionDirect:

                    var avatar = Game.EntityManager.GetEntity<Avatar>(settings.SourceEntityId);
                    var player = avatar?.GetOwnerOfType<Player>();
                    if (player == null) break;
                    Properties[PropertyEnum.RestrictedToPlayerGuidParty] = player.DatabaseUniqueId;

                    SetFromTarget(transProto.DirectTarget);
                    break;
            }

            base.OnEnteredWorld(settings);
        }

        public override bool Serialize(Archive archive)
        {
            bool success = base.Serialize(archive);

            if (archive.IsTransient)
            {
                success &= Serializer.Transfer(archive, ref _name);
                success &= Serializer.Transfer(archive, ref _regionRef);
                success &= Serializer.Transfer(archive, ref _areaRef);
                success &= Serializer.Transfer(archive, ref _nameId);
            }

            return success;
        }

        public void ConfigureTowerGen(Transition transition)
        {
            _entityId = transition.Id;
        }

        public bool UseTransition(Player player)
        {
            switch (TransitionPrototype.Type)
            {
                case RegionTransitionType.Transition:
                case RegionTransitionType.TransitionDirect:
                    Region region = player.GetRegion();
                    if (region == null) return Logger.WarnReturn(false, "UseTransition(): region == null");

                    PrototypeId targetRegionProtoRef = _regionRef.Get();

                    // Check if our target is outside of the current region and we need to do a remote teleport
                    // TODO: Additional checks if we need to transfer (e.g. when transferring to another instance of the same region proto).
                    if (targetRegionProtoRef != PrototypeId.Invalid && region.PrototypeDataRef != targetRegionProtoRef)
                        return TeleportToRemoteTarget(player, _targetRef);

                    // No need to transfer if we are already in the target region
                    return TeleportToLocalTarget(player, _targetRef);

                case RegionTransitionType.TowerUp:
                case RegionTransitionType.TowerDown:
                    return TeleportToTransition(player, _entityId);

                case RegionTransitionType.Waypoint:
                    // TODO: Unlock waypoint
                    return true;

                case RegionTransitionType.TransitionDirectReturn:
                    // TODO teleport to Position in region
                    return TeleportToRemoteTarget(player, _targetRef);

                default:
                    return Logger.WarnReturn(false, $"UseTransition(): Unimplemented region transition type {TransitionPrototype.Type}");
            }
        }

        public static bool TeleportToRemoteTarget(Player player, PrototypeId targetProtoRef)
        {
            //Logger.Trace($"TeleportToRemoteTarget(): targetProtoRef={targetProtoRef.GetNameFormatted()}");
            player.PlayerConnection.MoveToTarget(targetProtoRef);
            return true;
        }

        public static bool TeleportToLocalTarget(Player player, PrototypeId targetProtoRef)
        {
            var targetProto = targetProtoRef.As<RegionConnectionTargetPrototype>();
            if (targetProto == null) return Logger.WarnReturn(false, "TeleportToLocalTarget(): targetProto == null");

            Region region = player.GetRegion();
            if (region == null) return Logger.WarnReturn(false, "TeleportToLocalTarget(): region == null");

            Vector3 position = Vector3.Zero;
            Orientation orientation = Orientation.Zero;

            if (region.FindTargetLocation(ref position, ref orientation,
                targetProto.Area, GameDatabase.GetDataRefByAsset(targetProto.Cell), targetProto.Entity) == false)
            {
                return Logger.WarnReturn(false, $"TeleportToLocalTarget(): Failed to find target location for target {targetProtoRef.GetName()}");
            }

            // V10_NOTE: No NetMessageOneTimeSnapCamera in 1.10?

            ChangePositionResult result = player.CurrentAvatar.ChangeRegionPosition(position, orientation, ChangePositionFlags.Teleport);
            return result == ChangePositionResult.PositionChanged || result == ChangePositionResult.Teleport;
        }

        public static bool TeleportToTransition(Player player, ulong transitionEntityId)
        {
            // This looks quite similar to TeleportToLocalTarget(), maybe we should merge them

            Transition transition = player.Game.EntityManager.GetEntity<Transition>(transitionEntityId);
            if (transition == null) return Logger.WarnReturn(false, "TeleportToTransition(): transition == null");

            TransitionPrototype transitionProto = transition.TransitionPrototype;
            if (transitionProto == null) return Logger.WarnReturn(false, "TeleportToTransition(): transitionProto == null");

            Vector3 targetPos = transition.RegionLocation.Position;
            Orientation targetRot = transition.RegionLocation.Orientation;
            targetPos += transitionProto.CalcSpawnOffset(targetRot);

            ChangePositionResult result = player.CurrentAvatar.ChangeRegionPosition(targetPos, targetRot, ChangePositionFlags.Teleport);
            return result == ChangePositionResult.PositionChanged || result == ChangePositionResult.Teleport;
        }

        private bool FindDestination(Cell cell, TransitionPrototype transitionProto)
        {
            if (transitionProto.Type == RegionTransitionType.Waypoint)
                return true;

            if (cell == null)
                return false;

            PrototypeId area = cell.Area.PrototypeDataRef;
            Region region = cell.Region;
            PrototypeGuid entityGuid = GameDatabase.GetPrototypeGuid(transitionProto.DataRef);

            TargetObject node = RegionTransition.GetTargetNode(region.Targets, area, cell.PrototypeDataRef, entityGuid);
            if (node == null)
                return false;

            return SetFromTarget(node.TargetId);
        }

        private bool SetFromTarget(PrototypeId targetProtoRef)
        {
            RegionConnectionTargetPrototype target = targetProtoRef.As<RegionConnectionTargetPrototype>();
            if (target == null)
                return false;

            //Logger.Trace($"SetFromTarget(): {Prototype} => {target}");

            _regionRef.Set(target.Region);
            _areaRef.Set(target.Area);
            _nameId = target.Name;

            _targetRef = target.DataRef;

            return true;
        }
    }
}
