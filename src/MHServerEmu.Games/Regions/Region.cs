using System.Diagnostics;
using MHServerEmu.Core.Collisions;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.DRAG;
using MHServerEmu.Games.DRAG.Generators.Regions;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.Entities.Avatars;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Missions;
using MHServerEmu.Games.Navi;
using MHServerEmu.Games.Properties;

namespace MHServerEmu.Games.Regions
{
    [Flags]
    public enum RegionStatus
    {
        None = 0,
        GenerateAreas = 1 << 0,
        Shutdown = 1 << 2,
    }

    public enum RegionPartitionContext
    {
        Insert,
        Remove
    }

    public class Region : ISerialize, IMissionManagerOwner
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private Area _startArea;
        private RegionStatus _statusFlag;

        public Game Game { get; }

        public ulong Id { get; private set; } // InstanceAddress
        public RegionSettings Settings { get; private set; }
        public int RandomSeed { get; private set; }
        public int RegionLevel { get; private set; }

        public RegionPrototype Prototype { get; private set; }
        public PrototypeId PrototypeDataRef { get => Prototype != null ? Prototype.DataRef : PrototypeId.Invalid; }
        public string PrototypeName { get => GameDatabase.GetFormattedPrototypeName(PrototypeDataRef); }

        public Aabb Aabb { get; private set; } = new();

        public Dictionary<uint, Area> Areas { get; } = new();
        public IEnumerable<Cell> Cells { get => IterateCellsInVolume(Aabb); }
        public IEnumerable<Entity> Entities { get => Game.EntityManager.IterateEntities(this); }

        // ArchiveData
        public ReplicatedPropertyCollection Properties { get; private set; }
        public MissionManager MissionManager { get; private set; }

        public ConnectionNodeList Targets { get; private set; }
        public RegionProgressionGraph ProgressionGraph { get; set; }
        public EntityRegionSpatialPartition EntitySpatialPartition { get; private set; }
        public CellSpatialPartition CellSpatialPartition { get; private set; }
        public NaviSystem NaviSystem { get; private set; }
        public NaviMesh NaviMesh { get; private set; }

        public Region(Game game)
        {
            Game = game;

            NaviSystem = new();
            NaviMesh = new(NaviSystem);
        }

        public override string ToString()
        {
            return $"{PrototypeDataRef.GetName()}, ID=0x{Id:X} ({Id}), LEVEL={RegionLevel}, SEED={RandomSeed}, GAMEID={Game}";
        }

        public bool Initialize(RegionSettings settings)
        {
            if (Game == null) return false;

            MissionManager = new(Game, this);

            Settings = settings;
            Properties = new();   // V10_TODO Properties.Bind()

            Id = settings.InstanceAddress; // Region Id
            if (Id == 0) return Logger.WarnReturn(false, "Initialize(): settings.InstanceAddress == 0");

            Prototype = GameDatabase.GetPrototype<RegionPrototype>(settings.RegionDataRef);
            if (Prototype == null) return Logger.WarnReturn(false, "Initialize(): Prototype == null");

            RandomSeed = settings.Seed;
            Aabb = settings.Bounds;

            if (NaviSystem.Initialize(this) == false) return false;
            if (Aabb.IsZero() == false)
            {
                if (settings.GenerateAreas)
                    Logger.Warn("Initialize(): Bound is not Zero with GenerateAreas On");

                InitializeSpacialPartition(Aabb);
                NaviMesh.Initialize(Aabb, 1000.0f, this);
            }

            ProgressionGraph = new();

            Targets = RegionTransition.BuildConnectionEdges(settings.RegionDataRef); // For Teleport system

            if (settings.GenerateAreas)
            {
                if (GenerateAreas(settings.GenerateLog) == false)
                    return Logger.WarnReturn(false, $"Initialize(): Failed to generate areas for\n  region: {this}\n    seed: {RandomSeed}");
            }

            return true;
        }

        public bool Serialize(Archive archive)
        {
            bool success = true;
            success &= Serializer.Transfer(archive, Properties);
            success &= Serializer.Transfer(archive, MissionManager);
            return success;
        }

        public bool TestStatus(RegionStatus status)
        {
            return _statusFlag.HasFlag(status);
        }

        private void SetStatus(RegionStatus status, bool enable)
        {
            if (enable) _statusFlag |= status;
            else _statusFlag ^= status;
        }

        public void Shutdown()
        {
            SetStatus(RegionStatus.Shutdown, true);
        }

        #region Area Management

        public Area CreateArea(PrototypeId areaRef, Vector3 origin)
        {
            RegionManager regionManager = Game.RegionManager;
            if (regionManager == null) return null;

            AreaSettings settings = new()
            {
                Id = regionManager.AllocateAreaId(),
                AreaDataRef = areaRef,
                Origin = origin,
                RegionSettings = Settings
            };

            return AddArea(settings);
        }

