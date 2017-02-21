namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityModifyAttackData : BaseAbilityMixin
    {
        private ModifyAttackData config;

        public AbilityModifyAttackData(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (ModifyAttackData) config;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtHittingOther) && this.OnPostHittingOther((EvtHittingOther) evt));
        }

        private bool OnPostHittingOther(EvtHittingOther evt)
        {
            if (base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt) && this.config.NoTriggerEvadeAndDefend)
            {
                evt.attackData.noTriggerEvadeAndDefend = this.config.NoTriggerEvadeAndDefend;
            }
            return false;
        }
    }
}

