namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FollowAvatarAndBossState : FollowAvatarAndTargetState
    {
        private BaseMonoEntity _bossTarget;
        private Vector3[] _followPositions;
        private const int LPF_POSITIONS_COUNT = 60;
        public bool needLPF;

        public FollowAvatarAndBossState(MainCameraFollowState followState) : base(followState)
        {
            this.needLPF = true;
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

        private Vector3 GetAverageBossTargetPosition()
        {
            if (!this.needLPF)
            {
                return this.bossTarget.XZPosition;
            }
            Vector3 zero = Vector3.zero;
            for (int i = 0; i < this._followPositions.Length; i++)
            {
                zero += this._followPositions[i];
            }
            Vector3 start = (Vector3) (zero / ((float) this._followPositions.Length));
            Debug.DrawLine(start, start + Vector3.up);
            return start;
        }

        protected override void PreCalculate()
        {
            BaseMonoEntity bossTarget;
            Vector3 averageBossTargetPosition;
            BaseMonoEntity attackTarget = base._owner.avatar.AttackTarget;
            if (attackTarget != null)
            {
                if ((attackTarget is MonoBodyPartEntity) && !((MonoBodyPartEntity) attackTarget).IsCameraTargetable)
                {
                    bossTarget = this.bossTarget;
                    averageBossTargetPosition = this.GetAverageBossTargetPosition();
                }
                else
                {
                    bossTarget = attackTarget;
                    averageBossTargetPosition = attackTarget.XZPosition;
                }
            }
            else
            {
                bossTarget = this.bossTarget;
                averageBossTargetPosition = this.GetAverageBossTargetPosition();
            }
            Vector3 vector2 = averageBossTargetPosition - base._owner.avatar.XZPosition;
            vector2.y = 0f;
            base._disEntity2Target = vector2.magnitude;
            base._dirCamera2Entity = base._owner.avatar.XZPosition - base._owner.followData.cameraPosition;
            this._dirCamera2Entity.y = 0f;
            base._disCamera2Entity = this._dirCamera2Entity.magnitude;
            base._dirCamera2Target = averageBossTargetPosition - base._owner.followData.cameraPosition;
            this._dirCamera2Target.y = 0f;
            base._disCamera2Target = this._dirCamera2Target.magnitude;
            base._entityCollisionRadius = base.GetColliderRadius(base._owner.avatar);
            base._targetCollisionRadius = base.GetColliderRadius(bossTarget);
        }

        private void SampleBossTargetPosition()
        {
            if (this.needLPF)
            {
                for (int i = this._followPositions.Length - 1; i > 0; i--)
                {
                    this._followPositions[i] = this._followPositions[i - 1];
                }
                this._followPositions[0] = this.bossTarget.XZPosition;
            }
        }

        public override void Update()
        {
            base._feasibleAngles.Clear();
            if ((this.bossTarget == null) || !this.bossTarget.IsActive())
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

        public BaseMonoEntity bossTarget
        {
            get
            {
                return this._bossTarget;
            }
            set
            {
                this._bossTarget = value;
                if (this.needLPF)
                {
                    for (int i = 0; i < this._followPositions.Length; i++)
                    {
                        this._followPositions[i] = this._bossTarget.XZPosition;
                    }
                }
            }
        }
    }
}

