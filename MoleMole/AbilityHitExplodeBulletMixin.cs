namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AbilityHitExplodeBulletMixin : BaseAbilityMixin
    {
        protected Dictionary<uint, AttackData> _bulletAttackDatas;
        protected List<EvtHittingOther> _evtsLs;
        protected HitExplodeBulletMixin baseConfig;

        public AbilityHitExplodeBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.baseConfig = (HitExplodeBulletMixin) config;
            this._bulletAttackDatas = new Dictionary<uint, AttackData>();
            this._evtsLs = new List<EvtHittingOther>();
        }

        private void ClearBullets()
        {
            if (this.baseConfig.RemoveClearType != BulletClearBehavior.DoNothing)
            {
                foreach (uint num in this._bulletAttackDatas.Keys)
                {
                    AbilityTriggerBullet actor = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(num);
                    if ((actor != null) && actor.IsActive())
                    {
                        if (this.baseConfig.RemoveClearType == BulletClearBehavior.ClearAndKillAndPlayExplodeEffect)
                        {
                            this.FireTriggerBulletHitExplodeEffect(actor, actor.triggerBullet.transform.position, actor.triggerBullet.transform.forward, false);
                        }
                        if (actor != null)
                        {
                            actor.Kill();
                        }
                    }
                }
                this._bulletAttackDatas.Clear();
            }
        }

        protected virtual void CreateBullet(HitExplodeTracingBulletMixinArgument arg, uint bulletRuntimeID, uint otherID)
        {
            string bulletTypeName = this.baseConfig.BulletTypeName;
            float speed = base.instancedAbility.Evaluate(this.baseConfig.BulletSpeed);
            if (arg != null)
            {
                if (arg.BulletName != null)
                {
                    bulletTypeName = arg.BulletName;
                }
                if (arg.RandomBulletNames != null)
                {
                    bulletTypeName = arg.RandomBulletNames[UnityEngine.Random.Range(0, arg.RandomBulletNames.Length)];
                }
                if (arg.BulletSpeed != null)
                {
                    speed = base.instancedAbility.Evaluate(arg.BulletSpeed);
                }
            }
            AbilityTriggerBullet bullet = Singleton<DynamicObjectManager>.Instance.CreateAbilityLinearTriggerBullet(bulletTypeName, base.actor, speed, this.baseConfig.Targetting, this.baseConfig.IgnoreTimeScale, bulletRuntimeID, base.instancedAbility.Evaluate(this.baseConfig.AliveDuration));
            if ((this.baseConfig.BulletEffect != null) && (this.baseConfig.BulletEffect.EffectPattern != null))
            {
                Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(this.baseConfig.BulletEffect.EffectPattern, bullet.triggerBullet, this.baseConfig.BulletEffectGround);
            }
            this.InitBulletForward(bullet);
            this.InitBulletForwardWithArgument(bullet, arg, otherID);
            this._bulletAttackDatas.Add(bullet.runtimeID, DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(base.actor, this.baseConfig.HitAnimEventID));
        }

        protected void FireTriggerBulletHitExplodeEffect(AbilityTriggerBullet bulletActor, Vector3 position, Vector3 forward, bool selfExplode = false)
        {
            if (selfExplode && (this.baseConfig.SelfExplodeEffect != null))
            {
                base.FireMixinEffect(this.baseConfig.SelfExplodeEffect, bulletActor.triggerBullet, position, forward, true);
            }
            else if ((this.baseConfig.ApplyDistinctHitExplodeEffectPattern && (this.baseConfig.HitExplodeEffectAir != null)) && (this.baseConfig.HitExplodeEffectGround != null))
            {
                float num = base.instancedAbility.Evaluate(this.baseConfig.DistinctHitExplodeHeight);
                if (bulletActor.triggerBullet.transform.position.y > num)
                {
                    base.FireMixinEffect(this.baseConfig.HitExplodeEffectAir, bulletActor.triggerBullet, position, forward, true);
                }
                else
                {
                    base.FireMixinEffect(this.baseConfig.HitExplodeEffectGround, bulletActor.triggerBullet, position, forward, true);
                }
            }
            else
            {
                base.FireMixinEffect(this.baseConfig.HitExplodeEffect, bulletActor.triggerBullet, position, forward, true);
            }
        }

        protected virtual void InitBulletForward(AbilityTriggerBullet bullet)
        {
            Vector3 forward;
            BaseMonoEntity attackTarget = base.entity.GetAttackTarget();
            if ((attackTarget == null) || !this.baseConfig.FaceTarget)
            {
                forward = base.entity.transform.forward;
            }
            else
            {
                forward = attackTarget.GetAttachPoint("RootNode").position - bullet.triggerBullet.transform.position;
                Quaternion from = Quaternion.LookRotation(base.entity.transform.forward);
                Quaternion to = Quaternion.LookRotation(forward);
                forward = (Vector3) (Quaternion.RotateTowards(from, to, 15f) * Vector3.forward);
            }
            if (this.baseConfig.IsFixedHeight)
            {
                forward.y = 0f;
            }
            bullet.triggerBullet.transform.forward = forward;
            bullet.triggerBullet.IgnoreTimeScale = this.baseConfig.IgnoreTimeScale;
        }

        protected virtual void InitBulletForwardWithArgument(AbilityTriggerBullet bullet, HitExplodeTracingBulletMixinArgument arg, uint otherID)
        {
            if ((arg != null) && (arg.XZAngleOffset != 0f))
            {
                bullet.triggerBullet.transform.Rotate(new Vector3(0f, arg.XZAngleOffset, 0f));
            }
        }

        protected virtual bool ListenBulletHit(EvtBulletHit evt)
        {
            if (!this._bulletAttackDatas.ContainsKey(evt.targetID))
            {
                return false;
            }
            AttackData attackData = this._bulletAttackDatas[evt.targetID];
            attackData.isFromBullet = true;
            bool flag = this.baseConfig.BulletHitType == BulletHitBehavior.DestroyAndDoExplodeDamage;
            bool flag2 = (this.baseConfig.BulletHitType == BulletHitBehavior.DestroyAndDoExplodeDamage) || (this.baseConfig.BulletHitType == BulletHitBehavior.DestroyAndDamageHitTarget);
            bool flag3 = true;
            bool flag4 = this.baseConfig.BulletHitType == BulletHitBehavior.NoDestroyAndRefresh;
            BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(evt.otherID);
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor(evt.otherID) as BaseAbilityActor;
            if (entity is MonoDummyDynamicObject)
            {
                flag2 = false;
                flag = false;
                flag3 = false;
                flag4 = false;
            }
            else if (evt.hitEnvironment)
            {
                flag2 = true;
                flag4 = false;
            }
            else if ((!evt.cannotBeReflected && (actor != null)) && actor.abilityState.ContainsState(AbilityState.ReflectBullet))
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtAfterBulletReflected(evt.otherID, evt.targetID, base.actor.runtimeID, this._bulletAttackDatas[evt.targetID]), MPEventDispatchMode.Normal);
                return false;
            }
            AbilityTriggerBullet bulletActor = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(evt.targetID);
            if (flag2)
            {
                if (bulletActor != null)
                {
                    bulletActor.Kill();
                }
                this._bulletAttackDatas.Remove(evt.targetID);
            }
            else
            {
                attackData = attackData.Clone();
            }
            if (flag4 && (bulletActor != null))
            {
                bulletActor.triggerBullet.ResetInside(this.baseConfig.ResetTime);
            }
            this._evtsLs.Clear();
            if (evt.hitEnvironment)
            {
                if (!evt.hitGround)
                {
                    return true;
                }
                EvtHittingOther item = new EvtHittingOther(base.actor.runtimeID, evt.otherID, attackData) {
                    hitCollision = evt.hitCollision
                };
                this._evtsLs.Add(item);
            }
            else
            {
                attackData.hitCollision = evt.hitCollision;
                this._evtsLs.Add(new EvtHittingOther(base.actor.runtimeID, evt.otherID, this.baseConfig.HitAnimEventID, attackData));
            }
            if (flag)
            {
                List<CollisionResult> list = CollisionDetectPattern.CylinderCollisionDetectBySphere(evt.hitCollision.hitPoint, evt.hitCollision.hitPoint, base.instancedAbility.Evaluate(this.baseConfig.HitExplodeRadius), 1f, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(base.actor.runtimeID, this.baseConfig.Targetting));
                float y = evt.hitCollision.hitPoint.y;
                for (int j = 0; j < list.Count; j++)
                {
                    CollisionResult result = list[j];
                    BaseMonoEntity collisionResultEntity = AttackPattern.GetCollisionResultEntity(result.entity);
                    if ((collisionResultEntity != null) && (collisionResultEntity.GetRuntimeID() != evt.otherID))
                    {
                        result.hitPoint.y = y;
                        AttackData data2 = attackData.Clone();
                        AttackResult.HitCollsion collsion = new AttackResult.HitCollsion {
                            hitDir = result.hitForward,
                            hitPoint = result.hitPoint
                        };
                        data2.hitCollision = collsion;
                        this._evtsLs.Add(new EvtHittingOther(base.actor.runtimeID, collisionResultEntity.GetRuntimeID(), this.baseConfig.HitAnimEventID, data2));
                    }
                }
            }
            if (flag3)
            {
                Vector3 hitPoint = evt.hitCollision.hitPoint;
                if (this.baseConfig.ExplodeEffectGround)
                {
                    hitPoint.y = 0f;
                }
                Vector3 hitDir = evt.hitCollision.hitDir;
                hitDir.y = 0f;
                bool selfExplode = evt.selfExplode;
                if (bulletActor != null)
                {
                    this.FireTriggerBulletHitExplodeEffect(bulletActor, hitPoint, hitDir, selfExplode);
                }
            }
            if ((this.baseConfig.HitExplodeActions.Length > 0) && (!evt.selfExplode || !this.baseConfig.MuteSelfHitExplodeActions))
            {
                for (int k = 0; k < this._evtsLs.Count; k++)
                {
                    if (base.actor.abilityPlugin != null)
                    {
                        base.actor.abilityPlugin.HandleActionTargetDispatch(this.baseConfig.HitExplodeActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(this._evtsLs[k].toID), evt);
                    }
                }
            }
            for (int i = 0; i < this._evtsLs.Count; i++)
            {
                EvtHittingOther other2 = this._evtsLs[i];
                if (this.baseConfig.IsHitChangeTargetDirection && (other2.attackData.hitEffect >= AttackResult.AnimatorHitEffect.ThrowUp))
                {
                    BaseAbilityActor actor2 = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(other2.toID);
                    if (actor2 != null)
                    {
                        actor2.entity.transform.forward = -other2.attackData.hitCollision.hitDir;
                    }
                }
                Singleton<EventManager>.Instance.FireEvent(other2, MPEventDispatchMode.Normal);
            }
            return true;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtBulletHit) && this.ListenBulletHit((EvtBulletHit) evt));
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            HitExplodeTracingBulletMixinArgument abilityArgument = evt.abilityArgument as HitExplodeTracingBulletMixinArgument;
            this.CreateBullet(abilityArgument, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), evt.otherID);
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtBulletHit>(base.actor.runtimeID);
            this._bulletAttackDatas.Clear();
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RemoveEventListener<EvtBulletHit>(base.actor.runtimeID);
            this.ClearBullets();
        }
    }
}

