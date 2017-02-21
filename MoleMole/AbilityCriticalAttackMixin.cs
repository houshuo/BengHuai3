namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityCriticalAttackMixin : BaseAbilityMixin
    {
        private CriticalAttackMixin config;

        public AbilityCriticalAttackMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (CriticalAttackMixin) config;
        }

        private bool OnAttackLanded(EvtAttackLanded evt)
        {
            if (evt.attackResult.hitLevel == AttackResult.ActorHitLevel.Critical)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.OnCriticalAttackActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID), evt);
            }
            return true;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtAttackLanded) && this.OnAttackLanded((EvtAttackLanded) evt));
        }
    }
}

