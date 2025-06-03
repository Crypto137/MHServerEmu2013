using MHServerEmu.Games.Entities.Inventories;
using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.Loot;

namespace MHServerEmu.Games.GameData.Prototypes
{
    public class InventoryPrototype : Prototype
    {
        public short Capacity { get; protected set; }
        public PrototypeId[] EntityTypeFilter { get; protected set; }
        public bool ExitWorldOnAdd { get; protected set; }
        public bool ExitWorldOnRemove { get; protected set; }
        public bool PersistedToDatabase { get; protected set; }
        public bool OnPersonLocation { get; protected set; }
        public bool NotifyUI { get; protected set; }
        public short CollectionSortOrder { get; protected set; }
        public bool VisibleToOwner { get; protected set; }
        public bool VisibleToTrader { get; protected set; }
        public bool VisibleToParty { get; protected set; }
        public bool VisibleToProximity { get; protected set; }
        public bool AvatarTeam { get; protected set; }
        public InventoryConvenienceLabel ConvenienceLabel { get; protected set; }
        public bool PlaySoundOnAdd { get; protected set; }
        public bool CapacityUnlimited { get; protected set; }
        public bool VendorInvContentsCanBeBought { get; protected set; }
        public bool ContentsRecoverFromError { get; protected set; }
        public int DestroyContainedAfterSecs { get; protected set; }
        public InventoryEvent DestroyContainedOnEvent { get; protected set; }
        public InventoryCategory Category { get; protected set; }
        public OfferingInventoryUIDataPrototype OfferingInventoryUIData { get; protected set; }
        public bool LockedByDefault { get; protected set; }

        //---

        [DoNotCopy]
        public bool IsPlayerStashInventory { get => Category == InventoryCategory.PlayerStashAvatarSpecific || Category == InventoryCategory.PlayerStashCrafting || Category == InventoryCategory.PlayerStashGeneral; }

        [DoNotCopy]
        public bool IsEquipmentInventory { get => Category == InventoryCategory.AvatarEquipment; }

        [DoNotCopy]
        public bool IsPlayerGeneralInventory { get => Category == InventoryCategory.PlayerGeneral; }

        [DoNotCopy]
        public bool IsVisible { get => VisibleToOwner || VisibleToTrader || VisibleToParty || VisibleToProximity; }

        /// <summary>
        /// Returns <see langword="true"/> if entities that use the provided <see cref="EntityPrototype"/> are allowed to be stored in inventories that use this <see cref="InventoryPrototype"/>.
        /// </summary>
        public bool AllowEntity(EntityPrototype entityPrototype)
        {
            if (EntityTypeFilter == null || EntityTypeFilter.Length == 0) return false;

            foreach (PrototypeId entityTypeRef in EntityTypeFilter)
            {
                BlueprintId entityTypeBlueprintRef = GameDatabase.DataDirectory.GetPrototypeBlueprintDataRef(entityTypeRef);
                if (GameDatabase.DataDirectory.PrototypeIsChildOfBlueprint(entityPrototype.DataRef, entityTypeBlueprintRef))
                    return true;
            }

            return false;
        }

        public bool InventoryRequiresFlaggedVisibility()
        {
            // V10_TODO
            return false;
        }
    }

    public class PlayerStashInventoryPrototype : InventoryPrototype
    {
        public PrototypeId ForAvatar { get; protected set; }
        public AssetId IconPath { get; protected set; }
        public LocaleStringId DisplayName { get; protected set; }
    }

    public class EntityInventoryAssignmentPrototype : Prototype
    {
        public PrototypeId Inventory { get; protected set; }
        public PrototypeId LootTable { get; protected set; }
    }

    public class AvatarEquipInventoryAssignmentPrototype : EntityInventoryAssignmentPrototype
    {
        public EquipmentInvUISlot UISlot { get; protected set; }
        public PrototypeId UnlockedAtCostumeRarity { get; protected set; }
        public PrototypeId UIData { get; protected set; }
    }
}
