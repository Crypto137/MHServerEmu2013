namespace MHServerEmu.Games.GameData.Prototypes
{
    public class PropPrototype : AgentPrototype
    {
    }

    public class PropDensityEntryPrototype : Prototype
    {
        public PrototypeId Marker { get; protected set; }
        public long OverrideDensity { get; protected set; }
    }

    public class PropDensityPrototype : Prototype
    {
        public long DefaultDensity { get; protected set; }
        public PropDensityEntryPrototype[] MarkerDensityOverrides { get; protected set; }
    }

    public class DestructiblePropPrototype : PropPrototype
    {
    }

    // V10_NOTE: No smart props in 1.10
}
