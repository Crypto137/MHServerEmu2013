using MHServerEmu.Core.Network;

namespace MHServerEmu.Games
{
    public class GameInstanceService : IGameService
    {
        private Game _game;

        public GameInstanceService()
        {

        }

        public void Run()
        {
            // TODO: GameManager
            _game = new(1);
            _game.Run();
        }

        public void Shutdown()
        {
            
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
