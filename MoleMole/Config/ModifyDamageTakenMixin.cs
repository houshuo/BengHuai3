namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ModifyDamageTakenMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;
        public DynamicFloat AddAttackeeAniDefenceRatio = DynamicFloat.ZERO;
        public DynamicFloat DamageTakenDelta = DynamicFloat.ZERO;
        public DynamicFloat DamageTakenRatio = DynamicFloat.ZERO;
        public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityModifyDamageTakenMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.DamageTakenRatio != null)
            {
                HashUtils.ContentHashOnto(this.DamageTakenRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DamageTakenRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DamageTakenRatio.dynamicKey, ref lastHash);
            }
            if (this.DamageTakenDelta != null)
            {
                HashUtils.ContentHashOnto(this.DamageTakenDelta.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DamageTakenDelta.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DamageTakenDelta.dynamicKey, ref lastHash);
            }
            if (this.AddAttackeeAniDefenceRatio != null)
            {
                HashUtils.ContentHashOnto(this.AddAttackeeAniDefenceRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AddAttackeeAniDefenceRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AddAttackeeAniDefenceRatio.dynamicKey, ref lastHash);
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
        }
    }
}

