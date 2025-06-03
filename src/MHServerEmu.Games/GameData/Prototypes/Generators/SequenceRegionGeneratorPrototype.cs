using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes.Generators
{
    #region Enums

    [AssetEnum((int)NoRestriction)]
    [Flags]
    public enum RegionDirection
    {
        NoRestriction = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8,
        NorthSouth = 5,
        EastWest = 10,
    }

    #endregion

    public class SequenceRegionGeneratorPrototype : RegionGeneratorPrototype
    {
        public AreaSequenceInfoPrototype[] AreaSequence { get; protected set; }
    }

    public class AreaSequenceInfoPrototype : Prototype
    {
        public WeightedAreaPrototype[] AreaChoices { get; protected set; }
        public AreaSequenceInfoPrototype[] ConnectedTo { get; protected set; }
        public short ConnectedToPicks { get; protected set; }
        public bool ConnectAllShared { get; protected set; }
        public short SharedEdgeMinimum { get; protected set; }
        public short Weight { get; protected set; }
    }

    public class WeightedAreaPrototype : Prototype
    {
        public PrototypeId Area { get; protected set; }
        public int Weight { get; protected set; }
        public RegionDirection ConnectOn { get; protected set; }
    }
}
