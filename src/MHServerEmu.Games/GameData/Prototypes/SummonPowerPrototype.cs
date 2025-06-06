﻿namespace MHServerEmu.Games.GameData.Prototypes
{
    public class SummonPowerPrototype : PowerPrototype
    {
        public bool AttachSummonsToTarget { get; protected set; }
        public bool SummonsLiveWhilePowerActive { get; protected set; }
        public SummonEntityContextPrototype[] SummonEntityContexts { get; protected set; }
        public int SummonMax { get; protected set; }
        public bool SummonMaxReachedDestroyOwner { get; protected set; }
        public int SummonIntervalMS { get; protected set; }
        public bool SummonRandomSelection { get; protected set; }
        public bool TrackInInventory { get; protected set; }
    }

    public class SummonPowerOverridePrototype : PowerUnrealOverridePrototype
    {
        public PrototypeId SummonEntity { get; protected set; }
    }

    public class SummonRemovalPrototype : Prototype
    {
        public PrototypeId[] FromPowers { get; protected set; }
        public PrototypeId[] Keywords { get; protected set; }
    }

    public class SummonEntityContextPrototype : Prototype
    {
        public PrototypeId SummonEntity { get; protected set; }
        public bool AbandonedOnceSummoned { get; protected set; }
        public AssetId PathFilterOverride { get; protected set; }
        public bool RandomSpawnLocation { get; protected set; }
        public bool IgnoreBlockingOnSpawn { get; protected set; }
        public bool SnapToFloor { get; protected set; }
        public bool TransferMissionPrototype { get; protected set; }
        public float SummonRadius { get; protected set; }
        public bool EnforceExactSummonPos { get; protected set; }
        public bool ForceBlockingCollisionForSpawn { get; protected set; }
        public bool VisibleWhileAttached { get; protected set; }
        public Vector3Prototype SummonOffsetVector { get; protected set; }
        public SummonRemovalPrototype SummonEntityRemoval { get; protected set; }
        public EvalPrototype[] EvalOnSummon { get; protected set; }
        public float SummonOffsetAngle { get; protected set; }
        public bool HideEntityOnSummon { get; protected set; }
        public bool CopyOwnerProperties { get; protected set; }
    }
}
