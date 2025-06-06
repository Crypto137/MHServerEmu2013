﻿using MHServerEmu.Core.Collisions;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.System.Random;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.DRAG;
using MHServerEmu.Games.DRAG.Generators.Areas;
using MHServerEmu.Games.DRAG.Generators.Regions;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.GameData.Prototypes.Generators;
using MHServerEmu.Games.Populations;
using MHServerEmu.Games.Properties;

namespace MHServerEmu.Games.Regions
{
    [Flags]
    public enum GenerateFlag
    {
        Background      = 1 << 0,
        Population      = 1 << 1,
        Navi            = 1 << 2,
        PathCollection  = 1 << 3,
        PostInitialize  = 1 << 4,
        PostGenerate    = 1 << 5,
    }

    public enum ConnectPosition
    {
        One,
        Begin,
        Inside,
        End
    }

    public class Area
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private GenerateFlag _statusFlag;
        private List<TowerFixupData> _towerFixupList;

        public uint Id { get; private set; }
        public Vector3 Origin { get; set; }

        public bool GenerateLog { get; private set; }

        public AreaPrototype Prototype { get; private set; }
        public PrototypeId PrototypeDataRef { get => Prototype.DataRef; }
        public string PrototypeName { get => GameDatabase.GetFormattedPrototypeName(PrototypeDataRef); }

        public bool IsDynamicArea { get => PrototypeDataRef == GameDatabase.GlobalsPrototype.DynamicArea; }

        public Aabb RegionBounds { get; set; }
        public Aabb LocalBounds { get; set; }
        public int RandomSeed { get; set; }
        public List<uint> SubAreas { get; set; } = new();

        public PrototypeId DistrictDataRef { get; set; }
        public Game Game { get; private set; }
        public Region Region { get; private set; }

        //public PropTable PropTable { get; set; }  V10_TODO
        public Generator Generator { get; set; }
        //public SpawnMap SpawnMap { get; private set; } V10_TODO
        public float PlayableNavArea { get; set; }
        public float SpawnableNavArea { get; set; }
        public List<AreaConnectionPoint> AreaConnections { get; set; } = new();
        public List<RandomInstanceRegionPrototype> RandomInstances { get; } = new();
        public Dictionary<uint, Cell> Cells { get; } = new();

        public Area(Game game, Region region)
        {
            Game = game;
            Region = region;
        }

        public override string ToString()
        {
            return $"{PrototypeName}, areaid={Id}, aabb={RegionBounds}, game={Game}";
        }

        public bool Initialize(AreaSettings settings)
        {
            Id = settings.Id;
            if (Id == 0) return false;

            Prototype = GameDatabase.GetPrototype<AreaPrototype>(settings.AreaDataRef);
            if (Prototype == null) return false;

            Origin = settings.Origin;
            RegionBounds = new Aabb(Origin, Origin);

            RandomSeed = Region.RandomSeed;
            GenerateLog = settings.RegionSettings.GenerateLog;

            if (settings.RegionSettings.GenerateAreas)
            {
                Generator = DRAGSystem.LinkGenerator(GenerateLog, Prototype.Generator, this);
                if (Generator == null)
                {
                    if (GenerateLog)
                        Logger.Error("Area failed to link to a required generator.");

                    return false;
                }

                GRandom random = new(RandomSeed);

                LocalBounds = Generator.PreGenerate(random);

                RegionBounds = LocalBounds.Translate(Origin);
                RegionBounds.RoundToNearestInteger();
            }

            return true;
        }

        public void Shutdown()
        {
            // V10_TODO
        }

        public IEnumerable<Cell> CellIterator()
        {
            var enumerator = Cells.GetEnumerator();
            while (enumerator.MoveNext())
                yield return enumerator.Current.Value;
        }

        public AreaGenerationInterface GetAreaGenerationInterface()
        {
            if (TestStatus(GenerateFlag.Background))
                return null;

            return Generator as AreaGenerationInterface;
        }

        public List<TowerFixupData> GetTowerFixup(bool toCreate)
        {
            if (_towerFixupList == null && toCreate)
                _towerFixupList = new();

            return _towerFixupList;
        }

        public Cell AddCell(uint cellid, CellSettings settings)
        {
            if (settings.Seed == 0) settings.Seed = RandomSeed;

            Cell cell = new(this, cellid);
            Cells[cellid] = cell;
            cell.Initialize(settings);

            PlayableNavArea += cell.PlayableArea;
            SpawnableNavArea += cell.SpawnableArea;

            RegionManager regionManager = Game.RegionManager;
            regionManager.AddCell(cell);

            //Region.SpawnMarkerRegistry.AddCell(cell); V10_TODO

            return cell;
        }

