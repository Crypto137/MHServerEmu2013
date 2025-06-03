using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes.Missions
{
    #region Enums

    [AssetEnum((int)Invalid)]
    public enum MissionTimeExpiredResult        // Missions/Types/OnTimeExpired.type
    {
        Invalid,
        Complete,
        Fail,
    }

    [AssetEnum((int)Never)]
    public enum MissionShowInTracker            // Missions/Types/ShowInTracker.type
    {
        Never,
        IfObjectivesVisible,
        Always,
    }

    #endregion

    public class MissionGlobalsPrototype : Prototype
    {
        public int MissionLevelLowerBoundsOffset { get; protected set; }
        public int MissionLevelUpperBoundsOffset { get; protected set; }
        public CurveId OpenMissionContributionReward { get; protected set; }
        public PrototypeId InitialChapter { get; protected set; }
        public BannerMessagePrototype InventoryFullMessage { get; protected set; }
        public PrototypeId InitialStoryWarp { get; protected set; }
    }

    public class MissionItemDropEntryPrototype : Prototype
    {
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
        public PrototypeId LootTablePrototype { get; protected set; }
    }

    public class MissionPopulationEntryPrototype : Prototype
    {
        public long Count { get; protected set; }
        public Prototype Population { get; protected set; }
        public AssetId InitialTrigger { get; protected set; }
        public AssetId InitialVisibility { get; protected set; }
        public PrototypeId[] AreaList { get; protected set; }
        public PrototypeId Region { get; protected set; }
        public PrototypeId Area { get; protected set; }
    }

    public class MissionDialogTextPrototype : Prototype
    {
        public LocaleStringId Text { get; protected set; }
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
    }

    public class MissionObjectiveHintPrototype : Prototype
    {
        public EntityFilterPrototype PlayerStateFilter { get; protected set; }
        public EntityFilterPrototype TargetEntity { get; protected set; }
        public PrototypeId TargetArea { get; protected set; }
        public PrototypeId TargetRegion { get; protected set; }
    }

    public class MissionObjectivePrototype : Prototype
    {
        public MissionDialogTextPrototype[] DialogText { get; protected set; }
        public MissionConditionListPrototype FailureConditions { get; protected set; }
        public MissionItemDropEntryPrototype[] ItemDrops { get; protected set; }
        public bool ItemDropsCleanupRemaining { get; protected set; }
        public LocaleStringId Name { get; protected set; }
        public MissionActionPrototype[] OnFailActions { get; protected set; }
        public MissionActionPrototype[] OnStartActions { get; protected set; }
        public MissionActionPrototype[] OnSuccessActions { get; protected set; }
        public MissionConditionListPrototype ActivateConditions { get; protected set; }
        public MissionConditionListPrototype SuccessConditions { get; protected set; }
        public MissionTimeExpiredResult TimeExpiredResult { get; protected set; }
        public long TimeLimitSeconds { get; protected set; }
        public EntityBaseSpecPrototype[] InteractionsWhenActive { get; protected set; }
        public EntityBaseSpecPrototype[] InteractionsWhenComplete { get; protected set; }
        public LocaleStringId TextWhenCompleted { get; protected set; }
        public LocaleStringId TextWhenUpdated { get; protected set; }
        public bool ShowInMissionLog { get; protected set; }
        public bool Required { get; protected set; }
        public bool ShowNotificationIcon { get; protected set; }
        public bool Checkpoint { get; protected set; }
        public bool ShowInMissionTracker { get; protected set; }
        public LocaleStringId MissionLogAppendWhenActive { get; protected set; }
        public bool PlayerHUDShowObjsOnMap { get; protected set; }
        public bool PlayerHUDShowObjsOnMapNoPing { get; protected set; }
        public bool PlayerHUDShowObjsOnScreenEdge { get; protected set; }
        public bool PlayerHUDShowObjsOnEntity { get; protected set; }
        public long PlayerHUDObjectiveArrowDistOvrde { get; protected set; }
        public MissionObjectiveHintPrototype[] ObjectiveHints { get; protected set; }
        public bool ShowCountInUI { get; protected set; }
        public bool ShowTimerInUI { get; protected set; }
        public float Order { get; protected set; }
    }

    public class MissionNamedObjectivePrototype : MissionObjectivePrototype
    {
        public long ObjectiveID { get; protected set; }
    }

    public class OpenMissionRewardEntryPrototype : Prototype
    {
        public PrototypeId ChestEntity { get; protected set; }
        public double ContributionPercentage { get; protected set; }
        public PrototypeId[] Rewards { get; protected set; }
    }

    public class MissionPrototype : Prototype
    {
        public MissionConditionListPrototype ActivateConditions { get; protected set; }
        public PrototypeId Chapter { get; protected set; }
        public MissionDialogTextPrototype[] DialogText { get; protected set; }
        public MissionConditionListPrototype FailureConditions { get; protected set; }
        public long Level { get; protected set; }
        public LocaleStringId MissionLogDescription { get; protected set; }
        public LocaleStringId Name { get; protected set; }
        public MissionObjectivePrototype[] Objectives { get; protected set; }
        public MissionActionPrototype[] OnFailActions { get; protected set; }
        public MissionActionPrototype[] OnStartActions { get; protected set; }
        public MissionActionPrototype[] OnSuccessActions { get; protected set; }
        public MissionPopulationEntryPrototype[] PopulationSpawns { get; protected set; }
        public MissionConditionListPrototype PrereqConditions { get; protected set; }
        public MissionItemDropEntryPrototype[] PrereqItemDrops { get; protected set; }
        public bool Repeatable { get; protected set; }
        public LootTablePrototype[] Rewards { get; protected set; }
        public MissionTimeExpiredResult TimeExpiredResult { get; protected set; }
        public long TimeLimitSeconds { get; protected set; }
        public EntityBaseSpecPrototype[] InteractionsWhenActive { get; protected set; }
        public EntityBaseSpecPrototype[] InteractionsWhenComplete { get; protected set; }
        public LocaleStringId TextWhenActivated { get; protected set; }
        public LocaleStringId TextWhenCompleted { get; protected set; }
        public LocaleStringId TextWhenFailed { get; protected set; }
        public bool ShowInteractIndicators { get; protected set; }
        public bool ShowBannerMessages { get; protected set; }
        public bool ShowInMissionLog { get; protected set; }
        public bool ShowNotificationIcon { get; protected set; }
        public int SortOrder { get; protected set; }
        public MissionConditionListPrototype ActivateNowConditions { get; protected set; }
        public MissionShowInTracker ShowInMissionTracker { get; protected set; }
        public PrototypeId ResetsWithRegion { get; protected set; }
        public LocaleStringId MissionLogDescriptionComplete { get; protected set; }
        public bool PlayerHUDShowObjs { get; protected set; }
        public bool PlayerHUDShowObjsOnMap { get; protected set; }
        public bool PlayerHUDShowObjsOnMapNoPing { get; protected set; }
        public bool PlayerHUDShowObjsOnScreenEdge { get; protected set; }
        public bool PlayerHUDShowObjsOnEntity { get; protected set; }
        public bool PlayerHUDShowObjsNoActivateCond { get; protected set; }
        public MissionPerEntityRewardPrototype[] RewardsPerEntity { get; protected set; }
        public DesignWorkflowState DesignState { get; protected set; }
        public long ResetTimeSeconds { get; protected set; }
        public bool ShowInMissionTrackerFilterByChap { get; protected set; }
        public bool ShowMapPingOnPortals { get; protected set; }
        public bool PopulationRequired { get; protected set; }
        public bool ResetsOnStoryWarp { get; protected set; }
        public MissionTrackerFilterType ShowInMissionTrackerFilterType { get; protected set; }
        public int Version { get; protected set; }

        //---

        public override bool ApprovedForUse()
        {
            return GameDatabase.DesignStateOk(DesignState);
        }
    }

    public class OpenMissionPrototype : MissionPrototype
    {
        public PrototypeId Area { get; protected set; }
        public AssetId Cell { get; protected set; }
        public PrototypeId Hotspot { get; protected set; }
        public PrototypeId Region { get; protected set; }
        public OpenMissionRewardEntryPrototype[] RewardsByContribution { get; protected set; }
        public StoryNotificationPrototype StoryNotification { get; protected set; }
        public PrototypeId[] RegionList { get; protected set; }
        public PrototypeId[] AreaList { get; protected set; }
        public AssetId[] CellList { get; protected set; }
        public bool ResetWhenUnsimulated { get; protected set; }
        public double MinimumContributionForCredit { get; protected set; }
        public bool RespawnInPlace { get; protected set; }
        public double ParticipantTimeoutInSeconds { get; protected set; }
        public bool RespawnOnRestart { get; protected set; }
        public bool StaggerStartTime { get; protected set; }
    }

    public class MissionPerEntityRewardPrototype : Prototype
    {
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
        public LootTablePrototype[] Rewards { get; protected set; }
    }
}
