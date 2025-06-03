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

        public PlayerManagerService()
        {
        }

        public void Run()
        {

        }

        public void Shutdown()
        {

        }

        public void ReceiveServiceMessage<T>(in T message) where T : struct, IGameServiceMessage
        {
            switch (message)
            {
                case GameServiceProtocol.AddClient addClient:
                    // HACK: Add to a game with a delay to give the client time to connect to the grouping service
                    Task.Run(async () => {
                        await Task.Delay(50);
                        ServerManager.Instance.SendMessageToService(ServerType.GameInstanceServer, addClient);
                    });
                    break;

                case GameServiceProtocol.RemoveClient removeClient:
                    ServerManager.Instance.SendMessageToService(ServerType.GameInstanceServer, removeClient);
                    break;

                case GameServiceProtocol.RouteMessageBuffer routeMessageBuffer:
                    switch ((ClientToGameServerMessage)routeMessageBuffer.MessageBuffer.MessageId)
                    {
                        case ClientToGameServerMessage.NetMessageReadyForGameJoin:
                            OnReadyForGameJoin(routeMessageBuffer.Client, routeMessageBuffer.MessageBuffer);
                            break;

                        default:
                            ServerManager.Instance.SendMessageToService(ServerType.GameInstanceServer, routeMessageBuffer);
                            break;
                    }

                    break;

                case GameServiceProtocol.RouteMessage routeMessage:
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
            Logger.Info($"Received NetMessageReadyForGameJoin");
            Logger.Trace(readyForGameJoin.ToString());

            client.SendMessage(MuxChannel, NetMessageReadyAndLoggedIn.DefaultInstance);
        }
    }
}
