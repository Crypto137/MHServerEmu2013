using Gazillion;
using Google.ProtocolBuffers;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.Network;
using MHServerEmu.Core.System.Time;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.Commands;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.Entities.Avatars;
using MHServerEmu.Games.Entities.Inventories;
using MHServerEmu.Games.Entities.Items;
using MHServerEmu.Games.Entities.Locomotion;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.MTXStore;
using MHServerEmu.Games.Navi;
using MHServerEmu.Games.Regions;

namespace MHServerEmu.Games.Network
{
    public class PlayerConnection : NetClient
    {
        private const ushort MuxChannel = 1;

        private static readonly Logger Logger = LogManager.CreateLogger();

        private static ulong CurrentPlayerDbGuid = 0x2000000000000001;

        private readonly IFrontendClient _frontendClient;

        public Game Game { get; }

        public AreaOfInterest AOI { get; }
        public TransferParams TransferParams { get; }

        public Player Player { get; private set; }

        public ulong PlayerDbId { get; }

        public PlayerConnection(Game game, IFrontendClient frontendClient) : base(MuxChannel, frontendClient)
        {
            Game = game;
            _frontendClient = frontendClient;
            PlayerDbId = CurrentPlayerDbGuid++;

            AOI = new(this);
            TransferParams = new(this);

            // Set start target
            TransferParams.SetTarget(GameDatabase.GlobalsPrototype.DefaultStartTarget);

            // Initialize player
            InitializePlayer();
        }

        public override string ToString()
        {
            return _frontendClient.ToString();
        }

        public bool Initialize()
        {
            // V10_TODO: NetMessageQueryIsRegionAvailable
            Game.NetworkManager.SetPlayerConnectionPending(this);
            return true;
        }

        #region NetClient Implementation

        public override void OnDisconnect()
        {
            // Post-disconnection cleanup (save data, remove entities, etc).
            AOI.SetRegion(0, true);

            Game.EntityManager.DestroyEntity(Player);
        }

        #endregion

        public void InitializePlayer()
        {
            {
                using EntitySettings settings = ObjectPoolManager.Instance.Get<EntitySettings>();
                settings.EntityRef = GameDatabase.GlobalsPrototype.DefaultPlayer;
                settings.DbGuid = PlayerDbId;
                settings.OptionFlags = EntitySettingsOptionFlags.PopulateInventories;
                settings.PlayerConnection = this;
                settings.PlayerName = $"0x{settings.DbGuid:X}";
                Player = Game.EntityManager.CreateEntity(settings) as Player;
            }

            // AvatarLibrary
            foreach (PrototypeId avatarProtoRef in DataDirectory.Instance.IteratePrototypesInHierarchy<AvatarPrototype>(PrototypeIterateFlags.NoAbstractApprovedOnly))
                Player.CreateAvatar(avatarProtoRef);
        }

        public void EnterGame()
        {
            if (Player.IsInGame == false)
                Player.EnterGame();

            if (Player.CurrentAvatar == null)
            {
                if (Player.IsOnLoadingScreen)
                    Player.DequeueLoadingScreen();

                SendMessage(NetMessageSelectStartingAvatarForNewPlayer.DefaultInstance);
                return;
            }

            AOI.SetRegion(0, false);

            Region region = Game.RegionManager.GetOrGenerateRegionForPlayer(TransferParams.DestTargetRegionProtoRef);
            TransferParams.DestRegionId = region.Id;
            TransferParams.FindStartLocation(out Vector3 position, out Orientation orientation);

            AOI.SetRegion(region.Id, false, position, orientation);
        }

        public bool MoveToTarget(PrototypeId targetProtoRef)
        {
            RegionConnectionTargetPrototype targetProto = targetProtoRef.As<RegionConnectionTargetPrototype>();
            if (targetProto == null) return Logger.WarnReturn(false, "MoveToTarget(): targetProto == null");

            Region region = Game.RegionManager.GetOrGenerateRegionForPlayer(targetProto.Region);
            if (region == null) return Logger.WarnReturn(false, "MoveToTarget(): region == null");

            TransferParams.SetTarget(targetProtoRef);

            Player.CurrentAvatar.ExitWorld();
            Game.NetworkManager.SetPlayerConnectionPending(this);

            return true;
        }

