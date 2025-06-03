namespace MHServerEmu.Games.GameData.Prototypes
{
    public class LootLocationModifierPrototype : Prototype
    {
    }

    public class LootSearchRadiusPrototype : LootLocationModifierPrototype
    {
        public float MinRadius { get; protected set; }
        public float MaxRadius { get; protected set; }
    }

    public class LootBoundsOverridePrototype : LootLocationModifierPrototype
    {
        public float Radius { get; protected set; }
    }

    public class LootLocationOffsetPrototype : LootLocationModifierPrototype
    {
        public float Offset { get; protected set; }
    }

    #region 1.10 Prototypes

    public class EntityBoundsCheckPrototype : LootLocationModifierPrototype
    {
        public bool Check { get; protected set; }
    }

    public class CanPathToCheckPrototype : LootLocationModifierPrototype
    {
        public bool Check { get; protected set; }
    }

    public class FailSafeCheckPrototype : LootLocationModifierPrototype
    {
        public bool Check { get; protected set; }
    }

    public class LineOfSightCheckPrototype : LootLocationModifierPrototype
    {
        public bool Check { get; protected set; }
    }

    #endregion
}
