namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ModifyDamageMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;
        public DynamicFloat AddedAttackRatio = DynamicFloat.ZERO;
        public DynamicFloat AddedDamagePercentage = DynamicFloat.ZERO;
        public DynamicFloat AddedDamageRatio = DynamicFloat.ZERO;
        public DynamicFloat AddedDamageValue = DynamicFloat.ZERO;
        public DynamicFloat AllDamageReduceRatio = DynamicFloat.ZERO;
        public DynamicFloat AllienDamage = DynamicFloat.ZERO;
        public DynamicFloat AllienDamagePercentage = DynamicFloat.ZERO;
        public DynamicFloat AniDamageRatio = DynamicFloat.ZERO;
        public DynamicFloat AnimDamageRatioDelta = DynamicFloat.ZERO;
        public ConfigAbilityAction[] BreakActions = ConfigAbilityAction.EMPTY;
        public DynamicFloat CritChanceDelta = DynamicFloat.ZERO;
        public DynamicFloat CritDamageRatioDelta = DynamicFloat.ZERO;
        public DynamicFloat FireDamage = DynamicFloat.ZERO;
        public DynamicFloat FireDamagePercentage = DynamicFloat.ZERO;
        public DynamicFloat IceDamage = DynamicFloat.ZERO;
        public DynamicFloat IceDamagePercentage = DynamicFloat.ZERO;
        public bool IncludeNonAnimEventAttacks;
        public DynamicFloat ModifyChance = DynamicFloat.ONE;
        public DynamicFloat NormalDamage = DynamicFloat.ZERO;
        public DynamicFloat NormalDamagePercentage = DynamicFloat.ZERO;
        public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;
        public DynamicFloat ThunderDamage = DynamicFloat.ZERO;
        public DynamicFloat ThunderDamagePercentage = DynamicFloat.ZERO;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityModifiyDamageMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
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
            if (this.AddedAttackRatio != null)
            {
                HashUtils.ContentHashOnto(this.AddedAttackRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AddedAttackRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AddedAttackRatio.dynamicKey, ref lastHash);
            }
            if (this.AddedDamageValue != null)
            {
                HashUtils.ContentHashOnto(this.AddedDamageValue.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AddedDamageValue.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AddedDamageValue.dynamicKey, ref lastHash);
            }
            if (this.AddedDamageRatio != null)
            {
                HashUtils.ContentHashOnto(this.AddedDamageRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AddedDamageRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AddedDamageRatio.dynamicKey, ref lastHash);
            }
            if (this.AddedDamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.AddedDamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AddedDamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AddedDamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.AllDamageReduceRatio != null)
            {
                HashUtils.ContentHashOnto(this.AllDamageReduceRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AllDamageReduceRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AllDamageReduceRatio.dynamicKey, ref lastHash);
            }
            if (this.NormalDamage != null)
            {
                HashUtils.ContentHashOnto(this.NormalDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.NormalDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.NormalDamage.dynamicKey, ref lastHash);
            }
            if (this.FireDamage != null)
            {
                HashUtils.ContentHashOnto(this.FireDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.FireDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.FireDamage.dynamicKey, ref lastHash);
            }
            if (this.ThunderDamage != null)
            {
                HashUtils.ContentHashOnto(this.ThunderDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ThunderDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ThunderDamage.dynamicKey, ref lastHash);
            }
            if (this.IceDamage != null)
            {
                HashUtils.ContentHashOnto(this.IceDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.IceDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.IceDamage.dynamicKey, ref lastHash);
            }
            if (this.AllienDamage != null)
            {
                HashUtils.ContentHashOnto(this.AllienDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AllienDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AllienDamage.dynamicKey, ref lastHash);
            }
            if (this.NormalDamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.NormalDamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.NormalDamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.NormalDamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.FireDamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.FireDamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.FireDamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.FireDamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.ThunderDamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.ThunderDamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ThunderDamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ThunderDamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.IceDamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.IceDamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.IceDamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.IceDamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.AllienDamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.AllienDamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AllienDamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AllienDamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.AniDamageRatio != null)
            {
                HashUtils.ContentHashOnto(this.AniDamageRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AniDamageRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AniDamageRatio.dynamicKey, ref lastHash);
            }
            if (this.ModifyChance != null)
            {
                HashUtils.ContentHashOnto(this.ModifyChance.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ModifyChance.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ModifyChance.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.IncludeNonAnimEventAttacks, ref lastHash);
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
            if (this.BreakActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.BreakActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
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

