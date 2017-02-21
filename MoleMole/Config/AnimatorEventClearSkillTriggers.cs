namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class AnimatorEventClearSkillTriggers : AnimatorEvent
    {
        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
            (entity as BaseMonoAvatar).ClearSkillTriggers();
        }
    }
}

