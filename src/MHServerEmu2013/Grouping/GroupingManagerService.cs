using MHServerEmu.Core.Network;

namespace MHServerEmu.Grouping
{
    public class GroupingManagerService : IGameService
    {
        // Just a dummy for now

        public GameServiceState State { get; private set; } = GameServiceState.Created;

        public GroupingManagerService()
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
            
        }

        public string GetStatus()
        {
            return "Running";
        }
    }
}
