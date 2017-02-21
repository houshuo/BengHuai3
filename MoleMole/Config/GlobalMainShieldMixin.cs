namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class GlobalMainShieldMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat BetweenAttackResumeCD;
        public string ChildShieldModifierName;
        public AbilityState[] ControlledAbilityStates;
        public float ControlledForceResumeTimeRatio = 0.1f;
        public DynamicFloat ForceResumeByDamageHPRatio = DynamicFloat.ZERO;
        public float ForceResumeCD;
        public float ForceResumeRatio;
        public float MinForceResumeCD = 2f;
        public ConfigAbilityAction[] ShieldBrokenActions = ConfigAbilityAction.EMPTY;
        public float ShieldBrokenTimeSlow;
        public DynamicFloat ShieldHP;
        public DynamicFloat ShieldHPRatio;
        public string ShieldOffModifierName;
        public ConfigAbilityAction[] ShieldResumeActions = ConfigAbilityAction.EMPTY;
        public MixinEffect ShieldResumeEffect;
        public float ShieldResumeTimeSpan;
        public float ThrowForceResumeTimeRatio = 0.3f;
        public bool UseLevelTimeScale;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityGlobalMainShieldMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.ShieldResumeActions, this.ShieldBrokenActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.ShieldHPRatio != null)
            {
                HashUtils.ContentHashOnto(this.ShieldHPRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldHPRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldHPRatio.dynamicKey, ref lastHash);
            }
            if (this.ShieldHP != null)
            {
                HashUtils.ContentHashOnto(this.ShieldHP.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldHP.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldHP.dynamicKey, ref lastHash);
            }
            if (this.BetweenAttackResumeCD != null)
            {
                HashUtils.ContentHashOnto(this.BetweenAttackResumeCD.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.BetweenAttackResumeCD.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.BetweenAttackResumeCD.dynamicKey, ref lastHash);
            }
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
            HashUtils.ContentHashOnto(this.ShieldBrokenTimeSlow, ref lastHash);
            if (this.ShieldResumeEffect != null)
            {
                HashUtils.ContentHashOnto(this.ShieldResumeEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldResumeEffect.AudioPattern, ref lastHash);
            }
            if (this.ShieldResumeActions != null)
            {
                foreach (ConfigAbilityAction action in this.ShieldResumeActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.ShieldBrokenActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.ShieldBrokenActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
            HashUtils.ContentHashOnto(this.ChildShieldModifierName, ref lastHash);
            HashUtils.ContentHashOnto(this.ShieldOffModifierName, ref lastHash);
        }
    }
}

