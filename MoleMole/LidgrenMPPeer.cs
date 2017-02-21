namespace MoleMole
{
    using Lidgren.Network;
    using System;
    using System.Net;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class LidgrenMPPeer : MPPeer
    {
        private NetClient _lidgrenClient;
        private NetPeer _lidgrenPeer;
        private NetServer _lidgrenServer;
        private int _peerID;
        private NetConnection[] _peerMap;
        private int _serverConnectedPeerCount;
        private MPPeer.PeerState _state = MPPeer.PeerState.Unitialized;
        private int _totalPeerCount;
        private const string APP_ID = "ng";
        private const int BUFFER_SIZE = 0x400;
        private const int PEER_USE_SEQUENCE_ID = 0;

        public override void Connect(string ipAddress, int serverPort)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("ng") {
                AutoFlushSendQueue = true
            };
            this._lidgrenClient = new NetClient(config);
            this._lidgrenPeer = this._lidgrenClient;
            this._lidgrenClient.Start();
            NetOutgoingMessage hailMessage = this._lidgrenClient.CreateMessage(8);
            hailMessage.Write("ng");
            this._lidgrenClient.Connect(ipAddress, serverPort, hailMessage);
            this._state = MPPeer.PeerState.ClientConnecting;
        }

        public override void Core()
        {
            if (this._state > MPPeer.PeerState.Inited)
            {
                NetIncomingMessage message;
                while ((message = this._lidgrenPeer.ReadMessage()) != null)
                {
                    byte num;
                    int num2;
                    NetConnectionStatus status;
                    bool flag;
                    int num3;
                    NetIncomingMessageType messageType = message.MessageType;
                    switch (messageType)
                    {
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            Debug.LogError(message.ReadString());
                            break;
                    }
                    if (this._state != MPPeer.PeerState.ServerListening)
                    {
                        goto Label_024D;
                    }
                    messageType = message.MessageType;
                    switch (messageType)
                    {
                        case NetIncomingMessageType.StatusChanged:
                            status = (NetConnectionStatus) message.ReadByte();
                            Debug.Log(message.ReadString());
                            if (status != NetConnectionStatus.Disconnected)
                            {
                                goto Label_01C2;
                            }
                            flag = false;
                            num3 = 2;
                            goto Label_01AE;

                        case NetIncomingMessageType.ConnectionApproval:
                            if (!(message.ReadString() == "ng"))
                            {
                                goto Label_0130;
                            }
                            num = 0;
                            num2 = 2;
                            goto Label_00D2;

                        case NetIncomingMessageType.Data:
                            this.ServerDispatchDataEvent(message);
                            goto Label_0498;

                        default:
                            goto Label_0498;
                    }
                Label_00B9:
                    if (this._peerMap[num2] == null)
                    {
                        num = (byte) num2;
                        goto Label_00E0;
                    }
                    num2++;
                Label_00D2:
                    if (num2 < this._peerMap.Length)
                    {
                        goto Label_00B9;
                    }
                Label_00E0:
                    this._peerMap[num] = message.SenderConnection;
                    NetOutgoingMessage localHail = this._lidgrenPeer.CreateMessage();
                    localHail.Data[0] = base.PackHeader(num, 1);
                    localHail.Data[1] = num;
                    localHail.LengthBytes = 2;
                    message.SenderConnection.Approve(localHail);
                    goto Label_0498;
                Label_0130:
                    message.SenderConnection.Deny();
                    goto Label_0498;
                Label_016A:
                    if (this._peerMap[num3].RemoteUniqueIdentifier == message.SenderConnection.RemoteUniqueIdentifier)
                    {
                        flag = true;
                        this._peerMap[num3] = null;
                        this._serverConnectedPeerCount--;
                        goto Label_0498;
                    }
                    num3++;
                Label_01AE:
                    if (num3 < this._peerMap.Length)
                    {
                        goto Label_016A;
                    }
                    goto Label_0498;
                Label_01C2:
                    if (status == NetConnectionStatus.Connected)
                    {
                        int connID = 0;
                        for (int i = 2; i < this._peerMap.Length; i++)
                        {
                            if (this._peerMap[i].RemoteUniqueIdentifier == message.SenderConnection.RemoteUniqueIdentifier)
                            {
                                connID = i;
                                break;
                            }
                        }
                        this._serverConnectedPeerCount++;
                        if (base.onConnected != null)
                        {
                            base.onConnected(connID);
                        }
                    }
                    goto Label_0498;
                Label_024D:
                    if (this._state == MPPeer.PeerState.ClientConnecting)
                    {
                        if (message.MessageType == NetIncomingMessageType.StatusChanged)
                        {
                            NetConnectionStatus status2 = (NetConnectionStatus) message.ReadByte();
                            string str3 = message.ReadString();
                            Debug.Log(string.Format("{0}, {1}", status2, str3));
                            if (status2 == NetConnectionStatus.Connected)
                            {
                                NetIncomingMessage remoteHailMessage = message.SenderConnection.RemoteHailMessage;
                                int num6 = base.ParsePeerID(remoteHailMessage.Data);
                                int num7 = base.ParseMessageType(remoteHailMessage.Data);
                                int num8 = remoteHailMessage.Data[1];
                                this._peerID = num6;
                                this._state = MPPeer.PeerState.ClientConnected;
                                if (base.onConnected != null)
                                {
                                    base.onConnected(this._peerID);
                                }
                            }
                        }
                        else if (message.MessageType != NetIncomingMessageType.Data)
                        {
                        }
                    }
                    else
                    {
                        if (this._state != MPPeer.PeerState.ClientConnected)
                        {
                            goto Label_03F7;
                        }
                        messageType = message.MessageType;
                        if (messageType == NetIncomingMessageType.StatusChanged)
                        {
                            NetConnectionStatus status3 = (NetConnectionStatus) message.ReadByte();
                            Debug.Log(message.ReadString());
                            if (status3 == NetConnectionStatus.Disconnected)
                            {
                                if (base.onDisconnected != null)
                                {
                                    base.onDisconnected(this._peerID);
                                }
                                this._state = MPPeer.PeerState.ClientDropped;
                            }
                        }
                        else if (messageType == NetIncomingMessageType.Data)
                        {
                            goto Label_037C;
                        }
                    }
                    goto Label_0498;
                Label_037C:
                    if (base.ParseMessageType(message.Data) == 2)
                    {
                        this._totalPeerCount = message.Data[1];
                        this._state = MPPeer.PeerState.Established;
                        if (base.onEstablished != null)
                        {
                            base.onEstablished();
                        }
                    }
                    else if (base.onPacket != null)
                    {
                        base.onPacket(message.Data, message.LengthBytes - 1, 1, (int) message.DeliveryMethod);
                    }
                    goto Label_0498;
                Label_03F7:
                    if (this._state == MPPeer.PeerState.Established)
                    {
                        if (this.isServer)
                        {
                            messageType = message.MessageType;
                            if ((messageType != NetIncomingMessageType.StatusChanged) && (messageType == NetIncomingMessageType.Data))
                            {
                                this.ServerDispatchDataEvent(message);
                            }
                        }
                        else
                        {
                            messageType = message.MessageType;
                            if (((messageType != NetIncomingMessageType.StatusChanged) && (messageType == NetIncomingMessageType.Data)) && (base.onPacket != null))
                            {
                                base.onPacket(message.Data, message.LengthBytes - 1, 1, (int) message.DeliveryMethod);
                            }
                        }
                    }
                Label_0498:
                    this._lidgrenPeer.Recycle(message);
                }
            }
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
            if (this.isServer)
            {
                stat2 = "<server>";
            }
            else
            {
                stat2 = string.Format("{0,5:0000.00}", this._lidgrenClient.ServerConnection.AverageRoundtripTime * 1000f);
            }
        }

        public override void Init()
        {
            this._state = MPPeer.PeerState.Inited;
            this._peerMap = new NetConnection[7];
            this._serverConnectedPeerCount = 0;
        }

        public override void Listen(int serverPort = 0, string ipAddress = null)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("ng") {
                AutoFlushSendQueue = true
            };
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            if (serverPort != 0)
            {
                config.Port = serverPort;
            }
            if (ipAddress != null)
            {
                config.LocalAddress = IPAddress.Parse(ipAddress);
            }
            else if (ipAddress == null)
            {
                config.LocalAddress = IPAddress.Parse("0.0.0.0");
            }
            this._lidgrenServer = new NetServer(config);
            this._lidgrenPeer = this._lidgrenServer;
            this._lidgrenServer.Start();
            this._peerID = 1;
            this._state = MPPeer.PeerState.ServerListening;
        }

        public override void OnGUI()
        {
            if (this._state > MPPeer.PeerState.Inited)
            {
                object[] args = new object[] { this._state, this.isServer, this._peerID, this._totalPeerCount, this._lidgrenPeer.UniqueIdentifier };
                GUILayout.Label(string.Format("state: {0}, isServer: {1}, peerID: {2}, peerCnt: {3}, peerUID: {4}", args), new GUILayoutOption[0]);
            }
        }

        private void ResetServerData()
        {
            this._serverConnectedPeerCount = 0;
            for (int i = 0; i < this._peerMap.Length; i++)
            {
                this._peerMap[i] = null;
            }
        }

        public override void SendByChannel(byte[] data, int len, int channel, int channelSequence)
        {
            int index = base.ParsePeerID(data);
            if (index == this._peerID)
            {
                if (base.onPacket != null)
                {
                    base.onPacket(data, len - 1, 1, channel);
                }
            }
            else if (this.isServer)
            {
                if (index == 7)
                {
                    for (int i = 2; i < this._peerMap.Length; i++)
                    {
                        NetConnection conn = this._peerMap[i];
                        if (conn != null)
                        {
                            this.SendTo(conn, (NetDeliveryMethod) ((byte) channel), data, len, channelSequence);
                        }
                    }
                }
                else
                {
                    NetConnection connection2 = this._peerMap[index];
                    if (connection2 != null)
                    {
                        this.SendTo(connection2, (NetDeliveryMethod) ((byte) channel), data, len, channelSequence);
                    }
                }
            }
            else
            {
                this.SendTo(this._lidgrenClient.ServerConnection, (NetDeliveryMethod) ((byte) channel), data, len, channelSequence);
            }
        }

        private void SendTo(NetConnection conn, NetDeliveryMethod channel, byte[] data, int len, int channelSequence)
        {
            NetOutgoingMessage msg = this._lidgrenPeer.CreateMessage(data.Length);
            Buffer.BlockCopy(data, 0, msg.Data, 0, len);
            msg.LengthBytes = len;
            NetSendResult result = this._lidgrenPeer.SendMessage(msg, conn, channel, channelSequence);
        }

        private void ServerDispatchDataEvent(NetIncomingMessage recvMsg)
        {
            byte[] data = recvMsg.Data;
            int index = base.ParsePeerID(data);
            if (index == 7)
            {
                if (base.onPacket != null)
                {
                    base.onPacket(data, recvMsg.LengthBytes - 1, 1, (int) recvMsg.DeliveryMethod);
                }
                for (int i = 2; i < this._peerMap.Length; i++)
                {
                    NetConnection conn = this._peerMap[i];
                    if ((conn != null) && (conn.RemoteUniqueIdentifier != recvMsg.SenderConnection.RemoteUniqueIdentifier))
                    {
                        this.SendTo(conn, recvMsg.DeliveryMethod, data, recvMsg.LengthBytes, recvMsg.SequenceChannel);
                    }
                }
            }
            else if (index == this._peerID)
            {
                if (base.onPacket != null)
                {
                    base.onPacket(data, recvMsg.LengthBytes - 1, 1, (int) recvMsg.DeliveryMethod);
                }
            }
            else
            {
                NetConnection connection2 = this._peerMap[index];
                if (connection2 != null)
                {
                    this.SendTo(connection2, recvMsg.DeliveryMethod, data, recvMsg.LengthBytes, recvMsg.SequenceChannel);
                }
            }
        }

        public override void ServerReady()
        {
            this._totalPeerCount = 1 + this._serverConnectedPeerCount;
            byte[] data = new byte[0x40];
            data[0] = base.PackHeader(7, 2);
            data[1] = (byte) this._totalPeerCount;
            this.SendByChannel(data, data.Length, this.reliableChannel, 0);
            this._state = MPPeer.PeerState.Established;
            if (base.onEstablished != null)
            {
                base.onEstablished();
            }
        }

        public override void Shutdown()
        {
            if (this._state > MPPeer.PeerState.Inited)
            {
                this._lidgrenPeer.Shutdown("BYE");
            }
            this.ResetServerData();
            this._state = MPPeer.PeerState.Unitialized;
        }

        public override void StopListen()
        {
            this._lidgrenServer.Shutdown("what?");
            this._peerID = 0;
            this._state = MPPeer.PeerState.Inited;
            this.ResetServerData();
        }

        public override int channelSequenceCapacity
        {
            get
            {
                return 0x20;
            }
        }

        private bool isServer
        {
            get
            {
                return (this._peerID == 1);
            }
        }

        public override int peerID
        {
            get
            {
                return this._peerID;
            }
        }

        public override int reliableChannel
        {
            get
            {
                return 0x43;
            }
        }

        public override MPPeer.PeerState state
        {
            get
            {
                return this._state;
            }
        }

        public override int stateUpdateChannel
        {
            get
            {
                return 2;
            }
        }

        public override int totalPeerCount
        {
            get
            {
                return this._totalPeerCount;
            }
        }

        private enum ClientConnectState
        {
            Idle,
            JustConnectedWaitingForSetClientID,
            ConnectedAndGotClientID
        }
    }
}

