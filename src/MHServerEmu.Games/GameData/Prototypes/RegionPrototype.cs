using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.GameData.Prototypes.Generators;
using MHServerEmu.Games.Regions;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)Invalid)]
    public enum RegionBehavior
    {
        Invalid = -1,
        Town,
        PublicCombatZone,
        Private,
        MatchPlay,
    }

    [AssetEnum((int)None)]
    public enum MissionTrackerFilterType
    {
        None = -1,
        Standard,
        PvE,
        PvP,
        Daily,
        Challenge,
    }

    #endregion

    public class RegionPrototype : Prototype
    {
        public AssetId ClientMap { get; protected set; }
        public PrototypeId BodySliderTarget { get; protected set; }
        public PrototypeId StartTarget { get; protected set; }
        public ZoneLevelPrototype Level { get; protected set; }
        public AssetId[] Music { get; protected set; }
        public RegionGeneratorPrototype RegionGenerator { get; protected set; }
        public RegionBehavior Behavior { get; protected set; }
        public LocaleStringId RegionName { get; protected set; }
        public PrototypeId[] MetaGames { get; protected set; }
        public PrototypeId RespawnTarget { get; protected set; }
        public bool ForceSimulation { get; protected set; }
        public PrototypeId[] LoadingScreens { get; protected set; }
        public bool AlwaysRevealFullMap { get; protected set; }
        public PrototypeId Chapter { get; protected set; }
        public int PlayerLimit { get; protected set; }
        public float LifetimeInMinutes { get; protected set; }
        public PrototypeId WaypointAutoUnlock { get; protected set; }
        public bool PartyFormationAllowed { get; protected set; }
        public TransitionUIPrototype[] TransitionUITypes { get; protected set; }
        public AssetId AmbientSfx { get; protected set; }
        public PrototypeId[] PowerKeywordBlacklist { get; protected set; }
        public FactionLimitPrototype[] FactionLimits { get; protected set; }
        public bool CloseWhenReservationsReachesZero { get; protected set; }
        public bool Unreservable { get; protected set; }
        public float UIMapWallThickness { get; protected set; }
        public PrototypeId[] PopulationOverrides { get; protected set; }
        public int PopulationLevelOverride { get; protected set; }
        public int DifficultyIndex { get; protected set; }
        public MissionTrackerFilterType[] MissionTrackerFilterList { get; protected set; }
        public bool AllowAutoPartyOnEnter { get; protected set; }
        public float AutoPartyWindowSecs { get; protected set; }
        public PrototypeId DifficultyOverride { get; protected set; }
        public bool HardPlayerLimited { get; protected set; }
        public bool DailyCheckpointStartTarget { get; protected set; }
#if !BUILD_1_10_0_69
        public int LowPopulationPlayerLimit { get; protected set; }
#endif

        //---

        public PrototypeId GetDefaultAreaRef(Region region)
        {
            PrototypeId defaultArea = PrototypeId.Invalid;

            if (StartTarget != PrototypeId.Invalid)
            {
                var target = GameDatabase.GetPrototype<RegionConnectionTargetPrototype>(StartTarget);
                if (target != null)
                    defaultArea = target.Area;
            }

            if (RegionGenerator != null && defaultArea == PrototypeId.Invalid)
                return RegionGenerator.GetStartAreaRef(region); // TODO check return

            return defaultArea;
        }

        public static bool Equivalent(RegionPrototype regionA, RegionPrototype regionB)
        {
            // V10_NOTE: No alt regions in 1.10?
            if (regionA == null || regionB == null)
                return false;

            return regionA == regionB;
        }
    }

    public class FactionLimitPrototype : Prototype
    {
        public PrototypeId Faction { get; protected set; }
        public int PlayerLimit { get; protected set; }
    }

    public class ZoneLevelPrototype : Prototype
    {
    }

    public class ZoneLevelFixedPrototype : ZoneLevelPrototype
    {
        public short level { get; protected set; }
    }

    public class ZoneLevelRelativePrototype : ZoneLevelPrototype
    {
        public short modmax { get; protected set; }
        public short modmin { get; protected set; }
    }

    public class BlackOutZonePrototype : Prototype
    {
        public float BlackOutRadius { get; protected set; }
    }
}
