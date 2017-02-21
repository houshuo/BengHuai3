namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class AnimatorEventStopAllEffects : AnimatorEvent
    {
        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
            entity.StopAllEffects();
        }
    }
}

