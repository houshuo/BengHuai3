namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class AnimatorEventClearAttackTarget : AnimatorEvent
    {
        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
            entity.ClearAttackTarget();
        }
    }
}

