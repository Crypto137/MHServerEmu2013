using MHServerEmu.Games.Regions;

namespace MHServerEmu.Games.GameData.Prototypes.Generators
{
    public class RegionGeneratorPrototype : Prototype
    {
        public PrototypeId[] POIGroups { get; protected set; }

        //---

        public virtual PrototypeId GetStartAreaRef(Region region) { return PrototypeId.Invalid; }
    }
}
