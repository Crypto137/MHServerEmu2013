using System.Runtime.CompilerServices;
using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Entities.Avatars;
using MHServerEmu.Games.Entities.Inventories;
using MHServerEmu.Games.Events;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Properties;
using MHServerEmu.Games.Properties.Evals;

namespace MHServerEmu.Games.Entities
{
    #region Enums

    // V10_NOTE: It seems entity flags are an optimization feature that didn't exist back in 1.10.
    // Because many flags appear to be fast lookups for types/properties, we may be able to get away with keeping them.
    [Flags]
    public enum EntityFlags : ulong
    {
        Dormant                         = 1ul << 0,
        IsDead                          = 1ul << 1,
        HasMovementPreventionStatus     = 1ul << 2,
        AIMasterAvatar                  = 1ul << 3,
        Confused                        = 1ul << 4,
        Mesmerized                      = 1ul << 5,
        MissionXEncounterHostilityOk    = 1ul << 6,
        IgnoreMissionOwnerForTargeting  = 1ul << 7,
        IsSimulated                     = 1ul << 8,
        Untargetable                    = 1ul << 9,
        Unaffectable                    = 1ul << 10,
        IsNeverAffectedByPowers         = 1ul << 11,
        AITargetableOverride            = 1ul << 12,
        AIControlPowerLock              = 1ul << 13,
        Knockback                       = 1ul << 14,
        Knockdown                       = 1ul << 15,
        Knockup                         = 1ul << 16,
        Immobilized                     = 1ul << 17,
        ImmobilizedParam                = 1ul << 18,
        ImmobilizedByHitReact           = 1ul << 19,
        SystemImmobilized               = 1ul << 20,
        Stunned                         = 1ul << 21,
        StunnedByHitReact               = 1ul << 22,
        NPCAmbientLock                  = 1ul << 23,
        PowerLock                       = 1ul << 24,
        NoCollide                       = 1ul << 25,
        HasNoCollideException           = 1ul << 26,
        Intangible                      = 1ul << 27,
        PowerUserOverrideId             = 1ul << 28,
        MissileOwnedByPlayer            = 1ul << 29,
        HasMissionPrototype             = 1ul << 30,
        Flag31                          = 1ul << 31,
        IsPopulation                    = 1ul << 32,
        SummonDecremented               = 1ul << 33,
        AttachedToEntityId              = 1ul << 34,
        IsHotspot                       = 1ul << 35,
        IsCollidableHotspot             = 1ul << 36,
        IsReflectingHotspot             = 1ul << 37,
        ImmuneToPower                   = 1ul << 38,
        ClusterPrototype                = 1ul << 39,
        EncounterResource               = 1ul << 40,
        IgnoreNavi                      = 1ul << 41,
        TutorialImmobilized             = 1ul << 42,
        TutorialInvulnerable            = 1ul << 43,
        TutorialPowerLock               = 1ul << 44,
    }

    [Flags]
    public enum EntityStatus
    {                                       // Reference method
        PendingDestroy          = 1 << 0,
        Destroyed               = 1 << 1,
        ToTransform             = 1 << 2,
        InGame                  = 1 << 3,
        DisableDBOps            = 1 << 4,   // EntityManager::CreateEntity()
        Status5                 = 1 << 5,
        SkipItemBindingCheck    = 1 << 6,   // CItem::CanChangeInventoryLocation()
        HasArchiveData          = 1 << 7,
        ClientOnly              = 1 << 8,   // CEntity::ExitGame()
        EnteringWorld           = 1 << 9,   // WorldEntity::EnterWorld()
        ExitingWorld            = 1 << 10,  // WorldEntity::EnterWorld()
        DeferAdapterChanges     = 1 << 11   // CWorldEntity::OnEnteredWorld()
    }

    #endregion

    public class Entity : IArchiveMessageDispatcher, ISerialize
    {
        public const ulong InvalidId = 0;

        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly EventGroup _pendingEvents = new();

        private EntityFlags _flags;

        public ulong Id { get; set; }
        public ulong DatabaseUniqueId { get; private set; }

