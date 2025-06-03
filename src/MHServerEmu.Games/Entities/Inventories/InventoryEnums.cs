using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.Entities.Inventories
{
    public enum InventoryResult
    {
        Invalid = -1,
        Success,
        NotAttempted,
        UnknownFailure,
        InventoryFull,
        NotRootOwner,
        IsRootOwner,
        InvalidExistingEntityAtDest,
        InvalidSourceEntity,
        SourceEntityAlreadyInAnInventory,
        InvalidStackEntity,
        StackTypeMismatch,
        NotStackable,
        InvalidSlotParam,
        SlotExceedsCapacity,
        SlotAlreadyOccupied,
        NotInInventory,
        NotFoundInThisInventory,
        InventoryHasNoOwner,
        InvalidSplitParam,
        SplitParamExceedsStackSize,
        NoAvailableInventory,
        PlayerOwnerNotFound,
        InvalidGame,
        ErrorCreatingNewSplitEntity,
        InvalidReceivingInventory,
        InvalidDestInvContainmentFilters,
        InvalidBound,
        InvalidUnboundItemNotAllowed,
        InvalidPropertyRestriction,
        InvalidCharacterRestriction,
        InvalidItemTypeForAvatar,
        InvalidItemTypeForAvatarCostume,
        InvalidNotAnItem,
        InvalidBagItemPreventsPlayerAdds,
        InvalidPlayerCannotMoveIntoThisInventory,
        InvalidPlayerCannotMoveOutOfThisInventory,
        InvalidNotInteractingWithCrafter,
        InvalidNotInteractingWithStash,
    };

    [AssetEnum((int)None)]
    public enum InventoryCategory   // Entity/Inventory/Category.type
    {
        None,
        AvatarEquipment,
        BagItem,
        PlayerAdmin,
        PlayerAvatars,
        PlayerCrafting,
        PlayerGeneral,
        PlayerStashAvatarSpecific,
        PlayerStashCrafting,
        PlayerStashGeneral,
        PlayerVendor,
    }

    [AssetEnum((int)Invalid)]
    public enum InventoryEvent
    {
        Invalid,
        RegionChange,
    }

    [AssetEnum((int)None)]
    public enum InventoryConvenienceLabel
    {
        None,
        AvatarInPlay,
        AvatarLibrary,
        Costume,
        CraftingRecipeLibrary,
        CraftingInProgress,
        CraftingResults,
        General,
        DEPRECATEDPlayerStash,
        Summoned,
        UIItems,
        DeliveryBox,
        ErrorRecovery
    }

    public enum InventoryMetaDataType : byte
    {
        Invalid,
        Parent,
        Item
    }
}
