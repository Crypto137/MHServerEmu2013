namespace MHServerEmu.Games.GameData.Prototypes
{
    public class LootRollModifierPrototype : Prototype
    {
    }

    public class LootRollClampMobLevelPrototype : LootRollModifierPrototype     // V10_NOTE: Older version of LootRollClampLevelPrototype
    {
        public int LevelMin { get; protected set; }
        public int LevelMax { get; protected set; }
    }

    public class LootRollMarkSpecialPrototype : LootRollModifierPrototype
    {
    }

    public class LootRollOffsetMobLevelPrototype : LootRollModifierPrototype    // V10_NOTE: Older version of LootRollOffsetLevelPrototype
    {
        public int LevelOffset { get; protected set; }
    }

    public class LootRollOnceDailyPrototype : LootRollModifierPrototype
    {
#if !BUILD_1_10_0_69
        public bool PerAccount { get; protected set; }
#endif
    }

    public class LootRollSetAvatarPrototype : LootRollModifierPrototype
    {
        public PrototypeId Avatar { get; protected set; }
    }

    public class LootRollSetItemLevelPrototype : LootRollModifierPrototype
    {
        public int Level { get; protected set; }
    }

    public class LootRollSetRarityPrototype : LootRollModifierPrototype
    {
        public PrototypeId[] Choices { get; protected set; }
    }

    public class LootRollSetUsablePrototype : LootRollModifierPrototype
    {
        public float Usable { get; protected set; }
    }

    public class LootRollUseMobLevelPrototype : LootRollModifierPrototype   // V10_NOTE: Older version of LootRollUseLevelVerbatimPrototype?
    {
        public bool UseMobLevel { get; protected set; }
    }
}
