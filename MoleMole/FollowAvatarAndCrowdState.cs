namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class FollowAvatarAndCrowdState : FollowAvatarAndTargetState
    {
        private Vector3[] _followPositions;
        private const int LPF_POSITIONS_COUNT = 60;

        public FollowAvatarAndCrowdState(MainCameraFollowState followState) : base(followState)
        {
            this._followPositions = new Vector3[60];
        }

        protected override void CalcTargetPolarIgnoreWallHit()
        {
            BaseMonoEntity attackTarget = base._owner.avatar.AttackTarget;
            if (attackTarget != null)
            {
                if ((attackTarget is MonoBodyPartEntity) && !((MonoBodyPartEntity) attackTarget).IsCameraTargetable)
                {
                    this.CalcTargetPoloarIngoreWallHitWhenHasNoAttackTarget();
                }
                else
                {
                    base.CalcTargetPoloarIngoreWallHitWhenHasAttackTarget();
                }
            }
            else
            {
                this.CalcTargetPoloarIngoreWallHitWhenHasNoAttackTarget();
            }
        }

        private void CalcTargetPoloarIngoreWallHitWhenHasNoAttackTarget()
        {
            float targetsAngle = Mathf.Acos(Vector3.Dot(base._dirCamera2Entity, base._dirCamera2Target) / (base._disCamera2Entity * base._disCamera2Target)) * 57.29578f;
            if (base._disEntity2Target < base._disCamera2Entity)
            {
                float num2 = ((base._disEntity2Target * base._disEntity2Target) + (base._disCamera2Entity * base._disCamera2Entity)) - (base._disCamera2Target * base._disCamera2Target);
                if (num2 > 0f)
                {
                    float num3 = Mathf.Atan2(base._disEntity2Target, base._disCamera2Entity) * 57.29578f;
                    base.DoAdjToCorrectTargetsAngle(num3, false);
                }
                else
                {
                    base.DoAdjToCorrectTargetsAngle(targetsAngle, true);
                }
            }
            else if (targetsAngle > 45f)
            {
                base.DoAdjToCorrectTargetsAngle(45f, false);
            }
            else
            {
                base.DoAdjToCorrectTargetsAngle(targetsAngle, false);
            }
        }

        private void DoUpdateWhenHasNoAttackTarget()
        {
            if (base._isCameraSolveWallHit)
            {
                base.DoWhenSolveWallHit();
            }
            else
            {
                base.ReplaceCamera();
                base._owner.anchorPolar = base._targetPolar;
            }
        }

        private Vector3 GetCrowdPosition()
        {
            Vector3 zero = Vector3.zero;
            for (int i = 0; i < this._followPositions.Length; i++)
            {
                zero += this._followPositions[i];
            }
            return (Vector3) (zero / ((float) this._followPositions.Length));
        }

        protected override void PreCalculate()
        {
            BaseMonoEntity entity;
            Vector3 crowdPosition;
            BaseMonoEntity attackTarget = base._owner.avatar.AttackTarget;
            if (attackTarget != null)
            {
                if ((attackTarget is MonoBodyPartEntity) && !((MonoBodyPartEntity) attackTarget).IsCameraTargetable)
                {
                    entity = null;
                    crowdPosition = this.GetCrowdPosition();
                }
                else
                {
                    entity = attackTarget;
                    crowdPosition = attackTarget.XZPosition;
                }
            }
            else
            {
                entity = null;
                crowdPosition = this.GetCrowdPosition();
            }
            Vector3 vector2 = crowdPosition - base._owner.avatar.XZPosition;
            vector2.y = 0f;
            base._disEntity2Target = vector2.magnitude;
            base._dirCamera2Entity = base._owner.avatar.XZPosition - base._owner.followData.cameraPosition;
            this._dirCamera2Entity.y = 0f;
            base._disCamera2Entity = this._dirCamera2Entity.magnitude;
            base._dirCamera2Target = crowdPosition - base._owner.followData.cameraPosition;
            this._dirCamera2Target.y = 0f;
            base._disCamera2Target = this._dirCamera2Target.magnitude;
            base._entityCollisionRadius = base.GetColliderRadius(base._owner.avatar);
            if (entity != null)
            {
                base._targetCollisionRadius = base.GetColliderRadius(entity);
            }
            else
            {
                base._targetCollisionRadius = 0f;
            }
        }

        private void SampleBossTargetPosition()
        {
            Vector3 vector = new Vector3(0f, 0f, 0f);
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            for (int i = 0; i < allMonsters.Count; i++)
            {
                BaseMonoMonster monster = allMonsters[i];
                vector += monster.XZPosition;
            }
            Vector3 vector2 = (Vector3) (vector / ((float) allMonsters.Count));
            for (int j = this._followPositions.Length - 1; j > 0; j--)
            {
                this._followPositions[j] = this._followPositions[j - 1];
            }
            this._followPositions[0] = vector2;
        }

        public override void Update()
        {
            base._feasibleAngles.Clear();
            if (Singleton<MonsterManager>.Instance.GetAllMonsters().Count == 0)
            {
                base._owner.TransitBaseState(base._owner.followAvatarState, true);
            }
            else
            {
                this.SampleBossTargetPosition();
                base._hasSetTargetForwardDelta = false;
                base._owner.posLerpRatio = 1f;
                BaseMonoEntity attackTarget = base._owner.avatar.AttackTarget;
                if (attackTarget != null)
                {
                    if ((attackTarget is MonoBodyPartEntity) && !((MonoBodyPartEntity) attackTarget).IsCameraTargetable)
                    {
                        this.DoUpdateWhenHasNoAttackTarget();
                    }
                    else
                    {
                        base.DoUpdateWhenHasAttackTarget();
                    }
                }
                else
                {
                    this.DoUpdateWhenHasNoAttackTarget();
                }
                if (base._owner.recoverState.active)
                {
                    base._owner.recoverState.CancelForwardDeltaAngleRecover();
                }
                if (base._hasSetTargetForwardDelta)
                {
                    if ((base._targetForwardDelta * base._owner.forwardDeltaAngle) > 0f)
                    {
                        base._owner.forwardDeltaAngle = Mathf.Lerp(base._owner.forwardDeltaAngle, base._targetForwardDelta, Time.deltaTime * 5f);
                    }
                    else
                    {
                        base._owner.forwardDeltaAngle = base._targetForwardDelta;
                    }
                }
            }
        }
    }
}

