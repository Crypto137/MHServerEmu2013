using Gazillion;
using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.Entities.Inventories;
using MHServerEmu.Games.Entities.PowerCollections;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Calligraphy;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Powers;
using MHServerEmu.Games.Properties;
using MHServerEmu.Games.Regions;
using MHServerEmu.Games.Social.Guilds;

namespace MHServerEmu.Games.Entities.Avatars
{
    public class Avatar : Agent
    {
        private const int PowerRankLocked = -1;

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

        public bool IsAtLevelCap { get => CharacterLevel >= GetAvatarLevelCap(); }

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

            Properties[PropertyEnum.EnduranceMax] = Properties[PropertyEnum.EnduranceBase];
            Properties[PropertyEnum.EnduranceMaxOther] = Properties[PropertyEnum.EnduranceMax];
            Properties[PropertyEnum.Endurance] = Properties[PropertyEnum.EnduranceMax];

            // Init AbilityKeyMapping
            if (settings.ArchiveData == null)
            {
                AbilityKeyMapping keyMapping = new();
                _abilityKeyMappings.Add(keyMapping);
                keyMapping.SlotDefaultAbilities(this);
            }

            return true;
        }

        public override bool ApplyInitialReplicationState(ref EntitySettings settings)
        {
            if (base.ApplyInitialReplicationState(ref settings) == false)
                return false;

            Player player = Game.EntityManager.GetEntity<Player>(settings.InventoryLocation.ContainerId);

            if (settings.ArchiveData != null)
            {
                if (player != null)
                {
                    TryLevelUp(player, true);
                    //RestoreMissionRewardProperties(player);
                }

                //ResetResources(false);
            }

            // Restore level state by running the level up code
            int level = CharacterLevel;
            OnLevelUp(level, level, false);

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

            UpdatePowerProgressionPowers(false);
        }

        #endregion

        #region Power Ranks

        private int GetMaxPossibleRankForPowerAtLevel(PrototypeId powerProtoRef, int level, bool includeTemp)
        {
            AvatarPrototype avatarProto = AvatarPrototype;
            if (!Verify.IsNotNull(avatarProto)) return 0;

            PowerProgressionEntryPrototype powerProgEntryProto = avatarProto.GetPowerProgressionEntryForPower(powerProtoRef);
            if (!Verify.IsNotNull(powerProgEntryProto, $"Trying to get the max possible rank at current level for the following power not in the avatar's PowerProgressionTable:\nAvatar: [{this}]\nPower: [{powerProtoRef.GetName()}]"))
                return 0;

            if (level < powerProgEntryProto.Level)
                return PowerRankLocked;

            PrototypeId[] prereqs = powerProgEntryProto.Prerequisites;
            if (prereqs.HasValue())
            {
                foreach (PrototypeId prereqProtoRef in prereqs)
                {
                    int prereqRank = Properties[PropertyEnum.AvatarPower, prereqProtoRef];
                    if (includeTemp)
                        prereqRank += Properties[PropertyEnum.AvatarPowerTemp, prereqProtoRef];

                    if (prereqRank <= 0)
                        return 0;
                }
            }

            Curve maxRankAtCharLevelCurve = powerProgEntryProto.MaxRankForPowerAtCharacterLevel.AsCurve();
            if (!Verify.IsNotNull(maxRankAtCharLevelCurve)) return 0;

            return maxRankAtCharLevelCurve.GetIntAt(level);
        }

        private int GetMaxPossibleRankForPowerAtCurrentLevel(PrototypeId powerProtoRef, bool includeTemp)
        {
            return GetMaxPossibleRankForPowerAtLevel(powerProtoRef, CharacterLevel, includeTemp);
        }

        #endregion

        #region Power Points

        public bool PowerPointAllocationClearTemporary()
        {
            using var propsToRemoveHandle = ListPool<PropertyId>.Get(out List<PropertyId> propsToRemove);

            foreach (var kvp in Properties.IteratePropertyRange(PropertyEnum.AvatarPowerTemp))
                propsToRemove.Add(kvp.Key);

            bool removed = false;

            foreach (PropertyId propId in propsToRemove)
                removed |= Properties.RemoveProperty(propId);

            return removed;
        }

