namespace MoleMole
{
    using System;

    public class EvtAvatarSwapInEnd : BaseEvent
    {
        public EvtAvatarSwapInEnd(uint avatarID) : base(avatarID)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} swapped in", base.GetDebugName(base.targetID));
        }
    }
}