        public void SendSystemChatMessage(string message)
        {
            _frontendClient.SendMessage(2, ChatNormalMessage.CreateBuilder()
                .SetRoomType(ChatRoomTypes.CHAT_ROOM_TYPE_BROADCAST_ALL_SERVERS)
                .SetFromPlayerName(string.Empty)
                .SetToPlayerName(string.Empty)
                .SetTheMessage(ChatMessage.CreateBuilder().SetBody(message))
                .Build());
        }

        public void PlayKismetSeq(ulong kismetSeqProtoRef)
        {
            SendMessage(NetMessagePlayKismetSeq.CreateBuilder().SetKismetSeqPrototypeId(kismetSeqProtoRef).Build());
        }

        /// <summary>
        /// Sends an <see cref="IMessage"/> instance over this <see cref="PlayerConnection"/>.
        /// </summary>
        public void SendMessage(IMessage message)
        {
            // NOTE: The client goes Game -> NetworkManager -> SendMessage() -> postOutboundMessageToClient() -> postMessage() here,
            // but we simplify everything and just post the message directly.
            PostMessage(message);
        }

        #region Message Handling

        /// <summary>
        /// Handles a <see cref="MailboxMessage"/>.
        /// </summary>
        public override void ReceiveMessage(in MailboxMessage message)
        {
            if (!Verify.IsNotNull(Player)) return;

            switch ((ClientToGameServerMessage)message.Id)
            {
                // case ClientToGameServerMessage.NetMessagePlayerSystemMetrics:            OnPlayerSystemMetrics(message); break;
                case ClientToGameServerMessage.NetMessageSyncTimeRequest:                   OnSyncTimeRequest(message); break;
                // case ClientToGameServerMessage.NetMessageSetTimeDialation:               OnSetTimeDialation(message); break;
                // case ClientToGameServerMessage.NetMessageIsRegionAvailable:              OnIsRegionAvailable(message); break;
                case ClientToGameServerMessage.NetMessageUpdateAvatarState:                 OnUpdateAvatarState(message); break;
                case ClientToGameServerMessage.NetMessageCellLoaded:                        OnCellLoaded(message); break;
                case ClientToGameServerMessage.NetMessageAdminCommand:                      OnAdminCommand(message); break;
                // case ClientToGameServerMessage.NetMessageTeleportAckResponse:            OnTeleportAckResponse(message); break;
                case ClientToGameServerMessage.NetMessageTryActivatePower:                  OnTryActivatePower(message); break;
                case ClientToGameServerMessage.NetMessagePowerRelease:                      OnPowerRelease(message); break;
                case ClientToGameServerMessage.NetMessageTryCancelPower:                    OnTryCancelPower(message); break;
                case ClientToGameServerMessage.NetMessageTryCancelActivePower:              OnTryCancelActivePower(message); break;
                case ClientToGameServerMessage.NetMessageContinuousPowerUpdateToServer:     OnContinuousPowerUpdate(message); break;
                case ClientToGameServerMessage.NetMessageCancelPendingAction:               OnCancelPendingAction(message); break;
                // case ClientToGameServerMessage.NetMessageConfirmWeaponMissingStatus:     OnConfirmWeaponMissingStatus(message); break;
                // case ClientToGameServerMessage.NetMessageStartAIDebugUI:                 OnStartAIDebugUI(message); break;
                // case ClientToGameServerMessage.NetMessageStopAIDebugUI:                  OnStopAIDebugUI(message); break;
                // case ClientToGameServerMessage.NetMessageStartMissionDebugUI:            OnStartMissionDebugUI(message); break;
                // case ClientToGameServerMessage.NetMessageStopMissionDebugUI:             OnStopMissionDebugUI(message); break;
                // case ClientToGameServerMessage.NetMessageStartPropertiesDebugUI:         OnStartPropertiesDebugUI(message); break;
                // case ClientToGameServerMessage.NetMessageStopPropertiesDebugUI:          OnStopPropertiesDebugUI(message); break;
                // case ClientToGameServerMessage.NetMessageStartConditionsDebugUI:         OnStartConditionsDebugUI(message); break;
                // case ClientToGameServerMessage.NetMessageStopConditionsDebugUI:          OnStopConditionsDebugUI(message); break;
                // case ClientToGameServerMessage.NetMessageStartPowersDebugUI:             OnStartPowersDebugUI(message); break;
                // case ClientToGameServerMessage.NetMessageStopPowersDebugUI:              OnStopPowersDebugUI(message); break;
                case ClientToGameServerMessage.NetMessagePing:                              OnPing(message); break;
                // case ClientToGameServerMessage.NetMessagePickupInteraction:              OnPickupInteraction(message); break;
                case ClientToGameServerMessage.NetMessageTryInventoryMove:                  OnTryInventoryMove(message); break;
                // case ClientToGameServerMessage.NetMessageTryMoveCraftingResultsToGeneral:OnTryMoveCraftingResultsToGeneral(message); break;
                case ClientToGameServerMessage.NetMessageInventoryTrashItem:                OnInventoryTrashItem(message); break;
                // case ClientToGameServerMessage.NetMessageThrowInteraction:               OnThrowInteraction(message); break;
                // case ClientToGameServerMessage.NetMessagePerformPreInteractPower:        OnPerformPreInteractPower(message); break;
                case ClientToGameServerMessage.NetMessageUseInteractableObject:             OnUseInteractableObject(message); break;
                // case ClientToGameServerMessage.NetMessageTryCraft:                       OnTryCraft(message); break;
                case ClientToGameServerMessage.NetMessageUseWaypoint:                       OnUseWaypoint(message); break;
                // case ClientToGameServerMessage.NetMessageDebugAcquireAndSwitchToAvatar:  OnDebugAcquireAndSwitchToAvatar(message); break;
                case ClientToGameServerMessage.NetMessageSwitchAvatar:                      OnSwitchAvatar(message); break;
                // case ClientToGameServerMessage.NetMessageAssignHotkey:                   OnAssignHotkey(message); break;
                // case ClientToGameServerMessage.NetMessageUnassignHotkey:                 OnUnassignHotkey(message); break;
                case ClientToGameServerMessage.NetMessageAssignAbility:                     OnAssignAbility(message); break;
                case ClientToGameServerMessage.NetMessageUnassignAbility:                   OnUnassignAbility(message); break;
                case ClientToGameServerMessage.NetMessageSwapAbilities:                     OnSwapAbilities(message); break;
                // case ClientToGameServerMessage.NetMessageModCommitTemporary:             OnModCommitTemporary(message); break;
                // case ClientToGameServerMessage.NetMessageModReset:                       OnModReset(message); break;
                // case ClientToGameServerMessage.NetMessagePowerPointAllocationCommit:     OnPowerPointAllocationCommit(message); break;
                case ClientToGameServerMessage.NetMessageRequestDeathRelease:               OnRequestDeathRelease(message); break;
                // case ClientToGameServerMessage.NetMessageRequestResurrectDecline:        OnRequestResurrectDecline(message); break;
                // case ClientToGameServerMessage.NetMessageRequestResurrectAvatar:         OnRequestResurrectAvatar(message); break;
                case ClientToGameServerMessage.NetMessageReturnToHub:                       OnReturnToHub(message); break;
                // case ClientToGameServerMessage.NetMessageRequestStoryWarp:               OnRequestStoryWarp(message); break;
                // case ClientToGameServerMessage.NetMessageInvitePlayer:                   OnInvitePlayer(message); break;
                // case ClientToGameServerMessage.NetMessageRequestPartyJoinPortal:         OnRequestPartyJoinPortal(message); break;
                // case ClientToGameServerMessage.NetMessageDeclineGroupInvite:             OnDeclineGroupInvite(message); break;
                // case ClientToGameServerMessage.NetMessageLeaveGroup:                     OnLeaveGroup(message); break;
                // case ClientToGameServerMessage.NetMessageChangeGroupLeader:              OnChangeGroupLeader(message); break;
                // case ClientToGameServerMessage.NetMessageBootPlayer:                     OnBootPlayer(message); break;
                // case ClientToGameServerMessage.NetMessageDisbandGroup:                   OnDisbandGroup(message); break;
                // case ClientToGameServerMessage.NetMessageMatchQueueListRequest:          OnMatchQueueListRequest(message); break;
                // case ClientToGameServerMessage.NetMessageMatchQueueRequest:              OnMatchQueueRequest(message); break;
                // case ClientToGameServerMessage.NetMessageMatchStatisticsRequest:         OnMatchStatisticsRequest(message); break;
                // case ClientToGameServerMessage.NetMessageMatchInviteResponse:            OnMatchInviteResponse(message); break;
                // case ClientToGameServerMessage.NetMessageDuelInvite:                     OnDuelInvite(message); break;
                // case ClientToGameServerMessage.NetMessageDuelAccept:                     OnDuelAccept(message); break;
                // case ClientToGameServerMessage.NetMessageDuelCancel:                     OnDuelCancel(message); break;
                // case ClientToGameServerMessage.NetMessageMetaGameUpdateNotification:     OnMetaGameUpdateNotification(message); break;
                case ClientToGameServerMessage.NetMessageChat:                              OnChat(message); break;
                // case ClientToGameServerMessage.NetMessageTell:                           OnTell(message); break;
                // case ClientToGameServerMessage.NetMessageReportPlayer:                   OnReportPlayer(message); break;
                case ClientToGameServerMessage.NetMessageGetCatalog:                        OnGetCatalog(message); break;
                case ClientToGameServerMessage.NetMessageGetCurrencyBalance:                OnGetCurrencyBalance(message); break;
                case ClientToGameServerMessage.NetMessageBuyItemFromCatalog:                OnBuyItemFromCatalog(message); break;
                // case ClientToGameServerMessage.NetMessageEntityPreviewerNewTargets:      OnEntityPreviewerNewTargets(message); break;
                // case ClientToGameServerMessage.NetMessageEntityPreviewerClearTargets:    OnEntityPreviewerClearTargets(message); break;
                // case ClientToGameServerMessage.NetMessageEntityPreviewerSetTargetRef:    OnEntityPreviewerSetTargetRef(message); break;
                // case ClientToGameServerMessage.NetMessageEntityPreviewerActivatePower:   OnEntityPreviewerActivatePower(message); break;
                // case ClientToGameServerMessage.NetMessageEntityPreviewerAddTarget:       OnEntityPreviewerAddTarget(message); break;
                // case ClientToGameServerMessage.NetMessageEntityPreviewerSetEntityState:  OnEntityPreviewerSetEntityState(message); break;
                // case ClientToGameServerMessage.NetMessageEntityPreviewerApplyConditions: OnEntityPreviewerApplyConditions(message); break;
                case ClientToGameServerMessage.NetMessageCreateNewPlayerWithSelectedStartingAvatar: OnCreateNewPlayerWithSelectedStartingAvatar(message); break;
                // case ClientToGameServerMessage.NetMessageOnKioskStartButtonPressed:      OnKioskStartButtonPressed(message); break;
                // case ClientToGameServerMessage.NetMessageNotifyFullscreenMovieFinished:  OnNotifyFullscreenMovieFinished(message); break;
                // case ClientToGameServerMessage.NetMessageBotSetLevel:                    OnBotSetLevel(message); break;
                // case ClientToGameServerMessage.NetMessageBotGodMode:                     OnBotGodMode(message); break;
                // case ClientToGameServerMessage.NetMessageBotPickAvatar:                  OnBotPickAvatar(message); break;
                // case ClientToGameServerMessage.NetMessageBotRegionChange:                OnBotRegionChange(message); break;
                // case ClientToGameServerMessage.NetMessageBotWarpAreaNext:                OnBotWarpAreaNext(message); break;
                // case ClientToGameServerMessage.NetMessageBotLootGive:                    OnBotLootGive(message); break;
                // case ClientToGameServerMessage.NetMessageBotSetPvPFaction:               OnBotSetPvPFaction(message); break;
                // case ClientToGameServerMessage.NetMessageBotPvPQueue:                    OnBotPvPQueue(message); break;
                // case ClientToGameServerMessage.NetMessageGetTrackerReport:               OnGetTrackerReport(message); break;
                // case ClientToGameServerMessage.NetMessagePlayKismetSeqDone:              OnPlayKismetSeqDone(message); break;
                // case ClientToGameServerMessage.NetMessageVerifyFailedForRepId:           OnVerifyFailedForRepId(message); break;
                case ClientToGameServerMessage.NetMessageGracefulDisconnect:                OnGracefulDisconnect(message); break;
                // case ClientToGameServerMessage.NetMessageRequestStartNewGame:            OnRequestStartNewGame(message); break;
                case ClientToGameServerMessage.NetMessageSetDialogTarget:                   OnSetDialogTarget(message); break;
                // case ClientToGameServerMessage.NetMessageVendorRequestBuyItemFrom:       OnVendorRequestBuyItemFrom(message); break;
                // case ClientToGameServerMessage.NetMessageVendorRequestSellItemTo:        OnVendorRequestSellItemTo(message); break;
                // case ClientToGameServerMessage.NetMessageVendorRequestDonateItemTo:      OnVendorRequestDonateItemTo(message); break;
                // case ClientToGameServerMessage.NetMessageVendorRequestRefresh:           OnVendorRequestRefresh(message); break;
                // case ClientToGameServerMessage.NetMessageTryModifyCommunityMemberCircle: OnTryModifyCommunityMemberCircle(message); break;
                // case ClientToGameServerMessage.NetMessageGuildMessageToPlayerManager:    OnGuildMessageToPlayerManager(message); break;
                // case ClientToGameServerMessage.NetMessageSetShowTips:                    OnSetShowTips(message); break;
                case ClientToGameServerMessage.NetMessageSetTipSeen:                        OnSetTipSeen(message); break;
                // case ClientToGameServerMessage.NetMessageResetSeenTips:                  OnResetSeenTips(message); break;
                // case ClientToGameServerMessage.NetMessageTryMoveInventoryContentsToGeneral: OnTryMoveInventoryContentsToGeneral(message); break;
                // case ClientToGameServerMessage.NetMessageSetPlayerGameplayOptions:       OnSetPlayerGameplayOptions(message); break;
                // case ClientToGameServerMessage.NetMessageTeleportToPartyMember:          OnTeleportToPartyMember(message); break;

                default: Logger.Warn($"ReceiveMessage(): Unhandled {(ClientToGameServerMessage)message.Id} [{message.Id}]"); break;
            }
        }

