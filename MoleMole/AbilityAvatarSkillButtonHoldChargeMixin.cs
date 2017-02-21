namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAvatarSkillButtonHoldChargeMixin : BaseAbilityAvatarSkillButtonHoldChargeMixin
    {
        private EntityTimer _chargeTimer;
        private AvatarSkillButtonHoldChargeAnimatorMixin config;

        public AbilityAvatarSkillButtonHoldChargeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarSkillButtonHoldChargeAnimatorMixin) config;
            this._chargeTimer = new EntityTimer();
            base._chargeTimeRatio = instancedAbility.Evaluate(this.config.ChargeTimeRatio);
        }

        public override void OnAdded()
        {
            base.OnAdded();
            this._chargeTimer.Reset(false);
        }

        protected override void OnBeforeToInLoop()
        {
            this._chargeTimer.timespan = this.config.ChargeLoopDurations[base._loopIx] * base._chargeTimeRatio;
            this._chargeTimer.Reset(true);
        }

        protected override void OnInLoopToAfter()
        {
            this._chargeTimer.Reset(false);
        }

        protected override void OnMoveingToNextLoop(bool endLoop)
        {
            if (endLoop)
            {
                this._chargeTimer.Reset(false);
            }
            else
            {
                this._chargeTimer.timespan = this.config.ChargeLoopDurations[base._loopIx] * base._chargeTimeRatio;
                this._chargeTimer.Reset(true);
            }
        }

        protected override bool ShouldMoveToNextLoop()
        {
            return this._chargeTimer.isTimeUp;
        }

        protected override void UpdateInLoop()
        {
            this._chargeTimer.Core(base.actor.entity.GetProperty("Entity_AttackSpeed") + 1f);
        }
    }
}

