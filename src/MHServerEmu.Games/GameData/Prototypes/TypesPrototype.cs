using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)Undefined)]
    public enum TriBool
    {
        Undefined = -1,
        False = 0,
        True = 1,
    }

    #endregion

    public class Vector3Prototype : Prototype
    {
        public float X { get; protected set; }
        public float Y { get; protected set; }
        public float Z { get; protected set; }
    }

    public class Rotator3Prototype : Prototype
    {
        public float Yaw { get; protected set; }
        public float Pitch { get; protected set; }
        public float Roll { get; protected set; }
    }

    public class ContextPrototype : Prototype
    {
    }

    public class TranslationPrototype : Prototype
    {
        public LocaleStringId Value { get; protected set; }
    }

    public class LocomotorPrototype : Prototype
    {
        public float Height { get; protected set; }
        public float Speed { get; protected set; }
        public float RotationSpeed { get; protected set; }
        public bool WalkEnabled { get; protected set; }
        public float WalkSpeed { get; protected set; }
        public bool Immobile { get; protected set; }
    }
}
