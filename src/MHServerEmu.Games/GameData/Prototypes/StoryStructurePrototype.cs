namespace MHServerEmu.Games.GameData.Prototypes
{
    public class ActPrototype : Prototype
    {
        public LocaleStringId ActName { get; protected set; }
        public LocaleStringId ActTooltip { get; protected set; }
    }

    public class ChapterPrototype : Prototype
    {
        public LocaleStringId ChapterName { get; protected set; }
        public int ChapterNumber { get; protected set; }
        public LocaleStringId ChapterTooltip { get; protected set; }
        public PrototypeId ParentAct { get; protected set; }
        public bool IsDevOnly { get; protected set; }
        public PrototypeId HubWaypoint { get; protected set; }
        public bool ShowInShippingUI { get; protected set; }
        public LocaleStringId Description { get; protected set; }
        public int RecommendedLevelMin { get; protected set; }
        public int RecommendedLevelMax { get; protected set; }
        public bool ResetsOnStoryWarp { get; protected set; }
    }

    public class StoryWarpPrototype : Prototype
    {
        public PrototypeId Chapter { get; protected set; }
        public PrototypeId Waypoint { get; protected set; }
    }
}