        public Area AddArea(AreaSettings settings)
        {
            if (settings.AreaDataRef == 0 || settings.Id == 0 || settings.RegionSettings == null) return null;
            Area area = new(Game, this);

            if (area.Initialize(settings) == false)
            {
                DeallocateArea(area);
                return null;
            }

            Areas[area.Id] = area;

            if (settings.RegionSettings.GenerateLog)
                Logger.Debug($"Adding area {area.PrototypeName}, id={area.Id}, areapos = {area.Origin}, seed = {RandomSeed}");

            return area;
        }

        public Area GetArea(PrototypeId prototypeId)
        {
            foreach (var area in Areas.Values)
                if (area.PrototypeDataRef == prototypeId)
                    return area;

            return null;
        }

        public Area GetAreaById(uint id)
        {
            if (Areas.TryGetValue(id, out Area area))
                return area;

            return null;
        }

        public Area GetAreaAtPosition(Vector3 position)
        {
            foreach (Area area in Areas.Values)
                if (area.IntersectsXY(position))
                    return area;

            return null;
        }

        public Area GetStartArea()
        {
            if (_startArea == null && Areas.Count > 0)
                _startArea = IterateAreas().First();

            return _startArea;
        }

        public IEnumerable<Area> IterateAreas(Aabb? bound = null)
        {
            foreach (var area in Areas.Values.ToArray())
                if (bound == null || area.RegionBounds.Intersects(bound.Value))
                    yield return area;
        }

        /* V10_TODO
        public void RebuildBlackOutZone(BlackOutZone zone)
        {
            foreach (var area in IterateAreas())
                if (area.TestStatus(GenerateFlag.Population) && zone.Sphere.Intersects(area.RegionBounds))
                    area.RebuildBlackOutZone(zone);
        }

        public int GetAreaLevel(Area area)
        {
            if (Prototype.LevelUseAreaOffset)
                return area.GetAreaLevel();

            return RegionLevel;
        }
        */

        public void DestroyArea(uint id)
        {
            if (Areas.TryGetValue(id, out Area areaToRemove))
            {
                DeallocateArea(areaToRemove);
                Areas.Remove(id);
            }
        }

        private void DeallocateArea(Area area)
        {
            if (area == null) return;

            if (Settings.GenerateLog)
                Logger.Trace($"{Game} - Deallocating area id {area.Id}, {area}");

            area.Shutdown();
        }

        #endregion

        #region Cell Management

        public Cell GetCellbyId(uint cellId)
        {
            foreach (Cell cell in Cells)
            {
                if (cell.Id == cellId)
                    return cell;
            }

            return default;
        }

        public Cell GetCellAtPosition(Vector3 position)
        {
            foreach (Cell cell in Cells)
            {
                if (cell.IntersectsXY(position))
                    return cell;
            }

            return null;
        }

        public IEnumerable<Cell> IterateCellsInVolume<B>(B bounds) where B : IBounds
        {
            if (CellSpatialPartition != null)
                return CellSpatialPartition.IterateElementsInVolume(bounds);
            else
                return Enumerable.Empty<Cell>(); //new CellSpatialPartition.ElementIterator();
        }

        #endregion

        #region Entity Management

        public bool InsertEntityInSpatialPartition(WorldEntity entity) => EntitySpatialPartition.Insert(entity);
        public void UpdateEntityInSpatialPartition(WorldEntity entity) => EntitySpatialPartition.Update(entity);
        public bool RemoveEntityFromSpatialPartition(WorldEntity entity) => EntitySpatialPartition.Remove(entity);

        public IEnumerable<WorldEntity> IterateEntitiesInRegion(EntityRegionSPContext context)
        {
            return IterateEntitiesInVolume(Aabb, context);
        }

        public IEnumerable<WorldEntity> IterateEntitiesInVolume<B>(B bound, EntityRegionSPContext context) where B : IBounds
        {
            if (EntitySpatialPartition != null)
                return EntitySpatialPartition.IterateElementsInVolume(bound, context);
            else
                return Enumerable.Empty<WorldEntity>();
        }

        public IEnumerable<Avatar> IterateAvatarsInVolume(in Sphere bound)
        {
            if (EntitySpatialPartition != null)
                return EntitySpatialPartition.IterateAvatarsInVolume(bound);
            else
                return Enumerable.Empty<Avatar>();
        }

        public void GetEntitiesInVolume<B>(List<WorldEntity> entities, B volume, EntityRegionSPContext context) where B : IBounds
        {
            EntitySpatialPartition?.GetElementsInVolume(entities, volume, context);
        }

        #endregion

        #region Space & Physics

        public Aabb CalculateAabbFromAreas()
        {
            Aabb bounds = Aabb.InvertedLimit;

            foreach (var area in IterateAreas())
                bounds += area.RegionBounds;

            return bounds;
        }

