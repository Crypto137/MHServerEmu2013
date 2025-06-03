namespace MHServerEmu.Games.GameData.Prototypes
{
    public class EntityFilterPrototype : Prototype
    {
    }

    public class EntityFilterFilterListPrototype : EntityFilterPrototype
    {
        public EntityFilterPrototype[] Filters { get; protected set; }
    }

    public class EntityFilterAndPrototype : EntityFilterFilterListPrototype
    {
    }

    public class EntityFilterHasAlliancePrototype : EntityFilterPrototype
    {
        public PrototypeId Alliance { get; protected set; }
    }

    public class EntityFilterHasKeywordPrototype : EntityFilterPrototype
    {
        public PrototypeId Keyword { get; protected set; }
    }

    public class EntityFilterHasNegStatusEffectPrototype : EntityFilterPrototype
    {
    }

    public class EntityFilterHasPrototypePrototype : EntityFilterPrototype
    {
        public PrototypeId EntityPrototype { get; protected set; }
        public bool IncludeChildPrototypes { get; protected set; }
    }

    public class EntityFilterInAreaPrototype : EntityFilterPrototype
    {
        public PrototypeId InArea { get; protected set; }
    }

    public class EntityFilterInRegionPrototype : EntityFilterPrototype
    {
        public PrototypeId InRegion { get; protected set; }
    }

    public class EntityFilterIsHostileToPlayersPrototype : EntityFilterPrototype
    {
    }

    public class EntityFilterIsPartyMemberPrototype : EntityFilterPrototype
    {
    }

    public class EntityFilterIsPowerOwnerPrototype : EntityFilterPrototype
    {
    }

    public class EntityFilterNotPrototype : EntityFilterPrototype
    {
        public EntityFilterPrototype EntityFilter { get; protected set; }
    }

    public class EntityFilterOrPrototype : EntityFilterFilterListPrototype
    {
    }

    public class EntityFilterSpawnedByEncounterPrototype : EntityFilterPrototype
    {
        public AssetId EncounterResource { get; protected set; }
    }

    public class EntityFilterSpawnedByMissionPrototype : EntityFilterPrototype
    {
        public PrototypeId MissionPrototype { get; protected set; }
    }

    public class EntityFilterHasRankPrototype : EntityFilterPrototype
    {
        public PrototypeId RankPrototype { get; protected set; }
    }
}
