namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    [RequireComponent(typeof(ParticleSystem))]
    public class Rain : MonoBehaviour
    {
        private ParticleSystem __particleSystem;
        private Material _material;
        private int _particleCount;
        private ParticleSystemRenderer _particleRenderer;
        private ParticleSystem.Particle[] _particles;
        [HideInInspector]
        public float area;
        public AnimationCurve opaquenessCurve;
        [Range(-3f, 3f)]
        public float origAudioPitch = 1f;
        [Range(0f, 1f)]
        public float origAudioVolumn = 1f;
        public float origOpaqueness = 1f;
        public float origSize = 0.005f;
        [Header("Appearance")]
        public float origSpeedStrech = 0.02f;
        public float radius = 20f;
        public AnimationCurve sheetAnmCurve;
        public AnimationCurve sizeCurve;
        public float speed = 1f;
        public AnimationCurve speedStrechCurve;
        [Header("Sheet animation")]
        public int tiles_X = 1;
        public int tiles_Y = 1;
        [HideInInspector]
        public float timeScale;

        private void GetParticle()
        {
            if ((this._particles == null) || (this._particles.Length < this._particleSystem.maxParticles))
            {
                this._particles = new ParticleSystem.Particle[this._particleSystem.maxParticles];
            }
            this._particleCount = this._particleSystem.GetParticles(this._particles);
        }

        public void Init()
        {
            this._particleRenderer = this._particleSystem.GetComponent<ParticleSystemRenderer>();
            this._material = this._particleRenderer.material;
            this.area = (3.141593f * this.radius) * this.radius;
        }

        private void OnDisable()
        {
        }

        private void OnEnable()
        {
        }

        private void SetOpaqueness(float t)
        {
            float num = this.opaquenessCurve.Evaluate(t) * this.origOpaqueness;
            this._material.SetFloat("_Opaqueness", Mathf.Clamp01(num));
        }

        private void SetParticle()
        {
            this._particleSystem.SetParticles(this._particles, this._particleCount);
        }

        private void SetPlaybackSpeed(float t)
        {
            this._particleSystem.playbackSpeed = this.timeScale * this.speed;
        }

        private void SetSheetAnimation(float t)
        {
            int num = this.tiles_X * this.tiles_Y;
            int num2 = Mathf.FloorToInt(this.sheetAnmCurve.Evaluate(t) * num);
            float x = 1f / ((float) this.tiles_X);
            float y = 1f / ((float) this.tiles_Y);
            float num5 = (num2 % this.tiles_X) * x;
            float num6 = (((num - 1) - num2) / this.tiles_X) * y;
            this._material.SetTextureScale("_MainTex", new Vector2(x, y));
            this._material.SetTextureOffset("_MainTex", new Vector2(num5, num6));
        }

        private void SetSize(float t)
        {
            float num = this.origSize * this.sizeCurve.Evaluate(t);
            for (int i = 0; i < this._particleCount; i++)
            {
                this._particles[i].startSize = num;
            }
        }

        private void SetSound(float t)
        {
        }

        public void SetUp(ConfigRain config)
        {
            this._particleSystem.emission.rate = new ParticleSystem.MinMaxCurve(config.density * this.area);
            this.speed = config.speed;
            this.origSpeedStrech = config.speedStrech;
            this.origSize = config.size;
            this.origOpaqueness = config.opaqueness;
            this.SetUpAudio(config);
        }

        public void SetUpAudio(ConfigRain config)
        {
        }

        private void SetVelocityScale(float t)
        {
            this._particleRenderer.velocityScale = this.origSpeedStrech * this.speedStrechCurve.Evaluate(t);
        }

        private void Update()
        {
            this.UpdateMisc();
            this.GetParticle();
            float t = this.timeScale * Time.timeScale;
            this.SetPlaybackSpeed(t);
            this.SetSheetAnimation(t);
            this.SetSize(t);
            this.SetVelocityScale(t);
            this.SetOpaqueness(t);
            this.SetParticle();
            this.SetSound(t);
        }

        private void UpdateMisc()
        {
            if (Application.isEditor)
            {
                this.area = (3.141593f * this.radius) * this.radius;
            }
        }

        private ParticleSystem _particleSystem
        {
            get
            {
                if (this.__particleSystem == null)
                {
                    this.__particleSystem = base.GetComponent<ParticleSystem>();
                }
                return this.__particleSystem;
            }
        }
    }
}

