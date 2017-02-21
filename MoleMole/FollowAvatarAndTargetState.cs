namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class FollowAvatarAndTargetState : BaseFollowBaseState
    {
        private float _accelerationSolvingWallHit;
        private int _bioTargetCameraMode;
        private float _currentAnchorPolar;
        protected Vector3 _dirCamera2Entity;
        protected Vector3 _dirCamera2Target;
        private float _disableLerpSumUpTime;
        protected float _disCamera2Entity;
        protected float _disCamera2Target;
        protected float _disEntity2Target;
        protected float _entityCollisionRadius;
        protected List<float> _feasibleAngles;
        protected bool _hasSetTargetForwardDelta;
        protected bool _isCameraSolveWallHit;
        private bool _isCurrentWallHit;
        private float _minSpeedSolvingWallHit;
        [ShowInInspector]
        private FollowMode _mode;
        private List<MotionAngleInfo> _motionAngleInfos;
        private float _originAnchorPolar;
        private float _speedForOneCircle;
        private float _speedSolvingWallHit;
        protected float _targetCollisionRadius;
        protected float _targetForwardDelta;
        protected float _targetPolar;
        [CompilerGenerated]
        private static Comparison<MotionAngleInfo> <>f__am$cache18;
        [CompilerGenerated]
        private static Comparison<float> <>f__am$cache19;
        private const int BIO_TARGET_CAMERA_MODE_ALWAYS_HORIZONTAL = 1;
        private const int BIO_TARGET_CAMERA_MODE_DEFAULT = 0;
        protected const float BIO_TARGET_MAX_ANGLE = 45f;
        protected const float BIO_TARGET_MAX_ANGLE_RATIO = 1.1f;
        private const float BIO_TARGET_MIN_ANGLE = 17f;
        protected const float BIO_TARGET_MIN_ANGLE_RATIO = 0.4f;
        private const float BIO_TRIGGER_SOLVE_WALL_HIT_RATIO = 0.75f;
        private float DELTA_TIME_PER_FRAME;

        public FollowAvatarAndTargetState(MainCameraFollowState followState) : base(followState)
        {
            this._feasibleAngles = new List<float>(4);
            this._motionAngleInfos = new List<MotionAngleInfo>(4);
            this._speedForOneCircle = 3000f;
            this._accelerationSolvingWallHit = 7000f;
            this._minSpeedSolvingWallHit = 200f;
            this.DELTA_TIME_PER_FRAME = 0.016f;
            this._mode = FollowMode.TargetMode;
            base.maskedShortStates.Add(base._owner.rotateToAvatarFacingState);
        }

        private void CalcFeasibleAngles(float targetsAngle, float asinNum, float acosNum, bool allowObtuseAngle)
        {
            float item = ((180f - asinNum) - targetsAngle) - acosNum;
            float num2 = -(((180f - asinNum) - targetsAngle) + acosNum);
            this._feasibleAngles.Add(item);
            this._feasibleAngles.Add(num2);
            if (allowObtuseAngle)
            {
                float num3 = (asinNum - targetsAngle) - acosNum;
                float num4 = -((asinNum - targetsAngle) + acosNum);
                this._feasibleAngles.Add(num3);
                this._feasibleAngles.Add(num4);
            }
            if (Vector3.Cross(this._dirCamera2Entity, this._dirCamera2Target).y < 0.0)
            {
                for (int i = 0; i < this._feasibleAngles.Count; i++)
                {
                    this._feasibleAngles[i] = this.TruncateRotateAngle(this._feasibleAngles[i]);
                }
            }
            else
            {
                for (int j = 0; j < this._feasibleAngles.Count; j++)
                {
                    this._feasibleAngles[j] = this.TruncateRotateAngle(-this._feasibleAngles[j]);
                }
            }
        }

        private void CalcRotateAngles(float targetsAngle)
        {
            float f = Mathf.Acos((((this._disCamera2Entity * this._disCamera2Entity) + (this._disEntity2Target * this._disEntity2Target)) - (this._disCamera2Target * this._disCamera2Target)) / ((2f * this._disCamera2Entity) * this._disEntity2Target)) * 57.29578f;
            float num2 = Mathf.Asin((this._disCamera2Entity / this._disEntity2Target) * Mathf.Sin(targetsAngle * 0.01745329f)) * 57.29578f;
            if ((!float.IsNaN(targetsAngle) && !float.IsNaN(num2)) && !float.IsNaN(f))
            {
                this.CalcFeasibleAngles(targetsAngle, num2, f, false);
            }
        }

        protected virtual void CalcTargetPolarIgnoreWallHit()
        {
            this.CalcTargetPoloarIngoreWallHitWhenHasAttackTarget();
        }

        public void CalcTargetPolarWhenWallHit()
        {
            float maxAdjAngle = this.FindMaxAdjAngle();
            float minAdjAngle = this.FindMinAdjAngle();
            if (maxAdjAngle >= minAdjAngle)
            {
                this.SolveWallHit(maxAdjAngle, minAdjAngle);
            }
        }

        protected void CalcTargetPoloarIngoreWallHitWhenHasAttackTarget()
        {
            float currentAngle = Mathf.Acos(Vector3.Dot(this._dirCamera2Entity, this._dirCamera2Target) / (this._disCamera2Entity * this._disCamera2Target)) * 57.29578f;
            if (this._disEntity2Target < this._disCamera2Entity)
            {
                if (this.CheckNeedRotateToMaxAngle(currentAngle))
                {
                    this.DoAdjToCorrectTargetsAngle(this.FindMaxAdjAngle(), false);
                }
                else
                {
                    float targetsAngle = this.FindMinAdjAngle();
                    if (currentAngle < (targetsAngle * 0.4f))
                    {
                        this.DoAdjToCorrectTargetsAngle(targetsAngle, false);
                    }
                    else
                    {
                        this.DoAdjToCorrectTargetsAngle(currentAngle, true);
                    }
                }
            }
            else
            {
                float num3 = this.FindMinAdjAngle();
                if (currentAngle < (num3 * 0.4f))
                {
                    this.DoAdjToCorrectTargetsAngle(num3, false);
                }
                else if (currentAngle > 49.5f)
                {
                    this.DoAdjToCorrectTargetsAngle(45f, false);
                }
                else
                {
                    this.DoAdjToCorrectTargetsAngle(currentAngle, false);
                }
            }
        }

        private void CalculateTargetPolarModeHorizontal()
        {
            float num = Mathf.Acos(Vector3.Dot(this._dirCamera2Entity, this._dirCamera2Target) / (this._disCamera2Entity * this._disCamera2Target)) * 57.29578f;
            if (this._disEntity2Target < this._disCamera2Entity)
            {
                float num2 = Mathf.Acos((this._disEntity2Target / 2f) / this._disCamera2Entity) * 57.29578f;
                float targetsAngle = 180f - (2f * num2);
                this.DoAdjToCorrectTargetsAngle(targetsAngle, false);
            }
            else if (num < 17f)
            {
                this.DoAdjToCorrectTargetsAngle(17f, false);
            }
            else if (num > 45f)
            {
                this.DoAdjToCorrectTargetsAngle(45f, false);
            }
        }

        private bool CheckNeedRotateToMaxAngle(float currentAngle)
        {
            float num = ((this._disEntity2Target * this._disEntity2Target) + (this._disCamera2Target * this._disCamera2Target)) - (this._disCamera2Entity * this._disCamera2Entity);
            if (num >= 0f)
            {
                return false;
            }
            float num2 = Mathf.Atan(this._targetCollisionRadius / ((base._owner.anchorRadius * base._owner.cameraLocateRatio) - Mathf.Sqrt((this._disEntity2Target * this._disEntity2Target) - (this._targetCollisionRadius * this._targetCollisionRadius)))) * 57.29578f;
            return (currentAngle <= num2);
        }

        private bool CheckWallHit(Vector3 cameraPosition, Vector3 followCenterPosition, out RaycastHit wallHitRet)
        {
            RaycastHit hit;
            Vector3 vector = cameraPosition - followCenterPosition;
            float magnitude = vector.magnitude;
            bool flag = Physics.Raycast(followCenterPosition, cameraPosition - followCenterPosition, out hit, magnitude, ((int) 1) << InLevelData.CAMERA_COLLIDER_LAYER);
            wallHitRet = hit;
            return flag;
        }

        private bool CheckWallHitByTargetPolar(float targetPolar, out RaycastHit wallHitRet)
        {
            Vector3 vector;
            Vector3 vector2;
            RaycastHit hit;
            this.ConvertToWorldSpace(targetPolar, out vector, out vector2);
            bool flag = this.CheckWallHit(vector2, vector, out hit);
            wallHitRet = hit;
            return flag;
        }

        private void ConvertToWorldSpace(float angle, out Vector3 followCenterPosition, out Vector3 cameraPosition)
        {
            Vector3 zero = Vector3.zero;
            zero.x = (base._owner.anchorRadius * Mathf.Cos(angle * 0.01745329f)) * Mathf.Cos(base._owner.anchorElevation * 0.01745329f);
            zero.z = (base._owner.anchorRadius * Mathf.Sin(angle * 0.01745329f)) * Mathf.Cos(base._owner.anchorElevation * 0.01745329f);
            zero.y = base._owner.anchorRadius * Mathf.Sin(base._owner.anchorElevation * 0.01745329f);
            followCenterPosition = base._owner.avatar.XZPosition;
            followCenterPosition.y = base._owner.followCenterY;
            cameraPosition = (Vector3) (followCenterPosition + (zero * base._owner.cameraLocateRatio));
        }

        private void ConvertToWorldSpaceUseCurrentRadius(float angle, out Vector3 followCenterPosition, out Vector3 cameraPosition)
        {
            Vector3 zero = Vector3.zero;
            zero.x = (this._disCamera2Entity * Mathf.Cos(angle * 0.01745329f)) * Mathf.Cos(base._owner.anchorElevation * 0.01745329f);
            zero.z = (this._disCamera2Entity * Mathf.Sin(angle * 0.01745329f)) * Mathf.Cos(base._owner.anchorElevation * 0.01745329f);
            zero.y = this._disCamera2Entity * Mathf.Sin(base._owner.anchorElevation * 0.01745329f);
            followCenterPosition = base._owner.avatar.XZPosition;
            followCenterPosition.y = base._owner.followCenterY;
            cameraPosition = followCenterPosition + zero;
        }

        protected void DoAdjToCorrectTargetsAngle(float targetsAngle, bool allowObtuseAngle)
        {
            float f = Mathf.Acos((((this._disCamera2Entity * this._disCamera2Entity) + (this._disEntity2Target * this._disEntity2Target)) - (this._disCamera2Target * this._disCamera2Target)) / ((2f * this._disCamera2Entity) * this._disEntity2Target)) * 57.29578f;
            float num2 = Mathf.Asin((this._disCamera2Entity / this._disEntity2Target) * Mathf.Sin(targetsAngle * 0.01745329f)) * 57.29578f;
            if ((!float.IsNaN(targetsAngle) && !float.IsNaN(num2)) && !float.IsNaN(f))
            {
                this.CalcFeasibleAngles(targetsAngle, num2, f, allowObtuseAngle);
            }
            if (this._feasibleAngles.Count > 0)
            {
                float num3 = 0f;
                if (<>f__am$cache19 == null)
                {
                    <>f__am$cache19 = (a, b) => Mathf.Abs(a).CompareTo(Mathf.Abs(b));
                }
                this._feasibleAngles.Sort(<>f__am$cache19);
                num3 = this._feasibleAngles[0];
                this._targetPolar = base._owner.anchorPolar + num3;
                if (Vector3.Cross(this._dirCamera2Entity, this._dirCamera2Target).y < 0.0)
                {
                    this._targetForwardDelta = -targetsAngle / 2f;
                }
                else
                {
                    this._targetForwardDelta = targetsAngle / 2f;
                }
                this._hasSetTargetForwardDelta = true;
            }
        }

        protected void DoUpdateWhenHasAttackTarget()
        {
            if (!this._isCameraSolveWallHit)
            {
                RaycastHit hit;
                Vector3 xZPosition = base._owner.avatar.XZPosition;
                xZPosition.y = base._owner.followCenterY;
                this._isCurrentWallHit = this.CheckWallHit(base._owner.followData.cameraPosition, xZPosition, out hit);
                if (this._isCurrentWallHit)
                {
                    Vector3 vector2 = base._owner.followData.cameraPosition - xZPosition;
                    float magnitude = vector2.magnitude;
                    float num2 = magnitude - hit.distance;
                    if ((num2 / magnitude) > 0.75f)
                    {
                        this.TryToSolveWallHit();
                    }
                    else
                    {
                        this.ReplaceCamera();
                    }
                }
                else
                {
                    this.ReplaceCamera();
                }
                if (!this._isCameraSolveWallHit)
                {
                    if (Miscs.AbsAngleDiff(base._owner.anchorPolar, this._targetPolar) <= 150f)
                    {
                        base._owner.anchorPolar = this._targetPolar;
                    }
                }
                else
                {
                    this.DoWhenSolveWallHit();
                }
            }
            else
            {
                this.DoWhenSolveWallHit();
            }
        }

        protected void DoWhenSolveWallHit()
        {
            base._owner.needLerpPositionThisFrame = false;
            base._owner.needLerpForwardThisFrame = false;
            bool reachTargetPolar = false;
            float a = this._speedSolvingWallHit - (this._accelerationSolvingWallHit * (this._disableLerpSumUpTime + Time.deltaTime));
            float delta = Mathf.Max(a, this._minSpeedSolvingWallHit) * this.DELTA_TIME_PER_FRAME;
            float num3 = this.LerpAnchorPolarForMinMotion(this._originAnchorPolar, this._currentAnchorPolar, this._targetPolar, delta, out reachTargetPolar);
            base._owner.anchorPolar = this._currentAnchorPolar = num3;
            if (reachTargetPolar)
            {
                this.EndSolvingWallHit();
            }
            else
            {
                this._disableLerpSumUpTime += Time.deltaTime;
            }
        }

        private void EndSolvingWallHit()
        {
            if (this._isCameraSolveWallHit)
            {
                this._isCameraSolveWallHit = false;
                base._owner.needLerpForwardThisFrame = true;
                base._owner.needLerpPositionThisFrame = true;
                base._owner.focusOnAvatar = false;
                this._disableLerpSumUpTime = 0f;
            }
        }

        public override void Enter()
        {
            if (base._owner.rotateToAvatarFacingState.active)
            {
                base._owner.RemoveShortState();
            }
            if (base._owner.recoverState.active)
            {
                base._owner.recoverState.CancelLerpRatioRecovering();
            }
        }

        public override void Exit()
        {
            this.EndSolvingWallHit();
            float to = Mathf.Atan2(-base._owner.followData.cameraForward.z, -base._owner.followData.cameraForward.x) * 57.29578f;
            to = Miscs.NormalizedRotateAngle(base._owner.anchorPolar, to);
            base._owner.followAvatarState.SetEnteringLerpTarget(to);
            if (this._mode == FollowMode.DirectionMode)
            {
                this._mode = FollowMode.TargetMode;
                if (base._owner.timedPullZState.active)
                {
                    base._owner.timedPullZState.ForceToExitState();
                }
            }
        }

        private float FindMaxAdjAngle()
        {
            if (this._disEntity2Target >= this._disCamera2Entity)
            {
                return 45f;
            }
            float num = Mathf.Asin(this._disEntity2Target / (base._owner.anchorRadius * base._owner.cameraLocateRatio)) * 57.29578f;
            if (num < 0f)
            {
                Debug.LogError(string.Format("FindMaxAdjAngle not valid, accecptableMaxTargetsAngle{0:f}", num));
                Debug.Break();
            }
            return Mathf.Min(num, 45f);
        }

        protected float FindMinAdjAngle()
        {
            return (Mathf.Asin((((this._entityCollisionRadius + 0.3f) * base._owner.avatar.commonConfig.CommonArguments.CameraMinAngleRatio) / base._owner.anchorRadius) * base._owner.cameraLocateRatio) * 57.29578f);
        }

        protected float GetColliderRadius(BaseMonoEntity entity)
        {
            CapsuleCollider componentInChildren = entity.GetComponentInChildren<CapsuleCollider>();
            if (componentInChildren == null)
            {
                return 0.5f;
            }
            return componentInChildren.radius;
        }

        private MotionAngleInfo GetMotionAngleInfo(float targetsAngle, bool forceTarget)
        {
            this.CalcRotateAngles(targetsAngle);
            this._motionAngleInfos.Clear();
            for (int i = 0; i < this._feasibleAngles.Count; i++)
            {
                this._motionAngleInfos.Add(new MotionAngleInfo(targetsAngle, this._feasibleAngles[i]));
            }
            if (this._motionAngleInfos.Count <= 0)
            {
                return null;
            }
            if (<>f__am$cache18 == null)
            {
                <>f__am$cache18 = (a, b) => Mathf.Abs(a.rotateAngle).CompareTo(Mathf.Abs(b.rotateAngle));
            }
            this._motionAngleInfos.Sort(<>f__am$cache18);
            bool flag = false;
            MotionAngleInfo info = null;
            for (int j = 0; j < this._motionAngleInfos.Count; j++)
            {
                RaycastHit hit;
                float targetPolar = base._owner.anchorPolar + this._motionAngleInfos[j].rotateAngle;
                if (!this.CheckWallHitByTargetPolar(targetPolar, out hit))
                {
                    info = this._motionAngleInfos[j];
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                return info;
            }
            return (!forceTarget ? null : this._motionAngleInfos[0]);
        }

        private float GetRealPolar()
        {
            Vector3 vector2 = base._owner.cameraPosition - base._owner.avatar.XZPosition;
            Vector3 normalized = vector2.normalized;
            return (Mathf.Atan2(normalized.z, normalized.x) * 57.29578f);
        }

        private bool IsTargetBlockSightEntity()
        {
            float num = (((this._disCamera2Entity * this._disCamera2Entity) + (this._disEntity2Target * this._disEntity2Target)) - (this._disCamera2Target * this._disCamera2Target)) / ((2f * this._disCamera2Entity) * this._disEntity2Target);
            if (num <= 0f)
            {
                return false;
            }
            float num2 = Mathf.Sqrt((this._disEntity2Target * this._disEntity2Target) - (this._targetCollisionRadius * this._targetCollisionRadius)) / this._disEntity2Target;
            return (num > num2);
        }

        private float LerpAnchorPolarForMinMotion(float originAnchorPolar, float currentAnchorPolar, float targetAnchorPolar, float delta, out bool reachTargetPolar)
        {
            reachTargetPolar = false;
            float num = 0f;
            if (targetAnchorPolar >= originAnchorPolar)
            {
                num = currentAnchorPolar + delta;
                if (num >= targetAnchorPolar)
                {
                    reachTargetPolar = true;
                    num = targetAnchorPolar;
                }
                return num;
            }
            num = currentAnchorPolar - delta;
            if (num <= targetAnchorPolar)
            {
                reachTargetPolar = true;
                num = targetAnchorPolar;
            }
            return num;
        }

        protected virtual void PreCalculate()
        {
            Vector3 vector = base._owner.avatar.AttackTarget.XZPosition - base._owner.avatar.XZPosition;
            vector.y = 0f;
            this._disEntity2Target = vector.magnitude;
            this._dirCamera2Entity = base._owner.avatar.XZPosition - base._owner.followData.cameraPosition;
            this._dirCamera2Entity.y = 0f;
            this._disCamera2Entity = this._dirCamera2Entity.magnitude;
            this._dirCamera2Target = base._owner.avatar.AttackTarget.XZPosition - base._owner.followData.cameraPosition;
            this._dirCamera2Target.y = 0f;
            this._disCamera2Target = this._dirCamera2Target.magnitude;
            this._entityCollisionRadius = this.GetColliderRadius(base._owner.avatar);
            this._targetCollisionRadius = this.GetColliderRadius(base._owner.avatar.AttackTarget);
        }

        public void ReplaceCamera()
        {
            this.PreCalculate();
            if (this._disEntity2Target <= ((this._entityCollisionRadius + this._targetCollisionRadius) - 0.1f))
            {
                this._targetPolar = base._owner.anchorPolar;
            }
            else if (this._bioTargetCameraMode == 1)
            {
                this.CalculateTargetPolarModeHorizontal();
            }
            else
            {
                this.CalcTargetPolarIgnoreWallHit();
            }
        }

        public override void ResetState()
        {
        }

        private void SolveWallHit(float maxAdjAngle, float minAdjAngle)
        {
            MotionAngleInfo motionAngleInfo;
            if (this.IsTargetBlockSightEntity())
            {
                motionAngleInfo = this.GetMotionAngleInfo(maxAdjAngle, false);
                if (motionAngleInfo == null)
                {
                    motionAngleInfo = this.GetMotionAngleInfo(minAdjAngle, true);
                }
            }
            else
            {
                motionAngleInfo = this.GetMotionAngleInfo(minAdjAngle, false);
                if (motionAngleInfo == null)
                {
                    motionAngleInfo = this.GetMotionAngleInfo(maxAdjAngle, true);
                }
            }
            if (motionAngleInfo != null)
            {
                this._targetPolar = base._owner.anchorPolar + motionAngleInfo.rotateAngle;
                this._targetForwardDelta = 0f;
                this._hasSetTargetForwardDelta = true;
                this.StartSolvingWallHit();
            }
        }

        private void StartSolvingWallHit()
        {
            this._isCameraSolveWallHit = true;
            if (Application.targetFrameRate == -1)
            {
                this.DELTA_TIME_PER_FRAME = 0.01666667f;
            }
            else
            {
                this.DELTA_TIME_PER_FRAME = 1f / ((float) Application.targetFrameRate);
            }
            base._owner.needLerpForwardThisFrame = false;
            base._owner.needLerpPositionThisFrame = false;
            this._originAnchorPolar = this.GetRealPolar();
            this._currentAnchorPolar = this._originAnchorPolar;
            this._disableLerpSumUpTime = 0f;
            base._owner.focusOnAvatar = true;
            this._targetPolar = Miscs.NormalizedRotateAngle(this._originAnchorPolar, this._targetPolar);
            this._speedSolvingWallHit = (Miscs.AbsAngleDiff(this._originAnchorPolar, this._targetPolar) * this._speedForOneCircle) / 360f;
        }

        public void SwitchMode(FollowMode mode)
        {
            this._mode = mode;
        }

        private float TruncateRotateAngle(float rotateAngle)
        {
            if (Mathf.Abs(rotateAngle) <= 180f)
            {
                return rotateAngle;
            }
            if (rotateAngle > 0f)
            {
                return (rotateAngle - 360f);
            }
            return (rotateAngle + 360f);
        }

        public void TryToSolveWallHit()
        {
            this.PreCalculate();
            this.CalcTargetPolarWhenWallHit();
        }

        public override void Update()
        {
            this._feasibleAngles.Clear();
            if (this._mode != FollowMode.DirectionMode)
            {
                if (base._owner.avatar.AttackTarget == null)
                {
                    base._owner.TransitBaseState(base._owner.followAvatarState, false);
                }
                else
                {
                    this._hasSetTargetForwardDelta = false;
                    this.DoUpdateWhenHasAttackTarget();
                    if (base._owner.recoverState.active)
                    {
                        base._owner.recoverState.CancelForwardDeltaAngleRecover();
                    }
                    if (this._hasSetTargetForwardDelta)
                    {
                        if ((this._targetForwardDelta * base._owner.forwardDeltaAngle) > 0f)
                        {
                            base._owner.forwardDeltaAngle = Mathf.Lerp(base._owner.forwardDeltaAngle, this._targetForwardDelta, Time.deltaTime * 5f);
                        }
                        else
                        {
                            base._owner.forwardDeltaAngle = this._targetForwardDelta;
                        }
                    }
                }
            }
        }

        public enum FollowMode
        {
            TargetMode,
            DirectionMode
        }

        private class MotionAngleInfo
        {
            public float rotateAngle;
            public float targetsAngle;

            public MotionAngleInfo()
            {
            }

            public MotionAngleInfo(float targetsAngle, float rotateAngle)
            {
                this.targetsAngle = targetsAngle;
                this.rotateAngle = rotateAngle;
            }
        }
    }
}

