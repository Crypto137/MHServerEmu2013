using MHServerEmu.Games.Properties;

namespace MHServerEmu.Games.GameData.Prototypes
{
    public class PropertyPrototype : Prototype
    {
    }

    public class PropertyPickInRangeEntryPrototype : Prototype
    {
        public PropertyId Prop { get; protected set; }
        public EvalPrototype ValueMax { get; protected set; }
        public EvalPrototype ValueMin { get; protected set; }
        public bool RollAsInteger { get; protected set; }
        public LocaleStringId TooltipOverrideText { get; protected set; }
    }

    public class PropertyBasedBonusPrototype : Prototype
    {
        public PrototypeId Property { get; protected set; }
        public float Multiplier { get; protected set; }
    }

    public class PropertyFormulaPrototype : Prototype
    {
    }

    public class BasePlusBonusesFormulaPrototype : PropertyFormulaPrototype
    {
        public PrototypeId BaseProperty { get; protected set; }
        public PrototypeId BonusAddProperty { get; protected set; }
        public PrototypeId BonusPctProperty { get; protected set; }
        public PrototypeId MissingHealthBonusPct { get; protected set; }
        public PrototypeId MissingEnduranceBonusPct { get; protected set; }
        public PropertyBasedBonusPrototype[] MiscPropertyBonuses { get; protected set; }
        public PrototypeId MagnitudeProperty { get; protected set; }
    }

    public class SkillFormulaPrototype : PropertyFormulaPrototype
    {
        public PrototypeId BaseProperty { get; protected set; }
        public PrototypeId BonusAddProperty { get; protected set; }
        public PrototypeId TempProperty { get; protected set; }
    }
}
