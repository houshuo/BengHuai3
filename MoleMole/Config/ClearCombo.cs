namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ClearCombo : ConfigAbilityAction, IHashable
    {
        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.ClearComboHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
        {
            output = string.Format("{0} 对 {1} 触发技能:清除combo", Miscs.GetDebugActorName(instancedAbility.caster), Miscs.GetDebugActorName(target));
            return true;
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
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

