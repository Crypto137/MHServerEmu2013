using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum((int)None)]
    public enum ChatCommandArgumentType
    {
        None,
        Boolean,
        Float,
        Integer,
        String,
    }

    [AssetEnum((int)SystemError)]
    public enum ChatMessageFormatType
    {
        None = -1,
        ChatLocal,
        ChatSay,
        ChatParty,
        ChatTell,
        ChatBroadcast,
        ChatSocial,
        ChatTrade,
        ChatLFG,
        ChatGuild,
        ChatFaction,
        ChatEmote,
        ChatAll,
        ChatMission,
        CombatLog,
        SystemInfo,
        SystemError,
    }

    #endregion

    public class ChatCommandArgumentPrototype : Prototype
    {
        public LocaleStringId Description { get; protected set; }
        public ChatCommandArgumentType Type { get; protected set; }
        public bool Required { get; protected set; }
    }

    public class ChatCommandPrototype : Prototype
    {
        public LocaleStringId Command { get; protected set; }
        public LocaleStringId Description { get; protected set; }
        public AssetId Function { get; protected set; }
        public ChatCommandArgumentPrototype[] Parameters { get; protected set; }
        public bool ShowInHelp { get; protected set; }
        public bool RespondsToSpacebar { get; protected set; }
        public DesignWorkflowState DesignState { get; protected set; }
    }

    public class EmoteChatCommandPrototype : ChatCommandPrototype
    {
        public PrototypeId EmotePower { get; protected set; }
        public LocaleStringId EmoteText { get; protected set; }
    }

    public class ChatChannelPrototype : Prototype
    {
        public ChatMessageFormatType ChannelType { get; protected set; }
        public LocaleStringId PromptText { get; protected set; }
        public PrototypeId TextStyle { get; protected set; }
        public LocaleStringId DisplayName { get; protected set; }
        public PrototypeId ChatCommand { get; protected set; }
        public bool ShowChannelNameInChat { get; protected set; }
        public LocaleStringId ShortName { get; protected set; }
        public bool ShowInChannelList { get; protected set; }
        public bool VisibleOnAllTabs { get; protected set; }
        public DesignWorkflowState DesignState { get; protected set; }
    }
}
