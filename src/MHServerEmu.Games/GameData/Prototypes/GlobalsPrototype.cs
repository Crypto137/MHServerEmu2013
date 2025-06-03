using MHServerEmu.Games.GameData.Calligraphy;
using MHServerEmu.Games.GameData.Prototypes.AI;

namespace MHServerEmu.Games.GameData.Prototypes
{
    public class GlobalsPrototype : Prototype
    {
        public PrototypeId AdvancementGlobals { get; protected set; }
        public PrototypeId AvatarSwapOutPower { get; protected set; }
        public PrototypeId ConnectionMarkerPrototype { get; protected set; }
        public PrototypeId DebugGlobals { get; protected set; }
        public PrototypeId UIGlobals { get; protected set; }
        public PrototypeId DefaultPlayer { get; protected set; }
        public PrototypeId DefaultStartTarget { get; protected set; }
        public PrototypeId[] PVPAlliances { get; protected set; }
        public float HighFlyingHeight { get; protected set; }
        public float LowHealthTrigger { get; protected set; }
        public float MouseHitCollisionMultiplier { get; protected set; }
        public float MouseHitMovingTargetsIncrease { get; protected set; }
        public float MouseHitPowerTargetSearchDist { get; protected set; }
        public float MouseHitPreferredAddition { get; protected set; }
        public float MousePathingDeadZoneRadius { get; protected set; }
        public float MouseMovementNoPathRadius { get; protected set; }
        public PrototypeId MissionGlobals { get; protected set; }
        public int TaggingResetDurationMS { get; protected set; }
        public CameraSettingPrototype[] PlayerCameraSettings { get; protected set; }
        public long PlayerPartyMaxSize { get; protected set; }
        public float NaviBudgetBaseCellSizeWidth { get; protected set; }
        public float NaviBudgetBaseCellSizeLength { get; protected set; }
        public int NaviBudgetBaseCellMaxPoints { get; protected set; }
        public int NaviBudgetBaseCellMaxEdges { get; protected set; }
        public AssetId[] UIConfigFiles { get; protected set; }
        public float ServerAvatarRangeCheckPadding { get; protected set; }
        public int InteractRange { get; protected set; }
        public float LootDropAugmentationScalar { get; protected set; }
        public PrototypeId CreditsItemPrototype { get; protected set; }
        public PrototypeId[] NegStatusEffectList { get; protected set; }
        public PrototypeId PvPPrototype { get; protected set; }
        public PrototypeId MissionPrototype { get; protected set; }
        public PrototypeId DifficultyModeDefault { get; protected set; }
        public PrototypeId[] DifficultyModes { get; protected set; }
        public EvalPrototype ItemPriceMultiplierBuyFromVendor { get; protected set; }
        public EvalPrototype ItemPriceMultiplierSellToVendor { get; protected set; }
        public CameraSettingPrototype[] PlayerCameraSettingsFlying { get; protected set; }
        public ModGlobalsPrototype ModGlobals { get; protected set; }
        public float MouseMoveDrivePathMaxLengthMult { get; protected set; }
        public AssetId AudioGlobalEventsClass { get; protected set; }
        public PrototypeId PowerKeywordPrototype { get; protected set; }
        public PrototypeId MetaGamePrototype { get; protected set; }
        public int MobLOSVisUpdatePeriodMS { get; protected set; }
        public int MobLOSVisStayVisibleDelayMS { get; protected set; }
        public bool MobLOSVisEnabled { get; protected set; }
        public PrototypeId DestructibleKeyword { get; protected set; }
        public AssetTypeId[] BeginPlayAssetTypes { get; protected set; }
        public AssetTypeId[] CachedAssetTypes { get; protected set; }
        public AssetTypeId[] FileVerificationAssetTypes { get; protected set; }
        public int LootInitializationLevelOffset { get; protected set; }
        public AssetId LoadingMusic { get; protected set; }
        public float LootUsableByRecipientPercent { get; protected set; }
        public LocaleStringId SystemLocalized { get; protected set; }
        public PrototypeId PopulationGlobals { get; protected set; }
        public PrototypeId PlayerAlliance { get; protected set; }
        public PrototypeId LootContainerKeyword { get; protected set; }
        public PrototypeId ClusterConfigurationGlobals { get; protected set; }
        public PrototypeId DownloadChunks { get; protected set; }
        public PrototypeId UIItemInventory { get; protected set; }
        public PrototypeId AIGlobals { get; protected set; }
        public AssetTypeId MusicAssetType { get; protected set; }
        public PrototypeId PetPowerKeyword { get; protected set; }
        public PrototypeId ResurrectionDefaultInfo { get; protected set; }
        public PrototypeId PartyJoinPortal { get; protected set; }
        public PrototypeId PartyWarpToMemberPortal { get; protected set; }
        public PrototypeId MatchJoinPortal { get; protected set; }
        public AssetTypeId MovieAssetType { get; protected set; }
        public PrototypeId WaypointGraph { get; protected set; }
        public PrototypeId WaypointHotspot { get; protected set; }
        public float MouseHoldDeadZoneRadius { get; protected set; }
        public PrototypeId VacuumableKeyword { get; protected set; }
        public GlobalPropertiesPrototype Properties { get; protected set; }
        public int PlayerGracePeroidInSeconds { get; protected set; }
        public PrototypeId CheckpointHotspot { get; protected set; }
        public PrototypeId LootFailSafeLocationTable { get; protected set; }
        public PrototypeId ReturnToHubPower { get; protected set; }
        public float ResistanceDamageAbsorbedPerPoint { get; protected set; }
        public CurveId LootLevelDistribution { get; protected set; }
        public EvalPrototype EvalOnEnduranceUpdate { get; protected set; }
        public int EnduranceUpdateTimeMS { get; protected set; }
        public int DisableEndurRegenOnPowerEndMS { get; protected set; }
        public PrototypeId PowerPrototype { get; protected set; }
        public PrototypeId WorldEntityPrototype { get; protected set; }
        public PrototypeId AreaPrototype { get; protected set; }
        public PrototypeId PopulationObjectPrototype { get; protected set; }
        public PrototypeId RegionPrototype { get; protected set; }
        public AssetTypeId AmbientSfxType { get; protected set; }
        public PrototypeId CombatGlobals { get; protected set; }
        public float OrientForPowerMaxTimeSecs { get; protected set; }
        public PrototypeId KismetSequenceEntityPrototype { get; protected set; }
        public PrototypeId DynamicArea { get; protected set; }
        public PrototypeId ReturnToFieldPower { get; protected set; }
        public float AssetCacheCellLoadOutRunSeconds { get; protected set; }
        public int AssetCacheMRUSize { get; protected set; }
        public int AssetCachePrefetchMRUSize { get; protected set; }
        public PrototypeId EntityKeywordPrototype { get; protected set; }
        public PrototypeId AvatarSwapInPower { get; protected set; }
        public PrototypeId PlayerStartingFaction { get; protected set; }
        public PrototypeId VendorBuybackInventory { get; protected set; }
        public PrototypeId AnyAlliancePrototype { get; protected set; }
        public PrototypeId AnyFriendlyAlliancePrototype { get; protected set; }
        public PrototypeId AnyHostileAlliancePrototype { get; protected set; }
        public PrototypeId DifficultyModifier { get; protected set; }
        public PrototypeId BodysliderPowerKeyword { get; protected set; }
        public PrototypeId OrbEntityKeyword { get; protected set; }
        public PrototypeId DifficultyModePCZ { get; protected set; }
        public CurveId LootBonusRarityCurve { get; protected set; }
        public CurveId ExperienceBonusCurve { get; protected set; }
        public PrototypeId TransitionGlobals { get; protected set; }
        public int PlayerGuildMaxSize { get; protected set; }
        public bool AutoPartyEnabledInitially { get; protected set; }
        public float LootUnrestedSpecialFindScalar { get; protected set; }
        public PrototypeId ItemBindingAffix { get; protected set; }
        public int InteractFallbackRange { get; protected set; }
        public PrototypeId ItemAcquiredThroughMTXStoreAffix { get; protected set; }
    }

