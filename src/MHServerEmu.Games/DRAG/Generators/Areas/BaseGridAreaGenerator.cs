using System.Reflection;
using MHServerEmu.Core.Collections;
using MHServerEmu.Core.Collisions;
using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.System.Random;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.DRAG.Generators.Regions;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.GameData.Prototypes.Generators;
using MHServerEmu.Games.Regions;

namespace MHServerEmu.Games.DRAG.Generators.Areas
{
    public class BaseGridAreaGenerator : Generator
    {
        private List<RegionTransitionSpec> _requiredTransitions = new();

        public readonly CellSetRegistry CellSetRegistry = new();
        public GenCellGridContainer CellContainer { get; set; }
        public float IncrementZ { get; set; }
        public float IncrementX { get; set; }
        public float IncrementY { get; set; }

        public override Aabb PreGenerate(GRandom random)
        {
            if (!GetPrototype(out var proto)) return default;
            if (!CellSetRegistry.IsInitialized) return default;

            Aabb cellBounds = CellSetRegistry.CellBounds;
            if (!Segment.EpsilonTest(cellBounds.Width, cellBounds.Length)) return default;
            if (!Segment.EpsilonTest(cellBounds.Width, proto.CellSize))
            {
                if (Log) Logger.Warn($"Cell Size Differs between Cellset and Area. Area: {Area}");
                return default;
            }

            float сellsX = proto.CellsX;
            float сellsY = proto.CellsY;
            float width = cellBounds.Width;
            float halfWidth = width / 2.0f;
            float halfHeight = cellBounds.Height / 2.0f;

            Vector3 min = new(-halfWidth, -halfWidth, -halfHeight);
            Vector3 max = new(сellsX * width - halfWidth, сellsY * width - halfWidth, halfHeight);

            PreGenerated = true;

            return new(min, max);
        }

        private bool GetPrototype(out BaseGridAreaGeneratorPrototype proto)
        {
            proto = Area.Prototype.Generator as BaseGridAreaGeneratorPrototype;
            return proto != null;
        }

        public override bool Initialize(Area area)
        {
            if (!base.Initialize(area)) return false;

            Region region = area.Region;
            RegionPrototype regionProto = region.Prototype;

            if (regionProto.StartTarget != 0)
            {
                RegionConnectionTargetPrototype target = GameDatabase.GetPrototype<RegionConnectionTargetPrototype>(regionProto.StartTarget);
                if (target != null &&
                    RegionPrototype.Equivalent(GameDatabase.GetPrototype<RegionPrototype>(target.Region), regionProto) &&
                    target.Area == area.PrototypeDataRef && target.Cell != 0)
                {
                    RegionTransitionSpec spec = new(target.Cell, target.Entity, true);
                    _requiredTransitions.Add(spec);
                }
            }

            RegionTransition.GetRequiredTransitionData(region.PrototypeDataRef, area.PrototypeDataRef, ref _requiredTransitions);

            return InitializeCellRegistry() && InitializeContainer();
        }

        private bool InitializeCellRegistry()
        {
            if (!GetPrototype(out var proto)) return false;

            CellSetRegistry.Initialize(proto.SupressMissingCellErrors, Log);

            var cellSets = proto.CellSets;
            if (cellSets == null)
            {
                if (Log) Logger.Warn("CellGridGenerator with no CellSets specified.");
                return false;
            }

            if (proto.CellSize <= 0)
            {
                if (Log) Logger.Warn("CellGridGenerator called with zero CellSize.");
                return false;
            }

            if (proto.CellsX <= 0 || proto.CellsY <= 0)
            {
                if (Log) Logger.Warn("CellGridGenerator called with zero cells (0 Cell Area).");
                return false;
            }

            foreach (var cellSetEntry in cellSets)
            {
                if (cellSetEntry == null) continue;
                CellSetRegistry.LoadDirectory(cellSetEntry.CellSet, cellSetEntry, cellSetEntry.Weight, cellSetEntry.Unique);
            }

            if (!CellSetRegistry.IsComplete())
            {
                if (Log) Logger.Warn("CellSetRegistry is not complete.");
            }

            return true;
        }

        public virtual bool InitializeContainer()
        {
            if (!GetPrototype(out var proto)) return false;

            if (!CellContainer.Initialize(proto.CellsX, proto.CellsY, CellSetRegistry, proto.DeadEndMax))
            {
                if (Log) Logger.Warn("Failed to initialize cell container.");
                return false;
            }

            CellContainer.ConnectAll();
            return true;
        }

