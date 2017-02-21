namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class RemoveModifier : ConfigAbilityAction, IHashable
    {
        public string ModifierName;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.RemoveModifierHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override MPActorAbilityPlugin.MPAuthorityActionHandler MPGetAuthorityHandler(MPActorAbilityPlugin mpAbilityPlugin)
        {
            return new MPActorAbilityPlugin.MPAuthorityActionHandler(mpAbilityPlugin.RemoveModifier_AuthorityHandler);
        }

        public override MPActorAbilityPlugin.MPRemoteActionHandler MPGetRemoteHandler(MPActorAbilityPlugin mpAbilityPlugin)
        {
            return new MPActorAbilityPlugin.MPRemoteActionHandler(mpAbilityPlugin.RemoveModifier_RemoteHandler);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.ModifierName, ref lastHash);
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

