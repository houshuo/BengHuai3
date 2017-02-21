namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityFireAdditionalAttackEffectMixin : BaseAbilityMixin
    {
        private FireAdditionalAttackEffectMixin config;

        public AbilityFireAdditionalAttackEffectMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (FireAdditionalAttackEffectMixin) config;
        }

        private bool OnAttackLanded(EvtAttackLanded evt)
        {
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID);
            if (actor == null)
            {
                return false;
            }
            AttackPattern.ActAttackEffects(this.config.AttackEffect, actor.entity, evt.attackResult.hitCollision.hitPoint, evt.attackResult.hitCollision.hitDir);
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtAttackLanded) && this.OnAttackLanded((EvtAttackLanded) evt));
        }
    }
}

