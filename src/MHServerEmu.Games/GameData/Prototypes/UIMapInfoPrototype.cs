namespace MHServerEmu.Games.GameData.Prototypes
{
    public class UIMapInfoIconBehaviorPrototype : Prototype
    {
        public AssetId HighlightEffect { get; protected set; }
        public AssetId IconPath { get; protected set; }
        public bool Orient { get; protected set; }
    }

    public class UIMapInfoIconAppearancePrototype : Prototype
    {
        public PrototypeId IconOnScreen { get; protected set; }
        public PrototypeId IconOffScreen { get; protected set; }
    }

    public class UIMapInfoPrototype : Prototype     // V10_NOTE: Older version of ObjectiveInfoPrototype
    {
        public bool TrackAfterDiscovery { get; protected set; }
        public bool IsEnabled { get; protected set; }
        public bool UseScreenPointer { get; protected set; }
        public PrototypeId IconAppearance { get; protected set; }
        public int RenderPriority { get; protected set; }
        public int ScreenPointerRange { get; protected set; }
        public bool ScreenPointerOnlyInArea { get; protected set; }
        public bool ShowToSummonerOnly { get; protected set; }
    }
}
