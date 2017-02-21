namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    public class AbilityBrokenEnemyDraggedByHitBoxMixin : BaseAbilityMixin
    {
        private Dictionary<Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>, int> _addedVelocityActorsAndIndexDic;
        private List<Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>>> _draggedEnemyList;
        private float _pullVelocity;
        private List<Tuple<MonoAnimatedHitboxDetect, HashSet<uint>>> _touchedEnemyList;
        private BrokenEnemyDraggedByHitBoxMixin config;

        public AbilityBrokenEnemyDraggedByHitBoxMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._addedVelocityActorsAndIndexDic = new Dictionary<Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>, int>();
            this._draggedEnemyList = new List<Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>>>();
            this._touchedEnemyList = new List<Tuple<MonoAnimatedHitboxDetect, HashSet<uint>>>();
            this.config = (BrokenEnemyDraggedByHitBoxMixin) config;
            this._pullVelocity = instancedAbility.Evaluate(this.config.PullVelocity);
        }

        public override void Core()
        {
            foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> tuple in this._draggedEnemyList)
            {
                if (tuple.Item1 == null)
                {
                    foreach (BaseAbilityActor actor in tuple.Item2)
                    {
                        this.RemoveAdditiveVelocity(actor, tuple.Item1);
                    }
                }
                else
                {
                    foreach (BaseAbilityActor actor2 in tuple.Item2)
                    {
                        this.SetAdditiveVelocity(actor2, tuple.Item1);
                    }
                }
            }
        }

        private void DoSetAdditiveVelocity(BaseAbilityActor targetActor, MonoAnimatedHitboxDetect hitbox, Vector3 additiveVelocity)
        {
            if (targetActor != null)
            {
                Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor> key = new Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>(hitbox, targetActor);
                if (!this._addedVelocityActorsAndIndexDic.ContainsKey(key))
                {
                    targetActor.entity.SetHasAdditiveVelocity(true);
                    int num = targetActor.entity.AddAdditiveVelocity((Vector3) (additiveVelocity * this._pullVelocity));
                    this._addedVelocityActorsAndIndexDic.Add(key, num);
                }
                else
                {
                    targetActor.entity.SetHasAdditiveVelocity(true);
                    int index = this._addedVelocityActorsAndIndexDic[key];
                    targetActor.entity.SetAdditiveVelocityOfIndex((Vector3) (additiveVelocity * this._pullVelocity), index);
                }
            }
        }

        private void HitBoxTriggerEnterCallback(MonoAnimatedHitboxDetect hitbox, Collider other)
        {
            if (hitbox.entryName == this.config.ColliderEntryName)
            {
                BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(componentInParent.GetRuntimeID());
                if (actor != null)
                {
                    switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(componentInParent.GetRuntimeID()))
                    {
                        case 3:
                        case 4:
                        {
                            HashSet<BaseAbilityActor> set = new HashSet<BaseAbilityActor>(Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(actor));
                            if (set.Contains(base.actor))
                            {
                                bool flag = false;
                                foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> tuple in this._draggedEnemyList)
                                {
                                    if (tuple.Item1 == hitbox)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    HashSet<uint> set2 = null;
                                    foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<uint>> tuple2 in this._touchedEnemyList)
                                    {
                                        if (tuple2.Item1 == hitbox)
                                        {
                                            set2 = tuple2.Item2;
                                            if (tuple2.Item2.Contains(actor.runtimeID))
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    if (set2 != null)
                                    {
                                        set2.Add(actor.runtimeID);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        public override void OnAdded()
        {
            (base.entity as IAttacker).onAnimatedHitBoxCreatedCallBack += new AnimatedHitBoxCreatedHandler(this.onAnimatedHitBoxCreated);
        }

        private void onAnimatedHitBoxCreated(MonoAnimatedHitboxDetect hitBox, ConfigEntityAttackPattern attackPattern)
        {
            if ((attackPattern is AnimatedColliderDetect) || (attackPattern is TargetLockedAnimatedColliderDetect))
            {
                if (attackPattern is AnimatedColliderDetect)
                {
                    AnimatedColliderDetect detect = attackPattern as AnimatedColliderDetect;
                    if (!detect.brokenEnemyDragged)
                    {
                        return;
                    }
                }
                else if (attackPattern is TargetLockedAnimatedColliderDetect)
                {
                    TargetLockedAnimatedColliderDetect detect2 = attackPattern as TargetLockedAnimatedColliderDetect;
                    if (!detect2.brokenEnemyDragged)
                    {
                        return;
                    }
                }
                if (hitBox.entryName == this.config.ColliderEntryName)
                {
                    hitBox.enemyEnterCallback = (Action<MonoAnimatedHitboxDetect, Collider>) Delegate.Combine(hitBox.enemyEnterCallback, new Action<MonoAnimatedHitboxDetect, Collider>(this.HitBoxTriggerEnterCallback));
                    hitBox.destroyCallback = (Action<MonoAnimatedHitboxDetect>) Delegate.Combine(hitBox.destroyCallback, new Action<MonoAnimatedHitboxDetect>(this.onAnimatedHitBoxDestroy));
                    this._draggedEnemyList.Add(new Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>>(hitBox, new HashSet<BaseAbilityActor>()));
                    this._touchedEnemyList.Add(new Tuple<MonoAnimatedHitboxDetect, HashSet<uint>>(hitBox, new HashSet<uint>()));
                }
            }
        }

        private void onAnimatedHitBoxDestroy(MonoAnimatedHitboxDetect hitbox)
        {
            List<Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>>> list = new List<Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>>>();
            foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> tuple in this._draggedEnemyList)
            {
                if (tuple.Item1 == hitbox)
                {
                    list.Add(tuple);
                    foreach (BaseAbilityActor actor in tuple.Item2)
                    {
                        this.RemoveAdditiveVelocity(actor, hitbox);
                    }
                }
            }
            foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> tuple2 in list)
            {
                this._draggedEnemyList.Remove(tuple2);
            }
            List<Tuple<MonoAnimatedHitboxDetect, HashSet<uint>>> list2 = new List<Tuple<MonoAnimatedHitboxDetect, HashSet<uint>>>();
            foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<uint>> tuple3 in this._touchedEnemyList)
            {
                if (tuple3.Item1 == hitbox)
                {
                    list2.Add(tuple3);
                }
            }
            foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<uint>> tuple4 in list2)
            {
                this._touchedEnemyList.Remove(tuple4);
            }
            List<Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>> list3 = new List<Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>>();
            foreach (Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor> tuple5 in this._addedVelocityActorsAndIndexDic.Keys)
            {
                if (tuple5.Item1 == hitbox)
                {
                    list3.Add(tuple5);
                }
            }
        }

        private bool OnAttackLandedOther(EvtAttackLanded evt)
        {
            MonoAnimatedHitboxDetect hitbox = null;
            bool flag = false;
            foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<uint>> tuple in this._touchedEnemyList)
            {
                if ((tuple.Item1 != null) && tuple.Item2.Contains(evt.attackeeID))
                {
                    hitbox = tuple.Item1;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return false;
            }
            if (evt.attackResult.hitEffect <= AttackResult.AnimatorHitEffect.Light)
            {
                return false;
            }
            BaseAbilityActor item = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID);
            if (item == null)
            {
                return false;
            }
            HashSet<BaseAbilityActor> set = null;
            foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> tuple2 in this._draggedEnemyList)
            {
                if (tuple2.Item1 == hitbox)
                {
                    set = tuple2.Item2;
                    if (tuple2.Item2.Contains(item))
                    {
                        return false;
                    }
                }
            }
            if (set != null)
            {
                set.Add(item);
                this.SetAdditiveVelocity(item, hitbox);
            }
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            if (evt is EvtAttackLanded)
            {
                this.OnAttackLandedOther((EvtAttackLanded) evt);
            }
            return false;
        }

        public override void OnRemoved()
        {
            (base.entity as IAttacker).onAnimatedHitBoxCreatedCallBack -= new AnimatedHitBoxCreatedHandler(this.onAnimatedHitBoxCreated);
            foreach (Tuple<MonoAnimatedHitboxDetect, HashSet<BaseAbilityActor>> tuple in this._draggedEnemyList)
            {
                foreach (BaseAbilityActor actor in tuple.Item2)
                {
                    this.RemoveAdditiveVelocity(actor, tuple.Item1);
                }
            }
        }

        private void RemoveAdditiveVelocity(BaseAbilityActor targetActor, MonoAnimatedHitboxDetect hitbox)
        {
            if (((targetActor != null) && (targetActor.isAlive != 0)) && (targetActor.entity != null))
            {
                Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor> key = new Tuple<MonoAnimatedHitboxDetect, BaseAbilityActor>(hitbox, targetActor);
                if (this._addedVelocityActorsAndIndexDic.ContainsKey(key))
                {
                    int index = this._addedVelocityActorsAndIndexDic[key];
                    targetActor.entity.SetAdditiveVelocityOfIndex(Vector3.zero, index);
                    targetActor.entity.SetHasAdditiveVelocity(false);
                    this._addedVelocityActorsAndIndexDic.Remove(key);
                }
            }
        }

        private void SetAdditiveVelocity(BaseAbilityActor enemyActor, MonoAnimatedHitboxDetect hitbox)
        {
            if (((enemyActor != null) && (enemyActor.isAlive != 0)) && (enemyActor.entity != null))
            {
                Vector3 additiveVelocity = hitbox.collideCenterTransform.position - enemyActor.entity.XZPosition;
                additiveVelocity.y = 0f;
                additiveVelocity.Normalize();
                this.DoSetAdditiveVelocity(enemyActor, hitbox, additiveVelocity);
            }
        }
    }
}

