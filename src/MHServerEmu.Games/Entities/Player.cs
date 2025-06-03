using Gazillion;
using Google.ProtocolBuffers;
using MHServerEmu.Core.Collections;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Core.System.Time;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.Entities.Avatars;
using MHServerEmu.Games.Entities.Inventories;
using MHServerEmu.Games.Entities.Items;
using MHServerEmu.Games.Entities.Options;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Missions;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Properties;
using MHServerEmu.Games.Regions;
using MHServerEmu.Games.Social.Guilds;

namespace MHServerEmu.Games.Entities
{
    public enum AvailableBadges
    {
        CanGrantBadges = 1,         // User can grant badges to other users
        SiteCommands,               // User can run the site commands (player/regions lists, change to specific region etc)
        CanBroadcastChat,           // User can send a chat message to all players
        AllContentAccess,           // User has access to all content in the game
        CanLogInAsAnotherAccount,   // User has ability to log in as another account
        CanDisablePersistence,      // User has ability to play without saving
        PlaytestCommands,           // User can always use commands that are normally only available during a playtest (e.g. bug)
        NumberOfBadges
    }

    public class Player : Entity, IMissionManagerOwner
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private ulong _guildId;
        private string _guildName;
        private GuildMembership _guildMembership;

        private List<PrototypeId> _unlockedInventoryList = new();
        private SortedVector<AvailableBadges> _badges = new();

        private TeleportData _teleportData;

        public PlayerConnection PlayerConnection { get; private set; }
        public AreaOfInterest AOI { get => PlayerConnection.AOI; }

        public MissionManager MissionManager { get; private set; }
        public ReplicatedPropertyCollection AvatarProperties { get; private set; } = new();
        public RepVar_string PlayerName { get; private set; } = new();
        public RepVar_ulong PartyId { get; private set; } = new();
        public GameplayOptions GameplayOptions { get; private set; } = new();

        public bool IsOnLoadingScreen { get; private set; }

        public Avatar CurrentAvatar { get; private set; }

        public Player(Game game) : base(game)
        {
            MissionManager = new(Game, this);
        }

        public override bool Initialize(EntitySettings settings)
        {
            base.Initialize(settings);

            PlayerConnection = settings.PlayerConnection;
            PlayerName.Set(settings.PlayerName);

            Game.EntityManager.AddPlayer(this);

            // V10_NOTE: Showing a loading screen this early in the process causes a picker verify fail in the client
            // (probably due to it now having any valid loading screen tips to display at this point).

            foreach (PrototypeId waypointProtoRef in DataDirectory.Instance.IteratePrototypesInHierarchy<WaypointPrototype>(PrototypeIterateFlags.NoAbstractApprovedOnly))
                Properties[PropertyEnum.Waypoint, waypointProtoRef] = true;

            foreach (PrototypeId vendorTypeProtoRef in DataDirectory.Instance.IteratePrototypesInHierarchy<VendorTypePrototype>(PrototypeIterateFlags.NoAbstractApprovedOnly))
                Properties[PropertyEnum.VendorLevel, vendorTypeProtoRef] = 1;

            foreach (PrototypeId chapterProtoRef in DataDirectory.Instance.IteratePrototypesInHierarchy<ChapterPrototype>(PrototypeIterateFlags.NoAbstractApprovedOnly))
                Properties[PropertyEnum.ChapterUnlocked, chapterProtoRef] = true;

            for (AvailableBadges badge = AvailableBadges.CanGrantBadges; badge < AvailableBadges.NumberOfBadges; badge++)
                _badges.SortedInsert(badge);

            // V10_REMOVEME: Init missions here until we have mission update sending working
            MissionManager.InitializeForPlayer(this, null);

            return true;
        }

        public override void OnUnpackComplete(Archive archive)
        {
            base.OnUnpackComplete(archive);

            foreach (PrototypeId invProtoRef in _unlockedInventoryList)
            {
                PlayerStashInventoryPrototype stashInvProto = GameDatabase.GetPrototype<PlayerStashInventoryPrototype>(invProtoRef);
                if (stashInvProto == null)
                {
                    Logger.Warn("OnUnpackComplete(): stashInvProto == null");
                    continue;
                }

                // V10_NOTE: No unified stash in 1.10

                Inventory inventory = GetInventoryByRef(invProtoRef);
                if (inventory == null && AddInventory(invProtoRef) == false)
                    Logger.Warn($"OnUnpackComplete(): Failed to add inventory, invProtoRef={invProtoRef.GetName()}");
            }
        }

