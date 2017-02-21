namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class BlackHoleMixin : ConfigAbilityMixin, IHashable
    {
        public bool ApplyAttackerWitchTimeRatio;
        public MixinEffect CreationEffect;
        public DynamicFloat CreationZOffset;
        public MixinEffect DestroyEffect;
        public DynamicFloat Duration;
        public string ExplodeAnimEventID;
        public string[] ModifierNames;
        public DynamicFloat PullVelocity;
        public DynamicFloat Radius;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityBlackHoleMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.CreationZOffset != null)
            {
                HashUtils.ContentHashOnto(this.CreationZOffset.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.CreationZOffset.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.CreationZOffset.dynamicKey, ref lastHash);
            }
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
            HashUtils.ContentHashOnto(this.ApplyAttackerWitchTimeRatio, ref lastHash);
            if (this.PullVelocity != null)
            {
                HashUtils.ContentHashOnto(this.PullVelocity.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.PullVelocity.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.PullVelocity.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.ExplodeAnimEventID, ref lastHash);
            if (this.CreationEffect != null)
            {
                HashUtils.ContentHashOnto(this.CreationEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.CreationEffect.AudioPattern, ref lastHash);
            }
            if (this.DestroyEffect != null)
            {
                HashUtils.ContentHashOnto(this.DestroyEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.DestroyEffect.AudioPattern, ref lastHash);
            }
            if (this.ModifierNames != null)
            {
                foreach (string str in this.ModifierNames)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
        }
    }
}

