namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityLimitLoopWithMaskTriggerMixin : BaseAbilityMixin
    {
        private int _countLeft;
        private int _currentLimitLoopCount;
        private bool _isMasked;
        private int _originLimitLoopCount;
        private EntityTimer _overCountTimer;
        private EntityTimer _timer;
        public LimitLoopWithMaskTriggerMixin config;

        public AbilityLimitLoopWithMaskTriggerMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (LimitLoopWithMaskTriggerMixin) config;
            this._timer = new EntityTimer(this.config.MaskDuration, base.entity);
            if (this.config.UseOverCount)
            {
                this._overCountTimer = new EntityTimer(this.config.ResetOverCountTime, base.entity);
            }
        }

        public override void Core()
        {
            this._timer.Core(1f);
            if (this._timer.isTimeUp)
            {
                base.entity.UnmaskTrigger(this.config.MaskTriggerID);
                this._isMasked = false;
                this._timer.Reset(false);
            }
            if (this.config.UseOverCount)
            {
                this._overCountTimer.Core(1f);
                if (this._overCountTimer.isTimeUp)
                {
                    int num;
                    this._overCountTimer.Reset(true);
                    if (this._currentLimitLoopCount < this._originLimitLoopCount)
                    {
                        num = this._currentLimitLoopCount + 1;
                    }
                    else
                    {
                        num = this._originLimitLoopCount;
                    }
                    if (this._countLeft == this._currentLimitLoopCount)
                    {
                        this._countLeft = num;
                    }
                    this._currentLimitLoopCount = num;
                }
            }
        }

        public override void OnAdded()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            this._originLimitLoopCount = base.instancedAbility.Evaluate(this.config.LoopLimitCount);
            this._currentLimitLoopCount = this._originLimitLoopCount;
            this._countLeft = this._currentLimitLoopCount;
        }

        public override void OnRemoved()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            if (this._timer.isActive)
            {
                base.entity.UnmaskTrigger(this.config.MaskTriggerID);
                this._isMasked = false;
                this._timer.Reset(false);
            }
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (to == this.config.SkillID)
            {
                this._countLeft--;
                if ((this._countLeft <= 0) && !this._isMasked)
                {
                    base.entity.MaskTrigger(this.config.MaskTriggerID);
                    this._isMasked = true;
                    this._timer.Reset(true);
                    if (this.config.UseOverCount)
                    {
                        this._overCountTimer.Reset(true);
                        if (this._currentLimitLoopCount > 1)
                        {
                            this._currentLimitLoopCount--;
                        }
                    }
                }
            }
            else if ((from == this.config.SkillID) && (to != this.config.SkillID))
            {
                this._countLeft = this._currentLimitLoopCount;
            }
        }
    }
}

