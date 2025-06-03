using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)None)]
    public enum HUDEntityOverheadIcon   // UI/Types/InteractIndicatorType.type
    {
        None,
        DiscoveryBestower,
        DiscoveryAdvancer,
        MissionBestowerDisabled,
        Vendor,
        VendorArmor,
        VendorCrafter,
        VendorWeapon,
        Stash,
        Transporter,
        MissionAdvancerDisabled,
        MissionBestower,
        MissionAdvancer,
        FlavorText,
        Healer,
        StoryWarp,
    }

    #endregion

    public class VendorXPBarTooltipPrototype : Prototype
    {
        public LocaleStringId NextRankTooltip { get; protected set; }
        public LocaleStringId ThisRankTooltip { get; protected set; }
    }

    public class VendorLootTableEntryPrototype : Prototype  // V10_NOTE: Older version of VendorInventoryEntryPrototype
    {
        public PrototypeId LootTable { get; protected set; }
        public int UseStartingAtVendorLevel { get; protected set; }
        public VendorXPBarTooltipPrototype VendorXPBarTooltip { get; protected set; }
    }

    public class VendorTypePrototype : Prototype
    {
        public PrototypeId AssociatedPlayerInventory { get; protected set; }
        public VendorLootTableEntryPrototype[] LootTables { get; protected set; }
        public float VendorEnergyPctPerRefresh { get; protected set; }
        public float VendorEnergyFullRechargeTimeMins { get; protected set; }
        public LocaleStringId VendorXPTooltip { get; protected set; }
        public LocaleStringId VendorRefreshTooltip { get; protected set; }
        public LocaleStringId VendorDonateTooltip { get; protected set; }
        public bool AllowActionDonate { get; protected set; }
        public bool AllowActionRefresh { get; protected set; }
        public bool IsCrafter { get; protected set; }
        public LocaleStringId TypeName { get; protected set; }
        public LocaleStringId VendorIconTooltip { get; protected set; }
        public HUDEntityOverheadIcon InteractIndicator { get; protected set; }
    }
}
