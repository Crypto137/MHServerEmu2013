namespace MHServerEmu.Games.GameData.Prototypes
{
    public class ModifierCountEntryPrototype : Prototype
    {
        public int ChancePct { get; protected set; }
    }

    public class DamageCurveEntryPrototype : Prototype
    {
        public int Rank { get; protected set; }
        public CurveId MobToPlayerDamageCurve { get; protected set; }
        public CurveId PlayerToMobDamageCurve { get; protected set; }
    }

    public class NegStatusRankCurveEntryPrototype : Prototype
    {
        public int Rank { get; protected set; }
        public CurveId ChanceMultiplierCurve { get; protected set; }
        public CurveId DurationMultiplierCurve { get; protected set; }
    }

    public class NegStatusPropCurveEntryPrototype : Prototype
    {
        public PrototypeId NegStatusProp { get; protected set; }
        public NegStatusRankCurveEntryPrototype[] RankEntries { get; protected set; }
    }

    public class DamageMultiplierEntryPrototype : Prototype
    {
        public int Rank { get; protected set; }
        public float PlayerToMobReflectDamageMult { get; protected set; }
    }

    public class DifficultyPrototype : Prototype
    {
        public int EnemyModificationChance { get; protected set; }
        public ModifierCountEntryPrototype[] EnemyModificationCount { get; protected set; }
        public LocaleStringId Name { get; protected set; }
        public float PlayerInflictedDamageTimerSec { get; protected set; }
        public float PlayerNearbyRange { get; protected set; }
        public CurveId PctXPFromLevelCurve { get; protected set; }
        public DamageCurveEntryPrototype[] DamageCurves { get; protected set; }
        public CurveId RestedXPMultBonusFromLevelCurve { get; protected set; }
        public CurveId RestedXPMultByInactiveMinutes { get; protected set; }
        public CurveId RestedXPMaxByLevel { get; protected set; }
        public NegStatusPropCurveEntryPrototype[] NegativeStatusCurves { get; protected set; }
        public DamageMultiplierEntryPrototype[] DamageMultipliers { get; protected set; }
        public float PlayerXPNearbyRange { get; protected set; }
        public CurveId LootFindByLevelDeltaCurve { get; protected set; }
        public CurveId SpecialItemFindByLevelDeltaCurve { get; protected set; }
        public int EnemyModificationRankUpChance { get; protected set; }
        public CurveId MobConLevelCurve { get; protected set; }
    }

    public class DifficultyModifierPrototype : Prototype
    {
        public DamageCurveEntryPrototype[] DamageCurves { get; protected set; }
    }
}
