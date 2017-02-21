namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityDefendMixin : BaseAbilityMixin
    {
        private ActorModifier _defendDurationModifier;
        private EntityTimer _defendFailTimer;
        private ActorModifier _defendPerfectDurationModifier;
        private float _defendPerfectEndTime;
        private EntityTimer _defendPerfectFailTimer;
        private float _defendPerfectStartTime;
        private float _defendWindow;
        private bool _isDefendingPerfect;
        private State _state;
        private DefendMixin config;

        public AbilityDefendMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (DefendMixin) config;
            this._defendWindow = instancedAbility.Evaluate(this.config.DefendWindow);
            this._defendPerfectStartTime = instancedAbility.Evaluate(this.config.DefendPerfectStartTime);
            this._defendPerfectEndTime = instancedAbility.Evaluate(this.config.DefendPerfectEndTime);
            this._defendFailTimer = new EntityTimer(this._defendWindow);
            this._defendPerfectFailTimer = new EntityTimer(this._defendPerfectStartTime);
            this._isDefendingPerfect = false;
            this._defendFailTimer.SetActive(false);
            this._defendPerfectFailTimer.SetActive(false);
            this._state = State.Idle;
        }

        public override void Core()
        {
            this._defendFailTimer.Core(1f);
            if (this._isDefendingPerfect)
            {
                this._defendPerfectFailTimer.Core(1f);
            }
            if ((this._defendFailTimer.timer > this._defendPerfectStartTime) && !this._isDefendingPerfect)
            {
                this._isDefendingPerfect = true;
                this._defendPerfectFailTimer.Reset(true);
                if (this.config.DefendPerfectDurationModifierName != null)
                {
                    this._defendPerfectDurationModifier = base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.DefendPerfectDurationModifierName);
                }
            }
            if ((this._defendPerfectFailTimer.timer > this._defendPerfectEndTime) && this._isDefendingPerfect)
            {
                this._isDefendingPerfect = false;
                this._defendPerfectFailTimer.Reset(false);
                if (this._defendPerfectDurationModifier != null)
                {
                    base.actor.abilityPlugin.TryRemoveModifier(this._defendPerfectDurationModifier);
                    this._defendPerfectDurationModifier = null;
                }
                if (this._state == State.Defending)
                {
                    base.actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
                    this._state = State.Idle;
                }
            }
            if (this._defendFailTimer.isTimeUp)
            {
                if (this._defendDurationModifier != null)
                {
                    base.actor.abilityPlugin.TryRemoveModifier(this._defendDurationModifier);
                    this._defendDurationModifier = null;
                }
                this._isDefendingPerfect = false;
                this._defendFailTimer.Reset(false);
                this._defendPerfectFailTimer.Reset(false);
                if (this._state == State.Defending)
                {
                    base.actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
                    this._state = State.Idle;
                }
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.DefendStartActions, base.instancedAbility, base.instancedModifier, null, null);
            Singleton<EventManager>.Instance.FireEvent(new EvtDefendStart(base.actor.runtimeID), MPEventDispatchMode.Normal);
            this._defendFailTimer.Reset(true);
            if (!string.IsNullOrEmpty(this.config.DefendDurationModifierName))
            {
                this._defendDurationModifier = base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.DefendDurationModifierName);
            }
            if (this._state == State.Idle)
            {
                base.actor.AddAbilityState(AbilityState.BlockAnimEventAttack, true);
            }
            this._state = State.Defending;
        }

        private bool OnBeingHit(EvtBeingHit evt)
        {
            if (this._defendFailTimer.isActive && !this._defendFailTimer.isTimeUp)
            {
                if (this.config.DefendReplaceAttackEffect != null)
                {
                    evt.attackData.attackEffectPattern = this.config.DefendReplaceAttackEffect;
                }
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.DefendSuccessActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
                if (this._isDefendingPerfect)
                {
                    base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.DefendSuccessPerfectActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
                }
            }
            return true;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnBeingHit((EvtBeingHit) evt));
        }

        private enum State
        {
            Idle,
            Defending
        }
    }
}

