namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;

    public class ConfigAvatarSkill
    {
        public string AnimatorEventPattern;
        public string[] AnimatorStateNames;
        public float AnimDefenceNormalizedTimeStart;
        public float AnimDefenceNormalizedTimeStop = 1f;
        public float AnimDefenceRatio;
        public float AttackNormalizedTimeStart;
        public float AttackNormalizedTimeStop;
        public float BranchHighlightNormalizedTimeStart;
        public float BranchHighlightNormalizedTimeStop;
        public bool CanHold;
        [NonSerialized]
        public int ChargesCount;
        public DynamicInt ChargesCountDelta = DynamicInt.ZERO;
        public float ComboTimerPauseNormalizedTimeStart;
        public float ComboTimerPauseNormalizedTimeStop;
        public SkillEnterSetting EnterSteer;
        [InspectorNullable]
        public SkillEnterSteerOption EnterSteerOption;
        public bool ForceMuteSteer;
        public bool HaveBranch;
        public bool HighSpeedMovement;
        public string InstantTriggerEvent;
        public bool IsInstantTrigger;
        public string LastKillCameraAnimation;
        public float MassRatio = 1f;
        public bool MuteCameraControl;
        public bool MuteHighlighted;
        public bool NeedClearEffect;
        public ReviveSkillCDAction ReviveCDAction;
        [InspectorNullable]
        public AttackResult.AttackCategoryTag[] SkillCategoryTag;
        [NonSerialized]
        public float SkillCD;
        public DynamicFloat SkillCDDelta = DynamicFloat.ZERO;
        public AvatarSkillType SkillType;
        [NonSerialized]
        public float SPCost;
        public DynamicFloat SPCostDelta = DynamicFloat.ZERO;
        [NonSerialized]
        public float SPNeed;
    }
}

