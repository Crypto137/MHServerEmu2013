using System.Text;
using Gazillion;
using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.Helpers;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Navi;

namespace MHServerEmu.Games.Entities.Locomotion
{
    [Flags]
    public enum LocomotionMessageFlags : uint
    {
        None                    = 0,
        HasFullOrientation      = 1 << 0,
        NoLocomotionState       = 1 << 1,
        RelativeToPreviousState = 1 << 2,   // See LocomotionState::GetFieldFlags()
        HasLocomotionFlags      = 1 << 3,
        HasMethod               = 1 << 4,
        UpdatePathNodes         = 1 << 5,
        LocomotionFinished      = 1 << 6,
        HasMoveSpeed            = 1 << 7,
        HasHeight               = 1 << 8,
        HasFollowEntityId       = 1 << 9,
        HasFollowEntityRange    = 1 << 10,
        HasEntityPrototypeRef   = 1 << 11
    }

    [Flags]
    public enum LocomotionFlags : ulong
    {
        None                        = 0,
        IsLocomoting                = 1 << 0,
        IsWalking                   = 1 << 1,
        IsLooking                   = 1 << 2,
        SkipCurrentSpeedRate        = 1 << 3,
        LocomotionNoEntityCollide   = 1 << 4,
        IsMovementPower             = 1 << 5,
        DisableOrientation          = 1 << 6,
        IsDrivingMovementMode       = 1 << 7,
        MoveForward                 = 1 << 8,
        MoveTo                      = 1 << 9,
        IsSyncMoving                = 1 << 10,
        IgnoresWorldCollision       = 1 << 11,
    }

    /// <summary>
    /// Represents state of a <see cref="Locomotor"/>.
    /// </summary>
    public class LocomotionState : IPoolable, IDisposable
    {
        // TODO: For optimization reasons it may be a good idea to change this to a struct.
        // However, it is going to be large + mutable + have a reference type as one of its
        // fields (List<NaviPathNode>), so this should be done with care and only if actually needed.

        private static readonly Logger Logger = LogManager.CreateLogger();

        // NOTE: Due to how LocomotionState serialization is implemented, we should be able to
        // get away with using C# auto properties instead of private fields.
        public LocomotionFlags LocomotionFlags { get; set; }
        public LocomotorMethod Method { get; set; } = LocomotorMethod.Default;
        public float BaseMoveSpeed { get; set; }
        public int Height { get; set; }
        public ulong FollowEntityId { get; set; }
        public float FollowEntityRangeStart { get; set; }
        public float FollowEntityRangeEnd { get; set; }
        public int PathGoalNodeIndex { get; set; }
        public List<NaviPathNode> PathNodes { get; set; } = new();

        public bool IsInPool { get; set; }

        public LocomotionState() { }

        public LocomotionState(LocomotionState other)
        {
            Set(other);
        }

        public void Set(LocomotionState other)
        {
            LocomotionFlags = other.LocomotionFlags;
            Method = other.Method;
            BaseMoveSpeed = other.BaseMoveSpeed;
            Height = other.Height;
            FollowEntityId = other.FollowEntityId;
            FollowEntityRangeStart = other.FollowEntityRangeStart;
            FollowEntityRangeEnd = other.FollowEntityRangeEnd;
            PathGoalNodeIndex = other.PathGoalNodeIndex;

            // NOTE: Is it okay to add path nodes here by reference? Do we need a copy?
            // Review this if/when we change NaviPathNode to struct.
            PathNodes.Set(other.PathNodes);
        }

        public void ResetForPool()
        {
            LocomotionFlags = default;
            Method = LocomotorMethod.Default;
            BaseMoveSpeed = default;
            Height = default;
            FollowEntityId = default;
            FollowEntityRangeStart = default;
            FollowEntityRangeEnd = default;
            PathGoalNodeIndex = default;
            PathNodes.Clear();
        }

