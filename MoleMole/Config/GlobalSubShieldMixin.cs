namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class GlobalSubShieldMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] ShieldBrokenActions = ConfigAbilityAction.EMPTY;
        public MixinEffect ShieldBrokenEffect;
        public float ShieldBrokenTimeSlow;
        public DynamicFloat ShieldDefenceRatio;
        public float[] ShieldEffectRanges;
        public MixinEffect[] ShieldEffects;
        public string ShieldOffModifierName;
        public ConfigAbilityAction[] ShieldSuccessActions = ConfigAbilityAction.EMPTY;
        public int ShieldSuccessAddFrameHalt;
        public MixinEffect ShieldSuccessEffect;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityGlobalSubShieldMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.ShieldSuccessActions, this.ShieldBrokenActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.ShieldEffectRanges != null)
            {
                foreach (float num in this.ShieldEffectRanges)
                {
                    HashUtils.ContentHashOnto(num, ref lastHash);
                }
            }
            if (this.ShieldEffects != null)
            {
                foreach (MixinEffect effect in this.ShieldEffects)
                {
                    HashUtils.ContentHashOnto(effect.EffectPattern, ref lastHash);
                    HashUtils.ContentHashOnto(effect.AudioPattern, ref lastHash);
                }
            }
            if (this.ShieldSuccessEffect != null)
            {
                HashUtils.ContentHashOnto(this.ShieldSuccessEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldSuccessEffect.AudioPattern, ref lastHash);
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
            HashUtils.ContentHashOnto(this.ShieldSuccessAddFrameHalt, ref lastHash);
            HashUtils.ContentHashOnto(this.ShieldOffModifierName, ref lastHash);
            if (this.ShieldDefenceRatio != null)
            {
                HashUtils.ContentHashOnto(this.ShieldDefenceRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldDefenceRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldDefenceRatio.dynamicKey, ref lastHash);
            }
        }
    }
}