        public Game Game { get; }
        public EntityStatus Status { get; private set; }
        public bool IsInGame { get => TestStatus(EntityStatus.InGame); }
        public bool IsDestroyed { get => TestStatus(EntityStatus.Destroyed); }

        public ReplicatedPropertyCollection Properties { get; } = new();

        public EntityPrototype Prototype { get; private set; }
        public PrototypeId PrototypeDataRef { get; private set; }
        public string PrototypeName { get => GameDatabase.GetFormattedPrototypeName(PrototypeDataRef); }

        public virtual AOINetworkPolicyValues CompatibleReplicationChannels { get => Prototype.RepNetwork; }
        public InterestReferences InterestReferences { get; } = new();
        public AOINetworkPolicyValues InterestedPoliciesUnion { get; private set; }
        public bool CanSendArchiveMessages { get => IsInGame; }

        public InventoryCollection InventoryCollection { get; } = new();
        public InventoryLocation InventoryLocation { get; private set; } = new();
        public ulong OwnerId { get => InventoryLocation.ContainerId; }
        public bool IsRootOwner { get => OwnerId == 0; }

        #region Flag Properties

        public bool IsDead { get => _flags.HasFlag(EntityFlags.IsDead); }
        public bool IsNeverAffectedByPowers { get => _flags.HasFlag(EntityFlags.IsNeverAffectedByPowers); }
        public bool IsHotspot { get => _flags.HasFlag(EntityFlags.IsHotspot); }
        public bool IsCollidableHotspot { get => _flags.HasFlag(EntityFlags.IsCollidableHotspot); }
        public bool IsReflectingHotspot { get => _flags.HasFlag(EntityFlags.IsReflectingHotspot); }

        #endregion

        #region Property Properties (lol)

        public int CharacterLevel { get => Properties[PropertyEnum.CharacterLevel]; }
        public int CharacterLevelFromArea { get => Properties[PropertyEnum.CharacterLevelFromArea]; }   // CombatLevel?

        public PrototypeId State { get => Properties[PropertyEnum.EntityState]; }

        public int CurrentStackSize { get => Properties[PropertyEnum.InventoryStackCount]; }
        public int MaxStackSize { get => Properties[PropertyEnum.InventoryStackSizeMax]; }

        #endregion

        public Entity(Game game)
        {
            Game = game;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}={Id}, {nameof(Prototype)}={Prototype}";
        }

        public virtual bool PreInitialize(EntitySettings settings)
        {
            return true;
        }

        public virtual bool Initialize(EntitySettings settings)
        {
            if (Game == null) return Logger.WarnReturn(false, "Initialize(): Game == null");

            Id = settings.Id;
            if (Id == InvalidId) return Logger.WarnReturn(false, "Initialize(): Id == Entity.InvalidId");

            DatabaseUniqueId = settings.DbGuid;

            // Set prototype reference
            PrototypeDataRef = settings.EntityRef;
            if (PrototypeDataRef == PrototypeId.Invalid) return Logger.WarnReturn(false, "Initialize(): Invalid PrototypeDataRef");

            Prototype = PrototypeDataRef.As<EntityPrototype>();
            if (Prototype == null) return Logger.WarnReturn(false, "Initialize(): Prototype == null");

            BindReplicatedFields();

            if (Prototype.Properties != null)
                Properties.FlattenCopyFrom(Prototype.Properties, true);

            if (settings.Properties != null)
                Properties.FlattenCopyFrom(settings.Properties, false);

            // Inventories
            InventoryCollection.Initialize(this);
            InitInventories(settings.OptionFlags.HasFlag(EntitySettingsOptionFlags.PopulateInventories));

            return true;
        }

        public virtual void OnPostInit(EntitySettings settings)
        {
            if (settings.ArchiveData == null)
            {
                // Initialize health for new entities
                if (Properties.HasProperty(PropertyEnum.HealthBase) && Properties.HasProperty(PropertyEnum.Health) == false)
                    Properties[PropertyEnum.Health] = Properties[PropertyEnum.HealthMax];
            }

            if (Prototype.EvalOnCreate.HasValue())
            {
                using EvalContextData evalContext = ObjectPoolManager.Instance.Get<EvalContextData>();
                evalContext.Game = Game;
                evalContext.SetVar_PropertyCollectionPtr(EvalContext.Default, Properties);

                foreach (EvalPrototype evalProto in Prototype.EvalOnCreate)
                {
                    if (Eval.RunBool(evalProto, evalContext) == false)
                        Logger.Warn($"OnPostInit(): Failed to run eval {evalProto.ExpressionString()}");
                }
            }
        }

