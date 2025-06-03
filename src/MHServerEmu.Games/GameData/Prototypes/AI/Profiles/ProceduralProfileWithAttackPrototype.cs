namespace MHServerEmu.Games.GameData.Prototypes.AI.Profiles
{
    public class ProceduralProfileWithAttackPrototype : ProceduralProfileWithTargetPrototype
    {
        public int AttackRateMaxMS { get; protected set; }
        public int AttackRateMinMS { get; protected set; }
        public ProceduralUsePowerContextPrototype[] GenericProceduralPowers { get; protected set; }
#if !BUILD_1_10_0_69
        public ProceduralUseAffixPowerContextPrototype AffixSettings { get; protected set; }
#endif
    }

    public class ProceduralProfileStationaryTurretPrototype : ProceduralProfileWithAttackPrototype
    {
    }

    public class ProceduralProfileRotatingTurretWithTargetPrototype : ProceduralProfileWithAttackPrototype
    {
        public RotateContextPrototype Rotate { get; protected set; }
    }

    public class ProceduralProfileBasicMeleePrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype PrimaryPower { get; protected set; }
    }

    public class ProceduralProfileBasicMelee2PowerPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype Power1 { get; protected set; }
        public ProceduralUsePowerContextPrototype Power2 { get; protected set; }
    }

    public class ProceduralProfileBasicRangePrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype PrimaryPower { get; protected set; }
    }

    public class ProceduralProfileAlternateRange2Prototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public ProceduralFlankContextPrototype FlankTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype Power1 { get; protected set; }
        public ProceduralUsePowerContextPrototype Power2 { get; protected set; }
        public ProceduralUsePowerContextPrototype PowerSwap { get; protected set; }
    }

    public class ProceduralProfileMultishotRangePrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype MultishotPower { get; protected set; }
        public int NumShots { get; protected set; }
        public bool RetargetPerShot { get; protected set; }
    }

    public class ProceduralProfileMultishotFlankerPrototype : ProceduralProfileWithAttackPrototype
    {
        public ProceduralFlankContextPrototype FlankTarget { get; protected set; }
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype MultishotPower { get; protected set; }
        public int NumShots { get; protected set; }
        public bool RetargetPerShot { get; protected set; }
    }

    public class ProceduralProfileMultishotHiderPrototype : ProceduralProfileWithAttackPrototype
    {
        public ProceduralUsePowerContextPrototype HidePower { get; protected set; }
        public ProceduralUsePowerContextPrototype MultishotPower { get; protected set; }
        public int NumShots { get; protected set; }
        public bool RetargetPerShot { get; protected set; }
    }

    public class ProceduralProfileMeleeSpeedByDistancePrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype PrimaryPower { get; protected set; }
        public UsePowerContextPrototype ExtraSpeedPower { get; protected set; }
        public UsePowerContextPrototype SpeedRemovalPower { get; protected set; }
        public float DistanceFromTargetForSpeedBonus { get; protected set; }
    }

    public class ProceduralProfileRangeFlankerPrototype : ProceduralProfileWithAttackPrototype
    {
        public ProceduralFlankContextPrototype FlankTarget { get; protected set; }
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype PrimaryPower { get; protected set; }
    }

    public class ProceduralProfileSkirmisherPrototype : ProceduralProfileWithAttackPrototype
    {
        public WanderContextPrototype SkirmishMovement { get; protected set; }
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype PrimaryPower { get; protected set; }
        public float MoveToSpeedBonus { get; protected set; }
    }

    public class ProceduralProfileRangedWithMeleePriority2PowerPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype MeleePower { get; protected set; }
        public ProceduralUsePowerContextPrototype RangedPower { get; protected set; }
        public float MaxDistToMoveIntoMelee { get; protected set; }
        public MoveToContextPrototype MoveIntoMeleeRange { get; protected set; }
    }

    public class ProfMeleePwrSpecialAtHealthPctPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public float SpecialAtHealthChunkPct { get; protected set; }
        public UsePowerContextPrototype SpecialPowerAtHealthChunkPct { get; protected set; }
    }

    public class ProceduralProfileNoMoveSimplifiedSensoryPrototype : ProceduralProfileWithAttackPrototype
    {
    }

    public class ProfKillSelfAfterOnePowerNoMovePrototype : ProceduralProfileWithAttackPrototype
    {
    }

    public class ProceduralProfileNoMoveNoSensePrototype : ProceduralProfileWithAttackPrototype
    {
    }

    public class ProceduralProfilePvPMeleePrototype : ProceduralProfileWithAttackPrototype
    {
        public float AggroRadius { get; protected set; }
        public float AggroDropRadius { get; protected set; }
        public float AggroDropByLOSChance { get; protected set; }
        public PrototypeId PrimaryPower { get; protected set; }
        public int PathGroup { get; protected set; }
        public PathMethod PathMethod { get; protected set; }
        public float PathThreshold { get; protected set; }
    }

    public class ProceduralProfilePvPTowerPrototype : ProceduralProfileWithAttackPrototype
    {
        public float AggroDropByLOSChance { get; protected set; }
        public PrototypeId PrimaryPower { get; protected set; }
    }

    public class ProceduralProfileMeleeDropWeaponPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype PowerMeleeWithWeapon { get; protected set; }
        public ProceduralUsePowerContextPrototype PowerMeleeNoWeapon { get; protected set; }
        public ProceduralUsePowerContextPrototype PowerDropWeapon { get; protected set; }
        public ProceduralUsePowerContextPrototype PowerPickupWeapon { get; protected set; }
        public SelectEntityContextPrototype SelectWeaponAsTarget { get; protected set; }
    }

    public class ProceduralProfileMeleeAllyDeathFleePrototype : ProceduralProfileWithAttackPrototype
    {
        public FleeContextPrototype FleeFromTargetOnAllyDeath { get; protected set; }
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
    }

    public class ProceduralProfileRangedHotspotDropperPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype RangedPower { get; protected set; }
        public ProceduralUsePowerContextPrototype HotspotPower { get; protected set; }
        public WanderContextPrototype HotspotDroppingMovement { get; protected set; }
    }

    public class ProceduralProfilePetMeleePrototype : ProceduralProfileBasicMeleePrototype  // V10_NOTE: Older version of ProceduralProfilePetPrototype
    {
        public MoveToContextPrototype PetFollow { get; protected set; }
    }

    public class ProceduralProfilePetRangePrototype : ProceduralProfileBasicRangePrototype  // V10_NOTE: Older version of ProceduralProfilePetPrototype
    {
        public MoveToContextPrototype PetFollow { get; protected set; }
    }

    public class ProceduralProfileMeleePowerOnHitPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype PowerOnHit { get; protected set; }
    }

    public class ProceduralProfileBotAIPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public SelectEntityContextPrototype SelectTargetItem { get; protected set; }
        public WanderContextPrototype WanderMovement { get; protected set; }
        public ProceduralUsePowerContextPrototype[] SlottedAbilities { get; protected set; }
    }

    #region 1.10 Profiles

    public class ProceduralProfileBasicMelee3PowerPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype Power1 { get; protected set; }
        public ProceduralUsePowerContextPrototype Power2 { get; protected set; }
        public ProceduralUsePowerContextPrototype Power3 { get; protected set; }
    }

    public class ProceduralProfileBasicRange2PowerPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype Power1 { get; protected set; }
        public ProceduralUsePowerContextPrototype Power2 { get; protected set; }
    }

    public class ProceduralProfileBasicRange3PowerPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype Power1 { get; protected set; }
        public ProceduralUsePowerContextPrototype Power2 { get; protected set; }
        public ProceduralUsePowerContextPrototype Power3 { get; protected set; }
    }

    public class ProceduralProfileRangedBuffAllyPrototype : ProceduralProfileWithAttackPrototype
    {
        public PrototypeId FleeOnNoAlliesInRangeOverride { get; protected set; }
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype AllyBuffPower { get; protected set; }
        public SelectEntityContextPrototype AllyBuffSelectTarget { get; protected set; }
    }

    public class ProceduralProfileRangedWithMeleePriority3PowerPrototype : ProceduralProfileRangedWithMeleePriority2PowerPrototype
    {
        public ProceduralUsePowerContextPrototype RangedPower2 { get; protected set; }
    }

    public class ProceduralProfileRangeTargetSwitchPowerPrototype : ProceduralProfileBasicRangePrototype
    {
        public ProceduralUsePowerContextPrototype PowerWithSwitchBeforeIt { get; protected set; }
        public SelectEntityContextPrototype SwitchTargetBeforePower { get; protected set; }
    }

    public class ProceduralProfileMeleeRetargetPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype RetargetPower { get; protected set; }
        public SelectEntityContextPrototype RetargetSelectTarget { get; protected set; }
    }

    public class ProceduralProfileMeleeTargetSwitchPowerPrototype : ProceduralProfileBasicMeleePrototype
    {
        public ProceduralUsePowerContextPrototype PowerWithSwitchBeforeIt { get; protected set; }
        public SelectEntityContextPrototype SwitchTargetBeforePower { get; protected set; }
    }

    #endregion

