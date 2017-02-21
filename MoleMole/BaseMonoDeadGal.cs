namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public abstract class BaseMonoDeadGal : BaseMonoMonster
    {
        protected BaseMonoDeadGal()
        {
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void SetDied(KillEffect killEffect)
        {
            base.SetLocomotionRandom(2);
            base.SetDied(killEffect);
        }
    }
}

