namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityGlobalMainShieldMixin : BaseAbilityMixin
    {
        private EntityTimer _betweenAttackResumeTimer;
        private AbilityState _controlledAbilityState;
        private float _displayFloor;
        private EntityTimer _forceResumeTimer;
        private DynamicActorValue<float> _globalShieldValue;
        private EntityTimer _minForceResumeTimer;
        private DisplayValue<float> _shieldDisplay;
        private float _shieldResumeRatio;
        private float _shieldResumeStart;
        private float _shieldResumeTarget;
        private EntityTimer _shieldResumeTimer;
        private bool _showShieldBar;
        [ShowInInspector]
        private State _state;
        private GlobalMainShieldMixin config;
        public static string GLOBAL_SHIELD_KEY = "GlobalShield";
        public float maxShield;

        public AbilityGlobalMainShieldMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._shieldResumeRatio = 1f;
            this.config = (GlobalMainShieldMixin) config;
            if (this.config.UseLevelTimeScale)
            {
                this._betweenAttackResumeTimer = new EntityTimer(instancedAbility.Evaluate(this.config.BetweenAttackResumeCD));
                this._forceResumeTimer = new EntityTimer(this.config.ForceResumeCD);
                this._minForceResumeTimer = new EntityTimer(this.config.MinForceResumeCD);
                this._shieldResumeTimer = new EntityTimer(this.config.ShieldResumeTimeSpan);
            }
            else
            {
                this._betweenAttackResumeTimer = new EntityTimer(instancedAbility.Evaluate(this.config.BetweenAttackResumeCD), base.entity);
                this._forceResumeTimer = new EntityTimer(this.config.ForceResumeCD, base.entity);
                this._minForceResumeTimer = new EntityTimer(this.config.MinForceResumeCD, base.entity);
                this._shieldResumeTimer = new EntityTimer(this.config.ShieldResumeTimeSpan, base.entity);
            }
        }

        public override void Core()
        {
            if (base.actor.isAlive != 0)
            {
                this._betweenAttackResumeTimer.Core(1f);
                this._forceResumeTimer.Core(this._shieldResumeRatio);
                this._minForceResumeTimer.Core(1f);
                this._shieldResumeTimer.Core(1f);
                if (this._state == State.Idle)
                {
                    if (this._globalShieldValue.Value <= 0f)
                    {
                        this._state = State.ShieldBroken;
                        this._forceResumeTimer.SetActive(true);
                        this._minForceResumeTimer.SetActive(true);
                        this._globalShieldValue.Pub(0f);
                        foreach (BaseAbilityActor actor in Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(base.actor))
                        {
                            if (!string.IsNullOrEmpty(this.config.ShieldOffModifierName))
                            {
                                actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ShieldOffModifierName);
                            }
                        }
                        base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.ShieldBrokenActions, base.instancedAbility, base.instancedModifier, null, null);
                    }
                    else if (this._betweenAttackResumeTimer.isTimeUp && (this._globalShieldValue.Value < this.maxShield))
                    {
                        this._state = State.Resuming;
                        this._shieldResumeStart = this._globalShieldValue.Value;
                        this._shieldResumeTarget = this.maxShield;
                        this._shieldResumeTimer.Reset(true);
                        this._betweenAttackResumeTimer.Reset(false);
                    }
                }
                else if (this._state == State.ShieldBroken)
                {
                    this._shieldResumeRatio = this.GetShieldResumeRatio();
                    if ((this._forceResumeTimer.isTimeUp && this._minForceResumeTimer.isTimeUp) || (this._betweenAttackResumeTimer.isTimeUp && (this._globalShieldValue.Value < this.maxShield)))
                    {
                        this._state = State.Resuming;
                        this._shieldResumeStart = this._globalShieldValue.Value;
                        this._shieldResumeTarget = this.maxShield;
                        this._shieldResumeTimer.Reset(true);
                        this._forceResumeTimer.Reset(false);
                        this._betweenAttackResumeTimer.Reset(false);
                        this._globalShieldValue.Pub(0f);
                        foreach (BaseAbilityActor actor2 in Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(base.actor))
                        {
                            if (!string.IsNullOrEmpty(this.config.ShieldOffModifierName))
                            {
                                actor2.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ShieldOffModifierName);
                            }
                        }
                    }
                }
                else if (this._state == State.Resuming)
                {
                    float newValue = Mathf.Lerp(this._shieldResumeStart, this._shieldResumeTarget, this._shieldResumeTimer.GetTimingRatio());
                    this._globalShieldValue.Pub(newValue);
                    if (this._shieldResumeTimer.isTimeUp)
                    {
                        this._state = State.Idle;
                        this._shieldResumeTimer.timespan = this.config.ShieldResumeTimeSpan;
                        base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.ShieldResumeActions, base.instancedAbility, base.instancedModifier, null, null);
                    }
                }
            }
        }

        private float GetShieldResumeRatio()
        {
            float controlledForceResumeTimeRatio = 1f;
            if ((this._controlledAbilityState & base.actor.abilityState) != AbilityState.None)
            {
                controlledForceResumeTimeRatio = this.config.ControlledForceResumeTimeRatio;
            }
            return controlledForceResumeTimeRatio;
        }

        private bool ListenBeingHit(EvtBeingHit evt)
        {
            if (evt.attackData.isAnimEventAttack)
            {
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID);
                if ((actor == null) || !(actor is AvatarActor))
                {
                    return false;
                }
                if (actor.abilityState.ContainsState(AbilityState.Invincible) || actor.abilityState.ContainsState(AbilityState.Undamagable))
                {
                    return false;
                }
                this._betweenAttackResumeTimer.Reset(true);
            }
            return false;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.ListenBeingHit((EvtBeingHit) evt));
        }

        public override void OnAdded()
        {
            this._state = State.Idle;
            this.maxShield = (base.actor.baseMaxHP * base.instancedAbility.Evaluate(this.config.ShieldHPRatio)) + base.instancedAbility.Evaluate(this.config.ShieldHP);
            this._forceResumeTimer.SetActive(false);
            this._minForceResumeTimer.SetActive(false);
            this._shieldResumeTimer.SetActive(false);
            this._globalShieldValue = base.actor.abilityPlugin.CreateOrGetDynamicFloat(GLOBAL_SHIELD_KEY, this.maxShield);
            this._globalShieldValue.Pub(this.maxShield);
            foreach (BaseAbilityActor actor in Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(base.actor))
            {
                actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ChildShieldModifierName);
            }
            if (this.config.ControlledAbilityStates != null)
            {
                for (int i = 0; i < this.config.ControlledAbilityStates.Length; i++)
                {
                    this._controlledAbilityState |= this.config.ControlledAbilityStates[i];
                }
            }
            Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(base.actor.runtimeID);
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtKilled) && this.OnKilled((EvtKilled) evt));
        }

        private bool OnKilled(EvtKilled evt)
        {
            this._globalShieldValue.Pub(0f);
            return true;
        }

        public override void OnRemoved()
        {
            this._globalShieldValue.Pub(0f);
            foreach (BaseAbilityActor actor in Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(base.actor))
            {
                actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ChildShieldModifierName);
                if (!string.IsNullOrEmpty(this.config.ShieldOffModifierName))
                {
                    actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ShieldOffModifierName);
                }
            }
            Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(base.actor.runtimeID);
        }

        private enum State
        {
            Idle,
            ShieldBroken,
            Resuming
        }
    }
}

