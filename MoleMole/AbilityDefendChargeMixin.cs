namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityDefendChargeMixin : BaseAbilityMixin
    {
        private ActorModifier _defendAttachedModifier;
        private ActorModifier _defendPerfectAttachedModifier;
        private bool _isInDefend;
        private float _perfectDefendDuration;
        private PerfectDefendState _perfectDefendState;
        private EntityTimer _perfectDefendTimer;
        private State _state;
        public DefendChargeMixin config;

        public AbilityDefendChargeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (DefendChargeMixin) config;
            this._perfectDefendTimer = new EntityTimer(0f, base.entity);
            this._perfectDefendTimer.SetActive(false);
            this._perfectDefendDuration = instancedAbility.Evaluate(this.config.DefendPerfectEndTime) - instancedAbility.Evaluate(this.config.DefendPerfectStartTime);
        }

        public override void Core()
        {
            if (this._perfectDefendState == PerfectDefendState.WaitToStart)
            {
                this._perfectDefendTimer.Core(1f);
                if (this._perfectDefendTimer.isTimeUp)
                {
                    this._perfectDefendTimer.timespan = this._perfectDefendDuration;
                    this._perfectDefendTimer.Reset(true);
                    this._perfectDefendState = PerfectDefendState.PerfectDefend;
                    if (this.config.DefendPerfectDurationModifierName != null)
                    {
                        this._defendPerfectAttachedModifier = base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.DefendPerfectDurationModifierName);
                    }
                }
            }
            else if (this._perfectDefendState == PerfectDefendState.PerfectDefend)
            {
                this._perfectDefendTimer.Core(1f);
                if (this._perfectDefendTimer.isTimeUp)
                {
                    this._perfectDefendTimer.Reset(false);
                    this._perfectDefendState = PerfectDefendState.Idle;
                    if (this._defendPerfectAttachedModifier != null)
                    {
                        base.actor.abilityPlugin.TryRemoveModifier(this._defendPerfectAttachedModifier);
                        this._defendPerfectAttachedModifier = null;
                    }
                }
            }
            if (this._state == State.Before)
            {
                if (!this._isInDefend && (base.entity.GetCurrentNormalizedTime() > this.config.DefendBSNormalizedStartTime))
                {
                    this.SetInDefend(true);
                }
            }
            else if (((this._state == State.After) && this._isInDefend) && (base.entity.GetCurrentNormalizedTime() > this.config.DefendASNormalizedEndTime))
            {
                this.SetInDefend(false);
            }
        }

        public override void OnAdded()
        {
            this._state = State.Idle;
            this._perfectDefendState = PerfectDefendState.Idle;
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
        }

        private bool OnBeingHit(EvtBeingHit evt)
        {
            if (!this._isInDefend)
            {
                return false;
            }
            if (this.config.DefendReplaceAttackEffect != null)
            {
                evt.attackData.attackEffectPattern = this.config.DefendReplaceAttackEffect;
            }
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.DefendSuccessActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
            if (this._perfectDefendState == PerfectDefendState.PerfectDefend)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.DefendSuccessPerfectActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
            }
            return true;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnBeingHit((EvtBeingHit) evt));
        }

        public override void OnRemoved()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
        }

        private void SetInDefend(bool inDefend)
        {
            if (inDefend)
            {
                this._perfectDefendState = PerfectDefendState.WaitToStart;
                this._perfectDefendTimer.timespan = base.instancedAbility.Evaluate(this.config.DefendPerfectStartTime);
                this._perfectDefendTimer.Reset(true);
                if (this.config.DefendDurationModifierName != null)
                {
                    this._defendAttachedModifier = base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.DefendDurationModifierName);
                }
            }
            else if (this._defendAttachedModifier != null)
            {
                base.actor.abilityPlugin.TryRemoveModifier(this._defendAttachedModifier);
                this._defendAttachedModifier = null;
            }
            this._isInDefend = inDefend;
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (this._state == State.Idle)
            {
                if (to == this.config.DefendBSSkillID)
                {
                    this._state = State.Before;
                }
            }
            else if (this._state == State.Before)
            {
                if (to == this.config.DefendLoopSkillID)
                {
                    this._state = State.InLoop;
                }
                else
                {
                    if (this._isInDefend)
                    {
                        this.SetInDefend(false);
                    }
                    this._state = State.Idle;
                }
            }
            else if (this._state == State.InLoop)
            {
                if (to == this.config.DefendBSSkillID)
                {
                    this._state = State.After;
                }
                else
                {
                    this.SetInDefend(false);
                    this._state = State.Idle;
                }
            }
            else if (this._state == State.After)
            {
                if (to == this.config.DefendBSSkillID)
                {
                    if (this._isInDefend)
                    {
                        this.SetInDefend(false);
                    }
                    this._state = State.Before;
                }
                else
                {
                    if (this._isInDefend)
                    {
                        this.SetInDefend(false);
                    }
                    this._state = State.Idle;
                }
            }
        }

        private enum PerfectDefendState
        {
            Idle,
            WaitToStart,
            PerfectDefend
        }

        private enum State
        {
            Idle,
            Before,
            InLoop,
            After
        }
    }
}

