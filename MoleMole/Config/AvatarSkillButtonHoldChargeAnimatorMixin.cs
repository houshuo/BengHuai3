namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarSkillButtonHoldChargeAnimatorMixin : BaseAvatarSkillButtonHoldChargeAnimatorMixin, IHashable
    {
        public float[] ChargeLoopDurations;
        public DynamicFloat ChargeTimeRatio = DynamicFloat.ONE;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarSkillButtonHoldChargeMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.ChargeLoopDurations != null)
            {
                foreach (float num in this.ChargeLoopDurations)
                {
                    HashUtils.ContentHashOnto(num, ref lastHash);
                }
            }
            if (this.ChargeTimeRatio != null)
            {
                HashUtils.ContentHashOnto(this.ChargeTimeRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ChargeTimeRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ChargeTimeRatio.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(base.AllowHoldLockDirection, ref lastHash);
            HashUtils.ContentHashOnto(base.SkillButtonID, ref lastHash);
            HashUtils.ContentHashOnto(base.NextLoopTriggerID, ref lastHash);
            HashUtils.ContentHashOnto(base.AfterSkillTriggerID, ref lastHash);
            if (base.BeforeSkillIDs != null)
            {
                foreach (string str in base.BeforeSkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            if (base.ChargeLoopSkillIDs != null)
            {
                foreach (string str2 in base.ChargeLoopSkillIDs)
                {
                    HashUtils.ContentHashOnto(str2, ref lastHash);
                }
            }
            if (base.AfterSkillIDs != null)
            {
                foreach (string str3 in base.AfterSkillIDs)
                {
                    HashUtils.ContentHashOnto(str3, ref lastHash);
                }
            }
            if (base.TransientSkillIDs != null)
            {
                foreach (string str4 in base.TransientSkillIDs)
                {
                    HashUtils.ContentHashOnto(str4, ref lastHash);
                }
            }
            if (base.ChargeSubTargetAmount != null)
            {
                foreach (int num7 in base.ChargeSubTargetAmount)
                {
                    HashUtils.ContentHashOnto(num7, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(base.SubTargetModifierName, ref lastHash);
            HashUtils.ContentHashOnto(base.ChargeTimeRatioAIKey, ref lastHash);
            if (base.ChargeLoopEffects != null)
            {
                foreach (MixinEffect effect in base.ChargeLoopEffects)
                {
                    HashUtils.ContentHashOnto(effect.EffectPattern, ref lastHash);
                    HashUtils.ContentHashOnto(effect.AudioPattern, ref lastHash);
                }
            }
            if (base.ChargeSwitchEffects != null)
            {
                foreach (MixinEffect effect2 in base.ChargeSwitchEffects)
                {
                    HashUtils.ContentHashOnto(effect2.EffectPattern, ref lastHash);
                    HashUtils.ContentHashOnto(effect2.AudioPattern, ref lastHash);
                }
            }
            if (base.ChargeLoopAudioPatterns != null)
            {
                foreach (string str5 in base.ChargeLoopAudioPatterns)
                {
                    HashUtils.ContentHashOnto(str5, ref lastHash);
                }
            }
            if (base.ChargeSwitchAudioPatterns != null)
            {
                foreach (string str6 in base.ChargeSwitchAudioPatterns)
                {
                    HashUtils.ContentHashOnto(str6, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(base.ImmediatelyDetachLoopEffect, ref lastHash);
            HashUtils.ContentHashOnto(base.ChargeSwitchWindow, ref lastHash);
            HashUtils.ContentHashOnto(base.DisallowReleaseButtonInBS, ref lastHash);
        }
    }
}