        private void OnSyncTimeRequest(in MailboxMessage message)
        {
            var syncTimeRequest = message.As<NetMessageSyncTimeRequest>();
            if (!Verify.IsNotNull(syncTimeRequest)) return;

            NetMessageSyncTimeReply reply = NetMessageSyncTimeReply.CreateBuilder()
                .SetGameTimeClientSent(syncTimeRequest.GameTimeClientSent)
                .SetGameTimeServerReceived(message.GameTimeReceived.Ticks / 10)
                .SetGameTimeServerSent(Clock.GameTime.Ticks / 10)
                .SetDateTimeClientSent(syncTimeRequest.DateTimeClientSent)
                .SetDateTimeServerReceived(message.DateTimeReceived.Ticks / 10)
                .SetDateTimeServerSent(Clock.UnixTime.Ticks / 10)
                .SetDialation(1.0f)
                .SetGametimeDialationStarted(0)
                .SetDatetimeDialationStarted(0)
                .Build();

            SendMessage(reply);
            FlushMessages();    // Send the reply ASAP for more accurate timing
        }

        private void OnUpdateAvatarState(in MailboxMessage message)
        {
            var updateAvatarState = message.As<NetMessageUpdateAvatarState>();
            if (!Verify.IsNotNull(updateAvatarState)) return;

            Avatar avatar = Player.CurrentAvatar;
            if (avatar == null || avatar.IsAliveInWorld == false)
                return;

            ulong avatarEntityId = updateAvatarState.IdAvatar;
            if (avatarEntityId != avatar.Id)
                return;

            Vector3 syncPosition = new(updateAvatarState.Position);
            Orientation syncOrientation = new(updateAvatarState.Orientation.X, updateAvatarState.Orientation.Y, updateAvatarState.Orientation.Z);

            // Region location
            bool canMove = true; //avatar.CanMove();
            bool canRotate = true; //avatar.CanRotate();
            Vector3 position = avatar.RegionLocation.Position;
            Orientation orientation = avatar.RegionLocation.Orientation;

            if (canMove || canRotate)
            {
                position = syncPosition;
                orientation = syncOrientation;

                // Update position without sending it to clients (local avatar is moved by its own client, other avatars are moved by locomotion)
                if (avatar.ChangeRegionPosition(canMove ? position : null, canRotate ? orientation : null, ChangePositionFlags.DoNotSendToClients) == ChangePositionResult.PositionChanged)
                {
                    /* V10_TODO
                    // Clear pending action if successfully updated position
                    if (avatar.IsInPendingActionState(PendingActionState.MovingToRange) == false &&
                        avatar.IsInPendingActionState(PendingActionState.WaitingForPrevPower) == false &&
                        avatar.IsInPendingActionState(PendingActionState.FindingLandingSpot) == false)
                    {
                        avatar.CancelPendingAction();
                    }
                    */
                }

                //avatar.UpdateNavigationInfluence();
            }

            // Update locomotion state
            if (updateAvatarState.HasLocomotionstate && avatar.Locomotor != null)
            {
                using var pathNodesHandle = ListPool<NaviPathNode>.Instance.Get(out List<NaviPathNode> pathNodes);
                LocomotionState newSyncState = new(pathNodes);
                newSyncState.Set(ref avatar.Locomotor.LastSyncState);

                // V10_NOTE: Because this is just a protobuf instead of archive in 1.10, we don't need to be as careful with transferring state here
                LocomotionState.SerializeFrom(updateAvatarState.Locomotionstate, ref newSyncState);

                avatar.Locomotor.SetSyncState(ref newSyncState, position, orientation);
            }

            // optional bool isTeleport - what do we do with this?
        }

