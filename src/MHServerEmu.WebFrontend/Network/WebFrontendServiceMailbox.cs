using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Network;

namespace MHServerEmu.WebFrontend.Network
{
    internal sealed class WebFrontendServiceMailbox : ServiceMailbox
    {
        protected override void HandleServiceMessage(IGameServiceMessage message)
        {
            switch (message)
            {
                default:
                    Verify.IsTrue(false, $"Unhandled service message type {message.GetType().Name}");
                    break;
            }
        }
    }
}
