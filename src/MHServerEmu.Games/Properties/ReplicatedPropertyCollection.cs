using Gazillion;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.Network;

namespace MHServerEmu.Games.Properties
{
    public class ReplicatedPropertyCollection : PropertyCollection, IArchiveMessageHandler
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private IArchiveMessageDispatcher _messageDispatcher = null;
        private AOINetworkPolicyValues _interestPolicies;
        private ulong _replicationId = IArchiveMessageDispatcher.InvalidReplicationId;

        public ulong ReplicationId { get => _replicationId; }
        public bool IsBound { get => _replicationId != IArchiveMessageDispatcher.InvalidReplicationId && _messageDispatcher != null; }

        public ReplicatedPropertyCollection() { }

        public bool Bind(IArchiveMessageDispatcher messageDispatcher, AOINetworkPolicyValues interestPolicies)
        {
            if (messageDispatcher == null) return Logger.WarnReturn(false, "Bind(): messageDispatcher == null");

            if (IsBound)
            {
                // If already bound to the dispatcher we need, all good
                if (_messageDispatcher == messageDispatcher)
                    return true;

                return Logger.WarnReturn(false, $"Bind(): Already bound with replicationId {_replicationId} to {_messageDispatcher}");
            }

            _messageDispatcher = messageDispatcher;
            _interestPolicies = interestPolicies;
            _replicationId = messageDispatcher.RegisterMessageHandler(this, ref _replicationId);    // pass repId field by ref so that we don't have to expose a setter

            return true;
        }

        public void Unbind()
        {
            _messageDispatcher?.UnregisterMessageHandler(this);
            _messageDispatcher = null;

            _interestPolicies = AOINetworkPolicyValues.AOIChannelNone;
            _replicationId = IArchiveMessageDispatcher.InvalidReplicationId;
        }

        public override void ResetForPool()
        {
            base.ResetForPool();

            Unbind();
        }

        public override void Dispose()
        {
            // Need to override Dispose so that replicated collections don't get pulled with regular ones
            ObjectPoolManager.Instance.Return(this);
        }

        public override bool SerializeWithDefault(Archive archive, PropertyCollection defaultCollection)
        {
            bool success = true;

            if (archive.IsReplication)
                success &= Serializer.Transfer(archive, ref _replicationId);

            success &= base.SerializeWithDefault(archive, defaultCollection);
            return success;
        }

        public void OnEntityChangePlayerAOI(Player player, InterestTrackOperation operation,
            AOINetworkPolicyValues newInterestPolicies, AOINetworkPolicyValues previousInterestPolicies, AOINetworkPolicyValues archiveInterestPolicies)
        {
            // V10_TODO: Send properties revealed by new AOI policies
        }

        public override bool RemoveProperty(PropertyId id)
        {
            bool removed = base.RemoveProperty(id);

            if (removed)
                MarkPropertyRemoved(id);

            return removed;
        }

        protected override bool SetPropertyValue(PropertyId id, PropertyValue value, SetPropertyFlags flags = SetPropertyFlags.None)
        {
            bool changed = base.SetPropertyValue(id, value, flags);

            if (changed)
                MarkPropertyChanged(id, value, flags);

            return changed;
        }

        private void MarkPropertyChanged(PropertyId id, PropertyValue value, SetPropertyFlags flags)
        {
            if (_messageDispatcher == null || _messageDispatcher.CanSendArchiveMessages == false) return;

            // Get replication policy for this property
            PropertyInfo propertyInfo = GameDatabase.PropertyInfoTable.LookupPropertyInfo(id.Enum);
            AOINetworkPolicyValues interestFilter = propertyInfo.Prototype.RepNetwork & _interestPolicies;
            if (interestFilter == AOINetworkPolicyValues.AOIChannelNone) return;

            // Check if any there are any interested clients
            List<PlayerConnection> interestedClientList = ListPool<PlayerConnection>.Instance.Get();
            if (_messageDispatcher.GetInterestedClients(interestedClientList, interestFilter))
            {
                // Send update to interested
                //Logger.Trace($"MarkPropertyChanged(): [{ReplicationId}] {id}: {value.Print(propertyInfo.DataType)}");

                using Archive archive = new(ArchiveSerializeType.Replication, (ulong)_interestPolicies);
                Serializer.Transfer(archive, ref id);

                int rawFlags = 0;   // are these SetPropertyFlags?
                Serializer.Transfer(archive, ref rawFlags);

                byte dataType = (byte)propertyInfo.DataType;
                Serializer.Transfer(archive, ref dataType);

                switch (propertyInfo.DataType)
                {
                    case PropertyDataType.Boolean:
                        bool boolValue = value;
                        Serializer.Transfer(archive, ref boolValue);
                        break;

                    case PropertyDataType.Real:
                        Serializer.Transfer(archive, ref value.RawFloat);
                        break;

                    case PropertyDataType.EntityId:
                        ulong entityId = value;
                        Serializer.Transfer(archive, ref entityId);
                        break;

                    default:
                        Serializer.Transfer(archive, ref value.RawLong);
                        break;
                }

                NetMessageReplicationArchive setPropertyMessage = NetMessageReplicationArchive.CreateBuilder()
                    .SetReplicationId(ReplicationId)
                    .SetArchiveData(archive.ToByteString())
                    .Build();

                _messageDispatcher.Game.NetworkManager.SendMessageToMultiple(interestedClientList, setPropertyMessage);
            }

            ListPool<PlayerConnection>.Instance.Return(interestedClientList);
        }

        private void MarkPropertyRemoved(PropertyId id)
        {
            if (_messageDispatcher == null || _messageDispatcher.CanSendArchiveMessages == false) return;

            // Get replication policy for this property
            PropertyInfo propertyInfo = GameDatabase.PropertyInfoTable.LookupPropertyInfo(id.Enum);
            AOINetworkPolicyValues interestFilter = propertyInfo.Prototype.RepNetwork;
            if (interestFilter == AOINetworkPolicyValues.AOIChannelNone) return;

            // Check if any there are any interested clients
            List<PlayerConnection> interestedClientList = ListPool<PlayerConnection>.Instance.Get();
            if (_messageDispatcher.GetInterestedClients(interestedClientList, interestFilter))
            {
                // Send update to interested
                //Logger.Trace($"MarkPropertyRemoved(): [{ReplicationId}] {id}");
                using Archive archive = new(ArchiveSerializeType.Replication, (ulong)_interestPolicies);
                Serializer.Transfer(archive, ref id);

                NetMessageReplicationArchive removePropertyMessage = NetMessageReplicationArchive.CreateBuilder()
                    .SetReplicationId(ReplicationId)
                    .SetArchiveDataType(1)
                    .SetArchiveData(archive.ToByteString())
                    .Build();

                _messageDispatcher.Game.NetworkManager.SendMessageToMultiple(interestedClientList, removePropertyMessage);
            }

            ListPool<PlayerConnection>.Instance.Return(interestedClientList);
        }
    }
}
