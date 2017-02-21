namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilitySmokeBombMixin : BaseAbilityMixin
    {
        private AbilityTriggerField _alliedFieldActor;
        private EntityTimer _buffTimer;
        private Vector3 _dir;
        private AbilityTriggerField _enemyFieldActor;
        private List<BaseAbilityActor> _insideAlliedActors;
        private List<BaseAbilityActor> _insideEnemyActors;
        private bool _isSmokeAvaliable;
        private bool _isSmokeOn;
        private Vector3 _position;
        private List<MonoEffect> _smokeOffEffects;
        private List<MonoEffect> _smokeOnEffects;
        private SmokeBombMixin config;

        public AbilitySmokeBombMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (SmokeBombMixin) config;
            this._buffTimer = new EntityTimer(instancedAbility.Evaluate(this.config.Duration), base.entity);
            this._buffTimer.SetActive(false);
            this._isSmokeOn = false;
            this._isSmokeAvaliable = false;
            this._insideAlliedActors = new List<BaseAbilityActor>();
            this._insideEnemyActors = new List<BaseAbilityActor>();
        }

        private void ApplyModifierOn(BaseAbilityActor actor, string[] modifiers)
        {
            if ((actor != null) && actor.IsActive())
            {
                for (int i = 0; i < modifiers.Length; i++)
                {
                    actor.abilityPlugin.ApplyModifier(base.instancedAbility, modifiers[i]);
                }
            }
        }

        public override void Core()
        {
            this._buffTimer.Core(1f);
            if (this._buffTimer.isTimeUp)
            {
                this.StopAreaLastingBuff();
            }
        }

        private void DestroyEffects(List<MonoEffect> _effects)
        {
            if (_effects != null)
            {
                foreach (MonoEffect effect in _effects)
                {
                    ParticleSystem componentInChildren = effect.GetComponentInChildren<ParticleSystem>();
                    if (componentInChildren != null)
                    {
                        componentInChildren.Stop(true);
                    }
                }
                _effects.Clear();
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
            if (this._isSmokeOn)
            {
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
                if (evt.targetID == this._alliedFieldActor.runtimeID)
                {
                    this.OnAlliedEnter(actor);
                    return true;
                }
                if (evt.targetID == this._enemyFieldActor.runtimeID)
                {
                    this.OnEnemyEnter(actor);
                    return true;
                }
            }
            return false;
        }

        private bool ListenFieldExit(EvtFieldExit evt)
        {
            if (this._isSmokeOn)
            {
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
                if (evt.targetID == this._alliedFieldActor.runtimeID)
                {
                    this.OnAlliedExit(actor);
                    return true;
                }
                if (evt.targetID == this._enemyFieldActor.runtimeID)
                {
                    this.OnEnemyExit(actor);
                    return true;
                }
            }
            return false;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (!this._isSmokeOn)
            {
                this.StartAreaLastingBuff();
            }
        }

        public override void OnAdded()
        {
        }

        private void OnAlliedEnter(BaseAbilityActor actor)
        {
            this._insideAlliedActors.Add(actor);
            this.ApplyModifierOn(actor, this.config.Modifiers);
            if (this._isSmokeAvaliable)
            {
                this.ApplyModifierOn(actor, this.config.InSmokeModifiers);
            }
        }

        private void OnAlliedExit(BaseAbilityActor actor)
        {
            this._insideAlliedActors.Remove(actor);
            this.TryRemoveModifierOn(actor, this.config.Modifiers);
            this.TryRemoveModifierOn(actor, this.config.InSmokeModifiers);
        }

        private void OnEnemyEnter(BaseAbilityActor actor)
        {
            this._insideEnemyActors.Add(actor);
            if (this._isSmokeAvaliable)
            {
                this._isSmokeAvaliable = false;
                for (int i = 0; i < this._insideAlliedActors.Count; i++)
                {
                    this.TryRemoveModifierOn(this._insideAlliedActors[i], this.config.InSmokeModifiers);
                }
                this.SetSmokeOffEffects();
            }
        }

        private void OnEnemyExit(BaseAbilityActor actor)
        {
            this._insideEnemyActors.Remove(actor);
            if (!this._isSmokeAvaliable && (this._insideEnemyActors.Count == 0))
            {
                this._isSmokeAvaliable = true;
                for (int i = 0; i < this._insideAlliedActors.Count; i++)
                {
                    this.ApplyModifierOn(this._insideAlliedActors[i], this.config.InSmokeModifiers);
                }
                this.SetSmokeOnEffects();
            }
        }

        public override void OnRemoved()
        {
            if (this._isSmokeOn)
            {
                this.StopAreaLastingBuff();
            }
        }

        private void SetSmokeOffEffects()
        {
            if ((this._smokeOffEffects == null) || (this._smokeOffEffects.Count == 0))
            {
                this._smokeOffEffects = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(this.config.SmokeOffEffect.EffectPattern, this._position, this._dir, Vector3.one, base.entity);
            }
            this.DestroyEffects(this._smokeOnEffects);
        }

        private void SetSmokeOnEffects()
        {
            if ((this._smokeOnEffects == null) || (this._smokeOnEffects.Count == 0))
            {
                this._smokeOnEffects = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(this.config.SmokeOnEffect.EffectPattern, this._position, this._dir, Vector3.one, base.entity);
            }
            this.DestroyEffects(this._smokeOffEffects);
        }

        private void StartAreaLastingBuff()
        {
            this._isSmokeOn = true;
            this._isSmokeAvaliable = true;
            this._position = base.entity.XZPosition;
            this._dir = base.entity.transform.forward;
            this._insideAlliedActors.Clear();
            this._insideEnemyActors.Clear();
            this._alliedFieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(this._position, base.entity.transform.forward, base.actor, base.instancedAbility.Evaluate(this.config.Radius), MixinTargetting.Allied, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), false);
            this._enemyFieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(this._position, base.entity.transform.forward, base.actor, base.instancedAbility.Evaluate(this.config.Radius), MixinTargetting.Enemy, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), false);
            this.SetSmokeOnEffects();
            this._buffTimer.Reset(true);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldExit>(base.actor.runtimeID);
        }

        private void StopAreaLastingBuff()
        {
            this._isSmokeOn = false;
            this._isSmokeAvaliable = false;
            this._buffTimer.Reset(false);
            this._alliedFieldActor.Kill();
            this._enemyFieldActor.Kill();
            for (int i = 0; i < this._insideAlliedActors.Count; i++)
            {
                this.TryRemoveModifierOn(this._insideAlliedActors[i], this.config.InSmokeModifiers);
                this.TryRemoveModifierOn(this._insideAlliedActors[i], this.config.Modifiers);
            }
            this._insideAlliedActors.Clear();
            this._insideEnemyActors.Clear();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldExit>(base.actor.runtimeID);
            this.DestroyEffects(this._smokeOnEffects);
            this.DestroyEffects(this._smokeOffEffects);
        }

        private void TryRemoveModifierOn(BaseAbilityActor actor, string[] modifiers)
        {
            if (actor != null)
            {
                for (int i = 0; i < modifiers.Length; i++)
                {
                    actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, modifiers[i]);
                }
            }
        }
    }
}

