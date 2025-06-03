using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes.Generators
{
    #region Enums

    [AssetEnum((int)Random)]
    public enum CellDeletionEnum
    {
        Random,
        Corner,
        Edge,
    }

    #endregion

    public class GridAreaGeneratorPrototype : BaseGridAreaGeneratorPrototype
    {
        public CellGridBehaviorPrototype[] Behaviors { get; protected set; }
        public float ConnectionKillChancePct { get; protected set; }
    }

    public class WideGridAreaGeneratorPrototype : BaseGridAreaGeneratorPrototype
    {
        public CellGridBorderBehaviorPrototype BorderBehavior { get; protected set; }
        public bool ProceduralSuperCells { get; protected set; }
    }

    public class BaseGridAreaGeneratorPrototype : GeneratorPrototype
    {
        public CellSetEntryPrototype[] CellSets { get; protected set; }
        public int CellSize { get; protected set; }
        public int CellsX { get; protected set; }
        public int CellsY { get; protected set; }
        public CellDeletionEnum RoomKillMethod { get; protected set; }
        public float RoomKillChancePct { get; protected set; }
        public CellDeletionProfilePrototype[] SecondaryDeletionProfiles { get; protected set; }
        public RequiredCellPrototype[] RequiredCells { get; protected set; }
        public bool SupressMissingCellErrors { get; protected set; }
        public bool NoConnectionsOnCorners { get; protected set; }
        public RandomInstanceListPrototype RandomInstances { get; protected set; }
        public int DeadEndMax { get; protected set; }
        public RequiredSuperCellEntryPrototype[] RequiredSuperCells { get; protected set; }
        public RequiredSuperCellEntryPrototype[] NonRequiredSuperCells { get; protected set; }
        public int NonRequiredNormalCellsMin { get; protected set; }
        public int NonRequiredNormalCellsMax { get; protected set; }
        public RequiredCellPrototype[] NonRequiredNormalCells { get; protected set; }
        public int NonRequiredSuperCellsMin { get; protected set; }
        public int NonRequiredSuperCellsMax { get; protected set; }

        //---

        public bool RequiresCell(PrototypeId cellRef)
        {
            if (RequiredSuperCells != null)
            {
                foreach (RequiredSuperCellEntryPrototype entry in RequiredSuperCells)
                {
                    if (entry != null && entry.SuperCell != 0)
                    {
                        SuperCellPrototype superCellP = GameDatabase.GetPrototype<SuperCellPrototype>(entry.SuperCell);
                        if (superCellP != null && superCellP.ContainsCell(cellRef)) return true;
                    }
                }
            }

            if (RequiredCells != null)
            {
                foreach (RequiredCellPrototype requiredCell in RequiredCells)
                {
                    if (requiredCell != null && GameDatabase.GetDataRefByAsset(requiredCell.Cell) == cellRef) return true;
                }
            }
            return false;
        }
    }

    public class CellDeletionProfilePrototype : Prototype
    {
        public CellDeletionEnum RoomKillMethod { get; protected set; }
        public float RoomKillPct { get; protected set; }
    }

    #region RequiredCellRestrictBasePrototype

    // V10_NOTE: Doesn't exist in 1.10?

    #endregion

    #region RequiredCellBasePrototype

    public class RequiredCellBasePrototype : Prototype
    {
        public PrototypeId PopulationThemeOverride { get; protected set; }
    }

    public class RequiredCellPrototype : RequiredCellBasePrototype
    {
        public AssetId Cell { get; protected set; }
        public bool Destination { get; protected set; }
    }

    public class RandomInstanceRegionPrototype : RequiredCellBasePrototype
    {
        public AssetId OriginCell { get; protected set; }
        public PrototypeId OriginEntity { get; protected set; }
        public PrototypeId OverrideLocalPopulation { get; protected set; }
        public PrototypeId Target { get; protected set; }
        public int Weight { get; protected set; }
    }

    public class RequiredCellBaseListPrototype : RequiredCellBasePrototype
    {
        public RequiredCellBasePrototype[] RequiredCells { get; protected set; }
    }

    public class RequiredSuperCellEntryPrototype : RequiredCellBasePrototype
    {
        public PrototypeId SuperCell { get; protected set; }
    }

    #endregion

    public class RandomInstanceListPrototype : Prototype
    {
        public RandomInstanceRegionPrototype[] List { get; protected set; }
        public int Picks { get; protected set; }
    }
}
