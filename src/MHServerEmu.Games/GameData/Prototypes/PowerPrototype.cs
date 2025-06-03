using MHServerEmu.Games.GameData.Calligraphy;
using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.GameData.Prototypes.AI;
using MHServerEmu.Games.Powers;

namespace MHServerEmu.Games.GameData.Prototypes
{
    public class PowerPrototype : Prototype
    {
        public PrototypePropertyCollection Properties { get; protected set; }
        public TriggeredPowerEventActionPrototype[] ActionsTriggeredOnPowerEvent { get; protected set; }
        public AssetId Activation { get; protected set; }
        public float AnimationContactTimePercent { get; protected set; }
        public int AnimationTimeMS { get; protected set; }
        [ListMixin(typeof(ConditionPrototype))]
        public PrototypeMixinList AppliesConditions { get; protected set; }
        public bool CancelConditionsOnEnd { get; protected set; }
        public bool CancelledOnDamage { get; protected set; }
        public bool CancelledOnMove { get; protected set; }
        public bool CanBeDodged { get; protected set; }
        public bool CanCrit { get; protected set; }
        public EvalPrototype ChannelLoopTimeMS { get; protected set; }
        public int ChargingTimeMS { get; protected set; }
        [ListMixin(typeof(ConditionEffectPrototype))]
        public PrototypeMixinList ConditionEffects { get; protected set; }
        public EvalPrototype CooldownTimeMS { get; protected set; }
        public DesignWorkflowState DesignState { get; protected set; }
        public LocaleStringId DisplayName { get; protected set; }
        public AssetId IconPath { get; protected set; }
        public bool IsToggled { get; protected set; }
        public PowerCategoryType PowerCategory { get; protected set; }
        public AssetId PowerUnrealClass { get; protected set; }
        public EvalPrototype ProjectileSpeed { get; protected set; }
        public float Radius { get; protected set; }
        public bool RemovedOnUse { get; protected set; }
        public StackingBehaviorPrototype StackingBehavior { get; protected set; }
        public bool StopsUserMovementOnActivate { get; protected set; }
        public PrototypeId TargetingReach { get; protected set; }
        public PrototypeId TargetingStyle { get; protected set; }
        public bool UsableByAll { get; protected set; }
        public bool HideFloatingNumbers { get; protected set; }
        public int PostContactDelayMS { get; protected set; }
        public PrototypeId[] Keywords { get; protected set; }
        public bool CancelConditionsOnUnassign { get; protected set; }
        public float HeightCheckPadding { get; protected set; }
        public bool FlyingUsable { get; protected set; }
        public ExtraActivatePrototype ExtraActivation { get; protected set; }
        public bool CancelledOnButtonRelease { get; protected set; }
        public PowerUnrealOverridePrototype[] PowerUnrealOverrides { get; protected set; }
        public bool CanBeInterrupted { get; protected set; }
        public int ChannelStartTimeMS { get; protected set; }
        public int ChannelEndTimeMS { get; protected set; }
        public bool ForceNonExclusive { get; protected set; }
        public WhenOutOfRangeType WhenOutOfRange { get; protected set; }
        public int NoInterruptPreWindowMS { get; protected set; }
        public int NoInterruptPostWindowMS { get; protected set; }
        public LocaleStringId TooltipDescriptionText { get; protected set; }
        public float ProjectileTimeToImpactOverride { get; protected set; }
        public AbilitySlotRestrictionPrototype SlotRestriction { get; protected set; }
        public bool ActiveUntilCancelled { get; protected set; }
        public TooltipTranslationEntryPrototype[] TooltipInfoCurrentRank { get; protected set; }
        public TooltipTranslationEntryPrototype[] TooltipInfoFirstRankPreview { get; protected set; }
        public TooltipTranslationEntryPrototype[] TooltipInfoNextRank { get; protected set; }
        public bool StopsContinuousIfTargetMissing { get; protected set; }
        public bool ResetTargetPositionAtContactTime { get; protected set; }
        public float RangeMinimum { get; protected set; }
        public EvalPrototype Range { get; protected set; }
        public int ChannelMinTimeMS { get; protected set; }
        public int MaxAOETargets { get; protected set; }
        public EvalPrototype[] EvalOnActivate { get; protected set; }
        public EvalPrototype[] EvalOnCreate { get; protected set; }
        public bool CooldownOnPlayer { get; protected set; }
        public bool DisableEnduranceRegenOnEnd { get; protected set; }
        public PowerSynergyTooltipEntryPrototype[] TooltipPowerSynergyBonuses { get; protected set; }
        public SituationalPowerComponentPrototype SituationalComponent { get; protected set; }
        public bool DisableEnduranceRegenOnActivate { get; protected set; }
        public EvalPrototype[] EvalOnPreApply { get; protected set; }
        public int RecurringCostIntervalMS { get; protected set; }
        public PrototypeId[] ConditionsByRef { get; protected set; }
        public bool IsRecurring { get; protected set; }
        public EvalPrototype EvalCanTrigger { get; protected set; }
        public float RangeActivationReduction { get; protected set; }
        public EvalPrototype EvalPowerSynergies { get; protected set; }
        public bool DisableContinuous { get; protected set; }
        public bool CooldownDisableUI { get; protected set; }
        public bool DOTIsDirectionalToCaster { get; protected set; }
        public bool OmniDurationBonusExclude { get; protected set; }
        public PrototypeId ToggleGroup { get; protected set; }
        public bool IsUltimate { get; protected set; }
        public bool PlayNotifySfxOnAvailable { get; protected set; }
        public CurveId BounceDamagePctToSameIdCurve { get; protected set; }
        public PrototypeId[] RefreshDependentPassivePowers { get; protected set; }
        public EvalPrototype EvalAnimationTimeMultiplier { get; protected set; }
        public EvalPrototype TargetRestrictionEval { get; protected set; }
        public bool IsUseableWhileDead { get; protected set; }
        public float DamageBonusMultiplier { get; protected set; }
        public float OnHitProcChanceMultiplier { get; protected set; }
        public bool ApplyResultsImmediately { get; protected set; }
        public bool AllowHitReactOnClient { get; protected set; }
        public bool CanCauseHitReact { get; protected set; }
    }

