namespace MHServerEmu.Games.Entities.PowerCollections
{
    public readonly struct PowerIndexProperties
    {
        // V10_NOTE: No CombatLevel or ItemVariation in 1.10.
        public int PowerRank { get; } = 0;
        public int CharacterLevel { get; } = 1;
        public int ItemLevel { get; } = 1;

        public PowerIndexProperties(int powerRank = 0, int characterLevel = 1, int itemLevel = 1)
        {
            PowerRank = powerRank;
            CharacterLevel = characterLevel;
            ItemLevel = itemLevel;
        }

        public override string ToString()
        {
            return $"{nameof(PowerRank)}={PowerRank}, {nameof(CharacterLevel)}={CharacterLevel}, {nameof(ItemLevel)}={ItemLevel}";
        }
    }
}
