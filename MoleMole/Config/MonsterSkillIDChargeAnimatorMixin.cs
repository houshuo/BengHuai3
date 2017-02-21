namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MonsterSkillIDChargeAnimatorMixin : ConfigAbilityMixin, IHashable
    {
        public string[] AfterSkillIDs;
        public string AfterSkillTriggerID;
        public string[] BeforeSkillIDs;
        public string[] ChargeLoopAudioPatterns;
        public float[] ChargeLoopDurations;
        public MixinEffect[] ChargeLoopEffects;
        public string[] ChargeLoopSkillIDs;
        public string[] ChargeSwitchAudioPatterns;
        public MixinEffect[] ChargeSwitchEffects;
        public float ChargeSwitchWindow = 0.4f;
        public DynamicFloat ChargeTimeRatio = DynamicFloat.ONE;
        public string NextLoopTriggerID;
        public string[] TransientSkillIDs = Miscs.EMPTY_STRINGS;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterSkillIDChargeAnimatorMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.NextLoopTriggerID, ref lastHash);
            HashUtils.ContentHashOnto(this.AfterSkillTriggerID, ref lastHash);
            if (this.BeforeSkillIDs != null)
            {
                foreach (string str in this.BeforeSkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            if (this.ChargeLoopSkillIDs != null)
            {
                foreach (string str2 in this.ChargeLoopSkillIDs)
                {
                    HashUtils.ContentHashOnto(str2, ref lastHash);
                }
            }
            if (this.AfterSkillIDs != null)
            {
                foreach (string str3 in this.AfterSkillIDs)
                {
                    HashUtils.ContentHashOnto(str3, ref lastHash);
                }
            }
            if (this.TransientSkillIDs != null)
            {
                foreach (string str4 in this.TransientSkillIDs)
                {
                    HashUtils.ContentHashOnto(str4, ref lastHash);
                }
            }
            if (this.ChargeLoopDurations != null)
            {
                foreach (float num5 in this.ChargeLoopDurations)
                {
                    HashUtils.ContentHashOnto(num5, ref lastHash);
                }
            }
            if (this.ChargeTimeRatio != null)
            {
                HashUtils.ContentHashOnto(this.ChargeTimeRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ChargeTimeRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ChargeTimeRatio.dynamicKey, ref lastHash);
            }
            if (this.ChargeLoopEffects != null)
            {
                foreach (MixinEffect effect in this.ChargeLoopEffects)
                {
                    HashUtils.ContentHashOnto(effect.EffectPattern, ref lastHash);
                    HashUtils.ContentHashOnto(effect.AudioPattern, ref lastHash);
                }
            }
            if (this.ChargeSwitchEffects != null)
            {
                foreach (MixinEffect effect2 in this.ChargeSwitchEffects)
                {
                    HashUtils.ContentHashOnto(effect2.EffectPattern, ref lastHash);
                    HashUtils.ContentHashOnto(effect2.AudioPattern, ref lastHash);
                }
            }
            if (this.ChargeLoopAudioPatterns != null)
            {
                foreach (string str5 in this.ChargeLoopAudioPatterns)
                {
                    HashUtils.ContentHashOnto(str5, ref lastHash);
                }
            }
            if (this.ChargeSwitchAudioPatterns != null)
            {
                foreach (string str6 in this.ChargeSwitchAudioPatterns)
                {
                    HashUtils.ContentHashOnto(str6, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.ChargeSwitchWindow, ref lastHash);
        }
    }
}

