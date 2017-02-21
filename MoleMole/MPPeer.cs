namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public abstract class MPPeer
    {
        public const byte INVALID_PEER_ID = 0;
        public const byte MASTER_PEER_ID = 1;
        public ConnectedHandler onConnected;
        public DisconnectedHandler onDisconnected;
        public EstablishedHandler onEstablished;
        public ReceiveHandler onPacket;
        public const byte PACKET_METATYPE_MASK = 0x1f;
        public const byte PACKET_METATYPE_SERVER_FINALIZED = 2;
        public const byte PACKET_METATYPE_SET_CLIENT_PEER_ID = 1;
        public const byte PACKET_METATYPE_USER_MESSAGE = 0x1f;
        public const byte PACKET_PEER_ID_MASK = 0xe0;
        public const int PEER_ARR_COUNT = 7;
        public const int PEER_PACKET_RESERVE_BYTE_COUNT = 1;
        public const byte SEND_TO_ALL_PEER_ID = 7;

        protected MPPeer()
        {
        }

        public virtual bool CanSend()
        {
            return (((this.state == PeerState.Established) || (this.state == PeerState.ClientConnected)) || (this.state == PeerState.ServerListening));
        }

        public abstract void Connect(string ipAddress, int serverPort);
        public abstract void Core();
        public abstract void GetPeerStats(out ulong sendTotal, out ulong recvTotal, out ulong sendCount, out ulong recvCount);
        public abstract void GetPeerStats2(out string stat2);
        public abstract void Init();
        public abstract void Listen(int serverPort = 0, string ipAddress = null);
        public virtual void OnGUI()
        {
        }

        public byte PackHeader(byte peerID, byte metaType)
        {
            return (byte) ((peerID << 5) | metaType);
        }

        public int ParseMessageType(byte[] data)
        {
            return (data[0] & 0x1f);
        }

        public int ParsePeerID(byte[] data)
        {
            return ((data[0] & 0xe0) >> 5);
        }

        public abstract void SendByChannel(byte[] data, int len, int channel, int channelSequence);
        public abstract void ServerReady();
        public abstract void Shutdown();
        public abstract void StopListen();

        public abstract int channelSequenceCapacity { get; }

        public abstract int peerID { get; }

        public abstract int reliableChannel { get; }

        public abstract PeerState state { get; }

        public abstract int stateUpdateChannel { get; }

        public abstract int totalPeerCount { get; }

        public delegate void ConnectedHandler(int connID);

        public delegate void DisconnectedHandler(int connID);

        public delegate void EstablishedHandler();

        public enum PeerState
        {
            Unitialized,
            Inited,
            ServerListening,
            ClientConnecting,
            ClientConnected,
            ClientDropped,
            Established
        }

        public delegate void ReceiveHandler(byte[] buffer, int len, int offset, int channel);
    }
}

