namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginFakeWorldFollow : BaseMonoEffectPlugin
    {
        private int _curIx;
        private Vector3[] _holdPositions;
        private Quaternion[] _holdRotations;
        private float _timer;
        [Header("Main particle, should be like the rotating ones but is set to loop.")]
        public ParticleSystem MainParticle;
        public float PerParticleDuration = 0.5f;
        [Header("An array of particle systems that are the same.")]
        public ParticleSystem[] TailParticles;

        protected override void Awake()
        {
            base.Awake();
            this._holdPositions = new Vector3[this.TailParticles.Length];
            this._holdRotations = new Quaternion[this.TailParticles.Length];
        }

        private void FireTailAtIndex(int ix)
        {
            this.TailParticles[ix].Clear();
            this._holdPositions[ix] = this.MainParticle.transform.position;
            this._holdRotations[ix] = this.MainParticle.transform.rotation;
            this.TailParticles[ix].Emit(1);
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        private void LateUpdate()
        {
            this._timer += Time.deltaTime * base._effect.TimeScale;
            if (this._timer > this.PerParticleDuration)
            {
                this._curIx = (this._curIx + 1) % this.TailParticles.Length;
                this.FireTailAtIndex(this._curIx);
                this._timer = 0f;
            }
            for (int i = 0; i < this.TailParticles.Length; i++)
            {
                this.TailParticles[i].transform.position = this._holdPositions[i];
                this.TailParticles[i].transform.rotation = this._holdRotations[i];
            }
        }

        public override void SetDestroy()
        {
        }

        public override void Setup()
        {
            base.Setup();
            for (int i = 0; i < this.TailParticles.Length; i++)
            {
                this.TailParticles[i].Clear();
                this.TailParticles[i].Stop();
            }
            this._curIx = -1;
            this._timer = this.PerParticleDuration;
        }
    }
}

