using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.GameData.Prototypes.Markers;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)Transition)]
    public enum RegionTransitionType
    {
        Transition,
        TransitionDirect,
        BodySliderDestination,
        BodySliderPlayer,
        Marker,
        Waypoint,
        TowerUp,
        TowerDown,
        PartyJoin,
        PartyWarpToMember,
        MatchJoin,
        Checkpoint,
        PvPFactionJoin,
        PvEWaveBattleJoin,
        TransitionDirectReturn,
    }

    #endregion

    public class TransitionPrototype : WorldEntityPrototype
    {
        public RegionTransitionType Type { get; protected set; }
        public int SpawnOffset { get; protected set; }
        public PrototypeId Waypoint { get; protected set; }
        public bool SupressBlackout { get; protected set; }
        public bool ShowIndicator { get; protected set; }
        public PrototypeId Checkpoint { get; protected set; }
        public bool ShowConfirmationDialog { get; protected set; }
        public bool UseOrientationForAvatar { get; protected set; }
        public PrototypeId DirectTarget { get; protected set; }

        //---

        public Vector3 CalcSpawnOffset(in Orientation rotation)
        {
            return new(SpawnOffset * MathF.Cos(rotation.Yaw),
                       SpawnOffset * MathF.Sin(rotation.Yaw),
                       0f);
        }

        public static Vector3 CalcSpawnOffset(EntityMarkerPrototype entityMarkerProto)
        {
            var transitionProto = entityMarkerProto?.GetMarkedPrototype<TransitionPrototype>();
            if (transitionProto == null) return Vector3.Zero;

            return transitionProto.CalcSpawnOffset(entityMarkerProto.Rotation);
        }
    }
}
