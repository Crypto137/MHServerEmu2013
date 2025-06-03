﻿using System.Diagnostics;
using System.Globalization;
using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.Helpers;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.Network;
using MHServerEmu.Core.System.Random;
using MHServerEmu.Core.System.Time;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.Entities.Avatars;
using MHServerEmu.Games.Entities.Items;
using MHServerEmu.Games.Events;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.GameData.Prototypes.MetaGame;
using MHServerEmu.Games.MetaGames;
using MHServerEmu.Games.MTXStore;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Regions;

namespace MHServerEmu.Games
{
    public enum GameShutdownReason
    {
        ServerShuttingDown,
        GameInstanceCrash
    }

    public class Game
    {
#if BUILD_1_10_0_69
        public const string Version = "1.10.0.69";
#else
        public const string Version = "1.10.0.643";
#endif

        [ThreadStatic]
        internal static Game Current;

        private const int TargetFrameRate = 30;
        public static readonly TimeSpan StartTime = TimeSpan.FromMilliseconds(1);

        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly Stopwatch _gameTimer = new();
        private FixedQuantumGameTime _realGameTime = new(TimeSpan.FromMilliseconds(1));
        private TimeSpan _currentGameTime = TimeSpan.FromMilliseconds(1);   // Current time in the game simulation
        private TimeSpan _lastFixedTimeUpdateProcessTime;                   // How long the last fixed update took
        private TimeSpan _fixedTimeUpdateProcessTimeLogThreshold;
        private long _frameCount;

        private Thread _gameThread;

        private ulong _currentRepId;

        public ulong Id { get; }
        public bool IsRunning { get; private set; } = false;
        public bool HasBeenShutDown { get; private set; } = false;

        public GRandom Random { get; } = new();
        public PlayerConnectionManager NetworkManager { get; }
        public EventScheduler GameEventScheduler { get; private set; }
        public RegionManager RegionManager { get; }
        public EntityManager EntityManager { get; }

        public CatalogManager CatalogManager { get; } = new();

        public TimeSpan FixedTimeBetweenUpdates { get; } = TimeSpan.FromMilliseconds(1000f / TargetFrameRate);
        public TimeSpan RealGameTime { get => (TimeSpan)_realGameTime; }
        public TimeSpan CurrentTime { get => GameEventScheduler != null ? GameEventScheduler.CurrentTime : _currentGameTime; }
        public ulong NumQuantumFixedTimeUpdates { get => (ulong)CurrentTime.CalcNumTimeQuantums(FixedTimeBetweenUpdates); }

        public ulong CurrentRepId { get => ++_currentRepId; }
        public Dictionary<ulong, IArchiveMessageHandler> MessageHandlerDict { get; } = new();

        public Game(ulong id)
        {
            // Small lags are fine, and logging all of them creates too much noise
            _fixedTimeUpdateProcessTimeLogThreshold = FixedTimeBetweenUpdates * 2;

            Id = id;

            // The game uses 16 bits of the current UTC time in seconds as the initial replication id
            _currentRepId = (ulong)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond) & 0xFFFF;

            NetworkManager = new(this);
            RegionManager = new(this);
            EntityManager = new(this);

            Initialize();
        }

        public override string ToString()
        {
            return $"id=0x{Id:X}";
        }

        public bool Initialize()
        {
            bool success = true;

            _realGameTime.SetQuantumSize(FixedTimeBetweenUpdates);
            _realGameTime.UpdateToNow();
            _currentGameTime = RealGameTime;

            GameEventScheduler = new(RealGameTime, FixedTimeBetweenUpdates);

            success &= EntityManager.Initialize();

            //LiveTuningManager.Instance.CopyLiveTuningData(LiveTuningData);
            //LiveTuningData.GetLiveTuningUpdate();   // pre-generate update protobuf
            //_liveTuningChangeNum = LiveTuningData.ChangeNum;

            return success;
        }

        public void Run()
        {
            // NOTE: This is now separate from the constructor so that we can have
            // a dummy game with no simulation running that we use to parse messages.
            if (IsRunning) throw new InvalidOperationException();
            IsRunning = true;

            // Initialize and start game thread
            _gameThread = new(GameLoop) { Name = $"Game [{this}]", IsBackground = true, CurrentCulture = CultureInfo.InvariantCulture };
            _gameThread.Start();

            Logger.Info($"Game 0x{Id:X} started, initial replication id: {_currentRepId}");
        }

