namespace MHServerEmu.Games.GameData.Prototypes
{
#if !BUILD_1_10_0_69
    public class ItemStackPrototype : Prototype     // V10_NOTE: Older version of ItemCostItemStackPrototype
    {
        public PrototypeId ItemPrototype { get; protected set; }
        public EvalPrototype Number { get; protected set; }
    }

    public class ItemCostPrototype : Prototype
    {
        public ItemStackPrototype[] ItemStacks { get; protected set; }
    }
#endif
}
