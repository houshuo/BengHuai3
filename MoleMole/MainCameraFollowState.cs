namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MainCameraFollowState : BaseMainCameraState
    {
        private BaseFollowBaseState _baseState;
        private float _enterPolarArgument;
        private EnterPolarMode _enterPolarMode;
        private Vector3[] _lastFollowCenters;
        private BaseFollowBaseState _nextBaseState;
        private BaseFollowShortState _shortState;
        private float _stableModeCDTimer;
        private RaycastHit _wallHit;
        public float anchorElevation;
        public float anchorPolar;
        public float anchorRadius;
        public BaseMonoAvatar avatar;
        public float cameraLocateRatio;
        public bool focusOnAvatar;
        public FollowAvatarAndBossState followAvatarAndBossState;
        public FollowAvatarAndCrowdState followAvatarAndCrowdState;
        public FollowAvatarAndTargetState followAvatarAndTargetState;
        public FollowAvatarControlledRotate followAvatarControlledRotate;
        public FollowAvatarPoleState followAvatarState;
        public Vector3 followCenterXZPosition;
        public float followCenterY;
        public FollowData followData;
        public float forwardDeltaAngle;
        public float forwardLerpRatio;
        public bool isCameraLocateRatioUserDefined;
        public BaseFollowBaseState lastBaseState;
        public bool lerpForwardOvershootLastFrame;
        public bool lerpPositionOvershootLastFrame;
        public FollowLookAtPosition lookAtPositionState;
        private const int LPF_POSITIONS_COUNT = 3;
        public bool needLerpForwardThisFrame;
        public bool needLerpPositionThisFrame;
        public bool needLPF;
        public bool needSmoothFollowCenterThisFrame;
        public float posLerpRatio;
        public FollowRangeTransit rangeTransitState;
        public FollowRecovering recoverState;
        public FollowRotateToAvatarFacing rotateToAvatarFacingState;
        public FollowSlowMotionKill slowMotionKillState;
        public FollowStandBy standByState;
        public FollowSuddenChange suddenChangeState;
        public FollowSuddenRecover suddenRecoverState;
        public FollowTimedPullZ timedPullZState;

        public MainCameraFollowState(MonoMainCamera camera) : base(camera)
        {
            this.anchorRadius = 6f;
            this.cameraLocateRatio = 0.535f;
            this.posLerpRatio = 1f;
            this.forwardLerpRatio = 1f;
            base.lerpDirectionalLight = true;
            this.followData = new FollowData();
            this.rotateToAvatarFacingState = new FollowRotateToAvatarFacing(this);
            this.standByState = new FollowStandBy(this);
            this.lookAtPositionState = new FollowLookAtPosition(this);
            this.rangeTransitState = new FollowRangeTransit(this);
            this.timedPullZState = new FollowTimedPullZ(this);
            this.suddenChangeState = new FollowSuddenChange(this);
            this.suddenRecoverState = new FollowSuddenRecover(this);
            this.slowMotionKillState = new FollowSlowMotionKill(this);
            this.followAvatarState = new FollowAvatarPoleState(this);
            this.followAvatarAndTargetState = new FollowAvatarAndTargetState(this);
            this.followAvatarAndBossState = new FollowAvatarAndBossState(this);
            this.followAvatarAndCrowdState = new FollowAvatarAndCrowdState(this);
            this.followAvatarControlledRotate = new FollowAvatarControlledRotate(this);
            this.recoverState = new FollowRecovering(this);
            this.anchorRadius = 6f;
            this.followCenterY = 1.2f;
            this.anchorElevation = MainCameraData.CAMERA_DEFAULT_ELEVATION_DEGREE;
            this.forwardDeltaAngle = 0f;
            base.cameraFOV = this.mainCamera.originalFOV;
            this.recoverState.SetupRecoverRadius(this.anchorRadius);
            this.recoverState.SetupRecoverCenterY(this.followCenterY);
            this.recoverState.SetupRecoverElevation(this.anchorElevation);
            this.recoverState.SetupRecoverForwardDelta(this.forwardDeltaAngle);
            this.recoverState.SetupRecoverLerpPosRatio(1f);
            this.recoverState.SetupRecoverLerpForwardRatio(1f);
            this.recoverState.SetupRecoverFOV(base.cameraFOV);
        }

        public void AddOrReplaceShortState(BaseFollowShortState shortState)
        {
            if (this._shortState != null)
            {
                this.RemoveShortState();
                this.AddShortState(shortState);
            }
            else
            {
                this.AddShortState(shortState);
            }
        }

        public void AddShortState(BaseFollowShortState shortState)
        {
            if ((this._baseState != null) && !this._baseState.maskedShortStates.Contains(shortState))
            {
                this._shortState = shortState;
                this._shortState.SetActive(true);
                this._shortState.Enter();
                this.SubStateStatusReset();
            }
        }

        private bool CanBaseStateTransit(BaseFollowBaseState toSubState, bool forceTransit = false)
        {
            if (!forceTransit)
            {
                if ((this._baseState == this.followAvatarState) && (toSubState == this.followAvatarState))
                {
                    return false;
                }
                if (toSubState == this.followAvatarControlledRotate)
                {
                    return true;
                }
                if (this._baseState == this.followAvatarAndBossState)
                {
                    return false;
                }
                if (this._baseState == this.followAvatarAndCrowdState)
                {
                    return false;
                }
            }
            return true;
        }

        private void ConvertToFollowData()
        {
            Vector3 vector = new Vector3 {
                x = (this.anchorRadius * Mathf.Cos(this.anchorPolar * 0.01745329f)) * Mathf.Cos(this.anchorElevation * 0.01745329f),
                z = (this.anchorRadius * Mathf.Sin(this.anchorPolar * 0.01745329f)) * Mathf.Cos(this.anchorElevation * 0.01745329f),
                y = this.anchorRadius * Mathf.Sin(this.anchorElevation * 0.01745329f)
            };
            this.followData.followCenterPosition = this.followCenterXZPosition;
            this.followData.followCenterPosition.y = this.followCenterY;
            this.followData.anchorPosition = this.followData.followCenterPosition + vector;
            this.followData.cameraPosition = this.followData.followCenterPosition + ((Vector3) (vector * this.cameraLocateRatio));
            this.followData.cameraForward = this.followData.followCenterPosition - this.followData.cameraPosition;
            this.followData.cameraForward.Normalize();
            this.followData.cameraForwardNoForwardDelta = this.followData.cameraForward;
            this.followData.cameraForward = (Vector3) (Quaternion.AngleAxis(this.forwardDeltaAngle, Vector3.up) * this.followData.cameraForward);
            this.followData.cameraForward.Normalize();
        }

        [Conditional("UNITY_EDITOR"), Conditional("NG_HSOD_DEBUG")]
        public void DebugDrawSphereCoords(Color color, float duration)
        {
            Vector3 vector = new Vector3 {
                x = (this.anchorRadius * Mathf.Cos(this.anchorPolar * 0.01745329f)) * Mathf.Cos(this.anchorElevation * 0.01745329f),
                z = (this.anchorRadius * Mathf.Sin(this.anchorPolar * 0.01745329f)) * Mathf.Cos(this.anchorElevation * 0.01745329f),
                y = this.anchorRadius * Mathf.Sin(this.anchorElevation * 0.01745329f)
            };
            Vector3 followCenterXZPosition = this.followCenterXZPosition;
            followCenterXZPosition.y = this.followCenterY;
            Vector3 vector3 = followCenterXZPosition + ((Vector3) (vector * this.cameraLocateRatio));
            (followCenterXZPosition - vector3).Normalize();
            UnityEngine.Debug.DrawLine(followCenterXZPosition, followCenterXZPosition + vector, color, duration);
        }

        public override void Enter()
        {
            this.followCenterXZPosition = this.avatar.XZPosition;
            this.lastBaseState = this._baseState;
            this._baseState = this.GetTargetBaseState(false, this.avatar.AttackTarget);
            this._baseState.SetActive(true);
            this._baseState.Enter();
            this.lastBaseState = null;
            this._lastFollowCenters = new Vector3[3];
            if (this.needLPF)
            {
                for (int i = 0; i < this._lastFollowCenters.Length; i++)
                {
                    this._lastFollowCenters[i] = this.followCenterXZPosition;
                }
            }
            if (this._enterPolarMode == EnterPolarMode.AlongAvatarFacing)
            {
                Vector3 faceDirection = this.avatar.FaceDirection;
                this.anchorPolar = Mathf.Atan2(-faceDirection.z, -faceDirection.x) * 57.29578f;
            }
            else if (this._enterPolarMode == EnterPolarMode.NearestPointOnSphere)
            {
                Vector3 vector3 = base._owner.transform.position - this.followCenterXZPosition;
                Vector3 normalized = vector3.normalized;
                this.anchorPolar = Mathf.Atan2(normalized.z, normalized.x) * 57.29578f;
            }
            else if (this._enterPolarMode == EnterPolarMode.AlongTargetPolar)
            {
                this.anchorPolar = this._enterPolarArgument;
            }
            this.ConvertToFollowData();
            base.cameraPosition = this.followData.cameraPosition;
            base.cameraForward = this.followData.cameraForward;
            base.cameraFOV = base._owner.originalFOV;
            this._stableModeCDTimer = 0f;
        }

        public override void Exit()
        {
            this._baseState.Exit();
            this._baseState.SetActive(false);
            this.TryRemoveShortState();
            this.followAvatarState.ResetState();
            this.followAvatarAndTargetState.ResetState();
            this.followAvatarAndBossState.ResetState();
            this.followAvatarAndCrowdState.ResetState();
            this.followAvatarControlledRotate.ResetState();
            this.recoverState.RecoverImmediately();
        }

        private BaseFollowBaseState GetTargetBaseState(bool checkHasAnyControl, BaseMonoEntity attackTarget = null)
        {
            bool flag = false;
            if (this.lastBaseState == this.followAvatarAndBossState)
            {
                BaseMonoEntity bossTarget = ((FollowAvatarAndBossState) this.lastBaseState).bossTarget;
                if ((bossTarget != null) && bossTarget.IsActive())
                {
                    this.followAvatarAndBossState.bossTarget = bossTarget;
                    return this.followAvatarAndBossState;
                }
                flag = true;
            }
            else
            {
                if (this.lastBaseState == this.followAvatarAndCrowdState)
                {
                    return this.followAvatarAndCrowdState;
                }
                flag = true;
            }
            if (flag)
            {
                if ((attackTarget != null) && attackTarget.IsActive())
                {
                    return this.followAvatarAndTargetState;
                }
                if (this.avatar.IsLockDirection)
                {
                    this.followAvatarAndTargetState.SwitchMode(FollowAvatarAndTargetState.FollowMode.DirectionMode);
                }
                if (checkHasAnyControl)
                {
                    if (this.avatar.GetActiveControlData().hasAnyControl)
                    {
                        return this.followAvatarState;
                    }
                }
                else
                {
                    return this.followAvatarState;
                }
            }
            return null;
        }

        private void OnAvatarAttackTargetChanged(BaseMonoEntity attackTarget)
        {
            if (this._baseState == this.followAvatarControlledRotate)
            {
                this.TryToTransitToOtherBaseState(false, attackTarget);
            }
            else if (((attackTarget != null) && attackTarget.IsActive()) && (this._baseState == this.followAvatarState))
            {
                this.TransitBaseState(this.followAvatarAndTargetState, false);
            }
            else if (attackTarget == null)
            {
                if (this.avatar.IsLockDirection)
                {
                    this.followAvatarAndTargetState.SwitchMode(FollowAvatarAndTargetState.FollowMode.DirectionMode);
                }
                else
                {
                    this.TransitBaseState(this.followAvatarState, false);
                }
            }
        }

        private void OnAvatarLockDirectionChanged(bool direction)
        {
            if (!direction && (this.avatar.AttackTarget == null))
            {
                this.TransitBaseState(this.followAvatarState, false);
            }
        }

        public void RemoveShortState()
        {
            this._shortState.SetActive(false);
            this._shortState.Exit();
            this._shortState = null;
        }

        public void SetEnterPolarMode(EnterPolarMode mode, float polar = 0)
        {
            this._enterPolarMode = mode;
            this._enterPolarArgument = polar;
        }

        public void SetupFollowAvatar(uint avatarID)
        {
            if (this.avatar != null)
            {
                this.avatar.onAttackTargetChanged = (Action<BaseMonoEntity>) Delegate.Remove(this.avatar.onAttackTargetChanged, new Action<BaseMonoEntity>(this.OnAvatarAttackTargetChanged));
                this.avatar.onLockDirectionChanged = (Action<bool>) Delegate.Remove(this.avatar.onLockDirectionChanged, new Action<bool>(this.OnAvatarLockDirectionChanged));
            }
            this.avatar = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(avatarID);
            this.avatar.onAttackTargetChanged = (Action<BaseMonoEntity>) Delegate.Combine(this.avatar.onAttackTargetChanged, new Action<BaseMonoEntity>(this.OnAvatarAttackTargetChanged));
            this.avatar.onLockDirectionChanged = (Action<bool>) Delegate.Combine(this.avatar.onLockDirectionChanged, new Action<bool>(this.OnAvatarLockDirectionChanged));
        }

        private void SubStateStatusReset()
        {
            this._stableModeCDTimer = 0f;
        }

        private void SubStateTransitionUpdate()
        {
            if (this._shortState != this.standByState)
            {
                if ((this.recoverState.active || this.followAvatarAndTargetState.active) || (this.followAvatarAndBossState.active || this.followAvatarAndCrowdState.active))
                {
                    this._stableModeCDTimer = 0f;
                }
                else if (this.avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Stable))
                {
                    this._stableModeCDTimer += Time.deltaTime * base._owner.TimeScale;
                    if (this._stableModeCDTimer >= 3f)
                    {
                        this.AddOrReplaceShortState(this.standByState);
                        this._stableModeCDTimer = 0f;
                    }
                }
                else
                {
                    this._stableModeCDTimer = 0f;
                }
            }
        }

        public void TransitBaseState(BaseFollowBaseState toSubState, bool forceTransit = false)
        {
            if (this.CanBaseStateTransit(toSubState, forceTransit))
            {
                if (this._nextBaseState != null)
                {
                }
                this._nextBaseState = toSubState;
            }
        }

        public void TryRemoveShortState()
        {
            if (this._shortState != null)
            {
                this.RemoveShortState();
            }
        }

        public void TryToTransitToOtherBaseState(bool checkHasAnyControl, BaseMonoEntity attackTarget = null)
        {
            BaseFollowBaseState targetBaseState = this.GetTargetBaseState(checkHasAnyControl, attackTarget);
            if (targetBaseState != null)
            {
                this.TransitBaseState(targetBaseState, false);
            }
        }

        public override void Update()
        {
            if (this.needLPF)
            {
                for (int i = this._lastFollowCenters.Length - 1; i > 0; i--)
                {
                    this._lastFollowCenters[i] = this._lastFollowCenters[i - 1];
                }
                this._lastFollowCenters[0] = this.followCenterXZPosition;
            }
            Vector3 followCenterXZPosition = this.followCenterXZPosition;
            this.followCenterXZPosition = this.avatar.XZPosition;
            Vector3 vector2 = followCenterXZPosition - this.followCenterXZPosition;
            this.needLerpPositionThisFrame = true;
            this.needLerpForwardThisFrame = true;
            this.needSmoothFollowCenterThisFrame = false;
            this.SubStateTransitionUpdate();
            if (this.recoverState.active)
            {
                this.recoverState.Update();
            }
            if (this._nextBaseState != null)
            {
                this._baseState.Exit();
                this._baseState.SetActive(false);
                this.lastBaseState = this._baseState;
                this._baseState = this._nextBaseState;
                this._baseState.Enter();
                this._baseState.SetActive(true);
                this._nextBaseState = null;
                if ((this._shortState != null) && this._baseState.maskedShortStates.Contains(this._shortState))
                {
                    this.RemoveShortState();
                }
            }
            if (this._shortState != null)
            {
                this._shortState.Update();
                bool isSkippingBaseState = false;
                if (this._shortState != null)
                {
                    isSkippingBaseState = this._shortState.isSkippingBaseState;
                }
                if (!isSkippingBaseState || this._baseState.cannotBeSkipped)
                {
                    this._baseState.Update();
                }
                if (this._shortState != null)
                {
                    this._shortState.PostUpdate();
                }
            }
            else
            {
                this._baseState.Update();
            }
            if (this.needLPF && this.needSmoothFollowCenterThisFrame)
            {
                Vector3 vector3 = this.followCenterXZPosition;
                for (int j = 0; j < this._lastFollowCenters.Length; j++)
                {
                    vector3 += this._lastFollowCenters[j];
                }
                this.followCenterXZPosition = (Vector3) (vector3 / ((float) (this._lastFollowCenters.Length + 1)));
                UnityEngine.Debug.DrawLine(this.followCenterXZPosition, this.followCenterXZPosition + Vector3.up);
            }
            this.ConvertToFollowData();
            Vector3 cameraPosition = this.followData.cameraPosition;
            Vector3 cameraForward = this.followData.cameraForward;
            Vector3 vector6 = cameraPosition - this.followData.followCenterPosition;
            float magnitude = vector6.magnitude;
            if (Physics.Raycast(this.followData.followCenterPosition, -this.followData.cameraForwardNoForwardDelta, out this._wallHit, magnitude, ((int) 1) << InLevelData.CAMERA_COLLIDER_LAYER))
            {
                float num4 = magnitude - this._wallHit.distance;
                cameraPosition = Vector3.Lerp(this._wallHit.point, cameraPosition, 0.1f);
                cameraPosition.y += num4 * 0.1f;
                base._owner.cameraComponent.nearClipPlane = Mathf.Lerp(base._owner.originalNearClip, 0.01f, num4 / magnitude);
                Vector3 axis = Vector3.Cross(Vector3.up, base.cameraForward);
                cameraForward = (Vector3) (Quaternion.AngleAxis((0.05f * num4) * 57.29578f, axis) * cameraForward);
                cameraForward.Normalize();
                base.cameraShakeRatio = 1f - (num4 / magnitude);
            }
            else
            {
                base._owner.cameraComponent.nearClipPlane = base._owner.originalNearClip;
            }
            float num5 = Time.deltaTime * base._owner.TimeScale;
            float num6 = (num5 != 0f) ? (vector2.magnitude / Time.deltaTime) : 0f;
            float num7 = Miscs.NormalizedClamp(num6, 5f, 12f);
            if (this.needLerpPositionThisFrame)
            {
                float num8 = ((Time.deltaTime * 7.9f) * (1f + num7)) * this.posLerpRatio;
                Vector3 a = base.cameraPosition - this.followData.followCenterPosition;
                Vector3 b = cameraPosition - this.followData.followCenterPosition;
                Vector3 vector10 = Vector3.Slerp(a, b, Mathf.Clamp01(num8));
                base.cameraPosition = vector10 + this.followData.followCenterPosition;
                this.lerpPositionOvershootLastFrame = num8 >= 1f;
            }
            else
            {
                base.cameraPosition = cameraPosition;
            }
            if (this.needLerpForwardThisFrame)
            {
                float num9 = ((Time.deltaTime * 5f) * (1f + num7)) * this.forwardLerpRatio;
                this.cameraForward.Normalize();
                cameraForward.Normalize();
                base.cameraForward = MonoMainCamera.CameraForwardLerp(base.cameraForward, cameraForward, Mathf.Clamp01(num9));
                this.lerpForwardOvershootLastFrame = num9 >= 1f;
            }
            else if (this.focusOnAvatar)
            {
                Vector3 vector11 = this.followData.followCenterPosition - base.cameraPosition;
                base.cameraForward = vector11;
                this.cameraForward.Normalize();
            }
            else
            {
                base.cameraForward = cameraForward;
            }
            if (base.cameraFOV > 0f)
            {
                base._owner.cameraComponent.fieldOfView = base.cameraFOV;
            }
        }

        public MonoMainCamera mainCamera
        {
            get
            {
                return base._owner;
            }
        }

        public enum EnterPolarMode
        {
            NearestPointOnSphere,
            AlongAvatarFacing,
            AlongTargetPolar
        }

        public class FollowData
        {
            public Vector3 anchorPosition;
            public Vector3 cameraForward;
            public Vector3 cameraForwardNoForwardDelta;
            public Vector3 cameraPosition;
            public Vector3 followCenterPosition;
        }

        public enum RangeState
        {
            Near,
            Far,
            Furter,
            High,
            Higher
        }
    }
}

