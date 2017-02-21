namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DistanceBeyondMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;
        public DynamicFloat AniDamageRatioUp = DynamicFloat.ZERO;
        public DynamicFloat AttackRatio = DynamicFloat.ZERO;
        public DynamicFloat CriticalChanceRatioUp = DynamicFloat.ZERO;
        public DynamicFloat CriticalDamageRatioUp = DynamicFloat.ZERO;
        public DynamicFloat DamagePercentageUp = DynamicFloat.ZERO;
        public DynamicFloat Distance = DynamicFloat.ZERO;
        public DynamicFloat HitDistanceBeyond = DynamicFloat.ZERO;
        public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;
        public bool Reverse;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityDistanceBeyondMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.HitDistanceBeyond != null)
            {
                HashUtils.ContentHashOnto(this.HitDistanceBeyond.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.HitDistanceBeyond.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.HitDistanceBeyond.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.Reverse, ref lastHash);
            if (this.Distance != null)
            {
                HashUtils.ContentHashOnto(this.Distance.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Distance.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Distance.dynamicKey, ref lastHash);
            }
            if (this.DamagePercentageUp != null)
            {
                HashUtils.ContentHashOnto(this.DamagePercentageUp.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DamagePercentageUp.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DamagePercentageUp.dynamicKey, ref lastHash);
            }
            if (this.AttackRatio != null)
            {
                HashUtils.ContentHashOnto(this.AttackRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackRatio.dynamicKey, ref lastHash);
            }
            if (this.AniDamageRatioUp != null)
            {
                HashUtils.ContentHashOnto(this.AniDamageRatioUp.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AniDamageRatioUp.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AniDamageRatioUp.dynamicKey, ref lastHash);
            }
            if (this.CriticalChanceRatioUp != null)
            {
                HashUtils.ContentHashOnto(this.CriticalChanceRatioUp.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.CriticalChanceRatioUp.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.CriticalChanceRatioUp.dynamicKey, ref lastHash);
            }
            if (this.CriticalDamageRatioUp != null)
            {
                HashUtils.ContentHashOnto(this.CriticalDamageRatioUp.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.CriticalDamageRatioUp.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.CriticalDamageRatioUp.dynamicKey, ref lastHash);
            }
            if (this.Actions != null)
            {
                foreach (ConfigAbilityAction action in this.Actions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
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
        }
    }
}