        public void SetAabb(in Aabb boundingBox)
        {
            if (boundingBox.Volume <= 0 || (boundingBox.Min == Aabb.Min && boundingBox.Max == Aabb.Max)) return;

            Aabb = boundingBox;

            NaviMesh.Initialize(Aabb, 1000.0f, this);
            InitializeSpacialPartition(Aabb);
        }

        private bool InitializeSpacialPartition(in Aabb bound)
        {
            if (EntitySpatialPartition != null || CellSpatialPartition != null) return false;

            EntitySpatialPartition = new(bound);
            CellSpatialPartition = new(bound);

            foreach (Area area in IterateAreas())
            {
                foreach (var cellItr in area.Cells)
                    PartitionCell(cellItr.Value, RegionPartitionContext.Insert);
            }

            /* V10_TODO
            SpawnMarkerRegistry.InitializeSpacialPartition(bound);
            PopulationManager.InitializeSpacialPartition(bound);
            */

            return true;
        }

        public bool? PartitionCell(Cell cell, RegionPartitionContext context)
        {
            if (CellSpatialPartition != null)
            {
                switch (context)
                {
                    case RegionPartitionContext.Insert:
                        return CellSpatialPartition.Insert(cell);
                    case RegionPartitionContext.Remove:
                        return CellSpatialPartition.Remove(cell);
                }
            }

            return null;
        }

        public float GetDistanceToClosestAreaBounds(Vector3 position)
        {
            float minDistance = float.MaxValue;
            foreach (Area area in IterateAreas())
            {
                float distance = area.RegionBounds.DistanceToPoint2D(position);
                minDistance = Math.Min(distance, minDistance);
            }

            if (minDistance == float.MaxValue)
                Logger.Error("GetDistanceToClosestAreaBounds");

            return minDistance;
        }

        public bool FindTargetLocation(ref Vector3 markerPos, ref Orientation markerRot, PrototypeId areaProtoRef, PrototypeId cellProtoRef, PrototypeId entityProtoRef)
        {
            //Logger.Debug($"FindTargetLocation(): areaProtoRef={areaProtoRef.GetName()}, cellProtoRef={cellProtoRef.GetName()}, entityProtoRef={entityProtoRef.GetName()}");

            Area targetArea;

            bool found = false;

            // If we have a valid area ref, search only that area
            if (areaProtoRef != PrototypeId.Invalid)
            {
                targetArea = GetArea(areaProtoRef);
                if (targetArea != null)
                    found = targetArea.FindTargetLocation(ref markerPos, ref markerRot, cellProtoRef, entityProtoRef);
            }

            // Search all areas if we don't have a valid area ref
            if (found == false)
            {
                foreach (Area area in IterateAreas())
                {
                    targetArea = area;
                    if (targetArea.FindTargetLocation(ref markerPos, ref markerRot, cellProtoRef, entityProtoRef))
                        return true;
                }
            }

            return found;
        }

        #endregion

        #region Generation

        public bool GenerateAreas(bool log)
        {
            if (TestStatus(RegionStatus.GenerateAreas)) return false;

            RegionGenerator regionGenerator = DRAGSystem.LinkRegionGenerator(Prototype.RegionGenerator);

            regionGenerator.GenerateRegion(log, RandomSeed, this);

            _startArea = regionGenerator.StartArea;
            SetStatus(RegionStatus.GenerateAreas, true);
            SetAabb(CalculateAabbFromAreas());

            // V10_TODO: populate the generated region with stuff
            bool success = GenerateHelper(regionGenerator, GenerateFlag.Background)
                        && GenerateHelper(regionGenerator, GenerateFlag.PostInitialize)
                        && GenerateHelper(regionGenerator, GenerateFlag.Navi)
                        && GenerateNaviMesh();

            if (success)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                success &= GenerateHelper(regionGenerator, GenerateFlag.Population)
                        && GenerateHelper(regionGenerator, GenerateFlag.PostGenerate);

                Logger.Info($"Generated population in {stopwatch.ElapsedMilliseconds} ms");
            }

            return success;
        }

        public bool GenerateNaviMesh()
        {
            NaviSystem.ClearErrorLog();
            return NaviMesh.GenerateMesh();
        }

        public bool GenerateHelper(RegionGenerator regionGenerator, GenerateFlag flag)
        {
            bool success = Areas.Count > 0;

            foreach (Area area in IterateAreas())
            {
                if (area == null)
                {
                    success = false;
                }
                else
                {
                    List<PrototypeId> areas = new() { area.PrototypeDataRef };
                    success &= area.Generate(regionGenerator, areas, flag);
                    if (area.TestStatus(GenerateFlag.Background) == false)
                        Logger.Error($"{area} Not generated");
                }
            }

            return success;
        }

        #endregion

        public void OnAddedToAOI(Player player)
        {
        }

        public void OnRemovedFromAOI(Player player)
        {
        }
    }
}