        public bool IntersectsXY(Vector3 position)
        {
            return RegionBounds.IntersectsXY(position);
        }

        public Cell GetCellAtPosition(Vector3 position)
        {
            // V10_TODO: Implement IterateCellsInVolume
            /*
            Aabb volume = new(position, 0.000001f, 0.000001f, RegionBounds.Height * 2.0f);
            foreach (Cell cell in Region.IterateCellsInVolume(volume))
            {
                if (cell.Area == this)
                    return cell;
            }
            */
            foreach (Cell cell in Cells.Values)
            {
                if (cell.Area == this && cell.RegionBounds.IntersectsXY(position))
                    return cell;
            }

            return null;
        }

        public bool CreateCellConnection(Cell cellA, Cell cellB)
        {
            if (cellA.Area != this || cellB.Area != this) return false;

            cellA.AddCellConnection(cellB.Id);
            cellB.AddCellConnection(cellA.Id);

            return true;
        }

        public bool Generate(RegionGenerator generator, List<PrototypeId> areas, GenerateFlag flags)
        {
            bool success = true;
            if (success && flags.HasFlag(GenerateFlag.Background))
                success &= GenerateBackground(generator, areas);


            if (success && flags.HasFlag(GenerateFlag.PostInitialize))
                success &= GeneratePostInitialize();

            if (success && flags.HasFlag(GenerateFlag.Navi))
                success &= GenerateNavi();

            /* V10_TODO
            if (success && flags.HasFlag(GenerateFlag.PathCollection))
            {
                DistrictPrototype district = GameDatabase.GetPrototype<DistrictPrototype>(DistrictDataRef);
                if (district != null)
                    Region.PathCache.AppendPathCollection(district.PathCollection, Origin);
            }
            */

            if (success && flags.HasFlag(GenerateFlag.Population))
                success &= GeneratePopulation();

            if (success && flags.HasFlag(GenerateFlag.PostGenerate))
                success &= PostGenerate();

            return success;
        }

        private bool PostGenerate()
        {
            if (IsDynamicArea)
                return true;

            if (Prototype.FullyGenerateCells) // only TheRaft
                foreach (Cell cell in CellIterator())
                    cell.Generate();

            return true;
        }

        private bool GeneratePopulation()
        {
            if (TestStatus(GenerateFlag.Background) == false)
                return Logger.WarnReturn(false, $"Generate population should have background generator \nRegion:{Region}\nArea:{this}");

            if (TestStatus(GenerateFlag.Population)) return true;

            /* V10_TODO
            var populationProto = PopulationArea.PopulationPrototype;
            if (populationProto?.UseSpawnMap == true)
                SpawnMap = new(this, populationProto);

            BlackOutZonesRebuild();
            SetStatus(GenerateFlag.Population, true);

            Region.AreaCreatedEvent.Invoke(new(this));
            */

            if (Region.Settings.GenerateEntities)
                foreach (Cell cell in CellIterator())
                    cell.SpawnMarkerSet(MarkerSetOptions.SpawnMissionAssociated);

            //PopulationArea?.Generate();

            return true;
        }

        private bool GenerateNavi()
        {
            if (TestStatus(GenerateFlag.Background) == false)
                return Logger.WarnReturn(false, $"[Engineering Issue] Navi is getting generated out of order with, or after a failed area generator\nRegion:{Region}\nArea:{this}");

            if (TestStatus(GenerateFlag.Navi))
                return true;

            SetStatus(GenerateFlag.Navi, true);

            foreach (var cell in CellIterator())
                cell.AddNavigationDataToRegion();

            return true;
        }

        private bool GeneratePostInitialize()
        {
            bool success = true;

            if (TestStatus(GenerateFlag.Background) == false)
                return false;

            CellGridGenerator.CellGridBorderBehavior(this);
            WideGridAreaGenerator.CellGridBorderBehavior(this);
            SingleCellAreaGenerator.CellGridBorderBehavior(this);

            foreach (var cell in CellIterator())
            {
                if (cell == null) continue;
                success &= cell.PostInitialize();
            }

            if (_towerFixupList != null && _towerFixupList.Count > 0)
            {
                foreach (var towerData in _towerFixupList)
                {
                    Cell cell = towerData.Id != 0 ? GetCell(towerData.Id) : null;
                    Cell previous = towerData.Previous != 0 ? GetCell(towerData.Previous) : null;

                    if (cell != null && previous != null)
                    {
                        Transition towerUpTrans = null;
                        Transition towerDownTrans = null;

                        foreach (var entity in previous.Entities)
                        {
                            if (entity is Transition transition)
                            {
                                if (transition.TransitionPrototype.Type == RegionTransitionType.TowerUp)
                                {
                                    towerUpTrans = transition;
                                    break;
                                }
                            }
                        }

                        foreach (var entity in cell.Entities)
                        {
                            if (entity is Transition transition)
                            {
                                if (transition.TransitionPrototype.Type == RegionTransitionType.TowerDown)
                                {
                                    towerDownTrans = transition;
                                    break;
                                }
                            }
                        }

                        if (towerUpTrans != null && towerDownTrans != null)
                        {
                            towerUpTrans.ConfigureTowerGen(towerDownTrans);
                            towerDownTrans.ConfigureTowerGen(towerUpTrans);
                        }
                    }
                }
            }

            return success;
        }


