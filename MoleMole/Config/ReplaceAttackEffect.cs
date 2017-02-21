namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ReplaceAttackEffect : ConfigAbilityAction, IHashable
    {
        public ConfigEntityAttackEffect AttackEffect;
        public ConfigEntityAttackEffect BeHitEffect;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.ReplaceAttackEffectHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.AttackEffect != null)
            {
                HashUtils.ContentHashOnto(this.AttackEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackEffect.SwitchName, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackEffect.MuteAttackEffect, ref lastHash);
                HashUtils.ContentHashOnto((int) this.AttackEffect.AttackEffectTriggerPos, ref lastHash);
            }
            if (this.BeHitEffect != null)
            {
                HashUtils.ContentHashOnto(this.BeHitEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.BeHitEffect.SwitchName, ref lastHash);
                HashUtils.ContentHashOnto(this.BeHitEffect.MuteAttackEffect, ref lastHash);
                HashUtils.ContentHashOnto((int) this.BeHitEffect.AttackEffectTriggerPos, ref lastHash);
            }
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

