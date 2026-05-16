using Gazillion;
using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.Entities.Inventories;
using MHServerEmu.Games.Entities.PowerCollections;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Properties;
using MHServerEmu.Games.Regions;
using MHServerEmu.Games.Social.Guilds;

namespace MHServerEmu.Games.Entities.Avatars
{
    public class Avatar : Agent
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private Player _owner;

        private ulong _guildId;
        private string _guildName;
        private GuildMembership _guildMembership;

        private List<AbilityKeyMapping> _abilityKeyMappings = new();

        public uint AvatarWorldInstanceId { get; private set; } = 0;
        public RepVar_string PlayerName { get; } = new();
        public RepVar_ulong PlayerDbId { get; } = new();     // Is this really PlayerDbId?

        public AvatarPrototype AvatarPrototype { get => Prototype as AvatarPrototype; }

        public PrototypeId CurrentTransformMode { get; private set; } = PrototypeId.Invalid;

        public override bool IsMovementAuthoritative { get => false; }

        public Avatar(Game game) : base(game)
        {
        }

        public override bool Initialize(EntitySettings settings)
        {
            base.Initialize(settings);

            // Default stats for a level 1 avatar
            AvatarPrototype avatarProto = GameDatabase.GetPrototype<AvatarPrototype>(PrototypeDataRef);

            Properties[PropertyEnum.CharacterLevel] = 1;

            Properties[PropertyEnum.EnduranceMax] = Properties[PropertyEnum.EnduranceBase];
            Properties[PropertyEnum.EnduranceMaxOther] = Properties[PropertyEnum.EnduranceMax];
            Properties[PropertyEnum.Endurance] = Properties[PropertyEnum.EnduranceMax];

            foreach (PowerProgressionTablePrototype powerProgTable in avatarProto.PowerProgressionTables)
            {
                foreach (PowerProgressionEntryPrototype powerProgEntry in powerProgTable.PowerProgressionEntries)
                {
                    PrototypeId powerProtoRef = powerProgEntry.PowerAssignment.Ability;

                    Properties[PropertyEnum.PowerRankCurrentBest, powerProtoRef] = 1;
                    Properties[PropertyEnum.PowerGrantRank, powerProtoRef] = 1;
                }
            }

            // Init AbilityKeyMapping
            if (settings.ArchiveData == null)
            {
                AbilityKeyMapping keyMapping = new();
                _abilityKeyMappings.Add(keyMapping);
                keyMapping.SlotDefaultAbilities(this);
            }

            return true;
        }

        protected override void BindReplicatedFields()
        {
            base.BindReplicatedFields();

            PlayerName.Bind(this, AOINetworkPolicyValues.AOIChannelProximity | AOINetworkPolicyValues.AOIChannelParty | AOINetworkPolicyValues.AOIChannelOwner);
            PlayerDbId.Bind(this, AOINetworkPolicyValues.AOIChannelProximity | AOINetworkPolicyValues.AOIChannelParty | AOINetworkPolicyValues.AOIChannelOwner);
        }

        protected override void UnbindReplicatedFields()
        {
            base.UnbindReplicatedFields();

            PlayerName.Unbind();
            PlayerDbId.Unbind();
        }

        public void SetPlayer(Player owner)
        {
            _owner = owner;
            PlayerName.Set(owner.PlayerName.Get());
            PlayerDbId.Set(owner.DatabaseUniqueId);
        }

        public override bool Serialize(Archive archive)
        {
            bool success = base.Serialize(archive);

            if (archive.IsTransient)
            {
                success &= Serializer.Transfer(archive, PlayerName);
                success &= Serializer.Transfer(archive, PlayerDbId);

                string emptyString = string.Empty;
                success &= archive.Transfer(ref emptyString);

                if (archive.IsReplication)
                    success &= GuildMember.SerializeReplicationRuntimeInfo(archive, ref _guildId, ref _guildName, ref _guildMembership);
            }

            success &= Serializer.Transfer(archive, ref _abilityKeyMappings);

            return success;
        }

        #region World and Positioning