        public void AddSubArea(Area newarea)
        {
            if (newarea != null) SubAreas.Add(newarea.Id);
        }

        private Cell GetCell(uint id)
        {
            if (Cells.TryGetValue(id, out Cell cell)) return cell;
            return null;
        }

        public bool GenerateBackground(RegionGenerator regionGenerator, List<PrototypeId> areas)
        {
            if (Region == null) return false;
            if (TestStatus(GenerateFlag.Background)) return true;
            if (Generator == null) return false;

            GRandom random = new(RandomSeed);

            bool success = Generator.Generate(random, regionGenerator, areas);
            if (success == false) Logger.Warn($"Area {ToString()} in region {Region} failed to generate");

            Generator = null;

            if (success && SubAreas.Count > 0)
            {
                foreach (var areaId in SubAreas)
                    if (areaId != 0 && Region.Areas.TryGetValue(areaId, out Area area))
                        success &= area.GenerateBackground(regionGenerator, areas);
            }

            if (success) SetStatus(GenerateFlag.Background, true);
            return success;
        }

        private void SetStatus(GenerateFlag status, bool enable)
        {
            if (enable) _statusFlag |= status;
            else _statusFlag ^= status;
        }

        public bool TestStatus(GenerateFlag status)
        {
            return _statusFlag.HasFlag(status);
        }

        public void SetOrigin(Vector3 newPostion)
        {
            Vector3 offset = newPostion - Origin;
            Origin = newPostion;

            RegionBounds = LocalBounds.Translate(Origin);
            RegionBounds.RoundToNearestInteger();

            if (AreaConnections.Count > 0)
                foreach (var connection in AreaConnections)
                    connection.Position += offset;

            if (TestStatus(GenerateFlag.Background))
            {
                foreach (var cellIt in Cells)
                {
                    Cell cell = cellIt.Value;
                    if (cell == null) continue;
                    cell.SetAreaPosition(cell.AreaPosition, cell.AreaOrientation);
                }
            }
        }

        public static void CreateConnection(Area areaA, Area areaB, Vector3 position, ConnectPosition connectPosition)
        {
            if (areaA.GenerateLog)
                Logger.Debug($"CreateConnection(): Connect {position} {areaA.Id} <> {areaB.Id}");

            areaA.AddConnection(position, areaB, connectPosition);
            areaB.AddConnection(position, areaA, connectPosition);
        }

        public void AddConnection(Vector3 position, Area area, ConnectPosition connectPosition)
        {
            AreaConnectionPoint areaConnection = new()
            {
                Position = position,
                ConnectedArea = area,
                ConnectPosition = connectPosition
            };

            AreaConnections.Add(areaConnection);
        }
        public bool GetPossibleAreaConnections(ConnectionList connections, in Segment segment)
        {
            if (Generator == null) return false;
            return Generator.GetPossibleConnections(connections, segment);
        }

        public Vector3 AreaToRegion(Vector3 positionInArea)
        {
            return positionInArea + Origin;
        }

        public bool FindTargetLocation(ref Vector3 markerPos, ref Orientation markerRot, PrototypeId cellProtoRef, PrototypeId entityProtoRef)
        {
            foreach (Cell cell in CellIterator())
            {
                if (cellProtoRef != PrototypeId.Invalid && cellProtoRef != cell.PrototypeDataRef)
                    continue; // TODO check

                if (cell.FindTargetLocation(ref markerPos, ref markerRot, entityProtoRef))
                    return true;
            }

            return false;
        }

        public int GetCharacterLevel(WorldEntityPrototype entityProto)
        {
            // V10_TODO: Check if there are any region level overrides in 1.10
            int characterLevel = entityProto.Properties[PropertyEnum.CharacterLevel];
            return characterLevel;
        }
    }

    public class AreaConnectionPoint
    {
        public Area ConnectedArea { get; set; }
        public Vector3 Position { get; set; }
        public ulong Id { get; set; }
        public ConnectPosition ConnectPosition { get; set; }

        public AreaConnectionPoint()
        {
            ConnectedArea = null;
            Position = new();
            Id = 0;
            ConnectPosition = ConnectPosition.One;
        }
    }
}
