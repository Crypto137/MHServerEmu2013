namespace MHServerEmu.Games.GameData.Prototypes
{
    public class TooltipTranslationEntryPrototype : Prototype   // V10_NOTE: Older version of PowerTooltipEntryPrototype
    {
        public LocaleStringId TokenSourcePrefix { get; protected set; }
        public PrototypeId Translation { get; protected set; }
    }

    public class TooltipSectionPrototype : Prototype
    {
        public PrototypeId Style { get; protected set; }
        public LocaleStringId Text { get; protected set; }
        public bool ShowOnlyIfPreviousSectionHasText { get; protected set; }
        public AssetId AlignToPreviousSection { get; protected set; }
        public bool IsDivider { get; protected set; }
        public AssetId Font { get; protected set; }
        public bool ShowOnlyIfNextSectionHasText { get; protected set; }
    }

    public class TooltipTemplatePrototype : Prototype
    {
        public TooltipSectionPrototype[] TooltipSectionList { get; protected set; }
    }
}
