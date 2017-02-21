namespace MoleMole
{
    using System;
    using UnityEngine;

    public sealed class Mono_UL_010 : BaseMonoUlysses
    {
        public ParticleSystem headParticle;

        public override void OnTimeScaleChanged(float newTimeScale)
        {
            base.OnTimeScaleChanged(newTimeScale);
            this.headParticle.playbackSpeed = newTimeScale;
        }
    }
}

