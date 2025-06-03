using Gazillion;
using MHServerEmu.Core.Collisions;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.Entities.Avatars;
using MHServerEmu.Games.Entities.Locomotion;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Properties;
using MHServerEmu.Games.Regions;

namespace MHServerEmu.Games.Entities
{
    [Flags]
    public enum ChangePositionFlags
    {
        None                = 0,
        ForceUpdate         = 1 << 0,
        DoNotSendToOwner    = 1 << 1,
        DoNotSendToServer   = 1 << 2,
        DoNotSendToClients  = 1 << 3,
        Orientation         = 1 << 4,
        Force               = 1 << 5,
        Teleport            = 1 << 6,
        HighFlying          = 1 << 7,
        PhysicsResolve      = 1 << 8,
        SkipInterestUpdate  = 1 << 9,
        EnterWorld          = 1 << 10,
    }

    public enum ChangePositionResult
    {
        InvalidPosition,
        PositionChanged,
        NotChanged,
        Teleport
    }

    public class WorldEntity : Entity
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        public ConditionCollection ConditionCollection { get; private set; }
        public RegionLocation RegionLocation { get; private set; } = new();
        public Cell Cell { get => RegionLocation.Cell; }
        public Area Area { get => RegionLocation.Area; }
        public RegionLocationSafe ExitWorldRegionLocation { get; private set; } = new();
        public EntityRegionSpatialPartitionLocation SpatialPartitionLocation { get; }
        public Aabb RegionBounds { get; set; }
        public Bounds Bounds { get; set; } = new();
        public Region Region { get => RegionLocation.Region; }
        public Orientation Orientation { get => RegionLocation.Orientation; }
        public WorldEntityPrototype WorldEntityPrototype { get => Prototype as WorldEntityPrototype; }
        public bool ShouldSnapToFloorOnSpawn { get; private set; }
        public Locomotor Locomotor { get; protected set; }
        public virtual Bounds EntityCollideBounds { get => Bounds; set { } }
        public bool IsInWorld { get => RegionLocation.IsValid; }
        public bool IsAliveInWorld { get => IsInWorld && IsDead == false; }
        public virtual bool IsMovementAuthoritative { get => true; }


        // V10_NOTE: These seem to had been used instead of properties, all of these bind to Discovery/Party channels
        public RepVar_Vector3 MapLocation { get; private set; } = new();
        public RepVar_ulong MapRegionId { get; private set; } = new();
        public RepVar_uint MapAreaId { get; private set; } = new();
        public RepVar_uint MapCellId { get; private set; } = new();
        public RepVar_Vector3 MapOrientation { get; private set; } = new();    // Should be RepVar_Orientation

        public WorldEntity(Game game) : base(game)
        {
            SpatialPartitionLocation = new(this);
        }

        public override bool Initialize(EntitySettings settings)
        {
            if (base.Initialize(settings) == false) return Logger.WarnReturn(false, "Initialize(): base.Initialize(settings) == false");

            WorldEntityPrototype worldEntityProto = WorldEntityPrototype;

            ShouldSnapToFloorOnSpawn = settings.OptionFlags.HasFlag(EntitySettingsOptionFlags.HasOverrideSnapToFloor)
                ? settings.OptionFlags.HasFlag(EntitySettingsOptionFlags.OverrideSnapToFloorValue)
                : worldEntityProto.SnapToFloorOnSpawn;

            if (worldEntityProto.Bounds != null)
            {
                Bounds.InitializeFromPrototype(worldEntityProto.Bounds);
                if (settings.BoundsScaleOverride != 1f)
                    Bounds.Scale(settings.BoundsScaleOverride);
            }

            ConditionCollection = new(this);

            // V10_REMOVEME: replace with OnPropertyChange
            Properties[PropertyEnum.HealthMaxOther] = Properties[PropertyEnum.HealthMax];

            return true;
        }

        protected override void BindReplicatedFields()
        {
            base.BindReplicatedFields();

            MapLocation.Bind(this, AOINetworkPolicyValues.MapChannels);
            MapRegionId.Bind(this, AOINetworkPolicyValues.MapChannels);
            MapAreaId.Bind(this, AOINetworkPolicyValues.MapChannels);
            MapCellId.Bind(this, AOINetworkPolicyValues.MapChannels);
            MapOrientation.Bind(this, AOINetworkPolicyValues.MapChannels);
        }

        protected override void UnbindReplicatedFields()
        {
            base.UnbindReplicatedFields();

            MapLocation.Unbind();
            MapRegionId.Unbind();
            MapAreaId.Unbind();
            MapCellId.Unbind();
            MapOrientation.Unbind();
        }

