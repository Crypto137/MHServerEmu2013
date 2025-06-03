using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.Loot
{
    [AssetEnum((int)None)]
    [Flags]
    public enum LootContext
    {
        None                = 0,
        CashShop            = 1 << 0,
        Crafting            = 1 << 1,
        Drop                = 1 << 2,
        Initialization      = 1 << 3,
        Vendor              = 1 << 4,
        MissionReward       = 1 << 5,
        MysteryChest        = 1 << 6,
    }

    [AssetEnum((int)None)]
    public enum LootDropEventType
    {
        None,
        OnHealthBelowPctHit,
        OnHealthBelowPct,
        OnInteractedWith,
        OnKilled,
        OnKilledChampion,
        OnKilledElite,
        OnHit,
    }

    [AssetEnum]
    public enum CharacterFilterType
    {
        None,
        DropCurrentAvatarOnly,
        DropUnownedAvatarOnly,
    }

    [AssetEnum]
    public enum LootBindingType
    {
        None,
        Account,
        Avatar
    }

    [AssetEnum((int)Invalid)]
    public enum EquipmentInvUISlot
    {
        Invalid = -1,
        Costume,
        _01,
        _02,
        _03,
        _04,
        _05,
        _06,
        _07,
        _08,
        _09,
        _10,
        _11,
        _12,
        _13,
    }
}
