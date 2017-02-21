namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;

    public class AbilityModifyDamageTakenMixin : BaseAbilityMixin
    {
        private ModifyDamageTakenMixin config;

        public AbilityModifyDamageTakenMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (ModifyDamageTakenMixin) config;
        }

        protected virtual void ModifyDamage(EvtBeingHit evt, float multiple = 1)
        {
            evt.attackData.damage = (evt.attackData.damage * (1f + (base.instancedAbility.Evaluate(this.config.DamageTakenRatio) * multiple))) + (base.instancedAbility.Evaluate(this.config.DamageTakenDelta) * multiple);
            if (evt.attackData.damage < 0f)
            {
                evt.attackData.damage = 0f;
            }
        }

        protected virtual bool OnPostBeingHit(EvtBeingHit evt)
        {
            if (!base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt))
            {
                return false;
            }
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.Actions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
            this.ModifyDamage(evt, 1f);
            evt.attackData.attackeeAniDefenceRatio += base.instancedAbility.Evaluate(this.config.AddAttackeeAniDefenceRatio);
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnPostBeingHit((EvtBeingHit) evt));
        }
    }
}

