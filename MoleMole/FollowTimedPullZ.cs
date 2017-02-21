namespace MoleMole
{
    using FullInspector;
    using System;
    using UnityEngine;

    public class FollowTimedPullZ : BaseFollowShortState
    {
        [ShowInInspector]
        private float _exitDuration;
        [ShowInInspector]
        private AnimationCurve _lerpCurve;
        [ShowInInspector]
        private string _lerpCurveName;
        [ShowInInspector]
        private float _lerpTimer;
        [ShowInInspector]
        private float _lerpTotalTime;
        [ShowInInspector]
        private float _origCenterY;
        [ShowInInspector]
        private float _origElevation;
        [ShowInInspector]
        private float _origFOV;
        [ShowInInspector]
        private float _origRadius;
        [ShowInInspector]
        private float _pullZTimer;
        [ShowInInspector]
        private State _state;
        [ShowInInspector]
        private float _targetCenterY;
        [ShowInInspector]
        private float _targetElevation;
        [ShowInInspector]
        private float _targetFOV;
        [ShowInInspector]
        private float _targetRadius;
        [ShowInInspector]
        private float _timer;

        public FollowTimedPullZ(MainCameraFollowState followState) : base(followState)
        {
            base.isSkippingBaseState = false;
        }

        public override void Enter()
        {
            if (base._owner.recoverState.active)
            {
                base._owner.recoverState.CancelAndStopAtCurrentState();
            }
            this._origRadius = base._owner.recoverState.GetOriginalRadius();
            this._origElevation = base._owner.recoverState.GetOriginalElevation();
            this._origCenterY = base._owner.recoverState.GetOriginalCenterY();
            this._origFOV = base._owner.mainCamera.cameraComponent.fieldOfView;
            this._timer = 0f;
            this._state = State.Entering;
        }

        public override void Exit()
        {
            base._owner.recoverState.TryRecover();
        }

        public void ForceToExitState()
        {
            this._pullZTimer = 0f;
            float a = 0.1f * Mathf.Abs((float) (this._targetFOV - this._origFOV));
            float b = 0.35f * Mathf.Abs((float) (this._targetRadius - this._origRadius));
            this._exitDuration = Mathf.Max(a, b);
            this._state = State.Exiting;
        }

        public void SetTimedPullZ(float radiusRatio, float elevationAngles, float centerYOffset, float fovOffset, float time, float lerpTime, string lerpCurveName)
        {
            this._targetRadius = radiusRatio * base._owner.recoverState.GetOriginalRadius();
            this._targetElevation = base._owner.recoverState.GetOriginalElevation() + elevationAngles;
            this._targetCenterY = base._owner.recoverState.GetOriginalCenterY() + centerYOffset;
            this._targetFOV = Mathf.Max((float) 0f, (float) (base._owner.recoverState.GetOriginalFOV() + fovOffset));
            this._pullZTimer = time;
            this._lerpTimer = lerpTime;
            this._lerpTotalTime = lerpTime;
            this._lerpCurveName = lerpCurveName;
            this._lerpCurve = CameraData.GetCameraCurveByName(this._lerpCurveName);
        }

        public override void Update()
        {
            if (this._state == State.Entering)
            {
                this._lerpTimer -= Time.deltaTime * base._owner.mainCamera.TimeScale;
                if (this._lerpTimer > 0f)
                {
                    float t = (this._lerpCurve != null) ? this._lerpCurve.Evaluate(1f - (this._lerpTimer / this._lerpTotalTime)) : (1f - (this._lerpTimer / this._lerpTotalTime));
                    base._owner.anchorRadius = Mathf.Clamp(Mathf.Lerp(this._origRadius, this._targetRadius, t), 4f, 11f);
                    base._owner.anchorElevation = Mathf.Lerp(this._origElevation, this._targetElevation, t);
                    base._owner.followCenterY = Mathf.Lerp(this._origCenterY, this._targetCenterY, t);
                    base._owner.cameraFOV = Mathf.Lerp(this._origFOV, this._targetFOV, t);
                }
                else
                {
                    base._owner.anchorRadius = Mathf.Clamp(this._targetRadius, 4f, 11f);
                    base._owner.anchorElevation = this._targetElevation;
                    base._owner.followCenterY = this._targetCenterY;
                    base._owner.cameraFOV = this._targetFOV;
                    this._state = State.PullZ;
                }
            }
            else if (this._state == State.PullZ)
            {
                this._pullZTimer -= Time.deltaTime * base._owner.mainCamera.TimeScale;
                if (this._pullZTimer <= 0f)
                {
                    float a = 0.1f * Mathf.Abs((float) (this._targetFOV - this._origFOV));
                    float b = 0.35f * Mathf.Abs((float) (this._targetRadius - this._origRadius));
                    this._exitDuration = Mathf.Max(a, b);
                    this._state = State.Exiting;
                }
            }
            else if (this._state == State.Exiting)
            {
                this._timer += Time.deltaTime * base._owner.mainCamera.TimeScale;
                if (this._timer < this._exitDuration)
                {
                    base._owner.anchorRadius = Mathf.Clamp(Mathf.Lerp(this._targetRadius, this._origRadius, this._timer / this._exitDuration), 4f, 11f);
                    base._owner.anchorElevation = Mathf.Lerp(this._targetElevation, this._origElevation, this._timer / this._exitDuration);
                    base._owner.followCenterY = Mathf.Lerp(this._targetCenterY, this._origCenterY, this._timer / this._exitDuration);
                    base._owner.cameraFOV = Mathf.Lerp(this._targetFOV, this._origFOV, this._timer / this._exitDuration);
                }
                else
                {
                    base._owner.anchorRadius = Mathf.Clamp(this._origRadius, 4f, 11f);
                    base._owner.anchorElevation = this._origElevation;
                    base._owner.followCenterY = this._origCenterY;
                    base._owner.cameraFOV = this._origFOV;
                    base.End();
                }
            }
        }

        private enum State
        {
            Entering,
            PullZ,
            Exiting
        }
    }
}

