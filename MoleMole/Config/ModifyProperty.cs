namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ModifyProperty : ConfigAbilityAction, IHashable
    {
        public DynamicFloat Delta;
        public DynamicFloat Max;
        public DynamicFloat Min;
        public string Property;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.ModifyPropertyHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
        {
            object[] args = new object[] { Miscs.GetDebugActorName(instancedAbility.caster), Miscs.GetDebugActorName(target), this.Property, instancedAbility.Evaluate(this.Delta) };
            output = string.Format("{0} 对 {1} 更改属性 {2}:{3}", args);
            return true;
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.Property, ref lastHash);
            if (this.Delta != null)
            {
                HashUtils.ContentHashOnto(this.Delta.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Delta.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Delta.dynamicKey, ref lastHash);
            }
            if (this.Min != null)
            {
                HashUtils.ContentHashOnto(this.Min.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Min.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Min.dynamicKey, ref lastHash);
            }
            if (this.Max != null)
            {
                HashUtils.ContentHashOnto(this.Max.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Max.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Max.dynamicKey, ref lastHash);
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

