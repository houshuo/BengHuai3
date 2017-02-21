namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityModifyDamageByAttackeeMixin : BaseAbilityMixin
    {
        private ModifyDamageByAttackeeMixin config;

        public AbilityModifyDamageByAttackeeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (ModifyDamageByAttackeeMixin) config;
        }

        protected virtual bool OnBeingHit(EvtBeingHit evt)
        {
            if (!base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt))
            {
                return false;
            }
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.Actions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
            evt.attackData.attackeeAddedDamageTakeRatio += base.instancedAbility.Evaluate(this.config.AddedDamageTakeRatio);
            return true;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnBeingHit((EvtBeingHit) evt));
        }
    }
}

