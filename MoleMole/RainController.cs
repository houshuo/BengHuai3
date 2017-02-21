namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class RainController : MonoBehaviour
    {
        private float __lastSystemTimeScale;
        private ParticleSystem __splashParticleSystem;
        private float _currentTimeScale;
        private SlowModeState _slowModeState = SlowModeState.OUT;
        private float _startTimeScale;
        private float _transTimer;
        private static readonly float MAX_ACCEPTED_SYSTEM_TIMESCALE_DELTA = 2f;
        public Rain rain;
        public float slowModeEnterDuration = 0.6f;
        public float slowModeLeaveDuration = 0.6f;
        [Range(0f, 1f)]
        public float slowModeTimeScale;
        public GameObject splash;
        [Range(0f, 1f)]
        public float timeScale;
        public AnimationCurve timeScaleRemapCurve;

        public void EnterSlowMode(float startTimeScale)
        {
            if (this._slowModeState == SlowModeState.OUT)
            {
                this._slowModeState = SlowModeState.ENTERING;
                this._transTimer = 0f;
                this._startTimeScale = startTimeScale;
            }
        }

        public void Init()
        {
            this.__lastSystemTimeScale = Time.timeScale;
            this.rain.Init();
        }

        public void LeaveSlowMode()
        {
            if (this._slowModeState == SlowModeState.IN)
            {
                this._slowModeState = SlowModeState.LEAVING;
                this._transTimer = 0f;
            }
        }

        public void SetRain(ConfigRain config)
        {
            this.rain.SetUp(config);
            this._splashParticleSystem.emission.rate = new ParticleSystem.MinMaxCurve(config.splashDensity * this.rain.area);
            Material material = this._splashParticleSystem.GetComponent<Renderer>().material;
            Color color = material.GetColor("_TintColor");
            material.SetColor("_TintColor", new Color(color.r, color.g, color.b, config.splashOpaqueness * 0.5f));
        }

        private void SetupTimeScale()
        {
            this.UpdateSlowMode();
            float num = this.timeScaleRemapCurve.Evaluate(this._currentTimeScale * this._systemTimeScale) / Mathf.Max(0.001f, Time.timeScale);
            this.rain.timeScale = num;
            this._splashParticleSystem.playbackSpeed = num;
        }

        private void Update()
        {
            this.SetupTimeScale();
            Vector3 followCenterXZPosition = Singleton<CameraManager>.Instance.GetMainCamera().followState.followCenterXZPosition;
            followCenterXZPosition.y = 0f;
            base.transform.position = followCenterXZPosition;
        }

        private void UpdateSlowMode()
        {
            if (this._slowModeState == SlowModeState.IN)
            {
                this._currentTimeScale = this.slowModeTimeScale;
            }
            else if (this._slowModeState == SlowModeState.OUT)
            {
                this._currentTimeScale = this.timeScale;
            }
            else if (this._slowModeState == SlowModeState.ENTERING)
            {
                this._transTimer += Time.unscaledDeltaTime;
                this._currentTimeScale = Mathf.Lerp(this._startTimeScale, this.slowModeTimeScale, Mathf.Clamp01(this._transTimer / this.slowModeEnterDuration));
                if (this._transTimer > this.slowModeEnterDuration)
                {
                    this._slowModeState = SlowModeState.IN;
                }
            }
            else if (this._slowModeState == SlowModeState.LEAVING)
            {
                this._transTimer += Time.unscaledDeltaTime;
                this._currentTimeScale = Mathf.Lerp(this.slowModeTimeScale, this.timeScale, Mathf.Clamp01(this._transTimer / this.slowModeLeaveDuration));
                if (this._transTimer > this.slowModeLeaveDuration)
                {
                    this._slowModeState = SlowModeState.OUT;
                }
            }
            if (this._slowModeState != SlowModeState.OUT)
            {
                this._currentTimeScale /= this._systemTimeScale;
            }
        }

        private ParticleSystem _splashParticleSystem
        {
            get
            {
                if (this.__splashParticleSystem == null)
                {
                    this.__splashParticleSystem = this.splash.GetComponent<ParticleSystem>();
                }
                return this.__splashParticleSystem;
            }
        }

        private float _systemTimeScale
        {
            get
            {
                this.__lastSystemTimeScale = (Mathf.Abs((float) (Time.timeScale - this.__lastSystemTimeScale)) >= MAX_ACCEPTED_SYSTEM_TIMESCALE_DELTA) ? this.__lastSystemTimeScale : Time.timeScale;
                return this.__lastSystemTimeScale;
            }
        }

        private enum SlowModeState
        {
            IN,
            OUT,
            ENTERING,
            LEAVING
        }
    }
}

