using Gazillion;
using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.Entities.Avatars;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Powers;
using MHServerEmu.Games.Properties;

namespace MHServerEmu.Games.Entities.PowerCollections
{
    public class PowerCollection : ISerialize
    {
        private const int MaxNumRecordsToSerialize = 256;

        private readonly SortedDictionary<(PrototypeId, ulong), PowerCollectionRecord> _powers = new();
        private readonly Stack<Power> _condemnedPowers = new();
        private readonly WorldEntity _owner;

        // V10_NOTE: in 1.10 power collections can be "duplicating", allowing multiple instances of the same power for different target entity ids.
        // This is allowed only for hotspots with DuplicatePower enabled in their prototypes.
        // This doesn't appear to be actually used in 1.10 outside of a single legacy power (AnalyticalMindArea), but we're keeping it for accuracy.
        public bool DuplicatesPowers { get; }

        public PowerCollection(WorldEntity owner, bool duplicatePowers)
        {
            _owner = owner;
            DuplicatesPowers = duplicatePowers;

            Verify.IsTrue(DuplicatesPowers == false || owner is Hotspot, $"Constructing a Duplicating PowerCollection for an entity that is not a Hotspot, which is not advisable!\nEntity: [{_owner}]");
        }

        public bool Serialize(Archive archive)
        {
            bool success = true;

            uint numRecordsToSerialize = 0;

            // In very old versions of the game (before archive version 15) power collections were serialized to persistent archives.
            // We don't need a code path for persistent archives here like the client does because we don't have this kind of legacy data.
            if (archive.IsPacking)
            {
                if (archive.IsReplication && archive.HasReplicationPolicy(AOINetworkPolicyValues.AOIChannelProximity))
                {
                    foreach (PowerCollectionRecord record in _powers.Values)
                    {
                        if (record.ShouldSerializeRecordForPacking(archive) == false)
                            continue;

                        if (!Verify.IsTrue(numRecordsToSerialize < MaxNumRecordsToSerialize))
                            break;

                        numRecordsToSerialize++;
                    }

                    success &= Serializer.Transfer(archive, ref numRecordsToSerialize);
                }
            }
            else
            {
                if (archive.IsReplication && archive.HasReplicationPolicy(AOINetworkPolicyValues.AOIChannelProximity))
                    success &= Serializer.Transfer(archive, ref numRecordsToSerialize);
            }

            if (archive.IsPacking)
            {
                if (!Verify.IsTrue(archive.IsReplication)) return false;

                foreach (PowerCollectionRecord record in _powers.Values)
                {
                    if (record.ShouldSerializeRecordForPacking(archive))
                    {
                        success &= record.SerializeTo(archive);
                        numRecordsToSerialize--;
                    }
                }

                if (!Verify.IsTrue(numRecordsToSerialize == 0)) return false;
            }
            else
            {
                if (_powers.Count > 0)
                {
                    Verify.IsTrue(false, "When preparing to unpack a serialized PowerCollection, there was already data in the receiving _powers structure");
                    _powers.Clear();
                }

                for (uint i = 0; i < numRecordsToSerialize; i++)
                {
                    PowerCollectionRecord record = new();
                    success &= record.SerializeFrom(archive);
                    // V10_NOTE: doesn't seem like targetEntityId is serialized
                    _powers.Add((record.PowerPrototypeRef, 0), record);
                }
            }

            return success;
        }

        public SortedDictionary<(PrototypeId, ulong), PowerCollectionRecord>.Enumerator GetEnumerator()
        {
            return _powers.GetEnumerator();
        }

        public Power GetPower(PrototypeId powerProtoRef, ulong targetEntityId = 0)
        {
            return GetPowerRecordByRef(powerProtoRef, targetEntityId)?.Power;
        }

        public bool ContainsPower(PrototypeId powerProtoRef, ulong targetEntityId)
        {
            return GetPowerRecordByRef(powerProtoRef, targetEntityId) != null;
        }

        public Power AssignPower(PrototypeId powerProtoRef, in PowerIndexProperties indexProps, PrototypeId triggeringPowerRef = PrototypeId.Invalid, bool sendPowerAssignmentToClients = true)
        {
            PowerPrototype powerProto = powerProtoRef.As<PowerPrototype>();
            if (!Verify.IsNotNull(powerProto)) return null;

            if (Power.IsComboEffect(powerProto) == false)
            {
                if (!Verify.IsTrue(_owner != null && _owner.IsInWorld, $"PowerCollection now only supports Assign() of powers while the owner is in world!\nEntity: [{_owner}]\nPower: [{powerProtoRef.GetName()}]"))
                    return null;
            }

            if (!Verify.IsTrue(DuplicatesPowers == false, $"This version of AssignPower should only be called for PowerCollections that do NOT duplicate powers (which this entity's collection does!)\nEntity: [{_owner}]"))
                return null;

            return AssignPowerInternal(powerProtoRef, 0, indexProps, triggeringPowerRef, sendPowerAssignmentToClients);
        }

