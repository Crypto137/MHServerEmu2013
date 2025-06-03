namespace MHServerEmu.Games.GameData.Prototypes.Generators
{
    public class StaticRegionGeneratorPrototype : RegionGeneratorPrototype
    {
        public StaticAreaPrototype[] StaticAreas { get; protected set; }
        public AreaConnectionPrototype[] Connections { get; protected set; }
    }

    public class AreaConnectionPrototype : Prototype
    {
        public PrototypeId AreaA { get; protected set; }
        public PrototypeId AreaB { get; protected set; }
        public bool ConnectAllShared { get; protected set; }
    }

    public class StaticAreaPrototype : Prototype
    {
        public PrototypeId Area { get; protected set; }
        public long X { get; protected set; }
        public long Y { get; protected set; }
        public long Z { get; protected set; }
    }
}
