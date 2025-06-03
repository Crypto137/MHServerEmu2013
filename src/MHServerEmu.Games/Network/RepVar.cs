using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.GameData;

namespace MHServerEmu.Games.Network
{
    public abstract class RepVar<T> : IArchiveMessageHandler, ISerialize
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private IArchiveMessageDispatcher _messageDispatcher = null;
        private AOINetworkPolicyValues _interestPolicies = AOINetworkPolicyValues.AOIChannelNone;
        private ulong _replicationId;

        protected T _value;

        public ulong ReplicationId { get => _replicationId; }
        public bool IsBound { get => _replicationId != IArchiveMessageDispatcher.InvalidReplicationId && _messageDispatcher != null; }

        public RepVar()
        {
        }

        public override string ToString()
        {
            return $"[{_replicationId}] {_value}";
        }

        public T Get()
        {
            return _value;
        }

        public void Set(T value)
        {
            if (IsEquivalent(value))
                return;

            _value = value;
            // TODO: Send replication message
        }

        public virtual bool Serialize(Archive archive)
        {
            bool success = true;
            success &= Serializer.Transfer(archive, ref _replicationId);
            success &= SerializeValue(archive);
            return success;
        }

        public bool Bind(IArchiveMessageDispatcher messageDispatcher, AOINetworkPolicyValues interestPolicies)
        {
            if (messageDispatcher == null) return Logger.WarnReturn(false, "Bind(): messageDispatcher == null");

            if (IsBound)
                return Logger.WarnReturn(false, $"Bind(): Already bound with replicationId {_replicationId} to {_messageDispatcher}");

            _messageDispatcher = messageDispatcher;
            _interestPolicies = interestPolicies;
            _replicationId = messageDispatcher.RegisterMessageHandler(this, ref _replicationId);    // pass repId field by ref so that we don't have to expose a setter

            return true;
        }

        public void Unbind()
        {
            if (IsBound == false)
                return;

            _messageDispatcher.UnregisterMessageHandler(this);
            _replicationId = IArchiveMessageDispatcher.InvalidReplicationId;
        }

        protected virtual bool SerializeValue(Archive archive)
        {
            throw new NotImplementedException();
        }

        protected virtual bool IsEquivalent(T value)
        {
            // Override this in value types to avoid boxing
            return _value.Equals(value);
        }
    }

    #region Implementations

    // This copypasta fest is here for two reasons:
    // 1. Resolving types for Serializer.Transfer() overloads.
    // 2. Avoiding boxing when comparing value types.

    public class RepVar_byte : RepVar<byte>
    {
        protected override bool SerializeValue(Archive archive)
        {
            return Serializer.Transfer(archive, ref _value);
        }

        protected override bool IsEquivalent(byte value)
        {
            return _value == value;
        }
    }

    public class RepVar_uint : RepVar<uint>
    {
        protected override bool SerializeValue(Archive archive)
        {
            return Serializer.Transfer(archive, ref _value);
        }

        protected override bool IsEquivalent(uint value)
        {
            return _value == value;
        }
    }

    public class RepVar_ulong : RepVar<ulong>
    {
        protected override bool SerializeValue(Archive archive)
        {
            return Serializer.Transfer(archive, ref _value);
        }

        protected override bool IsEquivalent(ulong value)
        {
            return _value == value;
        }
    }

    public class RepVar_string : RepVar<string>
    {
        // string is a special case: because it's a reference type, we don't need to
        // override IsEquivalent(), but we do however need to set an initial non-null value.

        public RepVar_string()
        {
            _value = string.Empty;
        }

        protected override bool SerializeValue(Archive archive)
        {
            return Serializer.Transfer(archive, ref _value);
        }
    }

    public class RepVar_Vector3 : RepVar<Vector3>
    {
        protected override bool SerializeValue(Archive archive)
        {
            return Serializer.Transfer(archive, ref _value);
        }

        protected override bool IsEquivalent(Vector3 value)
        {
            return _value == value;
        }
    }

    public class RepVar_PrototypeDataRef : RepVar<PrototypeId>
    {
        protected override bool SerializeValue(Archive archive)
        {
            return Serializer.Transfer(archive, ref _value);
        }

        protected override bool IsEquivalent(PrototypeId value)
        {
            return _value == value;
        }
    }

    #endregion
}
