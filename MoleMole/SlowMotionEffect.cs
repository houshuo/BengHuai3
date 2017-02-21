namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class SlowMotionEffect
    {
        private AnimationCurve _cameraElevationCurve;
        private float _cameraElevationOffset;
        private bool _cameraNearCase;
        private float _cameraRadiusOffset;
        private AnimationCurve _cameraRadiusOffsetCurve;
        private float _cameraRotateSpeed;
        private AnimationCurve _cameraSpeedCurve;
        private float _cameraSpeedCurveIntegral;
        private Vector4 _distanceAttenuationFactorsForElevationOffset;
        private Vector4 _distanceAttenuationFactorsForRotateAngle;
        private float _distTarget;
        private float _duration;
        private float _duringTimer;
        protected bool _hasUserControled;
        private float _maxCameraElevationOffset;
        private float _maxCameraRadiusOffset;
        private float _maxCameraRotateAngle;
        private float _minCameraElevationOffset;
        private float _minCameraRadiusOffset;
        private float _minCameraRotateAngle;
        private bool _nearCase;
        private float _origForwardDeltaAngle;
        private float _polarVelSign;
        private float _targetForwardDeltaAngle;
        private AnimationCurve _timeScaleCurve;

        private static float CalcCurveIntegral01(AnimationCurve curve)
        {
            float num = 0f;
            for (int i = 0; i < 60; i++)
            {
                num += curve.Evaluate(0.01666667f * i);
            }
            return (num * 0.01666667f);
        }

        public void Enter(MainCameraFollowState cameraState)
        {
            this._origForwardDeltaAngle = cameraState.forwardDeltaAngle;
            this._duringTimer = 0f;
            this._hasUserControled = false;
            float num = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(cameraState.cameraForward, cameraState.avatar.FaceDirection)));
            if (this._nearCase)
            {
                this._polarVelSign = (UnityEngine.Random.value <= 0.3f) ? -num : num;
            }
            else
            {
                this._polarVelSign = -num;
            }
            float num2 = UnityEngine.Random.Range(this._minCameraRotateAngle, this._maxCameraRotateAngle);
            this._cameraRotateSpeed = (num2 / this._duration) / this._cameraSpeedCurveIntegral;
            float t = UnityEngine.Random.value;
            this._cameraElevationOffset = Mathf.Lerp(this._maxCameraElevationOffset, this._minCameraElevationOffset, t);
            this._cameraRadiusOffset = UnityEngine.Random.Range(this._minCameraRadiusOffset, this._maxCameraRadiusOffset);
            if (this._cameraNearCase)
            {
                this._cameraRadiusOffset = UnityEngine.Random.Range(0f, this._maxCameraRadiusOffset);
            }
            Vector4 vector = this._distanceAttenuationFactorsForRotateAngle;
            float b = ((vector.x + (this._distTarget * vector.y)) + ((this._distTarget * this._distTarget) * vector.z)) + (((this._distTarget * this._distTarget) * this._distTarget) * vector.w);
            b = 1f / Mathf.Max(1f, b);
            this._cameraRotateSpeed *= b;
            vector = this._distanceAttenuationFactorsForElevationOffset;
            float num5 = ((vector.x + (this._distTarget * vector.y)) + ((this._distTarget * this._distTarget) * vector.z)) + (((this._distTarget * this._distTarget) * this._distTarget) * vector.w);
            num5 = 1f / Mathf.Max(1f, num5);
            this._cameraElevationOffset *= num5;
            if (!cameraState.followAvatarControlledRotate.active)
            {
                cameraState.TransitBaseState(cameraState.followAvatarState, false);
            }
        }

        public float GetProgress()
        {
            return (this._duringTimer / this._duration);
        }

        public bool OverDuration(float progress)
        {
            return ((this._duringTimer / this._duration) > progress);
        }

        public void Set(ConfigCameraSlowMotionKill config, float distTarget, float distCamera)
        {
            this.outParams = new OutParams();
            this._duration = config.Duration;
            this._timeScaleCurve = config.TimeScaleCurve;
            this._cameraSpeedCurve = config.CameraSpeedCurve;
            this._cameraRadiusOffsetCurve = config.CameraRadiusOffsetCurve;
            this._minCameraRotateAngle = config.MinCameraRotateAngle;
            this._maxCameraRotateAngle = config.MaxCameraRotateAngle;
            this._minCameraElevationOffset = config.MinCameraElevationOffset;
            this._maxCameraElevationOffset = config.MaxCameraElevationOffset;
            this._minCameraRadiusOffset = config.MinCameraRadiusOffset;
            this._maxCameraRadiusOffset = config.MaxCameraRadiusOffset;
            this._distanceAttenuationFactorsForRotateAngle = config.DistanceAttenuationFactorsForRotateAngle;
            this._distanceAttenuationFactorsForElevationOffset = config.DistanceAttenuationFactorsForElevationOffset;
            this._distTarget = distTarget;
            this._nearCase = distTarget < config.DistanceThreshold;
            this._cameraNearCase = distCamera < config.CameraDistanceThreshold;
            this._cameraSpeedCurveIntegral = CalcCurveIntegral01(this._cameraSpeedCurve);
            this._cameraElevationCurve = AnimationCurve.EaseInOut(0f, 0f, 0.5f, 1f);
            this._cameraElevationCurve.postWrapMode = WrapMode.PingPong;
        }

        public void Update()
        {
            this._duringTimer += Time.unscaledDeltaTime;
            float time = this._duringTimer / this._duration;
            this.outParams.timeScale = this._timeScaleCurve.Evaluate(time);
            this.outParams.anchorDeltaPolar = ((this._polarVelSign * this._cameraSpeedCurve.Evaluate(time)) * this._cameraRotateSpeed) * Time.unscaledDeltaTime;
            this.outParams.anchorElevationOffset = this._cameraElevationCurve.Evaluate(time) * this._cameraElevationOffset;
            this.outParams.anchorRadiusOffset = this._cameraRadiusOffsetCurve.Evaluate(time) * this._cameraRadiusOffset;
            this.outParams.forwardDeltaAngle = Mathf.Lerp(this._origForwardDeltaAngle, this._targetForwardDeltaAngle, time);
            if (this._duringTimer > this._duration)
            {
                this.active = false;
            }
        }

        public bool active { get; set; }

        public OutParams outParams { get; private set; }

        public class OutParams
        {
            public float anchorDeltaPolar;
            public float anchorElevationOffset;
            public float anchorRadiusOffset;
            public float forwardDeltaAngle;
            public float timeScale;
        }
    }
}

