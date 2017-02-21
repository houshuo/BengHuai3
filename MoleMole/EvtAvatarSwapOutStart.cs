namespace MoleMole
{
    using System;

    public class EvtAvatarSwapOutStart : BaseEvent
    {
        public EvtAvatarSwapOutStart(uint avatarID) : base(avatarID)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} swapped out", base.GetDebugName(base.targetID));
        }
    }
}