        public void Shutdown(GameShutdownReason reason)
        {
            if (IsRunning == false || HasBeenShutDown)
                return;

            Logger.Info($"Game shutdown requested. Game={this}, Reason={reason}");

            // Clean up network manager
            NetworkManager.SendAllPendingMessages();
            foreach (PlayerConnection playerConnection in NetworkManager)
                playerConnection.Disconnect();
            NetworkManager.Update();        // We need this to process player saves (for now)

            // Clean up entities
            EntityManager.DestroyAllEntities();
            EntityManager.ProcessDeferredLists();

            // Clean up regions
            //RegionManager.DestroyAllRegions();  V10_TODO

            // Mark this game as shut down for the player manager
            HasBeenShutDown = true;
        }

        public void AddClient(IFrontendClient client)
        {
            NetworkManager.AsyncAddClient(client);
        }

        public void RemoveClient(IFrontendClient client)
        {
            NetworkManager.AsyncRemoveClient(client);
        }

        public void ReceiveMessageBuffer(IFrontendClient client, in MessageBuffer messageBuffer)
        {
            NetworkManager.AsyncReceiveMessageBuffer(client, messageBuffer);
        }

        public Entity AllocateEntity(PrototypeId entityRef)
        {
            var proto = GameDatabase.GetPrototype<EntityPrototype>(entityRef);

            Entity entity;
            if (proto is SpawnerPrototype)
                entity = new Spawner(this);
            else if (proto is TransitionPrototype)
                entity = new Transition(this);
            else if (proto is AvatarPrototype)
                entity = new Avatar(this);
            else if (proto is MissilePrototype)
                entity = new Missile(this);
            else if (proto is PropPrototype) // DestructiblePropPrototype
                entity = new WorldEntity(this);
            else if (proto is AgentPrototype) // AgentTeamUpPrototype OrbPrototype SmartPropPrototype
                entity = new Agent(this);
            else if (proto is ItemPrototype) // CharacterTokenPrototype BagItemPrototype CostumePrototype CraftingIngredientPrototype
                                             // CostumeCorePrototype CraftingRecipePrototype ArmorPrototype ArtifactPrototype
                                             // LegendaryPrototype MedalPrototype RelicPrototype TeamUpGearPrototype
                                             // InventoryStashTokenPrototype EmoteTokenPrototype
                entity = new Item(this);
            else if (proto is KismetSequenceEntityPrototype)
                entity = new KismetSequenceEntity(this);
            else if (proto is HotspotPrototype)
                entity = new Hotspot(this);
            else if (proto is WorldEntityPrototype)
                entity = new WorldEntity(this);
            else if (proto is MissionMetaGamePrototype)
                entity = new MissionMetaGame(this);
            else if (proto is PvPPrototype)
                entity = new PvP(this);
            else if (proto is MetaGamePrototype) // MatchMetaGamePrototype
                entity = new MetaGame(this);
            else if (proto is PlayerPrototype)
                entity = new Player(this);
            else
                entity = new Entity(this);

            return entity;
        }

        public static long GetTimeFromStart(TimeSpan gameTime) => (long)(gameTime - StartTime).TotalMilliseconds;
        public static TimeSpan GetTimeFromDelta(long delta) => StartTime.Add(TimeSpan.FromMilliseconds(delta));

        private void GameLoop()
        {
            Current = this;
            _gameTimer.Start();

            CollectionPoolSettings.UseThreadLocalStorage = true;
            ObjectPoolManager.UseThreadLocalStorage = true;

            try
            {
                while (IsRunning)
                {
                    Update();
                }

                Shutdown(GameShutdownReason.ServerShuttingDown);
            }
            catch (Exception e)
            {
                HandleGameInstanceCrash(e);
                Shutdown(GameShutdownReason.GameInstanceCrash);
            }
        }

        private void Update()
        {
            // NOTE: We process input in NetworkManager.ReceiveAllPendingMessages() outside of UpdateFixedTime(), same as the client.

            NetworkManager.Update();                            // Add / remove clients
            NetworkManager.ReceiveAllPendingMessages();         // Process input
            NetworkManager.ProcessPendingPlayerConnections();   // Load pending players

            UpdateFixedTime();                                  // Update simulation state
        }

