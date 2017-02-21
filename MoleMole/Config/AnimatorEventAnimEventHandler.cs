namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class AnimatorEventAnimEventHandler : AnimatorEvent
    {
        public string AnimEventID;

        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
            entity.AnimEventHandler(this.AnimEventID);
        }
    }
}