        public void Dispose()
        {
            ObjectPoolManager.Instance.Return(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"{nameof(LocomotionFlags)}: {LocomotionFlags}");
            sb.AppendLine($"{nameof(Method)}: {Method}");
            sb.AppendLine($"{nameof(BaseMoveSpeed)}: {BaseMoveSpeed}");
            sb.AppendLine($"{nameof(Height)}: {Height}");
            sb.AppendLine($"{nameof(FollowEntityId)}: {FollowEntityId}");
            sb.AppendLine($"{nameof(FollowEntityRangeStart)}: {FollowEntityRangeStart}");
            sb.AppendLine($"{nameof(FollowEntityRangeEnd)}: {FollowEntityRangeEnd}");
            sb.AppendLine($"{nameof(PathGoalNodeIndex)}: {PathGoalNodeIndex}");
            for (int i = 0; i < PathNodes.Count; i++)
                sb.AppendLine($"{nameof(PathNodes)}[{i}]: {PathNodes[i]}");
            return sb.ToString();
        }

        // V10_NOTE: 1.10 uses protobuf messages instead of archive serialization for replicating locomotion state

        public static bool SerializeTo(NetStructLocomotionState.Builder protobuf, LocomotionState state, LocomotionMessageFlags flags)
        {
            bool success = true;

            if (flags.HasFlag(LocomotionMessageFlags.HasLocomotionFlags))
                protobuf.SetLocomotionflags((uint)state.LocomotionFlags);

            if (flags.HasFlag(LocomotionMessageFlags.HasMethod))
                protobuf.SetMethod((int)state.Method);

            if (flags.HasFlag(LocomotionMessageFlags.HasMoveSpeed))
                protobuf.SetMovespeed(state.BaseMoveSpeed);

            if (flags.HasFlag(LocomotionMessageFlags.HasHeight))
                protobuf.SetHeight((uint)state.Height);

            if (flags.HasFlag(LocomotionMessageFlags.HasFollowEntityId))
                protobuf.SetFollowentityid(state.FollowEntityId);

            if (flags.HasFlag(LocomotionMessageFlags.HasFollowEntityRange))
                protobuf.SetFollowentityrange(state.FollowEntityRangeStart);

            bool updatePathNodes = flags.HasFlag(LocomotionMessageFlags.UpdatePathNodes);
            protobuf.SetUpdatepathnodes(updatePathNodes);

            if (updatePathNodes)
            {
                if (state.PathGoalNodeIndex < 0) Logger.Warn("SerializeTo(): state.PathGoalNodeIndex < 0");

                protobuf.SetPathgoalnodeindex(state.PathGoalNodeIndex);

                foreach (NaviPathNode pathNode in state.PathNodes)
                {
                    // Pack vertex side + radius into a single value
                    int vertexSideRadius = MathHelper.RoundUpToInt(pathNode.Radius);
                    if (pathNode.VertexSide == NaviSide.Left) vertexSideRadius = -vertexSideRadius;

                    protobuf.AddPathnodes(NetStructLocomotionPathNode.CreateBuilder()
                        .SetVertex(pathNode.Vertex.ToNetStructPoint3())
                        .SetVertexSideRadius(vertexSideRadius));
                }
            }

            return success;
        }