        protected override void BindReplicatedFields()
        {
            base.BindReplicatedFields();

            AvatarProperties.Bind(this, AOINetworkPolicyValues.AOIChannelOwner);
            PlayerName.Bind(this, AOINetworkPolicyValues.AOIChannelParty | AOINetworkPolicyValues.AOIChannelOwner);
            PartyId.Bind(this, AOINetworkPolicyValues.AOIChannelParty | AOINetworkPolicyValues.AOIChannelOwner);
        }

        protected override void UnbindReplicatedFields()
        {
            base.UnbindReplicatedFields();

            AvatarProperties.Unbind();
            PlayerName.Unbind();
            PartyId.Unbind();
        }

        public override bool Serialize(Archive archive)
        {
            bool success = base.Serialize(archive);

            success &= Serializer.Transfer(archive, MissionManager);
            success &= Serializer.Transfer(archive, AvatarProperties);

            if (archive.IsTransient)
            {
                ulong shardId = 0;
                success &= Serializer.Transfer(archive, ref shardId);

                success &= Serializer.Transfer(archive, PlayerName);

                if (archive.IsReplication)
                {
                    success &= Serializer.Transfer(archive, PartyId);
                    success &= GuildMember.SerializeReplicationRuntimeInfo(archive, ref _guildId, ref _guildName, ref _guildMembership);

                    // Unused string, always empty
                    string emptyString = string.Empty;
                    success &= Serializer.Transfer(archive, ref emptyString);
                }
            }

            bool hasCommunityData = false;
            success &= Serializer.Transfer(archive, ref hasCommunityData);
            // V10_TODO: Communities

            // Unused bool, always false
            bool unkBool = false;
            success &= Serializer.Transfer(archive, ref unkBool);

            success &= Serializer.Transfer(archive, ref _unlockedInventoryList);

            success &= Serializer.Transfer(archive, ref _badges);

            success &= Serializer.Transfer(archive, GameplayOptions);

            return success;
        }

        public override void EnterGame(EntitySettings settings = null)
        {
            SendMessage(NetMessageMarkFirstGameFrame.CreateBuilder()
                .SetCurrentservergametime((ulong)Clock.GameTime.TotalMilliseconds)
                .SetCurrentservergameid(Game.Id)
                .Build());

            SendMessage(NetMessageServerVersion.CreateBuilder().SetVersion(Game.Version).Build());

            SendMessage(NetMessageLocalPlayer.CreateBuilder().SetLocalPlayerEntityId(Id).Build());

            SendMessage(NetMessageReadyForTimeSync.DefaultInstance);

            base.EnterGame(settings);
        }

        public override void ExitGame()
        {
            // V10_NOTE: No trade in 1.10

            SendMessage(NetMessageBeginExitGame.DefaultInstance);
            AOI.SetRegion(0, true);

            base.ExitGame();
        }

        public Region GetRegion()
        {
            // This shouldn't need any null checks, at least for now
            return AOI.Region;
        }

        #region Inventories

        public override void OnOtherEntityAddedToMyInventory(Entity entity, InventoryLocation invLoc, bool unpackedArchivedEntity)
        {
            base.OnOtherEntityAddedToMyInventory(entity, invLoc, unpackedArchivedEntity);

            if (entity is Avatar avatar)
            {
                avatar.SetPlayer(this);

                if (invLoc.InventoryConvenienceLabel == InventoryConvenienceLabel.AvatarInPlay && invLoc.Slot == 0)
                    CurrentAvatar = avatar;
            }
        }

        protected override bool InitInventories(bool populateInventories)
        {
            bool success = base.InitInventories(populateInventories);

            PlayerPrototype playerProto = Prototype as PlayerPrototype;
            if (playerProto == null) return Logger.WarnReturn(false, "InitInventories(): playerProto == null");

            foreach (EntityInventoryAssignmentPrototype invEntryProto in playerProto.StashInventories)
            {
                var stashInvProto = invEntryProto.Inventory.As<PlayerStashInventoryPrototype>();
                if (stashInvProto == null)
                {
                    Logger.Warn("InitInventories(): stashInvProto == null");
                    continue;
                }

                // V10_NOTE: No unified stash in 1.10

                if (stashInvProto.LockedByDefault == false)
                {
                    if (AddInventory(invEntryProto.Inventory) == false)
                    {
                        Logger.Warn($"InitInventories(): Failed to add inventory, invProtoRef={GameDatabase.GetPrototypeName(invEntryProto.Inventory)}");
                        success = false;
                    }
                }
            }

            return success;
        }