        public override bool GetPossibleConnections(ConnectionList connections, in Segment segment)
        {
            if (CellContainer == null) return false;
            connections.Clear();
            if (!GetPrototype(out var proto)) return false;

            Vector3 origin = Area.Origin;
            float cellSize = proto.CellSize;

            void AddConnection(int x, int y, Vector3 point)
            {
                if (!CheckAllowedConnections(x, y) || CellContainer.GetCell(x, y) == null) return;

                connections.Add(new(
                    origin.X + x * cellSize + point.X,
                    origin.Y + y * cellSize + point.Y,
                    origin.Z + IncrementZ + IncrementX * x + IncrementY * y + point.Z));
            }

            int endx = proto.CellsX - 1;
            int endy = proto.CellsY - 1;
            if (segment.Start.X == segment.End.X)
            {
                var start = segment.Start.X;
                Vector3 pointN = CellSetRegistry.GetConnectionListForType(Cell.Type.N);
                var end = origin.X + endx * cellSize + pointN.X;

                if (Segment.EpsilonTest(start, end, 10.0f))
                {
                    for (int y = 0; y <= endy; ++y)
                    {
                        if (proto.NoConnectionsOnCorners && (y == 0 || y == endy)) continue;
                        AddConnection(endx, y, pointN);
                    }
                    return true;
                }

                Vector3 pointS = CellSetRegistry.GetConnectionListForType(Cell.Type.S);
                end = origin.X + pointS.X;

                if (Segment.EpsilonTest(start, end, 10.0f))
                {
                    for (int y = 0; y <= endy; ++y)
                    {
                        if (proto.NoConnectionsOnCorners && (y == 0 || y == endy)) continue;
                        AddConnection(0, y, pointS);
                    }
                    return true;
                }
            }
            else
            {
                var start = segment.Start.Y;
                Vector3 pointE = CellSetRegistry.GetConnectionListForType(Cell.Type.E);
                var end = origin.Y + endy * cellSize + pointE.Y;

                if (Segment.EpsilonTest(start, end, 10.0f))
                {
                    for (int x = 0; x <= endx; ++x)
                    {
                        if (proto.NoConnectionsOnCorners && (x == 0 || x == endx)) continue;
                        AddConnection(x, endy, pointE);
                    }
                    return true;
                }

                Vector3 pointW = CellSetRegistry.GetConnectionListForType(Cell.Type.W);
                end = origin.Y + pointW.Y;

                if (Segment.EpsilonTest(start, end, 10.0f))
                {
                    for (int x = 0; x <= endx; ++x)
                    {
                        if (proto.NoConnectionsOnCorners && (x == 0 || x == endx)) continue;
                        AddConnection(x, 0, pointW);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool CheckAllowedConnections(int x, int y)
        {
            /* V10_NOTE: Unused in 1.10?
            if (!GetPrototype(out var proto)) return false;

            if (proto.AllowedConnections.HasValue())
            {
                foreach (var connection in proto.AllowedConnections)
                {
                    if (connection == null) continue;

                    Vector2 point = connection.ToIPoint2();
                    if (point.X == x && point.Y == y) return true;
                }
                return false;
            }
            */

            return true;
        }

        public static void CreateConnectedCellList(GenCell cell, List<uint> connectedCells)
        {
            if (cell != null)
            {
                foreach (var connectedCell in cell.Connections)
                    connectedCells.Add(connectedCell.Id);
            }
        }

        public bool CreateRequiredCells(GRandom random, RegionGenerator regionGenerator, List<PrototypeId> areas)
        {
            if (CellContainer == null) return false;
            if (LogDebug) Logger.Debug($"[{MethodBase.GetCurrentMethod().Name}] => {random}");
            Picker<Point2> picker = new(random);
            if (!GetPrototype(out var proto)) return false;

            bool failed = false;

            if (!failed && proto.RequiredSuperCells.HasValue())
            {
                foreach (var requiredCellBase in proto.RequiredSuperCells)
                {
                    if (!SpawnRequiredCellBase(random, picker, requiredCellBase))
                    {
                        if (Log) Logger.Warn($"Failed to place Required Cell. CELL={requiredCellBase} AREA={Area}");
                        failed = true;
                    }
                }
            }

            if (!failed && proto.NonRequiredSuperCells.HasValue())
            {
                Picker<RequiredCellBasePrototype> cellPicker = new(random);
                AddCellsToPicker(cellPicker, proto.NonRequiredSuperCells);
                if (!SpawnNonRequiredCellList(random, picker, cellPicker, proto.NonRequiredSuperCellsMin, proto.NonRequiredSuperCellsMax))
                {
                    if (Log) Logger.Warn($"Failed to place the minimum number of Non-Required SuperCells. CELLS={Logger.ObjectCollectionToString(proto.NonRequiredSuperCells)} AREA={Area}");
                    failed = true;
                }
            }

            if (!failed && _requiredTransitions.Count > 0)
            {
                foreach (RegionTransitionSpec spec in _requiredTransitions)
                {
                    PrototypeId cellRef = spec.GetCellRef();
                    if (proto.RequiresCell(cellRef)) continue;

                    if (cellRef == 0)
                    {
                        if (Log) Logger.Warn($"Reservable Cell {GameDatabase.GetFormattedPrototypeName(GameDatabase.GetDataRefByAsset(spec.Cell))} Does not Exist in Area {Area}");
                        continue;
                    }

                    FillPickerWithReservableCells(picker, cellRef);

                    if (!picker.Empty() && picker.Pick(out Point2 cellCoord))
                    {
                        CellContainer.ReserveCell(cellCoord.X, cellCoord.Y, cellRef,
                            spec.Start ? GenCell.GenCellType.Start : GenCell.GenCellType.Destination);
                    }
                    else
                    {
                        failed = true;
                        if (Log) Logger.Warn($"Failed to place Required Transition Cell. CELL={GameDatabase.GetFormattedPrototypeName(cellRef)} AREA={Area}");
                        break;
                    }
                }
            }

            if (Area != null)
            {
                if (!failed && Area.RandomInstances.Count > 0)
                {
                    foreach (var randomInstance in Area.RandomInstances)
                    {
                        if (randomInstance == null) continue;

                        AssetId cellAsset = randomInstance.OriginCell;
                        if (cellAsset == 0) continue;

                        PrototypeId cellRef = GameDatabase.GetDataRefByAsset(cellAsset);
                        if (cellRef == 0) continue;

                        FillPickerWithReservableCells(picker, cellRef, randomInstance);

                        if (!picker.Empty() && picker.Pick(out Point2 cellCoord))
                        {
                            if (CellContainer.ReserveCell(cellCoord.X, cellCoord.Y, cellRef, GenCell.GenCellType.Destination))
                            {
                                if (randomInstance.OverrideLocalPopulation != 0)
                                {
                                    GenCell cell = CellContainer.GetCell(cellCoord.X, cellCoord.Y);
                                    if (cell != null) cell.PopulationThemeOverrideRef = randomInstance.OverrideLocalPopulation;
                                }
                            }
                            else
                            {
                                failed = true;
                                if (Log) Logger.Warn($"Failed to place Random Instance Cell. CELL={GameDatabase.GetFormattedPrototypeName(cellRef)}");
                            }
                        }
                    }
                }
            }

            if (!failed && proto.RequiredCells.HasValue())
            {
                foreach (var requiredCellBase in proto.RequiredCells)
                {
                    if (!SpawnRequiredCellBase(random, picker, requiredCellBase))
                    {
                        failed = true;
                        if (Log) Logger.Warn($"Failed to place 'RequiredCells'. CELLS={requiredCellBase}");
                    }
                }
            }

            if (!failed)
            {
                failed = !regionGenerator.GetRequiredPOICellsForArea(Area, random, out var list);
                if (!failed)
                {
                    foreach (Prototype requiredCellBase in list)
                    {
                        if (!SpawnRequiredCellBase(random, picker, (RequiredCellBasePrototype)requiredCellBase))
                        {
                            failed = true;
                            if (Log) Logger.Warn($"Failed to place RequiredPOI. CELLS={requiredCellBase}");
                        }
                    }
                }
            }

            if (!failed && proto.NonRequiredNormalCells.HasValue())
            {
                Picker<RequiredCellBasePrototype> cellPicker = new(random);
                AddCellsToPicker(cellPicker, proto.NonRequiredNormalCells);
                if (!SpawnNonRequiredCellList(random, picker, cellPicker, proto.NonRequiredNormalCellsMin, proto.NonRequiredNormalCellsMax))
                {
                    failed = true;
                    if (Log) Logger.Warn($"Failed to place the minimum number of Non-Required Normal Cells. CELLS={Logger.ObjectCollectionToString(proto.NonRequiredNormalCells)}");
                }
            }
            return !failed;
        }

        private bool FillPickerWithReservableCells(Picker<Point2> picker, PrototypeId cellRef, RequiredCellBasePrototype requiredCell = null)
        {
            if (CellContainer == null) return false;
            picker.Clear();

            for (int y = 0; y < CellContainer.Height; ++y)
            {
                for (int x = 0; x < CellContainer.Width; ++x)
                {
                    if ((requiredCell == null || CheckRequiredCellLocationRestrictions(requiredCell, x, y))
                        && CellContainer.ReservableCell(x, y, cellRef))
                        picker.Add(new(x, y));
                }
            }
            return !picker.Empty();
        }

        private bool CheckRequiredCellLocationRestrictions(RequiredCellBasePrototype requiredCell, int x, int y)
        {
            /* V10_NOTE: No location restrictions in 1.10?
            if (requiredCell == null || requiredCell.LocationRestrictions.IsNullOrEmpty()) return true;

            foreach (var requiredCellRestrict in requiredCell.LocationRestrictions)
                if (requiredCellRestrict.CheckPoint(x, y, CellContainer.Width, CellContainer.Height)) return true;

            return false;
            */
            return true;
        }

        private bool SpawnNonRequiredCellList(GRandom random, Picker<Point2> picker, Picker<RequiredCellBasePrototype> cellPicker, int min, int max)
        {
            if (max <= 0 || min > max) return false;

            int next = random.Next(min, max + 1);
            int cellIndex = 0;

            while (!cellPicker.Empty())
            {
                if (cellPicker.PickRemove(out RequiredCellBasePrototype cellBase) && cellBase != null)
                {
                    if (SpawnRequiredCellBase(random, picker, cellBase))
                    {
                        cellIndex++;
                        if (cellIndex == next || cellIndex == max) break;
                    }
                }
            }

            return cellIndex >= min;
        }

        private static void AddCellsToPicker(Picker<RequiredCellBasePrototype> cellPicker, RequiredCellBasePrototype[] requiredCellBase)
        {
            foreach (var cell in requiredCellBase) cellPicker.Add(cell);
        }

        private bool SpawnRequiredCellBase(GRandom random, Picker<Point2> picker, RequiredCellBasePrototype requiredCellBase)
        {
            if (requiredCellBase == null) return false;

            if (requiredCellBase is RequiredSuperCellEntryPrototype requiredSuperCellEntry)
            {
                SuperCellPrototype superCell = GameDatabase.GetPrototype<SuperCellPrototype>(requiredSuperCellEntry.SuperCell);
                if (superCell == null) return false;

                if (!TrySpawningSuperCell(requiredSuperCellEntry, random, superCell)) return false;
                return true;
            }

            if (requiredCellBase is RequiredCellPrototype requiredCell)
            {
                AssetId cellAssetRef = requiredCell.Cell;
                if (cellAssetRef == 0)
                {
                    if (Log) Logger.Trace($"{Region}\n  Generator contains a RequiredCell entry that has an empty cell field.");
                    return false;
                }

                PrototypeId cellRef = GameDatabase.GetDataRefByAsset(cellAssetRef);
                if (cellRef == 0)
                {
                    if (Log) Logger.Trace($"{Region}\n  Generator contains a RequiredCell Asset, {GameDatabase.GetAssetName(cellAssetRef)}, that does not match the corresponding filename");
                    return false;
                }

                FillPickerWithReservableCells(picker, cellRef, requiredCell);

                if (!picker.Empty() && picker.Pick(out Point2 cellCoord))
                {
                    CellContainer.ReserveCell(cellCoord.X, cellCoord.Y, cellRef,
                        requiredCell.Destination ? GenCell.GenCellType.Destination : GenCell.GenCellType.None);

                    if (requiredCell.PopulationThemeOverride != 0)
                    {
                        GenCell cell = CellContainer.GetCell(cellCoord.X, cellCoord.Y);
                        if (cell != null) cell.PopulationThemeOverrideRef = requiredCell.PopulationThemeOverride;
                    }

                    return true;
                }
            }

            return false;
        }

        private bool TrySpawningSuperCell(RequiredSuperCellEntryPrototype requiredSuperCellEntry, GRandom random, SuperCellPrototype superCell)
        {
            if (superCell == null || superCell.Entries.IsNullOrEmpty()) return false;

            Picker<Point2> picker = new(random);
            for (int x = 0; x < CellContainer.Width - superCell.Max.X; x++)
            {
                for (int y = 0; y < CellContainer.Height - superCell.Max.Y; y++)
                {
                    if (CheckRequiredCellLocationRestrictions(requiredSuperCellEntry, x, y))
                        picker.Add(new(x, y));
                }
            }

            bool success = false;
            Point2 pick = new(0, 0);
            while (!picker.Empty() && !success)
            {
                if (picker.PickRemove(out pick))
                {
                    success = true;
                    foreach (SuperCellEntryPrototype superCellEntry in superCell.Entries)
                    {
                        if (superCellEntry == null) continue;
                        Point2 cellCoord = new(pick.X + superCellEntry.Offset.X, pick.Y + superCellEntry.Offset.Y);
                        if (!CellContainer.ReservableCell(cellCoord.X, cellCoord.Y, GameDatabase.GetDataRefByAsset(superCellEntry.Cell)))
                            success = false;
                    }
                }
            }

            if (success)
            {
                List<PrototypeId> list = ListPool<PrototypeId>.Instance.Get();
                foreach (SuperCellEntryPrototype superCellEntry in superCell.Entries)
                {
                    if (superCellEntry == null) continue;
                    Point2 cellCoord = new(pick.X + superCellEntry.Offset.X, pick.Y + superCellEntry.Offset.Y);
                    if (!CellContainer.ReservableCell(cellCoord.X, cellCoord.Y, GameDatabase.GetDataRefByAsset(superCellEntry.Cell))) continue;

                    PrototypeId cellRef = GameDatabase.GetDataRefByAsset(superCellEntry.Cell);  // V10_NOTE: No alt cells in 1.10
                    CellContainer.ReserveCell(cellCoord.X, cellCoord.Y, cellRef, GenCell.GenCellType.None);
                    list.Add(cellRef);

                    if (requiredSuperCellEntry.PopulationThemeOverride != 0)
                    {
                        GenCell cell = CellContainer.GetCell(cellCoord.X, cellCoord.Y);
                        cell.PopulationThemeOverrideRef = requiredSuperCellEntry.PopulationThemeOverride;
                    }

                    RemoveCellFromRegionTransitionSpecList(superCellEntry.Cell);
                }
                ListPool<PrototypeId>.Instance.Return(list);
            }

            return success;
        }

        private void RemoveCellFromRegionTransitionSpecList(AssetId cell)
        {
            _requiredTransitions.RemoveAll(transition => cell == transition.Cell);
        }

        public bool GenerateRandomInstanceLinks(GRandom random)
        {
            if (Area == null) return false;
            if (LogDebug) Logger.Debug($"[{MethodBase.GetCurrentMethod().Name}] => {random}");
            List<RandomInstanceRegionPrototype> randomInstances = Area.RandomInstances;
            randomInstances.Clear();

            if (!GetPrototype(out var proto)) return false;

            if (proto.RandomInstances != null)
            {
                RandomInstanceListPrototype randomInstanceList = proto.RandomInstances;
                if (randomInstanceList != null && randomInstanceList.List != null)
                {
                    Picker<RandomInstanceRegionPrototype> picker = new(random);
                    foreach (var randomInstanceRegion in randomInstanceList.List)
                    {
                        if (randomInstanceRegion == null) continue;
                        picker.Add(randomInstanceRegion, randomInstanceRegion.Weight);
                    }

                    int picks = randomInstanceList.Picks;
                    while (picks > 0 && !picker.Empty())
                    {
                        if (picker.PickRemove(out RandomInstanceRegionPrototype pick))
                            randomInstances.Add(pick);
                        picks--;
                    }
                }
            }

            return true;
        }

        public void ProcessDeleteExtraneousCells(GRandom random, int chance)
        {
            if (CellContainer == null) return;
            if (LogDebug) Logger.Debug($"[{MethodBase.GetCurrentMethod().Name}] => {random}");
            // number of cells to be deleted
            int cells = chance * CellContainer.NumCells / 100;
            GetPrototype(out BaseGridAreaGeneratorPrototype proto);

            void CheckRoomKill(CellDeletionEnum method)
            {
                if (method == CellDeletionEnum.Random)
                    DeleteGuessAndCheck(random, cells);
                else if (method == CellDeletionEnum.Edge)
                    DeleteEdgeCreep(random, cells);
                else if (method == CellDeletionEnum.Corner)
                    DeleteCornerCreep(random, cells);
            }

            CheckRoomKill(proto.RoomKillMethod);

            if (proto.SecondaryDeletionProfiles != null)
            {
                foreach (var profile in proto.SecondaryDeletionProfiles)
                {
                    cells = (int)(profile.RoomKillPct * CellContainer.NumCells) / 100;
                    CheckRoomKill(profile.RoomKillMethod);
                }
            }
        }

        private void DeleteEdgeCreep(GRandom random, int cells)
        {
            if (!GetPrototype(out var proto)) return;

            List<Point2> deleteList = new();
            int min = Math.Min(proto.CellsX / 2, proto.CellsY / 2);
            for (int radius = 0; radius < min && cells > 0; ++radius)
            {
                Picker<Point2> picker = new(random);
                while (cells > 0 && GetEdgeRadiusDeletableCellList(deleteList, radius, true))
                {
                    picker.Clear();
                    foreach (var point in deleteList)
                        picker.Add(point);

                    if (picker.PickRemove(out Point2 cellCoord))
                    {
                        if (CellContainer.GetCell(cellCoord.X, cellCoord.Y) != null
                            && CellContainer.DestroyableCell(cellCoord.X, cellCoord.Y))
                        {
                            CellContainer.DestroyCell(cellCoord.X, cellCoord.Y);
                            --cells;
                            continue;
                        }
                    }
                }
            }
        }

        private void DeleteCornerCreep(GRandom random, int cells)
        {
            if (!GetPrototype(out var proto)) return;

            List<Point2> deleteList = new();
            int max = Math.Max(proto.CellsX / 2, proto.CellsY / 2);
            for (int radius = 0; radius < max && cells > 0; ++radius)
            {
                Picker<Point2> picker = new(random);
                while (cells > 0 && GetCornerRadusDeletableCellList(deleteList, radius, true))
                {
                    picker.Clear();
                    foreach (var point in deleteList)
                        picker.Add(point);

                    if (picker.PickRemove(out Point2 cellCoord))
                    {
                        if (CellContainer.GetCell(cellCoord.X, cellCoord.Y) != null
                            && CellContainer.DestroyableCell(cellCoord.X, cellCoord.Y))
                        {
                            CellContainer.DestroyCell(cellCoord.X, cellCoord.Y);
                            --cells;
                        }
                    }
                }
            }
        }

        private bool GetEdgeRadiusDeletableCellList(List<Point2> deleteList, int radius, bool clear)
        {
            if (clear) deleteList.Clear();
            if (!GetPrototype(out var proto)) return false;

            if (radius < proto.CellsX - radius - 1)
            {
                for (int x = radius; x < proto.CellsX - radius - 1; ++x)
                {
                    UniqueAddDeletableCell(CellContainer, deleteList, x, radius);
                    UniqueAddDeletableCell(CellContainer, deleteList, x, proto.CellsY - radius - 1);
                }
            }
            else if (radius == proto.CellsX - radius - 1)
            {
                for (int x = radius; x < proto.CellsX - radius - 1; ++x)
                {
                    UniqueAddDeletableCell(CellContainer, deleteList, x, radius);
                }
            }

            if (radius + 1 < proto.CellsY - radius - 2)
            {
                for (int y = radius + 1; y < proto.CellsY - radius - 2; ++y)
                {
                    UniqueAddDeletableCell(CellContainer, deleteList, radius, y);
                    UniqueAddDeletableCell(CellContainer, deleteList, proto.CellsX - radius - 1, y);
                }
            }
            else if (radius + 1 == proto.CellsY - radius - 2)
            {
                for (int y = radius + 1; y < proto.CellsY - radius - 2; ++y)
                {
                    UniqueAddDeletableCell(CellContainer, deleteList, radius, radius + 1);
                }
            }

            return deleteList.Count > 0;
        }

        private bool GetCornerRadusDeletableCellList(List<Point2> deleteList, int radius, bool clear)
        {
            if (clear) deleteList.Clear();

            if (!GetPrototype(out var proto)) return false;

            for (int x = 0; x <= radius; ++x)
            {
                UniqueAddDeletableCell(CellContainer, deleteList, x, radius);
                UniqueAddDeletableCell(CellContainer, deleteList, x, proto.CellsY - 1 - radius);
            }

            for (int y = 0; y <= radius; ++y)
            {
                UniqueAddDeletableCell(CellContainer, deleteList, radius, y);
                UniqueAddDeletableCell(CellContainer, deleteList, proto.CellsX - 1 - radius, y);
            }

            for (int x = proto.CellsX - 1; x >= proto.CellsX - 1 - radius; --x)
            {
                UniqueAddDeletableCell(CellContainer, deleteList, x, proto.CellsY - 1 - radius);
                UniqueAddDeletableCell(CellContainer, deleteList, x, radius);
            }

            for (int y = proto.CellsY - 1; y >= proto.CellsY - 1 - radius; --y)
            {
                UniqueAddDeletableCell(CellContainer, deleteList, proto.CellsX - 1 - radius, y);
                UniqueAddDeletableCell(CellContainer, deleteList, radius, y);
            }

            return deleteList.Count > 0;
        }

        private static void UniqueAddDeletableCell(GenCellGridContainer cellContainer, List<Point2> deleteList, int x, int y)
        {
            if (cellContainer.GetCell(x, y) != null
                && cellContainer.DestroyableCell(x, y))
            {
                Point2 cellCoord = new(x, y);
                foreach (var deleteCoord in deleteList)
                    if (deleteCoord == cellCoord) return;

                deleteList.Add(cellCoord);
            }
        }

        private void DeleteGuessAndCheck(GRandom random, int cells)
        {
            Picker<Point2> picker = new(random);

            for (int y = 0; y < CellContainer.Height; ++y)
            {
                for (int x = 0; x < CellContainer.Width; ++x)
                {
                    if (CellContainer.GetCell(x, y) != null
                        && CellContainer.DestroyableCell(x, y))
                        picker.Add(new(x, y));
                }
            }

            while (cells > 0 && !picker.Empty())
            {
                if (picker.PickRemove(out Point2 cellCoord))
                {
                    if (CellContainer.GetCell(cellCoord.X, cellCoord.Y) != null
                        && CellContainer.DestroyableCell(cellCoord.X, cellCoord.Y))
                    {
                        CellContainer.DestroyCell(cellCoord.X, cellCoord.Y);
                        --cells;
                    }
                }
            }
        }

        public bool EstablishExternalConnections()
        {
            if (CellContainer == null) return false;
            GetPrototype(out var proto);

            Aabb regionBounds = Area.RegionBounds;
            float cellSize = proto.CellSize;
            float halfCellSize = cellSize / 2.0f;
            Vector3 origin = Area.Origin;

            Area previousArea = Region.ProgressionGraph.GetPreviousArea(Area);

            foreach (AreaConnectionPoint areaConnection in Area.AreaConnections)
            {
                Vector3 position = areaConnection.Position;

                float x = position.X + halfCellSize - origin.X - 1.0f;
                float y = position.Y + halfCellSize - origin.Y - 1.0f;
                int xCell = (int)(x / cellSize);
                int yCell = (int)(y / cellSize);

                GenCell cell = CellContainer.GetCell(xCell, yCell);
                if (cell == null) continue;

                GenCell.GenCellType type = GenCell.GenCellType.Destination;
                if (previousArea != null && previousArea == areaConnection.ConnectedArea)
                    type = GenCell.GenCellType.Start;

                Cell.Type connectionType = Cell.Type.None;
                if (Segment.EpsilonTest(regionBounds.Max.X, position.X, 10.0f))
                    connectionType = Cell.Type.N;
                else if (Segment.EpsilonTest(regionBounds.Max.Y, position.Y, 10.0f))
                    connectionType = Cell.Type.E;
                else if (Segment.EpsilonTest(regionBounds.Min.X, position.X, 10.0f))
                    connectionType = Cell.Type.S;
                else if (Segment.EpsilonTest(regionBounds.Min.Y, position.Y, 10.0f))
                    connectionType = Cell.Type.W;

                if (connectionType != Cell.Type.None)
                {
                    cell.SetExternalConnection(connectionType, areaConnection.ConnectedArea, areaConnection.ConnectPosition);
                    CellContainer.AddStartOrDestinationCell(cell, type);
                }
                else
                {
                    if (Log) Logger.Trace("No cells were flagged with connections.");
                }
            }
            return true;
        }

        public void ProcessCellPositions(float cellSize)
        {
            if (CellContainer == null) return;

            for (int x = 0; x < CellContainer.Width; x++)
            {
                for (int y = 0; y < CellContainer.Height; y++)
                {
                    GenCell cell = CellContainer.GetCell(x, y);
                    if (cell != null) cell.Position = GetCellOffset(x, y, cellSize);
                }
            }
        }

        private Vector3 GetCellOffset(int x, int y, float cellSize)
        {
            return new(x * cellSize, y * cellSize, IncrementZ + IncrementX * x + IncrementY * y);
        }

        public void ProcessAssignUniqueCellIds()
        {
            if (CellContainer == null) return;
            foreach (GenCell cell in CellContainer)
                if (cell != null) cell.Id = AllocateCellId();
        }

        public void ProcessRegionConnectionsAndDepth()
        {
            if (Region != null) CellContainer.DetermineCellDepthsAndShortestPath();
        }

        // V10_NOTE: No roads in 1.10

        public void BuildExcludedListLikeCells(int x, int y, Cell.Type cellType, List<PrototypeId> excludedList)
        {
            if (CellContainer == null) return;

            void AddExcludedCell(int tX, int tY)
            {
                if (CellContainer.TestCoord(tX, tY))
                {
                    GenCell testCell = CellContainer.GetCell(tX, tY);
                    if (testCell != null && testCell.CellRef != 0 && CellContainer.DetermineType(tX, tY) == cellType)
                        excludedList.Add(testCell.CellRef);
                }
            }

            AddExcludedCell(x + 1, y);  // N
            AddExcludedCell(x, y + 1);  // E
            AddExcludedCell(x - 1, y);  // S
            AddExcludedCell(x, y - 1);  // W 
        }

        public bool PlaceFillerRoom(GRandom random, int x, int y, Vector3 position)
        {
            Cell.Filler filler = Cell.Filler.None;

            if (CellContainer.GetCell(x + 1, y, false) != null) filler |= Cell.Filler.N;
            if (CellContainer.GetCell(x + 1, y + 1, false) != null) filler |= Cell.Filler.NE;
            if (CellContainer.GetCell(x, y + 1, false) != null) filler |= Cell.Filler.E;
            if (CellContainer.GetCell(x - 1, y + 1, false) != null) filler |= Cell.Filler.SE;
            if (CellContainer.GetCell(x - 1, y, false) != null) filler |= Cell.Filler.S;
            if (CellContainer.GetCell(x - 1, y - 1, false) != null) filler |= Cell.Filler.SW;
            if (CellContainer.GetCell(x, y - 1, false) != null) filler |= Cell.Filler.W;
            if (CellContainer.GetCell(x + 1, y - 1, false) != null) filler |= Cell.Filler.NW;

            PrototypeId cellRef = CellSetRegistry.GetCellSetAssetPickedByFiller(random, filler);
            if (cellRef == 0) cellRef = CellSetRegistry.GetCellSetAssetPickedByFiller(random, Cell.Filler.None);

            if (cellRef != 0)
            {
                CellSettings cellSettings = new()
                {
                    PositionInArea = position,
                    CellRef = cellRef
                };
                return Area.AddCell(AllocateCellId(), cellSettings) != null;
            }
            return false;
        }

        // V10_NOTE: No roads in 1.10

        private bool CellsBothInSuperCell(GenCell cellA, GenCell cellB)
        {
            if (!GetPrototype(out var proto)) return false;
            if (proto.RequiredSuperCells.IsNullOrEmpty()) return false;
            if (cellA == null || cellB == null || cellA.CellRef == 0 || cellB.CellRef == 0) return false;

            foreach (var superCellEntry in proto.RequiredSuperCells)
            {
                var superCellProto = GameDatabase.GetPrototype<SuperCellPrototype>(superCellEntry.SuperCell);
                if (superCellProto != null && superCellProto.ContainsCell(cellA.CellRef) && superCellProto.ContainsCell(cellB.CellRef))
                    return true;
            }
            return false;
        }

        private static bool Permutations(HashSet<int> setIndexes, List<int> workingStack, List<List<int>> results, int count)
        {
            if (workingStack.Count == count)
                results.Add(new(workingStack));

            List<int> indexes = ListPool<int>.Instance.Get(setIndexes);
            foreach (var index in indexes)
            {
                workingStack.Add(index);
                setIndexes.Remove(index);
                Permutations(setIndexes, workingStack, results, count);
                setIndexes.Add(workingStack[workingStack.Count - 1]);
                workingStack.RemoveAt(workingStack.Count - 1);
            }
            ListPool<int>.Instance.Return(indexes);

            return true;
        }

    }
}
