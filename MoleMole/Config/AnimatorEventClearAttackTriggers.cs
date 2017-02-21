namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class AnimatorEventClearAttackTriggers : AnimatorEvent
    {
        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
            entity.ClearAttackTriggers();
        }
    }
}