        private void OnCellLoaded(in MailboxMessage message)
        {
            var cellLoaded = message.As<NetMessageCellLoaded>();
            if (!Verify.IsNotNull(cellLoaded)) return;

            Player.OnCellLoaded(cellLoaded.CellId, cellLoaded.RegionId);
        }

        private void OnAdminCommand(in MailboxMessage message)
        {
            var adminCommand = message.As<NetMessageAdminCommand>();
            if (!Verify.IsNotNull(adminCommand)) return;

            Game.AdminCommandManager.OnAdminCommand(Player, adminCommand);
        }

        private void OnTryActivatePower(in MailboxMessage message)
        {
            var tryActivatePower = message.As<NetMessageTryActivatePower>();
            if (!Verify.IsNotNull(tryActivatePower)) return;

            // V10_TODO
        }

        private void OnPowerRelease(in MailboxMessage message)
        {
            var powerRelease = message.As<NetMessagePowerRelease>();
            if (!Verify.IsNotNull(powerRelease)) return;

            // V10_TODO
        }

        private void OnTryCancelPower(in MailboxMessage message)
        {
            var tryCancelPower = message.As<NetMessageTryCancelPower>();
            if (!Verify.IsNotNull(tryCancelPower)) return;

            // V10_TODO
        }

        private void OnTryCancelActivePower(in MailboxMessage message)
        {
            var tryCancelActivePower = message.As<NetMessageTryCancelActivePower>();
            if (!Verify.IsNotNull(tryCancelActivePower)) return;

            // V10_TODO
        }

