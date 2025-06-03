using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)BiDirectional)]
    public enum RegionTransitionDirectionality  // Regions/RegionConnectionType.type
    {
        BiDirectional,
        OneWay,
        Disabled,
    }

    #endregion

    public class RegionPortalControlEntryPrototype : Prototype
    {
        public PrototypeId Region { get; protected set; }
        public int UnlockDurationMinutes { get; protected set; }
        public int UnlockPeriodMinutes { get; protected set; }
    }

    public class RegionConnectionTargetPrototype : Prototype
    {
        public PrototypeId Region { get; protected set; }
        public PrototypeId Area { get; protected set; }
        public AssetId Cell { get; protected set; }
        public PrototypeId Entity { get; protected set; }
        public PrototypeId IntroKismetSeq { get; protected set; }
        public LocaleStringId Name { get; protected set; }
    }

    public class RegionConnectionNodePrototype : Prototype
    {
        public PrototypeId Origin { get; protected set; }
        public PrototypeId Target { get; protected set; }
        public RegionTransitionDirectionality Type { get; protected set; }
    }
}
