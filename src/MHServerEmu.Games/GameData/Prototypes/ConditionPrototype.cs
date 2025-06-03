using MHServerEmu.Games.GameData.Calligraphy;
using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.Powers;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)Target)]
    public enum ConditionScopeType
    {
        Target = 0,
        User = 1,
    }

    [AssetEnum((int)Neither)]
    public enum ConditionType
    {
        Neither,
        Buff,
        Debuff,
    }

    [AssetEnum((int)Immediate)]
    public enum ConditionRemovalType
    {
        Immediate,
        DeferToEndOfFrame,
        DisableAndDeferToEndOfFrame,
    }

    #endregion

    public class ConditionUnrealPrototype : Prototype
    {
        public AssetId ConditionArt { get; protected set; }
        public AssetId EntityArt { get; protected set; }
    }

    public class ConditionPrototype : Prototype
    {
        public bool CancelOnHit { get; protected set; }
        public bool CancelOnPowerUse { get; protected set; }
        public long DurationMS { get; protected set; }
        public LocaleStringId TooltipText { get; protected set; }
        public AssetId IconPath { get; protected set; }
        public bool PauseDurationCountdown { get; protected set; }
        public PrototypePropertyCollection Properties { get; protected set; }
        public ConditionScopeType Scope { get; protected set; }
        public AssetId UnrealClass { get; protected set; }
        public EvalPrototype ChanceToApplyCondition { get; protected set; }
        public ConditionType ConditionType { get; protected set; }
        public bool VisualOnly { get; protected set; }
        public ConditionUnrealPrototype[] UnrealOverrides { get; protected set; }
        public PrototypeId[] Keywords { get; protected set; }
        public ConditionRemovalType RemovalType { get; protected set; }
        public CurveId DurationMSCurve { get; protected set; }
        public PrototypeId DurationMSCurveIndex { get; protected set; }
        public bool ForceShowClientConditionFX { get; protected set; }
        public ProcTriggerType[] CancelOnProcTriggers { get; protected set; }
        public int UpdateIntervalMS { get; protected set; }
        public EvalPrototype DurationMSEval { get; protected set; }
        public PrototypeId TooltipStyle { get; protected set; }
        public AssetId TooltipFont { get; protected set; }
        public EvalPrototype[] EvalOnCreate { get; protected set; }
        public PrototypeId CancelOnPowerUseKeyword { get; protected set; }
        public bool CancelOnPowerUsePost { get; protected set; }
        public bool PersistToDB { get; protected set; }
        public bool CancelOnKilled { get; protected set; }
        public bool DisplayInBoostsHUD { get; protected set; }
        public bool ApplyOverTimeEffectsToOriginator { get; protected set; }
        public bool TransferToCurrentAvatar { get; protected set; }
    }

    public class ConditionEffectPrototype : Prototype
    {
        public PrototypePropertyCollection Properties { get; protected set; }
        public int ConditionNum { get; protected set; }
    }
}
