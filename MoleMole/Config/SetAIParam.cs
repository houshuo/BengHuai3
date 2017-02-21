namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class SetAIParam : ConfigAbilityAction, IHashable
    {
        public bool IsBool;
        public ParamLogicType LogicType;
        public DynamicString Param;
        public float Value;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.SetAIParamHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
        {
            object[] args = new object[] { Miscs.GetDebugActorName(instancedAbility.caster), Miscs.GetDebugActorName(target), this.Param, this.Value, this.LogicType };
            output = string.Format("{0} 对 {1} 设置 AI 参数 {2}:{3}:{4}", args);
            return true;
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Param != null)
            {
                HashUtils.ContentHashOnto(this.Param.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Param.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Param.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.Value, ref lastHash);
            HashUtils.ContentHashOnto(this.IsBool, ref lastHash);
            HashUtils.ContentHashOnto((int) this.LogicType, ref lastHash);
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

