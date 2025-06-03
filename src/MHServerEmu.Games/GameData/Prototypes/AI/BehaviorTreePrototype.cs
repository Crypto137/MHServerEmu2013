using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes.AI
{
    // V10_TODO: BehaviorTree is an alternative brain type used in place of ProceduralAI,
    // and it's more data-driven. Need to investigate further how it works at some point.

    #region Enums

    [AssetEnum]
    public enum BehaviorParallelMode
    {
        None,
        All,
        One,
    }

    [AssetEnum((int)Probabilistic)]
    public enum BehaviorSelectorType
    {
        Invalid,
        Sequential,
        Probabilistic,
    }

    [AssetEnum((int)Active)]
    public enum BehaviorBranchType
    {
        Active,
        Combat,
        Custom1,
        Idle,
        None,
    }

    [AssetEnum((int)None)]
    public enum ClearSelectedEntityType
    {
        None,
        ClearAssistedEntity,
        ClearTarget,
    }

    [AssetEnum((int)ClosestAllyToTarget)]
    public enum WhoToGiveCommandsToType
    {
        ClosestAllyToTarget,
        RandomAlliesInSenses,
    }

    [AssetEnum((int)NumberOfPlayers)]
    public enum ActionGiveCommandCurveType
    {
        NumberOfPlayers,
        None,
    }

    [AssetEnum((int)EqualTo)]
    public enum PropertyOperatorType
    {
        EqualTo,
        NotEqualTo,
        LessThan,
        LessThanEqualTo,
        GreaterThan,
        GreaterThanEqualTo,
        HasProperty,
        DoesNotHaveProperty,
    }

    [AssetEnum((int)None)]
    public enum ConditionCheckOwnerStateType
    {
        None,
        CanMove,
    }

    [AssetEnum((int)CenterToCenter)]
    public enum DistanceCheckType
    {
        CenterToCenter,
        EdgeToEdge,
        None,
    }

    [AssetEnum((int)Target)]
    public enum ConditionDistanceToEntityType
    {
        Target,
        AssistedEntity,
    }

    [AssetEnum((int)CheckAllies)]
    public enum AllianceCheckType
    {
        CheckAllies,
        CheckEnemies,
    }

    [AssetEnum((int)Absolute)]
    public enum ThresholdValueType
    {
        Absolute,
        Percentage,
        None
    }

    [AssetEnum((int)Target)]
    public enum ConditionNoSelectedEntityType
    {
        Target,
        AssistedEntity,
    }

    [AssetEnum((int)Active)]
    public enum PowerStateType
    {
        Active,
        Cooling,
        IsToggledOn,
        IsChanneling,
        None,
    }

    #endregion

    public class BehaviorActionPrototype : BehaviorTreeNodePrototype
    {
    }

    public class BehaviorConditionalActionPrototype : BehaviorTreeNodePrototype
    {
        public BehaviorActionPrototype Action { get; protected set; }
        public BehaviorConditionPrototype[] Conditions { get; protected set; }
    }

    public class BehaviorConditionPrototype : BehaviorTreeNodePrototype
    {
    }

    public class BehaviorDecoratorPrototype : BehaviorTreeNodePrototype
    {
        public BehaviorTreeNodePrototype Child { get; protected set; }
        public AssetId DecoratorType { get; protected set; }
        public CurveId LoopCount { get; protected set; }
        public AssetId LoopCurveIndex { get; protected set; }
    }

    public class BehaviorParallelPrototype : BehaviorTreeNodePrototype
    {
        public BehaviorTreeNodePrototype[] Children { get; protected set; }
        public BehaviorParallelMode Mode { get; protected set; }
    }

    public class SelectorEntryPrototype : Prototype
    {
        public BehaviorTreeNodePrototype Node { get; protected set; }
        public int Weight { get; protected set; }
    }

    public class BehaviorSelectorPrototype : BehaviorTreeNodePrototype
    {
        public SelectorEntryPrototype[] Options { get; protected set; }
        public BehaviorSelectorType SelectorType { get; protected set; }
    }

    public class BehaviorSequencePrototype : BehaviorTreeNodePrototype
    {
        public BehaviorTreeNodePrototype[] Steps { get; protected set; }
    }

    public class BehaviorTreeInterruptPrototype : Prototype
    {
        public bool CanInterruptOtherNodes { get; protected set; }
        public bool CanBeInterrupted { get; protected set; }
    }

    public class BehaviorTreeInterruptTableEntryPrototype : Prototype
    {
        public BehaviorBranchType Branch { get; protected set; }
        public BehaviorConditionPrototype[] Conditions { get; protected set; }
        public BehaviorInterruptType Interrupt { get; protected set; }
    }

    public class BehaviorTreeNodePrototype : Prototype
    {
        public BehaviorTreeInterruptPrototype Interrupts { get; protected set; }
    }

    public class BehaviorTreePrototype : BrainPrototype
    {
        public BehaviorTreeNodePrototype ActiveBranch { get; protected set; }
        public BehaviorTreeNodePrototype CombatBranch { get; protected set; }
        public BehaviorTreeNodePrototype CustomBranch1 { get; protected set; }
        public BehaviorTreeNodePrototype IdleBranch { get; protected set; }
        public BehaviorTreeInterruptTableEntryPrototype[] InterruptTable { get; protected set; }
    }

    #region Actions

    public class ActionAlertAlliesPrototype : BehaviorActionPrototype
    {
    }

    public class ActionChangeAlliancePrototype : BehaviorActionPrototype
    {
        public PrototypeId NewAlliance { get; protected set; }
    }

    public class ActionChangeBlackboardFactPrototype : BehaviorActionPrototype
    {
        public BlackboardOperatorType Operation { get; protected set; }
        public PrototypeId PropertyInfo { get; protected set; }
        public int Value { get; protected set; }
    }

    public class ActionClearSelectedEntityPrototype : BehaviorActionPrototype
    {
        public ClearSelectedEntityType Type { get; protected set; }
    }

    public class ActionDelayPrototype : BehaviorActionPrototype
    {
        public int MaxDelayMS { get; protected set; }
        public int MinDelayMS { get; protected set; }
    }

    public class ActionDeleteInventoryPrototype : BehaviorActionPrototype
    {
    }

    public class ActionDespawnPrototype : BehaviorActionPrototype
    {
        public bool DespawnOwner { get; protected set; }
        public bool DespawnTarget { get; protected set; }
    }

    public class ActionDropAggroPrototype : BehaviorActionPrototype
    {
    }

    public class ActionFlankPrototype : BehaviorActionPrototype
    {
        public float RangeMax { get; protected set; }
        public float RangeMin { get; protected set; }
        public bool StopAtFlankingWaypoint { get; protected set; }
        public float ToTargetFlankingAngle { get; protected set; }
        public float WaypointRadius { get; protected set; }
        public int TimeoutMS { get; protected set; }
        public bool FailOnTimeout { get; protected set; }
    }

    public class ActionFleePrototype : BehaviorActionPrototype
    {
        public float FleeTime { get; protected set; }
    }

    public class ActionFlockPrototype : BehaviorActionPrototype
    {
        public float RangeMax { get; protected set; }
        public float RangeMin { get; protected set; }
        public float SeparationWeight { get; protected set; }
        public float AlignmentWeight { get; protected set; }
        public float CohesionWeight { get; protected set; }
        public float SeparationThreshold { get; protected set; }
        public float AlignmentThreshold { get; protected set; }
        public float CohesionThreshold { get; protected set; }
        public float MaxSteeringForce { get; protected set; }
        public float ForceToLeaderWeight { get; protected set; }
        public bool SwitchLeaderOnCompletion { get; protected set; }
        public bool ChooseRandomPointAsDestination { get; protected set; }
        public WanderBasePointType WanderFromPointType { get; protected set; }
        public float WanderRadius { get; protected set; }
    }

    public class ActionGiveCommandPrototype : BehaviorActionPrototype
    {
        public WhoToGiveCommandsToType WhomToGiveCommands { get; protected set; }
        public AIEntityAttributePrototype[] AttributeList { get; protected set; }
        public CurveId NumEntitiesToCommandCount { get; protected set; }
        public ActionGiveCommandCurveType NumEntitiesToCommandCurveIndex { get; protected set; }
    }

    public class ActionInteractWithTargetPrototype : BehaviorActionPrototype
    {
    }

    public class ActionMoveToPrototype : BehaviorActionPrototype
    {
        public float LOSSweepPadding { get; protected set; }
        public float RangeMax { get; protected set; }
        public float RangeMin { get; protected set; }
        public bool EnforceLOS { get; protected set; }
        public MoveToType MoveTo { get; protected set; }
        public PathMethod PathNodeSetMethod { get; protected set; }
        public int PathNodeSetGroup { get; protected set; }
        public MovementSpeedOverride MovementSpeed { get; protected set; }
        public bool StopLocomotorOnMoveToFail { get; protected set; }
    }

    public class ActionOrbitPrototype : BehaviorTreeNodePrototype
    {
        public float ThetaInDegrees { get; protected set; }
    }

    public class ActionPerformInterruptPrototype : BehaviorActionPrototype
    {
        public BehaviorInterruptType InterruptType { get; protected set; }
    }

    public class ActionRotatePrototype : BehaviorActionPrototype
    {
        public bool Clockwise { get; protected set; }
        public int Degrees { get; protected set; }
        public bool RotateTowardsTarget { get; protected set; }
        public float SpeedOverride { get; protected set; }
    }

    public class ActionSelectEntityPrototype : BehaviorActionPrototype
    {
        public AIEntityAttributePrototype[] AttributeList { get; protected set; }
        public float MaxDistanceThreshold { get; protected set; }
        public float MinDistanceThreshold { get; protected set; }
        public SelectEntityPoolType PoolType { get; protected set; }
        public SelectEntityMethodType SelectionMethod { get; protected set; }
        public PrototypeId EntitiesPropertyForComparison { get; protected set; }
        public SelectEntityType SelectEntityType { get; protected set; }
        public bool LockEntityOnceSelected { get; protected set; }
        public float CellOrRegionAABBScale { get; protected set; }
    }

    public class ActionSetCombatIdlingPrototype : BehaviorActionPrototype
    {
        public bool CombatIdle { get; protected set; }
    }

    public class ActionSetIgnoreSensesPrototype : BehaviorActionPrototype
    {
        public bool IgnoreSenses { get; protected set; }
    }

    public class ActionShowOverheadTextPrototype : BehaviorActionPrototype
    {
        public LocaleStringId Text { get; protected set; }
        public float Duration { get; protected set; }
    }

    public class ActionTeleportPrototype : BehaviorActionPrototype
    {
        public TeleportContextPrototype TeleportContext { get; protected set; }
    }

    public class ActionTriggerSpawnersPrototype : BehaviorActionPrototype
    {
        public bool EnableSpawner { get; protected set; }
        public PrototypeId[] Spawners { get; protected set; }
    }

    public class ActionUseAffixPowerPrototype : BehaviorActionPrototype
    {
        public UseAffixPowerContextPrototype Context { get; protected set; }
    }

    public class ActionUsePowerPrototype : BehaviorActionPrototype
    {
        public PrototypeId Power { get; protected set; }
        public float TargetOffset { get; protected set; }
        public bool RequireOriPriorToActivate { get; protected set; }
        public float OrientationThreshold { get; protected set; }
        public bool ForceIgnoreLOS { get; protected set; }
        public float OffsetVarianceMagnitude { get; protected set; }
        public bool ChooseRandomTargetPosition { get; protected set; }
        public float OwnerOffset { get; protected set; }
        public SelectEntityContextPrototype SecondaryTargetSelection { get; protected set; }
        public bool TargetsWorldPosition { get; protected set; }
        public bool ForceCheckTargetRegionLocation { get; protected set; }
        public float TargetAngleOffset { get; protected set; }
        public bool UseMainTargetForAOEActivation { get; protected set; }
        public float MinDistanceFromOwner { get; protected set; }
        public bool ForceInvalidTargetActivation { get; protected set; }    // not in the blueprint
        public bool AllowMovementClipping { get; protected set; }
        public float MinDistanceFromTarget { get; protected set; }
    }

    public class ActionWanderPrototype : BehaviorActionPrototype
    {
        public WanderBasePointType FromPoint { get; protected set; }
        public float RangeMax { get; protected set; }
        public float RangeMin { get; protected set; }
        public MovementSpeedOverride MovementSpeed { get; protected set; }
    }

    #endregion

    #region Conditions

    public class ConditionAngleToTargetPrototype : BehaviorConditionPrototype
    {
        public ComparisonOperatorType OperatorType { get; protected set; }
        public float Threshold { get; protected set; }
        public bool DifferentiateLeftAndRight { get; protected set; }
    }

    public class ConditionCheckBlackboardFactPrototype : BehaviorConditionPrototype
    {
        public PropertyOperatorType Operator { get; protected set; }
        public PrototypeId PropertyInfo { get; protected set; }
        public int Value { get; protected set; }
    }

    public class ConditionCheckOwnerStatePrototype : BehaviorConditionPrototype
    {
        public ComparisonOperatorType Operator { get; protected set; }
        public ConditionCheckOwnerStateType OwnerStateToCheck { get; protected set; }
    }

    public class ConditionCheckPropertyPrototype : BehaviorConditionPrototype
    {
        public PropertyOperatorType Operator { get; protected set; }
        public PrototypeId PropertyInfo { get; protected set; }
        public int Value { get; protected set; }
    }

    public class ConditionDistanceToEntityPrototype : BehaviorConditionPrototype
    {
        public ComparisonOperatorType OperatorType { get; protected set; }
        public float Threshold { get; protected set; }
        public DistanceCheckType DistanceCheckType { get; protected set; }
        public ConditionDistanceToEntityType CheckDistanceTo { get; protected set; }
    }

    public class ConditionEnemiesWithinPowerRangePrototype : BehaviorConditionPrototype
    {
        public ComparisonOperatorType OperatorType { get; protected set; }
        public PrototypeId Power { get; protected set; }
        public int Threshold { get; protected set; }
    }

    public class ConditionEnemiesWithinRangePrototype : BehaviorConditionPrototype
    {
        public ComparisonOperatorType OperatorType { get; protected set; }
        public float RangeMax { get; protected set; }
        public int Threshold { get; protected set; }
        public float RangeMin { get; protected set; }
        public AllianceCheckType CheckAlliance { get; protected set; }
        public AIEntityAttributePrototype[] AttributeList { get; protected set; }
        public bool IgnoreDead { get; protected set; }
    }

    public class ConditionHealthSelfPrototype : BehaviorConditionPrototype
    {
        public ComparisonOperatorType OperatorType { get; protected set; }
        public ThresholdValueType ValueType { get; protected set; }
        public float Threshold { get; protected set; }
    }

    public class ConditionLosToTargetPrototype : BehaviorConditionPrototype
    {
        public float SweepRadius { get; protected set; }
        public float SweepPadding { get; protected set; }
    }

    public class ConditionNoSelectedEntityPrototype : BehaviorConditionPrototype
    {
        public ConditionNoSelectedEntityType Type { get; protected set; }
    }

    public class ConditionPowerStatePrototype : BehaviorConditionPrototype
    {
        public ComparisonOperatorType OperatorType { get; protected set; }
        public PrototypeId Power { get; protected set; }
        public PowerStateType PowerState { get; protected set; }
    }

    public class ConditionTargetIsPrototypeOfPrototype : BehaviorConditionPrototype
    {
        public PrototypeId PrototypeRef { get; protected set; }
    }

    public class ConditionTimerPrototype : BehaviorConditionPrototype
    {
        public bool ResetOnComplete { get; protected set; }
        public bool ResetOnBranchChange { get; protected set; }     // not in the blueprint
        public float MinSeconds { get; protected set; }
        public float MaxSeconds { get; protected set; }
    }

    #endregion
}
