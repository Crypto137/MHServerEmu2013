using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.GameData.Prototypes.AI;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)FaceRandomFull)]
    public enum FacingMethod
    {
        FaceRandomFull,
        FaceRandomSlight,
        FacePlayer,
        FacePlayerSlight,
    }

    [AssetEnum((int)NoRestriction)]
    public enum AreaDepth
    {
        NoRestriction,
        Near,
        Midway,
        Far,
        Max,
    }

    #endregion

    public class PopulationObjectPrototype : PopulatablePrototype
    {
        public PrototypeId AllianceOverride { get; protected set; }
        public PrototypeId[] DifficultyRestriction { get; protected set; }
        public TriBool Respawn { get; protected set; }
        public float LeashDistance { get; protected set; }
        public float RespawnCooldown { get; protected set; }
        public int Weight { get; protected set; }
        public int MaxPerCell { get; protected set; }
        public int MaxPerArea { get; protected set; }
        public BehaviorInterruptType InitialBehaviorInterruptOverride { get; protected set; }
        public FacingMethod Facing { get; protected set; }
        public AreaDepth SpawnDepth { get; protected set; }
        public PrototypeId UsePopulationMarker { get; protected set; }
        public PopulationRiderPrototype[] Riders { get; protected set; }
        public PrototypeId RankOverrideIfModified { get; protected set; }
        public bool IgnoreEntityNaviCheck { get; protected set; }
        public bool IgnoreBlackout { get; protected set; }
        public bool UseMarkerOrientation { get; protected set; }
        public bool AllowCrossMissionHostility { get; protected set; }
        public int EnemyBoostCountOverride { get; protected set; }
    }

    public class PopulationEntityPrototype : PopulationObjectPrototype
    {
        public PrototypeId Entity { get; protected set; }
    }

    public class PopulationClusterPrototype : PopulationObjectPrototype
    {
        public short Max { get; protected set; }
        public short Min { get; protected set; }
        public float RandomOffset { get; protected set; }
        public PrototypeId Entity { get; protected set; }
    }

    public class PopulationClusterMixedPrototype : PopulationObjectPrototype
    {
        public short Max { get; protected set; }
        public short Min { get; protected set; }
        public float RandomOffset { get; protected set; }
        public PopulationEntityPrototype[] Choices { get; protected set; }
    }

    public class PopulationLeaderPrototype : PopulationObjectPrototype
    {
        public PrototypeId Leader { get; protected set; }
        public PopulationObjectPrototype[] Henchmen { get; protected set; }
    }

    public class PopulationEncounterPrototype : PopulationObjectPrototype
    {
        public AssetId EncounterResource { get; protected set; }
    }

    public class PopulationGroupPrototype : PopulationObjectPrototype
    {
        public PopulationObjectPrototype[] EntitiesAndGroups { get; protected set; }
    }

    public class PopulationRiderPrototype : Prototype
    {
    }

    public class PopulationRiderEntityPrototype : PopulationRiderPrototype
    {
        public PrototypeId Entity { get; protected set; }
    }

    public class PopulationRiderBlackOutPrototype : PopulationRiderPrototype
    {
        public PrototypeId BlackOutZone { get; protected set; }
    }

    public class PopulationRequiredObjectPrototype : Prototype
    {
        public PrototypeId Object { get; protected set; }
        public short Count { get; protected set; }
    }

    public class PopulationRequiredObjectListPrototype : Prototype
    {
        public PopulationRequiredObjectPrototype[] RequiredObjects { get; protected set; }
    }

    public class PopulationListTagObjectPrototype : Prototype
    {
    }

    public class PopulationListTagEncounterPrototype : Prototype
    {
    }

    public class PopulationListTagThemePrototype : Prototype
    {
    }
}
