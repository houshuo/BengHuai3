namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    public interface MiClientInterface
    {
        bool connect(string host, ushort port, int timeout_ms = 0x7d0);
        void disconnect();
        bool isConnected();
        NetPacketV1 recv(int timeout_ms = 0);
        bool send(NetPacketV1 packet);
        void setDisconnectCallback(Action callback);
        bool setKeepalive(int time_ms, NetPacketV1 packet);
    }
}

