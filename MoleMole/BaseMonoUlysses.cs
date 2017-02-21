namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public abstract class BaseMonoUlysses : BaseMonoMonster
    {
        protected BaseMonoUlysses()
        {
        }

        public override void SetDied(KillEffect killEffect)
        {
            base.SetLocomotionRandom(2);
            base.SetDied(killEffect);
        }
    }
}

