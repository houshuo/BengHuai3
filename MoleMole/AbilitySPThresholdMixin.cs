namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilitySPThresholdMixin : BaseAbilityMixin
    {
        private bool _isApplied;
        private SPTHresholdMixin config;

        public AbilitySPThresholdMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (SPTHresholdMixin) config;
        }

        public override void OnAdded()
        {
            this._isApplied = false;
            this.OnSPChangedCallback((float) base.actor.SP, (float) base.actor.SP, 0f);
            base.actor.onSPChanged = (Action<float, float, float>) Delegate.Combine(base.actor.onSPChanged, new Action<float, float, float>(this.OnSPChangedCallback));
        }

        public override void OnRemoved()
        {
            if (this._isApplied)
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierName);
            }
            base.actor.onSPChanged = (Action<float, float, float>) Delegate.Remove(base.actor.onSPChanged, new Action<float, float, float>(this.OnSPChangedCallback));
        }

        private void OnSPChangedCallback(float from, float to, float delta)
        {
            float lhs = to / base.actor.maxSP;
            bool flag = base.EvaluatePredicate(lhs, base.instancedAbility.Evaluate(this.config.Threshold), this.config.Predicate);
            if (this._isApplied)
            {
                if (!flag)
                {
                    base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ModifierName);
                    this._isApplied = false;
                }
            }
            else if (flag)
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierName);
                this._isApplied = true;
            }
        }
    }
}

