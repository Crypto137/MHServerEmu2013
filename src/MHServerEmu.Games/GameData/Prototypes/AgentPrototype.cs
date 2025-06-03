using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.GameData.Prototypes.AI;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)None)]
    public enum Allegiance
    {
        None,
        Hero,
        Neutral,
        Villain,
    }

    #endregion

    public class AgentPrototype : WorldEntityPrototype
    {
        public Allegiance Allegiance { get; protected set; }
        [Mixin]
        public LocomotorPrototype Locomotion { get; protected set; }
        public PrototypeId HitReactCondition { get; protected set; }
        public BehaviorProfilePrototype BehaviorProfile { get; protected set; }
        [Mixin]
        public PopulationInfoPrototype PopulationInfo { get; protected set; }
        public int WakeDelayMS { get; protected set; }
        public int WakeRandomStartMS { get; protected set; }
        public float WakeRange { get; protected set; }
        public float ReturnToDormantRange { get; protected set; }
        public bool TriggersOcclusion { get; protected set; }
        public int HitReactCooldownMS { get; protected set; }
        public LocaleStringId BriefDescription { get; protected set; }
        public float HealthBarRadius { get; protected set; }
        public PrototypeId OnResurrectedPower { get; protected set; }
        public bool PlayDramaticEntranceOnce { get; protected set; }
        public bool WakeStartsVisible { get; protected set; }
        public VOStoryNotificationPrototype[] VOStoryNotifications { get; protected set; }
        public bool HitReactOnClient { get; protected set; }
    }
}
