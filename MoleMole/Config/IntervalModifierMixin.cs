namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class IntervalModifierMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat Interval = DynamicFloat.ZERO;
        public string ModifierName;
        public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityIntervalModifierMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Interval != null)
            {
                HashUtils.ContentHashOnto(this.Interval.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Interval.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Interval.dynamicKey, ref lastHash);
            }
            if (this.Predicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in this.Predicates)
                {
                    if (predicate is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate, ref lastHash);
                    }
                }
            }
            HashUtils.ContentHashOnto(this.ModifierName, ref lastHash);
        }
    }
}

