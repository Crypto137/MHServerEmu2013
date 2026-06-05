using Gazillion;
using Google.ProtocolBuffers;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.Network;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.Regions;

namespace MHServerEmu.Games.Network
{
    // This is the equivalent of the client-side ClientServiceConnectionManager and GameConnectionManager implementations of the NetworkManager abstract class.

    /// <summary>
    /// Manages <see cref="PlayerConnection"/> instances.
    /// </summary>
    public class PlayerConnectionManager : NetworkManager<PlayerConnection, ClientToGameServerMessage>
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly Game _game;

        // HashSet for preventing the same account from logging in multiple times
        private readonly HashSet<ulong> _playerDbIds = new();

        // Queue for pending player connections (i.e. players currently loading)
        private readonly Queue<PlayerConnection> _pendingPlayerConnectionQueue = new(); // REMOVEME

        /// <summary>
        /// Constructs a new <see cref="PlayerConnectionManager"/> instance for the provided <see cref="Game"/>.
        /// </summary>
        public PlayerConnectionManager(Game game)
        {
            _game = game;
        }

        #region Player Connection Getters

        /// <summary>
        /// Adds all <see cref="Player"/> instances that are interested in the provided <see cref="Entity"/> to the provided <see cref="List{T}"/>.
        /// Returns <see langword="true"/> if the number of interested players is > 0.
        /// </summary>
        public bool GetInterestedPlayers(List<Player> interestedPlayerList, Entity entity,
            AOINetworkPolicyValues interestFilter = AOINetworkPolicyValues.AllChannels, bool skipOwner = false)
        {
            // Early out if we already know that none of the players match the interest channel filter
            if ((entity.InterestedPoliciesUnion & interestFilter) == 0)
                return false;

            // Use InterestReferences to skip players that we know for sure are not interested in this entity
            EntityManager entityManager = _game.EntityManager;
            foreach (ulong playerId in entity.InterestReferences)
            {
                Player player = entityManager.GetEntity<Player>(playerId);
                if (!Verify.IsNotNull(player))
                    continue;

                if (!Verify.IsNotNull(player.PlayerConnection))
                    continue;

                // Check ownership
                if (skipOwner && entity.IsOwnedBy(playerId))
                    continue;

                // Check channel filter
                if (player.AOI.InterestedInEntity(entity.Id, interestFilter) == false)
                    continue;

                interestedPlayerList.Add(player);
            }

            return interestedPlayerList.Count > 0;
        }

        /// <summary>
        /// Adds all <see cref="Player"/> instances that are interested in the provided <see cref="Region"/> to the provided <see cref="List{T}"/>.
        /// Returns <see langword="true"/> if the number of interested players is > 0.
        /// </summary>
        public bool GetInterestedPlayers(List<Player> interestedPlayerList, Region region)
        {
            foreach (Player player in new PlayerIterator(region))
            {
                if (player.AOI.Region == region)
                    interestedPlayerList.Add(player);
            }

            return interestedPlayerList.Count > 0;
        }

        /// <summary>
        /// Adds all <see cref="PlayerConnection"/> instances that are interested in the provided <see cref="Entity"/> to the provided <see cref="List{T}"/>.
        /// Returns <see langword="true"/> if the number of interested player connections is > 0.
        /// </summary>
        public bool GetInterestedClients(List<PlayerConnection> interestedClientList, Entity entity,
            AOINetworkPolicyValues interestFilter = AOINetworkPolicyValues.AllChannels, bool skipOwner = false)
        {
            using var interestedPlayerListHandle = ListPool<Player>.Instance.Get(out List<Player> interestedPlayerList);
            GetInterestedPlayers(interestedPlayerList, entity, interestFilter, skipOwner);

            foreach (Player player in interestedPlayerList)
                interestedClientList.Add(player.PlayerConnection);

            return interestedClientList.Count > 0;
        }

        /// <summary>
        /// Returns <see cref="PlayerConnection"/> instances that are bound to players that are interested in the provided <see cref="Region"/>.
        /// </summary>
        /// <summary>
        /// Adds all <see cref="PlayerConnection"/> instances that are interested in the provided <see cref="Region"/> to the provided <see cref="List{T}"/>.
        /// Returns <see langword="true"/> if the number of interested clients is > 0.
        /// </summary>
        public bool GetInterestedClients(List<PlayerConnection> interestedClientList, Region region)
        {
            using var interestedPlayerListHandle = ListPool<Player>.Instance.Get(out List<Player> interestedPlayerList);
            GetInterestedPlayers(interestedPlayerList, region);

            foreach (Player player in interestedPlayerList)
                interestedClientList.Add(player.PlayerConnection);

            return interestedClientList.Count > 0;
        }

        #endregion