        public Power AssignPowerDuplicating(PrototypeId powerProtoRef, ulong targetEntityId, in PowerIndexProperties indexProps, PrototypeId triggeringPowerRef = PrototypeId.Invalid)
        {
            PowerPrototype powerProto = powerProtoRef.As<PowerPrototype>();
            if (!Verify.IsNotNull(powerProto)) return null;

            if (Power.IsComboEffect(powerProto) == false)
            {
                if (!Verify.IsTrue(_owner != null && _owner.IsInWorld, $"PowerCollection now only supports Assign() of powers while the owner is in world!\nEntity: [{_owner}]\nPower: [{powerProtoRef.GetName()}]"))
                    return null;
            }

            if (!Verify.IsTrue(DuplicatesPowers, $"This version of AssignPower should only be called for PowerCollections that duplicate powers (which this entity's collection does not!)\nEntity: [{_owner}]"))
                return null;

            return AssignPowerInternal(powerProtoRef, targetEntityId, indexProps, triggeringPowerRef, true);
        }

        public bool UnassignPower(PrototypeId powerProtoRef, bool sendPowerUnassignToClients = true)
        {
            if (!Verify.IsTrue(_owner != null && _owner.IsInWorld, $"PowerCollection now only supports Unassign() of powers while the owner is in world!\nEntity: [{_owner}]\nPower: [{powerProtoRef.GetName()}]"))
                return false;

            if (!Verify.IsTrue(DuplicatesPowers == false, $"This version of UnassignPower should only be called for PowerCollections that do NOT duplicate powers (which this entity's collection does!)\nEntity: [{_owner}]"))
                return false;

            return UnassignPowerInternal(powerProtoRef, 0, sendPowerUnassignToClients);
        }

        public bool UnassignPowerDuplicating(PrototypeId powerProtoRef, ulong targetEntityId)
        {
            if (!Verify.IsTrue(_owner != null && _owner.IsInWorld, $"PowerCollection now only supports Unassign() of powers while the owner is in world!\nEntity: [{_owner}]\nPower: [{powerProtoRef.GetName()}]"))
                return false;

            if (!Verify.IsTrue(DuplicatesPowers, $"This version of UnassignPower should only be called for PowerCollections that duplicate powers (which this entity's collection does not!)\nEntity: [{_owner}]"))
                return false;

            return UnassignPowerInternal(powerProtoRef, targetEntityId, true);
        }

        public void DeleteCondemnedPowers()
        {
            while (_condemnedPowers.Count > 0)
            {
                Power condemnedPower = _condemnedPowers.Pop();
                condemnedPower.OnDeallocate();
            }
        }

        public void OnOwnerEnteredWorld()
        {
            // Notify powers of the owner entering world
            foreach (PowerCollectionRecord record in _powers.Values)
                record.Power?.OnOwnerEnteredWorld();
        }

        public void OnOwnerExitedWorld()
        {
            // Notify powers of the owner exiting world
            foreach (PowerCollectionRecord record in _powers.Values)
                record.Power?.OnOwnerExitedWorld();

            // Copy to a temporary list to be able to remove entries while iterating
            using var recordsHandle = ListPool<KeyValuePair<(PrototypeId, ulong), PowerCollectionRecord>>.Instance.Get(out var records);

            // This needs to be done in a loop to remove all copies of powers with RefCount higher than 0.
            while (_powers.Count > 0)
            {
                records.Set(_powers);

                bool unassignedAny = false;
                foreach (var kvp in records)
                {
                    Power power = kvp.Value.Power;

                    // This is needed for our iteration workaround because triggered powers may remain in the temp list after being unassigned from the main collection.
                    if (_powers.ContainsKey(kvp.Key) == false)
                        continue;

                    // Simply remove records that have no valid powers
                    if (!Verify.IsNotNull(power))
                    {
                        _powers.Remove(kvp.Key);
                        continue;
                    }

                    // Combo effects are unassigned separately
                    if (power.IsComboEffect())
                        continue;

                    // Unassign power
                    UnassignPower(kvp.Value.PowerPrototypeRef, false);
                    unassignedAny = true;
                }

                // Combo powers that are used to enter/exit a transform mode are not unassigned along with their triggering power.
                // Because of this, there may still be powers left in the collection when a transformed owner avatar exits world.
                // This appears to be not a bug, but rather a questionable design decision made by Gazillion.
                if (unassignedAny == false && _powers.Count > 0)
                {
                    Verify.IsTrue(_owner is Avatar avatar && avatar.CurrentTransformMode != PrototypeId.Invalid);
                    break;
                }
            }
        }

