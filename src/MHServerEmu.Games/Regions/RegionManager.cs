using MHServerEmu.Core.Logging;
using MHServerEmu.Core.System;
using MHServerEmu.Games.GameData;

namespace MHServerEmu.Games.Regions
{
    public class RegionManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly IdGenerator _idGenerator = new(IdType.Region, 0);

        private readonly Dictionary<uint, Cell> _allCells = new();
        private readonly Dictionary<ulong, Region> _allRegions = new();

        private readonly Dictionary<PrototypeId, Region> _regionDict = new();

        private uint _areaId = 1;
        private uint _cellId = 1;

        public Game Game { get; }

        public RegionManager(Game game)
        {
            Game = game;
        }

        public uint AllocateCellId()
        {
            return _cellId++;
        }

        public uint AllocateAreaId()
        {
            return _areaId++;
        }

        public bool AddCell(Cell cell)
        {
            if (cell != null && _allCells.ContainsKey(cell.Id) == false)
            {
                _allCells[cell.Id] = cell;
                if (cell.Area.GenerateLog)
                    Logger.Trace($"Adding cell {cell} in region {cell.Region} area id={cell.Area.Id}");
                return true;
            }
            return false;
        }

        public Cell GetCell(uint cellId)
        {
            if (_allCells.TryGetValue(cellId, out var cell)) return cell;
            return null;
        }

        public bool RemoveCell(Cell cell)
        {
            if (cell == null) return false;

            if (cell.Area.GenerateLog)
                Logger.Trace($"Removing cell {cell} from region {cell.Region}");

            if (_allCells.ContainsKey(cell.Id))
            {
                _allCells.Remove(cell.Id);
                return true;
            }

            return false;
        }

        public Region GetOrGenerateRegionForPlayer(PrototypeId regionProtoRef)
        {
            if (_regionDict.TryGetValue(regionProtoRef, out Region region) == false)
            {
                RegionSettings settings = new()
                {
                    InstanceAddress = _idGenerator.Generate(),
                    RegionDataRef = regionProtoRef,
                    Level = 1,
                    GenerateAreas = true,
                    GenerateEntities = true,
                    GenerateLog = false
                };

                int tries = 10;

                while (region == null && (--tries > 0))
                {
                    if (tries < 9) settings.Seed = Game.Random.Next(); // random.Next(); 
                    region = CreateRegion(settings);
                }

                if (region == null)
                    Logger.Error($"GenerateRegion failed after {10 - tries} attempts | regionId: {regionProtoRef.GetNameFormatted()} | Last Seed: {settings.Seed}");
            }

            return region;
        }

        public Region GetRegion(ulong id)
        {
            if (id == 0) return null;

            if (_allRegions.TryGetValue(id, out Region region))
                return region;

            return null;
        }

        public Dictionary<ulong, Region>.ValueCollection.Enumerator GetEnumerator()
        {
            return _allRegions.Values.GetEnumerator();
        }

        private Region CreateRegion(RegionSettings settings)
        {
            if (settings.RegionDataRef == 0) return null;

            ulong instanceAddress = settings.InstanceAddress;
            if (instanceAddress == 0 || GetRegion(instanceAddress) != null) return null;

            Region region = new(Game);

            _allRegions[instanceAddress] = region;

            if (region.Initialize(settings) == false)
            {
                _allRegions.Remove(instanceAddress);
                region.Shutdown();
                return null;
            }

            _regionDict.Add(region.PrototypeDataRef, region);

            return region;
        }
    }
}
