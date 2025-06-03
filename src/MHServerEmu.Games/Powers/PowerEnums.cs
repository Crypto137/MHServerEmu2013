using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.Powers
{
    [AssetEnum((int)Physical)]
    public enum DamageType
    {
        Physical,
        Energy,
        Mental,
        NumDamageTypes,
        Any,
    }

    [AssetEnum((int)MoveIntoRange)]
    public enum WhenOutOfRangeType
    {
        MoveIntoRange,
        DoNothing,
        ActivateInDirection,
        MoveIfTargetingMOB,
    }

    [AssetEnum((int)None)]
    public enum PowerActivationType
    {
        None,
        Passive,
        Instant,
        InstantRecurring,
        InstantTargeted,
        TwoStageTargeted,
    }

    [AssetEnum((int)None)]
    public enum PowerCategoryType
    {
        None,
        ComboEffect,
        EmotePower,
        GameFunctionPower,
        HiddenPassivePower,
        HotspotEffect,
        ItemPower,
        MissileEffect,
        NormalPower,
        ProcEffect,
        ThrowableCancelPower,
        ThrowablePower,
    }

    [AssetEnum((int)None)]
    public enum PowerEventType
    {
        None,
        OnContactTime,
        OnCriticalHit,
        OnHitKeyword,
        OnPowerApply,
        OnPowerEnd,
        OnPowerEquip,
        OnPowerHit,
        OnPowerStart,
        OnProjectileHit,
        OnStackCount,
        OnTargetKill,
        OnSummonEntity,
        OnHoldBegin,
        OnMissileHit,
        OnMissileKilled,
        OnHotspotOverlapBegin,
        OnHotspotOverlapEnd,
        OnRemoveNegStatusEffect,
        OnPowerToggleOn,
        OnPowerToggleOff,
    }

    [AssetEnum((int)None)]
    public enum PowerEventActionType
    {
        None,
        BodySlide,
        BodySlidePvP,
        BodySlidePvE,
        CancelScheduledActivation,
        ContextCallback,
        DespawnTarget,
        InteractFinish,
        RestoreThrowable,
        Action9,
        ReturnToTown,
        ScheduleActivationAtPercent,
        ScheduleActivationInSeconds,
        ShowBannerMessage,
        SwitchAvatar,
        TransformModeChange,
        TransformModeStart,
        UsePower,
    }

    [AssetEnum((int)None)]
    public enum TargetingShapeType
    {
        None,
        ArcArea,
        BeamSweep,
        CapsuleArea,
        CircleArea,
        RingArea,
        Self,
        SingleTarget,
        SingleTargetRandom,
        SkillShot,
        SkillShotAlongGround,
        WedgeArea,
    }

    [AssetEnum((int)_0)]
    public enum AOEAngleType
    {
        _0,
        _1,
        _10,
        _30,
        _60,
        _90,
        _120,
        _180,
        _240,
        _300,
        _360,
    }

    [AssetEnum((int)All)]
    public enum TargetingHeightType
    {
        All,
        GroundOnly,
        SameHeight,
        FlyingOnly,
    }

    [AssetEnum((int)None)]
    public enum SubsequentActivateType
    {
        None,
        DestroySummonedEntity,
        RepeatActivation,
    }

    [AssetEnum((int)None)]
    public enum TargetRestrictionType
    {
        None,
        HealthGreaterThanPercentage,
        HealthLessThanPercentage,
        EnduranceGreaterThanPercentage,
        EnduranceLessThanPercentage,
        HealthOrEnduranceGreaterThanPercentage,
        HealthOrEnduranceLessThanPercentage,
        HasKeyword,
        DoesNotHaveKeyword,
        HasAI,
        IsPrototypeOf,
    }

    [AssetEnum((int)SingleStack)]
    public enum StackingApplicationStyleType
    {
        SingleStack,
        SelfAndAlliesInRadius,
        SingleStackRecreateSameType,
        SingleStackRefreshSameType,
        SingleStackSameTypeAddDuration,
    }

    [AssetEnum((int)DoNothing)]
    public enum StackingActionType
    {
        DoNothing,
        DoNotStackMore,
        RemoveAllDoNotStackMore
    }

    [AssetEnum((int)None)]
    public enum ProcTriggerType
    {
        None,
        OnAnyHit,
        OnAnyHitForPctHealth,
        OnAnyHitTargetHealthBelowPct,
        OnBlock,
        OnCollide,
        OnCollideEntity,
        OnCollideWorldGeo,
        OnConditionEnd,
        OnCrit,
        OnGotHitByCrit,
        OnDeath,
        OnDodge,
        OnEnduranceBelow,
        OnGotAttacked,
        OnGotHit,
        OnGotHitPriorResist,
        OnGotHitContact,
        OnGotHitEnergy,
        OnGotHitEnergyPriorResist,
        OnGotHitForPctHealth,
        OnGotHitHealthBelowPct,
        OnGotHitMental,
        OnGotHitMentalPriorResist,
        OnGotHitPhysical,
        OnGotHitPhysicalPriorResist,
        OnHealthAbove,
        OnHealthAboveToggle,
        OnHealthBelow,
        OnHealthBelowToggle,
        OnInteractedWith,
        OnInteractedWithOutOfUses,
        OnKillOther,
        OnKnockdownEnd,
        OnLifespanExpired,
        OnLootPickup,
        OnMissileAbsorbed,
        OnNegStatusApplied,
        OnOrbPickup,
        OnOverlapBegin,
        OnPowerHit,
        OnPowerHitEnergy,
        OnPowerHitMental,
        OnPowerHitPhysical,
        OnPowerUseConsumable,
        OnPowerUseNormal,
        OnSecondaryResourceEmpty,
        OnSecondaryResourcePipGain,
        OnSecondaryResourcePipLoss,
        OnSecondaryResourcePipMax,
        OnSecondaryResourcePipZero,
        OnSkillshotReflect,
        OnSummonPet,
        OnMissileHit,

        // V10_NOTE: These values are in the asset type, but do not have entries in the lookup table
        OnHotspotOverlapBegin,
        OnHotspotOverlapEnd,
    }
}