#if BUILD_1_10_0_69
    public class ProceduralProfileMelee1PwrSwitchTarAfterSpecPrototype : ProceduralProfileWithAttackPrototype
    {
        public MoveToContextPrototype MoveToTarget { get; protected set; }
        public OrbitContextPrototype OrbitTarget { get; protected set; }
        public ProceduralUsePowerContextPrototype SpecialPower { get; protected set; }
        public SelectEntityContextPrototype SwitchTargetAfterSpecial { get; protected set; }
    }

    public class ProceduralProfileMelee2PwrSwitchTarAfterSpecPrototype : ProceduralProfileMelee1PwrSwitchTarAfterSpecPrototype
    {
        public ProceduralUsePowerContextPrototype Power1 { get; protected set; }
    }

    public class ProceduralProfileMelee3PwrSwitchTarAfterSpecPrototype : ProceduralProfileMelee2PwrSwitchTarAfterSpecPrototype
    {
        public ProceduralUsePowerContextPrototype Power2 { get; protected set; }
    }

    public class ProceduralProfileMelee4PwrSwitchTarAfterSpecPrototype : ProceduralProfileMelee3PwrSwitchTarAfterSpecPrototype
    {
        public ProceduralUsePowerContextPrototype Power3 { get; protected set; }
    }

    public class ProceduralProfileMelee5PwrSwitchTarAfterSpecPrototype : ProceduralProfileMelee4PwrSwitchTarAfterSpecPrototype
    {
        public ProceduralUsePowerContextPrototype Power4 { get; protected set; }
    }

    public class ProceduralProfileMelee6PwrSwitchTarAfterSpecPrototype : ProceduralProfileMelee5PwrSwitchTarAfterSpecPrototype
    {
        public ProceduralUsePowerContextPrototype Power5 { get; protected set; }
    }
#endif
}
