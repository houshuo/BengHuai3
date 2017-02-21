namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityIntervalModifierMixin : BaseAbilityMixin
    {
        private EntityTimer _intervalTimer;
        private IntervalModifierMixin config;

        public AbilityIntervalModifierMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (IntervalModifierMixin) config;
            this._intervalTimer = new EntityTimer(instancedAbility.Evaluate(this.config.Interval));
        }

        private void AddModifier()
        {
            base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierName);
        }

        public override void Core()
        {
            if (this._intervalTimer.isActive)
            {
                if (!this.GetPredicateResult())
                {
                    this._intervalTimer.Core(1f);
                }
                if (this._intervalTimer.isTimeUp)
                {
                    this.AddModifier();
                    this._intervalTimer.Reset(true);
                }
            }
        }

        private bool GetPredicateResult()
        {
            bool flag = false;
            if (this.config.Predicates.Length > 0)
            {
                flag = base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, base.actor, null);
            }
            return flag;
        }

        public override void OnAdded()
        {
            this._intervalTimer.Reset(true);
        }

        public override void OnRemoved()
        {
            this._intervalTimer.Reset(false);
            base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ModifierName);
        }
    }
}

