namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginParticleWorldRotate : BaseMonoEffectPlugin
    {
        [Header("Target Particle Systems To Rotate")]
        public ParticleSystem[] targetParticleSystems;

        public override bool IsToBeRemove()
        {
            return false;
        }

        private void OnAwake()
        {
            this.SyncStartRotation();
        }

        private void OnEanble()
        {
            this.SyncStartRotation();
        }

        public override void SetDestroy()
        {
        }

        private void Start()
        {
            for (int i = 0; i < this.targetParticleSystems.Length; i++)
            {
                this.targetParticleSystems[i].Clear();
            }
        }

        private void SyncStartRotation()
        {
            for (int i = 0; i < this.targetParticleSystems.Length; i++)
            {
                this.targetParticleSystems[i].startRotation = base.transform.rotation.eulerAngles.y * 0.01745329f;
            }
        }

        private void Update()
        {
            this.SyncStartRotation();
        }
    }
}

