namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MPAbilityHitExplodeBulletMixin_TotallyLocal : AbilityHitExplodeBulletMixin
    {
        public MPAbilityHitExplodeBulletMixin_TotallyLocal(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
        }

        protected override bool ListenBulletHit(EvtBulletHit evt)
        {
            if (!base._bulletAttackDatas.ContainsKey(evt.targetID))
            {
                return false;
            }
            BaseMPIdentity identity = Singleton<MPManager>.Instance.TryGetIdentity(evt.otherID);
            if (((identity != null) && !identity.isAuthority) && !identity.remoteMode.IsRemoteReceive())
            {
                return false;
            }
            AttackData attackData = base._bulletAttackDatas[evt.targetID];
            attackData.isFromBullet = true;
            bool flag = base.baseConfig.BulletHitType == BulletHitBehavior.DestroyAndDoExplodeDamage;
            bool flag2 = (base.baseConfig.BulletHitType == BulletHitBehavior.DestroyAndDoExplodeDamage) || (base.baseConfig.BulletHitType == BulletHitBehavior.DestroyAndDamageHitTarget);
            bool flag3 = true;
            bool flag4 = base.baseConfig.BulletHitType == BulletHitBehavior.NoDestroyAndRefresh;
            BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(evt.otherID);
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor(evt.otherID) as BaseAbilityActor;
            if ((entity is MonoDummyDynamicObject) || ((identity != null) && !identity.remoteMode.IsRemoteReceive()))
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
                Singleton<EventManager>.Instance.FireEvent(new EvtAfterBulletReflected(evt.otherID, evt.targetID, base.actor.runtimeID, base._bulletAttackDatas[evt.targetID]), MPEventDispatchMode.Normal);
                return false;
            }
            AbilityTriggerBullet bulletActor = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(evt.targetID);
            if (flag2)
            {
                bulletActor.Kill();
                base._bulletAttackDatas.Remove(evt.targetID);
            }
            else
            {
                attackData = attackData.Clone();
            }
            if (flag4)
            {
                bulletActor.triggerBullet.ResetInside(base.baseConfig.ResetTime);
            }
            base._evtsLs.Clear();
            if (evt.hitEnvironment)
            {
                if (!evt.hitGround)
                {
                    return true;
                }
                EvtHittingOther item = new EvtHittingOther(base.actor.runtimeID, evt.otherID, attackData) {
                    hitCollision = evt.hitCollision
                };
                base._evtsLs.Add(item);
            }
            else
            {
                attackData.hitCollision = evt.hitCollision;
                base._evtsLs.Add(new EvtHittingOther(base.actor.runtimeID, evt.otherID, base.baseConfig.HitAnimEventID, attackData));
            }
            if (flag)
            {
                List<CollisionResult> list = CollisionDetectPattern.CylinderCollisionDetectBySphere(evt.hitCollision.hitPoint, evt.hitCollision.hitPoint, base.instancedAbility.Evaluate(base.baseConfig.HitExplodeRadius), 1f, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(base.actor.runtimeID, base.baseConfig.Targetting));
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
                        base._evtsLs.Add(new EvtHittingOther(base.actor.runtimeID, collisionResultEntity.GetRuntimeID(), base.baseConfig.HitAnimEventID, data2));
                    }
                }
            }
            if (flag3)
            {
                Vector3 hitPoint = evt.hitCollision.hitPoint;
                if (base.baseConfig.ExplodeEffectGround)
                {
                    hitPoint.y = 0f;
                }
                Vector3 hitDir = evt.hitCollision.hitDir;
                hitDir.y = 0f;
                base.FireTriggerBulletHitExplodeEffect(bulletActor, hitPoint, hitDir, false);
            }
            if (base.baseConfig.HitExplodeActions.Length > 0)
            {
                for (int k = 0; k < base._evtsLs.Count; k++)
                {
                    base.actor.abilityPlugin.HandleActionTargetDispatch(base.baseConfig.HitExplodeActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(base._evtsLs[k].toID), evt);
                }
            }
            for (int i = 0; i < base._evtsLs.Count; i++)
            {
                EvtHittingOther other2 = base._evtsLs[i];
                AttackPattern.SendHitEvent(base.actor.runtimeID, other2.toID, other2.animEventID, other2.hitCollision, other2.attackData, false, MPEventDispatchMode.CheckRemoteMode);
            }
            return true;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            HitExplodeTracingBulletMixinArgument abilityArgument = evt.abilityArgument as HitExplodeTracingBulletMixinArgument;
            uint nextNonSyncedRuntimeID = Singleton<RuntimeIDManager>.Instance.GetNextNonSyncedRuntimeID(6);
            this.CreateBullet(abilityArgument, nextNonSyncedRuntimeID, evt.otherID);
        }
    }
}

