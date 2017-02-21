namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class PredicateByHitType : ConfigAbilityAction, IHashable
    {
        public ConfigAbilityAction[] MeleeActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] RangeActions = ConfigAbilityAction.EMPTY;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.PredicateByHitTypeHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.MeleeActions, this.RangeActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.MeleeActions != null)
            {
                foreach (ConfigAbilityAction action in this.MeleeActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.RangeActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.RangeActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
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

