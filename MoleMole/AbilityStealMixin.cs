namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityStealMixin : BaseAbilityMixin
    {
        private StealHPMixin config;

        public AbilityStealMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (StealHPMixin) config;
        }

        public override void OnAdded()
        {
        }

        private bool OnHittingOther(EvtHittingOther evt)
        {
            float amount = evt.attackData.attackerAttackValue * base.instancedAbility.Evaluate(this.config.HPStealRatio);
            base.actor.HealHP(amount);
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtHittingOther) && this.OnHittingOther((EvtHittingOther) evt));
        }
    }
}

