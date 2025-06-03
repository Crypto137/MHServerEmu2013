using Google.ProtocolBuffers;
using Gazillion;
using MHServerEmu.Core.Collections;

namespace MHServerEmu.Core.Network
{
    public static class LatencyBufferTable
    {
        private static readonly GameServerToClientMessage[] NoLatencyBufferMessages = [
            GameServerToClientMessage.NetMessageReadyAndLoggedIn,
            GameServerToClientMessage.NetMessageReadyForTimeSync,
            GameServerToClientMessage.NetMessageAdminCommandResponse,
            GameServerToClientMessage.NetMessageContinuousPowerUpdateToClient,
            GameServerToClientMessage.NetMessageRegionPrimitiveBox,
            GameServerToClientMessage.NetMessageRegionPrimitiveTriangle,
            GameServerToClientMessage.NetMessageRegionPrimitiveSphere,
            GameServerToClientMessage.NetMessageRegionPrimitiveLine,
            GameServerToClientMessage.NetMessageSystemMessage,
            GameServerToClientMessage.NetMessageMissionDebugUIUpdate,
            GameServerToClientMessage.NetMessageDebugEntityPosition,
            GameServerToClientMessage.NetMessageServerVersion,
            GameServerToClientMessage.NetStructPrefetchEntityPower,
            GameServerToClientMessage.NetStructPrefetchCell,
            GameServerToClientMessage.NetMessagePrefetchRegionsForDownload,
            GameServerToClientMessage.NetMessageQueryIsRegionAvailable,
            GameServerToClientMessage.NetStructMatchQueueEntry,
            GameServerToClientMessage.NetMessageMatchQueueListResponse,
            GameServerToClientMessage.NetMessageMatchQueueResponse,
            GameServerToClientMessage.NetMessageMatchInviteNotification,
            GameServerToClientMessage.NetMessageMatchStatsResponse,
            GameServerToClientMessage.NetMessageChatFromGameSystem,
            GameServerToClientMessage.NetMessageChatError,
            GameServerToClientMessage.NetMessageCatalogItems,
            GameServerToClientMessage.NetMessageGetCurrencyBalanceResponse,
            GameServerToClientMessage.NetMessageBuyItemFromCatalogResponse,
            GameServerToClientMessage.NetMessageServerNotification,
            GameServerToClientMessage.NetMessageSyncTimeReply,
            GameServerToClientMessage.NetMessageForceDisconnect,
            GameServerToClientMessage.NetMessageCraftingFinished,
            GameServerToClientMessage.NetMessageCraftingFailed,
            GameServerToClientMessage.MessageReportEntry,
            GameServerToClientMessage.NetMessageMessageReport,
            GameServerToClientMessage.NetMessageConsoleMessage,
            GameServerToClientMessage.NetMessageReloadPackagesStart,
            GameServerToClientMessage.NetMessagePlayStoryBanter,
            GameServerToClientMessage.NetMessagePlayKismetSeq,
            GameServerToClientMessage.NetMessageGracefulDisconnectAck,
            GameServerToClientMessage.NetMessageLiveTuningUpdate,
            GameServerToClientMessage.NetMessageUpdateSituationalTarget,
            GameServerToClientMessage.NetMessageItemBindingChanged,
            GameServerToClientMessage.NetMessageItemsHeldForRecovery,
            GameServerToClientMessage.NetMessageItemRecovered,
            GameServerToClientMessage.NetMessageSwitchToPendingNewAvatarFailed,
            GameServerToClientMessage.NetMessageMetaGameWaveUpdate,
            GameServerToClientMessage.NetMessagePvEInstanceCrystalUpdate,
            GameServerToClientMessage.NetMessagePvEInstanceDeathUpdate,
            GameServerToClientMessage.NetMessageShowTutorialTip
        ];

        // Cache message ids in a bit array for fast checks
        private static readonly GBitArray NoLatencyBufferMessageBits = new();

        static LatencyBufferTable()
        {
            NoLatencyBufferMessageBits.Resize((int)NoLatencyBufferMessages[^1]);
            foreach (GameServerToClientMessage messageId in NoLatencyBufferMessages)
                NoLatencyBufferMessageBits.Set((int)messageId);
        }

        public static bool IsLatencyBufferMessage(IMessage message)
        {
            if (message.DescriptorForType.File != GameServerToClient.Descriptor)
                return false;

            int index = (int)ProtocolDispatchTable.Instance.GetMessageProtocolId(message);
            return NoLatencyBufferMessageBits.Get(index) == false;
        }
    }
}
