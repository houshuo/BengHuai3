namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityBlackHoleMixin : BaseAbilityMixin
    {
        private Dictionary<BaseAbilityActor, int> _addedVelocityActorsAndIndexDic;
        private int _blackHoleEffectIx;
        private EntityTimer _blackHoleTimer;
        private AbilityTriggerField _fieldActor;
        private List<BaseAbilityActor> _insideActors;
        private float _pullVelocity;
        private BlackHoleMixin config;

        public AbilityBlackHoleMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._addedVelocityActorsAndIndexDic = new Dictionary<BaseAbilityActor, int>();
            this.config = (BlackHoleMixin) config;
            this._blackHoleTimer = new EntityTimer(instancedAbility.Evaluate(this.config.Duration));
            this._blackHoleTimer.SetActive(false);
            this._pullVelocity = instancedAbility.Evaluate(this.config.PullVelocity);
            this._insideActors = new List<BaseAbilityActor>();
        }

        public override void Core()
        {
            if (this._blackHoleTimer.isActive)
            {
                this._blackHoleTimer.Core(1f);
                if (this._blackHoleTimer.isTimeUp)
                {
                    this.KillBlackHole(true);
                }
                else
                {
                    for (int i = 0; i < this._insideActors.Count; i++)
                    {
                        BaseAbilityActor targetActor = this._insideActors[i];
                        if ((targetActor != null) && targetActor.IsActive())
                        {
                            if ((this._pullVelocity > 0f) && (Miscs.DistancForVec3IgnoreY(targetActor.entity.XZPosition, this._fieldActor.triggerField.transform.position) < 0.3f))
                            {
                                this.RemoveAdditiveVelocity(targetActor);
                            }
                            else
                            {
                                Vector3 additiveVelocity = this._fieldActor.triggerField.transform.position - targetActor.entity.XZPosition;
                                additiveVelocity.y = 0f;
                                additiveVelocity.Normalize();
                                this.SetAdditiveVelocity(targetActor, additiveVelocity);
                            }
                        }
                    }
                }
            }
        }

        private void KillBlackHole(bool doExplodeHit)
        {
            if ((this._fieldActor != null) && (this._fieldActor.triggerField != null))
            {
                List<uint> insideRuntimeIDs = this._fieldActor.GetInsideRuntimeIDs();
                this._fieldActor.triggerField.transform.position.y = 1f;
                for (int i = 0; i < insideRuntimeIDs.Count; i++)
                {
                    uint runtimeID = insideRuntimeIDs[i];
                    BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(runtimeID);
                    if (((actor != null) && (actor.gameObject != null)) && ((this.config.ModifierNames != null) && (this.config.ModifierNames.Length > 0)))
                    {
                        int index = 0;
                        int length = this.config.ModifierNames.Length;
                        while (index < length)
                        {
                            if (actor.abilityPlugin != null)
                            {
                                actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ModifierNames[index]);
                            }
                            index++;
                        }
                    }
                }
                if (doExplodeHit && (this.config.ExplodeAnimEventID != null))
                {
                    List<CollisionResult> list2 = CollisionDetectPattern.CylinderCollisionDetectBySphere(this._fieldActor.triggerField.XZPosition, this._fieldActor.triggerField.XZPosition, base.instancedAbility.Evaluate(this.config.Radius), 2f, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(base.actor.runtimeID, MixinTargetting.Enemy));
                    for (int j = 0; j < list2.Count; j++)
                    {
                        CollisionResult result = list2[j];
                        BaseMonoEntity collisionResultEntity = AttackPattern.GetCollisionResultEntity(result.entity);
                        if (collisionResultEntity != null)
                        {
                            Singleton<EventManager>.Instance.FireEvent(new EvtHittingOther(base.actor.runtimeID, collisionResultEntity.GetRuntimeID(), this.config.ExplodeAnimEventID, result.hitPoint, result.hitForward), MPEventDispatchMode.Normal);
                        }
                    }
                }
                base.FireMixinEffect(this.config.DestroyEffect, this._fieldActor.triggerField, false);
                this._fieldActor.Kill();
                this._blackHoleTimer.timespan = base.instancedAbility.Evaluate(this.config.Duration);
                this._blackHoleTimer.Reset(false);
                if ((this.config.CreationEffect != null) && (this.config.CreationEffect.EffectPattern != null))
                {
                    Singleton<EffectManager>.Instance.SetDestroyIndexedEffectPattern(this._blackHoleEffectIx);
                }
                foreach (KeyValuePair<BaseAbilityActor, int> pair in this._addedVelocityActorsAndIndexDic)
                {
                    if ((pair.Key != null) && (pair.Key.entity != null))
                    {
                        pair.Key.entity.SetAdditiveVelocityOfIndex(Vector3.zero, pair.Value);
                        pair.Key.entity.SetHasAdditiveVelocity(false);
                    }
                }
                this._addedVelocityActorsAndIndexDic.Clear();
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
            if ((this._fieldActor == null) || (this._fieldActor.runtimeID != evt.targetID))
            {
                return false;
            }
            BaseAbilityActor item = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
            if ((item == null) || (item.gameObject == null))
            {
                return false;
            }
            item.entity.SetHasAdditiveVelocity(true);
            if ((this.config.ModifierNames != null) && (this.config.ModifierNames.Length > 0))
            {
                int index = 0;
                int length = this.config.ModifierNames.Length;
                while (index < length)
                {
                    item.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierNames[index]);
                    index++;
                }
            }
            this._insideActors.Add(item);
            return true;
        }

        private bool ListenFieldExit(EvtFieldExit evt)
        {
            if ((this._fieldActor == null) || (this._fieldActor.runtimeID != evt.targetID))
            {
                return false;
            }
            BaseAbilityActor item = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
            if ((item != null) && (item.gameObject != null))
            {
                if (this._insideActors.Contains(item))
                {
                    this._insideActors.Remove(item);
                    this.RemoveAdditiveVelocity(item);
                }
                if ((this.config.ModifierNames != null) && (this.config.ModifierNames.Length > 0))
                {
                    int index = 0;
                    int length = this.config.ModifierNames.Length;
                    while (index < length)
                    {
                        item.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ModifierNames[index]);
                        index++;
                    }
                }
            }
            return true;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            Vector3 vector;
            if (this._blackHoleTimer.isActive)
            {
                this.KillBlackHole(true);
            }
            BaseMonoEntity entity = null;
            if (evt.otherID != 0)
            {
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
                if ((actor != null) && (actor.entity != null))
                {
                    entity = actor.entity;
                }
            }
            if (entity != null)
            {
                vector = entity.XZPosition + new Vector3(0f, 0.5f, 0f);
            }
            else
            {
                Vector3 origin = base.entity.XZPosition + new Vector3(0f, 0.5f, 0f);
                vector = CollisionDetectPattern.GetRaycastPoint(origin, base.entity.transform.forward, base.instancedAbility.Evaluate(this.config.CreationZOffset), 0.2f, ((int) 1) << InLevelData.STAGE_COLLIDER_LAYER);
            }
            this._fieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(vector, base.entity.transform.forward, base.actor, base.instancedAbility.Evaluate(this.config.Radius), MixinTargetting.Enemy, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), false);
            if ((this.config.CreationEffect != null) && (this.config.CreationEffect.EffectPattern != null))
            {
                this._blackHoleEffectIx = Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(this.config.CreationEffect.EffectPattern, this._fieldActor.triggerField);
            }
            if (this.config.ApplyAttackerWitchTimeRatio && (evt.TriggerEvent != null))
            {
                EvtEvadeSuccess triggerEvent = evt.TriggerEvent as EvtEvadeSuccess;
                if (triggerEvent != null)
                {
                    MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(triggerEvent.attackerID);
                    if (actor2 != null)
                    {
                        ConfigMonsterAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(actor2.config, triggerEvent.skillID);
                        if (event2 != null)
                        {
                            this._blackHoleTimer.timespan *= event2.AttackProperty.WitchTimeRatio;
                        }
                    }
                }
            }
            this._blackHoleTimer.Reset(true);
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldExit>(base.actor.runtimeID);
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldExit>(base.actor.runtimeID);
            if (this._blackHoleTimer.isActive && !this._blackHoleTimer.isTimeUp)
            {
                this.KillBlackHole(false);
            }
        }

        private void RemoveAdditiveVelocity(BaseAbilityActor targetActor)
        {
            if (this._addedVelocityActorsAndIndexDic.ContainsKey(targetActor))
            {
                int index = this._addedVelocityActorsAndIndexDic[targetActor];
                targetActor.entity.SetAdditiveVelocityOfIndex(Vector3.zero, index);
                targetActor.entity.SetHasAdditiveVelocity(false);
                this._addedVelocityActorsAndIndexDic.Remove(targetActor);
            }
        }

        private void SetAdditiveVelocity(BaseAbilityActor targetActor, Vector3 additiveVelocity)
        {
            if (targetActor != null)
            {
                if (!this._addedVelocityActorsAndIndexDic.ContainsKey(targetActor))
                {
                    targetActor.entity.SetHasAdditiveVelocity(true);
                    int num = targetActor.entity.AddAdditiveVelocity((Vector3) (additiveVelocity * this._pullVelocity));
                    this._addedVelocityActorsAndIndexDic.Add(targetActor, num);
                }
                else
                {
                    int index = this._addedVelocityActorsAndIndexDic[targetActor];
                    targetActor.entity.SetAdditiveVelocityOfIndex((Vector3) (additiveVelocity * this._pullVelocity), index);
                }
            }
        }
    }
}