        private void OnContinuousPowerUpdate(in MailboxMessage message)
        {
            var continuousPowerUpdate = message.As<NetMessageContinuousPowerUpdateToServer>();
            if (!Verify.IsNotNull(continuousPowerUpdate)) return;

            // V10_TODO
        }

        private void OnCancelPendingAction(in MailboxMessage message)
        {
            var cancelPendingAction = message.As<NetMessageCancelPendingAction>();
            if (!Verify.IsNotNull(cancelPendingAction)) return;

            // V10_TODO
        }

        private void OnPing(in MailboxMessage message)
        {
            var ping = message.As<NetMessagePing>();
            if (!Verify.IsNotNull(ping)) return;

            // Copy request info
            var response = NetMessagePingResponse.CreateBuilder()
                .SetDisplayOutput(ping.DisplayOutput)
                .SetRequestSentClientTime(ping.SendClientTime);

            if (ping.HasSendGameTime)
                response.SetRequestSentGameTime(ping.SendGameTime);

            // We ignore other ping metrics (client latency, fps, etc.)

            // Add response data
            response.SetRequestNetReceivedGameTime((ulong)message.GameTimeReceived.TotalMilliseconds)
                .SetResponseSendTime((ulong)Clock.GameTime.TotalMilliseconds);

            SendMessage(response.Build());
            FlushMessages();    // Send the reply ASAP for more accurate timing (NOTE: this is not accurate to our packet dumps, but gives better ping values)
        }