    public class MovementPowerPrototype : PowerPrototype
    {
        public bool MoveToExactTargetLocation { get; protected set; }
        public bool NoCollideIncludesTarget { get; protected set; }
        public bool MoveToOppositeEdgeOfTarget { get; protected set; }
        public bool ConstantMoveTime { get; protected set; }
        public float AdditionalTargetPosOffset { get; protected set; }
        public bool MoveToSecondaryTarget { get; protected set; }
        public bool MoveFullDistance { get; protected set; }
        public bool IsTeleport { get; protected set; }
        public float MoveMinDistance { get; protected set; }
        public bool UserNoEntityCollide { get; protected set; }
        public bool StrafeTarget { get; protected set; }
        public bool AllowOrientationChange { get; protected set; }
        public float PowerMovementPathPct { get; protected set; }
        public int MovementHeightBonus { get; protected set; }
        public bool IsHighFlying { get; protected set; }
        public bool FollowsMouseWhileActive { get; protected set; }
        public EvalPrototype EvalUserMoveSpeed { get; protected set; }
    }

    public class PowerEventContextTransformModePrototype : PowerEventContextPrototype
    {
        public PrototypeId TransformMode { get; protected set; }
    }

    public class PowerEventContextShowBannerMessagePrototype : PowerEventContextPrototype
    {
        public PrototypeId BannerMessage { get; protected set; }
    }

    public class PowerToggleGroupPrototype : Prototype
    {
    }

    public class PowerEventContextPrototype : Prototype
    {
    }

    public class PowerEventContextOffsetActivationAOEPrototype : PowerEventContextPrototype
    {
        public float PositionOffsetMagnitude { get; protected set; }
        public float RotationOffsetDegrees { get; protected set; }
        public bool UseIncomingTargetPosAsUserPos { get; protected set; }
    }

    public class AbilityAssignmentPrototype : Prototype
    {
        public PrototypeId Ability { get; protected set; }
        public int Rank { get; protected set; }
    }

