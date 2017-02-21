namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityLimitWithMaskTriggerMixin : BaseAbilityMixin
    {
        private int _countLeft;
        private EntityTimer _countTimer;
        private bool _isMasked;
        private EntityTimer _maskTimer;
        private int _originEvaLimitCount;
        public LimitTimeWithMaskTriggerMixin config;

        public AbilityLimitWithMaskTriggerMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (LimitTimeWithMaskTriggerMixin) config;
            this._maskTimer = new EntityTimer(this.config.MaskDuration, base.entity);
            this._countTimer = new EntityTimer(this.config.CountTime, base.entity);
        }

        public override void Core()
        {
            this._maskTimer.Core(1f);
            this._countTimer.Core(1f);
            if (this._maskTimer.isTimeUp)
            {
                base.entity.UnmaskTrigger(this.config.MaskTriggerID);
                this._isMasked = false;
                this._maskTimer.Reset(false);
            }
            if (this._countTimer.isTimeUp)
            {
                this._countLeft = this._originEvaLimitCount;
            }
        }

        public override void OnAdded()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            this._originEvaLimitCount = base.instancedAbility.Evaluate(this.config.EvadeLimitCount);
            this._countLeft = this._originEvaLimitCount;
        }

        public override void OnRemoved()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            if (this._maskTimer.isActive)
            {
                base.entity.UnmaskTrigger(this.config.MaskTriggerID);
                this._isMasked = false;
                this._maskTimer.Reset(false);
            }
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (to == this.config.SkillID)
            {
                if (this._countLeft == this._originEvaLimitCount)
                {
                    this._countTimer.Reset(true);
                }
                this._countLeft--;
                if ((this._countLeft <= 0) && !this._isMasked)
                {
                    base.entity.MaskTrigger(this.config.MaskTriggerID);
                    this._isMasked = true;
                    this._maskTimer.Reset(true);
                }
            }
        }
    }
}

