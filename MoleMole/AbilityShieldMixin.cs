namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityShieldMixin : BaseAbilityMixin
    {
        private EntityTimer _betweenAttackResumeTimer;
        private AbilityState _controlledAbilityState;
        private float _displayFloor;
        private float _forceResumeDamage;
        private EntityTimer _forceResumeTimer;
        private EntityTimer _minForceResumeTimer;
        private MonsterActor _monsterActor;
        private BaseMonoMonster _monsterEntity;
        private DisplayValue<float> _shieldDisplay;
        private float _shieldResumeRatio;
        private float _shieldResumeStart;
        private float _shieldResumeTarget;
        private EntityTimer _shieldResumeTimer;
        private bool _showShieldBar;
        [ShowInInspector]
        private State _state;
        private ShieldMixin config;
        public float maxShield;
        public float shield;

        public AbilityShieldMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._shieldResumeRatio = 1f;
            this.config = (ShieldMixin) config;
            if (base.entity is BaseMonoMonster)
            {
                this._monsterEntity = (BaseMonoMonster) base.entity;
                this._monsterActor = (MonsterActor) base.actor;
            }
            this._showShieldBar = instancedAbility.Evaluate(this.config.ShowShieldBar) != 0;
            if (this.config.UseLevelTimeScale)
            {
                this._betweenAttackResumeTimer = new EntityTimer(this.config.BetweenAttackResumeCD);
                this._forceResumeTimer = new EntityTimer(this.config.ForceResumeCD);
                this._minForceResumeTimer = new EntityTimer(this.config.MinForceResumeCD);
                this._shieldResumeTimer = new EntityTimer(this.config.ShieldResumeTimeSpan);
            }
            else
            {
                this._betweenAttackResumeTimer = new EntityTimer(this.config.BetweenAttackResumeCD, base.entity);
                this._forceResumeTimer = new EntityTimer(this.config.ForceResumeCD, base.entity);
                this._minForceResumeTimer = new EntityTimer(this.config.MinForceResumeCD, base.entity);
                this._shieldResumeTimer = new EntityTimer(this.config.ShieldResumeTimeSpan, base.entity);
            }
        }

        public override void Core()
        {
            this._betweenAttackResumeTimer.Core(1f);
            this._forceResumeTimer.Core(this._shieldResumeRatio);
            this._minForceResumeTimer.Core(1f);
            this._shieldResumeTimer.Core(1f);
            if (this._state == State.Idle)
            {
                if (this.shield <= 0f)
                {
                    this._state = State.ShieldBroken;
                    this._forceResumeTimer.SetActive(true);
                    this._minForceResumeTimer.SetActive(true);
                    base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ShieldOnModifierName);
                    base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ShieldOffModifierName);
                }
                else if (this._betweenAttackResumeTimer.isTimeUp)
                {
                    this._state = State.Resuming;
                    this._displayFloor = this.config.ShieldDisplayRatioFloor;
                    this._shieldResumeStart = this.shield;
                    this._shieldResumeTarget = Mathf.Min(this.shield + (this.maxShield * this.config.BetweenAttackResumeCD), this.maxShield);
                    this._shieldResumeTimer.Reset(true);
                    this._betweenAttackResumeTimer.Reset(true);
                }
            }
            else if (this._state == State.ShieldBroken)
            {
                this._shieldResumeRatio = this.GetShieldResumeRatio();
                if (this._forceResumeTimer.isTimeUp && this._minForceResumeTimer.isTimeUp)
                {
                    this._state = State.Resuming;
                    this._displayFloor = this.config.ShieldDisplayRatioFloor;
                    this._shieldResumeStart = this.shield;
                    this._shieldResumeTarget = Mathf.Min(this.shield + (this.maxShield * this.config.ForceResumeRatio), this.maxShield);
                    this._shieldResumeTimer.Reset(true);
                    this._forceResumeTimer.Reset(false);
                    base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ShieldOffModifierName);
                    base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ShieldOnModifierName);
                }
            }
            else if (this._state == State.Resuming)
            {
                float newValue = Mathf.Lerp(this._shieldResumeStart, this._shieldResumeTarget, this._shieldResumeTimer.GetTimingRatio());
                DelegateUtils.UpdateField<float>(ref this.shield, newValue, new Action<float, float>(this.OnShieldChanged));
                if (this._shieldResumeTimer.isTimeUp)
                {
                    this._state = State.Idle;
                    this._shieldResumeTimer.timespan = this.config.ShieldResumeTimeSpan;
                    this._displayFloor = this.config.ShieldDisplayRatioFloor;
                    if (this._monsterActor.isElite)
                    {
                        this._monsterEntity.SwitchEliteShader(true);
                    }
                }
            }
        }

        private float GetShieldResumeRatio()
        {
            float throwForceResumeTimeRatio = 1f;
            if ((this._monsterEntity != null) && this._monsterEntity.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
            {
                throwForceResumeTimeRatio = this.config.ThrowForceResumeTimeRatio;
            }
            if ((this._controlledAbilityState & base.actor.abilityState) != AbilityState.None)
            {
                throwForceResumeTimeRatio = this.config.ControlledForceResumeTimeRatio;
            }
            return throwForceResumeTimeRatio;
        }

        public override void OnAdded()
        {
            this._state = State.Idle;
            this.shield = this.maxShield = base.actor.baseMaxHP * base.instancedAbility.Evaluate(this.config.ShieldHPRatio);
            this._betweenAttackResumeTimer.SetActive(true);
            this._forceResumeTimer.SetActive(false);
            this._minForceResumeTimer.SetActive(false);
            this._shieldResumeTimer.SetActive(false);
            base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ShieldOnModifierName);
            if (this._showShieldBar)
            {
                this._shieldDisplay = base.actor.abilityPlugin.CreateOrGetDisplayFloat("Shield", 0f, 1f, this.config.ShieldDisplayRatioCeiling);
                if (this._shieldDisplay.value < this.config.ShieldDisplayRatioCeiling)
                {
                    this._state = State.Resuming;
                    this._displayFloor = this._shieldDisplay.value;
                    this._shieldResumeTimer.timespan = 0.3f;
                    this._shieldResumeTimer.Reset(true);
                    this._shieldResumeStart = 0f;
                    this._shieldResumeTarget = this.maxShield;
                }
                else
                {
                    this._displayFloor = this.config.ShieldDisplayRatioFloor;
                }
            }
            if (this.config.ControlledAbilityStates != null)
            {
                for (int i = 0; i < this.config.ControlledAbilityStates.Length; i++)
                {
                    this._controlledAbilityState |= this.config.ControlledAbilityStates[i];
                }
            }
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtKilled) && this.OnKilled((EvtKilled) evt));
        }

        private bool OnKilled(EvtKilled evt)
        {
            DelegateUtils.UpdateField<float>(ref this.shield, 0f, new Action<float, float>(this.OnShieldChanged));
            return true;
        }

        private bool OnPostBeingHit(EvtBeingHit evt)
        {
            if (!evt.attackData.isAnimEventAttack)
            {
                return false;
            }
            if (evt.attackData.rejected)
            {
                return false;
            }
            if (base.actor.abilityState.ContainsState(AbilityState.Invincible) || base.actor.abilityState.ContainsState(AbilityState.Undamagable))
            {
                return false;
            }
            this._betweenAttackResumeTimer.Reset();
            float attackerAniDamageRatio = evt.attackData.attackerAniDamageRatio;
            if (attackerAniDamageRatio < 0f)
            {
                attackerAniDamageRatio = 0f;
            }
            float num3 = Mathf.Pow(evt.attackData.damage, this.config.DamagePower) * Mathf.Pow(evt.attackData.attackerShieldDamageRatio, this.config.ShieldDamagePower);
            num3 *= Mathf.Pow(attackerAniDamageRatio, this.config.AniDamagePower);
            num3 += evt.attackData.attackerShieldDamageDelta;
            float newValue = this.shield - num3;
            if (newValue <= 0f)
            {
                newValue = 0f;
            }
            if ((newValue > 0f) && (this.config.ShieldSuccessEffect != null))
            {
                evt.attackData.beHitEffectPattern = this.config.ShieldSuccessEffect;
            }
            if (((this.shield > 0f) && (newValue == 0f)) && (this._state != State.Resuming))
            {
                if (this.config.ShieldBrokenTimeSlow > 0f)
                {
                    Singleton<LevelManager>.Instance.levelActor.TimeSlow(this.config.ShieldBrokenTimeSlow);
                }
                base.FireMixinEffect(this.config.ShieldBrokenEffect, base.entity, false);
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.ShiedlBrokenActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
                evt.attackData.frameHalt = 0;
                evt.attackData.attackerAniDamageRatio = 10f;
                Singleton<EventManager>.Instance.FireEvent(new EvtShieldBroken(base.actor.runtimeID), MPEventDispatchMode.Normal);
                evt.attackData.AddHitFlag(AttackResult.ActorHitFlag.Count);
                this._forceResumeDamage = 0f;
                if (this._monsterActor.isElite)
                {
                    this._monsterEntity.SwitchEliteShader(false);
                }
            }
            if (this._state != State.Resuming)
            {
                DelegateUtils.UpdateField<float>(ref this.shield, newValue, new Action<float, float>(this.OnShieldChanged));
            }
            if ((this.shield > 0f) && (evt.attackData.attackeeAniDefenceRatio > evt.attackData.attackerAniDamageRatio))
            {
                evt.attackData.frameHalt += this.config.ShieldSuccessAddFrameHalt;
                if (this.config.MuteHitEffect)
                {
                    evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Mute;
                }
                else
                {
                    evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Light;
                }
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.ShieldSuccessActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
            }
            if (((this._state == State.ShieldBroken) && (this.shield == 0f)) && (base.instancedAbility.Evaluate(this.config.ForceResumeByDamageHPRatio) > 0f))
            {
                this._forceResumeDamage += evt.attackData.GetTotalDamage() * this._shieldResumeRatio;
                if ((this._forceResumeDamage / base.actor.baseMaxHP) >= base.instancedAbility.Evaluate(this.config.ForceResumeByDamageHPRatio))
                {
                    this._forceResumeTimer.isTimeUp = true;
                }
            }
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnPostBeingHit((EvtBeingHit) evt));
        }

        public override void OnRemoved()
        {
            base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ShieldOnModifierName);
            base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ShieldOffModifierName);
        }

        private void OnShieldChanged(float from, float to)
        {
            if (this._shieldDisplay != null)
            {
                this._shieldDisplay.Pub(Mathf.Lerp(this._displayFloor, this.config.ShieldDisplayRatioCeiling, to / this.maxShield));
            }
        }

        private enum State
        {
            Idle,
            ShieldBroken,
            Resuming
        }
    }
}

