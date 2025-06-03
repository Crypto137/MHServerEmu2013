using MHServerEmu.Games.GameData.Calligraphy;
using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes.AI
{
    #region Enums

    [AssetEnum((int)None)]
    public enum ComparisonOperatorType
    {
        EqualTo,
        GreaterThan,
        GreaterThanEqualTo,
        LessThan,
        LessThanEqualTo,
        NotEqualTo,
        None,
    }

    [AssetEnum((int)None)]
    public enum TeleportType
    {
        None,
        AssistedEntity,
        SpawnPosition,
    }

    [AssetEnum((int)None)]
    public enum SelectEntityPoolType
    {
        None,
        AllEntitiesInCellOfAgent,
        AllEntitiesInRegionOfAgent,
        PotentialAlliesOfAgent,
        PotentialEnemiesOfAgent,
        // Not found in client
        ItemsAroundAgent = 0,
    }

    [AssetEnum((int)None)]
    public enum SelectEntityMethodType
    {
        None,
        ClosestEntity,
        FarthestEntity,
        Type3,
        FirstFound,
        HighestValueOfProperty,
        LowestValueOfProperty,
        MostDamageInTimeInterval,
        RandomEntity,
        Self,
    }

    [AssetEnum((int)None)]
    public enum SelectEntityType
    {
        None,
        SelectAssistedEntity,
        SelectTarget,
        SelectTargetByAssistedEntitiesLastTarget,
    }

    [AssetEnum((int)Target)]
    public enum MoveToType
    {
        Target,
        SpawnPosition,
        DespawnPosition,
        PathNode,
        AssistedEntity
    }

    [AssetEnum((int)Invalid)]
    public enum PathMethod  // AI/Misc/Types/MoveToPathMethodType.type
    {
        Invalid,
        Forward,
        Reverse,
        ForwardBackAndForth,
        ReverseBackAndForth,
        ForwardLoop,
        ReverseLoop,
    }

    [AssetEnum((int)Default)]
    public enum MovementSpeedOverride
    {
        Default,
        Walk,
        Run,
    }

    [AssetEnum((int)None)]
    public enum WanderBasePointType
    {
        CurrentPosition,
        SpawnPoint,
        TargetPosition,
        None,
    }

    [AssetEnum((int)Set)]
    public enum BlackboardOperatorType
    {
        Add,
        Div,
        Mul,
        Set,
        Sub,
        SetTargetId,
        ClearTargetId,
    }

    [AssetEnum((int)None)]
    [Flags]
    public enum BehaviorInterruptType
    {
        None                = 0,
        Alerted             = 1 << 0,
        AllyDeath           = 1 << 1,
        CollisionWithTarget = 1 << 2,
        Command             = 1 << 3,
        Defeated            = 1 << 4,
        ForceIdle           = 1 << 5,
        InitialBranch       = 1 << 6,
        LeashDistanceMet    = 1 << 7,
        NoTarget            = 1 << 8,
        Override            = 1 << 9,
        TargetSighted       = 1 << 10,
    }

    #endregion

    public class BrainPrototype : Prototype
    {
    }

    public class AlliancePrototype : Prototype
    {
        public PrototypeId[] HostileTo { get; protected set; }
        public PrototypeId[] FriendlyTo { get; protected set; }
    }

    public class BotDefinitionEntryPrototype : Prototype
    {
        public PrototypeId Avatar { get; protected set; }
        public BehaviorProfilePrototype BehaviorProfile { get; protected set; }
    }

    public class BotSettingsPrototype : Prototype
    {
        public BotDefinitionEntryPrototype[] BotDefinitions { get; protected set; }
        public BehaviorProfilePrototype DefaultProceduralBotProfile { get; protected set; }
    }

    public class AIEntityAttributePrototype : Prototype
    {
        public ComparisonOperatorType OperatorType { get; protected set; }
    }

    public class AIEntityAttributeHasKeywordPrototype : AIEntityAttributePrototype
    {
        public PrototypeId Keyword { get; protected set; }
    }

    public class AIEntityAttributeIsHostilePrototype : AIEntityAttributePrototype
    {
    }

    public class AIEntityAttributeIsMeleePrototype : AIEntityAttributePrototype
    {
    }

    public class AIEntityAttributeIsPrototypeRefPrototype : AIEntityAttributePrototype
    {
        public PrototypeId ProtoRef { get; protected set; }
    }

    public class AIEntityAttributeIsPrototypePrototype : AIEntityAttributePrototype
    {
        public PrototypeId RefToPrototype { get; protected set; }
    }

    public class AIEntityAttributeIsSimulatedPrototype : AIEntityAttributePrototype
    {
    }

    public class AIEntityAttributeIsCurrentTargetEntityPrototype : AIEntityAttributePrototype
    {
    }

    public class AIEntityAttributeIsCurrentTargetEntityOfAgentOfTypePrototype : AIEntityAttributePrototype
    {
        public PrototypeId OtherAgentProtoRef { get; protected set; }
    }

    public class AIEntityAttributeIsSummonedByPowerPrototype : AIEntityAttributePrototype
    {
        public PrototypeId Power { get; protected set; }
    }

    public class AIEntityAttributeCanBePlayerOwnedPrototype : AIEntityAttributePrototype
    {
    }

    public class AIEntityAttributeHasBlackboardPropertyValuePrototype : AIEntityAttributePrototype
    {
        public PrototypeId PropertyInfoRef { get; protected set; }
        public int Value { get; protected set; }
    }

    public class AIEntityAttributeHasPropertyPrototype : AIEntityAttributePrototype
    {
        public PrototypeId PropertyInfoRef { get; protected set; }
    }

    public class AIEntityAttributeHasHealthValuePrototype : AIEntityAttributePrototype  // V10_NOTE: Older version of AIEntityAttributeHasHealthValuePercentPrototype
    {
        public float Value { get; protected set; }
    }

    public class AIEntityAttributeIsDestructiblePrototype : AIEntityAttributePrototype
    {
    }

    public class AIEntityAttributeCanPathToPrototype : AIEntityAttributePrototype
    {
        public LocomotorMethod LocomotorMethod { get; protected set; }
    }

    public class DelayContextPrototype : Prototype
    {
        public int MaxDelayMS { get; protected set; }
        public int MinDelayMS { get; protected set; }
    }

    public class TeleportContextPrototype : Prototype
    {
        public TeleportType TeleportType { get; protected set; }
    }

    public class SelectEntityContextPrototype : Prototype
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

    public class FlankContextPrototype : Prototype
    {
        public float RangeMax { get; protected set; }
        public float RangeMin { get; protected set; }
        public bool StopAtFlankingWaypoint { get; protected set; }
        public float ToTargetFlankingAngle { get; protected set; }
        public float WaypointRadius { get; protected set; }
        public int TimeoutMS { get; protected set; }
        public bool FailOnTimeout { get; protected set; }
    }

    public class FleeContextPrototype : Prototype
    {
        public float FleeTime { get; protected set; }
    }

    public class UseAffixPowerContextPrototype : Prototype
    {
    }

    public class UsePowerContextPrototype : Prototype
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
        public bool ForceInvalidTargetActivation { get; protected set; }
        public bool AllowMovementClipping { get; protected set; }
        public float MinDistanceFromTarget { get; protected set; }
    }

    public class MoveToContextPrototype : Prototype
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

    public class OrbitContextPrototype : Prototype
    {
        public float ThetaInDegrees { get; protected set; }
    }

    public class RotateContextPrototype : Prototype
    {
        public bool Clockwise { get; protected set; }
        public int Degrees { get; protected set; }
        public bool RotateTowardsTarget { get; protected set; }
        public float SpeedOverride { get; protected set; }
    }

    public class WanderContextPrototype : Prototype
    {
        public WanderBasePointType FromPoint { get; protected set; }
        public float RangeMax { get; protected set; }
        public float RangeMin { get; protected set; }
        public MovementSpeedOverride MovementSpeed { get; protected set; }
    }

    public class DespawnContextPrototype : Prototype
    {
        public bool DespawnOwner { get; protected set; }
        public bool DespawnTarget { get; protected set; }
        public bool UseKillInsteadOfDestroy { get; protected set; }
    }

    public class TriggerSpawnersContextPrototype : Prototype
    {
        public bool DoPulse { get; protected set; }
        public bool EnableSpawner { get; protected set; }
        public PrototypeId[] Spawners { get; protected set; }
    }

    public class BehaviorProfilePrototype : Prototype
    {
        public float AggroDropChanceLOS { get; protected set; }
        public float AggroDropDistance { get; protected set; }
        public float AggroRangeAlly { get; protected set; }
        public float AggroRangeHostile { get; protected set; }
        public PrototypeId Brain { get; protected set; }
        public PrototypeId[] EquippedPassivePowers { get; protected set; }
        public bool IsBot { get; protected set; }
        public int InterruptCooldownMS { get; protected set; }
        public bool CanLeash { get; protected set; }
        public PrototypePropertyCollection Properties { get; protected set; }
    }
}