        #region Pending Processing (REMOVEME)

        /// <summary>
        /// Loads pending players.
        /// </summary>
        public void ProcessPendingPlayerConnections()
        {
            while (_pendingPlayerConnectionQueue.Count > 0)
            {
                PlayerConnection playerConnection = _pendingPlayerConnectionQueue.Dequeue();
                playerConnection.EnterGame();
            }
        }

        /// <summary>
        /// Requests a player to be loaded.
        /// </summary>
        public void SetPlayerConnectionPending(PlayerConnection playerConnection)
        {
            // NOTE: We flush messages when we set the connection as pending so that
            // we can deliver the loading screen message to the client ASAP.
            playerConnection.FlushMessages();
            _pendingPlayerConnectionQueue.Enqueue(playerConnection);
        }

        #endregion

        #region Message Sending

        /// <summary>
        /// Sends the provided <see cref="IMessage"/> over all <see cref="PlayerConnection"/> instaces in the provided <see cref="List{T}"/>.
        /// </summary>
        public void SendMessageToMultiple(List<PlayerConnection> clientList, IMessage message)
        {
            foreach (PlayerConnection playerConnection in clientList)
                playerConnection.SendMessage(message);
        }

        /// <summary>
        /// Sends the provided <see cref="IMessage"/> to all <see cref="PlayerConnection"/> instances that are interested in the provided <see cref="Region"/>.
        /// </summary>
        public void SendMessageToInterested(IMessage message, Region region)
        {
            using var interestedClientListHandle = ListPool<PlayerConnection>.Instance.Get(out List<PlayerConnection> interestedClientList);
            GetInterestedClients(interestedClientList, region);

            foreach (PlayerConnection playerConnection in interestedClientList)
                playerConnection.SendMessage(message);
        }

        /// <summary>
        /// Sends the provided <see cref="IMessage"/> to all <see cref="PlayerConnection"/> instances that are interested in the provided <see cref="Entity"/>.
        /// </summary>
        public void SendMessageToInterested(IMessage message, Entity entity, AOINetworkPolicyValues interestFilter = AOINetworkPolicyValues.AllChannels, bool skipOwner = false)
        {
            using var interestedClientListHandle = ListPool<PlayerConnection>.Instance.Get(out List<PlayerConnection> interestedClientList);
            GetInterestedClients(interestedClientList, entity, interestFilter, skipOwner);

            foreach (PlayerConnection playerConnection in interestedClientList)
                playerConnection.SendMessage(message);
        }

        /// <summary>
        /// Broadcasts an <see cref="IMessage"/> instance to all active <see cref="PlayerConnection"/> instances.
        /// </summary>
        public void BroadcastMessage(IMessage message)
        {
            foreach (PlayerConnection playerConnection in this)
                playerConnection.PostMessage(message);
        }

        /// <summary>
        /// Posts the provided <see cref="IMessage"/> to the specified <see cref="PlayerConnection"/> and immediately flushes it.
        /// </summary>
        public void SendMessageImmediate(PlayerConnection playerConnection, IMessage message)
        {
            playerConnection.PostMessage(message);
            playerConnection.FlushMessages();
        }

        #endregion

        protected override PlayerConnection AcceptAndRegisterNewClient(IFrontendClient frontendClient)
        {
            // Make sure this client is still connected (it may not be if we are lagging hard)
            if (!Verify.IsTrue(frontendClient.IsConnected, $"Client [{frontendClient}] is no longer connected"))
            {
                // Self-initiate a removal request (V10_TODO)
                //_game.GameManager.RemoveClientFromGame(frontendClient, _game.Id, true);
                //_game.GameManager.OnClientRemoved(_game, frontendClient);
                return null;
            }

            // Construct a new PlayerConnection bound to this IFrontendClient
            PlayerConnection playerConnection = new(_game, frontendClient);

            // V10_TODO: dbids

            // Register the client to allow it to receive messages
            Verify.IsTrue(RegisterNetClient(playerConnection), LoggingLevel.Error, $"Failed to add client [{frontendClient}]");

            // V10_NOTE: 1.10 uses ReadyForTimeSync instead of InitialTimeSync
            SendMessageImmediate(playerConnection, NetMessageReadyForTimeSync.DefaultInstance);

            // Initializing a player connection loads player data and sends the achievement database if needed
            // V10_NOTE: No achievements in 1.10
            if (playerConnection.Initialize() == false)
                return null;

            // This connection will be set as pending when we receive region availability query response
            // V10_TODO: GameManager

            Logger.Info($"Accepted and registered client [{frontendClient}] to game [{_game}]");
            return playerConnection;
        }

        protected override void OnNetClientDisconnected(PlayerConnection playerConnection)
        {
            // V10_TODO: dbids
        }
    }
}
