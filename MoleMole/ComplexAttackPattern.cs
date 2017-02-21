namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class ComplexAttackPattern
    {
        private static RaycastHit _wallHit;

        public static void AnimatedColliderDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
        {
            <AnimatedColliderDetectAttack>c__AnonStoreyBE ybe = new <AnimatedColliderDetectAttack>c__AnonStoreyBE {
                attacker = attacker,
                attackName = attackName,
                patternConfig = patternConfig
            };
            AnimatedColliderDetect detect = (AnimatedColliderDetect) ybe.patternConfig;
            ybe.hitboxCollider = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoAnimatedHitboxDetect>(detect.ColliderEntryName, ybe.attacker.GetRuntimeID());
            Transform followTarget = ybe.attacker.transform;
            if (!string.IsNullOrEmpty(detect.FollowAttachPoint) && (ybe.attacker is BaseMonoAnimatorEntity))
            {
                followTarget = ((BaseMonoAnimatorEntity) ybe.attacker).GetAttachPoint(detect.FollowAttachPoint);
            }
            ybe.hitboxCollider.Init((BaseMonoEntity) ybe.attacker, layerMask, followTarget, detect.Follow, false);
            ybe.hitboxCollider.SetIgnoreTimeScale(detect.IgnoreTimeScale);
            ybe.hitboxCollider.SetFollowOwnerTimeScale(detect.FollowOwnerTimeScale);
            ybe.hitboxCollider.dontDestroyWhenOwnerEvade = detect.dontDestroyWhenEvade;
            if (detect.brokenEnemyDragged)
            {
                ybe.attacker.onAnimatedHitBoxCreated(ybe.hitboxCollider, ybe.patternConfig);
            }
            if (detect.DestroyOnOwnerBeHitCanceled)
            {
                ybe.hitboxCollider.EnableOnOwnerBeHitCanceledDestroySelf();
            }
            if (detect.EnableHitWallStop)
            {
                ybe.hitboxCollider.EnableWallHitCheck();
            }
            if (detect.DestroyOnHitWall)
            {
                ybe.hitboxCollider.EnableWallHitDestroy(detect.HitWallDestroyEffect);
            }
            ybe.hitboxCollider.transform.position = ybe.attacker.XZPosition;
            ybe.hitboxCollider.transform.forward = ybe.attacker.transform.forward;
            ybe.hitboxCollider.triggerEnterCallback = new Action<Collider, string>(ybe.<>m__A6);
        }

        public static void FanCollisionWithHeightDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
        {
            FanCollisionWithHeightDetect detect = (FanCollisionWithHeightDetect) patternConfig;
            Vector3 vector = new Vector3(attacker.XZPosition.x, (attacker.XZPosition.y + detect.CenterYOffset) + (!detect.FollowRootNodeY ? 0f : attacker.GetAttachPoint("RootNode").transform.position.y), attacker.XZPosition.z);
            Vector3 fanCenterPoint = vector;
            fanCenterPoint.y += detect.Height * 0.5f;
            List<CollisionResult> list = CollisionDetectPattern.MeleeFanCollisionDetectBySphere(fanCenterPoint, detect.OffsetZ, attacker.FaceDirection, detect.Radius, detect.FanAngle, detect.MeleeRadius, detect.MeleeFanAngle, layerMask);
            fanCenterPoint = vector;
            fanCenterPoint.y -= detect.Height * 0.5f;
            List<CollisionResult> list2 = CollisionDetectPattern.MeleeFanCollisionDetectBySphere(fanCenterPoint, detect.OffsetZ, attacker.FaceDirection, detect.Radius, detect.FanAngle, detect.MeleeRadius, detect.MeleeFanAngle, layerMask);
            List<CollisionResult> results = list2;
            for (int i = 0; i < list.Count; i++)
            {
                CollisionResult item = list[i];
                bool flag = false;
                for (int j = 0; j < list2.Count; j++)
                {
                    CollisionResult result2 = list2[j];
                    if (result2.entity.GetRuntimeID() == item.entity.GetRuntimeID())
                    {
                        flag = true;
                        result2.hitPoint.y = vector.y;
                        break;
                    }
                }
                if (!flag)
                {
                    results.Add(item);
                }
            }
            AttackPattern.TestAndActHit(attackName, patternConfig, attacker, results);
        }

        public static void HitscanDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
        {
            HitscanDetect detect = (HitscanDetect) patternConfig;
            Vector3 xZPosition = attacker.XZPosition;
            xZPosition.y = detect.CenterYOffset;
            xZPosition.z += detect.OffsetZ;
            List<CollisionResult> results = CollisionDetectPattern.HitscanSingleDetect(xZPosition, attacker.FaceDirection, detect.MaxHitDistance, layerMask);
            AttackPattern.TestAndActHit(attackName, patternConfig, attacker, results);
        }

        public static void RectCollisionWithHeightDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
        {
            RectCollisionWithHeightDetect detect = (RectCollisionWithHeightDetect) patternConfig;
            Vector3 vector = new Vector3(attacker.XZPosition.x, attacker.XZPosition.y + detect.CenterYOffset, attacker.XZPosition.z);
            Vector3 recCenterPoint = vector;
            recCenterPoint.y += detect.Height * 0.5f;
            List<CollisionResult> list = CollisionDetectPattern.RectCollisionDetectByRay(recCenterPoint, detect.OffsetZ, attacker.FaceDirection, detect.Width, detect.Distance, layerMask);
            recCenterPoint = vector;
            recCenterPoint.y -= detect.Height * 0.5f;
            List<CollisionResult> list2 = CollisionDetectPattern.RectCollisionDetectByRay(recCenterPoint, detect.OffsetZ, attacker.FaceDirection, detect.Width, detect.Distance, layerMask);
            List<CollisionResult> results = list2;
            for (int i = 0; i < list.Count; i++)
            {
                CollisionResult item = list[i];
                bool flag = false;
                for (int j = 0; j < list2.Count; j++)
                {
                    CollisionResult result2 = list2[j];
                    if (result2.entity.GetRuntimeID() == item.entity.GetRuntimeID())
                    {
                        flag = true;
                        result2.hitPoint.y = vector.y;
                        break;
                    }
                }
                if (!flag)
                {
                    results.Add(item);
                }
            }
            AttackPattern.TestAndActHit(attackName, patternConfig, attacker, results);
        }

        public static void StaticHitBoxDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
        {
            <StaticHitBoxDetectAttack>c__AnonStoreyC0 yc = new <StaticHitBoxDetectAttack>c__AnonStoreyC0 {
                attacker = attacker,
                attackName = attackName,
                patternConfig = patternConfig
            };
            StaticHitBoxDetect detect = (StaticHitBoxDetect) yc.patternConfig;
            yc.hitboxCollider = (yc.attacker as IStaticHitBox).GetStaticHitBox();
            yc.hitboxCollider.useOwnerCenterForRetreatDirection = detect.UseOwnerCenterForRetreatDirection;
            if (detect.Enable)
            {
                yc.hitboxCollider.SetColliderScale(detect.SizeRatio, detect.LengthRatio);
                switch (detect.ResetType)
                {
                    case StaticHitBoxDetect.HitBoxResetType.WithInside:
                        yc.hitboxCollider.ResetTriggerWithResetInside();
                        break;

                    case StaticHitBoxDetect.HitBoxResetType.WithoutInside:
                        yc.hitboxCollider.ResetTriggerWithoutResetInside();
                        break;
                }
                yc.hitboxCollider.EnableHitBoxDetect(true);
                yc.hitboxCollider.triggerEnterCallback = new Action<Collider>(yc.<>m__A8);
            }
            else
            {
                yc.hitboxCollider.EnableHitBoxDetect(false);
                yc.hitboxCollider.ResetColliderScale();
            }
        }

        public static void TargetLockedAnimatedColliderDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
        {
            Vector3 vector2;
            <TargetLockedAnimatedColliderDetectAttack>c__AnonStoreyBF ybf = new <TargetLockedAnimatedColliderDetectAttack>c__AnonStoreyBF {
                attacker = attacker,
                attackName = attackName,
                patternConfig = patternConfig
            };
            TargetLockedAnimatedColliderDetect detect = (TargetLockedAnimatedColliderDetect) ybf.patternConfig;
            ybf.hitboxCollider = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoAnimatedHitboxDetect>(detect.ColliderEntryName, ybf.attacker.GetRuntimeID());
            ybf.hitboxCollider.Init((BaseMonoEntity) ybf.attacker, layerMask, null, false, detect.StopOnFirstContact);
            ybf.hitboxCollider.dontDestroyWhenOwnerEvade = detect.dontDestroyWhenEvade;
            if (detect.brokenEnemyDragged)
            {
                ybf.attacker.onAnimatedHitBoxCreated(ybf.hitboxCollider, ybf.patternConfig);
            }
            float maxLockDistance = detect.MaxLockDistance;
            Vector3 forward = ybf.attacker.transform.forward;
            if (detect.DestroyOnOwnerBeHitCanceled)
            {
                ybf.hitboxCollider.EnableOnOwnerBeHitCanceledDestroySelf();
            }
            if ((ybf.attacker.AttackTarget != null) && ybf.attacker.AttackTarget.IsActive())
            {
                if (detect.LockX)
                {
                    maxLockDistance = Mathf.Min(Miscs.DistancForVec3IgnoreY(ybf.attacker.XZPosition, ybf.attacker.AttackTarget.XZPosition), detect.MaxLockDistance);
                    forward = Vector3.Normalize(ybf.attacker.AttackTarget.XZPosition - ybf.attacker.XZPosition);
                    vector2 = (Vector3) (ybf.attacker.transform.InverseTransformDirection(forward) * maxLockDistance);
                }
                else
                {
                    maxLockDistance = Miscs.DistancForVec3IgnoreY(ybf.attacker.XZPosition, ybf.attacker.AttackTarget.XZPosition);
                    vector2 = new Vector3(0f, 0f, Mathf.Min(maxLockDistance, detect.MaxLockDistance));
                }
            }
            else
            {
                float b = detect.MaxLockDistance * 0.6f;
                maxLockDistance = b;
                if (Physics.Raycast(ybf.attacker.XZPosition, ybf.attacker.transform.forward, out _wallHit, maxLockDistance, ((int) 1) << InLevelData.STAGE_COLLIDER_LAYER))
                {
                    maxLockDistance = _wallHit.distance;
                }
                maxLockDistance = Mathf.Min(maxLockDistance, b);
                vector2 = new Vector3(0f, 0f, Mathf.Min(maxLockDistance, detect.MaxLockDistance));
            }
            float num3 = UnityEngine.Random.Range(0f, detect.ScatteringDistance);
            float f = UnityEngine.Random.Range((float) 0f, (float) 360f);
            vector2.x += num3 * Mathf.Sin(f);
            vector2.z += num3 * Mathf.Cos(f);
            ybf.hitboxCollider.transform.forward = forward;
            ybf.hitboxCollider.transform.position = ybf.attacker.transform.position + ybf.attacker.transform.TransformDirection(vector2);
            ybf.hitboxCollider.triggerEnterCallback = new Action<Collider, string>(ybf.<>m__A7);
        }

        [CompilerGenerated]
        private sealed class <AnimatedColliderDetectAttack>c__AnonStoreyBE
        {
            internal IAttacker attacker;
            internal string attackName;
            internal MonoAnimatedHitboxDetect hitboxCollider;
            internal ConfigEntityAttackPattern patternConfig;

            internal void <>m__A6(Collider other, string overrideAnimEventID)
            {
                Vector3 xZPosition;
                Vector3 vector3;
                BaseMonoEntity componentInParent = other.gameObject.GetComponentInParent<BaseMonoEntity>();
                if (this.hitboxCollider.useOwnerCenterForRetreatDirection && (this.hitboxCollider.owner != null))
                {
                    xZPosition = this.hitboxCollider.owner.XZPosition;
                    xZPosition.y = this.hitboxCollider.collideCenterTransform.position.y;
                }
                else
                {
                    xZPosition = this.hitboxCollider.collideCenterTransform.position;
                }
                Vector3 hitPoint = other.ClosestPointOnBounds(xZPosition);
                if (this.hitboxCollider.useFixedReteatDirection)
                {
                    vector3 = this.hitboxCollider.CalculateFixedRetreatDirection(hitPoint);
                }
                else
                {
                    vector3 = hitPoint - xZPosition;
                }
                vector3.y = 0f;
                vector3.Normalize();
                if (vector3 == Vector3.zero)
                {
                    vector3 = other.transform.position - this.attacker.XZPosition;
                    vector3.y = 0f;
                    vector3.Normalize();
                }
                CollisionResult result = new CollisionResult(componentInParent, hitPoint, vector3);
                List<CollisionResult> results = new List<CollisionResult> {
                    result
                };
                string attackName = (overrideAnimEventID == null) ? this.attackName : overrideAnimEventID;
                AttackPattern.TestAndActHit(attackName, this.patternConfig, this.attacker, results);
            }
        }

        [CompilerGenerated]
        private sealed class <StaticHitBoxDetectAttack>c__AnonStoreyC0
        {
            internal IAttacker attacker;
            internal string attackName;
            internal MonoStaticHitboxDetect hitboxCollider;
            internal ConfigEntityAttackPattern patternConfig;

            internal void <>m__A8(Collider other)
            {
                Vector3 xZPosition;
                BaseMonoEntity componentInParent = other.gameObject.GetComponentInParent<BaseMonoEntity>();
                if (this.hitboxCollider.useOwnerCenterForRetreatDirection && (this.hitboxCollider.owner != null))
                {
                    xZPosition = this.hitboxCollider.owner.XZPosition;
                    xZPosition.y = this.hitboxCollider.collideCenterTransform.position.y;
                }
                else
                {
                    xZPosition = this.hitboxCollider.collideCenterTransform.position;
                }
                Vector3 hitPoint = other.ClosestPointOnBounds(xZPosition);
                Vector3 hitForward = hitPoint - xZPosition;
                hitForward.y = 0f;
                hitForward.Normalize();
                if (hitForward == Vector3.zero)
                {
                    hitForward = other.transform.position - this.attacker.XZPosition;
                    hitForward.y = 0f;
                    hitForward.Normalize();
                }
                CollisionResult result = new CollisionResult(componentInParent, hitPoint, hitForward);
                List<CollisionResult> results = new List<CollisionResult> {
                    result
                };
                AttackPattern.TestAndActHit(this.attackName, this.patternConfig, this.attacker, results);
            }
        }

        [CompilerGenerated]
        private sealed class <TargetLockedAnimatedColliderDetectAttack>c__AnonStoreyBF
        {
            internal IAttacker attacker;
            internal string attackName;
            internal MonoAnimatedHitboxDetect hitboxCollider;
            internal ConfigEntityAttackPattern patternConfig;

            internal void <>m__A7(Collider other, string overrideAnimEventID)
            {
                BaseMonoEntity componentInParent = other.gameObject.GetComponentInParent<BaseMonoEntity>();
                Vector3 hitPoint = other.ClosestPointOnBounds(this.hitboxCollider.collideCenterTransform.position);
                Vector3 vector3 = hitPoint - this.hitboxCollider.collideCenterTransform.position;
                Vector3 normalized = vector3.normalized;
                if (normalized == Vector3.zero)
                {
                    normalized = other.transform.position - this.attacker.XZPosition;
                    normalized.y = 0f;
                }
                CollisionResult result = new CollisionResult(componentInParent, hitPoint, normalized);
                List<CollisionResult> results = new List<CollisionResult> {
                    result
                };
                string attackName = (overrideAnimEventID == null) ? this.attackName : overrideAnimEventID;
                AttackPattern.TestAndActHit(attackName, this.patternConfig, this.attacker, results);
            }
        }
    }
}

