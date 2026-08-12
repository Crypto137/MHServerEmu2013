using MHServerEmu.Core.Logging;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Regions;

namespace MHServerEmu.Games.Network
{
    public class TransferParams
    {
        // This class determines where a player needs to be put after loading into a game.
        // According to PlayerMgrToGameServer protocol from 1.53, it was sent as a NetStructTransferParams
        // in a GameAndRegionForPlayer message from the player manager to the GIS when a player connects.

        private static readonly Logger Logger = LogManager.CreateLogger();

        public PlayerConnection PlayerConnection { get; }

        public ulong DestRegionId { get; set; }

        public PrototypeId DestTargetRegionProtoRef { get; private set; }
        public PrototypeId DestTargetAreaProtoRef { get; private set; }
        public PrototypeId DestTargetCellProtoRef { get; private set; }
        public PrototypeId DestTargetEntityProtoRef { get; private set; }

        public TransferParams(PlayerConnection playerConnection)
        {
            PlayerConnection = playerConnection;
        }

        public void SetTarget(PrototypeId targetProtoRef, PrototypeId regionProtoRefOverride = PrototypeId.Invalid)
        {
            var targetProto = GameDatabase.GetPrototype<RegionConnectionTargetPrototype>(targetProtoRef);
            if (!Verify.IsNotNull(targetProto)) return;

            DestTargetRegionProtoRef = regionProtoRefOverride != PrototypeId.Invalid ? regionProtoRefOverride : targetProto.Region;
            DestTargetAreaProtoRef = targetProto.Area;
            DestTargetCellProtoRef = GameDatabase.GetDataRefByAsset(targetProto.Cell);
            DestTargetEntityProtoRef = targetProto.Entity;
        }

        public bool FindStartLocation(out Vector3 position, out Orientation orientation)
        {
            position = Vector3.Zero;
            orientation = Orientation.Zero;

            Game game = PlayerConnection.Game;

            Region region = game.RegionManager.GetRegion(DestRegionId);
            if (!Verify.IsNotNull(region)) return false;

            Area startArea = region.GetStartArea();
            if (!Verify.IsNotNull(startArea)) return false;

            if (region.FindTargetLocation(ref position, ref orientation, DestTargetAreaProtoRef, DestTargetCellProtoRef, DestTargetEntityProtoRef))
                return true;

            // Fall back to the center of the first cell in the start area if all else fails
            position = startArea.Cells.First().Value.RegionBounds.Center;
            Logger.Warn($"FindStartPosition(): Failed to find target location, falling back to {position} as the last resort!");
            return true;
        }
    }
}
