namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class HPThresholdMixin : ConfigAbilityMixin, IHashable
    {
        public string ModifierName;
        public MixinPredicate Predicate;
        public DynamicFloat Threshold;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityHPThresholdMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Threshold != null)
            {
                HashUtils.ContentHashOnto(this.Threshold.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Threshold.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Threshold.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto((int) this.Predicate, ref lastHash);
            HashUtils.ContentHashOnto(this.ModifierName, ref lastHash);
        }
    }
}