        public virtual bool Serialize(Archive archive)
        {
            return Properties.Serialize(archive);
        }

        public virtual void OnUnpackComplete(Archive archive)
        {
        }

        public virtual bool ApplyInitialReplicationState(EntitySettings settings)
        {
            return true;
        }

        protected virtual void BindReplicatedFields()
        {
            Properties.Bind(this, AOINetworkPolicyValues.AllChannels);
        }

        protected virtual void UnbindReplicatedFields()
        {
            Properties.Unbind();
        }

        public virtual void EnterGame(EntitySettings settings = null)
        {
            if (IsInGame) return;

            SetStatus(EntityStatus.InGame, true);
            UpdateInterestPolicies(true);

            // Put all inventory entities into the game as well
            EntityManager entityManager = Game.EntityManager;

            foreach (Inventory inventory in new InventoryIterator(this))
            {
                foreach (var entry in inventory)
                {
                    Entity containedEntity = entityManager.GetEntity<Entity>(entry.Id);
                    if (containedEntity != null) containedEntity.EnterGame();
                }
            }
        }

        public virtual void ExitGame()
        {
            SetStatus(EntityStatus.InGame, false);
            UpdateInterestPolicies(false);

            // Remove contained entities
            EntityManager entityManager = Game.EntityManager;

            foreach (Inventory inventory in new InventoryIterator(this))
            {
                foreach (var entry in inventory)
                {
                    Entity containedEntity = entityManager.GetEntity<Entity>(entry.Id);
                    if (containedEntity != null) containedEntity.ExitGame();
                }
            }
        }

        // NOTE: TestStatus and SetStatus can be potentially replaced with an indexer property

        public bool TestStatus(EntityStatus status)
        {
            return Status.HasFlag(status);
        }

        public void SetStatus(EntityStatus status, bool value)
        {
            if (value) Status |= status;
            else Status &= ~status;
        }

        protected void SetFlag(EntityFlags flag, bool value)
        {
            if (value)
                _flags |= flag;
            else
                _flags &= ~flag;
        }

        public virtual void Destroy()
        {
            /* V10_TODO
            CancelScheduledLifespanExpireEvent();
            CancelDestroyEvent();
            */
            Game?.EntityManager?.DestroyEntity(this);
        }

        public bool DestroyContained()
        {
            if (Game == null) return Logger.WarnReturn(false, "DestroyContained(): Game == null");

            foreach (Inventory inventory in InventoryCollection)
                inventory.DestroyContained();

            return true;
        }

        #region AOI

        public virtual void UpdateInterestPolicies(bool updateForAllPlayers, EntitySettings settings = null)
        {
            if (updateForAllPlayers)
            {
                // Update interest policies for all players in the game (slow).
                foreach (Player player in new PlayerIterator(Game))
                    player.AOI.ConsiderEntity(this, settings);
            }
            else
            {
                // Update only players who are already interested in this entity.
                // This is what should be used to remove entities if possible.
                EntityManager entityManager = Game.EntityManager;

                foreach (ulong playerId in InterestReferences)
                {
                    Player player = entityManager.GetEntity<Player>(playerId);
                    player?.AOI.ConsiderEntity(this, settings);
                }
            }
        }

        public bool GetInterestedClients(List<PlayerConnection> interestedClientList, AOINetworkPolicyValues interestPolicies)
        {
            return Game.NetworkManager.GetInterestedClients(interestedClientList, this, interestPolicies);
        }

        #endregion

        #region Event Handlers

        public virtual void OnSelfAddedToOtherInventory()
        {
        }

        public virtual void OnSelfRemovedFromOtherInventory(InventoryLocation prevInvLoc)
        {
        }

        public virtual void OnOtherEntityAddedToMyInventory(Entity entity, InventoryLocation invLoc, bool unpackedArchivedEntity)
        {
        }

