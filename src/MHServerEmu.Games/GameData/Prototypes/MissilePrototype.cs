using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)Invalid)]
    public enum MissilePowerActivationEventType    // Powers/Types/MissilePowerActivationEvent.type
    {
        Invalid,
        OnCollide,
        OnLifespanExpired,
        OnReturned,
        OnReturning,
        OnOutOfWorld,
    }

    [AssetEnum((int)Forward)]
    public enum MissileInitialDirectionType
    {
        Forward,
        Backward,
        Left,
        Right,
        Up,
        OwnersForward,
    }

    [AssetEnum((int)InFrontOfOwner)]
    public enum MissileSpawnLocationType
    {
        CenteredOnOwner,
        InFrontOfOwner,
    }

    #endregion

    public class MissilePrototype : AgentPrototype
    {
    }

    public class MissilePowerContextPrototype : Prototype
    {
        public PrototypeId Power { get; protected set; }
        public MissilePowerActivationEventType MissilePowerActivationEvent { get; protected set; }
        public EvalPrototype EvalPctChanceToActivate { get; protected set; }
    }

    public class MissileCreationContextPrototype : Prototype
    {
        public bool IndependentClientMovement { get; protected set; }
        public bool IsReturningMissile { get; protected set; }
        public bool ReturningMissileExplodeOnCollide { get; protected set; }
        public bool MissileOneShot { get; protected set; }
        public PrototypeId MissileEntity { get; protected set; }
        public Vector3Prototype CreationOffset { get; protected set; }
        public float MissileSizeIncreasePerSec { get; protected set; }
        public bool MissileIgnoresPitch { get; protected set; }
        public float MissileRadius { get; protected set; }
        public MissileInitialDirectionType InitialDirection { get; protected set; }
        public Rotator3Prototype InitialDirectionAxisRotation { get; protected set; }
        public Rotator3Prototype InitialDirectionRandomVariance { get; protected set; }
        public MissileSpawnLocationType SpawnLocation { get; protected set; }
        public bool MissileInterpolateRotationSpeed { get; protected set; }
        public float MissileInterpolateRotMultByDist { get; protected set; }
        public float MissileInterpolateOvershotAccel { get; protected set; }
        public bool NoCollide { get; protected set; }
        public MissilePowerContextPrototype[] MissilePowerList { get; protected set; }
        public bool ReturnWeaponOnlyOnMiss { get; protected set; }
        public float MissileRadiusEffectOverride { get; protected set; }
    }

    public class MissilePowerPrototype : PowerPrototype
    {
        public float MissileSweepTestRadius { get; protected set; }
        public MissileCreationContextPrototype[] MissileCreationContexts { get; protected set; }
        public bool MissileAllowCreationAfterPwrEnds { get; protected set; }
        public bool MissileUsesActualTargetPos { get; protected set; }
    }
}
