namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class SmokeBombMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat Duration = DynamicFloat.ZERO;
        public string[] InSmokeModifiers;
        public string[] Modifiers;
        public DynamicFloat Radius = DynamicFloat.ONE;
        public MixinEffect SmokeOffEffect;
        public MixinEffect SmokeOnEffect;
        public MixinTargetting Target = MixinTargetting.Allied;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilitySmokeBombMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Radius != null)
            {
                HashUtils.ContentHashOnto(this.Radius.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Radius.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Radius.dynamicKey, ref lastHash);
            }
            if (this.Duration != null)
            {
                HashUtils.ContentHashOnto(this.Duration.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Duration.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Duration.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto((int) this.Target, ref lastHash);
            if (this.SmokeOnEffect != null)
            {
                HashUtils.ContentHashOnto(this.SmokeOnEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.SmokeOnEffect.AudioPattern, ref lastHash);
            }
            if (this.SmokeOffEffect != null)
            {
                HashUtils.ContentHashOnto(this.SmokeOffEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.SmokeOffEffect.AudioPattern, ref lastHash);
            }
            if (this.Modifiers != null)
            {
                foreach (string str in this.Modifiers)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            if (this.InSmokeModifiers != null)
            {
                foreach (string str2 in this.InSmokeModifiers)
                {
                    HashUtils.ContentHashOnto(str2, ref lastHash);
                }
            }
        }
    }
}

