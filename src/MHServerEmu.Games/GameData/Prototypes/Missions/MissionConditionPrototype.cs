using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes.Missions
{
    #region Enums

    [AssetEnum((int)None)]
    public enum OnInteractAction
    {
        None,
        Despawn,
        Disable,
    }

    #endregion

    public class MissionItemRequiredEntryPrototype : Prototype
    {
        public PrototypeId ItemPrototype { get; protected set; }
        public long Num { get; protected set; }
        public bool Remove { get; protected set; }
    }

    public class MissionConditionPrototype : Prototype
    {
        public StoryNotificationPrototype StoryNotification { get; protected set; }
        public bool NoTrackingOptimization { get; protected set; }
    }

    public class MissionPlayerConditionPrototype : MissionConditionPrototype
    {
        public bool PartyMembersGetCredit { get; protected set; }
    }

    public class MissionConditionListPrototype : MissionConditionPrototype
    {
        public MissionConditionPrototype[] Conditions { get; protected set; }
    }

    public class MissionConditionAndPrototype : MissionConditionListPrototype
    {
    }

    public class MissionConditionActiveChapterPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId Chapter { get; protected set; }
    }

    public class MissionConditionAreaEnterPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId RegionPrototype { get; protected set; }
        public PrototypeId AreaPrototype { get; protected set; }
    }

    public class MissionConditionAreaLeavePrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId AreaPrototype { get; protected set; }
    }

    public class MissionConditionAvatarIsUnlockedPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId AvatarPrototype { get; protected set; }
    }

    public class MissionConditionAvatarLevelUpPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId AvatarPrototype { get; protected set; }
        public long Level { get; protected set; }
    }

    public class MissionConditionAvatarUsedPowerPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId AreaPrototype { get; protected set; }
        public PrototypeId AvatarPrototype { get; protected set; }
        public PrototypeId PowerPrototype { get; protected set; }
        public PrototypeId RegionPrototype { get; protected set; }
        public PrototypeId WithinHotspot { get; protected set; }
    }

    public class MissionConditionCreditsCollectedPrototype : MissionPlayerConditionPrototype
    {
        public long NumCredits { get; protected set; }

        // V10_NOTE: Older version of MissionConditionCurrencyCollectedPrototype
    }

    public class MissionConditionEntityAggroPrototype : MissionPlayerConditionPrototype
    {
        public EntityFilterPrototype EntityFilter { get; protected set; }
    }

    public class MissionConditionEntityDamagedPrototype : MissionPlayerConditionPrototype
    {
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
        public AssetId EncounterResource { get; protected set; }
        public bool LimitToDamageFromPlayerOMOnly { get; protected set; }
    }

    public class MissionConditionEntityDeathPrototype : MissionPlayerConditionPrototype
    {
        public long Count { get; protected set; }
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
        public long OpenMissionContribValueDamage { get; protected set; }
        public long OpenMissionContribValueKill { get; protected set; }
        public long OpenMissionContribValueTanking { get; protected set; }
        public AssetId EncounterResource { get; protected set; }
        public int DelayDeathMS { get; protected set; }
    }

    public class MissionConditionEntityInteractPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId Cinematic { get; protected set; }
        public long Count { get; protected set; }
        public LocaleStringId DialogText { get; protected set; }
        public WeightedTextEntryPrototype[] DialogTextList { get; protected set; }
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
        public LootTablePrototype[] GiveItems { get; protected set; }
        public bool IsTurnInNPC { get; protected set; }
        public OnInteractAction OnInteract { get; protected set; }
        public long OpenMissionContributionValue { get; protected set; }
        public MissionItemRequiredEntryPrototype[] RequiredItems { get; protected set; }
        public PrototypeId WithinHotspot { get; protected set; }
        public PrototypeId OnInteractBehavior { get; protected set; }
        public MissionActionEntityTargetPrototype[] OnInteractEntityActions { get; protected set; }
        public IncrementalActionEntryPrototype[] OnIncrementalActions { get; protected set; }
        public LocaleStringId DialogTextWhenInventoryFull { get; protected set; }
    }

    public class MissionConditionFactionPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId Faction { get; protected set; }
        public bool EventOnly { get; protected set; }
    }

    public class MissionConditionLogicFalsePrototype : MissionConditionPrototype
    {
    }

    public class MissionConditionLogicTruePrototype : MissionConditionPrototype
    {
    }

    public class MissionConditionCompleteMissionPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId MissionPrototype { get; protected set; }

        // V10_NOTE: Older version of MissionConditionMissionCompletePrototype
    }

    public class MissionConditionCompleteObjectivePrototype : MissionPlayerConditionPrototype
    {
        public long ObjectiveID { get; protected set; }
        public PrototypeId MissionPrototype { get; protected set; }

        // V10_NOTE: Older version of MissionConditionObjectiveCompletePrototype
    }

    public class MissionConditionOrPrototype : MissionConditionListPrototype
    {
    }

    public class MissionConditionHealthRangePrototype : MissionPlayerConditionPrototype
    {
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
        public double HealthMinPct { get; protected set; }
        public double HealthMaxPct { get; protected set; }
    }

    public class MissionConditionHotspotEnterPrototype : MissionPlayerConditionPrototype
    {
        public EntityFilterFilterListPrototype TargetFilter { get; protected set; }
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
    }

    public class MissionConditionHotspotLeavePrototype : MissionPlayerConditionPrototype
    {
        public EntityFilterFilterListPrototype TargetFilter { get; protected set; }
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
    }

    public class MissionConditionItemCollectPrototype : MissionPlayerConditionPrototype
    {
        public long Count { get; protected set; }
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
        public long OpenMissionContributionValue { get; protected set; }
        public bool MustBeEquippableByAvatar { get; protected set; }
        public bool DestroyOnPickup { get; protected set; }
    }

    public class MissionConditionItemEquipPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId ItemPrototype { get; protected set; }
    }

    public class MissionConditionPowerEquipPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId PowerPrototype { get; protected set; }
    }

    public class MissionConditionRegionBeginTravelToPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId RegionPrototype { get; protected set; }
    }

    public class MissionConditionRegionEnterPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId RegionPrototype { get; protected set; }
    }

    public class MissionConditionRegionLeavePrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId RegionPrototype { get; protected set; }
    }

    public class MissionConditionRemoteNotificationPrototype : MissionPlayerConditionPrototype
    {
        public LocaleStringId DialogText { get; protected set; }
        public PrototypeId WorldEntityPrototype { get; protected set; }
        public GameNotificationType NotificationType { get; protected set; }
    }

    public class MissionConditionThrowablePickUpPrototype : MissionPlayerConditionPrototype
    {
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
    }

    public class MissionConditionRegionClearOfMobsPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId RegionPrototype { get; protected set; }
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }

        // V10_NOTE: Was this replaced by MissionConditionClusterEnemiesClearedPrototype / MissionConditionSpawnerDefeatedPrototype?
    }

    public class MissionConditionAreaClearOfMobsPrototype : MissionPlayerConditionPrototype
    {
        public PrototypeId AreaPrototype { get; protected set; }
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }

        // V10_NOTE: Was this replaced by MissionConditionClusterEnemiesClearedPrototype / MissionConditionSpawnerDefeatedPrototype?
    }
}
