namespace MoleMole.Config
{
    using System;

    public abstract class BaseAvatarSkillButtonHoldChargeAnimatorMixin : ConfigAbilityMixin
    {
        public string[] AfterSkillIDs;
        public string AfterSkillTriggerID;
        public bool AllowHoldLockDirection;
        public string[] BeforeSkillIDs;
        public string[] ChargeLoopAudioPatterns;
        public MixinEffect[] ChargeLoopEffects;
        public string[] ChargeLoopSkillIDs;
        public int[] ChargeSubTargetAmount;
        public string[] ChargeSwitchAudioPatterns;
        public MixinEffect[] ChargeSwitchEffects;
        public float ChargeSwitchWindow = 0.4f;
        public string ChargeTimeRatioAIKey;
        public bool DisallowReleaseButtonInBS;
        public bool ImmediatelyDetachLoopEffect;
        public string NextLoopTriggerID;
        public string SkillButtonID;
        public string SubTargetModifierName;
        public string[] TransientSkillIDs = Miscs.EMPTY_STRINGS;

        protected BaseAvatarSkillButtonHoldChargeAnimatorMixin()
        {
        }
    }
}

