namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class SetAnimatorBool : ConfigAbilityAction, IHashable
    {
        public string BoolID;
        public bool Value;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.SetAnimatorBoolHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
        {
            object[] args = new object[] { Miscs.GetDebugActorName(instancedAbility.caster), Miscs.GetDebugActorName(target), this.BoolID, this.Value };
            output = string.Format("{0} 对 {1} 设置 Animator Bool {2}:{3}", args);
            return true;
        }

        public override MPActorAbilityPlugin.MPAuthorityActionHandler MPGetAuthorityHandler(MPActorAbilityPlugin mpAbilityPlugin)
        {
            return new MPActorAbilityPlugin.MPAuthorityActionHandler(mpAbilityPlugin.SetAnimatorBool_AuthorityHandler);
        }

        public override MPActorAbilityPlugin.MPRemoteActionHandler MPGetRemoteHandler(MPActorAbilityPlugin mpAbilityPlugin)
        {
            return new MPActorAbilityPlugin.MPRemoteActionHandler(mpAbilityPlugin.SetAnimatorBool_RemoteHandler);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.BoolID, ref lastHash);
            HashUtils.ContentHashOnto(this.Value, ref lastHash);
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