    public class AdvancementGlobalsPrototype : Prototype
    {
        public CurveId LevelingCurve { get; protected set; }
        public CurveId DeathPenaltyCost { get; protected set; }
        public CurveId ItemEquipRequirementOffset { get; protected set; }
        public CurveId PowerPointsGrantedAtLevel { get; protected set; }
        public CurveId VendorLevelingCurve { get; protected set; }
        public PrototypeId StatsEval { get; protected set; }
        public PrototypeId AvatarThrowabilityEval { get; protected set; }
        public EvalPrototype VendorLevelingEval { get; protected set; }
        public EvalPrototype VendorRollTableLevelEval { get; protected set; }
        public float RestedHealthPerMinMult { get; protected set; }
    }

    public class AIGlobalsPrototype : Prototype
    {
        public PrototypeId LeashReturnHeal { get; protected set; }
        public PrototypeId LeashReturnImmunity { get; protected set; }
        public PrototypeId LeashingBehaviorTreeNodeOverride { get; protected set; }
        public PrototypeId LeashingProceduralProfile { get; protected set; }
        public int RandomThinkVarianceMS { get; protected set; }
    }

    public class DebugGlobalsPrototype : Prototype
    {
        public PrototypeId CreateEntityShortcutEntity { get; protected set; }
        public PrototypeId DynamicRegion { get; protected set; }
        public float HardModeMobDmgBuff { get; protected set; }
        public float HardModeMobHealthBuff { get; protected set; }
        public float HardModeMobMoveSpdBuff { get; protected set; }
        public float HardModePlayerEnduranceCostDebuff { get; protected set; }
        public PrototypeId PowersArtModeEntity { get; protected set; }
        public int StartingLevelMobs { get; protected set; }
        public float StartingLevelAvatars { get; protected set; }
        public PrototypeId TransitionRef { get; protected set; }
        public PrototypeId CreateLootDummyEntity { get; protected set; }
        public PrototypeId MapErrorMapInfo { get; protected set; }
        public bool IgnoreDeathPenalty { get; protected set; }
        public bool TrashedItemsDropInWorld { get; protected set; }
        public PrototypeId PAMEnemyAlliance { get; protected set; }
        public EvalPrototype DebugEval { get; protected set; }
        public EvalPrototype DebugEvalUnitTest { get; protected set; }
        public BotSettingsPrototype BotSettings { get; protected set; }
    }