        private bool OnTryInventoryMove(in MailboxMessage message)
        {
            var tryInventoryMove = message.As<NetMessageTryInventoryMove>();
            if (tryInventoryMove == null) return Logger.WarnReturn(false, "OnTryInventoryMove(): Failed to retrieve message");

            Item item = Game.EntityManager.GetEntity<Item>(tryInventoryMove.ItemId);
            if (item == null) return Logger.WarnReturn(false, "OnTryInventoryMove(): item == null");

            if (item.GetOwnerOfType<Player>() != Player) return Logger.WarnReturn(false, "OnTryInventoryMove(): item.GetOwnerOfType<Player>() != Player");

            Entity newOwner = Game.EntityManager.GetEntity<Entity>(tryInventoryMove.ToInventoryOwnerId);
            if (newOwner == null) return Logger.WarnReturn(false, "OnTryInventoryMove(): newOwner == null");

            Inventory inventory = newOwner.GetInventoryByRef((PrototypeId)tryInventoryMove.ToInventoryPrototype);
            if (inventory == null) return Logger.WarnReturn(false, "OnTryInventoryMove(): inventory == null");

            InventoryResult result = item.ChangeInventoryLocation(inventory, tryInventoryMove.ToSlot);
            if (result != InventoryResult.Success)
                return Logger.WarnReturn(false, $"OnTryInventoryMove(): Failed because {result}");

            return true;
        }

