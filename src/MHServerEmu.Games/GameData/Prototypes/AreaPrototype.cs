using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.GameData.Prototypes.Generators;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)Standard)]
    public enum AreaMinimapReveal
    {
        Standard,
        PlayerAreaOnly,
        PlayerCellOnly,
        PlayerAreaGroup,
    }

    #endregion

    public class AreaPrototype : Prototype
    {
        public GeneratorPrototype Generator { get; protected set; }
        public ZoneLevelPrototype Level { get; protected set; }
        public PrototypeId Population { get; protected set; }
        public LocaleStringId AreaName { get; protected set; }
        public PrototypeId PropDensity { get; protected set; }
        public AssetId[] PropSets { get; protected set; }
        public StyleEntryPrototype[] Styles { get; protected set; }
        public AssetId ClientMap { get; protected set; }
        public AssetId[] Music { get; protected set; }
        public bool FullyGenerateCells { get; protected set; }
        public AreaMinimapReveal MinimapRevealMode { get; protected set; }
        public LootTableAssignmentPrototype[] LootTables { get; protected set; }
        public AssetId AmbientSfx { get; protected set; }
        public LocaleStringId MinimapName { get; protected set; }
        public int MinimapRevealGroupId { get; protected set; }
    }

    public class AreaTransitionPrototype : Prototype
    {
        public AssetId Type { get; protected set; }
    }

    public class StyleEntryPrototype : Prototype
    {
        public PrototypeId Population { get; protected set; }
        public AssetId[] PropSets { get; protected set; }
        public int Weight { get; protected set; }
    }

    public class AreaListPrototype : Prototype
    {
        public PrototypeId[] Areas { get; protected set; }
    }
}
