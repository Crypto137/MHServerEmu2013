namespace MHServerEmu.Games.GameData.Prototypes
{
    public class LootNodePrototype : Prototype
    {
        public short Weight { get; protected set; }
        public LootRollModifierPrototype[] Modifiers { get; protected set; }
    }

    public class LootDropPrototype : LootNodePrototype
    {
        public short Num { get; protected set; }
    }

    public class LootTablePrototype : LootDropPrototype
    {
        public PickMethod PickMethod { get; protected set; }
        public float NoDropPercent { get; protected set; }
        public LootNodePrototype[] Choices { get; protected set; }
    }

    public class LootTableAssignmentPrototype : Prototype
    {
        public AssetId Name { get; protected set; }
        public PrototypeId Table { get; protected set; }
    }
}
