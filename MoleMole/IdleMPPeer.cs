namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    public class IdleMPPeer : MPPeer
    {
        public static IdleMPPeer IDLE_PEER = new IdleMPPeer();

        public override void Connect(string ipAddress, int serverPort)
        {
        }

        public override void Core()
        {
        }

        public override void GetPeerStats(out ulong sendTotal, out ulong recvTotal, out ulong sendCount, out ulong recvCount)
        {
            sendTotal = 0L;
            recvTotal = 0L;
            sendCount = 0L;
            recvCount = 0L;
        }

        public override void GetPeerStats2(out string stat2)
        {
            stat2 = string.Empty;
        }

        public override void Init()
        {
        }

        public override void Listen(int serverPort = 0, string ipAddress = null)
        {
        }

        public override void SendByChannel(byte[] data, int len, int channel, int channelSequence)
        {
        }

        public override void ServerReady()
        {
        }

        public override void Shutdown()
        {
        }

        public override void StopListen()
        {
        }

        public override int channelSequenceCapacity
        {
            get
            {
                return 0;
            }
        }

        public override int peerID
        {
            get
            {
                return 0;
            }
        }

        public override int reliableChannel
        {
            get
            {
                return 0;
            }
        }

        public override MPPeer.PeerState state
        {
            get
            {
                return MPPeer.PeerState.Unitialized;
            }
        }

        public override int stateUpdateChannel
        {
            get
            {
                return 0;
            }
        }

        public override int totalPeerCount
        {
            get
            {
                return 0;
            }
        }
    }
}

