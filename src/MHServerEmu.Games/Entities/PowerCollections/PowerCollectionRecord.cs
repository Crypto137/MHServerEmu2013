using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Powers;

namespace MHServerEmu.Games.Entities.PowerCollections
{
    public class PowerCollectionRecord
    {
        public PrototypeId PowerPrototypeRef { get => PowerPrototype != null ? PowerPrototype.DataRef : PrototypeId.Invalid; }
        public PowerPrototype PowerPrototype { get; private set; }
        public ulong TargetId { get; private set; }
        public PowerIndexProperties IndexProps { get; private set; }
        public uint PowerRefCount { get; set; }

        // The rest of data is not serialized
        public Power Power { get; private set; }

        public PowerCollectionRecord() { }

        public override string ToString()
        {
            return $"[x{PowerRefCount}] {PowerPrototype} (targetId={TargetId}, {IndexProps})";
        }

        public void Initialize(Power power, PrototypeId powerPrototypeRef, ulong targetId, PowerIndexProperties indexProps, uint powerRefCount)
        {
            PowerPrototype = powerPrototypeRef.As<PowerPrototype>();
            TargetId = targetId;
            IndexProps = indexProps;
            PowerRefCount = powerRefCount;

            Power = power;
        }

        public void ClearPower()
        {
            Power = null;
        }

        public bool ShouldSerializeRecordForPacking(Archive archive)
        {
            if (!Verify.IsTrue(archive.IsPacking)) return false;

            if (archive.IsReplication == false)
                return false;

            if (!Verify.IsNotNull(PowerPrototype)) return false;

            if (PowerPrototype.PowerCategory == PowerCategoryType.ComboEffect)
                return false;

            return true;
        }

        public bool SerializeTo(Archive archive, PowerCollectionRecord previousRecord)
        {
            if (!Verify.IsTrue(archive.IsPacking && archive.IsReplication)) return false;

            bool success = true;

            PrototypeId powerProtoRef = PowerPrototypeRef;
            success &= Serializer.Transfer(archive, ref powerProtoRef);

            int powerRank = IndexProps.PowerRank;
            success &= Serializer.Transfer(archive, ref powerRank);

            int characterLevel = IndexProps.CharacterLevel;
            success &= Serializer.Transfer(archive, ref characterLevel);

            int itemLevel = IndexProps.ItemLevel;
            success &= Serializer.Transfer(archive, ref itemLevel);

            if (archive.IsReplication)
            {
                uint powerRefCount = PowerRefCount;
                success &= Serializer.Transfer(archive, ref powerRefCount);
            }

            return success;
        }

        public bool SerializeFrom(Archive archive)
        {
            if (!Verify.IsTrue(archive.IsUnpacking)) return false;

            bool success = true;

            PrototypeId powerProtoRef = PrototypeId.Invalid;
            success &= Serializer.Transfer(archive, ref powerProtoRef);

            int powerRank = 0;
            success &= Serializer.Transfer(archive, ref powerRank);

            int characterLevel = 0;
            success &= Serializer.Transfer(archive, ref characterLevel);

            int itemLevel = 0;
            success &= Serializer.Transfer(archive, ref itemLevel);

            IndexProps = new(powerRank, characterLevel, itemLevel);

            if (archive.IsReplication)
            {
                uint powerRefCount = 0;
                success &= Serializer.Transfer(archive, ref powerRefCount);
                PowerRefCount = powerRefCount;
            }

            return success;
        }
    }
}
