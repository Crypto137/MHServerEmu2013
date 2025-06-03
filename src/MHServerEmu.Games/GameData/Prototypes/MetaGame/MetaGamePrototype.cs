using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes.MetaGame
{
    #region Enums

    [AssetEnum((int)Invalid)]
    public enum ScoreTableValueType
    {
        Invalid,
        Int,
        Float,
    }

    #endregion

    public class MetaGamePrototype : EntityPrototype
    {
        public float Duration { get; protected set; }
        public PrototypeId[] Teams { get; protected set; }
        public PrototypeId[] GameModes { get; protected set; }
        public PrototypeId BodysliderOverride { get; protected set; }
        public LocaleStringId MetaGameMissionText { get; protected set; }
        public LocaleStringId MetaGameObjectiveText { get; protected set; }
        public PrototypeId MapInfoAvatarDefeatedOverride { get; protected set; }
        public bool DiscoverAvatarsForPlayers { get; protected set; }
#if !BUILD_1_10_0_69
        public int SoftLockRegionMode { get; protected set; }
#endif
    }

    public class MatchMetaGamePrototype : MetaGamePrototype
    {
        public PrototypeId StartRegion { get; protected set; }
    }

    public class MatchQueuePrototype : Prototype
    {
        public PrototypeId[] MatchTypes { get; protected set; }
        public LocaleStringId Name { get; protected set; }
        public LocaleStringId QueueMsg { get; protected set; }
        public int BalanceMethod { get; protected set; }
        public int RegionLevel { get; protected set; }
        public AssetId GameSystem { get; protected set; }
    }

    public class PvPScoreEventHandlerPrototype : Prototype
    {
    }

    public class PvPTeamPrototype : MatchTeamPrototype
    {
        public PrototypeId Alliance { get; protected set; }
        public PrototypeId SpawnMarker { get; protected set; }
        public PrototypeId StartHealingAura { get; protected set; }
        public PrototypeId StartTarget { get; protected set; }
        public AssetId IconPath { get; protected set; }
    }

    public class PvPMiniMapIconsPrototype : Prototype
    {
        public PrototypeId AlliedMinion { get; protected set; }
        public PrototypeId Ally { get; protected set; }
        public PrototypeId Enemy { get; protected set; }
        public PrototypeId EnemyMinion { get; protected set; }
    }

    public class PvPPrototype : MatchMetaGamePrototype
    {
        public int RespawnCooldown { get; protected set; }
        public int StartingScore { get; protected set; }
        public PrototypeId ScoreSchema { get; protected set; }
        public PrototypeId UpgradeShop { get; protected set; }
        public PrototypeId MiniMapFilter { get; protected set; }
        public PrototypeId AvatarKilledLootTable { get; protected set; }
        public bool IsPvP { get; protected set; }
    }

    public class PvEScaleEnemyBoostEntryPrototype : Prototype
    {
        public PrototypeId EnemyBoost { get; protected set; }
        public PrototypeId UINotification { get; protected set; }
    }

    public class PvEScaleWavePopulationPrototype : Prototype
    {
        public PopulationRequiredObjectListPrototype[] Choices { get; protected set; }
    }

    public class MetaGameModePrototype : Prototype
    {
        public PrototypeId AvatarOnKilledInfoOverride { get; protected set; }
        public PrototypeId EventHandler { get; protected set; }
        public PrototypeId UINotificationOnActivate { get; protected set; }
        public PrototypeId UINotificationOnDeactivate { get; protected set; }
        public bool ShowTimer { get; protected set; }
        public LocaleStringId Name { get; protected set; }
        public int ActiveGoalRepeatTimeMS { get; protected set; }
        public PrototypeId UINotificationActiveGoalRepeat { get; protected set; }
    }

    public class MetaGameIdleModePrototype : MetaGameModePrototype  // V10_NOTE: Older version of MetaGameModeIdlePrototype 
    {
        public int DurationMS { get; protected set; }
        public int NextMode { get; protected set; }
        public bool PlayersCanMove { get; protected set; }
        public bool DisplayScoreInfoOnActivate { get; protected set; }
        public bool TeleportPlayersToStartOnActivate { get; protected set; }
        public PrototypeId KismetSequenceOnActivate { get; protected set; }
        public int PlayerCountToAdvance { get; protected set; }
        public PrototypeId DeathRegionTarget { get; protected set; }
    }

    public class PvPShutdownModePrototype : MetaGameModePrototype   // V10_NOTE: Older version of MetaGameModeShutdownPrototype
    {
        public PrototypeId ShutdownTarget { get; protected set; }
    }

    public class PvEScaleGameModePrototype : MetaGameModePrototype
    {
        public int WaveDurationMS { get; protected set; }
        public int WaveDurationCriticalTimeMS { get; protected set; }
        public int WaveDurationLowTimeMS { get; protected set; }
        public int WaveBossDelayMS { get; protected set; }
        public PrototypeId[] BossPopulationObjects { get; protected set; }
        public PrototypeId BossUINotification { get; protected set; }
        public PrototypeId DeathRegionTarget { get; protected set; }
        public PrototypeId PopulationOverrideTheme { get; protected set; }
        public PrototypeId PopulationOverrideAreas { get; protected set; }
        public PrototypeId PowerUpMarkerType { get; protected set; }
        public PrototypeId PowerUpItem { get; protected set; }
        public PrototypeId PowerUpPowerToRemove { get; protected set; }
        public int NextMode { get; protected set; }
        public int FailMode { get; protected set; }
        public PrototypeId FailUINotification { get; protected set; }
        public PrototypeId SuccessUINotification { get; protected set; }
        public int DifficultyIndex { get; protected set; }
        public LocaleStringId BossModeNameOverride { get; protected set; }
        public LocaleStringId PowerUpExtraText { get; protected set; }
        public PrototypeId WavePopulation { get; protected set; }
        public PvEScaleEnemyBoostEntryPrototype[] WaveEnemyBoosts { get; protected set; }
        public float WaveDifficultyPerSecond { get; protected set; }
        public int PowerUpSpawnMS { get; protected set; }
        public float PowerUpDifficultyReduction { get; protected set; }
        public float MobTotalDifficultyReduction { get; protected set; }
        public PrototypeId PowerUpSpawnUINotification { get; protected set; }
        public int WaveDifficultyFailureThreshold { get; protected set; }
        public int WaveDifficultyWarningThreshold { get; protected set; }
        public int WaveEnemyBoostsPickCount { get; protected set; }
        public PrototypeId WaveOnSpawnPower { get; protected set; }
        public PrototypeId[] BossEnemyBoosts { get; protected set; }
        public int BossEnemyBoostsPicks { get; protected set; }
        public PrototypeId WaveOnDespawnPower { get; protected set; }
    }

    public class PvEWaveGameModePrototype : MetaGameModePrototype
    {
        public int WaveDurationMS { get; protected set; }
        public int WaveDurationCriticalTimeMS { get; protected set; }
        public int WaveDurationLowTimeMS { get; protected set; }
        public int WaveBossDelayMS { get; protected set; }
        public PrototypeId BossSpawner { get; protected set; }
        public PrototypeId BossPopulationObject { get; protected set; }
        public PrototypeId BossUINotification { get; protected set; }
        public PrototypeId DeathRegionTarget { get; protected set; }
        public PrototypeId PopulationOverrideTheme { get; protected set; }
        public PrototypeId PopulationOverrideAreas { get; protected set; }
        public PrototypeId PowerUpMarkerType { get; protected set; }
        public PrototypeId PowerUpItem { get; protected set; }
        public PrototypeId PowerUpPowerToRemove { get; protected set; }
        public int NextMode { get; protected set; }
        public int FailMode { get; protected set; }
        public PrototypeId FailUINotification { get; protected set; }
        public PrototypeId SuccessUINotification { get; protected set; }
        public int DifficultyIndex { get; protected set; }
        public LocaleStringId BossModeNameOverride { get; protected set; }
        public LocaleStringId PowerUpExtraText { get; protected set; }
    }

    public class PvPFactionGameModePrototype : MetaGameModePrototype
    {
        public FactionDefenderDataPrototype[] Defenders { get; protected set; }
    }

    public class MissionMetaGamePrototype : MetaGamePrototype
    {
        public int LevelLowerBoundsOffset { get; protected set; }
        public int LevelUpperBoundsOffset { get; protected set; }
    }

    public class PvPScoreSchemaEntryPrototype : Prototype   // V10_NOTE: Older version of ScoreTableSchemaEntryPrototype
    {
        public ScoreTableValueType Type { get; protected set; }
        public LocaleStringId Name { get; protected set; }
    }

    public class PvPScoreSchemaPrototype : Prototype    // V10_NOTE: Older version of ScoreTableSchemaPrototype
    {
        public PvPScoreSchemaEntryPrototype[] Schema { get; protected set; }
    }

    #region 1.10 Specific Prototypes

    public class PvEInstanceModePrototype : MetaGameModePrototype
    {
        public PrototypeId[] BossChoices { get; protected set; }
        public PrototypeId[] BossEnemyBoosts { get; protected set; }
        public EntityFilterPrototype BossFilter { get; protected set; }
        public PrototypeId Crystal { get; protected set; }
        public int CrystalCount { get; protected set; }
        public PrototypeId CrystalMarker { get; protected set; }
        public int DeathsBeforeShutdown { get; protected set; }
        public LocaleStringId DeathsTimerText { get; protected set; }
        public int FailMode { get; protected set; }
        public int SuccessMode { get; protected set; }
        public int WaveDurationMS { get; protected set; }
        public int BossSpawnDelayMS { get; protected set; }
        public PrototypeId WavePopulation { get; protected set; }
        public int BossEnemyBoostPicks { get; protected set; }
        public PrototypeId[] WaveEnemyBoosts { get; protected set; }
        public int WaveEnemyBoostPicks { get; protected set; }
        public int DifficultyStartIndex { get; protected set; }
        public int DifficultyBarMax { get; protected set; }
        public float DifficultyOnCrystalInteract { get; protected set; }
        public float DifficultyTotalMobReduction { get; protected set; }
        public PrototypeId WavePopulationThemes { get; protected set; }
        public float WavePopulationThemesDensity { get; protected set; }
        public PrototypeId DeathUINotification { get; protected set; }
        public PrototypeId BossSpawnUINotification { get; protected set; }
        public PrototypeId BossTriggerUINotification { get; protected set; }
    }

    public class PvPMOBAGameModePrototype : MetaGameModePrototype
    {
        public MOBAPhasePrototype[] Phases { get; protected set; }
        public MOBASupportPrototype[] Support { get; protected set; }
        public MOBAMainPrototype[] Main { get; protected set; }
        public int SupportRespawnMS { get; protected set; }
        public int NextMode { get; protected set; }
    }

    public class PvEWaveBattleEventHandlerPrototype : PvPScoreEventHandlerPrototype
    {
        public int KillsEntry { get; protected set; }
        public int DamageTakenEntry { get; protected set; }
        public int DamageVsMinionsEntry { get; protected set; }
        public int DamageVsBossEntry { get; protected set; }
        public int DamageVsTotalEntry { get; protected set; }
        public int DeathsEntry { get; protected set; }
    }

    public class PvPFactionEventHandlerPrototype : PvPScoreEventHandlerPrototype
    {
        public int KillsEntry { get; protected set; }
        public int DamageTakenEntry { get; protected set; }
        public int DamageVsMinionsEntry { get; protected set; }
        public int DamageVsPlayersEntry { get; protected set; }
        public int DamageVsTotalEntry { get; protected set; }
        public int DeathsEntry { get; protected set; }
    }

    public class MOBAScoreEventHandlerPrototype : PvPScoreEventHandlerPrototype
    {
        public int AssistsEntry { get; protected set; }
        public int DamageTaken { get; protected set; }
        public int DamageVsPlayersEntry { get; protected set; }
        public int DamageVsTotalEntry { get; protected set; }
        public int DeathsEntry { get; protected set; }
        public int KillsCurrency { get; protected set; }
        public int KillsEntry { get; protected set; }
        public MOBAEntityRewardEntryPrototype[] XPObjects { get; protected set; }
        public int XPPlayerCurrency { get; protected set; }
        public int XPRadius { get; protected set; }
        public float AssistsRequestRange { get; protected set; }
        public int AssistsCurrency { get; protected set; }
    }

    public class PvPUpgradeEntryPrototype : Prototype
    {
        public PrototypeId Upgrade { get; protected set; }
        public int Cost { get; protected set; }
    }

    public class PvPUpgradePrototype : ModPrototype
    {
    }

    public class PvPUpgradeShopPrototype : Prototype
    {
        public PrototypeId[] Upgrades { get; protected set; }
    }

    public class MOBAEntityRewardEntryPrototype : Prototype
    {
        public int CurrencyXP { get; protected set; }
        public int CurrencyKill { get; protected set; }
        public PrototypeId Entity { get; protected set; }
    }

    public class MOBAMainPrototype : Prototype
    {
        public PrototypeId Entity { get; protected set; }
    }

    public class MOBAPhasePrototype : Prototype
    {
        public int SwitchWaveCount { get; protected set; }
        public int WavePeriodMS { get; protected set; }
        public MOBAWaveTriggerPrototype[] WaveTriggers { get; protected set; }
    }

    public class MOBASupportPrototype : Prototype
    {
        public PrototypeId Entity { get; protected set; }
        public PrototypeId SpawnerWhenDestroyed { get; protected set; }
    }

    public class MOBAWaveTriggerPrototype : Prototype
    {
        public int EveryNWaves { get; protected set; }
        public PrototypeId[] TriggerSpawners { get; protected set; }
    }

    public class FactionDefenderDataPrototype : Prototype
    {
        public PrototypeId Defender { get; protected set; }
        public PrototypeId Boost { get; protected set; }
        public PrototypeId UnderAttackUINotification { get; protected set; }
        public PrototypeId DeathUINotification { get; protected set; }
        public PrototypeId RespawnUINotification { get; protected set; }
    }

    public class MatchTeamPrototype : Prototype
    {
        public LocaleStringId Name { get; protected set; }
        public int MinPlayers { get; protected set; }
        public int MaxPlayers { get; protected set; }
        public PrototypeId Faction { get; protected set; }
    }

    #endregion
}
