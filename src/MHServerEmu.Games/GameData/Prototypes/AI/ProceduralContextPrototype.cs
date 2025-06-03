namespace MHServerEmu.Games.GameData.Prototypes.AI
{
    public class ProceduralContextPrototype : Prototype
    {
    }

    public class ProceduralUsePowerContextPrototype : ProceduralContextPrototype
    {
        public int InitialCooldownMS { get; protected set; }
        public int MaxCooldownMS { get; protected set; }
        public int MinCooldownMS { get; protected set; }
        public UsePowerContextPrototype PowerContext { get; protected set; }
        public int PickWeight { get; protected set; }
    }

#if !BUILD_1_10_0_69
    public class ProceduralUseAffixPowerContextPrototype : ProceduralContextPrototype
    {
        public int MaxCooldownMS { get; protected set; }
        public int MinCooldownMS { get; protected set; }
        public UseAffixPowerContextPrototype AffixContext { get; protected set; }
    }
#endif

    public class ProceduralFlankContextPrototype : ProceduralContextPrototype
    {
        public int MaxFlankCooldownMS { get; protected set; }
        public int MinFlankCooldownMS { get; protected set; }
        public FlankContextPrototype FlankContext { get; protected set; }
    }

    public class ProceduralFleeContextPrototype : ProceduralContextPrototype
    {
        public int MaxFleeCooldownMS { get; protected set; }
        public int MinFleeCooldownMS { get; protected set; }
        public FleeContextPrototype FleeContext { get; protected set; }
    }
}