    public class CharacterSheetDetailedStatPrototype : Prototype
    {
        public EvalPrototype Expression { get; protected set; }
        public PrototypeId ExpressionExt { get; protected set; }
        public LocaleStringId Format { get; protected set; }
        public LocaleStringId Label { get; protected set; }
        public LocaleStringId Tooltip { get; protected set; }
    }

    public class HelpGameTermPrototype : Prototype
    {
        public LocaleStringId Name { get; protected set; }
        public LocaleStringId Description { get; protected set; }
    }

    public class HelpTextPrototype : Prototype
    {
        public LocaleStringId GeneralControls { get; protected set; }
        public HelpGameTermPrototype[] GameTerms { get; protected set; }
        public LocaleStringId Crafting { get; protected set; }
        public LocaleStringId EndgamePvE { get; protected set; }
        public LocaleStringId PvP { get; protected set; }
        public LocaleStringId Tutorial { get; protected set; }
    }

    public class UIGlobalsPrototype : Prototype
    {
        public PrototypeId MessageDefault { get; protected set; }
        public PrototypeId MessageLevelUp { get; protected set; }
        public PrototypeId MessageItemError { get; protected set; }
        public PrototypeId MessageRegionChange { get; protected set; }
        public PrototypeId MessageMissionAccepted { get; protected set; }
        public PrototypeId MessageMissionCompleted { get; protected set; }
        public PrototypeId MessageMissionFailed { get; protected set; }
        public int AvatarSwitchUIDeathDelayMS { get; protected set; }
        public PrototypeId UINotificationGlobals { get; protected set; }
        public int RosterPageSize { get; protected set; }
        public AssetId LocalizedInfoDirectory { get; protected set; }
        public int TooltipHideDelayMS { get; protected set; }
        public PrototypeId MessagePowerError { get; protected set; }
        public PrototypeId UIStringGlobals { get; protected set; }
        public PrototypeId MessagePartyInvite { get; protected set; }
        public PrototypeId MapInfoMissionGiver { get; protected set; }
        public PrototypeId MapInfoMissionObjectiveTalk { get; protected set; }
        public int NumAvatarsToDisplayInItemUsableLists { get; protected set; }
        public PrototypeId TextStyleCraftComponentHave { get; protected set; }
        public PrototypeId TextStyleCraftComponentMissing { get; protected set; }
        public PrototypeId[] LoadingScreens { get; protected set; }
        public int ChatFadeInMS { get; protected set; }
        public int ChatBeginFadeOutMS { get; protected set; }
        public int ChatFadeOutMS { get; protected set; }
        public PrototypeId MessageWaypointUnlocked { get; protected set; }
        public PrototypeId MessagePowerUnlocked { get; protected set; }
        public PrototypeId UIMapGlobals { get; protected set; }
        public PrototypeId TextStyleCurrentlyEquipped { get; protected set; }
        public int ChatTextFadeOutMS { get; protected set; }
        public int ChatTextHistoryMax { get; protected set; }
        public PrototypeId KeywordFemale { get; protected set; }
        public PrototypeId KeywordMale { get; protected set; }
        public PrototypeId TextStylePowerUpgradeImprovement { get; protected set; }
        public PrototypeId TextStylePowerUpgradeNoImprovement { get; protected set; }
        public PrototypeId LoadingScreenIntraRegion { get; protected set; }
        public PrototypeId TextStyleVendorPriceCanBuy { get; protected set; }
        public PrototypeId TextStyleVendorPriceCantBuy { get; protected set; }
        public PrototypeId TextStyleItemRestrictionFailure { get; protected set; }
        public int CostumeClosetNumAvatarsVisible { get; protected set; }
        public int CostumeClosetNumCostumesVisible { get; protected set; }
        public PrototypeId MessagePowerErrorDoNotQueue { get; protected set; }
        public PrototypeId TextStylePvPShopPurchased { get; protected set; }
        public PrototypeId TextStylePvPShopUnpurchased { get; protected set; }
        public PrototypeId MessagePowerPointsAwarded { get; protected set; }
        public PrototypeId MapInfoMissionObjectiveUse { get; protected set; }
        public PrototypeId TextStyleMissionRewardFloaty { get; protected set; }
        public PrototypeId PowerTooltipBodyCurRank0Unlkd { get; protected set; }
        public PrototypeId PowerTooltipBodyCurRankLocked { get; protected set; }
        public PrototypeId PowerTooltipBodyCurRank1AndUp { get; protected set; }
        public PrototypeId PowerTooltipBodyNextRank1First { get; protected set; }
        public PrototypeId PowerTooltipBodyNextRank2AndUp { get; protected set; }
        public PrototypeId PowerTooltipHeader { get; protected set; }
        public PrototypeId MapInfoFlavorNPC { get; protected set; }
        public int TooltipSpawnHideDelayMS { get; protected set; }
        public int KioskIdleResetTimeSec { get; protected set; }
        public PrototypeId KioskSizzleMovie { get; protected set; }
        public int KioskSizzleMovieStartTimeSec { get; protected set; }
        public PrototypeId MapInfoHealer { get; protected set; }
        public PrototypeId TextStyleOpenMission { get; protected set; }
        public PrototypeId MapInfoPartyMember { get; protected set; }
        public int LoadingScreenTipTimeIntervalMS { get; protected set; }
        public PrototypeId TextStyleKillRewardFloaty { get; protected set; }
        public PrototypeId TextStyleAvatarOverheadNormal { get; protected set; }
        public PrototypeId TextStyleAvatarOverheadParty { get; protected set; }
        public CharacterSheetDetailedStatPrototype[] CharacterSheetDetailedStats { get; protected set; }
        public PrototypeId PowerProgTableTabRefTab1 { get; protected set; }
        public PrototypeId PowerProgTableTabRefTab2 { get; protected set; }
        public PrototypeId PowerProgTableTabRefTab3 { get; protected set; }
        public float ScreenEdgeArrowRange { get; protected set; }
        public PrototypeId HelpText { get; protected set; }
        public PrototypeId MessagePvPFactionPortalFail { get; protected set; }
        public PrototypeId PropertyTooltipTextOverride { get; protected set; }
        public PrototypeId MessagePvPDisabledPortalFail { get; protected set; }
        public PrototypeId MessageStatProgression { get; protected set; }
        public PrototypeId MessagePvPPartyPortalFail { get; protected set; }
        public PrototypeId TextStyleMissionHudOpenMission { get; protected set; }
        public PrototypeId CreditsMovie { get; protected set; }
        public PrototypeId MapInfoAvatarDefeated { get; protected set; }
        public PrototypeId MapInfoPartyMemberDefeated { get; protected set; }
        public PrototypeId MessageGuildInvite { get; protected set; }
        public PrototypeId MapInfoMissionObjectiveMob { get; protected set; }
        public PrototypeId MapInfoMissionObjectivePortal { get; protected set; }
        public PrototypeId CinematicsListLoginScreen { get; protected set; }
        public PrototypeId TextStyleGuildLeader { get; protected set; }
        public PrototypeId TextStyleGuildOfficer { get; protected set; }
        public PrototypeId TextStyleGuildMember { get; protected set; }
        public AffixDisplaySlotPrototype[] CostumeAffixDisplaySlots { get; protected set; }
#if !BUILD_1_10_0_69
        public PrototypeId MessagePartyError { get; protected set; }
#endif
    }

