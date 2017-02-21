namespace MoleMole
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode]
    public class MonoEffectParticleUVAnm : BaseMonoEffectPlugin
    {
        private Material _material;
        private bool _prepared;
        private float _step_X;
        private float _step_Y;
        private int _totalFrames;
        public bool Continuous;
        [Range(1f, 1000f)]
        public int Cycles;
        public AnimationCurve FrameOverTime;
        [Header("Particle System To Use For UV Animation")]
        public ParticleSystem TargetParticleSystem;
        public int Tiles_X = 3;
        public int Tiles_Y = 2;

        public MonoEffectParticleUVAnm()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) };
            this.FrameOverTime = new AnimationCurve(keys);
            this.Cycles = 1;
        }

        protected override void Awake()
        {
            base.Awake();
            if (this.TargetParticleSystem == null)
            {
                this.TargetParticleSystem = base.GetComponent<ParticleSystem>();
            }
            this.Preparation();
        }

        private void ContinuousUpdate(float time)
        {
            float num = (time * this._totalFrames) / this.TargetParticleSystem.startLifetime;
            num = num % ((float) this._totalFrames);
            float x = (num % ((float) this.Tiles_X)) * this._step_X;
            float y = ((this.Tiles_Y - 1) - (num / ((float) this.Tiles_X))) * this._step_Y;
            this._material.SetTextureScale("_MainTex", new Vector2(this._step_X, this._step_Y));
            this._material.SetTextureOffset("_MainTex", new Vector2(x, y));
        }

        private void DiscreteUpdate(float time)
        {
            int num = (int) ((time * this._totalFrames) / (this.TargetParticleSystem.startLifetime + 0.001f));
            num = num % this._totalFrames;
            float x = (num % this.Tiles_X) * this._step_X;
            float y = ((this.Tiles_Y - 1) - (num / this.Tiles_X)) * this._step_Y;
            this._material.SetTextureScale("_MainTex", new Vector2(this._step_X, this._step_Y));
            this._material.SetTextureOffset("_MainTex", new Vector2(x, y));
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
        }

        private void Preparation()
        {
            if (this.TargetParticleSystem != null)
            {
                ParticleSystemRenderer component = this.TargetParticleSystem.GetComponent<ParticleSystemRenderer>();
                if (Application.isPlaying)
                {
                    GraphicsUtils.CreateAndAssignInstancedMaterial(component, component.sharedMaterial);
                    this._material = component.sharedMaterial;
                }
                else
                {
                    this._material = component.sharedMaterial;
                }
                this._totalFrames = this.Tiles_X * this.Tiles_Y;
                this._step_X = 1f / ((float) this.Tiles_X);
                this._step_Y = 1f / ((float) this.Tiles_Y);
                this._material.SetTextureScale("_MainTex", new Vector2(this._step_X, this._step_Y));
                this._prepared = true;
            }
        }

        public override void SetDestroy()
        {
        }

        public override void Setup()
        {
            base.Setup();
        }

        public void Update()
        {
            if (!this._prepared)
            {
                this.Preparation();
            }
            if (this._prepared && (this.TargetParticleSystem.particleCount != 0))
            {
                float time = this.TargetParticleSystem.time * this.Cycles;
                time = this.FrameOverTime.Evaluate(time);
                if (this.Continuous)
                {
                    this.ContinuousUpdate(time);
                }
                else
                {
                    this.DiscreteUpdate(time);
                }
            }
        }
    }
}

