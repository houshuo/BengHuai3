namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class Retargeted : BaseUtilityAction, IHashable
    {
        public bool IgnoreSelf;
        public bool RandomedTarget;
        public AbilityTargetting Retarget;
        public ConfigAbilityAction[] RetargetedActions = ConfigAbilityAction.EMPTY;
        public TargettingOption RetargetOption;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.RetargetedHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.RetargetedActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto((int) this.Retarget, ref lastHash);
            if ((this.RetargetOption != null) && (this.RetargetOption.Range != null))
            {
                HashUtils.ContentHashOnto(this.RetargetOption.Range.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.RetargetOption.Range.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.RetargetOption.Range.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.RandomedTarget, ref lastHash);
            HashUtils.ContentHashOnto(this.IgnoreSelf, ref lastHash);
            if (this.RetargetedActions != null)
            {
                foreach (ConfigAbilityAction action in this.RetargetedActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
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

