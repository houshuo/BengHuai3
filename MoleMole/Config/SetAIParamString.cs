namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class SetAIParamString : ConfigAbilityAction, IHashable
    {
        public DynamicString Param;
        public DynamicString Value;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.SetAIParamStringHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Param != null)
            {
                HashUtils.ContentHashOnto(this.Param.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Param.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Param.dynamicKey, ref lastHash);
            }
            if (this.Value != null)
            {
                HashUtils.ContentHashOnto(this.Value.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Value.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Value.dynamicKey, ref lastHash);
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

