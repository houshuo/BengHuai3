namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoEffectPluginParticleRotateAxis : BaseMonoEffectPlugin
    {
        private List<ParticleSystem.Particle[]> _particles;
        public Vector3 axis = Vector3.forward;
        public int maxParticleCount = 0x20;
        public ParticleSystem[] targetParticleSystems;

        protected override void Awake()
        {
            base.Awake();
            this._particles = new List<ParticleSystem.Particle[]>();
            for (int i = 0; i < this.targetParticleSystems.Length; i++)
            {
                this._particles.Add(new ParticleSystem.Particle[this.maxParticleCount]);
            }
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        public override void SetDestroy()
        {
        }

        private void SetParticleAxis()
        {
            for (int i = 0; i < this.targetParticleSystems.Length; i++)
            {
                ParticleSystem system = this.targetParticleSystems[i];
                if ((system != null) && system.IsAlive())
                {
                    ParticleSystem.Particle[] particles = this._particles[i];
                    system.GetParticles(particles);
                    for (int j = 0; j < particles.Length; j++)
                    {
                        particles[j].axisOfRotation = this.axis;
                    }
                }
            }
        }

        private void Update()
        {
            this.SetParticleAxis();
        }
    }
}

