using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)Force)]
    public enum ResourceType
    {
        Force,
        Focus,
        Fury,
        Secondary_Pips,
        Secondary_Gauge,
    }

    #endregion

    public class ManaBehaviorPrototype : Prototype
    {
        public LocaleStringId DisplayName { get; protected set; }
        public ResourceType MeterType { get; protected set; }
        public PrototypeId[] Powers { get; protected set; }
        public bool StartsEmpty { get; protected set; }
        public LocaleStringId Description { get; protected set; }
    }

    public class SecondaryResourceManaBehaviorPrototype : ManaBehaviorPrototype
    {
        public EvalPrototype EvalGetCurrentForDisplay { get; protected set; }
        public EvalPrototype EvalGetCurrentPipsForDisplay { get; protected set; }
        public EvalPrototype EvalGetMaxForDisplay { get; protected set; }
        public EvalPrototype EvalGetMaxPipsForDisplay { get; protected set; }
        public bool DepleteOnDeath { get; protected set; }
    }
}
