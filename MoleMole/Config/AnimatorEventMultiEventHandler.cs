namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class AnimatorEventMultiEventHandler : AnimatorEvent
    {
        public string MultiEventID;

        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
            entity.MultiAnimEventHandler(this.MultiEventID);
        }
    }
}

