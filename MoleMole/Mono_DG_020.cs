namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class Mono_DG_020 : BaseMonoDeadGal
    {
        public override void SetDied(KillEffect killEffect)
        {
            base.SetDied(killEffect);
            base.SetLocomotionRandom(2);
        }
    }
}