    public class UINotificationGlobalsPrototype : Prototype
    {
        public PrototypeId NotificationPartyInvite { get; protected set; }
        public PrototypeId NotificationLevelUp { get; protected set; }
        public PrototypeId NotificationServerMessage { get; protected set; }
        public PrototypeId NotificationRemoteMission { get; protected set; }
        public PrototypeId NotificationMissionUpdate { get; protected set; }
        public PrototypeId NotificationMatchInvite { get; protected set; }
        public PrototypeId NotificationMatchQueue { get; protected set; }
        public PrototypeId NotificationPvPShop { get; protected set; }
        public PrototypeId NotificationPowerPointsAwarded { get; protected set; }
        public int NotificationPartyAIAggroRange { get; protected set; }
        public PrototypeId NotificationOfferingUI { get; protected set; }
        public PrototypeId NotificationGuildInvite { get; protected set; }
    }

    public class UIMapGlobalsPrototype : Prototype
    {
        public float DefaultRevealRadius { get; protected set; }
        public float DefaultZoom { get; protected set; }
        public float FullScreenMapAlphaMax { get; protected set; }
        public float FullScreenMapAlphaMin { get; protected set; }
        public int FullScreenMapResolutionHeight { get; protected set; }
        public int FullScreenMapResolutionWidth { get; protected set; }
        public float FullScreenMapScale { get; protected set; }
        public float LowResRevealMultiplier { get; protected set; }
        public AssetId MapColorFiller { get; protected set; }
        public AssetId MapColorWalkable { get; protected set; }
        public AssetId MapColorWall { get; protected set; }
        public float MiniMapAlpha { get; protected set; }
        public int MiniMapResolution { get; protected set; }
        public float CameraAngleX { get; protected set; }
        public float CameraAngleY { get; protected set; }
        public float CameraAngleZ { get; protected set; }
        public float CameraFOV { get; protected set; }
        public float CameraNearPlane { get; protected set; }
        public float FullScreenMapPOISize { get; protected set; }
        public float POIScreenFacingRot { get; protected set; }
        public bool DrawPOIInCanvas { get; protected set; }
        public bool EnableMinimapProjection { get; protected set; }
        public float DefaultZoomMin { get; protected set; }
        public float DefaultZoomMax { get; protected set; }
        public float MiniMapPOISizeMin { get; protected set; }
        public float MiniMapPOISizeMax { get; protected set; }
    }

