namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ModifyAnimEventAttackMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;
        public DynamicFloat AnimDamageRatioDelta = DynamicFloat.ZERO;
        public string[] AnimEventIDs;
        public DynamicFloat AttackValueDelta = DynamicFloat.ZERO;
        public DynamicFloat CritChanceDelta = DynamicFloat.ZERO;
        public DynamicFloat CritDamageRatioDelta = DynamicFloat.ZERO;
        public DynamicFloat DamagePercentageDelta = DynamicFloat.ZERO;
        public DynamicFloat Dmage = DynamicFloat.ZERO;
        public bool ModifyAllAnimEvents;
        public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;
        public DynamicFloat ShieldDamageDelta = DynamicFloat.ZERO;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityModifyAnimEventAttackMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.AnimEventIDs != null)
            {
                foreach (string str in this.AnimEventIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.ModifyAllAnimEvents, ref lastHash);
            if (this.Dmage != null)
            {
                HashUtils.ContentHashOnto(this.Dmage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Dmage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Dmage.dynamicKey, ref lastHash);
            }
            if (this.AnimDamageRatioDelta != null)
            {
                HashUtils.ContentHashOnto(this.AnimDamageRatioDelta.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AnimDamageRatioDelta.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AnimDamageRatioDelta.dynamicKey, ref lastHash);
            }
            if (this.CritChanceDelta != null)
            {
                HashUtils.ContentHashOnto(this.CritChanceDelta.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.CritChanceDelta.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.CritChanceDelta.dynamicKey, ref lastHash);
            }
            if (this.CritDamageRatioDelta != null)
            {
                HashUtils.ContentHashOnto(this.CritDamageRatioDelta.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.CritDamageRatioDelta.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.CritDamageRatioDelta.dynamicKey, ref lastHash);
            }
            if (this.DamagePercentageDelta != null)
            {
                HashUtils.ContentHashOnto(this.DamagePercentageDelta.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DamagePercentageDelta.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DamagePercentageDelta.dynamicKey, ref lastHash);
            }
            if (this.ShieldDamageDelta != null)
            {
                HashUtils.ContentHashOnto(this.ShieldDamageDelta.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldDamageDelta.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldDamageDelta.dynamicKey, ref lastHash);
            }
            if (this.AttackValueDelta != null)
            {
                HashUtils.ContentHashOnto(this.AttackValueDelta.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackValueDelta.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackValueDelta.dynamicKey, ref lastHash);
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

