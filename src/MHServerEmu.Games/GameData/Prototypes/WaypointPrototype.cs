namespace MHServerEmu.Games.GameData.Prototypes
{
    public class WaypointPrototype : Prototype
    {
        public PrototypeId Destination { get; protected set; }
        public LocaleStringId Name { get; protected set; }
        public bool SupressBannerMessage { get; protected set; }
        public bool IsCheckpoint { get; protected set; }
        public PrototypeId WaypointGraph { get; protected set; }
        public PrototypeId RequiresItem { get; protected set; }
#if !BUILD_1_10_0_69
        public LocaleStringId Tooltip { get; protected set; }
#endif
    }

    public class WaypointActPrototype : Prototype
    {
        public PrototypeId Act { get; protected set; }
        public WaypointChapterPrototype[] Chapters { get; protected set; }
    }

    public class WaypointChapterPrototype : Prototype
    {
        public PrototypeId Chapter { get; protected set; }
        public PrototypeId[] Waypoints { get; protected set; }
    }

    public class WaypointGraphPrototype : Prototype
    {
        public WaypointActPrototype[] Acts { get; protected set; }
    }

    public class CheckpointPrototype : Prototype
    {
        public PrototypeId Destination { get; protected set; }
    }
}