        public void PowerPointAllocationCommit(NetMessagePowerPointAllocationCommit commitMessage)
        {
            Verify.IsTrue(PowerPointAllocationClearTemporary() == false, $"[{this}] already had a pending allocation");

            using var propsToAdjustHandle = DictionaryPool<PropertyId, PropertyValue>.Get(out Dictionary<PropertyId, PropertyValue> propsToAdjust);

            long pointsSpent = 0;

            for (int i = 0; i < commitMessage.AllocationsCount; i++)
            {
                NetStructPowerPointAllocation allocation = commitMessage.AllocationsList[i];
                PrototypeId powerProtoRef = (PrototypeId)allocation.PowerProtoId;
                int delta = (int)allocation.Delta;
                if (!Verify.IsTrue(delta > 0))
                    goto End;

                Properties[PropertyEnum.AvatarPowerTemp, powerProtoRef] = delta;
                pointsSpent += delta;
            }

            long powerPointsAvailable = Properties[PropertyEnum.AvatarPowerPoints];
            if (!Verify.IsTrue(pointsSpent <= powerPointsAvailable, $"Number of points spent [{pointsSpent}] exceeds the total available number [{powerPointsAvailable}] for [{this}]"))
                goto End;

            foreach (var kvp in Properties.IteratePropertyRange(PropertyEnum.AvatarPowerTemp))
            {
                Property.FromParam(kvp.Key, 0, out PrototypeId powerProtoRef);

                if (!Verify.IsTrue(ValidatePendingPowerPointAllocation(powerProtoRef)))
                    goto End;

                propsToAdjust[new(PropertyEnum.AvatarPower, powerProtoRef)] = kvp.Value;
            }

            foreach (var kvp in propsToAdjust)
                Properties.AdjustProperty((int)kvp.Value, kvp.Key);

            UpdatePowerProgressionPowers(false);
            UpdatePowerPointsUnspent();

        End:
            PowerPointAllocationClearTemporary();
        }

        private bool ValidatePendingPowerPointAllocation(PrototypeId powerProtoRef)
        {
            if (!Verify.IsTrue(powerProtoRef != PrototypeId.Invalid)) return false;
            if (!Verify.IsTrue(Power.IsUltimatePower(powerProtoRef) == false)) return false;
            if (!Verify.IsTrue(Properties.HasProperty(new PropertyId(PropertyEnum.AvatarPowerTemp, powerProtoRef)))) return false;

            int avatarPower = Properties[PropertyEnum.AvatarPower, powerProtoRef];
            int avatarPowerTemp = Properties[PropertyEnum.AvatarPowerTemp, powerProtoRef];
            int totalAfterAllocation = avatarPower + avatarPowerTemp;

            PropertyInfoPrototype avatarPowerPropInfoProto = GameDatabase.PropertyInfoTable.LookupPropertyInfo(PropertyEnum.AvatarPower).Prototype;
            if (!Verify.IsNotNull(avatarPowerPropInfoProto)) return false;

            if (!Verify.IsTrue(totalAfterAllocation <= avatarPowerPropInfoProto.Max)) return false;
            if (!Verify.IsTrue(totalAfterAllocation <= GetMaxPossibleRankForPowerAtCurrentLevel(powerProtoRef, true))) return false;

            return true;
        }

        private void UpdatePowerPointsUnspent()
        {
            // V10_NOTE: This whole thing needs to be investigated further for 1.10.
            // AvatarPowerPoints appears to be persistent, starting rank seems to come from StartingEquippedAbilities,
            // and AvatarPowerPoints seems to be required for powers to be unlocked for power point allocation.
            AdvancementGlobalsPrototype advancementGlobals = GameDatabase.AdvancementGlobalsPrototype;
            if (!Verify.IsNotNull(advancementGlobals)) return;

            int numPowerPoints = advancementGlobals.GetPowerPointsGrantedAtLevel(CharacterLevel);

            numPowerPoints += Properties[PropertyEnum.AvatarPowerPointsBonus];

            foreach (var kvp in Properties.IteratePropertyRange(PropertyEnum.AvatarPower))
                numPowerPoints -= kvp.Value;

            numPowerPoints = Math.Max(numPowerPoints, 0);
            Properties[PropertyEnum.AvatarPowerPoints] = numPowerPoints;
        }

        #endregion

        #region Power Progression