    public class CameraSettingPrototype : Prototype
    {
        public float DirectionX { get; protected set; }
        public float DirectionY { get; protected set; }
        public float DirectionZ { get; protected set; }
        public float Distance { get; protected set; }
        public float FieldOfView { get; protected set; }
        public float ListenerDistance { get; protected set; }
        public int RotationPitch { get; protected set; }
        public int RotationRoll { get; protected set; }
        public int RotationYaw { get; protected set; }
        public float LookAtOffsetX { get; protected set; }
        public float LookAtOffsetY { get; protected set; }
        public float LookAtOffsetZ { get; protected set; }
    }

    public class GlobalPropertiesPrototype : Prototype
    {
        public PrototypePropertyCollection Properties { get; protected set; }
    }

    public class PopulationGlobalsPrototype : Prototype
    {
        public LocaleStringId MessageEnemiesGrowStronger { get; protected set; }
        public LocaleStringId MessageEnemiesGrowWeaker { get; protected set; }
        public int SpawnMapPoolTickMS { get; protected set; }
        public int SpawnMapLevelTickMS { get; protected set; }
        public float CrowdSupressionRadius { get; protected set; }
        public bool SupressSpawnOnPlayer { get; protected set; }
        public int SpawnMapGimbalRadius { get; protected set; }
        public int SpawnMapHorizon { get; protected set; }
        public PrototypeId[] UpgradableMobRanks { get; protected set; }
        public float SpawnMapMaxChance { get; protected set; }
        public PrototypeId EmptyPopulation { get; protected set; }
        public PrototypeId TwinEnemyBoost { get; protected set; }
        public PrototypeId RankMiniBoss { get; protected set; }
#if !BUILD_1_10_0_69
        public int DestructiblesForceSpawnMS { get; protected set; }
#endif
    }

