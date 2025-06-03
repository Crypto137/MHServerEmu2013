using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.GameData;

namespace MHServerEmu.Games.Entities.Avatars
{
    public class HotkeyData : ISerialize
    {
        public PrototypeId AbilityProtoRef { get; set; } = PrototypeId.Invalid;
        public AbilitySlot AbilitySlot { get; set; } = AbilitySlot.Invalid;

        public HotkeyData()
        {
        }

        public HotkeyData(PrototypeId abilityProtoRef, AbilitySlot abilitySlot)
        {
            AbilityProtoRef = abilityProtoRef;
            AbilitySlot = abilitySlot;
        }

        public bool Serialize(Archive archive)
        {
            bool success = true;

            PrototypeId abilityProtoRef = AbilityProtoRef;
            success &= Serializer.Transfer(archive, ref abilityProtoRef);
            AbilityProtoRef = abilityProtoRef;

            int abilitySlot = (int)AbilitySlot;
            success &= Serializer.Transfer(archive, ref abilitySlot);
            AbilitySlot = (AbilitySlot)abilitySlot;

            return success;
        }
    }
}