        private PowerCollectionRecord GetPowerRecordByRef(PrototypeId powerProtoRef, ulong targetEntityId)
        {
            if (!Verify.IsTrue(DuplicatesPowers == false || targetEntityId != Entity.InvalidId, $"Invalid target ID specified when retrieving record from a DuplicatePowers power collection\n requested power hash: {powerProtoRef.GetName()}\n collection owner: {_owner}"))
                return null;

            if (_powers.TryGetValue((powerProtoRef, targetEntityId), out PowerCollectionRecord record) == false)
                return null;

            return record;
        }

        private Power AssignPowerInternal(PrototypeId powerProtoRef, ulong targetEntityId, in PowerIndexProperties indexProps, PrototypeId triggeringPowerRef, bool sendPowerAssignmentToClients)
        {
            // Do pre-assignment validation, this check combines and inlines PowerCollection::preAssignPowerInternal() and PowerCollection::validatePowerData()
            if (!Verify.IsTrue(GameDatabase.DataDirectory.PrototypeIsApproved(powerProtoRef), $"Power is not yet ready for game or review.\n[{powerProtoRef.GetName()}]"))
                return null;

            // Send power assignment message to interested clients
            if (sendPowerAssignmentToClients && _owner != null && _owner.IsInGame)
            {
                var assignPowerMessage = NetMessagePowerCollectionAssignPower.CreateBuilder()
                    .SetEntityId(_owner.Id)
                    .SetPowerProtoId((ulong)powerProtoRef)
                    .SetTargetentityid(targetEntityId)
                    .SetPowerRank(indexProps.PowerRank)
                    .SetCharacterLevel(indexProps.CharacterLevel)
                    .SetItemLevel(indexProps.ItemLevel)
                    .SetPowerCollectionIsduplicating(DuplicatesPowers)
                    .Build();

                _owner.Game.NetworkManager.SendMessageToInterested(assignPowerMessage, _owner, AOINetworkPolicyValues.AOIChannelProximity);
            }

            // See if the power we are trying to assign is already in this collection
            PowerCollectionRecord powerRecord = GetPowerRecordByRef(powerProtoRef, targetEntityId);
            if (powerRecord == null)
            {
                // V10_NOTE: Doesn't seem like there are any power source flags in 1.10.
                powerRecord = CreatePowerRecord(powerProtoRef, targetEntityId, indexProps, triggeringPowerRef);
                if (!Verify.IsNotNull(powerRecord)) return null;
            }
            else
            {
                if (!Verify.IsNotNull(powerRecord.Power)) return null;
                if (!Verify.IsTrue(powerRecord.PowerPrototypeRef == powerProtoRef)) return null;

                // Only proc and combo effects can be assigned multiple times
                bool isProcEffect = powerRecord.Power.IsProcEffect();
                bool isComboEffect = powerRecord.Power.IsComboEffect() && powerRecord.Power.GetActivationType() != PowerActivationType.Passive;
                if (!Verify.IsTrue(isProcEffect || isComboEffect, $"The following power being assigned multiple times to a PowerCollection is not a Combo or Proc effect power, which is not allowed!\nOwner: [{_owner}]\nPower: [{powerRecord.Power}]"))
                    return null;

                // Increment power ref count
                powerRecord.PowerRefCount++;
            }

            return powerRecord.Power;
        }

        private PowerCollectionRecord CreatePowerRecord(PrototypeId powerProtoRef, ulong targetEntityId, in PowerIndexProperties indexProps, PrototypeId triggeringPowerRef)
        {
            if (!Verify.IsTrue(DuplicatesPowers == false || targetEntityId != Entity.InvalidId)) return null;

            Power power = CreatePower(powerProtoRef, indexProps, triggeringPowerRef);
            if (!Verify.IsNotNull(power)) return null;

            // Here we have a custom Initialize() method not present in the client to clean up record initialization
            PowerCollectionRecord record = new();
            record.Initialize(power, powerProtoRef, targetEntityId, indexProps, 1);
            _powers.Add((powerProtoRef, targetEntityId), record);   // PowerCollection::addPowerRecord()

            FinishAssignPower(power);
            return record;
        }

