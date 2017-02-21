namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class AnimatorEventStopAllEffectsImmediately : AnimatorEvent
    {
        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
            entity.StopAllEffectsImmediately();
        }
    }
}

