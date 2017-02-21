namespace MoleMole
{
    using System;

    public class EvtLocalAvatarChanged : BaseLevelEvent
    {
        public uint localAvatarID;
        public uint previousAvatarID;

        public EvtLocalAvatarChanged(uint localAvatarID)
        {
            this.localAvatarID = localAvatarID;
        }

        public EvtLocalAvatarChanged(uint localAvatarID, uint previousAvatarID)
        {
            this.localAvatarID = localAvatarID;
            this.previousAvatarID = previousAvatarID;
        }
    }
}