        private void UpdatePowerProgressionPowers(bool forceUnassign)
        {
            // V10_FIXME
            AvatarPrototype avatarProto = AvatarPrototype;
            if (!Verify.IsNotNull(avatarProto)) return;

            foreach (PowerProgressionTablePrototype powerProgTable in avatarProto.PowerProgressionTables)
            {
                foreach (PowerProgressionEntryPrototype powerProgEntry in powerProgTable.PowerProgressionEntries)
                {
                    PrototypeId powerProtoRef = powerProgEntry.PowerAssignment.Ability;
                    int level = CharacterLevel;
                    
                    if (level < powerProgEntry.Level)
                    {
                        Properties.RemoveProperty(new(PropertyEnum.AvatarPower, powerProtoRef));
                        continue;
                    }

                    if (Properties.HasProperty(new PropertyId(PropertyEnum.AvatarPower, powerProtoRef)) == false)
                        Properties[PropertyEnum.AvatarPower, powerProtoRef] = powerProgEntry.PowerAssignment.Rank;

                    int rankBase = Properties[PropertyEnum.AvatarPower, powerProtoRef];
                    Properties[PropertyEnum.PowerRankCurrentBest, powerProtoRef] = rankBase;
                }
            }

            foreach (var kvp in Properties.IteratePropertyRange(PropertyEnum.PowerRankCurrentBest))
            {
                Property.FromParam(kvp.Key, 0, out PrototypeId powerProtoRef);
                int rank = kvp.Value;

                if (rank > 0)
                {
                    if (PowerCollection.ContainsPower(powerProtoRef) == false)
                    {
                        PowerIndexProperties indexProps = new(kvp.Value, CharacterLevel);
                        AssignPower(powerProtoRef, indexProps);
                    }
                }
                else
                {
                    if (PowerCollection.ContainsPower(powerProtoRef))
                        UnassignPower(powerProtoRef);
                }
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

        #region Leveling

        public void InitializeLevel(int newLevel)
        {
            int oldLevel = CharacterLevel;
            CharacterLevel = newLevel;

            Properties[PropertyEnum.ExperienceLevelCurrent] = 0;
            Properties[PropertyEnum.ExperienceLevelMax] = GetLevelUpXPRequirement(newLevel);

            OnLevelUp(oldLevel, newLevel);
        }

        public long AwardXP(long amount)
        {
            // Only entities owned by players can earn experience
            Player owner = GetOwnerOfType<Player>();
            if (!Verify.IsNotNull(owner)) return 0;

            if (IsAtLevelCap == false)
            {
                Properties[PropertyEnum.ExperienceLevelCurrent] += amount;
                TryLevelUp(owner);
            }

            return amount;
        }

        public static int GetAvatarLevelCap()
        {
            AdvancementGlobalsPrototype advancementProto = GameDatabase.AdvancementGlobalsPrototype;
            return advancementProto != null ? advancementProto.GetAvatarLevelCap() : 0;
        }

        public long GetLevelUpXPRequirement(int level)
        {
            AdvancementGlobalsPrototype advancementProto = GameDatabase.AdvancementGlobalsPrototype;
            if (!Verify.IsNotNull(advancementProto)) return 0;

            return advancementProto.GetAvatarLevelUpXPRequirement(level);
        }

        public int TryLevelUp(Player owner, bool isInitializing = false)
        {
            int oldLevel = CharacterLevel;
            int newLevel = oldLevel;

            long xp = Properties[PropertyEnum.ExperienceLevelCurrent];
            long xpNeeded = Properties[PropertyEnum.ExperienceLevelMax];

            int levelCap = GetAvatarLevelCap();
            while (newLevel < levelCap && xp >= xpNeeded)
            {
                xp -= xpNeeded;
                newLevel++;
                xpNeeded = GetLevelUpXPRequirement(newLevel);
            }

            int levelDelta = newLevel - oldLevel;
            if (levelDelta != 0)
            {
                CharacterLevel = newLevel;
                Properties[PropertyEnum.ExperienceLevelCurrent] = xp;
                Properties[PropertyEnum.ExperienceLevelMax] = xpNeeded;
            }

            if (isInitializing || levelDelta != 0)
                OnLevelUp(oldLevel, newLevel);

            Properties[PropertyEnum.CharacterLevelFromArea] = newLevel;

            return levelDelta;
        }

        private void OnLevelUp(int oldLevel, int newLevel, bool restoreHealthAndEndurance = true)
        {
            AvatarPrototype avatarProto = AvatarPrototype;
            if (!Verify.IsNotNull(avatarProto)) return;

            // V10_TODO: update stats

            // Notify clients
            SendLevelUpMessage();

            if (IsInWorld)
                UpdatePowerProgressionPowers(false);

            UpdatePowerPointsUnspent();

            // Restore health if needed
            if (restoreHealthAndEndurance && IsDead == false)
                Properties[PropertyEnum.Health] = Properties[PropertyEnum.HealthMax];
        }

        private void SendLevelUpMessage()
        {
            using var interestedClientListHandle = ListPool<PlayerConnection>.Get(out List<PlayerConnection> interestedClientList);
            PlayerConnectionManager networkManager = Game.NetworkManager;
            if (networkManager.GetInterestedClients(interestedClientList, this, AOINetworkPolicyValues.AOIChannelOwner | AOINetworkPolicyValues.AOIChannelProximity))
            {
                var levelUpMessage = NetMessageLevelUp.CreateBuilder().SetEntityID(Id).Build();
                networkManager.SendMessageToMultiple(interestedClientList, levelUpMessage);
            }
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
