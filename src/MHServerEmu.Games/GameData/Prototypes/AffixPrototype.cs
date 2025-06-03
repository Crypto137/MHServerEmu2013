using MHServerEmu.Games.GameData.Calligraphy;
using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.Loot;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)None)]
    public enum AffixPosition
    {
        None,
        Prefix,
        Suffix,
        Visual,
        Ultimate,
        Metadata,
    }

    [AssetEnum((int)Fail)]
    public enum DuplicateHandlingBehavior
    {
        Fail,
        Ignore,
        Overwrite,
        Append,
    }

    [AssetEnum((int)Default)]
    public enum HealthBarType
    {
        Default = 0,
        EliteMinion = 1,
        MiniBoss = 2,
        Boss = 3,
        None = 4,
    }

    #endregion

    public class AffixPrototype : Prototype
    {
        public AssetId Position { get; protected set; }
        public PrototypePropertyCollection Properties { get; protected set; }
        public LocaleStringId DisplayNameText { get; protected set; }
        public int Weight { get; protected set; }
        public PropertyPickInRangeEntryPrototype[] PropertyEntries { get; protected set; }
        public AssetId[] Keywords { get; protected set; }
        public DropRestrictionPrototype[] DropRestrictions { get; protected set; }
        public DuplicateHandlingBehavior DuplicateHandlingBehavior { get; protected set; }
    }

    public class AffixPowerModifierPrototype : AffixPrototype
    {
        public bool IsForSinglePowerOnly { get; protected set; }
        public EvalPrototype PowerBoostMax { get; protected set; }
        public EvalPrototype PowerGrantRankMax { get; protected set; }
        public PrototypeId PowerKeywordFilter { get; protected set; }
        public EvalPrototype PowerUnlockLevelMax { get; protected set; }
        public EvalPrototype PowerUnlockLevelMin { get; protected set; }
        public EvalPrototype PowerBoostMin { get; protected set; }
        public EvalPrototype PowerGrantRankMin { get; protected set; }
        public PrototypeId PowerProgTableTabRef { get; protected set; }
    }

    public class AffixEntryPrototype : Prototype
    {
        public PrototypeId Affix { get; protected set; }
        public PrototypeId Power { get; protected set; }
        public PrototypeId Avatar { get; protected set; }
    }

    public class AffixDisplaySlotPrototype : Prototype
    {
        public AssetId[] AffixKeywords { get; protected set; }
        public LocaleStringId DisplayText { get; protected set; }
    }

    public class ModPrototype : Prototype
    {
        public LocaleStringId TooltipTitle { get; protected set; }
        public AssetId UIIcon { get; protected set; }
        public LocaleStringId UITooltip { get; protected set; }
        public PrototypePropertyCollection Properties { get; protected set; }
        public PrototypeId[] PassivePowers { get; protected set; }
        public PrototypeId Type { get; protected set; }
        public int RanksMax { get; protected set; }
        public CurveId RankCostCurve { get; protected set; }
        public long XCoord { get; protected set; }
        public long YCoord { get; protected set; }
        public ModDisableByBasePrototype[] DisableBy { get; protected set; }
        public PrototypeId TooltipTemplate { get; protected set; }
    }

    public class ModTypePrototype : Prototype
    {
        public PrototypeId AggregateProperty { get; protected set; }
        public PrototypeId TempProperty { get; protected set; }
        public PrototypeId BaseProperty { get; protected set; }
        public PrototypeId CurrencyIndexProperty { get; protected set; }
        public CurveId CurrencyCurve { get; protected set; }
        public bool UseCurrencyIndexAsValue { get; protected set; }
    }

    public class ModGlobalsPrototype : Prototype
    {
        public PrototypeId RankModType { get; protected set; }
        public PrototypeId SkillModType { get; protected set; }
        public PrototypeId EnemyBoostModType { get; protected set; }
        public PrototypeId PvPUpgradeModType { get; protected set; }
        public PrototypeId TalentModType { get; protected set; }
    }

    public class SkillPrototype : ModPrototype
    {
        public CurveId DamageBonusByRank { get; protected set; }
    }

    public class TalentSetPrototype : Prototype
    {
        public LocaleStringId UITitle { get; protected set; }
        public PrototypeId[] Talents { get; protected set; }
    }

    public class TalentPrototype : ModPrototype
    {
    }

    public class RankPrototype : ModPrototype
    {
        public int Rank { get; protected set; }
        public HealthBarType HealthBarType { get; protected set; }
        public LootRollModifierPrototype[] LootModifiers { get; protected set; }
        public LootDropEventType LootTableParam { get; protected set; }
    }

    public class EnemyBoostPrototype : ModPrototype
    {
        public PrototypeId ActivePower { get; protected set; }
        public PrototypeId Quality { get; protected set; }
    }

    public class RarityPrototype : Prototype
    {
        public PrototypeId DowngradeTo { get; protected set; }
        public PrototypeId TextStyle { get; protected set; }
        public int Weight { get; protected set; }
    }

    #region 1.10 Prototypes

    public class ModDisableByBasePrototype : Prototype
    {
    }

    public class ModDisableByMissionRequirementPrototype : ModDisableByBasePrototype
    {
        public PrototypeId Mission { get; protected set; }
    }

    public class ModDisableByModSelectedPrototype : ModDisableByBasePrototype
    {
        public PrototypeId Mod { get; protected set; }
    }

    public class ModDisableByModTypeRequirementPrototype : ModDisableByBasePrototype
    {
        public PrototypeId ModType { get; protected set; }
        public int RanksMin { get; protected set; }
    }

    public class ModDisableBySetPointRequirementPrototype : ModDisableByBasePrototype
    {
        public PrototypeId TalentSet { get; protected set; }
        public int PointsRequired { get; protected set; }
    }

    public class ModDisableByUniqueRequirementPrototype : ModDisableByBasePrototype
    {
        public PrototypeId UniqueSet { get; protected set; }
    }

    public class ModifierSetEntryPrototype : Prototype
    {
        public PrototypeId Modifier { get; protected set; }
    }

    public class ModifierSetPrototype : Prototype
    {
        public ModifierSetEntryPrototype[] Modifiers { get; protected set; }
    }

    public class ModUniqueSetPrototype : Prototype
    {
        public PrototypeId[] Set { get; protected set; }
    }

    #endregion
}
