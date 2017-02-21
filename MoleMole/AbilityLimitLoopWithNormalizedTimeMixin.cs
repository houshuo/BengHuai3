namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityLimitLoopWithNormalizedTimeMixin : BaseAbilityMixin
    {
        private int _countLeft;
        private State _state;
        public LimitLoopWithNormalizedTime config;

        public AbilityLimitLoopWithNormalizedTimeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (LimitLoopWithNormalizedTime) config;
        }

        public override void Core()
        {
            if (this._state == State.WaitingForLoop)
            {
                float currentNormalizedTime = base.entity.GetCurrentNormalizedTime();
                if (currentNormalizedTime > this.config.NormalizedTimeStart)
                {
                    this._state = State.Looping;
                    if (this._countLeft <= 0)
                    {
                        base.entity.SetPersistentAnimatorBool(this.config.AllowLoopBoolID, false);
                    }
                }
                else if (currentNormalizedTime > this.config.NormalizedTimeStop)
                {
                    this._state = State.Exiting;
                    base.entity.SetPersistentAnimatorBool(this.config.AllowLoopBoolID, true);
                }
            }
            else if ((this._state == State.Looping) && (base.entity.GetCurrentNormalizedTime() > this.config.NormalizedTimeStop))
            {
                this._state = State.Exiting;
                base.entity.SetPersistentAnimatorBool(this.config.AllowLoopBoolID, true);
            }
        }

        public override void OnAdded()
        {
            this._countLeft = base.instancedAbility.Evaluate(this.config.LoopLimitCount);
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            base.entity.SetPersistentAnimatorBool(this.config.AllowLoopBoolID, true);
            this._state = State.Idle;
        }

        public override void OnRemoved()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            base.entity.RemovePersistentAnimatorBool(this.config.AllowLoopBoolID);
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (this._state == State.Idle)
            {
                if (to == this.config.SkillID)
                {
                    this._state = State.WaitingForLoop;
                    this._countLeft--;
                    base.entity.SetPersistentAnimatorBool(this.config.AllowLoopBoolID, true);
                }
            }
            else if (to == this.config.SkillID)
            {
                this._state = State.WaitingForLoop;
                this._countLeft--;
                base.entity.SetPersistentAnimatorBool(this.config.AllowLoopBoolID, true);
            }
            else if (to != this.config.SkillID)
            {
                this._state = State.Idle;
                this._countLeft = base.instancedAbility.Evaluate(this.config.LoopLimitCount);
                base.entity.SetPersistentAnimatorBool(this.config.AllowLoopBoolID, true);
            }
        }

        private enum State
        {
            Idle,
            WaitingForLoop,
            Looping,
            Exiting
        }
    }
}