        public bool TrashItem(Item item)
        {
            // V10_TODO: Dropping items
            item.Destroy();
            return true;
        }

        #endregion

        #region Avatar Management

        public Avatar GetAvatar(PrototypeId avatarProtoRef)
        {
            if (avatarProtoRef == PrototypeId.Invalid) return Logger.WarnReturn<Avatar>(null, "GetAvatar(): avatarProtoRef == PrototypeId.Invalid");

            AvatarIterator iterator = new(this, AvatarIteratorMode.IncludeArchived, avatarProtoRef);
            AvatarIterator.Enumerator enumerator = iterator.GetEnumerator();
            if (enumerator.MoveNext())
                return enumerator.Current;

            return null;
        }

        public bool CreateAvatar(PrototypeId avatarProtoRef)
        {
            Avatar avatar = GetAvatar(avatarProtoRef);
            if (avatar != null)
                return Logger.WarnReturn(false, $"CreateAvatar(): avatarProtoRef {avatarProtoRef.GetName()} already exists for player {Id}");

            Inventory avatarLibrary = GetInventory(InventoryConvenienceLabel.AvatarLibrary);
            if (avatarLibrary == null) return Logger.WarnReturn(false, "CreateAvatar(): avatarLibrary == null");

            using EntitySettings settings = ObjectPoolManager.Instance.Get<EntitySettings>();
            settings.EntityRef = avatarProtoRef;
            settings.InventoryLocation = new(Id, avatarLibrary.PrototypeDataRef);

            if (PlayerConnection.MigrationData.TryGetArchivedAvatar(avatarProtoRef, out byte[] archiveData))
            {
                settings.ArchiveSerializeType = ArchiveSerializeType.Database;
                settings.ArchiveData = archiveData;
            }

            return Game.EntityManager.CreateEntity(settings) != null;
        }

        public bool SwitchAvatar(PrototypeId avatarProtoRef)
        {
            // V10_TODO: Clean this up
            Avatar currentAvatar = CurrentAvatar;

            Avatar avatarToSwitchTo = GetAvatar(avatarProtoRef);
            if (avatarToSwitchTo == null) return Logger.WarnReturn(false, "SwitchAvatar(): avatarToSwitchTo == null");

            Inventory avatarInPlay = GetInventory(InventoryConvenienceLabel.AvatarInPlay);
            if (avatarInPlay == null) return Logger.WarnReturn(false, "OnSwitchAvatar(): avatarInPlay == null");

            Region region = default;
            Vector3 position = default;
            Orientation orientation = default;
            if (currentAvatar != null)
            {
                region = currentAvatar.Region;
                position = currentAvatar.RegionLocation.Position;
                orientation = currentAvatar.RegionLocation.Orientation;
            }

            avatarToSwitchTo.ChangeInventoryLocation(avatarInPlay, 0);

            if (currentAvatar != null)
            {
                using EntitySettings settings = ObjectPoolManager.Instance.Get<EntitySettings>();
                settings.OptionFlags |= EntitySettingsOptionFlags.IsClientEntityHidden;

                avatarToSwitchTo.EnterWorld(region, position, orientation, settings);

                var swapInPowerMessage = NetMessageActivatePower.CreateBuilder()
                    .SetIdUserEntity(avatarToSwitchTo.Id)
                    .SetPowerPrototypeId((ulong)GameDatabase.GlobalsPrototype.AvatarSwapInPower)
                    .SetParentPowerPrototypeId(0)
                    .SetIdTargetEntity(avatarToSwitchTo.Id)
                    .SetTargetPosition(avatarToSwitchTo.RegionLocation.Position.ToNetStructPoint3())
                    .SetUserPosition(avatarToSwitchTo.RegionLocation.Position.ToNetStructPoint3())
                    .Build();

                Game.NetworkManager.SendMessageToInterested(swapInPowerMessage, avatarToSwitchTo, AOINetworkPolicyValues.AOIChannelProximity);
            }

            return true;
        }