        private Power CreatePower(PrototypeId powerProtoRef, in PowerIndexProperties indexProps, PrototypeId triggeringPowerRef)
        {
            if (!Verify.IsTrue(powerProtoRef != PrototypeId.Invalid)) return null;
            if (!Verify.IsNotNull(_owner)) return null;

            Game game = _owner.Game;
            if (!Verify.IsNotNull(game)) return null;

            Power retPower = game.AllocatePower(powerProtoRef);
            if (!Verify.IsNotNull(retPower)) return null;

            // Assemble property values passed as arguments into a collection
            using PropertyCollection initializeProperties = ObjectPoolManager.Instance.Get<PropertyCollection>();

            initializeProperties[PropertyEnum.PowerRank] = indexProps.PowerRank;
            initializeProperties[PropertyEnum.CharacterLevel] = indexProps.CharacterLevel;
            initializeProperties[PropertyEnum.ItemLevel] = indexProps.ItemLevel;

            if (triggeringPowerRef != PrototypeId.Invalid)
                initializeProperties[PropertyEnum.TriggeringPowerRef, powerProtoRef] = triggeringPowerRef;

            retPower.Initialize(_owner, initializeProperties);

            return retPower;
        }

        private void FinishAssignPower(Power power)
        {
            // V10_TODO
        }

        private bool UnassignPowerInternal(PrototypeId powerProtoRef, ulong targetEntityId, bool sendPowerUnassignToClients)
        {
            if (!Verify.IsNotNull(_owner)) return false;

            Game game = _owner.Game;
            if (!Verify.IsNotNull(game)) return false;

            // Find and validate the record for our powerProtoRef
            PowerCollectionRecord powerRecord = GetPowerRecordByRef(powerProtoRef, targetEntityId);
            if (!Verify.IsNotNull(powerRecord, $"When unassigning, failed to find power record for {powerProtoRef.GetName()}\n  Owner:[{_owner}]\n  NumRecordsInCollection:{_powers.Count} NumCondemnedPowers:{_condemnedPowers.Count}"))
                return false;

            if (!Verify.IsNotNull(powerRecord.Power, $"When unassigning, the power record was found but had no power instance! Power: [{powerProtoRef.GetName()}], RefCount: [{powerRecord.PowerRefCount}], Owner: [{_owner}]"))
                return false;

            if (!Verify.IsTrue(powerRecord.PowerRefCount > 0, $"When unassigned, the power record had an invalid refcount! Power: [{powerProtoRef.GetName()}], RefCount: [{powerRecord.PowerRefCount}], Owner: [{_owner}]"))
                return false;

            // Start by subtracting from the PowerRefCount
            powerRecord.PowerRefCount--;

            // Remove the record when our PowerRefCount reaches 0
            if (powerRecord.PowerRefCount == 0)
            {
                FinishUnassignPower(powerRecord.Power);

                game.EntityManager.RegisterEntityForCondemnedPowerDeletion(_owner.Id);
                _condemnedPowers.Push(powerRecord.Power);
                powerRecord.ClearPower();

                Verify.IsTrue(DestroyPowerRecord(powerRecord.PowerPrototypeRef, powerRecord.TargetId));
            }

            // Send power unassignment message to interested clients
            if (sendPowerUnassignToClients && _owner.IsInGame && _owner.IsInWorld)
            {
                var unassignPowerMessage = NetMessagePowerCollectionUnassignPower.CreateBuilder()
                    .SetEntityId(_owner.Id)
                    .SetPowerProtoId((ulong)powerProtoRef)
                    .Build();

                game.NetworkManager.SendMessageToInterested(unassignPowerMessage, _owner, AOINetworkPolicyValues.AOIChannelProximity);
            }

            return true;
        }

        private bool DestroyPowerRecord(PrototypeId powerProtoRef, ulong targetEntityId)
        {
            if (!Verify.IsTrue(DuplicatesPowers == false || targetEntityId != Entity.InvalidId)) return false;

            var key = (powerProtoRef, targetEntityId);
            if (_powers.TryGetValue(key, out PowerCollectionRecord powerRecord) == false)
                return false;

            Verify.IsTrue(powerRecord.Power == null && powerRecord.PowerRefCount == 0, $"Power Record not empty during Destroy Power Record - Power Ref [{powerProtoRef.GetName()}]");

            _powers.Remove(key);
            return true;
        }

        private void FinishUnassignPower(Power power)
        {
            // V10_TODO
        }
    }
}