        public virtual void OnOtherEntityRemovedFromMyInventory(Entity entity, InventoryLocation invLoc)
        {
        }

        public virtual void OnDetachedFromDestroyedContainer()
        {
        }

        public virtual void OnDeallocate()
        {
            Game.GameEventScheduler.CancelAllEvents(_pendingEvents);
            UnbindReplicatedFields();
            Properties.RemoveAllWatchers();
        }

        public virtual void OnChangePlayerAOI(Player player, InterestTrackOperation operation,
            AOINetworkPolicyValues newInterestPolicies, AOINetworkPolicyValues previousInterestPolicies,
            AOINetworkPolicyValues archiveInterestPolicies = AOINetworkPolicyValues.AOIChannelNone)
        {
            Properties.OnEntityChangePlayerAOI(player, operation, newInterestPolicies, previousInterestPolicies, archiveInterestPolicies);

            AOINetworkPolicyValues gainedPolicies = newInterestPolicies & ~previousInterestPolicies;
            AOINetworkPolicyValues lostPolicies = previousInterestPolicies & ~newInterestPolicies;
            InterestReferences.Track(this, player.Id, operation, gainedPolicies, lostPolicies);

            // Cache current policies for map location updates
            InterestedPoliciesUnion = InterestReferences.GetInterestedPoliciesUnion();
        }

        public virtual void OnPostAOIAddOrRemove(Player player, InterestTrackOperation operation,
            AOINetworkPolicyValues newInterestPolicies, AOINetworkPolicyValues previousInterestPolicies)
        {

        }

        public virtual void OnLifespanExpired()
        {
            Destroy();
        }

        #endregion

        #region Inventory Management

        public ref RegionLocation GetOwnerLocation(out bool found)
        {
            Entity owner = GetOwner();
            while (owner != null)
            {
                if (owner is WorldEntity worldEntity)
                {
                    if (worldEntity.IsInWorld)
                    {
                        found = true;
                        return ref worldEntity.RegionLocation;
                    }
                }
                else if (owner is Player player)
                {
                    Avatar avatar = player.CurrentAvatar;
                    if (avatar != null && avatar.IsInWorld)
                    {
                        found = true;
                        return ref avatar.RegionLocation;
                    }
                }

                owner = owner.GetOwner();
            }

            found = false;
            return ref Unsafe.NullRef<RegionLocation>();
        }

        public Entity GetOwner()
        {
            return Game.EntityManager.GetEntity<Entity>(OwnerId);
        }

        public T GetOwnerOfType<T>() where T : Entity
        {
            Entity owner = GetOwner();
            while (owner != null)
            {
                if (owner is T currentCast)
                    return currentCast;
                owner = owner.GetOwner();
            }
            return null;
        }

        public T GetSelfOrOwnerOfType<T>() where T : Entity
        {
            if (this is T typedOwner) return typedOwner;
            return GetOwnerOfType<T>();
        }

        /// <summary>
        /// Returns <see langword="true"/> if the specified entity id matches this <see cref="Entity"/> or one of its owners.
        /// </summary>
        public bool IsOwnedBy(ulong entityId)
        {
            // NOTE: If the provided entityId matches this entity, this check will
            // return true, even though GetOwner() would return null. In other words,
            // an entity without an owner is owned by itself. This is expected behavior,
            // because Player entities own themselves.

            Entity potentialOwner = this;

            while (potentialOwner != null)
            {
                if (potentialOwner.Id == entityId)
                    return true;

                potentialOwner = potentialOwner.GetOwner();
            }

            return false;
        }

        public bool Owns(ulong entityId)
        {
            Entity entity = Game.EntityManager.GetEntity<Entity>(entityId);
            return Owns(entity);
        }

        public bool Owns(Entity entity)
        {
            if (entity == null) return Logger.WarnReturn(false, "Owns(): entity == null");
            return entity.IsOwnedBy(Id);
        }

        public Entity GetRootOwner()
        {
            Entity owner = this;
            while (owner != null)
            {
                if (owner.IsRootOwner) return owner;
                owner = owner.GetOwner();
            }
            return this;
        }

