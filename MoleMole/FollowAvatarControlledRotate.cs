namespace MoleMole
{
    using FullInspector;
    using System;
    using UnityEngine;

    public class FollowAvatarControlledRotate : BaseFollowBaseState
    {
        [ShowInInspector]
        private float _elevationAngularVelocity;
        [ShowInInspector]
        private EntityTimer _exitTimer;
        [ShowInInspector]
        private bool _hasDragDataThisFrame;
        [ShowInInspector]
        private bool _hasZoomDataThisFrame;
        private float _lerpTimer;
        [ShowInInspector]
        private Vector2 _pointerDragDelta;
        [ShowInInspector]
        private float _polarAngularVelcity;
        [ShowInInspector]
        private State _state;
        private float _zoomDelta;
        private float _zoomVelocity;
        private const float ANGULAR_VELOCITY_DRAG_RATIO = 16f;
        private const float DELTA_TO_ANGULAR_ELEVATION_RATIO = 400f;
        private const float DELTA_TO_ANGULAR_POLAR_RATIO = 2000f;
        private const float DELTA_TO_ZOOM_END_RATIO = 10f;
        private const float DELTA_TO_ZOOM_RATIO = 3f;
        private const float DRAG_ELEVATION_MAX = 50f;
        private const float DRAG_ELEVATION_MIN = -10f;
        private const float FORWARD_LERP_INCREASE_RATIO = 9f;
        private const float MAX_ZOOM_RADIUS = 12f;
        private const float MIN_ZOOM_RADIUS = 4f;
        private const float POS_LERP_INCREASE_RATIO = 9f;
        private const float TO_EXIT_DURATION = 0.75f;

        public FollowAvatarControlledRotate(MainCameraFollowState followState) : base(followState)
        {
            base.maskedShortStates.Add(base._owner.standByState);
            base.maskedShortStates.Add(base._owner.lookAtPositionState);
            base.maskedShortStates.Add(base._owner.timedPullZState);
            this._exitTimer = new EntityTimer(0.75f, base._owner.mainCamera);
        }

        public override void Enter()
        {
            base._owner.recoverState.TryRecover();
            base._owner.recoverState.CancelElevationRecover();
            this._state = State.Entering;
            this._lerpTimer = 0f;
            this._exitTimer.Reset(false);
            base._owner.forwardLerpRatio = 1f;
            base._owner.posLerpRatio = 1f;
        }

        public override void Exit()
        {
            base._owner.forwardLerpRatio = 8f;
            base._owner.posLerpRatio = 8f;
            base._owner.recoverState.TryRecover();
        }

        public bool IsExiting()
        {
            return this._exitTimer.isActive;
        }

        public override void ResetState()
        {
        }

        public void SetDragDelta(Vector2 delta)
        {
            this._state = (!base._owner.lerpPositionOvershootLastFrame || !base._owner.lerpForwardOvershootLastFrame) ? State.Entering : State.Controlling;
            this._hasDragDataThisFrame = true;
            this._pointerDragDelta.x = -delta.x;
            this._pointerDragDelta.y = -delta.y;
        }

        public void SetExitingControl(bool exiting)
        {
            if (exiting)
            {
                this._exitTimer.Reset(true);
            }
            else
            {
                this._exitTimer.Reset(false);
            }
        }

        public void SetZoomDelta(float zoomDelta)
        {
            this._state = (!base._owner.lerpPositionOvershootLastFrame || !base._owner.lerpForwardOvershootLastFrame) ? State.Entering : State.Controlling;
            this._hasZoomDataThisFrame = true;
            this._zoomDelta = zoomDelta;
        }

        public override void Update()
        {
            if (this._state == State.Entering)
            {
                this.UpdateControlled();
                this._lerpTimer += Time.deltaTime * base._owner.mainCamera.TimeScale;
                if (base._owner.lerpPositionOvershootLastFrame && base._owner.lerpForwardOvershootLastFrame)
                {
                    base._owner.forwardLerpRatio = 1f;
                    base._owner.posLerpRatio = 1f;
                    base._owner.needLerpPositionThisFrame = false;
                    base._owner.needLerpForwardThisFrame = false;
                    this._state = State.Controlling;
                }
                else
                {
                    base._owner.forwardLerpRatio = this._lerpTimer * 9f;
                    base._owner.posLerpRatio = this._lerpTimer * 9f;
                    base._owner.needLerpPositionThisFrame = true;
                    base._owner.needLerpForwardThisFrame = true;
                }
            }
            else
            {
                this.UpdateControlled();
                base._owner.needLerpPositionThisFrame = false;
                base._owner.needLerpForwardThisFrame = false;
                this._exitTimer.Core(1f);
                if (this._exitTimer.isTimeUp && !Singleton<CameraManager>.Instance.controlledRotateKeepManual)
                {
                    base._owner.TryToTransitToOtherBaseState(true, null);
                }
            }
        }

        private void UpdateControlled()
        {
            if (this._hasDragDataThisFrame)
            {
                this._hasDragDataThisFrame = false;
                this._polarAngularVelcity += this._pointerDragDelta.x * 2000f;
                this._elevationAngularVelocity += this._pointerDragDelta.y * 400f;
            }
            if (this._hasZoomDataThisFrame)
            {
                this._hasZoomDataThisFrame = false;
                this._zoomVelocity = this._zoomDelta * 3f;
            }
            this._zoomVelocity = Mathf.Lerp(this._zoomVelocity, 0f, (Time.deltaTime * base._owner.mainCamera.TimeScale) * 10f);
            base._owner.anchorRadius += (this._zoomVelocity * base._owner.mainCamera.TimeScale) * Time.deltaTime;
            base._owner.anchorRadius = Mathf.Clamp(base._owner.anchorRadius, 4f, 12f);
            base._owner.anchorPolar += (Time.deltaTime * base._owner.mainCamera.TimeScale) * this._polarAngularVelcity;
            base._owner.anchorElevation += (Time.deltaTime * base._owner.mainCamera.TimeScale) * this._elevationAngularVelocity;
            base._owner.anchorElevation = Mathf.Clamp(base._owner.anchorElevation, -10f, 50f);
            this._polarAngularVelcity = Mathf.Lerp(this._polarAngularVelcity, 0f, (Time.deltaTime * base._owner.mainCamera.TimeScale) * 16f);
            this._elevationAngularVelocity = Mathf.Lerp(this._elevationAngularVelocity, 0f, (Time.deltaTime * base._owner.mainCamera.TimeScale) * 16f);
        }

        private enum State
        {
            Entering,
            Controlling
        }
    }
}

