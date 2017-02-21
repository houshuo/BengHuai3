namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityGlobalComboClearResistMixin : BaseAbilityMixin
    {
        private LevelActor _levelActor;
        private EntityTimer _resumeTimer;
        private State _state;
        private GlobalComboClearResistMixin config;

        public AbilityGlobalComboClearResistMixin(ActorAbility instancedAbility, ActorModifier instancedModifer, ConfigAbilityMixin config) : base(instancedAbility, instancedModifer, config)
        {
            this.config = (GlobalComboClearResistMixin) config;
            this._resumeTimer = new EntityTimer(instancedAbility.Evaluate(this.config.ResumeTimeSpan));
            this._levelActor = Singleton<LevelManager>.Instance.levelActor;
        }

        public override void Core()
        {
            if (base.actor.isAlive != 0)
            {
                this._resumeTimer.Core(1f);
                State state = this._state;
                if (((state != State.Effect) && (state == State.Resuming)) && this._resumeTimer.isTimeUp)
                {
                    this._resumeTimer.timespan = base.instancedAbility.Evaluate(this.config.ResumeTimeSpan);
                    this._resumeTimer.Reset(false);
                    this._state = State.Effect;
                    this._levelActor.comboTimeUPCallback = (Action) Delegate.Combine(this._levelActor.comboTimeUPCallback, new Action(this.OnComboTimeUp));
                }
            }
        }

        public override void OnAdded()
        {
            this._state = State.Effect;
            this._levelActor.comboTimeUPCallback = (Action) Delegate.Combine(this._levelActor.comboTimeUPCallback, new Action(this.OnComboTimeUp));
            this._resumeTimer.SetActive(false);
        }

        private void OnComboTimeUp()
        {
            if (this._state == State.Effect)
            {
                this._state = State.Resuming;
                this._levelActor.ResetComboTimer();
                this._levelActor.comboTimeUPCallback = (Action) Delegate.Remove(this._levelActor.comboTimeUPCallback, new Action(this.OnComboTimeUp));
                this._resumeTimer.timespan = base.instancedAbility.Evaluate(this.config.ResumeTimeSpan);
                this._resumeTimer.SetActive(true);
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ResitComboClear, null));
            }
        }

        public override void OnRemoved()
        {
            this._levelActor.comboTimeUPCallback = (Action) Delegate.Remove(this._levelActor.comboTimeUPCallback, new Action(this.OnComboTimeUp));
            this._resumeTimer.SetActive(false);
        }

        private enum State
        {
            Effect,
            Resuming
        }
    }
}

