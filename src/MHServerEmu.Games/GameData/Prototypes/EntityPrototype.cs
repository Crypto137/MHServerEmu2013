using MHServerEmu.Games.GameData.Calligraphy;
using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.GameData.Prototypes.AI;
using MHServerEmu.Games.Network;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)None)]
    public enum DesignWorkflowState
    {
        None            = 0,
        Declared        = 1 << 0,
        Populated       = 1 << 1,
        ReadyForReview  = 1 << 2,
        ReadyForGame    = 1 << 3,
    }

    [AssetEnum((int)None)]
    public enum DialogStyle
    {
        None,
        ComputerTerminal,
        NPCDialog,
        OverheadText,
    }

    [AssetEnum((int)None)]
    public enum LocomotorMethod
    {
        None,
        Ground,
        Airborne,
        TallGround,
        Missile,
        MissileSeeking,
        HighFlying,
        Default,
    }

    [AssetEnum((int)Invalid)]
    public enum EntityAppearanceEnum
    {
        Invalid = -1,
        None,
        Closed,
        Destroyed,
        Disabled,
        Enabled,
        Locked,
        Open,
        Dead,
    }

    [AssetEnum((int)None)]
    public enum HotspotOverlapEventTriggerType
    {
        None,
        Allies,
        Enemies,
        All,
    }

    [AssetEnum((int)RandomScatter)]
    public enum SpawnMethod
    {
        RandomScatter,
    }

    #endregion

    public class EntityPrototype : Prototype
    {
        public LocaleStringId DisplayName { get; protected set; }
        public AssetId IconPath { get; protected set; }
        public PrototypePropertyCollection Properties { get; protected set; }
        public bool ReplicateToProximity { get; protected set; }
        public bool ReplicateToParty { get; protected set; }
        public bool ReplicateToOwner { get; protected set; }
        public bool ReplicateToDiscovered { get; protected set; }
        public EntityInventoryAssignmentPrototype[] Inventories { get; protected set; }
        public EvalPrototype[] EvalOnCreate { get; protected set; }

        //---

        [DoNotCopy]
        public AOINetworkPolicyValues RepNetwork { get; private set; } = AOINetworkPolicyValues.AOIChannelNone;

        public override void PostProcess()
        {
            base.PostProcess();

            // Reconstruct AOI network policy (same thing as PropertyInfoPrototype::PostProcess())
            if (ReplicateToProximity)
                RepNetwork |= AOINetworkPolicyValues.AOIChannelProximity;

            if (ReplicateToParty)
                RepNetwork |= AOINetworkPolicyValues.AOIChannelParty;

            if (ReplicateToOwner)
                RepNetwork |= AOINetworkPolicyValues.AOIChannelOwner;

            if (ReplicateToDiscovered)
                RepNetwork |= AOINetworkPolicyValues.AOIChannelDiscovery;
        }
    }

    public class WorldEntityPrototype : EntityPrototype
    {
        public PrototypeId Alliance { get; protected set; }
        public BoundsPrototype Bounds { get; protected set; }
        public LocaleStringId DialogText { get; protected set; }
        public AssetId UnrealClass { get; protected set; }
        public CurveId XPGrantedCurve { get; protected set; }
        public bool HACKBuildMouseCollision { get; protected set; }
        public PrototypeId PreInteractPower { get; protected set; }
        public DialogStyle DialogStyle { get; protected set; }
        public WeightedTextEntryPrototype[] DialogTextList { get; protected set; }
        public PrototypeId[] Keywords { get; protected set; }
        public PrototypeId WorldEntityMapInfo { get; protected set; }
        public DesignWorkflowState DesignState { get; protected set; }
        public PrototypeId ModifierSet { get; protected set; }
        public PrototypeId Rank { get; protected set; }
        public LocomotorMethod NaviMethod { get; protected set; }
        public bool SnapToFloorOnSpawn { get; protected set; }
        public bool AffectNavigation { get; protected set; }
        public StateChangePrototype PostInteractState { get; protected set; }
        public StateChangePrototype PostKilledState { get; protected set; }
        public bool OrientToInteractor { get; protected set; }
        public PrototypeId TooltipInWorldTemplate { get; protected set; }
        public bool InteractIgnoreBoundsForDistance { get; protected set; }
        public AssetId EdgePointerIconPath { get; protected set; }
        public float PopulationWeight { get; protected set; }
        public bool VisibleByDefault { get; protected set; }
        public int RemoveFromWorldTimerMS { get; protected set; }
        public bool RemoveNavInfluenceOnKilled { get; protected set; }
        public bool AlwaysSimulated { get; protected set; }
        public bool XPIsShared { get; protected set; }
        public PrototypeId TutorialTip { get; protected set; }
        public bool TrackingDisabled { get; protected set; }

        //---

        public override bool ApprovedForUse()
        {
            return GameDatabase.DesignStateOk(DesignState);
        }
    }

    public class StateChangePrototype : Prototype
    {
    }

    public class StateTogglePrototype : StateChangePrototype
    {
        public PrototypeId StateA { get; protected set; }
        public PrototypeId StateB { get; protected set; }
    }

    public class StateSetPrototype : StateChangePrototype
    {
        public PrototypeId State { get; protected set; }
    }

    public class WeightedTextEntryPrototype : Prototype
    {
        public LocaleStringId Text { get; protected set; }
        public long Weight { get; protected set; }
    }

    public class EntityAppearancePrototype : Prototype
    {
        public EntityAppearanceEnum AppearanceEnum { get; protected set; }
    }

    public class EntityStatePrototype : Prototype
    {
        public PrototypeId Appearance { get; protected set; }
        public PrototypeId[] OnActivatePowers { get; protected set; }
        public BehaviorTreeNodePrototype OnActivateBehavior { get; protected set; }
        public AssetId OnActivateInterrupt { get; protected set; }
    }

    public class DoorEntityStatePrototype : EntityStatePrototype
    {
        public bool IsOpen { get; protected set; }
    }

    public class EntityBaseSpecPrototype : Prototype    // V10_NOTE: No InteractionSpecPrototype in 1.10?
    {
        public EntityFilterPrototype EntityFilter { get; protected set; }
    }

    public class EntityVisibilitySpecPrototype : EntityBaseSpecPrototype
    {
        public bool Visible { get; protected set; }
    }

    public class EntityAppearanceSpecPrototype : EntityBaseSpecPrototype
    {
        public PrototypeId Appearance { get; protected set; }
        public LocaleStringId FailureReasonText { get; protected set; }
        public TriBool InteractionEnabled { get; protected set; }
        public PrototypeId MapInfoPrototype { get; protected set; }
    }

    public class HotspotDirectApplyToMissilesDataPrototype : Prototype
    {
        public bool AffectsAllyMissiles { get; protected set; }
        public bool AffectsHostileMissiles { get; protected set; }
        public EvalPrototype EvalPropertiesToApply { get; protected set; }
    }

    public class HotspotPrototype : WorldEntityPrototype
    {
        public PrototypeId[] AppliesPowers { get; protected set; }
        public bool DuplicatePowers { get; protected set; }
        public PrototypeId[] AppliesIntervalPowers { get; protected set; }
        public int IntervalPowersTimeDelayMS { get; protected set; }
        public bool IntervalPowersRandomTarget { get; protected set; }
        public int IntervalPowersNumRandomTargets { get; protected set; }
        public UINotificationPrototype UINotificationOnEnter { get; protected set; }
        public int AppliesPowersNumTargets { get; protected set; }
        public bool KillCreatorWhenHotspotIsEmpty { get; protected set; }
        public PrototypeId KismetSeq { get; protected set; }
        public bool Negatable { get; protected set; }
        public bool KillSelfWhenPowerApplied { get; protected set; }
        public HotspotOverlapEventTriggerType OverlapEventsTriggerOn { get; protected set; }
        public int OverlapEventsMaxTargets { get; protected set; }
        public bool ForwardOnPowerHitProcsToOwner { get; protected set; }
        public HotspotDirectApplyToMissilesDataPrototype DirectApplyToMissilesData { get; protected set; }
        public int ApplyEffectsDelayMS { get; protected set; }
    }

    public class SpawnerSequenceEntryPrototype : Prototype
    {
        public PrototypeId Object { get; protected set; }
        public int Count { get; protected set; }
    }

    public class SpawnerPrototype : WorldEntityPrototype
    {
        public int SpawnLifetimeMax { get; protected set; }
        public int SpawnDistanceMin { get; protected set; }
        public int SpawnDistanceMax { get; protected set; }
        public int SpawnIntervalMS { get; protected set; }
        public int SpawnSimultaneousMax { get; protected set; }
        public PrototypeId SpawnedEntityInventory { get; protected set; }
        public SpawnMethod SpawnMethod { get; protected set; }
        public SpawnerSequenceEntryPrototype[] SpawnSequence { get; protected set; }
        public bool SpawnsInheritMissionPrototype { get; protected set; }
        public bool StartEnabled { get; protected set; }
        public bool SpawnsDestroyedOnDestroy { get; protected set; }
        public bool SpawnFailWithoutExactPosition { get; protected set; }
    }

    public class KismetSequenceEntityPrototype : WorldEntityPrototype
    {
        public PrototypeId KismetSequence { get; protected set; }
    }

    public class FactionPrototype : Prototype
    {
        public AssetId IconPath { get; protected set; }
        public PrototypeId TextStyle { get; protected set; }
        public AssetId HealthColor { get; protected set; }
    }
}
