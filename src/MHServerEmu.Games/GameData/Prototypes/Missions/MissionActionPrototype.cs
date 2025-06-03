using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes.Missions
{
    #region Enums

    [AssetEnum((int)Participants)]
    public enum DistributionType
    {
        Participants,
        Contributors,
    }

    [AssetEnum((int)NoChange)]
    public enum EntityTriggerEnum
    {
        NoChange,
        Enabled,
        Disabled,
        Pulse,
    }

    #endregion

    public class MissionActionPrototype : Prototype
    {
    }

    public class IncrementalActionEntryPrototype : Prototype
    {
        public int TriggerCount { get; protected set; }
        public MissionActionPrototype[] Actions { get; protected set; }
    }

    public class MissionActionSetActiveChapterPrototype : MissionActionPrototype
    {
        public PrototypeId Chapter { get; protected set; }
    }

    public class MissionActionEncounterSpawnPrototype : MissionActionPrototype
    {
        public AssetId EncounterResource { get; protected set; }
        public int Phase { get; protected set; }
        public bool MissionSpawnOnly { get; protected set; }
    }

    public class MissionActionDifficultyOverridePrototype : MissionActionPrototype
    {
        public int DifficultyIncrement { get; protected set; }
        public int DifficultyIndex { get; protected set; }
        public PrototypeId DifficultyOverride { get; protected set; }
    }

    public class MissionActionEntityTargetPrototype : MissionActionPrototype
    {
        public EntityFilterFilterListPrototype EntityFilter { get; protected set; }
        public bool AllowWhenDead { get; protected set; }
    }

    public class MissionActionEntityCreatePrototype : MissionActionPrototype
    {
        public PrototypeId EntityPrototype { get; protected set; }
    }

    public class MissionActionEntityDestroyPrototype : MissionActionEntityTargetPrototype
    {
    }

    public class MissionActionEntityKillPrototype : MissionActionEntityTargetPrototype
    {
    }

    public class MissionActionEntityPerformPowerPrototype : MissionActionEntityTargetPrototype
    {
        public PrototypeId PowerPrototype { get; protected set; }
        public bool PowerRemove { get; protected set; }
        public PrototypeId BrainOverride { get; protected set; }
        public bool BrainOverrideRemove { get; protected set; }
    }

    public class MissionActionEntitySetStatePrototype : MissionActionEntityTargetPrototype
    {
        public PrototypeId EntityState { get; protected set; }
        public TriBool Interactable { get; protected set; }
    }

    public class MissionActionFactionSetPrototype : MissionActionPrototype
    {
        public PrototypeId Faction { get; protected set; }
        public DistributionType SendTo { get; protected set; }
    }

    public class MissionActionSpawnerTriggerPrototype : MissionActionEntityTargetPrototype
    {
        public EntityTriggerEnum Trigger { get; protected set; }
    }

    public class MissionActionInventoryGiveAvatarPrototype : MissionActionPrototype
    {
        public PrototypeId AvatarPrototype { get; protected set; }
    }

    public class MissionActionInventoryRemoveItemPrototype : MissionActionPrototype
    {
        public PrototypeId ItemPrototype { get; protected set; }
        public long Count { get; protected set; }
        public MissionActionPrototype[] OnRemoveActions { get; protected set; }
    }

    public class MissionActionMissionActivatePrototype : MissionActionPrototype
    {
        public PrototypeId MissionPrototype { get; protected set; }
    }

    public class MissionActionRegionShutdownPrototype : MissionActionPrototype
    {
        public PrototypeId RegionPrototype { get; protected set; }
    }

    public class MissionActionResetAllMissionsPrototype : MissionActionPrototype
    {
    }

    public class MissionActionTimedActionPrototype : MissionActionPrototype
    {
        public MissionActionPrototype[] ActionsToPerform { get; protected set; }
        public double DelayInSeconds { get; protected set; }
        public bool Repeat { get; protected set; }
    }

    public class MissionActionStoryNotificationPrototype : MissionActionPrototype
    {
        public StoryNotificationPrototype StoryNotification { get; protected set; }
        public DistributionType SendTo { get; protected set; }
    }

    public class MissionActionShowBannerMessagePrototype : MissionActionPrototype
    {
        public BannerMessagePrototype BannerMessage { get; protected set; }
        public DistributionType SendTo { get; protected set; }
    }

    public class MissionActionShowMotionComicPrototype : MissionActionPrototype
    {
        public PrototypeId MotionComic { get; protected set; }
        public PrototypeId DownloadChunkOverride { get; protected set; }
        public DistributionType SendTo { get; protected set; }
    }

    public class MissionActionShowOverheadTextPrototype : MissionActionEntityTargetPrototype
    {
        public LocaleStringId DisplayText { get; protected set; }
        public long DurationMS { get; protected set; }
    }

    public class MissionActionWaypointUnlockPrototype : MissionActionPrototype
    {
        public PrototypeId WaypointToUnlock { get; protected set; }
    }

    public class MissionActionWaypointLockPrototype : MissionActionPrototype
    {
        public PrototypeId WaypointToLock { get; protected set; }
    }

    public class MissionActionPlayBanterPrototype : MissionActionPrototype
    {
        public AssetId BanterAsset { get; protected set; }
        public DistributionType SendTo { get; protected set; }
    }

    public class MissionActionPlayKismetSeqPrototype : MissionActionPrototype
    {
        public PrototypeId KismetSeqPrototype { get; protected set; }
        public DistributionType SendTo { get; protected set; }
    }
}
