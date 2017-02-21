namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityEvadeMixin : BaseAbilityMixin
    {
        protected EvadeEntityDummy _dummyActor;
        protected EntityTimer _evadeTimer;
        protected EntityTimer _extendedBlockAttackTimer;
        protected State _state;
        protected EvadeMixin config;

        public AbilityEvadeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (EvadeMixin) config;
            this._evadeTimer = new EntityTimer(instancedAbility.Evaluate(this.config.EvadeWindow));
            this._evadeTimer.SetActive(false);
            this._extendedBlockAttackTimer = new EntityTimer(instancedAbility.Evaluate(this.config.EvadeSuccessExtendedInvincibleWindow));
            this._extendedBlockAttackTimer.SetActive(false);
            this._state = State.Idle;
        }

        public override void Core()
        {
            if (this._state == State.Evading)
            {
                this._evadeTimer.Core(1f);
                if (this._evadeTimer.isTimeUp)
                {
                    this.EvadeFail();
                }
            }
            else if (this._state == State.EvadeSuccessed)
            {
                this._extendedBlockAttackTimer.Core(1f);
                if (this._extendedBlockAttackTimer.isTimeUp)
                {
                    base.actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
                    base.entity.SetCountedIsGhost(false);
                    this._state = State.Idle;
                }
            }
        }

        protected virtual void EvadeFail()
        {
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.EvadeFailActions, base.instancedAbility, base.instancedModifier, null, null);
            this._evadeTimer.Reset(false);
            this._dummyActor.Kill();
            base.actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
            base.entity.SetCountedIsGhost(false);
            this._state = State.Idle;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if ((this._dummyActor != null) && this._dummyActor.IsEntityExists())
            {
                this._dummyActor.Kill();
            }
            uint runtimeID = Singleton<DynamicObjectManager>.Instance.CreateEvadeDummy(base.actor.runtimeID, this.config.EvadeDummyName, base.actor.entity.XZPosition, base.actor.entity.transform.forward);
            this._dummyActor = Singleton<EventManager>.Instance.GetActor<EvadeEntityDummy>(runtimeID);
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.EvadeStartActions, base.instancedAbility, base.instancedModifier, null, null);
            Singleton<EventManager>.Instance.FireEvent(new EvtEvadeStart(base.actor.runtimeID), MPEventDispatchMode.Normal);
            if (this._state == State.Idle)
            {
                base.actor.AddAbilityState(AbilityState.BlockAnimEventAttack, true);
                base.entity.SetCountedIsGhost(true);
            }
            this._evadeTimer.Reset(true);
            this._extendedBlockAttackTimer.Reset(true);
            this._state = State.Evading;
        }

        protected virtual bool OnEvadeSuccess(EvtEvadeSuccess evt)
        {
            if (this._state != State.Evading)
            {
                return false;
            }
            this._state = State.EvadeSuccessed;
            this._extendedBlockAttackTimer.Reset(true);
            base.instancedAbility.CurrentTriggerEvent = evt;
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.EvadeSuccessActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackerID), evt);
            return true;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtEvadeSuccess) && this.OnEvadeSuccess((EvtEvadeSuccess) evt));
        }

        public override void OnRemoved()
        {
            if ((this._state == State.Evading) || (this._state == State.EvadeSuccessed))
            {
                base.entity.SetCountedIsGhost(false);
            }
        }

        protected enum State
        {
            Idle,
            Evading,
            EvadeSuccessed
        }
    }
}

