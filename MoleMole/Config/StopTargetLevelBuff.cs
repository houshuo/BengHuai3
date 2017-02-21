namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class StopTargetLevelBuff : ConfigAbilityAction, IHashable
    {
        public LevelBuffType LevelBuff;
        public bool stopOtherSide;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.StopTargetLevelBuffHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
        {
            output = string.Format("{0} 停止 LevelBuff {1}, 停止对面 side: {2}", Miscs.GetDebugActorName(instancedAbility.caster), this.LevelBuff, this.stopOtherSide);
            return true;
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto((int) this.LevelBuff, ref lastHash);
            HashUtils.ContentHashOnto(this.stopOtherSide, ref lastHash);
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

