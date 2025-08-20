using Gazillion;
using Google.ProtocolBuffers;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Network;
using MHServerEmu.Frontend;

namespace MHServerEmu.PlayerManagement
{
    public class PlayerManagerService : IGameService
    {
        private const ushort MuxChannel = 1;

        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly HashSet<IFrontendClient> _clients = new();

        public GameServiceState State { get; private set; } = GameServiceState.Created;

        public PlayerManagerService()
        {
        }

        public void Run()
        {
            State = GameServiceState.Running;
        }

        public void Shutdown()
        {
            State = GameServiceState.Shutdown;
        }

        public void ReceiveServiceMessage<T>(in T message) where T : struct, IGameServiceMessage
        {
            switch (message)
            {
                case ServiceMessage.AddClient addClient:
                    _clients.Add(addClient.Client);
                    break;

                case ServiceMessage.RemoveClient removeClient:
                    _clients.Remove(removeClient.Client);
                    ServerManager.Instance.SendMessageToService(GameServiceType.GameInstance, removeClient);
                    break;

                case ServiceMessage.RouteMessageBuffer routeMessageBuffer:
                    switch ((ClientToGameServerMessage)routeMessageBuffer.MessageBuffer.MessageId)
                    {
                        case ClientToGameServerMessage.NetMessageReadyForGameJoin:
                            OnReadyForGameJoin(routeMessageBuffer.Client, routeMessageBuffer.MessageBuffer);
                            break;

                        default:
                            ServerManager.Instance.SendMessageToService(GameServiceType.GameInstance, routeMessageBuffer);
                            break;
                    }

                    break;

                case ServiceMessage.RouteMessage routeMessage:
                    if (routeMessage.Protocol == typeof(FrontendProtocolMessage) && routeMessage.Message.Id == (uint)FrontendProtocolMessage.ClientCredentials)
                    {
                        FrontendClient client = (FrontendClient)routeMessage.Client;
                        client.AssignSession(new ClientSession());

                        client.SendMessage(MuxChannel, SessionEncryptionChanged.CreateBuilder()
                            .SetRandomNumberIndex(0)
                            .SetEncryptedRandomNumber(ByteString.Empty)
                            .Build());
                    }

                    break;
            }
        }

        public string GetStatus()
        {
            return "Running";
        }

        private void OnReadyForGameJoin(IFrontendClient client, in MessageBuffer messageBuffer)
        {
            NetMessageReadyForGameJoin readyForGameJoin = messageBuffer.DeserializeReadyForGameJoin();
            Logger.Info($"NetMessageReadyForGameJoin\n{readyForGameJoin}");

            client.SendMessage(MuxChannel, NetMessageReadyAndLoggedIn.DefaultInstance);

            ServiceMessage.AddClient addClient = new(client);
            ServerManager.Instance.SendMessageToService(GameServiceType.GameInstance, addClient);
        }
    }
}