        private void UpdateFixedTime()
        {
            // First we make sure enough time has passed to do another fixed time update
            _realGameTime.UpdateToNow();

            if (_currentGameTime + FixedTimeBetweenUpdates > RealGameTime)
            {
                // Thread.Sleep() can sleep for longer than specified, so rather than sleeping
                // for the entire time window between fixed updates, we do it in 1 ms intervals.
                Thread.Sleep(1);
                return;
            }

            int timesUpdated = 0;

            TimeSpan updateStartTime = _gameTimer.Elapsed;
            while (_currentGameTime + FixedTimeBetweenUpdates <= RealGameTime)
            {
                _currentGameTime += FixedTimeBetweenUpdates;

                TimeSpan stepStartTime = _gameTimer.Elapsed;

                DoFixedTimeUpdate();
                _frameCount++;
                timesUpdated++;

                _lastFixedTimeUpdateProcessTime = _gameTimer.Elapsed - stepStartTime;

                if (_lastFixedTimeUpdateProcessTime > _fixedTimeUpdateProcessTimeLogThreshold)
                    Logger.Trace($"UpdateFixedTime(): Frame took longer ({_lastFixedTimeUpdateProcessTime.TotalMilliseconds:0.00} ms) than _fixedTimeUpdateWarningThreshold ({_fixedTimeUpdateProcessTimeLogThreshold.TotalMilliseconds:0.00} ms)");

                // Bail out if we have fallen behind more exceeded frame budget
                if (_gameTimer.Elapsed - updateStartTime > FixedTimeBetweenUpdates)
                    break;
            }

            // Track catch-up frames
            if (timesUpdated > 1)
            {
                Logger.Trace($"UpdateFixedTime(): Simulated {timesUpdated} frames in a single fixed update to catch up");
            }

            // Skip time if we have fallen behind
            TimeSpan timeSkip = RealGameTime - _currentGameTime;
            if (timeSkip != TimeSpan.Zero)
            {
                Logger.Trace($"UpdateFixedTime(): Taking too long to catch up, skipping {timeSkip.TotalMilliseconds} ms");
            }

            _currentGameTime = RealGameTime;
        }

        private void DoFixedTimeUpdate()
        {
            GameEventScheduler.TriggerEvents(_currentGameTime);

            EntityManager.LocomoteEntities();

            EntityManager.PhysicsResolveEntities();

            EntityManager.ProcessDeferredLists();

            // Send responses to all clients
            NetworkManager.SendAllPendingMessages();
        }

        private void HandleGameInstanceCrash(Exception exception)
        {
#if DEBUG
            const string buildConfiguration = "Debug";
#elif RELEASE
            const string buildConfiguration = "Release";
#endif

            DateTime now = DateTime.Now;

            string crashReportDir = Path.Combine(FileHelper.ServerRoot, "CrashReports");
            if (Directory.Exists(crashReportDir) == false)
                Directory.CreateDirectory(crashReportDir);

            string crashReportFilePath = Path.Combine(crashReportDir, $"GameInstanceCrash_{now.ToString(FileHelper.FileNameDateFormat)}.txt");

            using (StreamWriter writer = new(crashReportFilePath))
            {
                writer.WriteLine(string.Format("Assembly Version: {0} | {1} UTC | {2}\n",
                    AssemblyHelper.GetAssemblyInformationalVersion(),
                    AssemblyHelper.ParseAssemblyBuildTime().ToString("yyyy.MM.dd HH:mm:ss"),
                    buildConfiguration));

                writer.WriteLine($"Local Server Time: {now:yyyy.MM.dd HH:mm:ss.fff}\n");

                writer.WriteLine($"Game: {this}\n");

                writer.WriteLine($"Exception:\n{exception}\n");

                writer.WriteLine("Active Regions:");
                foreach (Region region in RegionManager)
                    writer.WriteLine(region.ToString());
                writer.WriteLine();

                writer.WriteLine("Scheduled Event Pool:");
                writer.Write(GameEventScheduler.GetPoolReportString());
                writer.WriteLine();

                writer.WriteLine($"Server Status:\n{ServerManager.Instance.GetServerStatus(true)}\n");
            }

            Logger.ErrorException(exception, $"Game instance crashed, report saved to {crashReportFilePath}");
        }
    }
}
