namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class HealHP : ConfigAbilityAction, IHashable
    {
        public DynamicFloat Amount;
        public DynamicFloat AmountByCasterMaxHPRatio;
        public DynamicFloat AmountByTargetMaxHPRatio;
        public float HealRatio = 1f;
        public bool MuteHealEffect;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.HealHPHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Amount != null)
            {
                HashUtils.ContentHashOnto(this.Amount.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Amount.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Amount.dynamicKey, ref lastHash);
            }
            if (this.AmountByCasterMaxHPRatio != null)
            {
                HashUtils.ContentHashOnto(this.AmountByCasterMaxHPRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AmountByCasterMaxHPRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AmountByCasterMaxHPRatio.dynamicKey, ref lastHash);
            }
            if (this.AmountByTargetMaxHPRatio != null)
            {
                HashUtils.ContentHashOnto(this.AmountByTargetMaxHPRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AmountByTargetMaxHPRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AmountByTargetMaxHPRatio.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.MuteHealEffect, ref lastHash);
            HashUtils.ContentHashOnto(this.HealRatio, ref lastHash);
            HashUtils.ContentHashOnto((int) base.Target, ref lastHash);
            if ((base.TargetOption != null) && (base.TargetOption.Range != null))
            {
                HashUtils.ContentHashOnto(base.TargetOption.Range.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(base.TargetOption.Range.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(base.TargetOption.Range.dynamicKey, ref lastHash);
            }
            if (base.Predicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in base.Predicates)
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

