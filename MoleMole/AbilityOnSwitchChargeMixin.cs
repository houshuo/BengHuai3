namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityOnSwitchChargeMixin : BaseAbilityMixin
    {
        private OnSwitchChargeMixin config;

        public AbilityOnSwitchChargeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (OnSwitchChargeMixin) config;
        }

        private bool OnChargeRelease(EvtChargeRelease evt)
        {
            if (evt.isSwitchRelease && Miscs.ArrayContains<string>(this.config.AfterSkillIDs, evt.releaseSkillID))
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.Actions, base.instancedAbility, base.instancedModifier, null, evt);
                return true;
            }
            return false;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtChargeRelease) && this.OnChargeRelease((EvtChargeRelease) evt));
        }
    }
}

