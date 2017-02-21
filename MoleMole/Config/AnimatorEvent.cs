namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public abstract class AnimatorEvent
    {
        public bool forceTrigger;
        public bool forceTriggerOnTransitionOut;
        public float normalizedTime;

        protected AnimatorEvent()
        {
        }

        public AnimatorEvent Clone()
        {
            return (base.MemberwiseClone() as AnimatorEvent);
        }

        public abstract void HandleAnimatorEvent(BaseMonoAnimatorEntity entity);
    }
}

