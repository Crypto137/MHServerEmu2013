using System.Globalization;
using MHServerEmu.Core.Config;
using MHServerEmu.Core.Helpers;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Logging.Targets;
using MHServerEmu.Core.Network;
using MHServerEmu.Frontend;
using MHServerEmu.Games;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.Network.InstanceManagement;
using MHServerEmu.Grouping;
using MHServerEmu.PlayerManagement;

namespace MHServerEmu
{
    public class ServerApp
    {
#if DEBUG
        public const string BuildConfiguration = "Debug";
#elif RELEASE
        public const string BuildConfiguration = "Release";
#endif

        private static readonly Logger Logger = LogManager.CreateLogger();

        private bool _isRunning = false;

        public static readonly string VersionInfo = $"Version {AssemblyHelper.GetAssemblyInformationalVersion()} | {AssemblyHelper.ParseAssemblyBuildTime():yyyy.MM.dd HH:mm:ss} UTC | {BuildConfiguration} | Game Version {Game.Version}";

        public static ServerApp Instance { get; } = new();

        public DateTime StartupTime { get; private set; }

        private ServerApp() { }

        public void Run()
        {
            if (_isRunning) return;
            _isRunning = true;

            StartupTime = DateTime.Now;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintBanner();
            PrintVersionInfo();
            Console.ResetColor();

            // Init loggers before anything else
            InitLoggers();

            Logger.Info("MHServerEmu starting...");

            // Our encoding is not going to work unless we are running on a little-endian system
            if (BitConverter.IsLittleEndian == false)
            {
                Logger.Fatal("This computer's architecture uses big-endian byte order, which is not compatible with MHServerEmu.");
                Console.ReadLine();
                return;
            }

            // Initialize everything else and start the servers
            if (InitSystems() == false)
            {
                Console.ReadLine();
                return;
            }

            // Create and register game services
            ServerManager serverManager = ServerManager.Instance;
            serverManager.Initialize();

            serverManager.RegisterGameService(new GameInstanceService(), GameServiceType.GameInstance);
            serverManager.RegisterGameService(new PlayerManagerService(), GameServiceType.PlayerManager);
            serverManager.RegisterGameService(new GroupingManagerService(), GameServiceType.GroupingManager);
            serverManager.RegisterGameService(new FrontendServer(), GameServiceType.Frontend);

            serverManager.RunServices();


            while (true)
            {
                string input = Console.ReadLine();
            }
        }

        /// <summary>
        /// Prints a fancy ASCII banner to console.
        /// </summary>
        private void PrintBanner()
        {
            Console.WriteLine(@"  __  __ _    _  _____                          ______                 ___   ___  __ ____  ");
            Console.WriteLine(@" |  \/  | |  | |/ ____|                        |  ____|               |__ \ / _ \/_ |___ \ ");
            Console.WriteLine(@" | \  / | |__| | (___   ___ _ ____   _____ _ __| |__   _ __ ___  _   _   ) | | | || | __) |");
            Console.WriteLine(@" | |\/| |  __  |\___ \ / _ \ '__\ \ / / _ \ '__|  __| | '_ ` _ \| | | | / /| | | || ||__ < ");
            Console.WriteLine(@" | |  | | |  | |____) |  __/ |   \ V /  __/ |  | |____| | | | | | |_| |/ /_| |_| || |___) |");
            Console.WriteLine(@" |_|  |_|_|  |_|_____/ \___|_|    \_/ \___|_|  |______|_| |_| |_|\__,_|____|\___/ |_|____/ ");
            Console.WriteLine();
        }

        /// <summary>
        /// Prints formatted version info to console.
        /// </summary>
        private void PrintVersionInfo()
        {
            Console.WriteLine($"\t{VersionInfo}");
            Console.WriteLine();
        }

        /// <summary>
        /// Handles unhandled exceptions.
        /// </summary>
        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            if (e.IsTerminating)
            {
                Logger.FatalException(ex, "MHServerEmu terminating because of unhandled exception.");
                ServerManager.Instance.ShutdownServices();
            }
            else
            {
                Logger.ErrorException(ex, "Caught unhandled exception.");
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Initializes log targets.
        /// </summary>
        private void InitLoggers()
        {
            var config = ConfigManager.Instance.GetConfig<LoggingConfig>();

            LogManager.Enabled = config.EnableLogging;

            // Attach console log target
            if (config.EnableConsole)
            {
                ConsoleTarget target = new(config.GetConsoleSettings());
                LogManager.AttachTarget(target);
            }

            // Attach file log target
            if (config.EnableFile)
            {
                FileTarget target = new(config.GetFileSettings(), $"MHServerEmu_{StartupTime.ToString(FileHelper.FileNameDateFormat)}", config.FileSplitOutput, false);
                LogManager.AttachTarget(target);
            }

            if (config.SynchronousMode)
                Logger.Debug($"Synchronous logging enabled");
        }

        /// <summary>
        /// Initializes systems needed to run the servers.
        /// </summary>
        private bool InitSystems()
        {
            return PakFileSystem.Instance.Initialize()
                && ProtocolDispatchTable.Instance.Initialize()
                && GameDatabase.IsInitialized;
        }
    }
}
