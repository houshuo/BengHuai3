namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ShieldMixin : ConfigAbilityMixin, IHashable
    {
        public float AniDamagePower = 1f;
        public float BetweenAttackResumeCD;
        public float BetweenAttackResumeRatio;
        public AbilityState[] ControlledAbilityStates;
        public float ControlledForceResumeTimeRatio = 0.1f;
        public float DamagePower = 1f;
        public DynamicFloat ForceResumeByDamageHPRatio = DynamicFloat.ZERO;
        public float ForceResumeCD;
        public float ForceResumeRatio;
        public float MinForceResumeCD = 2f;
        public bool MuteHitEffect = true;
        public ConfigAbilityAction[] ShiedlBrokenActions = ConfigAbilityAction.EMPTY;
        public MixinEffect ShieldBrokenEffect;
        public float ShieldBrokenTimeSlow;
        public float ShieldDamagePower = 1f;
        public float ShieldDisplayRatioCeiling = 1f;
        public float ShieldDisplayRatioFloor;
        public DynamicFloat ShieldHPRatio;
        public string ShieldOffModifierName;
        public string ShieldOnModifierName;
        public float ShieldResumeTimeSpan;
        public ConfigAbilityAction[] ShieldSuccessActions = ConfigAbilityAction.EMPTY;
        public int ShieldSuccessAddFrameHalt;
        public ConfigEntityAttackEffect ShieldSuccessEffect;
        public DynamicInt ShowShieldBar;
        public float ThrowForceResumeTimeRatio = 0.3f;
        public bool UseLevelTimeScale;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityShieldMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.ShieldSuccessActions, this.ShiedlBrokenActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.ShieldHPRatio != null)
            {
                HashUtils.ContentHashOnto(this.ShieldHPRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldHPRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldHPRatio.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.BetweenAttackResumeCD, ref lastHash);
            HashUtils.ContentHashOnto(this.BetweenAttackResumeRatio, ref lastHash);
            HashUtils.ContentHashOnto(this.ForceResumeCD, ref lastHash);
            HashUtils.ContentHashOnto(this.MinForceResumeCD, ref lastHash);
            HashUtils.ContentHashOnto(this.UseLevelTimeScale, ref lastHash);
            HashUtils.ContentHashOnto(this.ForceResumeRatio, ref lastHash);
            HashUtils.ContentHashOnto(this.ThrowForceResumeTimeRatio, ref lastHash);
            HashUtils.ContentHashOnto(this.ControlledForceResumeTimeRatio, ref lastHash);
            if (this.ControlledAbilityStates != null)
            {
                foreach (AbilityState state in this.ControlledAbilityStates)
                {
                    HashUtils.ContentHashOnto((int) state, ref lastHash);
                }
            }
            if (this.ForceResumeByDamageHPRatio != null)
            {
                HashUtils.ContentHashOnto(this.ForceResumeByDamageHPRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ForceResumeByDamageHPRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ForceResumeByDamageHPRatio.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.ShieldResumeTimeSpan, ref lastHash);
            if (this.ShowShieldBar != null)
            {
                HashUtils.ContentHashOnto(this.ShowShieldBar.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ShowShieldBar.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ShowShieldBar.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.ShieldDisplayRatioFloor, ref lastHash);
            HashUtils.ContentHashOnto(this.ShieldDisplayRatioCeiling, ref lastHash);
            HashUtils.ContentHashOnto(this.DamagePower, ref lastHash);
            HashUtils.ContentHashOnto(this.AniDamagePower, ref lastHash);
            HashUtils.ContentHashOnto(this.ShieldDamagePower, ref lastHash);
            if (this.ShieldSuccessEffect != null)
            {
                HashUtils.ContentHashOnto(this.ShieldSuccessEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldSuccessEffect.SwitchName, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldSuccessEffect.MuteAttackEffect, ref lastHash);
                HashUtils.ContentHashOnto((int) this.ShieldSuccessEffect.AttackEffectTriggerPos, ref lastHash);
            }
            if (this.ShieldSuccessActions != null)
            {
                foreach (ConfigAbilityAction action in this.ShieldSuccessActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            HashUtils.ContentHashOnto(this.ShieldBrokenTimeSlow, ref lastHash);
            if (this.ShieldBrokenEffect != null)
            {
                HashUtils.ContentHashOnto(this.ShieldBrokenEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldBrokenEffect.AudioPattern, ref lastHash);
            }
            if (this.ShiedlBrokenActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.ShiedlBrokenActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
            HashUtils.ContentHashOnto(this.ShieldSuccessAddFrameHalt, ref lastHash);
            HashUtils.ContentHashOnto(this.MuteHitEffect, ref lastHash);
            HashUtils.ContentHashOnto(this.ShieldOnModifierName, ref lastHash);
            HashUtils.ContentHashOnto(this.ShieldOffModifierName, ref lastHash);
        }
    }
}

