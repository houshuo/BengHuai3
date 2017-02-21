namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginSpeedSensitive : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _lastPos;
        [SerializeField]
        private float[] _origRateMaxes;
        [Header("Max Speed")]
        public float maxSpeed = 1f;
        [Header("Target Particle Systems")]
        public ParticleSystem[] targetParticleSystems;

        private void Awake()
        {
            this._origRateMaxes = new float[this.targetParticleSystems.Length];
            this._lastPos = base.transform.position;
            for (int i = 0; i < this.targetParticleSystems.Length; i++)
            {
                ParticleSystem.EmissionModule emission = this.targetParticleSystems[i].emission;
                ParticleSystem.MinMaxCurve rate = emission.rate;
                this._origRateMaxes[i] = rate.constantMax;
                rate.curveScalar = 0f;
                emission.rate = rate;
            }
        }

        private void Update()
        {
            float num = Mathf.Min((float) 1f, (float) (Vector3.Distance(base.transform.position, this._lastPos) / (Time.deltaTime * this.maxSpeed)));
            for (int i = 0; i < this.targetParticleSystems.Length; i++)
            {
                ParticleSystem.EmissionModule emission = this.targetParticleSystems[i].emission;
                ParticleSystem.MinMaxCurve rate = emission.rate;
                rate.constantMax = this._origRateMaxes[i] * num;
                emission.rate = rate;
            }
            this._lastPos = base.transform.position;
        }
    }
}

