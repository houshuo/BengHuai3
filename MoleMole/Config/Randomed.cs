namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class Randomed : BaseUtilityAction, IHashable
    {
        public DynamicFloat Chance;
        public ConfigAbilityAction[] FailActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] SuccessActions = ConfigAbilityAction.EMPTY;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.RandomedHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.SuccessActions, this.FailActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Chance != null)
            {
                HashUtils.ContentHashOnto(this.Chance.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Chance.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Chance.dynamicKey, ref lastHash);
            }
            if (this.SuccessActions != null)
            {
                foreach (ConfigAbilityAction action in this.SuccessActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.FailActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.FailActions)
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

        public override void OnLoaded()
        {
        }
    }
}

