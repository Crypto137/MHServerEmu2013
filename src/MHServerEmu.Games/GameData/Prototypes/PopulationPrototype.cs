﻿using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region MarkerType

    [AssetEnum((int)Invalid)]
    public enum MarkerType  // SpawnMarkers/PopulationType.type? Doesn't match exactly
    {
        Invalid,
        Enemies,    // Officer / Trash
        Encounter,
        QuestGiver,
        Transition,
        Prop,
    }

    #endregion

    public class PopulationPrototype : Prototype
    {
        public PrototypeId RespawnMethod { get; protected set; }
        public float ClusterDensityPct { get; protected set; }
        public float ClusterDensityPeak { get; protected set; }
        public float EncounterDensityBase { get; protected set; }
        public float SpawnMapDensityMin { get; protected set; }
        public float SpawnMapDensityMax { get; protected set; }
        public float SpawnMapDensityStep { get; protected set; }
        public int SpawnMapHeatReturnPerSecond { get; protected set; }
        public EvalPrototype SpawnMapHeatReturnPerSecondEval { get; protected set; }
        public float SpawnMapHeatBleed { get; protected set; }
        public float SpawnMapCrowdSupression { get; protected set; }
        public int SpawnMapCrowdSupressionStart { get; protected set; }
        public EncounterDensityOverrideEntryPrototype[] EncounterDensityOverrides { get; protected set; }
        public PopulationObjectListPrototype GlobalEncounters { get; protected set; }
        public PopulationObjectListPrototype Themes { get; protected set; }
        public int SpawnMapDistributeDistance { get; protected set; }
        public int SpawnMapDistributeSpread { get; protected set; }
        public bool SpawnMapEnabled { get; protected set; }
    }

    public class SpawnMarkerPrototype : Prototype
    {
        public MarkerType Type { get; protected set; }
        public PrototypeId Shape { get; protected set; }
        public AssetId EditorIcon { get; protected set; }
    }

    public class PopulationMarkerPrototype : SpawnMarkerPrototype
    {
        
    }

    public class PropMarkerPrototype : SpawnMarkerPrototype
    {
    }

    public class PopulatablePrototype : Prototype
    {
    }

    public class PopulationInfoPrototype : PopulatablePrototype
    {
        public PrototypeId[] Ranks { get; protected set; }
        public bool Unique { get; protected set; }
    }

    public class RespawnMethodPrototype : Prototype
    {
        public float PlayerPresentDeferral { get; protected set; }
        public int DeferralMax { get; protected set; }
        public float RandomTimeOffset { get; protected set; }
    }

    public class RespawnReducerByThresholdPrototype : RespawnMethodPrototype
    {
        public float BaseRespawnTime { get; protected set; }
        public float RespawnReductionThreshold { get; protected set; }
        public float MinimumRespawnTime { get; protected set; }
        public float ReducedRespawnTime { get; protected set; }
    }

    public class PopulationObjectInstancePrototype : Prototype
    {
        public short Weight { get; protected set; }
        public PrototypeId Object { get; protected set; }
    }

    public class PopulationObjectListPrototype : Prototype
    {
        public PopulationObjectInstancePrototype[] List { get; protected set; }
    }

    public class PopulationThemePrototype : PopulationListTagThemePrototype
    {
        public PopulationObjectListPrototype Enemies { get; protected set; }
        public int EnemyPicks { get; protected set; }
        public PopulationObjectListPrototype Encounters { get; protected set; }
    }

    public class PopulationThemeSetPrototype : Prototype
    {
        public PrototypeId[] Themes { get; protected set; }
    }

    public class EncounterDensityOverrideEntryPrototype : Prototype
    {
        public PrototypeId MarkerType { get; protected set; }
        public float Density { get; protected set; }
    }
}