        public override ChangePositionResult ChangeRegionPosition(Vector3? position, Orientation? orientation, ChangePositionFlags flags = ChangePositionFlags.None)
        {
            if (!Verify.IsTrue(position != null || orientation != null)) return ChangePositionResult.NotChanged;

            // Orientation only changes skip AOI processing
            if (position == null)
                return base.ChangeRegionPosition(position, orientation, flags);

            // Get player for AOI update
            Player player = GetOwnerOfType<Player>();
            if (!Verify.IsNotNull(player)) return ChangePositionResult.NotChanged;

            ChangePositionResult result;

            if (player.AOI.ContainsPosition(position.Value))
            {
                // V10_TODO: Persistent agents?

                // Do a normal position change and update AOI if the position is loaded
                result = base.ChangeRegionPosition(position, orientation, flags);
                if (result == ChangePositionResult.PositionChanged)
                {
                    // Increment AvatarWorldInstanceId before updating AOI to make sure it reaches clients.
                    if (flags.HasFlag(ChangePositionFlags.EnterWorld))
                        AvatarWorldInstanceId++;

                    player.AOI.Update(RegionLocation.Position);
                }
            }
            else
            {
                // If we are moving outside of our AOI, start a teleport and exit world.
                // The avatar will be put back into the world when all cells at the destination are loaded.
                if (!Verify.IsTrue(IsInWorld)) return ChangePositionResult.NotChanged;

                Region region = Region;
                if (!Verify.IsNotNull(region)) return ChangePositionResult.NotChanged;

                Cell cellAtPosition = region.GetCellAtPosition(position.Value);
                if (!Verify.IsNotNull(cellAtPosition)) return ChangePositionResult.NotChanged;

                player.BeginTeleport(RegionLocation.RegionId, position.Value, orientation != null ? orientation.Value : Orientation.Zero);
                // V10_NOTE: No CancelOnTransfer conditions in 1.10.
                ExitWorld();
                player.AOI.Update(position.Value);
                result = ChangePositionResult.Teleport;
            }

            /* V10_TODO
            if (result == ChangePositionResult.PositionChanged)
            {
                player.RevealDiscoveryMap(position.Value);
                player.UpdateSpawnMap(position.Value);
            }
            */

            return result;
        }

        #endregion

        #region Powers

        private void InitializePowers()
        {
            PowerIndexProperties indexProps = new(1, 1, 1);

            AssignPower(GameDatabase.GlobalsPrototype.AvatarSwapOutPower, indexProps);
            AssignPower(GameDatabase.GlobalsPrototype.AvatarSwapInPower, indexProps);
            AssignPower(GameDatabase.GlobalsPrototype.ReturnToHubPower, indexProps);
            AssignPower(GameDatabase.GlobalsPrototype.ReturnToFieldPower, indexProps);

            foreach (var kvp in Properties.IteratePropertyRange(PropertyEnum.PowerRankCurrentBest))
            {
                Property.FromParam(kvp.Key, 0, out PrototypeId powerProtoRef);
                AssignPower(powerProtoRef, indexProps);
            }
        }

        #endregion

        #region Ability Slot Management

        public bool SlotAbility(PrototypeId abilityProtoRef, AbilitySlot slot, bool skipEquipValidation, bool sendToClient)
        {
            AbilityKeyMapping keyMapping = GetAbilityKeyMappingIgnoreTransient();
            if (!Verify.IsNotNull(keyMapping)) return false;

            // V10_TODO: Validation

            // Set
            keyMapping.SetAbilityInAbilitySlot(abilityProtoRef, slot);

            // Notify the client if needed
            if (sendToClient)
            {
                Player player = GetOwnerOfType<Player>();
                if (!Verify.IsNotNull(player)) return false;

                if (player.InterestedInEntity(this, AOINetworkPolicyValues.AOIChannelOwner))
                {
                    player.SendMessage(NetMessageAbilityAssign.CreateBuilder()
                        .SetAvatarId(Id)
                        .SetAbilityProtoId((ulong)abilityProtoRef)
                        .SetSlot((int)slot)
                        .Build());
                }
            }

            return true;
        }

        public bool UnslotAbility(AbilitySlot slot, bool sendToClient)
        {
            AbilityKeyMapping keyMapping = GetAbilityKeyMappingIgnoreTransient();
            if (!Verify.IsNotNull(keyMapping)) return false;

            // V10_TODO: Validation

            // Remove by assigning invalid id
            keyMapping.SetAbilityInAbilitySlot(PrototypeId.Invalid, slot);

            // Notify the client if needed
            if (sendToClient)
            {
                Player player = GetOwnerOfType<Player>();
                if (!Verify.IsNotNull(player)) return false;

                if (player.InterestedInEntity(this, AOINetworkPolicyValues.AOIChannelOwner))
                {
                    player.SendMessage(NetMessageAbilityUnassign.CreateBuilder()
                        .SetAvatarId(Id)
                        .SetSlot((int)slot)
                        .Build());
                }
            }

            return true;
        }

