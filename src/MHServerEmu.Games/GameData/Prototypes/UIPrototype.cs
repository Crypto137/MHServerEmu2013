using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.GameData.Calligraphy;
using MHServerEmu.Games.GameData.Resources;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    public enum PanelScaleMode
    {
        None,
        XStretch,
        YOnly,
        Both,
        ScreenSize
    }

    [AssetEnum((int)GenericGameplay)]
    public enum TipType
    {
        GenericGameplay,
        SpecificGameplay,
    }

    [AssetEnum((int)Environment)]
    public enum TransitionUIType
    {
        Environment,
        HeroOwned,
    }

    [AssetEnum((int)Standard)]
    public enum BannerMessageStyle
    {
        Standard,
        Error,
        FlyIn,
    }

    [AssetEnum((int)None)]
    public enum GameNotificationType
    {
        None,
        PartyInvite,
        GuildInvite,
        PowerPointsAwarded,
        ServerMessage,
        RemoteMission,
        MissionUpdate,
        MatchInvite,
        MatchQueue,
        PvPShop,
        OfferingUI,
    }

    [AssetEnum((int)Default)]
    public enum BannerMessageType
    {
        Default,
        DiscoveryCompleted,
        LevelUp,
        PowerPointsAwarded,
        ItemError,
        PartyInvite,
        PartyError,
        GuildInvite,
        PowerError,
        PowerErrorDoNotQueue,
        PvPDisabledPortalFail, // The client uses the same string lookup as PvPFactionPortalFail, probably a mistake
        PvPFactionPortalFail,
        PvPPartyPortalFail,
        RegionChange,
        MissionAccepted,
        MissionCompleted,
        MissionFailed,
        WaypointUnlocked,
        PowerUnlocked,
        StatProgression,
    }

    [AssetEnum((int)Invalid)]
    public enum VOEventType
    {
        Invalid,
        Spawned,
        Aggro,
    }

    [AssetEnum((int)ScreenLeft)]
    public enum TutorialTipPosition
    {
        ScreenLeft,
        ScreenMiddle,
        ScreenRight
    }

    #endregion

    #region Resource UI prototypes

    public class UIPrototype : Prototype, IBinaryResource
    {
        public UIPanelPrototype[] UIPanels { get; protected set; }

        //---

        public void Deserialize(BinaryReader reader)
        {
            UIPanels = BinaryResourceSerializer.ReadPrototypeContainer<UIPanelPrototype>(reader);
        }
    }

    public class UIPanelPrototype : Prototype, IBinaryResource
    {
        public string PanelName { get; protected set; }
        public string TargetName { get; protected set; }
        public PanelScaleMode ScaleMode { get; protected set; }
        public UIPanelPrototype[] Children { get; protected set; }
        public string WidgetClass { get; protected set; }
        public string SWFName { get; protected set; }
        public bool OpenOnStart { get; protected set; }
        public bool VisibilityToggleable { get; protected set; }
        public bool CanClickThrough { get; protected set; }
        public bool StaticPosition { get; protected set; }
        public bool EntityInteractPanel { get; protected set; }
        public bool UseNewPlacementSystem { get; protected set; }
        // V10_NOTE: No KeepLoaded in 1.10

        //---

        public virtual void Deserialize(BinaryReader reader)
        {
            PanelName = reader.ReadFixedString32();
            TargetName = reader.ReadFixedString32();
            ScaleMode = (PanelScaleMode)reader.ReadUInt32();
            Children = BinaryResourceSerializer.ReadPrototypeContainer<UIPanelPrototype>(reader);
            WidgetClass = reader.ReadFixedString32();
            SWFName = reader.ReadFixedString32();
            OpenOnStart = reader.ReadBoolean();
            VisibilityToggleable = reader.ReadBoolean();
            CanClickThrough = reader.ReadBoolean();
            StaticPosition = reader.ReadBoolean();
            EntityInteractPanel = reader.ReadBoolean();
            UseNewPlacementSystem = reader.ReadBoolean();
        }
    }

    public class StretchedPanelPrototype : UIPanelPrototype
    {
        public Vector2 TopLeftPin { get; protected set; }
        public string TL_X_TargetName { get; protected set; }
        public string TL_Y_TargetName { get; protected set; }
        public Vector2 BottomRightPin { get; protected set; }
        public string BR_X_TargetName { get; protected set; }
        public string BR_Y_TargetName { get; protected set; }

        //---

        public override void Deserialize(BinaryReader reader)
        {
            TopLeftPin = reader.Read<Vector2>();
            TL_X_TargetName = reader.ReadFixedString32();
            TL_Y_TargetName = reader.ReadFixedString32();
            BottomRightPin = reader.Read<Vector2>();
            BR_X_TargetName = reader.ReadFixedString32();
            BR_Y_TargetName = reader.ReadFixedString32();

            base.Deserialize(reader);
        }
    }

    public class AnchoredPanelPrototype : UIPanelPrototype
    {
        public Vector2 SourceAttachmentPin { get; protected set; }
        public Vector2 TargetAttachmentPin { get; protected set; }
        public Vector2 VirtualPixelOffset { get; protected set; }
        public string PreferredLane { get; protected set; }
        public Vector2 OuterEdgePin { get; protected set; }
        public Vector2 NewSourceAttachmentPin { get; protected set; }

        //---

        public override void Deserialize(BinaryReader reader)
        {
            SourceAttachmentPin = reader.Read<Vector2>();
            TargetAttachmentPin = reader.Read<Vector2>();
            VirtualPixelOffset = reader.Read<Vector2>();
            PreferredLane = reader.ReadFixedString32();
            OuterEdgePin = reader.Read<Vector2>();
            NewSourceAttachmentPin = reader.Read<Vector2>();

            base.Deserialize(reader);
        }
    }

    #endregion

    public class UILocalizedInfoPrototype : Prototype
    {
        public LocaleStringId DisplayText { get; protected set; }
        public LocaleStringId TooltipText { get; protected set; }
        public PrototypeId TooltipStyle { get; protected set; }
        public AssetId TooltipFont { get; protected set; }
    }

    public class UILocalizedStatInfoPrototype : UILocalizedInfoPrototype
    {
        public PrototypeId Stat { get; protected set; }
        public int StatValue { get; protected set; }
        public PrototypeId LevelUnlockTooltipStyle { get; protected set; }
        public TooltipSectionPrototype[] TooltipSectionList { get; protected set; }
    }

    public class InventoryUIDataPrototype : Prototype
    {
        public PrototypeId EmptySlotTooltip { get; protected set; }
        public AssetId SlotBackgroundIcon { get; protected set; }
    }

    public class OfferingInventoryUIDataPrototype : Prototype
    {
        public AssetId NotificationIcon { get; protected set; }
        public LocaleStringId NotificationTooltip { get; protected set; }
        public LocaleStringId OfferingDescription { get; protected set; }
        public LocaleStringId OfferingTitle { get; protected set; }
    }

    public class TipEntryPrototype : Prototype
    {
        public LocaleStringId Entry { get; protected set; }
        public int Weight { get; protected set; }
    }

    public class TipEntryCollectionPrototype : Prototype
    {
        public TipEntryPrototype[] TipEntries { get; protected set; }
    }

    public class GenericTipEntryCollectionPrototype : TipEntryCollectionPrototype
    {
    }

    public class RegionTipEntryCollectionPrototype : TipEntryCollectionPrototype
    {
        public PrototypeId[] RegionBindings { get; protected set; }
    }

    public class AvatarTipEntryCollectionPrototype : TipEntryCollectionPrototype
    {
        public PrototypeId[] AvatarBindings { get; protected set; }
    }

    public class WeightedTipCategoryPrototype : Prototype
    {
        public TipType TipType { get; protected set; }
        public int Weight { get; protected set; }
    }

    public class TransitionUIPrototype : Prototype
    {
        public WeightedTipCategoryPrototype[] TipCategories { get; protected set; }
        public TransitionUIType TransitionType { get; protected set; }
        public int Weight { get; protected set; }
    }

    public class TextStylePrototype : Prototype
    {
        public bool Bold { get; protected set; }
        public AssetId Color { get; protected set; }
        public LocaleStringId Tag { get; protected set; }
        public bool Underline { get; protected set; }
        public int FontSize { get; protected set; }
        public AssetId Alignment { get; protected set; }
    }

    public class UINotificationPrototype : Prototype
    {
    }

    public class BannerMessagePrototype : UINotificationPrototype
    {
        public LocaleStringId BannerText { get; protected set; }
        public int FontSize { get; protected set; }
        public int TimeToLiveMS { get; protected set; }
        public BannerMessageStyle MessageStyle { get; protected set; }
        public bool DoNotQueue { get; protected set; }
        public AssetId FontColor { get; protected set; }
    }

    public class GameNotificationPrototype : UINotificationPrototype
    {
        public LocaleStringId BannerText { get; protected set; }
        public GameNotificationType GameNotificationType { get; protected set; }
        public AssetId IconPath { get; protected set; }
        public LocaleStringId TooltipText { get; protected set; }
        public bool PlayAudio { get; protected set; }
        public BannerMessageType BannerType { get; protected set; }
        public bool FlashContinuously { get; protected set; }
        public bool StackNotifications { get; protected set; }
        public bool ShowTimer { get; protected set; }
        public bool ShowScore { get; protected set; }
        public PrototypeId TooltipStyle { get; protected set; }
        public AssetId TooltipFont { get; protected set; }
        public LocaleStringId DisplayText { get; protected set; }
        public int MinimizeTimeDelayMS { get; protected set; }
    }

    public class StoryNotificationPrototype : UINotificationPrototype
    {
        public LocaleStringId DisplayText { get; protected set; }
        public int TimeToLiveMS { get; protected set; }
        public PrototypeId SpeakingEntity { get; protected set; }
    }

    public class VOStoryNotificationPrototype : Prototype
    {
        public VOEventType VOEventType { get; protected set; }
        public StoryNotificationPrototype StoryNotification { get; protected set; }
    }

    public class UICinematicsListPrototype : Prototype
    {
        public PrototypeId[] CinematicsListToPopulate { get; protected set; }
    }

    public class TipPrototype : Prototype
    {
        public LocaleStringId Header { get; protected set; }
        public TutorialTipPosition ScreenPosition { get; protected set; }
        public LocaleStringId Text { get; protected set; }
        public bool SendToChat { get; protected set; }
    }
}
