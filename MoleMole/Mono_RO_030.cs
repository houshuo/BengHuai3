namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class Mono_RO_030 : BaseMonoRobot
    {
        public override void SetDied(KillEffect killEffect)
        {
            base.SetLocomotionRandom(2);
            base.SetDied(killEffect);
        }
    }
}