        public static bool SerializeFrom(NetStructLocomotionState protobuf, LocomotionState state)
        {
            if (protobuf.HasLocomotionflags)
                state.LocomotionFlags = (LocomotionFlags)protobuf.Locomotionflags;

            if (protobuf.HasMethod)
                state.Method = (LocomotorMethod)protobuf.Method;

            if (protobuf.HasMovespeed)
                state.BaseMoveSpeed = protobuf.Movespeed;

            if (protobuf.HasHeight)
                state.Height = (int)protobuf.Height;

            if (protobuf.HasFollowentityid)
                state.FollowEntityId = protobuf.Followentityid;

            if (protobuf.HasFollowentityrange)
            {
                // Just a single value instead of start/end for follow range in 1.10,
                // setting both to the same value should have the same result in theory.
                state.FollowEntityRangeStart = protobuf.Followentityrange;
                state.FollowEntityRangeEnd = protobuf.Followentityrange;
            }

            if (protobuf.Updatepathnodes)
            {
                if (protobuf.HasPathgoalnodeindex)
                    state.PathGoalNodeIndex = protobuf.Pathgoalnodeindex;

                state.PathNodes.Clear();
                for (int i = 0; i < protobuf.PathnodesCount; i++)
                {
                    NetStructLocomotionPathNode protobufPathNode = protobuf.PathnodesList[i];

                    NaviPathNode pathNode = new();
                    pathNode.Vertex = new(protobufPathNode.Vertex);

                    int vertexSideRadius = protobufPathNode.VertexSideRadius;
                    if (vertexSideRadius < 0)
                    {
                        pathNode.VertexSide = NaviSide.Left;
                        pathNode.Radius = -vertexSideRadius;
                    }
                    else if (vertexSideRadius > 0)
                    {
                        pathNode.VertexSide = NaviSide.Right;
                        pathNode.Radius = vertexSideRadius;
                    }
                    else /* if (vertexSideRadius == 0) */
                    {
                        pathNode.VertexSide = NaviSide.Point;
                        pathNode.Radius = 0f;
                    }

                    state.PathNodes.Add(pathNode);
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two <see cref="LocomotionState"/> instances and returns <see cref="LocomotionMessageFlags"/> for serialization.
        /// </summary>
        public static LocomotionMessageFlags GetFieldFlags(LocomotionState currentState, LocomotionState previousState, bool withPathNodes)
        {
            if (currentState == null) return LocomotionMessageFlags.NoLocomotionState;

            LocomotionMessageFlags flags = LocomotionMessageFlags.None;

            if (previousState != null)
            {
                // If we have a previous state, it means we are sending a relative update that contains only what has changed
                flags |= LocomotionMessageFlags.RelativeToPreviousState;

                if (currentState.LocomotionFlags != previousState.LocomotionFlags)
                    flags |= LocomotionMessageFlags.HasLocomotionFlags;

                if (currentState.Method != previousState.Method)
                    flags |= LocomotionMessageFlags.HasMethod;

                if (currentState.BaseMoveSpeed != previousState.BaseMoveSpeed)
                    flags |= LocomotionMessageFlags.HasMoveSpeed;

                if (currentState.Height != previousState.Height)
                    flags |= LocomotionMessageFlags.HasHeight;

                if (currentState.FollowEntityId != previousState.FollowEntityId)
                    flags |= LocomotionMessageFlags.HasFollowEntityId;

                if (currentState.FollowEntityRangeStart != previousState.FollowEntityRangeStart)
                    flags |= LocomotionMessageFlags.HasFollowEntityRange;

                if (withPathNodes)
                {
                    bool isLocomoting = currentState.LocomotionFlags.HasFlag(LocomotionFlags.IsLocomoting);
                    bool isLooking = currentState.LocomotionFlags.HasFlag(LocomotionFlags.IsLooking);

                    if (isLocomoting || isLooking)
                        flags |= LocomotionMessageFlags.UpdatePathNodes;
                    else if ((previousState.LocomotionFlags.HasFlag(LocomotionFlags.IsLocomoting) && isLocomoting == false)
                        || (previousState.LocomotionFlags.HasFlag(LocomotionFlags.IsLooking) && isLooking == false))
                    {
                        // If we were locomoting or looking, and no longer are, flag the current locomotion state as finished
                        flags |= LocomotionMessageFlags.LocomotionFinished;
                    }
                }
            }
            else
            {
                // If no previous state is provided, it means we are sending a full locomotion state (we still omit default values)
                if (currentState.LocomotionFlags != LocomotionFlags.None)
                    flags |= LocomotionMessageFlags.HasLocomotionFlags;

                if (currentState.Method != LocomotorMethod.Ground)
                    flags |= LocomotionMessageFlags.HasMethod;

                if (currentState.BaseMoveSpeed != 0f)
                    flags |= LocomotionMessageFlags.HasMoveSpeed;

                if (currentState.Height != 0)
                    flags |= LocomotionMessageFlags.HasHeight;

                if (currentState.FollowEntityId != 0)
                    flags |= LocomotionMessageFlags.HasFollowEntityId;

                if (currentState.FollowEntityRangeStart != 0f)
                    flags |= LocomotionMessageFlags.HasFollowEntityRange;

                if (withPathNodes)
                    flags |= LocomotionMessageFlags.UpdatePathNodes;
            }

            return flags;
        }

        /// <summary>
        /// Compares two <see cref="LocomotionState"/> instances and returns whether or not sync is required.
        /// </summary>
        public static void CompareLocomotionStatesForSync(LocomotionState newState, LocomotionState oldState, out bool syncRequired, out bool pathNodeSyncRequired, bool skipGoalNode)
        {
            pathNodeSyncRequired = CompareLocomotionPathNodesForSync(newState, oldState, skipGoalNode);

            syncRequired = newState.LocomotionFlags != oldState.LocomotionFlags;
            syncRequired |= newState.BaseMoveSpeed != oldState.BaseMoveSpeed;
            syncRequired |= newState.Height != oldState.Height;
            syncRequired |= newState.Method != oldState.Method;
            syncRequired |= newState.FollowEntityId != oldState.FollowEntityId;
            syncRequired |= newState.FollowEntityRangeStart != oldState.FollowEntityRangeStart;
            syncRequired |= newState.FollowEntityRangeEnd != oldState.FollowEntityRangeEnd;
        }

        /// <summary>
        /// Compares <see cref="NaviPathNode"/> collections of two <see cref="LocomotionState"/> instances
        /// and returns <see langword="true"/> if an update is required.
        /// </summary>
        public static bool CompareLocomotionPathNodesForSync(LocomotionState newState, LocomotionState oldState, bool skipGoalNode)
        {
            if ((newState.LocomotionFlags.HasFlag(LocomotionFlags.IsLocomoting) ^ oldState.LocomotionFlags.HasFlag(LocomotionFlags.IsLocomoting))
                || (newState.LocomotionFlags.HasFlag(LocomotionFlags.IsLooking) ^ oldState.LocomotionFlags.HasFlag(LocomotionFlags.IsLooking)))
                return true;

            if (newState.LocomotionFlags.HasFlag(LocomotionFlags.IsLocomoting) == false
                && newState.LocomotionFlags.HasFlag(LocomotionFlags.IsLooking) == false)
                return false;

            if ((newState.PathNodes.Count > oldState.PathNodes.Count)
                || (newState.PathNodes.Count == 0 && oldState.PathNodes.Count > 0))
                return true;

            if (newState.PathNodes.Count > 0)
            {
                int nodesRemaining = newState.PathNodes.Count - newState.PathGoalNodeIndex;
                int newNodeIndex = newState.PathGoalNodeIndex;
                int oldNodeIndex = oldState.PathNodes.Count - nodesRemaining;
                if (skipGoalNode) --nodesRemaining;

                for (int i = 0; i < nodesRemaining; ++i)
                {
                    if (newState.PathNodes[newNodeIndex + i].VertexSide != oldState.PathNodes[oldNodeIndex + i].VertexSide
                    || newState.PathNodes[newNodeIndex + i].Radius != oldState.PathNodes[oldNodeIndex + i].Radius
                    || newState.PathNodes[newNodeIndex + i].Vertex != oldState.PathNodes[oldNodeIndex + i].Vertex)
                        return true;
                }
            }

            return false;
        }
    }
}