        public bool SwapAbilities(AbilitySlot slotA, AbilitySlot slotB, bool sendToClient)
        {
            // V10_NOTE: Transient swaps are allowed in modern versions of the game, is this true in 1.10?
            AbilityKeyMapping keyMapping = GetAbilityKeyMappingIgnoreTransient();
            if (!Verify.IsNotNull(keyMapping)) return false;

            // V10_TODO: Validation

            // Do the swap            
            PrototypeId abilityA = keyMapping.GetAbilityInAbilitySlot(slotA);
            PrototypeId abilityB = keyMapping.GetAbilityInAbilitySlot(slotB);
            keyMapping.SetAbilityInAbilitySlot(abilityB, slotA);
            keyMapping.SetAbilityInAbilitySlot(abilityA, slotB);

            // Notify the client if needed
            if (sendToClient)
            {
                Player player = GetOwnerOfType<Player>();
                if (!Verify.IsNotNull(player)) return false;

                if (player.InterestedInEntity(this, AOINetworkPolicyValues.AOIChannelOwner))
                {
                    player.SendMessage(NetMessageAbilitySwap.CreateBuilder()
                        .SetAvatarId(Id)
                        .SetSlotA((int)slotA)
                        .SetSlotB((int)slotB)
                        .Build());
                }
            }

            return true;
        }

        private AbilityKeyMapping GetAbilityKeyMappingIgnoreTransient()
        {
            // 1.25 method - getAbilityKeyMappingForTransformMode(PrototypeId)
            if (_abilityKeyMappings.Count == 0)
                return null;

            return _abilityKeyMappings[0];
        }

        #endregion

        #region Interaction

        public bool UseInteractableObject(ulong entityId, PrototypeId missionRef)
        {
            // V10_TODO: More stuff, virtual override
            Player player = GetOwnerOfType<Player>();
            if (!Verify.IsNotNull(player)) return false;

            if (missionRef != PrototypeId.Invalid)
                player.MissionInteractRelease(this, missionRef);

            WorldEntity interactableObject = Game.EntityManager.GetEntity<WorldEntity>(entityId);
            if (interactableObject == null)
                return false;

            if (interactableObject is Transition transition)
                transition.UseTransition(player);

            return true;
        }

        #endregion

        #region Inventories

        public InventoryResult GetEquipmentInventoryAvailableStatus(PrototypeId invProtoRef)
        {
            AvatarPrototype avatarProto = AvatarPrototype;
            if (!Verify.IsNotNull(avatarProto)) return InventoryResult.UnknownFailure;

            foreach (AvatarEquipInventoryAssignmentPrototype equipInvEntryProto in avatarProto.EquipmentInventories)
            {
                if (!Verify.IsNotNull(equipInvEntryProto))
                    continue;

                if (equipInvEntryProto.Inventory == invProtoRef)
                {
                    /* V10_TODO: UnlockedAtCostumeRarity?
                    if (CharacterLevel < equipInvEntryProto.UnlocksAtCharacterLevel)
                        return InventoryResult.InvalidEquipmentInventoryNotUnlocked;
                    else
                        return InventoryResult.Success;
                    */

                    return InventoryResult.Success;
                }
            }

            return InventoryResult.UnknownFailure;
        }

        protected override bool InitInventories(bool populate)
        {
            bool success = base.InitInventories(populate);

            AvatarPrototype avatarProto = AvatarPrototype;
            if (!Verify.IsNotNull(avatarProto)) return false;

            if (Verify.IsTrue(avatarProto.EquipmentInventories.HasValue()))
            {
                foreach (AvatarEquipInventoryAssignmentPrototype equipInvAssignment in avatarProto.EquipmentInventories)
                    success &= Verify.IsTrue(AddInventory(equipInvAssignment.Inventory, populate ? equipInvAssignment.LootTable : PrototypeId.Invalid));
            }

            return success;
        }

        #endregion

        #region Event Handlers

        protected override void OnEnteredWorld(EntitySettings settings)
        {
            // V10_FIXME: Resurrect
            if (Properties[PropertyEnum.Health] == 0L)
                Properties[PropertyEnum.Health] = Properties[PropertyEnum.HealthMax];

            Player player = GetOwnerOfType<Player>();
            if (!Verify.IsNotNull(player)) return;

            Region region = Region;
            if (!Verify.IsNotNull(region)) return;

            base.OnEnteredWorld(settings);

            // Assign powers
            InitializePowers();

            // Update AOI of the owner player
            AreaOfInterest aoi = player.AOI;
            aoi.Update(RegionLocation.Position, true);
        }

        protected override void OnExitedWorld()
        {
            base.OnExitedWorld();
        }

        #endregion
    }
}
