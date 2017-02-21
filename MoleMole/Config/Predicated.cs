namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class Predicated : BaseUtilityAction, IHashable
    {
        public ConfigAbilityAction[] FailActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] SuccessActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityPredicate[] TargetPredicates = ConfigAbilityPredicate.EMPTY;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.PredicatedHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.SuccessActions, this.FailActions };
        }

        public override MPActorAbilityPlugin.MPAuthorityActionHandler MPGetAuthorityHandler(MPActorAbilityPlugin mpAbilityPlugin)
        {
            return new MPActorAbilityPlugin.MPAuthorityActionHandler(mpAbilityPlugin.Predicated_AuthorityHandler);
        }

        public override MPActorAbilityPlugin.MPRemoteActionHandler MPGetRemoteHandler(MPActorAbilityPlugin mpAbilityPlugin)
        {
            return new MPActorAbilityPlugin.MPRemoteActionHandler(MPActorAbilityPlugin.STUB_RemoteMute);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.TargetPredicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in this.TargetPredicates)
                {
                    if (predicate is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate, ref lastHash);
                    }
                }
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
                foreach (ConfigAbilityPredicate predicate2 in base.Predicates)
                {
                    if (predicate2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate2, ref lastHash);
                    }
                }
            }
        }
    }
}