        public override bool Serialize(Archive archive)
        {
            bool success = base.Serialize(archive);

            if (archive.IsTransient)
            {
                success &= Serializer.Transfer(archive, MapLocation);
                success &= Serializer.Transfer(archive, MapRegionId);
                success &= Serializer.Transfer(archive, MapAreaId);
                success &= Serializer.Transfer(archive, MapCellId);
                success &= Serializer.Transfer(archive, MapOrientation);

                // V10_NOTE: In 1.10 Locomotor inherits from ArchiveMessageHandler, meaning it has
                // a replication id that needs to be serialized if this world entity has a locomotor.
                // However, it has no data of its own (probably moved to LocomotionStateUpdate),
                // so we are just going to write a dummy value and move on with our lives.
                if (Locomotor != null)
                {
                    ulong dummyRepId = 0;
                    Serializer.Transfer(archive, ref dummyRepId);
                }
            }

            success &= Serializer.Transfer(archive, ConditionCollection);

            // PowerCollection
            bool hasPowerCollection = false;
            success &= Serializer.Transfer(archive, ref hasPowerCollection);
            // V10_TODO: PowerCollection serialization

            return success;
        }

        public virtual ChangePositionResult ChangeRegionPosition(Vector3? position, Orientation? orientation, ChangePositionFlags flags = ChangePositionFlags.None)
        {
            bool positionChanged = false;
            bool orientationChanged = false;
            Cell previousCell = Cell;

            RegionLocation preChangeLocation = new(RegionLocation);
            Region region = Game.RegionManager.GetRegion(preChangeLocation.RegionId);
            if (region == null) return ChangePositionResult.NotChanged;

            if (position.HasValue && (flags.HasFlag(ChangePositionFlags.ForceUpdate) || preChangeLocation.Position != position))
            {
                var result = RegionLocation.SetPosition(position.Value);

                if (result != RegionLocation.SetPositionResult.Success)     // onSetPositionFailure()
                {
                    return Logger.WarnReturn(ChangePositionResult.NotChanged, string.Format(
                        "ChangeRegionPosition(): Failed to set entity new position (Moved out of world)\n\tEntity: {0}\n\tResult: {1}\n\tPrev Loc: {2}\n\tNew Pos: {3}",
                        this, result, RegionLocation, position));
                }

                if (Bounds.Geometry != GeometryType.None)
                    Bounds.Center = position.Value;

                /* V10_TODO
                if (flags.HasFlag(ChangePositionFlags.PhysicsResolve) == false)
                    RegisterForPendingPhysicsResolve();
                */

                positionChanged = true;
            }

            if (orientation.HasValue && (flags.HasFlag(ChangePositionFlags.ForceUpdate) || preChangeLocation.Orientation != orientation))
            {
                RegionLocation.Orientation = orientation.Value;

                if (Bounds.Geometry != GeometryType.None)
                    Bounds.Orientation = orientation.Value;
                /* V10_TODO
                if (Physics.HasAttachedEntities())
                    RegisterForPendingPhysicsResolve();
                */
                orientationChanged = true;
            }

            if (positionChanged == false && orientationChanged == false)
                return ChangePositionResult.NotChanged;

            UpdateRegionBounds(); // Add to Quadtree

            if (RegionLocation.IsValid)
                ExitWorldRegionLocation.Set(RegionLocation);

            if (flags.HasFlag(ChangePositionFlags.DoNotSendToClients) == false)
            {
                var messageBuilder = NetMessageEntityPosition.CreateBuilder()
                    .SetIdEntity(Id)
                    .SetFlags((uint)flags);

                if (position != null)
                    messageBuilder.SetPosition(position.Value.ToNetStructPoint3());

                if (orientation != null)
                    messageBuilder.SetOrientation(orientation.Value.ToNetStructPoint3());

#if BUILD_1_10_0_69
                messageBuilder.SetCellId(RegionLocation.CellId);
                messageBuilder.SetAreaId(RegionLocation.AreaId);
                messageBuilder.SetEntityPrototypeId((ulong)PrototypeDataRef);
#endif

                Game.NetworkManager.SendMessageToInterested(messageBuilder.Build(), this, AOINetworkPolicyValues.AOIChannelProximity);
            }

            return ChangePositionResult.PositionChanged;
        }

        public RegionLocation ClearWorldLocation()
        {
            if (RegionLocation.IsValid) ExitWorldRegionLocation.Set(RegionLocation);
            if (Region != null && SpatialPartitionLocation.IsValid()) Region.RemoveEntityFromSpatialPartition(this);
            RegionLocation oldLocation = new(RegionLocation);
            RegionLocation.Set(RegionLocation.Invalid);
            return oldLocation;
        }

        public Vector3 FloorToCenter(Vector3 position)
        {
            Vector3 resultPosition = position;
            if (Bounds.Geometry != GeometryType.None)
                resultPosition.Z += Bounds.HalfHeight;
            // TODO Locomotor.GetCurrentFlyingHeight
            return resultPosition;
        }

