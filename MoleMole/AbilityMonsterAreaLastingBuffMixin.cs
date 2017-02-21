namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;

    public class AbilityMonsterAreaLastingBuffMixin : BaseAbilityMixin
    {
        private EntityTimer _buffTimer;
        private AbilityTriggerField _fieldActor;
        private List<BaseAbilityActor> _insideActors;
        private State _state;
        private MonsterAreaLastingBuffMixin config;

        public AbilityMonsterAreaLastingBuffMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MonsterAreaLastingBuffMixin) config;
            this._buffTimer = new EntityTimer(instancedAbility.Evaluate(this.config.Duration), base.entity);
            this._buffTimer.SetActive(false);
            this._state = State.Idle;
            this._insideActors = new List<BaseAbilityActor>();
        }

        private void ApplyModifierOn(BaseAbilityActor actor)
        {
            if ((actor != null) && actor.IsActive())
            {
                for (int i = 0; i < this.config.ModifierNames.Length; i++)
                {
                    actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierNames[i]);
                }
            }
        }

        public override void Core()
        {
            this._buffTimer.Core(1f);
            if (this._buffTimer.isTimeUp)
            {
                this.StopAreaLastingBuff();
                if (!string.IsNullOrEmpty(this.config.BuffTimeRatioAnimatorParam))
                {
                    (base.actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(this.config.BuffTimeRatioAnimatorParam, 0f, false);
                }
            }
            else if (!string.IsNullOrEmpty(this.config.BuffTimeRatioAnimatorParam))
            {
                (base.actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(this.config.BuffTimeRatioAnimatorParam, this._buffTimer.GetTimingRatio(), false);
            }
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtFieldEnter)
            {
                return this.ListenFieldEnter((EvtFieldEnter) evt);
            }
            return ((evt is EvtFieldExit) && this.ListenFieldExit((EvtFieldExit) evt));
        }

        private bool ListenFieldEnter(EvtFieldEnter evt)
        {
            if (((this._state != State.Buffing) || (evt.targetID != this._fieldActor.runtimeID)) || (!this.config.IncludeSelf && (evt.otherID == base.actor.runtimeID)))
            {
                return false;
            }
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
            if (actor != null)
            {
                this.ApplyModifierOn(actor);
                this._insideActors.Add(actor);
            }
            return true;
        }

        private bool ListenFieldExit(EvtFieldExit evt)
        {
            if ((this._state == State.Buffing) && (evt.targetID == this._fieldActor.runtimeID))
            {
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
                if (actor != null)
                {
                    this.TryRemoveModifierOn(actor);
                    this._insideActors.Remove(actor);
                }
            }
            return false;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (this._state == State.Idle)
            {
                this.StartAreaLastingBuff();
            }
        }

        public override void OnAdded()
        {
            if (this.config.TriggerOnAdded)
            {
                this.StartAreaLastingBuff();
            }
        }

        private bool OnPostBeingHit(EvtBeingHit evt)
        {
            if ((this._state == State.Buffing) && (this.config.HitBreakType != MonsterAreaLastingBuffMixin.AreaLastingHitBreakType.Normal))
            {
                if (this.config.HitBreakType == MonsterAreaLastingBuffMixin.AreaLastingHitBreakType.ConvertAllHitsToLightHit)
                {
                    if (evt.attackData.attackerAniDamageRatio < evt.attackData.attackeeAniDefenceRatio)
                    {
                        evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Light;
                    }
                    if (evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light)
                    {
                        this.StopAreaLastingBuff();
                    }
                }
                else if ((this.config.HitBreakType == MonsterAreaLastingBuffMixin.AreaLastingHitBreakType.BreakingHitCancels) && (evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light))
                {
                    this.StopAreaLastingBuff();
                }
            }
            return false;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnPostBeingHit((EvtBeingHit) evt));
        }

        public override void OnRemoved()
        {
            if (this._state == State.Buffing)
            {
                this.StopAreaLastingBuff();
            }
        }

        private void StartAreaLastingBuff()
        {
            this._state = State.Buffing;
            this._fieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(base.entity.XZPosition, base.entity.transform.forward, base.actor, base.instancedAbility.Evaluate(this.config.Radius), this.config.Target, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), true);
            if (!this.config.TriggerOnAdded)
            {
                this._buffTimer.Reset(true);
            }
            base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.SelfLastingModifierName);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldExit>(base.actor.runtimeID);
        }

        private void StopAreaLastingBuff()
        {
            this._state = State.Idle;
            this._buffTimer.Reset(false);
            this._fieldActor.Kill();
            base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.SelfLastingModifierName);
            for (int i = 0; i < this._insideActors.Count; i++)
            {
                this.TryRemoveModifierOn(this._insideActors[i]);
            }
            this._insideActors.Clear();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldExit>(base.actor.runtimeID);
            if (!string.IsNullOrEmpty(this.config.BuffDurationEndTrigger))
            {
                base.entity.SetTrigger(this.config.BuffDurationEndTrigger);
            }
        }

        private void TryRemoveModifierOn(BaseAbilityActor actor)
        {
            if (actor != null)
            {
                for (int i = 0; i < this.config.ModifierNames.Length; i++)
                {
                    actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ModifierNames[i]);
                }
            }
        }

        private enum State
        {
            Idle,
            Buffing
        }
    }
}

