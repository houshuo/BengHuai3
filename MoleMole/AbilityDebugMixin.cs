namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityDebugMixin : BaseAbilityMixin
    {
        private bool _hasAdded;

        public AbilityDebugMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._hasAdded = false;
        }

        public override void OnAdded()
        {
            if (!this._hasAdded)
            {
                this._hasAdded = true;
            }
        }
    }
}

