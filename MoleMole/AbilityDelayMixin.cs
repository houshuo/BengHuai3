namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityDelayMixin : BaseAbilityMixin
    {
        private EntityTimer _waitTimer;
        private DelayMixin config;

        public AbilityDelayMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (DelayMixin) config;
            this._waitTimer = new EntityTimer(instancedAbility.Evaluate(this.config.Delay));
        }

        public override void Core()
        {
            this._waitTimer.Core(1f);
            if (this._waitTimer.isTimeUp)
            {
                this.OnTimeUp();
            }
        }

        public override void OnAdded()
        {
            this._waitTimer.Reset(true);
            this._waitTimer.SetActive(true);
        }

        public void OnTimeUp()
        {
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.OnTimeUp, base.instancedAbility, base.instancedModifier, base.actor, null);
            this._waitTimer.Reset(false);
        }
    }
}

