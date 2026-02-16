using Gazillion;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.Entities.Locomotion;

namespace MHServerEmu.Games.Network
{
    /// <summary>
    /// A helper class for building archive protobufs.
    /// </summary>
    public static class ArchiveMessageBuilder
    {
        // V10_NOTE: 1.10 uses archive version 36 (no gameplay version yet?)

        /// <summary>
        /// Builds <see cref="NetMessageEntityCreate"/> for the provided <see cref="Entity"/>.
        /// </summary>
        public static NetMessageEntityCreate BuildEntityCreateMessage(Entity entity, AOINetworkPolicyValues interestPolicies, bool includeInvLoc, EntitySettings settings = null)
        {
            // V10_TODO: initConditionComponent, startFullInWorldHierarchyUpdate
            
            var builder = NetMessageEntityCreate.CreateBuilder()
                .SetIdEntity(entity.Id)
                .SetPrototypeId((ulong)entity.PrototypeDataRef)
                .SetInterestPolicies((uint)interestPolicies)
                .SetDbId(entity is Player ? entity.DatabaseUniqueId : 0);

            using (Archive archive = new(ArchiveSerializeType.Replication, (ulong)interestPolicies))
            {
                entity.Serialize(archive);
                builder.SetArchiveData(archive.ToByteString());
            }

            if (entity is WorldEntity worldEntity)
            {
                if (interestPolicies.HasFlag(AOINetworkPolicyValues.AOIChannelProximity) && worldEntity.IsInWorld)
                {
                    RegionLocation regionLocation = worldEntity.RegionLocation;

                    builder.SetPosition(regionLocation.Position.ToNetStructPoint3());
                    builder.SetOrientation(regionLocation.Orientation.ToNetStructPoint3());
#if BUILD_1_10_0_69
                    builder.SetCellId(regionLocation.CellId);
                    builder.SetAreaId(regionLocation.AreaId);
#endif

                    ref LocomotionState locomotionState = ref LocomotionState.Null;
                    if (worldEntity.Locomotor != null)
                        locomotionState = ref worldEntity.Locomotor.LocomotionState;

                    LocomotionMessageFlags locoFieldFlags = LocomotionState.GetFieldFlags(ref locomotionState, ref LocomotionState.Null, true);
                    if (locoFieldFlags.HasFlag(LocomotionMessageFlags.NoLocomotionState) == false)
                    {
                        NetStructLocomotionState.Builder locomotionStateBuilder = NetStructLocomotionState.CreateBuilder();
                        LocomotionState.SerializeTo(locomotionStateBuilder, ref locomotionState, locoFieldFlags);
                        builder.SetLocomotionState(locomotionStateBuilder.Build());
                    }
                }

                if (worldEntity.ShouldSnapToFloorOnSpawn != worldEntity.WorldEntityPrototype.SnapToFloorOnSpawn)
                    builder.SetOverrideSnapToFloorOnSpawn(worldEntity.ShouldSnapToFloorOnSpawn);
            }

            // Apply settings
            if (settings != null)
            {
                if (settings.OptionFlags.HasFlag(EntitySettingsOptionFlags.IsNewOnServer))
                    builder.SetIsNewOnServer(true);

                if (settings.SourceEntityId != Entity.InvalidId)
                    builder.SetSourceEntityId(settings.SourceEntityId);

                if (settings.SourcePosition != Vector3.Zero)
                    builder.SetSourcePosition(settings.SourcePosition.ToNetStructPoint3());

                // No BoundsScaleOverride in 1.10

                if (settings.OptionFlags.HasFlag(EntitySettingsOptionFlags.IsClientEntityHidden))
                    builder.SetIsClientEntityHidden(true);

                // No IgnoreNavi in 1.10
            }

            if (includeInvLoc)
            {
                builder.SetInvLocContainerEntityId(entity.InventoryLocation.ContainerId);
                builder.SetInvLocInventoryPrototypeId((ulong)entity.InventoryLocation.InventoryRef);
                builder.SetInvLocSlot(entity.InventoryLocation.Slot);

                // No InvLocPrev in 1.10
            }

            return builder.Build();
        }

        /// <summary>
        /// Builds <see cref="NetMessageLocomotionStateUpdate"/> for the provided <see cref="WorldEntity"/>.
        /// </summary>
        public static NetMessageLocomotionStateUpdate BuildLocomotionStateUpdateMessage(WorldEntity worldEntity, ref LocomotionState oldLocomotionState, ref LocomotionState newLocomotionState,
            bool withPathNodes)
        {
            NetStructLocomotionState.Builder locomotionStateBuilder = NetStructLocomotionState.CreateBuilder();

            // We reuse LocomotionMessageFlags from archive implementation used in later version for convenience
            LocomotionMessageFlags fieldFlags = LocomotionState.GetFieldFlags(ref newLocomotionState, ref oldLocomotionState, withPathNodes);
            LocomotionState.SerializeTo(locomotionStateBuilder, ref newLocomotionState, fieldFlags);

            // optional uint64 entityPrototypeId - is this needed for internal builds?
            return NetMessageLocomotionStateUpdate.CreateBuilder()
                .SetIdEntity(worldEntity.Id)
                .SetLocomotionstate(locomotionStateBuilder.Build())
                .SetPosition(worldEntity.RegionLocation.Position.ToNetStructPoint3())
                .SetOrientation(worldEntity.RegionLocation.Orientation.ToNetStructPoint3())
                .Build();
        }

        /// <summary>
        /// Builds <see cref="NetMessageEntityEnterGameWorld"/> for the provided <see cref="WorldEntity"/>.
        /// </summary>
        public static NetMessageEntityEnterGameWorld BuildEntityEnterGameWorldMessage(WorldEntity worldEntity, EntitySettings settings = null)
        {
            var builder = NetMessageEntityEnterGameWorld.CreateBuilder()
                .SetEntityId(worldEntity.Id)
                .SetPosition(worldEntity.RegionLocation.Position.ToNetStructPoint3())
                .SetOrientation(worldEntity.RegionLocation.Orientation.ToNetStructPoint3());

            // TODO: entity prototype ref

            ref LocomotionState locomotionState = ref LocomotionState.Null;
            if (worldEntity.Locomotor != null)
                locomotionState = ref worldEntity.Locomotor.LocomotionState;

            LocomotionMessageFlags locoFieldFlags = LocomotionState.GetFieldFlags(ref locomotionState, ref LocomotionState.Null, true);
            if (locoFieldFlags.HasFlag(LocomotionMessageFlags.NoLocomotionState) == false)
            {
                NetStructLocomotionState.Builder locomotionStateBuilder = NetStructLocomotionState.CreateBuilder();
                LocomotionState.SerializeTo(locomotionStateBuilder, ref locomotionState, locoFieldFlags);
                builder.SetLocomotionState(locomotionStateBuilder.Build());
            }

            // Only the IsClientEntityHidden flag is here in 1.10
            if (settings != null && settings.OptionFlags.HasFlag(EntitySettingsOptionFlags.IsClientEntityHidden))
                builder.SetIsClientEntityHidden(true);

            return builder.Build();
        }
    }
}