    public class PowerEventContextCallbackPrototype : PowerEventContextPrototype
    {
        public bool SetContextOnOwnerAgent { get; protected set; }
        public bool SetContextOnOwnerSummonEntities { get; protected set; }
    }

    public class AbilitySlotRestrictionPrototype : Prototype
    {
        public bool ActionKeySlotOK { get; protected set; }
        public bool LeftMouseSlotOK { get; protected set; }
        public bool RightMouseSlotOK { get; protected set; }
    }

    public class TriggeredPowerEventActionPrototype : Prototype     // V10_NOTE: Older version of PowerEventActionPrototype
    {
        public PowerEventActionType EventAction { get; protected set; }
        public float EventParam { get; protected set; }
        public PrototypeId Power { get; protected set; }
        public PowerEventType PowerEvent { get; protected set; }
        public PowerEventContextPrototype PowerEventContext { get; protected set; }
        public PrototypeId[] Keywords { get; protected set; }
        public bool UseTriggerPowerOriginalTargetPos { get; protected set; }
        public bool UseTriggeringPowerTargetVerbatim { get; protected set; }
        public EvalPrototype EvalEventTriggerChance { get; protected set; }
    }

    #region SituationalTriggerPrototype

    public class SituationalTriggerPrototype : Prototype
    {
        public PrototypeId TriggerCollider { get; protected set; }
        public float TriggerRadiusScaling { get; protected set; }
        public EntityFilterPrototype EntityFilter { get; protected set; }
        public bool AllowDead { get; protected set; }
    }

    public class SituationalTriggerOnKilledPrototype : SituationalTriggerPrototype
    {
        public bool Friendly { get; protected set; }
        public bool Hostile { get; protected set; }
        public bool KilledByOther { get; protected set; }
        public bool KilledBySelf { get; protected set; }
        public bool WasLastInRange { get; protected set; }
    }

    public class SituationalTriggerOnHealthThresholdPrototype : SituationalTriggerPrototype
    {
        public bool HealthBelow { get; protected set; }
        public float HealthPercent { get; protected set; }
    }

    public class SituationalTriggerOnStatusEffectPrototype : SituationalTriggerPrototype
    {
        public PrototypeId[] TriggeringProperties { get; protected set; }
        public bool TriggersOnStatusApplied { get; protected set; }
    }

    #endregion

    public class SituationalPowerComponentPrototype : Prototype
    {
        public int ActivationWindowMS { get; protected set; }
        public EvalPrototype ChanceToTrigger { get; protected set; }
        public bool ForceRelockOnTriggerRevert { get; protected set; }
        public bool RemoveTriggeringEntityOnActivate { get; protected set; }
        public SituationalTriggerPrototype SituationalTrigger { get; protected set; }
        public bool TargetsTriggeringEntity { get; protected set; }
        public bool ForceRelockOnActivate { get; protected set; }
    }

    public class PowerUnrealReplacementPrototype : Prototype
    {
        public AssetId EntityArt { get; protected set; }
        public AssetId PowerArt { get; protected set; }
    }

    public class PowerUnrealOverridePrototype : Prototype
    {
        public float AnimationContactTimePercent { get; protected set; }
        public int AnimationTimeMS { get; protected set; }
        public AssetId EntityArt { get; protected set; }
        public AssetId PowerArt { get; protected set; }
        public PowerUnrealReplacementPrototype[] ArtOnlyReplacements { get; protected set; }
    }

    public class PowerSynergyTooltipEntryPrototype : Prototype
    {
        public PrototypeId SynergyPower { get; protected set; }
        public PrototypeId Translation { get; protected set; }
    }

    public class PowerEventContextCallbackAIChangeBlackboardPropertyPrototype : PowerEventContextCallbackPrototype
    {
        public BlackboardOperatorType Operation { get; protected set; }
        public PrototypeId PropertyInfoRef { get; protected set; }
        public int Value { get; protected set; }
    }

    public class PowerEventContextCallbackAISetAssistedEntityFromCreatorPrototype : PowerEventContextCallbackPrototype
    {
    }

