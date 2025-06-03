using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)Tab1Elements)]
    public enum CraftingRecipeCategory
    {
        Tab1Elements,
        Tab2Costumes,
        Tab3Gear,
        Tab4Misc,
    }

    #endregion

    public class CraftingInputPrototype : Prototype
    {
        public PrototypeId SlotDisplayInfo { get; protected set; }
        public bool OnlyUsableItems { get; protected set; }
    }

    public class AvatarLookupInputPrototype : CraftingInputPrototype
    {
        public int AllowedIngredientLookup { get; protected set; }
        public int Rank { get; protected set; }
    }

    public class MatchOtherInputPrototype : CraftingInputPrototype
    {
        public int Index { get; protected set; }
    }

    public class RestrictionSetInputPrototype : CraftingInputPrototype
    {
        public DropRestrictionPrototype[] Restrictions { get; protected set; }
    }

    public class SpecificItemInputPrototype : CraftingInputPrototype    // V10_NOTE: Older version of AllowedItemListInputPrototype
    {
        public PrototypeId[] AllowedItems { get; protected set; }
    }

    public class CraftingIngredientPrototype : ItemPrototype
    {
        public PrototypeId AvatarLookupCraftSlotDisplayInfo { get; protected set; }
    }

    public class CostumeCorePrototype : CraftingIngredientPrototype
    {
    }

    public class CraftingRecipePrototype : ItemPrototype
    {
        public float CraftingTimeMins { get; protected set; }
        public CraftingInputPrototype[] RecipeInputs { get; protected set; }
        public LootTablePrototype RecipeOutput { get; protected set; }
        public LocaleStringId RecipeDescription { get; protected set; }
        public AssetId RecipeIconPath { get; protected set; }
        public int SortOrder { get; protected set; }
        public CraftingRecipeCategory RecipeCategory { get; protected set; }
        public EvalPrototype CostEval { get; protected set; }
        public LocaleStringId RecipeTooltip { get; protected set; }
    }
}