    public class ClusterConfigurationGlobalsPrototype : Prototype
    {
        public int MinutesToKeepOfflinePlayerGames { get; protected set; }
        public int MinutesToKeepUnusedRegions { get; protected set; }
    }

    public class CombatGlobalsPrototype : Prototype
    {
        public PrototypeId EvalDodgeChanceFormula { get; protected set; }
        public PrototypeId EvalCastSpeedFormula { get; protected set; }
        public PrototypeId EvalInterruptChanceFormula { get; protected set; }
        public PrototypeId EvalNegStatusResistChanceFormula { get; protected set; }
        public CurveId DefaultMobToPlayerDamageCurve { get; protected set; }
        public CurveId DefaultPlayerToMobDamageCurve { get; protected set; }
        public float DurationBasedEffectConstant { get; protected set; }
        public float ServerRangeCheckPadding { get; protected set; }
        public EvalPrototype EvalDamageMobToPlayer { get; protected set; }
        public EvalPrototype EvalDamagePlayerToMob { get; protected set; }
        public PrototypeId ChannelInterruptCondition { get; protected set; }
        public float PowerDmgBonusHardcoreAttenuation { get; protected set; }
        public int MouseHoldStartMoveDelayMeleeMS { get; protected set; }
        public int MouseHoldStartMoveDelayRangedMS { get; protected set; }
        public float PlayerToPlayerDamageMultiplier { get; protected set; }
    }

    public class TransitionGlobalsPrototype : Prototype
    {
        public RegionPortalControlEntryPrototype[] ControlledRegions { get; protected set; }
        public PrototypeId EnabledState { get; protected set; }
        public PrototypeId DisabledState { get; protected set; }
    }

    public class AvatarOnKilledInfoPrototype : Prototype
    {
        public LocaleStringId DeathReleaseButton { get; protected set; }
        public LocaleStringId DeathReleaseDialogMessage { get; protected set; }
        public int DeathReleaseTimeoutMS { get; protected set; }
        public LocaleStringId ResurrectionDialogMessage { get; protected set; }
        public int ResurrectionTimeoutMS { get; protected set; }
    }
}
