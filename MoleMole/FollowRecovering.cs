namespace MoleMole
{
    using FullInspector;
    using System;
    using UnityEngine;

    public class FollowRecovering : State<MainCameraFollowState>
    {
        [ShowInInspector]
        private RecoverParameter _recoverCenterY;
        [ShowInInspector]
        private RecoverParameter _recoverElevation;
        [ShowInInspector]
        private RecoverParameter _recoverForwardDelta;
        [ShowInInspector]
        private RecoverParameter _recoverFOV;
        [ShowInInspector]
        private RecoverParameter _recoverLerpForwardRatio;
        [ShowInInspector]
        private RecoverParameter _recoverLerpPosRatio;
        [ShowInInspector]
        private RecoverParameter _recoverRadius;
        private const float ANGLE_RECOVER_DURATION_RATIO = 0.04f;
        private const float ELEVATION_ANGLE_RECOVER_DURATION_RATIO = 0.01f;
        private const float FOV_RECOVER_DURATION_RATIO = 0.1f;
        private const float LERPRATIO_RECOVER_DURATION_RATIO = 0.05f;
        private const float POS_RECOVER_DURATION_RATIO = 0.3f;

        public FollowRecovering(MainCameraFollowState owner) : base(owner)
        {
            base.SetActive(false);
            RecoverParameter.followState = base._owner;
            this._recoverRadius = new RecoverParameter(0.3f);
            this._recoverCenterY = new RecoverParameter(0.3f);
            this._recoverElevation = new RecoverParameter(0.01f);
            this._recoverForwardDelta = new RecoverParameter(0.04f);
            this._recoverLerpPosRatio = new RecoverParameter(0.05f);
            this._recoverLerpForwardRatio = new RecoverParameter(0.05f);
            this._recoverFOV = new RecoverParameter(0.1f);
        }

        public void CancelAndJumpToOriginalState()
        {
            base._owner.anchorRadius = this._recoverRadius.originalValue;
            base._owner.anchorElevation = this._recoverElevation.originalValue;
            base._owner.followCenterY = this._recoverCenterY.originalValue;
            base._owner.forwardDeltaAngle = this._recoverForwardDelta.originalValue;
            base._owner.posLerpRatio = this._recoverLerpPosRatio.originalValue;
            base._owner.forwardLerpRatio = this._recoverLerpForwardRatio.originalValue;
            base._owner.cameraFOV = this._recoverFOV.originalValue;
            this.Exit();
        }

        public void CancelAndStopAtCurrentState()
        {
            this.Exit();
        }

        public void CancelElevationRecover()
        {
            this._recoverElevation.needRecover = false;
        }

        public void CancelForwardDeltaAngleRecover()
        {
            this._recoverForwardDelta.needRecover = false;
        }

        public void CancelLerpRatioRecovering()
        {
            this._recoverLerpForwardRatio.needRecover = false;
            this._recoverLerpPosRatio.needRecover = false;
            base._owner.posLerpRatio = 1f;
            base._owner.forwardLerpRatio = 1f;
        }

        public void CancelPosAndForwardRecover()
        {
            this._recoverRadius.needRecover = false;
            this._recoverElevation.needRecover = false;
            this._recoverCenterY.needRecover = false;
            this._recoverForwardDelta.needRecover = false;
            this._recoverLerpForwardRatio.needRecover = false;
            this._recoverLerpPosRatio.needRecover = false;
        }

        public override void Enter()
        {
            base.SetActive(true);
        }

        public override void Exit()
        {
            this._recoverRadius.needRecover = false;
            this._recoverElevation.needRecover = false;
            this._recoverCenterY.needRecover = false;
            this._recoverForwardDelta.needRecover = false;
            this._recoverLerpForwardRatio.needRecover = false;
            this._recoverLerpPosRatio.needRecover = false;
            this._recoverFOV.needRecover = false;
            base.SetActive(false);
        }

        public float GetOriginalCenterY()
        {
            return this._recoverCenterY.originalValue;
        }

        public float GetOriginalElevation()
        {
            return this._recoverElevation.originalValue;
        }

        public float GetOriginalFOV()
        {
            return this._recoverFOV.originalValue;
        }

        public float GetOriginalRadius()
        {
            return this._recoverRadius.originalValue;
        }

        public void RecoverImmediately()
        {
            this._recoverRadius.RecoverImmediately(ref base._owner.anchorRadius);
            this._recoverElevation.RecoverImmediately(ref base._owner.anchorElevation);
            this._recoverCenterY.RecoverImmediately(ref base._owner.followCenterY);
            this._recoverForwardDelta.RecoverImmediately(ref base._owner.forwardDeltaAngle);
            this._recoverLerpPosRatio.RecoverImmediately(ref base._owner.posLerpRatio);
            this._recoverLerpForwardRatio.RecoverImmediately(ref base._owner.forwardLerpRatio);
            this._recoverFOV.RecoverImmediately(ref base._owner.cameraFOV);
        }

        public void SetupRecoverCenterY(float origCenterY)
        {
            this._recoverCenterY.originalValue = origCenterY;
        }

        public void SetupRecoverElevation(float origElevation)
        {
            this._recoverElevation.originalValue = origElevation;
        }

        public void SetupRecoverForwardDelta(float origForwardDelta)
        {
            this._recoverForwardDelta.originalValue = origForwardDelta;
        }

        public void SetupRecoverFOV(float origFOV)
        {
            this._recoverFOV.originalValue = origFOV;
        }

        public void SetupRecoverLerpForwardRatio(float origLerpForwardRatio)
        {
            this._recoverLerpForwardRatio.originalValue = origLerpForwardRatio;
        }

        public void SetupRecoverLerpPosRatio(float origLerpPosRatio)
        {
            this._recoverLerpPosRatio.originalValue = origLerpPosRatio;
        }

        public void SetupRecoverRadius(float origRadius)
        {
            this._recoverRadius.originalValue = origRadius;
        }

        public void TryRecover()
        {
            this._recoverRadius.SetupRecover(base._owner.anchorRadius);
            this._recoverElevation.SetupRecover(base._owner.anchorElevation);
            this._recoverCenterY.SetupRecover(base._owner.followCenterY);
            this._recoverForwardDelta.SetupRecover(base._owner.forwardDeltaAngle);
            this._recoverLerpPosRatio.SetupRecover(base._owner.posLerpRatio);
            this._recoverLerpForwardRatio.SetupRecover(base._owner.forwardLerpRatio);
            this._recoverFOV.SetupRecover(base._owner.cameraFOV);
            if (((this._recoverRadius.needRecover || this._recoverElevation.needRecover) || (this._recoverCenterY.needRecover || this._recoverForwardDelta.needRecover)) || ((this._recoverLerpPosRatio.needRecover || this._recoverLerpForwardRatio.needRecover) || this._recoverFOV.needRecover))
            {
                this.Enter();
            }
        }

        public override void Update()
        {
            if (this._recoverRadius.needRecover)
            {
                this._recoverRadius.LerpStep(ref base._owner.anchorRadius);
            }
            if (this._recoverElevation.needRecover)
            {
                this._recoverElevation.LerpStep(ref base._owner.anchorElevation);
            }
            if (this._recoverCenterY.needRecover)
            {
                this._recoverCenterY.LerpStep(ref base._owner.followCenterY);
            }
            if (this._recoverForwardDelta.needRecover)
            {
                this._recoverForwardDelta.LerpStep(ref base._owner.forwardDeltaAngle);
            }
            if (this._recoverLerpPosRatio.needRecover)
            {
                this._recoverLerpPosRatio.LerpStep(ref base._owner.posLerpRatio);
            }
            if (this._recoverLerpForwardRatio.needRecover)
            {
                this._recoverLerpForwardRatio.LerpStep(ref base._owner.forwardLerpRatio);
            }
            if (this._recoverFOV.needRecover)
            {
                this._recoverFOV.LerpStep(ref base._owner.cameraFOV);
            }
            if (((this._recoverRadius.IsDone() && this._recoverElevation.IsDone()) && (this._recoverCenterY.IsDone() && this._recoverForwardDelta.IsDone())) && ((this._recoverLerpPosRatio.IsDone() && this._recoverLerpForwardRatio.IsDone()) && this._recoverFOV.IsDone()))
            {
                base._owner.recoverState.Exit();
            }
        }

        private class RecoverParameter
        {
            private float _duration;
            private float _durationRatio;
            private float _startValue;
            private float _timer;
            public static MainCameraFollowState followState;
            public bool needRecover;
            public float originalValue;

            public RecoverParameter(float durationRatio)
            {
                this._durationRatio = durationRatio;
            }

            public bool IsDone()
            {
                return (!this.needRecover || (this._timer >= this._duration));
            }

            public void LerpStep(ref float target)
            {
                if (this._timer <= this._duration)
                {
                    this._timer += Time.deltaTime * followState.mainCamera.TimeScale;
                    if (this._timer < this._duration)
                    {
                        target = Mathf.Lerp(this._startValue, this.originalValue, this._timer / this._duration);
                    }
                    else
                    {
                        target = this.originalValue;
                    }
                }
            }

            public void RecoverImmediately(ref float target)
            {
                target = this.originalValue;
                this.needRecover = false;
            }

            public void SetupRecover(float startValue)
            {
                if (this.needRecover && (this._timer < this._duration))
                {
                    startValue = Mathf.Lerp(this._startValue, this.originalValue, this._timer / this._duration);
                }
                if (startValue == this.originalValue)
                {
                    this.needRecover = false;
                }
                else
                {
                    this.needRecover = true;
                    this._startValue = startValue;
                    this._duration = this._durationRatio * Mathf.Abs((float) (this._startValue - this.originalValue));
                    this._timer = 0f;
                }
            }
        }
    }
}

