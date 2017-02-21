namespace MoleMole
{
    using System;

    public class EvtAvatarCreated : BaseLevelEvent
    {
        public readonly uint avatarID;

        public EvtAvatarCreated(uint avatarID)
        {
            this.avatarID = avatarID;
        }

        public override string ToString()
        {
            return string.Format("{0} avatar created", base.GetDebugName(this.avatarID));
        }
    }
}

