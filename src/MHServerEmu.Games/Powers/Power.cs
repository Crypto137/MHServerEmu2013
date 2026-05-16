using MHServerEmu.Games.Entities;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Properties;

namespace MHServerEmu.Games.Powers
{
    public class Power
    {
        public Game Game { get; }
        public PrototypeId PrototypeDataRef { get; }
        public PowerPrototype Prototype { get; }

        public WorldEntity Owner { get; private set; }

        public Power(Game game, PrototypeId prototypeDataRef)
        {
            Game = game;
            PrototypeDataRef = prototypeDataRef;
            Prototype = prototypeDataRef.As<PowerPrototype>();
        }

        public override string ToString()
        {
            return $"powerProtoRef={GameDatabase.GetPrototypeName(PrototypeDataRef)}, owner={Owner}";
        }

        public bool Initialize(WorldEntity owner, PropertyCollection initializeProperties)
        {
            Owner = owner;

            return true;
        }

        public void OnOwnerEnteredWorld()
        {
        }

        public void OnOwnerExitedWorld()
        {
        }

        public void OnDeallocate()
        {
        }

        #region Data Accessors

        public PowerCategoryType GetPowerCategory()
        {
            return Prototype != null ? Prototype.PowerCategory : PowerCategoryType.None;
        }

        public static PowerCategoryType GetPowerCategory(PowerPrototype powerProto)
        {
            return powerProto.PowerCategory;
        }

        public bool IsProcEffect()
        {
            return GetPowerCategory() == PowerCategoryType.ProcEffect;
        }

        public static bool IsProcEffect(PowerPrototype powerProto)
        {
            return GetPowerCategory(powerProto) == PowerCategoryType.ProcEffect;
        }

        public PowerActivationType GetActivationType()
        {
            return Prototype != null ? Prototype.Activation : PowerActivationType.None;
        }

        public static PowerActivationType GetActivationType(PowerPrototype powerProto)
        {
            return powerProto.Activation;
        }

        public bool IsComboEffect()
        {
            return GetPowerCategory() == PowerCategoryType.ComboEffect;
        }

        public static bool IsComboEffect(PowerPrototype powerProto)
        {
            return GetPowerCategory(powerProto) == PowerCategoryType.ComboEffect;
        }

        #endregion
    }
}