        public bool EnableCurrentAvatar(bool withSwapInPower, ulong lastCurrentAvatarId, ulong regionId, in Vector3 position, in Orientation orientation)
        {
            if (CurrentAvatar == null)
                return Logger.WarnReturn(false, "EnableCurrentAvatar(): CurrentAvatar == null");

            if (CurrentAvatar.IsInWorld)
                return Logger.WarnReturn(false, "EnableCurrentAvatar(): Current avatar is already active");

            Region region = Game.RegionManager.GetRegion(regionId);
            if (region == null)
                return Logger.WarnReturn(false, "EnableCurrentAvtar(): region == null");

            Logger.Trace($"EnableCurrentAvatar(): [{CurrentAvatar}] entering world in region [{region}]");

            using EntitySettings settings = ObjectPoolManager.Instance.Get<EntitySettings>();

            // V10_TODO: More stuff

            // Add new avatar to the world
            if (CurrentAvatar.EnterWorld(region, CurrentAvatar.FloorToCenter(position), orientation, settings) == false)
                return false;

            return true;
        }

        #endregion

        #region Loading and Teleports

        public void QueueLoadingScreen(PrototypeId regionProtoRef)
        {
            IsOnLoadingScreen = true;

            SendMessage(NetMessageQueueLoadingScreen.CreateBuilder()
                .SetRegionPrototypeId((ulong)regionProtoRef)
                .Build());
        }

        public void QueueLoadingScreen(ulong regionId)
        {
            Region region = Game.RegionManager.GetRegion(regionId);
            PrototypeId regionProtoRef = region != null ? region.PrototypeDataRef : PrototypeId.Invalid;
            QueueLoadingScreen(regionProtoRef);
        }

        public void DequeueLoadingScreen()
        {
            SendMessage(NetMessageDequeueLoadingScreen.DefaultInstance);
        }

        public void OnLoadingScreenFinished()
        {
            if (IsOnLoadingScreen)
            {
                IsOnLoadingScreen = false;
            }
        }

        public void BeginTeleport(ulong regionId, in Vector3 position, in Orientation orientation)
        {
            _teleportData.Set(regionId, position, orientation);
            QueueLoadingScreen(regionId);
        }

        public void OnCellLoaded(uint cellId, ulong regionId)
        {
            AOI.OnCellLoaded(cellId, regionId);
            int numLoaded = AOI.GetLoadedCellCount();

            //Logger.Trace($"Player {this} loaded cell id={cellId} in region id=0x{regionId:X} ({numLoaded}/{AOI.TrackedCellCount})");

            if (_teleportData.IsValid && numLoaded == AOI.TrackedCellCount)
                FinishTeleport();
        }

        private bool FinishTeleport()
        {
            if (_teleportData.IsValid == false) return Logger.WarnReturn(false, "FinishTeleport(): No valid teleport data");

            EnableCurrentAvatar(false, CurrentAvatar.Id, _teleportData.RegionId, _teleportData.Position, _teleportData.Orientation);
            _teleportData.Clear();
            DequeueLoadingScreen();
            //TryPlayKismetSequences();

            return true;
        }

        #endregion

        #region AOI & Discovery

        public bool InterestedInEntity(Entity entity, AOINetworkPolicyValues interestFilter)
        {
            if (entity == null) return Logger.WarnReturn(false, "InterestedInEntity(): entity == null");

            if (entity.InterestReferences.IsPlayerInterested(this) == false)
                return false;

            return AOI.InterestedInEntity(entity.Id, interestFilter);
        }

        #endregion

        #region Missions and Chapters

        public void MissionInteractRelease(WorldEntity entity, PrototypeId missionRef)
        {
            if (missionRef == PrototypeId.Invalid) return;
            if (InterestedInEntity(entity, AOINetworkPolicyValues.AOIChannelOwner))
                SendMessage(NetMessageMissionInteractRelease.DefaultInstance);
        }

        #endregion

        #region SendMessage

        public void SendMessage(IMessage message)
        {
            PlayerConnection?.SendMessage(message);
        }

        #endregion

        #region Tutorial

        public void SetTipSeen(PrototypeId tipDataRef)
        {
            if (tipDataRef == PrototypeId.Invalid) return;
            Properties[PropertyEnum.TutorialHasSeenTip, tipDataRef] = true;
        }

        #endregion

        private struct TeleportData
        {
            public ulong RegionId { get; private set; }
            public Vector3 Position { get; private set; }
            public Orientation Orientation { get; private set; }

            public bool IsValid { get => RegionId != 0; }

            public void Set(ulong regionId, in Vector3 position, in Orientation orientation)
            {
                RegionId = regionId;
                Position = position;
                Orientation = orientation;
            }

            public void Clear()
            {
                RegionId = 0;
                Position = Vector3.Zero;
                Orientation = Orientation.Zero;
            }
        }
    }
}
