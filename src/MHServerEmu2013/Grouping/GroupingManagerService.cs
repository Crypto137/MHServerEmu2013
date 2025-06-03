using MHServerEmu.Core.Network;

namespace MHServerEmu.Grouping
{
    public class GroupingManagerService : IGameService
    {
        // Just a dummy for now

        public GroupingManagerService()
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
            
        }

        public string GetStatus()
        {
            return "Running";
        }
    }
}