        private void OnInventoryTrashItem(in MailboxMessage message)
        {
            var inventoryTrashItem = message.As<NetMessageInventoryTrashItem>();
            if (!Verify.IsNotNull(inventoryTrashItem)) return;

            ulong itemId = inventoryTrashItem.ItemId;
            if (!Verify.IsTrue(itemId != Entity.InvalidId)) return;

            Item item = Game.EntityManager.GetEntity<Item>(itemId);
            if (!Verify.IsNotNull(item)) return;

            Player.TrashItem(item);
        }

        private void OnUseInteractableObject(in MailboxMessage message)
        {
            var useInteractableObject = message.As<NetMessageUseInteractableObject>();
            if (!Verify.IsNotNull(useInteractableObject)) return;

            Avatar avatar = Player.CurrentAvatar;
            if (!Verify.IsNotNull(avatar)) return;

            avatar.UseInteractableObject(useInteractableObject.IdTarget, (PrototypeId)useInteractableObject.MissionPrototypeRef);
        }

        private void OnUseWaypoint(in MailboxMessage message)
        {
            var useWaypoint = message.As<NetMessageUseWaypoint>();
            if (!Verify.IsNotNull(useWaypoint)) return;

            WaypointPrototype waypointProto = GameDatabase.GetPrototype<WaypointPrototype>((PrototypeId)useWaypoint.WaypointDataRef);
            if (!Verify.IsNotNull(waypointProto)) return;

            if (MoveToTarget(waypointProto.Destination) == false)
                SendSystemChatMessage($"Waypoint destination {waypointProto.Destination.GetName()} is not available.");
        }

        private void OnSwitchAvatar(in MailboxMessage message)
        {
            var switchAvatar = message.As<NetMessageSwitchAvatar>();
            if (!Verify.IsNotNull(switchAvatar)) return;

            Avatar avatar = Game.EntityManager.GetEntity<Avatar>(switchAvatar.AvatarId);
            if (!Verify.IsNotNull(avatar)) return;

            if (!Verify.IsTrue(avatar.GetOwnerOfType<Player>() == Player)) return;

            Player.SwitchAvatar(avatar.PrototypeDataRef);
        }

        private void OnAssignAbility(in MailboxMessage message)
        {
            var assignAbility = message.As<NetMessageAssignAbility>();
            if (!Verify.IsNotNull(assignAbility)) return;

            Avatar avatar = Game.EntityManager.GetEntity<Avatar>(assignAbility.AvatarId);
            if (!Verify.IsNotNull(avatar)) return;

            if (!Verify.IsTrue(avatar.GetOwnerOfType<Player>() == Player, $"Player [{Player}] is attempting to slot ability for avatar [{avatar}] that belongs to another player"))
                return;

            avatar.SlotAbility((PrototypeId)assignAbility.PrototypeRefId, (AbilitySlot)assignAbility.SlotNumber, false, true);
        }

        private void OnUnassignAbility(in MailboxMessage message)
        {
            var unassignAbility = message.As<NetMessageUnassignAbility>();
            if (!Verify.IsNotNull(unassignAbility)) return;

            Avatar avatar = Game.EntityManager.GetEntity<Avatar>(unassignAbility.AvatarId);
            if (!Verify.IsNotNull(avatar)) return;

            if (!Verify.IsTrue(avatar.GetOwnerOfType<Player>() == Player, $"Player [{Player}] is attempting to unslot ability for avatar [{avatar}] that belongs to another player"))
                return;

            avatar.UnslotAbility((AbilitySlot)unassignAbility.SlotNumber, true);
        }

