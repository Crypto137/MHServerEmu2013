using MHServerEmu.Core.Network;

namespace MHServerEmu.Games.Network.InstanceManagement
{
    public class GameInstanceService : IGameService
    {
        private Game _game;

        public GameServiceState State { get; private set; } = GameServiceState.Created;

        public GameInstanceService()
        {

        }

        public void Run()
        {
            // TODO: GameManager
            _game = new(1);
            _game.Run();

            State = GameServiceState.Running;
        }

        public void Shutdown()
        {
            State = GameServiceState.Shutdown;
        }

        public string GetStatus()
        {
            return "Running";
        }

        public void ReceiveServiceMessage<T>(in T message) where T : struct, IGameServiceMessage
        {
            switch (message)
            {
                case GameServiceProtocol.AddClient addClient:
                    _game.AddClient(addClient.Client);
                    break;

                case GameServiceProtocol.RemoveClient removeClient:
                    _game.RemoveClient(removeClient.Client);
                    break;

                case GameServiceProtocol.RouteMessageBuffer routeMessageBuffer:
                    _game.ReceiveMessageBuffer(routeMessageBuffer.Client, routeMessageBuffer.MessageBuffer);
                    break;

            }
        }
    }
}
