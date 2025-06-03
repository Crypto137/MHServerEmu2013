using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;

namespace MHServerEmu.Games.Entities.Avatars
{
    public enum AbilitySlot
    {
        Invalid = -1,
        PrimaryAction = 0,          // Left Click
        SecondaryAction = 1,        // Right Click
        ActionKey0 = 2,             // A
        ActionKey1 = 3,             // S
        ActionKey2 = 4,             // D
        ActionKey3 = 5,             // F
        ActionKey4 = 6,             // G
        ActionKey5 = 7,             // H
        NumActions = 8,
    }

    public class AbilityKeyMapping : ISerialize
    {
        private const int NumActionKeySlots = 6;   // non-mouse slots
        private const int NumHotkeys = 6;

        private static readonly Logger Logger = LogManager.CreateLogger();

        private PrototypeId _associatedTransformMode;

        // Assignable slots
        private PrototypeId _primaryAction;
        private PrototypeId _secondaryAction;
        private PrototypeId[] _actionKeys = new PrototypeId[NumActionKeySlots];

        private HotkeyData[] _hotkeys = new HotkeyData[NumHotkeys];

        public AbilityKeyMapping()
        {
            for (int i = 0; i < NumHotkeys; i++)
                _hotkeys[i] = new();
        }

        public bool Serialize(Archive archive)
        {
            bool success = true;
            success &= Serializer.Transfer(archive, ref _associatedTransformMode);
            success &= Serializer.Transfer(archive, ref _primaryAction);
            success &= Serializer.Transfer(archive, ref _secondaryAction);
            success &= Serializer.Transfer(archive, ref _actionKeys);
            success &= Serializer.Transfer(archive, ref _hotkeys);
            return success;
        }

        /// <summary>
        /// Returns the <see cref="PrototypeId"/> of the ability slotted in the specified <see cref="AbilitySlot"/>.
        /// </summary>
        public PrototypeId GetAbilityInAbilitySlot(AbilitySlot abilitySlot)
        {
            switch (abilitySlot)
            {
                case AbilitySlot.PrimaryAction: return _primaryAction;
                case AbilitySlot.SecondaryAction: return _secondaryAction;
            }

            // Action keys
            int index = ConvertSlotToArrayIndex(abilitySlot);
            return _actionKeys[index];
        }

        /// <summary>
        /// Sets the ability <see cref="PrototypeId"/> to the specified <see cref="AbilitySlot"/>.
        /// </summary>
        public bool SetAbilityInAbilitySlot(PrototypeId abilityProtoRef, AbilitySlot abilitySlot)
        {
            switch (abilitySlot)
            {
                case AbilitySlot.PrimaryAction:
                    _primaryAction = abilityProtoRef;
                    break;

                case AbilitySlot.SecondaryAction:
                    _secondaryAction = abilityProtoRef;
                    break;

                default:
                    int index = ConvertSlotToArrayIndex(abilitySlot);
                    _actionKeys[index] = abilityProtoRef;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Slots default abilities into all slots.
        /// </summary>
        public bool SlotDefaultAbilities(Avatar avatar)
        {
            AvatarPrototype avatarProto = avatar.AvatarPrototype;
            if (avatarProto == null) return Logger.WarnReturn(false, "SlotDefaultAbilities(): avatarProto == null");

            if (avatarProto.StartingEquippedAbilities.IsNullOrEmpty())
                return true;

            AbilitySlot lastSlot = (AbilitySlot)Math.Min(avatarProto.StartingEquippedAbilities.Length, (int)AbilitySlot.NumActions);

            for (AbilitySlot slot = AbilitySlot.PrimaryAction; slot < lastSlot; slot++)
            {
                AbilityAssignmentPrototype abilityAssignment = avatarProto.StartingEquippedAbilities[(int)slot];
                SetAbilityInAbilitySlot(abilityAssignment.Ability, slot);
            }

            return true;
        }

        /// <summary>
        /// Converts an <see cref="AbilitySlot"/> to an array index.
        /// </summary>
        private static int ConvertSlotToArrayIndex(AbilitySlot slot)
        {
            if (slot < AbilitySlot.ActionKey0)
                return (int)slot;

            if (slot < AbilitySlot.NumActions)
                return (int)slot - 2;

            return Logger.WarnReturn((int)slot, $"ConvertSlotToArrayIndex(): Enum argument is not within an array-stored ability slot range");
        }
    }
}