        public bool ShouldUseSpatialPartitioning() => Bounds.Geometry != GeometryType.None;

        public EntityRegionSPContext GetEntityRegionSPContext()
        {
            EntityRegionSPContextFlags flags = EntityRegionSPContextFlags.ActivePartition;
            ulong playerRestrictedGuid = 0;

            WorldEntityPrototype entityProto = WorldEntityPrototype;
            if (entityProto == null) return new(flags);

            // V10_NOTE: No CanCollideWithPowerUserItems in 1.10?

            /* V10_TODO
            if (!(IsNeverAffectedByPowers || (IsHotspot && !IsCollidableHotspot && !IsReflectingHotspot)))
                flags |= EntityRegionSPContextFlags.StaticPartition;
            */

            return new(flags, playerRestrictedGuid);
        }

        public void UpdateRegionBounds()
        {
            RegionBounds = Bounds.ToAabb();
            if (ShouldUseSpatialPartitioning())
                Region.UpdateEntityInSpatialPartition(this);
        }

        public virtual bool EnterWorld(Region region, Vector3 position, Orientation orientation, EntitySettings settings = null)
        {
            SetStatus(EntityStatus.EnteringWorld, true);

            RegionLocation.Region = region;

            //Physics.AcquireCollisionId();

            ChangePositionResult result = ChangeRegionPosition(position, orientation,
                ChangePositionFlags.ForceUpdate | ChangePositionFlags.DoNotSendToServer | ChangePositionFlags.SkipInterestUpdate | ChangePositionFlags.EnterWorld);

            if (result == ChangePositionResult.PositionChanged)
            {
                //CancelExitWorldEvent();

                //ApplyState(Properties[PropertyEnum.EntityState]);

                OnEnteredWorld(settings);
            }
            else
            {
                ClearWorldLocation();
            }

            SetStatus(EntityStatus.EnteringWorld, false);

            return IsInWorld;
        }

        public void ExitWorld()
        {
            // V10_TODO: Stuff
            if (IsInWorld == false)
                return;

            bool exitStatus = !TestStatus(EntityStatus.ExitingWorld);
            SetStatus(EntityStatus.ExitingWorld, true);

            RegionLocation.Region = null;

            OnExitedWorld();
            var oldLocation = ClearWorldLocation();

            if (exitStatus)
                SetStatus(EntityStatus.ExitingWorld, false);
        }

        public void AssignPower(PrototypeId powerProtoRef)
        {
            var message = NetMessagePowerCollectionAssignPower.CreateBuilder()
                .SetEntityId(Id)
                .SetPowerProtoId((ulong)powerProtoRef)
                .SetTargetentityid(Id)
                .SetPowerRank(1)
                .SetCharacterLevel(1)
                .SetItemLevel(1)
                .SetPowerCollectionIsduplicating(false)
                .Build();

            Game.NetworkManager.SendMessageToInterested(message, this, AOINetworkPolicyValues.AOIChannelProximity);
        }

        #region Event Handlers

        protected virtual void OnEnteredWorld(EntitySettings settings)
        {
            UpdateInterestPolicies(true, settings);
        }

        protected virtual void OnExitedWorld()
        {
            UpdateInterestPolicies(false);
        }

        public virtual void OnLocomotionStateChanged(LocomotionState oldLocomotionState, LocomotionState newLocomotionState)
        {
            if (IsInWorld == false) return;

            // Check if locomotion state requires updating
            LocomotionState.CompareLocomotionStatesForSync(oldLocomotionState, newLocomotionState,
                out bool syncRequired, out bool pathNodeSyncRequired, newLocomotionState.FollowEntityId != InvalidId);

            if (syncRequired == false && pathNodeSyncRequired == false) return;

            // Send locomotion update to interested clients
            // NOTE: Avatars are locomoted on their local client independently, so they are excluded from locomotion updates.
            PlayerConnectionManager networkManager = Game.NetworkManager;
            List<PlayerConnection> interestedClientList = ListPool<PlayerConnection>.Instance.Get();
            if (networkManager.GetInterestedClients(interestedClientList, this, AOINetworkPolicyValues.AOIChannelProximity, IsMovementAuthoritative == false))
            {
                NetMessageLocomotionStateUpdate locomotionStateUpdateMessage = ArchiveMessageBuilder.BuildLocomotionStateUpdateMessage(
                    this, oldLocomotionState, newLocomotionState, pathNodeSyncRequired);

                networkManager.SendMessageToMultiple(interestedClientList, locomotionStateUpdateMessage);
            }

            ListPool<PlayerConnection>.Instance.Return(interestedClientList);
        }

        #endregion
    }
}
