namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityTriggerFieldExMixin : BaseAbilityMixin
    {
        private EntityTimer _createEffectDelayTimer;
        private int _creationEffectIx;
        private int _durationEffectIx;
        private AbilityTriggerField _fieldActor;
        private List<BaseAbilityActor> _insideActors;
        private EntityTimer _timer;
        private TriggerFieldExMixin config;

        public AbilityTriggerFieldExMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (TriggerFieldExMixin) config;
            this._timer = new EntityTimer(instancedAbility.Evaluate(this.config.Duration));
            this._timer.SetActive(false);
            this._createEffectDelayTimer = new EntityTimer(this.config.CreateEffectDelay);
            this._insideActors = new List<BaseAbilityActor>();
        }

        private void ApplyModifierOn(BaseAbilityActor actor)
        {
            if ((actor != null) && actor.IsActive())
            {
                for (int i = 0; i < this.config.TargetModifiers.Length; i++)
                {
                    actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.TargetModifiers[i]);
                }
            }
        }

        public override void Core()
        {
            if (this._createEffectDelayTimer.isActive && !this._createEffectDelayTimer.isTimeUp)
            {
                this._createEffectDelayTimer.Core(1f);
                if (this._createEffectDelayTimer.isTimeUp)
                {
                    this.StartField();
                }
            }
            if (this._timer.isActive && !this._timer.isTimeUp)
            {
                this._timer.Core(1f);
                if (this._timer.isTimeUp)
                {
                    this.StopField();
                }
            }
        }

        private void CreateField(EvtAbilityStart evt)
        {
            Vector3 vector;
            if (this._timer.isActive)
            {
                this.StopField();
            }
            BaseMonoEntity attackTarget = null;
            float maxDistance = base.instancedAbility.Evaluate(this.config.CreationZOffset);
            float num2 = base.instancedAbility.Evaluate(this.config.CreationXOffset);
            switch (this.config.TriggerPositionType)
            {
                case TriggerFieldExMixin.PositionType.Caster:
                    attackTarget = base.actor.entity;
                    break;

                case TriggerFieldExMixin.PositionType.AttackTarget:
                    attackTarget = base.entity.GetAttackTarget();
                    if (attackTarget == null)
                    {
                        maxDistance += base.instancedAbility.Evaluate(this.config.NoAttackTargetZOffset);
                    }
                    break;

                case TriggerFieldExMixin.PositionType.Target:
                    if (evt.otherID != 0)
                    {
                        BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
                        if ((actor != null) && (actor.entity != null))
                        {
                            attackTarget = actor.entity;
                        }
                    }
                    break;
            }
            Vector3 vector2 = (Vector3) (Vector3.Cross(Vector3.up, base.entity.transform.forward).normalized * num2);
            if (attackTarget != null)
            {
                vector = (attackTarget.XZPosition + new Vector3(0f, 0.5f, 0f)) + vector2;
            }
            else
            {
                attackTarget = base.actor.entity;
                Vector3 origin = (base.entity.XZPosition + new Vector3(0f, 0.5f, 0f)) + vector2;
                vector = CollisionDetectPattern.GetRaycastPoint(origin, base.entity.transform.forward, maxDistance, 0.2f, ((int) 1) << InLevelData.STAGE_COLLIDER_LAYER);
            }
            this._fieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(vector, attackTarget.transform.forward, base.actor, base.instancedAbility.Evaluate(this.config.Radius), this.config.Targetting, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), this.config.Follow);
        }

        private bool ListenAvatarSwapOut(EvtAvatarSwapOutStart evt)
        {
            if (((evt.targetID == base.actor.runtimeID) && this._timer.isActive) && (!this._timer.isTimeUp && this.config.DestoryAfterSwitch))
            {
                this.StopField();
            }
            return false;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtFieldEnter)
            {
                return this.ListenFieldEnter((EvtFieldEnter) evt);
            }
            if (evt is EvtFieldExit)
            {
                return this.ListenFieldExit((EvtFieldExit) evt);
            }
            return ((evt is EvtAvatarSwapOutStart) && this.ListenAvatarSwapOut((EvtAvatarSwapOutStart) evt));
        }

        private bool ListenFieldEnter(EvtFieldEnter evt)
        {
            if ((this._fieldActor == null) || (this._fieldActor.runtimeID != evt.targetID))
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
            if ((this._fieldActor != null) && (this._fieldActor.runtimeID == evt.targetID))
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
            this.CreateField(evt);
            this.StartCreateEffect();
            if (this.config.ApplyAttackerWitchTimeRatio && (evt.TriggerEvent != null))
            {
                EvtEvadeSuccess triggerEvent = evt.TriggerEvent as EvtEvadeSuccess;
                if (triggerEvent != null)
                {
                    MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(triggerEvent.attackerID);
                    if (actor != null)
                    {
                        ConfigMonsterAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(actor.config, triggerEvent.skillID);
                        if (event2 != null)
                        {
                            this._timer.timespan = base.instancedAbility.Evaluate(this.config.Duration) * event2.AttackProperty.WitchTimeRatio;
                        }
                    }
                }
            }
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldExit>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapOutStart>(base.actor.runtimeID);
            if (this.config.TriggerOnAdded)
            {
                this.CreateField(null);
                this.StartCreateEffect();
            }
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldExit>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarSwapOutStart>(base.actor.runtimeID);
            if ((this._timer.isActive && !this._timer.isTimeUp) || this.config.TriggerOnAdded)
            {
                this.StopField();
            }
        }

        private void StartCreateEffect()
        {
            this._createEffectDelayTimer.Reset(true);
            if (this.config.CreationEffect != null)
            {
                this._creationEffectIx = Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(this.config.CreationEffect.EffectPattern, this._fieldActor.triggerField);
            }
        }

        private void StartDestroyEffect()
        {
            if (this.config.DestroyEffect != null)
            {
                Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(this.config.DestroyEffect.EffectPattern, this._fieldActor.triggerField);
            }
        }

        private void StartField()
        {
            if (!this.config.TriggerOnAdded)
            {
                this._timer.Reset(true);
            }
            if ((this.config.DurationEffect != null) && (this.config.DurationEffect.EffectPattern != null))
            {
                this._durationEffectIx = Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(this.config.DurationEffect.EffectPattern, this._fieldActor.triggerField);
            }
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.OnStartCasterActions, base.instancedAbility, base.instancedModifier, base.actor, null);
        }

        private void StopField()
        {
            this._timer.Reset(false);
            this.StartDestroyEffect();
            this._fieldActor.Kill();
            for (int i = 0; i < this._insideActors.Count; i++)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.OnDestroyTargetActions, base.instancedAbility, base.instancedModifier, this._insideActors[i], null);
            }
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.OnDestroyCasterActions, base.instancedAbility, base.instancedModifier, base.actor, null);
            for (int j = 0; j < this._insideActors.Count; j++)
            {
                this.TryRemoveModifierOn(this._insideActors[j]);
            }
            this._insideActors.Clear();
            if ((this.config.DurationEffect != null) && (this.config.DurationEffect.EffectPattern != null))
            {
                Singleton<EffectManager>.Instance.SetDestroyIndexedEffectPattern(this._durationEffectIx);
            }
            if ((this.config.CreationEffect != null) && (this.config.CreationEffect.EffectPattern != null))
            {
                Singleton<EffectManager>.Instance.SetDestroyIndexedEffectPattern(this._creationEffectIx);
            }
        }

        private void TryRemoveModifierOn(BaseAbilityActor actor)
        {
            if (actor != null)
            {
                for (int i = 0; i < this.config.TargetModifiers.Length; i++)
                {
                    actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.TargetModifiers[i]);
                }
            }
        }
    }
}

