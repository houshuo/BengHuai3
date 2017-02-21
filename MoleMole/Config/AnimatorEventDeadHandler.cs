namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class AnimatorEventDeadHandler : AnimatorEvent
    {
        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
            entity.DeadHandler();
        }
    }
}

