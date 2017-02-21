namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityHPThresholdMixin : BaseAbilityMixin
    {
        private bool _isApplied;
        private HPThresholdMixin config;

        public AbilityHPThresholdMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (HPThresholdMixin) config;
        }

        public override void Core()
        {
            float lhs = (float) (base.actor.HP / base.actor.maxHP);
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

        public override void OnAdded()
        {
            this._isApplied = false;
        }
    }
}

