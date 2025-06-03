using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.Loot;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)None)]
    public enum ItemActionType
    {
        None,
        AssignPower,
        DestroySelf,
        ReplaceSelfWithItem,
        ReplaceSelfWithLootTableOutput,
        Respec,
        UsePower,
        GuildsUnlock,
    }

    [AssetEnum((int)None)]
    public enum ItemEventType
    {
        None,
        OnEquip,
        OnUse,
        OnUsePowerActivated,
    }

    [AssetEnum((int)PickWeight)]
    public enum PickMethod
    {
        PickWeight,
        PickAll,
    }

    [AssetEnum]
    public enum CharacterTokenType  // Entity/Items/CharacterTokens/TokenType.type
    {
        None = 0,
        UnlockCharacterOnly = 1,
        UnlockCharOrUpgradeUlt = 2,
        UpgradeUltimateOnly = 4,
    }

    #endregion

    public class ItemPrototype : WorldEntityPrototype
    {
        public int ExpirationTimeMS { get; protected set; }
        public bool IsMissionItem { get; protected set; }
        public bool IsUsable { get; protected set; }
        public int BaseValue { get; protected set; }
        public bool CanBeSoldToVendor { get; protected set; }
        public CurveId ValueCurveLevel { get; protected set; }
        public CurveId ValueCurveNumAffixes { get; protected set; }
        public CurveId ValueCurveTier { get; protected set; }
        public int MaxVisiblePrefixes { get; protected set; }
        public int MaxVisibleSuffixes { get; protected set; }
        public TooltipTranslationEntryPrototype[] TooltipBaseBonuses { get; protected set; }
        public LocaleStringId TooltipDescription { get; protected set; }
        public LocaleStringId TooltipFlavorText { get; protected set; }
        public PrototypeId TooltipTemplate { get; protected set; }
        public ItemStackSettingsPrototype StackSettings { get; protected set; }
        public bool AlwaysDisplayAsUsable { get; protected set; }
        public PrototypeId[] TooltipEquipRestrictions { get; protected set; }
        public AffixEntryPrototype[] AffixesBuiltIn { get; protected set; }
        public PropertyPickInRangeEntryPrototype[] PropertiesBuiltIn { get; protected set; }
        [Mixin]
        public ProductPrototype Product { get; protected set; }
        public LocaleStringId ItemCategory { get; protected set; }
        public LocaleStringId ItemSubcategory { get; protected set; }
        public bool IsAvatarRestricted { get; protected set; }
        public DropRestrictionPrototype[] LootDropRestrictions { get; protected set; }
        public ItemBindingSettingsPrototype BindingSettings { get; protected set; }
        public AffixLimitsPrototype[] AffixLimits { get; protected set; }
        public PrototypeId TextStyleOverride { get; protected set; }
        public ItemAbilitySettingsPrototype AbilitySettings { get; protected set; }
        public AssetId StoreIconPath { get; protected set; }
        public bool ClonedWhenPurchasedFromVendor { get; protected set; }
        public TriggeredItemEventActionSetPrototype ActionsTriggeredOnItemEvent { get; protected set; }
        public bool ConfirmOnDonate { get; protected set; }
#if !BUILD_1_10_0_69
        public bool CanBeDestroyed { get; protected set; }
        public bool ConfirmPurchase { get; protected set; }
        public ItemCostPrototype Cost { get; protected set; }
#endif
    }

    public class ItemAbilitySettingsPrototype : Prototype
    {
        public AbilitySlotRestrictionPrototype AbilitySlotRestriction { get; protected set; }
        public bool OnlySlottableWhileEquipped { get; protected set; }
    }

    public class ItemBindingSettingsEntryPrototype : Prototype
    {
        public bool BindsToAccountOnPickup { get; protected set; }
        public bool BindsToAvatarOnEquip { get; protected set; }
        public PrototypeId RarityFilter { get; protected set; }
    }

    public class ItemBindingSettingsPrototype : Prototype
    {
        public ItemBindingSettingsEntryPrototype DefaultSettings { get; protected set; }
        public ItemBindingSettingsEntryPrototype[] PerRaritySettings { get; protected set; }
    }

    public class ItemStackSettingsPrototype : Prototype
    {
        public int ItemLevelOverride { get; protected set; }
        public int MaxStacks { get; protected set; }
        public int RequiredCharLevelOverride { get; protected set; }
    }

    public class ItemEventActionPrototype : Prototype   // V10_NOTE: Older version of ItemActionBasePrototype
    {
        public int Weight { get; protected set; }
    }

    public class TriggeredItemEventActionPrototype : ItemEventActionPrototype       // V10_NOTE: Older version of various item actions
    {
        public ItemActionType EventAction { get; protected set; }
        public ItemEventType ItemEvent { get; protected set; }
        public PrototypeId Item { get; protected set; }
        public PrototypeId Power { get; protected set; }
        public LootTablePrototype LootTable { get; protected set; }
    }

    public class TriggeredItemEventActionSetPrototype : ItemEventActionPrototype    // V10_NOTE: Older version of ItemActionSetPrototype
    {
        public ItemEventActionPrototype[] Choices { get; protected set; }
        public PickMethod PickMethod { get; protected set; }
    }

    public class AffixLimitsPrototype : Prototype
    {
        public LootContext[] AllowedContexts { get; protected set; }
        public PrototypeId ItemRarity { get; protected set; }
        public short MaxAffixes { get; protected set; }
        public short MaxPrefixes { get; protected set; }
        public short MaxSuffixes { get; protected set; }
        public short MinAffixes { get; protected set; }
        public short MinPrefixes { get; protected set; }
        public short MinSuffixes { get; protected set; }
#if !BUILD_1_10_0_69
        public short NumUltimates { get; protected set; }
#endif
    }

    public class ArmorPrototype : ItemPrototype
    {
    }

    public class ArtifactPrototype : ItemPrototype
    {
    }

    public class BagItemPrototype : ItemPrototype
    {
        public bool AllowsPlayerAdds { get; protected set; }
    }

    public class CharacterTokenPrototype : ItemPrototype
    {
        public PrototypeId Character { get; protected set; }
#if !BUILD_1_10_0_69
        public CharacterTokenType TokenType { get; protected set; }
#endif
    }

    public class CostumePrototype : ItemPrototype
    {
        public AssetId CostumeUnrealClass { get; protected set; }
        public AssetId FullBodyIconPath { get; protected set; }
        public PrototypeId UsableBy { get; protected set; }
        public AssetId PortraitIconPath { get; protected set; }
        public AssetId FullBodyIconPathDisabled { get; protected set; }
        public AssetId PartyPortraitIconPath { get; protected set; }
    }

    public class MedalPrototype : ItemPrototype
    {
    }

    public class PermaBuffPrototype : Prototype
    {
        public EvalPrototype EvalAvatarProperties { get; protected set; }
    }

    public class ProductPrototype : Prototype
    {
        public bool ForSale { get; protected set; }
    }
}
