using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Properties;

namespace MHServerEmu.Games.Entities.Locomotion
{
    public class Locomotor
    {
        private WorldEntity _owner;
        public LocomotionState LocomotionState { get; private set; }
        public float DefaultRunSpeed { get => _runSpeed; }
        public float Height { get; private set; }

        public bool SupportsWalking { get; private set; }
        public LocomotionState LastSyncState { get; private set; }

        private float _giveUpDistanceThreshold;
        private TimeSpan _giveUpTime;

        private float _runSpeed;
        private float _walkSpeed;
        private float _rotationSpeed;

        private LocomotorMethod _defaultMethod;
        private LocomotionState _lastLocomotionState;

        public Locomotor()
        {
            LocomotionState = new();
            _lastLocomotionState = new();
            LastSyncState = new();
        }

        public void Initialize(LocomotorPrototype locomotorProto, WorldEntity entity, float heightOverride = 0.0f)
        {
            _owner = entity;
            if (entity != null && entity.Properties.HasProperty(PropertyEnum.MissileBaseMoveSpeed))
                _runSpeed = entity.Properties[PropertyEnum.MissileBaseMoveSpeed];
            else
                _runSpeed = locomotorProto.Speed;

            SupportsWalking = locomotorProto.WalkEnabled;
            _walkSpeed = locomotorProto.WalkSpeed;
            _rotationSpeed = locomotorProto.RotationSpeed;
            Height = heightOverride != 0.0f ? heightOverride : locomotorProto.Height;

            if (_owner != null)
            {
                var worldEntityProto = _owner.WorldEntityPrototype;
                if (worldEntityProto != null)
                    _defaultMethod = worldEntityProto.NaviMethod;
            }

            LocomotionState.Method = _defaultMethod;
            LocomotionState.BaseMoveSpeed = DefaultRunSpeed;
        }

        public void SetGiveUpLimits(float distanceThreshold, TimeSpan time)
        {
            _giveUpDistanceThreshold = distanceThreshold;
            _giveUpTime = time;
        }

        public void SetSyncState(LocomotionState locomotionState, Vector3 syncPosition, Orientation syncOrientation)
        {
            LastSyncState.Set(locomotionState);
            LocomotionState.Set(locomotionState);

            PushLocomotionStateChanges();
        }

        private void PushLocomotionStateChanges()
        {
            if (LocomotionState.LocomotionFlags.HasFlag(LocomotionFlags.IsSyncMoving) == false)
            {
                _owner.OnLocomotionStateChanged(_lastLocomotionState, LocomotionState);
                _lastLocomotionState.Set(LocomotionState);
            }
        }
    }
}
