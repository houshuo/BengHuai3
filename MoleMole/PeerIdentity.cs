namespace MoleMole
{
    using System;

    public class PeerIdentity : BaseMPIdentity
    {
        public override void OnStateUpdatePacket(MPRecvPacketContainer pc)
        {
        }

        public override IdentityRemoteMode remoteMode
        {
            get
            {
                return IdentityRemoteMode.Mute;
            }
        }
    }
}

