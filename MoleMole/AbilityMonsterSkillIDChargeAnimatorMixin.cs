namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityMonsterSkillIDChargeAnimatorMixin : BaseAbilityMixin
    {
        private string _chargeAudioLoopName;
        private int _chargeEffectPatternIx;
        private EntityTimer _chargeTimer;
        private float _chargeTimeRatio;
        private string _lastFrom;
        protected int _loopCount;
        protected int _loopIx;
        private BaseMonoMonster _monster;
        private State _state;
        private EntityTimer _switchTimer;
        private EntityTimer _triggeredChargeTimer;
        private MonsterSkillIDChargeAnimatorMixin config;

        public AbilityMonsterSkillIDChargeAnimatorMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MonsterSkillIDChargeAnimatorMixin) config;
            this._monster = (BaseMonoMonster) base.entity;
            this._chargeTimer = new EntityTimer();
            this._triggeredChargeTimer = new EntityTimer();
            this._switchTimer = new EntityTimer(this.config.ChargeSwitchWindow, base.entity);
            this._loopCount = this.config.ChargeLoopSkillIDs.Length;
            this._chargeTimeRatio = instancedAbility.Evaluate(this.config.ChargeTimeRatio);
        }

        public override void Core()
        {
            if (this._triggeredChargeTimer.isActive && (this._state != State.Idle))
            {
                this._triggeredChargeTimer.Core(1f);
                if (this._state == State.Before)
                {
                    if (this._triggeredChargeTimer.isTimeUp)
                    {
                        this._monster.ResetTrigger(this.config.NextLoopTriggerID);
                        this._monster.SetTrigger(this.config.AfterSkillTriggerID);
                    }
                    else
                    {
                        this._monster.ResetTrigger(this.config.AfterSkillTriggerID);
                        this._monster.SetTrigger(this.config.NextLoopTriggerID);
                    }
                }
                else if ((this._state == State.InLoop) && this._triggeredChargeTimer.isTimeUp)
                {
                    this._monster.SetTrigger(this.config.AfterSkillTriggerID);
                }
            }
            if (this._state == State.InLoop)
            {
                this._chargeTimer.Core(1f);
                if (this._chargeTimer.isTimeUp)
                {
                    this._loopIx++;
                    if (this._loopIx == this._loopCount)
                    {
                        this._monster.SetTrigger(this.config.AfterSkillTriggerID);
                        this._chargeTimer.Reset(false);
                    }
                    else
                    {
                        this._monster.SetTrigger(this.config.NextLoopTriggerID);
                        this._chargeTimer.timespan = this.config.ChargeLoopDurations[this._loopIx] * this._chargeTimeRatio;
                        this._chargeTimer.Reset(true);
                        this._switchTimer.Reset(true);
                    }
                }
                this._switchTimer.Core(1f);
                if (this._switchTimer.isTimeUp)
                {
                    this._switchTimer.Reset(false);
                }
            }
        }

        private bool IsTriggerCharging()
        {
            return (this._triggeredChargeTimer.isActive && !this._triggeredChargeTimer.isTimeUp);
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            float abilityArgument = (float) evt.abilityArgument;
            this._triggeredChargeTimer.timespan = abilityArgument;
            this._triggeredChargeTimer.Reset(true);
        }

        public override void OnAdded()
        {
            this._monster.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(this._monster.onCurrentSkillIDChanged, new Action<string, string>(this.WithTransientSkillIDChangedCallback));
            this._state = State.Idle;
            this._chargeTimer.Reset(false);
            this._switchTimer.Reset(false);
            this._loopIx = 0;
            this._chargeEffectPatternIx = -1;
            if ((this.config.ChargeLoopEffects != null) && (this.config.ChargeSwitchEffects != null))
            {
            }
        }

        public override void OnRemoved()
        {
            this._monster.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(this._monster.onCurrentSkillIDChanged, new Action<string, string>(this.WithTransientSkillIDChangedCallback));
            if (this._chargeEffectPatternIx != -1)
            {
                base.entity.DetachEffectImmediately(this._chargeEffectPatternIx);
            }
            if (this._chargeAudioLoopName != null)
            {
                base.entity.StopAudio(this._chargeAudioLoopName);
            }
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (this._state == State.Idle)
            {
                if (Miscs.ArrayContains<string>(this.config.BeforeSkillIDs, to))
                {
                    if (this.IsTriggerCharging())
                    {
                        this._monster.ResetTrigger(this.config.AfterSkillTriggerID);
                        this._monster.SetTrigger(this.config.NextLoopTriggerID);
                        this._state = State.Before;
                        this._loopIx = 0;
                    }
                    else
                    {
                        this._monster.ResetTrigger(this.config.NextLoopTriggerID);
                        this._monster.SetTrigger(this.config.AfterSkillTriggerID);
                    }
                }
            }
            else if (this._state == State.Before)
            {
                if (to == this.config.ChargeLoopSkillIDs[this._loopIx])
                {
                    if (this.config.ChargeLoopEffects != null)
                    {
                        MixinEffect effect = this.config.ChargeLoopEffects[this._loopIx];
                        if (effect.EffectPattern != null)
                        {
                            this._chargeEffectPatternIx = base.entity.AttachEffect(effect.EffectPattern);
                        }
                        if (effect.AudioPattern != null)
                        {
                            this._chargeAudioLoopName = effect.AudioPattern;
                            base.entity.PlayAudio(effect.AudioPattern);
                        }
                    }
                    this._state = State.InLoop;
                    this._chargeTimer.timespan = this.config.ChargeLoopDurations[this._loopIx] * this._chargeTimeRatio;
                    this._chargeTimer.Reset(true);
                }
                else if (Miscs.ArrayContains<string>(this.config.AfterSkillIDs, to))
                {
                    this._monster.ResetTrigger(this.config.AfterSkillTriggerID);
                    this._monster.ResetTrigger(this.config.NextLoopTriggerID);
                    this._state = State.After;
                }
                else if (!Miscs.ArrayContains<string>(this.config.BeforeSkillIDs, to))
                {
                    this._monster.ResetTrigger(this.config.AfterSkillTriggerID);
                    this._monster.ResetTrigger(this.config.NextLoopTriggerID);
                    this._state = State.Idle;
                }
            }
            else if (this._state == State.InLoop)
            {
                if (Miscs.ArrayContains<string>(this.config.ChargeLoopSkillIDs, to))
                {
                    if (this.config.ChargeLoopEffects != null)
                    {
                        base.entity.DetachEffect(this._chargeEffectPatternIx);
                        if (this._chargeAudioLoopName != null)
                        {
                            base.entity.StopAudio(this._chargeAudioLoopName);
                            this._chargeAudioLoopName = null;
                        }
                        MixinEffect effect2 = this.config.ChargeLoopEffects[this._loopIx];
                        if (effect2.EffectPattern != null)
                        {
                            this._chargeEffectPatternIx = base.entity.AttachEffect(effect2.EffectPattern);
                        }
                        if (effect2.AudioPattern != null)
                        {
                            this._chargeAudioLoopName = effect2.AudioPattern;
                            base.entity.PlayAudio(effect2.AudioPattern);
                        }
                        if (this.config.ChargeSwitchEffects != null)
                        {
                            base.FireMixinEffect(this.config.ChargeSwitchEffects[this._loopIx - 1], base.entity, false);
                        }
                    }
                }
                else if (Miscs.ArrayContains<string>(this.config.AfterSkillIDs, to))
                {
                    if (this.config.ChargeLoopEffects != null)
                    {
                        base.entity.DetachEffectImmediately(this._chargeEffectPatternIx);
                        this._chargeEffectPatternIx = -1;
                        if (this._chargeAudioLoopName != null)
                        {
                            base.entity.StopAudio(this._chargeAudioLoopName);
                            this._chargeAudioLoopName = null;
                        }
                    }
                    EvtChargeRelease evt = new EvtChargeRelease(base.actor.runtimeID, to) {
                        isSwitchRelease = this._switchTimer.isActive && !this._switchTimer.isTimeUp
                    };
                    Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
                    this._switchTimer.Reset(false);
                    this._chargeTimer.Reset(false);
                    this._state = State.After;
                }
                else
                {
                    if (this.config.ChargeLoopEffects != null)
                    {
                        base.entity.DetachEffectImmediately(this._chargeEffectPatternIx);
                        this._chargeEffectPatternIx = -1;
                        if (this._chargeAudioLoopName != null)
                        {
                            base.entity.StopAudio(this._chargeAudioLoopName);
                            this._chargeAudioLoopName = null;
                        }
                    }
                    this._monster.ResetTrigger(this.config.AfterSkillTriggerID);
                    this._monster.ResetTrigger(this.config.NextLoopTriggerID);
                    this._state = State.Idle;
                }
            }
            else if (this._state == State.After)
            {
                if (Miscs.ArrayContains<string>(this.config.BeforeSkillIDs, to) && this.IsTriggerCharging())
                {
                    this._monster.ResetTrigger(this.config.AfterSkillTriggerID);
                    this._monster.SetTrigger(this.config.NextLoopTriggerID);
                    this._state = State.Before;
                    this._loopIx = 0;
                }
                else
                {
                    this._monster.ResetTrigger(this.config.AfterSkillTriggerID);
                    this._monster.ResetTrigger(this.config.NextLoopTriggerID);
                    this._state = State.Idle;
                }
            }
        }

        private void WithTransientSkillIDChangedCallback(string from, string to)
        {
            if (Miscs.ArrayContains<string>(this.config.TransientSkillIDs, to))
            {
                this._lastFrom = from;
            }
            else if (Miscs.ArrayContains<string>(this.config.TransientSkillIDs, from))
            {
                this.SkillIDChangedCallback(this._lastFrom, to);
            }
            else
            {
                this.SkillIDChangedCallback(from, to);
            }
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

