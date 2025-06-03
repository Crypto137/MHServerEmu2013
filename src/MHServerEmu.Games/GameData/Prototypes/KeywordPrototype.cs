namespace MHServerEmu.Games.GameData.Prototypes
{
    public class EntityKeywordPrototype : Prototype
    {
    }

    public class MobKeywordPrototype : EntityKeywordPrototype
    {
    }

    public class AvatarKeywordPrototype : EntityKeywordPrototype
    {
    }

    public class PowerKeywordPrototype : Prototype
    {
        public LocaleStringId DisplayName { get; protected set; }
        public bool DisplayInPowerKeywordsList { get; protected set; }
    }
}