    public class TransformModePrototype : Prototype
    {
        public AbilityAssignmentPrototype[] DefaultEquippedAbilities { get; protected set; }
        public PrototypeId EnterTransformModePower { get; protected set; }
        public PrototypeId ExitTransformModePower { get; protected set; }
        public AssetId UnrealClass { get; protected set; }
        public PrototypeId[] HiddenPassivePowers { get; protected set; }
    }

    public class TransformModeEntryPrototype : Prototype
    {
        public PrototypeId[] AllowedPowers { get; protected set; }
        public PrototypeId TransformMode { get; protected set; }
    }

    public class TargetingStylePrototype : Prototype
    {
        public AOEAngleType AOEAngle { get; protected set; }
        public bool AOESelfCentered { get; protected set; }
        public bool NeedsTarget { get; protected set; }
        public bool OffsetWedgeBehindUser { get; protected set; }
        public float OrientationOffset { get; protected set; }
        public TargetingShapeType TargetingShape { get; protected set; }
        public bool TurnsToFaceTarget { get; protected set; }
        public float Width { get; protected set; }
        public bool AlwaysTargetMousePos { get; protected set; }
        public bool MovesToRangeOfPrimaryTarget { get; protected set; }
        public bool UseDefaultRotationSpeed { get; protected set; }
    }

    public class TargetingReachPrototype : Prototype
    {
        public bool ExcludesPrimaryTarget { get; protected set; }
        public bool Melee { get; protected set; }
        public bool RequiresLineOfSight { get; protected set; }
        public bool TargetsEnemy { get; protected set; }
        public bool TargetsFlying { get; protected set; }
        public bool TargetsFriendly { get; protected set; }
        public bool TargetsGround { get; protected set; }
        public bool WillTargetCaster { get; protected set; }
        public bool LowestHealth { get; protected set; }
        public TargetingHeightType TargetingHeightType { get; protected set; }
        public bool PartyOnly { get; protected set; }
        public bool WillTargetUltimateOwner { get; protected set; }
        public bool TargetsDestructibles { get; protected set; }
        public bool LOSCheckAlongGround { get; protected set; }
    }

    public class ExtraActivatePrototype : Prototype
    {
    }

    public class SecondaryActivateOnReleasePrototype : ExtraActivatePrototype
    {
        public CurveId DamageIncreasePerSecond { get; protected set; }
        public DamageType DamageIncreaseType { get; protected set; }
        public CurveId EnduranceCostIncreasePerSecond { get; protected set; }
        public int MaxReleaseTimeMS { get; protected set; }
        public int MinReleaseTimeMS { get; protected set; }
        public CurveId RangeIncreasePerSecond { get; protected set; }
        public CurveId RadiusIncreasePerSecond { get; protected set; }
        public bool ActivateOnMaxReleaseTime { get; protected set; }
        public CurveId ResistancePenetrationIncrPerSec { get; protected set; }
        public AssetId ResistancePenetrationType { get; protected set; }
    }

    public class ExtraActivateOnSubsequentPrototype : ExtraActivatePrototype
    {
        public int NumActivatesBeforeCooldown { get; protected set; }
        public int TimeoutLengthMS { get; protected set; }
        public SubsequentActivateType ExtraActivateEffect { get; protected set; }
    }

    public class ExtraActivateCycleToPowerPrototype : ExtraActivatePrototype
    {
        public PrototypeId[] CyclePowerList { get; protected set; }
    }

    public class StackingBehaviorPrototype : Prototype
    {
        public StackingApplicationStyleType ApplicationStyle { get; protected set; }
        public float ApplicationStyleRadius { get; protected set; }
        public int MaxNumStacks { get; protected set; }
        public StackingActionType OnMaxNumStacksReached { get; protected set; }
        public StackingActionType OnSameTargetIncomingPower { get; protected set; }
        public PrototypeId[] StacksWithOtherPrototypesList { get; protected set; }
        public bool StacksFromDifferentOwners { get; protected set; }
    }

    public class PowerReplacementPowerPrototype : PowerPrototype    // V10_NOTE: This looks like an older version of transform modes
    {
        public PrototypeId[] PowerReplacements { get; protected set; }
    }
}
