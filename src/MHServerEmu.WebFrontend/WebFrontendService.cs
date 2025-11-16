using MHServerEmu.Core.Config;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Network;
using MHServerEmu.Core.Network.Web;
using MHServerEmu.WebFrontend.Handlers;
using MHServerEmu.WebFrontend.Handlers.WebApi;
using MHServerEmu.WebFrontend.Network;

namespace MHServerEmu.WebFrontend
{
    /// <summary>
    /// Handles HTTP requests from clients.
    /// </summary>
    public class WebFrontendService : IGameService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly WebFrontendServiceMailbox _serviceMailbox = new();

        private readonly WebService _webService;

        public GameServiceState State { get; private set; } = GameServiceState.Created;

        public WebFrontendService()
        {
            var config = ConfigManager.Instance.GetConfig<WebFrontendConfig>();

            WebServiceSettings webServiceSettings = new()
            {
                Name = "WebFrontend",
                ListenUrl = $"http://{config.Address}:{config.Port}/",
                FallbackHandler = new NotFoundWebHandler(),
            };

            _webService = new(webServiceSettings);

            // Register the protobuf handler to the /Login/IndexPB path for compatibility with legacy reverse proxy setups.
            // We should probably prefer to use /AuthServer/Login/IndexPB because it's more accurate to what Gazillion had.
            ProtobufWebHandler protobufHandler = new(config.EnableLoginRateLimit, TimeSpan.FromMilliseconds(config.LoginRateLimitCostMS), config.LoginRateLimitBurst);
            _webService.RegisterHandler("/Login/IndexPB", protobufHandler);
            _webService.RegisterHandler("/AuthServer/Login/IndexPB", protobufHandler);

            // V10_TODO: MTXStore

            if (config.EnableWebApi)
            {
                InitializeWebBackend();

                // V10_TODO: Dashboard
            }
        }

        #region IGameService Implementation

        /// <summary>
        /// Runs this <see cref="WebFrontendService"/> instance.
        /// </summary>
        public void Run()
        {
            _webService.Start();
            State = GameServiceState.Running;

            while (_webService.IsRunning)
            {
                _serviceMailbox.ProcessMessages();
                Thread.Sleep(1);
            }

            State = GameServiceState.Shutdown;
        }

        /// <summary>
        /// Stops listening and shuts down this <see cref="WebFrontendService"/> instance.
        /// </summary>
        public void Shutdown()
        {
            _webService.Stop();
        }

        public void ReceiveServiceMessage<T>(in T message) where T : struct, IGameServiceMessage
        {
            _serviceMailbox.PostMessage(message);
        }

        public void GetStatus(Dictionary<string, long> statusDict)
        {
            statusDict["WebFrontendHandlers"] = _webService.HandlerCount;
            statusDict["WebFrontendHandledRequests"] = _webService.HandledRequests;
        }

        #endregion

        private void InitializeWebBackend()
        {
            _webService.RegisterHandler("/ServerStatus", new ServerStatusWebHandler());
        }
    }
}