        private void OnSwapAbilities(in MailboxMessage message)
        {
            var swapAbilities = message.As<NetMessageSwapAbilities>();
            if (!Verify.IsNotNull(swapAbilities)) return;

            Avatar avatar = Game.EntityManager.GetEntity<Avatar>(swapAbilities.AvatarId);
            if (!Verify.IsNotNull(avatar)) return;

            if (!Verify.IsTrue(avatar.GetOwnerOfType<Player>() == Player, $"Player [{Player}] is attempting to swap abilities for avatar [{avatar}] that belongs to another player"))
                return;

            avatar.SwapAbilities((AbilitySlot)swapAbilities.SlotNumberA, (AbilitySlot)swapAbilities.SlotNumberB, true);
        }

        private void OnRequestDeathRelease(in MailboxMessage message)
        {
            var requestDeathRelease = message.As<NetMessageRequestDeathRelease>();
            if (!Verify.IsNotNull(requestDeathRelease)) return;

            PrototypeId respawnTarget = Player.CurrentAvatar.Region.Prototype.RespawnTarget;
            MoveToTarget(respawnTarget);
        }

        private void OnReturnToHub(in MailboxMessage message)
        {
            var returnToHub = message.As<NetMessageReturnToHub>();
            if (!Verify.IsNotNull(returnToHub)) return;

            MoveToTarget((PrototypeId)15718422430560164872);
        }

        private void OnChat(in MailboxMessage message)
        {
            var chat = message.As<NetMessageChat>();
            if (!Verify.IsNotNull(chat)) return;

            if (CommandManagerMini.Instance.TryParse(chat.TheMessage.Body, this))
                return;

            Logger.Info($"[{chat.RoomType}] {chat.TheMessage.Body}");

            var chatMessage = ChatNormalMessage.CreateBuilder()
                .SetRoomType(chat.RoomType)
                .SetFromPlayerName(Player.PlayerName.Get())
                .SetToPlayerName(string.Empty)
                .SetTheMessage(chat.TheMessage)
                .Build();

            foreach (PlayerConnection playerConnection in Game.NetworkManager)
                playerConnection.FrontendClient.SendMessage(2, chatMessage);
        }

        private void OnGetCatalog(in MailboxMessage message)
        {
            var getCatalog = message.As<NetMessageGetCatalog>();
            if (!Verify.IsNotNull(getCatalog)) return;

            CatalogManager.Instance.OnGetCatalog(Player, getCatalog);
        }

        private void OnGetCurrencyBalance(in MailboxMessage message)
        {
            var getCurrencyBalance = message.As<NetMessageGetCurrencyBalance>();
            if (!Verify.IsNotNull(getCurrencyBalance)) return;

            CatalogManager.Instance.OnGetCurrencyBalance(Player);
        }

        private void OnBuyItemFromCatalog(in MailboxMessage message)
        {
            var buyItemFromCatalog = message.As<NetMessageBuyItemFromCatalog>();
            if (!Verify.IsNotNull(buyItemFromCatalog)) return;

            CatalogManager.Instance.OnBuyItemFromCatalog(Player, buyItemFromCatalog);
        }

        private void OnCreateNewPlayerWithSelectedStartingAvatar(in MailboxMessage message)
        {
            var createNewPlayer = message.As<NetMessageCreateNewPlayerWithSelectedStartingAvatar>();
            if (!Verify.IsNotNull(createNewPlayer)) return;

            Game.NetworkManager.SetPlayerConnectionPending(this);

            Avatar avatar = Player.GetAvatar((PrototypeId)createNewPlayer.StartingAvatarPrototypeId);
            Inventory avatarInPlay = Player.GetInventory(InventoryConvenienceLabel.AvatarInPlay);
            avatar.ChangeInventoryLocation(avatarInPlay);
        }

        private void OnGracefulDisconnect(in MailboxMessage message)
        {
            Logger.Trace($"OnGracefulDisconnect(): Player=[{Player}]");
            SendMessage(NetMessageGracefulDisconnectAck.DefaultInstance);
        }

        private void OnSetDialogTarget(in MailboxMessage message)
        {
            var setDialogTarget = message.As<NetMessageSetDialogTarget>();
            if (!Verify.IsNotNull(setDialogTarget)) return;

            // V10_TODO
        }

        private void OnSetTipSeen(in MailboxMessage message)
        {
            var setTipSeen = message.As<NetMessageSetTipSeen>();
            if (!Verify.IsNotNull(setTipSeen)) return;

            Player.SetTipSeen((PrototypeId)setTipSeen.TipDataRefId);
        }

        #endregion
    }
}
