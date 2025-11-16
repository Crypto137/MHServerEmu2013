using MHServerEmu.Core.Config;
using MHServerEmu.Core.Network;

namespace MHServerEmu.Games.Network.InstanceManagement
{
    public class GameInstanceService : IGameService
    {
        private Game _game;

        internal GameThreadManager GameThreadManager { get; }

        public GameInstanceConfig Config { get; }

        public GameServiceState State { get; private set; } = GameServiceState.Created;

        public GameInstanceService()
        {
            GameThreadManager = new(this);

            Config = ConfigManager.Instance.GetConfig<GameInstanceConfig>();
        }

        public void Run()
        {
            GameThreadManager.Initialize();

            // TODO: Backport GameManager
            _game = new(1);
            GameThreadManager.EnqueueGameToUpdate(_game);

            State = GameServiceState.Running;
        }

        public void Shutdown()
        {
            State = GameServiceState.Shutdown;
        }

        public void GetStatus(Dictionary<string, long> statusDict)
        {

        }

        public void ReceiveServiceMessage<T>(in T message) where T : struct, IGameServiceMessage
        {
            switch (message)
            {
                case ServiceMessage.AddClient addClient:
                    _game.AddClient(addClient.Client);
                    break;

                case ServiceMessage.RemoveClient removeClient:
                    _game.RemoveClient(removeClient.Client);
                    break;

                case ServiceMessage.RouteMessageBuffer routeMessageBuffer:
                    _game.ReceiveMessageBuffer(routeMessageBuffer.Client, routeMessageBuffer.MessageBuffer);
                    break;

            }
        }
    }
}
