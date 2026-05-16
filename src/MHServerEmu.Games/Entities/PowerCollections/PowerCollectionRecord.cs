using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Powers;

namespace MHServerEmu.Games.Entities.PowerCollections
{
    public class PowerCollectionRecord
    {
        private PowerPrototype _powerProto = null;
        private ulong _targetId = 0;
        private PowerIndexProperties _indexProps = new();
        private uint _powerRefCount;

        public PrototypeId PowerPrototypeRef { get => _powerProto != null ? _powerProto.DataRef : PrototypeId.Invalid; }
        public PowerPrototype PowerPrototype { get => _powerProto; }
        public ulong TargetId { get => _targetId; }
        public PowerIndexProperties IndexProps { get => _indexProps; }
        public uint PowerRefCount { get => _powerRefCount; set => _powerRefCount = value; }

        // The rest of data is not serialized
        public Power Power { get; private set; }

        public PowerCollectionRecord() { }

        public override string ToString()
        {
            return $"[x{_powerRefCount}] {_powerProto} (targetId={_targetId}, {_indexProps})";
        }

        public void Initialize(Power power, PrototypeId powerPrototypeRef, ulong targetId, PowerIndexProperties indexProps, uint powerRefCount)
        {
            _powerProto = powerPrototypeRef.As<PowerPrototype>();
            _targetId = targetId;
            _indexProps = indexProps;
            _powerRefCount = powerRefCount;

            Power = power;
        }

        public void ClearPower()
        {
            Power = null;
        }
    }
}