        public Inventory GetInventory(InventoryConvenienceLabel convenienceLabel)
        {
            foreach (Inventory inventory in InventoryCollection)
            {
                if (inventory.ConvenienceLabel == convenienceLabel)
                    return inventory;
            }

            return null;
        }

        public Inventory GetInventoryByRef(PrototypeId invProtoRef)
        {
            return InventoryCollection.GetInventoryByRef(invProtoRef);
        }

        public Inventory GetOwnerInventory()
        {
            Entity container = Game.EntityManager.GetEntity<Entity>(InventoryLocation.ContainerId);
            if (container == null) return null;
            return container.GetInventoryByRef(InventoryLocation.InventoryRef);
        }

        public InventoryResult CanChangeInventoryLocation(Inventory destInventory)
        {
            return CanChangeInventoryLocation(destInventory, out _);
        }

        public InventoryResult CanChangeInventoryLocation(Inventory destInventory, out PropertyEnum propertyRestriction)
        {
            propertyRestriction = PropertyEnum.Invalid;

            InventoryResult result = destInventory.PassesContainmentFilter(PrototypeDataRef);
            if (result != InventoryResult.Success)
                return result;

            return destInventory.PassesEquipmentRestrictions(this, out propertyRestriction);
        }

        public InventoryResult ChangeInventoryLocation(Inventory destination, uint destSlot = Inventory.InvalidSlot)
        {
            ulong? stackEntityId = null;
            return ChangeInventoryLocation(destination, destSlot, ref stackEntityId, true);
        }

        public InventoryResult ChangeInventoryLocation(Inventory destInventory, uint destSlot, ref ulong? stackEntityId, bool allowStacking)
        {
            allowStacking &= IsInGame;

            // If we have a valid destination, it means we are adding or moving, so we need to verify that this entity matches the destination inventory
            if (destInventory != null)
            {
                InventoryResult destInventoryResult = CanChangeInventoryLocation(destInventory);
                if (destInventoryResult != InventoryResult.Success) return Logger.WarnReturn(destInventoryResult,
                    $"ChangeInventoryLocation(): result=[{destInventoryResult}] allowStacking=[{allowStacking}] destSlot=[{destSlot}] destInventory=[{destInventory}] entity=[{Id}]");
            }

            return Inventory.ChangeEntityInventoryLocation(this, destInventory, destSlot, ref stackEntityId, allowStacking);
        }

        public bool ValidateInventorySlot(Inventory inventory, uint slot)
        {
            // this literally does nothing
            return true;
        }

        public bool CanStack()
        {
            if (MaxStackSize < 2) return false;
            if (CurrentStackSize > MaxStackSize) Logger.WarnReturn(false, "CanStack(): CurrentStackSize > MaxStackSize");
            if (CurrentStackSize == MaxStackSize) return false;
            return true;
        }

        public virtual bool IsAutoStackedWhenAddedToInventory()
        {
            return CanStack();
        }

        public bool CanStackOnto(Entity other, bool isAdding = false)
        {
            if (CanStack() == false || other.CanStack() == false) return false;
            if (PrototypeDataRef != other.PrototypeDataRef) return false;
            if (isAdding && CurrentStackSize + other.CurrentStackSize > other.MaxStackSize) return false;
            return true;
        }

        protected virtual bool InitInventories(bool populateInventories)
        {
            bool success = true;

            EntityPrototype entityPrototype = Prototype;
            if (entityPrototype == null) return Logger.WarnReturn(false, "InitInventories(): entityPrototype == null");

            foreach (EntityInventoryAssignmentPrototype invAssignmentProto in entityPrototype.Inventories)
            {
                if (AddInventory(invAssignmentProto.Inventory, invAssignmentProto.LootTable) == false)
                {
                    Logger.Warn($"InitInventories(): Failed to add inventory, invProtoRef={GameDatabase.GetPrototypeName(invAssignmentProto.Inventory)}");
                    success = false;
                }
            }

            return success;
        }

        protected bool AddInventory(PrototypeId invProtoRef, PrototypeId lootTableRef = PrototypeId.Invalid)
        {
            // lootTableRef seems to be unused
            return InventoryCollection.CreateAndAddInventory(invProtoRef);
        }

        #endregion
    }
}
