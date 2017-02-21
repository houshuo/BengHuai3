namespace MoleMole
{
    using System;
    using UnityEngine;

    public class Mono_UL_041 : Mono_UL_040
    {
        public ParticleSystem weaponParticle;

        public override void OnTimeScaleChanged(float newTimeScale)
        {
            base.OnTimeScaleChanged(newTimeScale);
            this.weaponParticle.playbackSpeed = newTimeScale;
        }
    }
}

