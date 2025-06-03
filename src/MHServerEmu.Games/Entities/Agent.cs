using MHServerEmu.Core.Logging;
using MHServerEmu.Games.Entities.Inventories;
using MHServerEmu.Games.Entities.Items;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.Properties;

namespace MHServerEmu.Games.Entities
{
    public class Agent : WorldEntity
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        public AgentPrototype AgentPrototype { get => Prototype as AgentPrototype; }

        public Agent(Game game) : base(game)
        {
        }

        public override bool Initialize(EntitySettings settings)
        {
            AgentPrototype agentProto = GameDatabase.GetPrototype<AgentPrototype>(settings.EntityRef);
            if (agentProto == null) return Logger.WarnReturn(false, "Initialize(): agentProto == null");

            if (agentProto.Locomotion.Immobile == false)
                Locomotor = new();

            base.Initialize(settings);

            // InitPowersCollection
            InitLocomotor(settings.LocomotorHeightOverride);

            return true;
        }

        #region World and Positioning

        private bool InitLocomotor(float height = 0.0f)
        {
            if (Locomotor != null)
            {
                AgentPrototype agentPrototype = AgentPrototype;
                if (agentPrototype == null) return false;

                Locomotor.Initialize(agentPrototype.Locomotion, this, height);
                Locomotor.SetGiveUpLimits(8.0f, TimeSpan.FromMilliseconds(250));
            }
            return true;
        }

        #endregion

        #region Inventory

        public InventoryResult CanEquip(Item item, out PropertyEnum propertyRestriction)
        {
            // V10_TODO
            propertyRestriction = PropertyEnum.Invalid;
            return InventoryResult.Success;
        }

        #endregion
    }
}
