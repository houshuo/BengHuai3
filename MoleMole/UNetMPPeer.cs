namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Networking;

    public class UNetMPPeer : MPPeer
    {
        private ClientConnectState _clientConnectState;
        private ConnectionConfig _connConfig;
        private GlobalConfig _globalConfig = new GlobalConfig();
        private int _hostID;
        private HostTopology _ht;
        private bool _isDelaySimulator;
        private int _peerID;
        private int[] _peerMap;
        private byte[] _recvBuffer;
        private int _recvChannelID;
        private int _recvConnID;
        private int _recvDataSize;
        private byte _recvErr;
        private byte[] _selfBuffer;
        private int _selfConnID;
        private int _serverConnectedPeerCount;
        private ConnectionSimulatorConfig _simulatorConfig;
        private MPPeer.PeerState _state = MPPeer.PeerState.Unitialized;
        private int _totalPeerCount;
        private const int INVALID_CONNECTION_ID = -1;
        private const int MAX_CONNECTION_COUNT = 5;
        private const int PEER_CHANNEL_COUNT = 2;
        private const int PEER_USE_CHANNEL = 0;
        private const int SIMULATOR_AVG_TIME_OUT = 0x4e2;
        private const int SIMULATOR_MAX_TIME_OUT = 0x5dc;
        private const int SIMULATOR_MIN_TIME_OUT = 0x3e8;

        public UNetMPPeer(bool delaySimulator = false)
        {
            this._globalConfig.ReactorModel = ReactorModel.FixRateReactor;
            this._globalConfig.ThreadAwakeTimeout = 0x16;
            this._globalConfig.MaxPacketSize = 0x3e8;
            ConnectionConfig defaultConfig = new ConnectionConfig {
                IsAcksLong = true,
                PacketSize = 800,
                MaxCombinedReliableMessageCount = 8,
                MaxCombinedReliableMessageSize = 0x40,
                DisconnectTimeout = 0x2710,
                PingTimeout = 0x2710
            };
            for (int i = 0; i < this.channelSequenceCapacity; i++)
            {
                defaultConfig.AddChannel(QosType.ReliableSequenced);
                defaultConfig.AddChannel(QosType.StateUpdate);
            }
            this._connConfig = defaultConfig;
            this._ht = new HostTopology(defaultConfig, 5);
            this._ht.ReceivedMessagePoolSize = 0x400;
            this._ht.SentMessagePoolSize = 0x400;
            this._recvBuffer = new byte[defaultConfig.PacketSize + 0x80];
            this._isDelaySimulator = delaySimulator;
            this._simulatorConfig = new ConnectionSimulatorConfig(0x3e8, 0x4e2, 0x3e8, 0x4e2, 0f);
        }

        private void CheckNetError(byte err, NetworkEventType evt = 3)
        {
            if (err != 0)
            {
                string message = string.Format("net error: {0} ", (NetworkError) err);
                if (evt != NetworkEventType.Nothing)
                {
                    message = message + " evt: " + evt;
                }
                throw new UnityException(message);
            }
        }

        public override void Connect(string ipAddress, int serverPort)
        {
            byte num;
            if (this._isDelaySimulator)
            {
                this._hostID = NetworkTransport.AddHostWithSimulator(this._ht, 0x3e8, 0x5dc);
                this._selfConnID = NetworkTransport.ConnectWithSimulator(this._hostID, ipAddress, serverPort, 0, out num, this._simulatorConfig);
            }
            else
            {
                this._hostID = NetworkTransport.AddHost(this._ht);
                this._selfConnID = NetworkTransport.Connect(this._hostID, ipAddress, serverPort, 0, out num);
            }
            if (num != 0)
            {
                throw new UnityException("bad connection : " + ((NetworkError) num));
            }
            this._state = MPPeer.PeerState.ClientConnecting;
            this._clientConnectState = ClientConnectState.Idle;
        }

        public override void Core()
        {
            if (this._state > MPPeer.PeerState.Inited)
            {
                NetworkEventType dataEvent = NetworkEventType.DataEvent;
                while ((dataEvent != NetworkEventType.Nothing) && (this._state > MPPeer.PeerState.Inited))
                {
                    bool flag;
                    int num2;
                    dataEvent = NetworkTransport.ReceiveFromHost(this._hostID, out this._recvConnID, out this._recvChannelID, this._recvBuffer, this._recvBuffer.Length, out this._recvDataSize, out this._recvErr);
                    if (this._state != MPPeer.PeerState.ServerListening)
                    {
                        goto Label_0167;
                    }
                    switch (dataEvent)
                    {
                        case NetworkEventType.DataEvent:
                        {
                            this.ServerDispatchDataEvent(dataEvent);
                            continue;
                        }
                        case NetworkEventType.ConnectEvent:
                        {
                            this.CheckNetError(this._recvErr, dataEvent);
                            this._serverConnectedPeerCount++;
                            byte index = (byte) (this._peerID + this._serverConnectedPeerCount);
                            this._peerMap[index] = this._recvConnID;
                            this._selfBuffer[0] = base.PackHeader(index, 1);
                            this._selfBuffer[1] = index;
                            this.SendTo(this._recvConnID, 0, this._selfBuffer, 2);
                            if (base.onConnected != null)
                            {
                                base.onConnected(index);
                            }
                            continue;
                        }
                        case NetworkEventType.DisconnectEvent:
                            flag = false;
                            num2 = 2;
                            goto Label_0143;

                        default:
                        {
                            continue;
                        }
                    }
                Label_00F7:
                    if ((this._peerMap[num2] != -1) && (this._peerMap[num2] == this._recvConnID))
                    {
                        this._peerMap[num2] = -1;
                        if (base.onDisconnected != null)
                        {
                            base.onDisconnected(num2);
                        }
                        flag = true;
                        continue;
                    }
                    num2++;
                Label_0143:
                    if (num2 <= (1 + this._serverConnectedPeerCount))
                    {
                        goto Label_00F7;
                    }
                    continue;
                Label_0167:
                    if (this._state == MPPeer.PeerState.ClientConnecting)
                    {
                        if (this._clientConnectState == ClientConnectState.Idle)
                        {
                            if (dataEvent == NetworkEventType.ConnectEvent)
                            {
                                this.CheckNetError(this._recvErr, dataEvent);
                                this._clientConnectState = ClientConnectState.JustConnectedWaitingForSetClientID;
                            }
                            else if (dataEvent == NetworkEventType.DataEvent)
                            {
                                Debug.LogError("not expecting data event before peer ID is set");
                            }
                        }
                        else if ((this._clientConnectState == ClientConnectState.JustConnectedWaitingForSetClientID) && (dataEvent == NetworkEventType.DataEvent))
                        {
                            this.CheckNetError(this._recvErr, dataEvent);
                            int num3 = base.ParseMessageType(this._recvBuffer);
                            int num4 = base.ParsePeerID(this._recvBuffer);
                            this._peerID = num4;
                            this._state = MPPeer.PeerState.ClientConnected;
                            this._clientConnectState = ClientConnectState.ConnectedAndGotClientID;
                            if (base.onConnected != null)
                            {
                                base.onConnected(this._selfConnID);
                            }
                        }
                    }
                    else
                    {
                        if (this._state != MPPeer.PeerState.ClientConnected)
                        {
                            goto Label_02FE;
                        }
                        switch (dataEvent)
                        {
                            case NetworkEventType.DataEvent:
                            {
                                int num5 = base.ParseMessageType(this._recvBuffer);
                                this.CheckNetError(this._recvErr, dataEvent);
                                if (num5 != 2)
                                {
                                    goto Label_02C7;
                                }
                                this._totalPeerCount = this._recvBuffer[1];
                                this._state = MPPeer.PeerState.Established;
                                if (base.onEstablished != null)
                                {
                                    base.onEstablished();
                                }
                                break;
                            }
                            case NetworkEventType.DisconnectEvent:
                                if (base.onDisconnected != null)
                                {
                                    base.onDisconnected(this._selfConnID);
                                }
                                this._state = MPPeer.PeerState.ClientDropped;
                                break;
                        }
                    }
                    continue;
                Label_02C7:
                    if (base.onPacket != null)
                    {
                        base.onPacket(this._recvBuffer, this._recvDataSize - 1, 1, this._recvChannelID % 2);
                    }
                    continue;
                Label_02FE:
                    if (this._state == MPPeer.PeerState.Established)
                    {
                        if (this.isServer)
                        {
                            switch (dataEvent)
                            {
                                case NetworkEventType.DataEvent:
                                {
                                    this.ServerDispatchDataEvent(dataEvent);
                                    continue;
                                }
                                case NetworkEventType.Nothing:
                                {
                                    continue;
                                }
                            }
                            continue;
                        }
                        switch (dataEvent)
                        {
                            case NetworkEventType.DataEvent:
                            {
                                this.CheckNetError(this._recvErr, dataEvent);
                                if (base.onPacket != null)
                                {
                                    base.onPacket(this._recvBuffer, this._recvDataSize - 1, 1, this._recvChannelID % 2);
                                }
                                continue;
                            }
                            case NetworkEventType.Nothing:
                            {
                                continue;
                            }
                        }
                    }
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
                byte num;
                stat2 = string.Format("rtt: {0,5:0000.00}", NetworkTransport.GetCurrentRtt(this._hostID, this._selfConnID, out num));
            }
        }

        public override void Init()
        {
            NetworkTransport.Init(this._globalConfig);
            this._state = MPPeer.PeerState.Inited;
            this._selfConnID = -1;
            this._hostID = -1;
            this._selfBuffer = new byte[0x10];
            this._serverConnectedPeerCount = 0;
            this._peerMap = new int[7];
            this._totalPeerCount = 0;
            this.ResetServerData();
        }

        public override void Listen(int serverPort = 0, string ipAddress = null)
        {
            if ((ipAddress != null) && (serverPort != 0))
            {
                this._hostID = NetworkTransport.AddHost(this._ht, serverPort, ipAddress);
            }
            else if ((ipAddress == null) && (serverPort != 0))
            {
                this._hostID = NetworkTransport.AddHost(this._ht, serverPort);
            }
            else
            {
                this._hostID = NetworkTransport.AddHost(this._ht);
            }
            this._peerID = 1;
            this._state = MPPeer.PeerState.ServerListening;
        }

        public override void OnGUI()
        {
            byte num;
            object[] args = new object[] { this._state, this.isServer, this._peerID, this._totalPeerCount, (this.state != MPPeer.PeerState.Established) ? "<>" : NetworkTransport.GetPacketReceivedRate(this._hostID, this._selfConnID, out num).ToString(), (this.state != MPPeer.PeerState.Established) ? "<>" : NetworkTransport.GetPacketSentRate(this._hostID, this._selfConnID, out num).ToString() };
            GUILayout.Label(string.Format("state: {0}, isServer: {1}, peerID: {2}, peerCnt: {3}, trans recv: {4,5}, trans send {5,5}", args), new GUILayoutOption[0]);
        }

        private void ResetServerData()
        {
            this._serverConnectedPeerCount = 0;
            for (int i = 0; i < this._peerMap.Length; i++)
            {
                this._peerMap[i] = -1;
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
            else
            {
                int num2 = (channelSequence * 2) + channel;
                if (this.isServer)
                {
                    if (index == 7)
                    {
                        for (int i = 2; i <= (1 + this._serverConnectedPeerCount); i++)
                        {
                            int connID = this._peerMap[i];
                            if (connID != -1)
                            {
                                this.SendTo(connID, num2, data, len);
                            }
                        }
                    }
                    else
                    {
                        int num5 = this._peerMap[index];
                        if (num5 != -1)
                        {
                            this.SendTo(num5, num2, data, len);
                        }
                    }
                }
                else
                {
                    this.SendTo(this._selfConnID, num2, data, len);
                }
            }
        }

        private void SendTo(int connID, int channel, byte[] data, int len)
        {
            byte num;
            NetworkTransport.Send(this._hostID, connID, channel, data, len, out num);
            this.CheckNetError(num, NetworkEventType.Nothing);
        }

        private void ServerDispatchDataEvent(NetworkEventType netEvent)
        {
            this.CheckNetError(this._recvErr, netEvent);
            int index = base.ParsePeerID(this._recvBuffer);
            if (index == 7)
            {
                if (base.onPacket != null)
                {
                    base.onPacket(this._recvBuffer, this._recvDataSize - 1, 1, this._recvChannelID % 2);
                }
                for (int i = 2; i <= (1 + this._serverConnectedPeerCount); i++)
                {
                    int connID = this._peerMap[i];
                    if ((connID != -1) && (connID != this._recvConnID))
                    {
                        this.SendTo(connID, this._recvChannelID, this._recvBuffer, this._recvDataSize);
                    }
                }
            }
            else if (index == this._peerID)
            {
                if (base.onPacket != null)
                {
                    base.onPacket(this._recvBuffer, this._recvDataSize - 1, 1, this._recvChannelID % 2);
                }
            }
            else
            {
                int num4 = this._peerMap[index];
                if (num4 != -1)
                {
                    this.SendTo(num4, this._recvChannelID, this._recvBuffer, this._recvDataSize);
                }
            }
        }

        public override void ServerReady()
        {
            this._totalPeerCount = 1 + this._serverConnectedPeerCount;
            for (int i = 2; i <= (1 + this._serverConnectedPeerCount); i++)
            {
                this._selfBuffer[0] = base.PackHeader((byte) i, 2);
                this._selfBuffer[1] = (byte) this._totalPeerCount;
                this.SendTo(this._peerMap[i], 0, this._selfBuffer, 2);
            }
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
                NetworkTransport.RemoveHost(this._hostID);
            }
            NetworkTransport.Shutdown();
            this.ResetServerData();
            this._clientConnectState = ClientConnectState.Idle;
            this._state = MPPeer.PeerState.Unitialized;
        }

        public override void StopListen()
        {
            NetworkTransport.RemoveHost(this._hostID);
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
                return 0;
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
                return 1;
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

