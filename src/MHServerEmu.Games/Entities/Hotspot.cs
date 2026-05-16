using MHServerEmu.Core.Logging;
using MHServerEmu.Games.GameData.Prototypes;

namespace MHServerEmu.Games.Entities
{
    public class Hotspot : WorldEntity
    {
        public HotspotPrototype HotspotPrototype { get => Prototype as HotspotPrototype; }

        public Hotspot(Game game) : base(game)
        {
        }

        public override bool Initialize(EntitySettings settings)
        {
            if (!Verify.IsTrue(base.Initialize(settings))) return false;

            HotspotPrototype hotspotProto = HotspotPrototype;
            if (!Verify.IsNotNull(hotspotProto)) return false;

            if (!Verify.IsNotNull(GetPowerCollectionAllocateIfNull(hotspotProto.DuplicatePowers))) return false;

            return true;
        }
    }
}
