namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Networking;

    public class UNetDiscovery
    {
        private int _clientHostID;
        private HostTopology _ht;
        private int _interval;
        private int _key;
        private byte[] _recvBuffer;
        protected int _recvChannelID;
        protected int _recvConnID;
        private int _recvDataSize;
        private byte _recvErr;
        private int _recvHostID;
        private int _serverHostID;
        private bool _setupTeardownTransport;
        private int _subVersion;
        private int _version;
        private const string ANY_IP = "<ANY>";
        private const int BROADCAST_PORT = 0xbaa1;
        public ClientDiscoveredServerDelegate onClientDiscoveredServer;

        public UNetDiscovery(int key, int version, int subVersion, int interval, bool setupTeardownTransport = false)
        {
            this._key = key;
            this._version = version;
            this._subVersion = subVersion;
            this._interval = interval;
            this._setupTeardownTransport = setupTeardownTransport;
            ConnectionConfig defaultConfig = new ConnectionConfig();
            defaultConfig.AddChannel(QosType.Unreliable);
            this._ht = new HostTopology(defaultConfig, 1);
            this._recvBuffer = new byte[0x400];
        }

        private void CheckHostID(int hostID)
        {
            if (hostID < 0)
            {
                throw new UnityException("failed to add host: ");
            }
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

        public void ClientStartListen()
        {
            byte num;
            this._clientHostID = NetworkTransport.AddHost(this._ht, 0xbaa1);
            this.CheckHostID(this._clientHostID);
            NetworkTransport.SetBroadcastCredentials(this._clientHostID, this._key, this._version, this._subVersion, out num);
            this.CheckNetError(num, NetworkEventType.Nothing);
            this.isClientBroadcasting = true;
        }

        public void ClientStopListen()
        {
            NetworkTransport.RemoveHost(this._clientHostID);
            this._clientHostID = -1;
            this.isClientBroadcasting = false;
        }

        public void Core()
        {
            if (this.isClientBroadcasting)
            {
                NetworkEventType dataEvent = NetworkEventType.DataEvent;
                while (this.isClientBroadcasting && (dataEvent != NetworkEventType.Nothing))
                {
                    dataEvent = NetworkTransport.ReceiveFromHost(this._clientHostID, out this._recvConnID, out this._recvChannelID, this._recvBuffer, this._recvBuffer.Length, out this._recvDataSize, out this._recvErr);
                    if (dataEvent == NetworkEventType.BroadcastEvent)
                    {
                        string str;
                        int num;
                        string str2;
                        int num2;
                        NetworkTransport.GetBroadcastConnectionMessage(this._clientHostID, this._recvBuffer, this._recvBuffer.Length, out this._recvDataSize, out this._recvErr);
                        this.CheckNetError(this._recvErr, NetworkEventType.Nothing);
                        NetworkTransport.GetBroadcastConnectionInfo(this._clientHostID, out str, out num, out this._recvErr);
                        this.CheckNetError(this._recvErr, NetworkEventType.Nothing);
                        this.ParseServerTuple(this._recvBuffer, this._recvDataSize, out str2, out num2);
                        if (str2 == "<ANY>")
                        {
                            str2 = str;
                        }
                        if (this.onClientDiscoveredServer != null)
                        {
                            this.onClientDiscoveredServer(str2, num2);
                        }
                    }
                }
            }
        }

        public void Init()
        {
            if (this._setupTeardownTransport)
            {
                NetworkTransport.Init();
            }
            this._serverHostID = -1;
            this._clientHostID = -1;
        }

        private void ParseServerTuple(byte[] buffer, int len, out string ip, out int port)
        {
            char[] separator = new char[] { '_' };
            string[] strArray = Encoding.ASCII.GetString(buffer, 0, len).Split(separator);
            ip = strArray[0];
            port = int.Parse(strArray[1]);
        }

        public void ServerStartBroadcast(int targetPort, string targetIp = null)
        {
            byte num;
            this._serverHostID = NetworkTransport.AddHost(this._ht, 0);
            this.CheckHostID(this._serverHostID);
            byte[] buffer = this.ServerTupleToByte(targetIp, targetPort);
            bool flag = NetworkTransport.StartBroadcastDiscovery(this._serverHostID, 0xbaa1, this._key, this._version, this._subVersion, buffer, buffer.Length, this._interval, out num);
            this.CheckNetError(num, NetworkEventType.Nothing);
            this.isServerBroadcasting = true;
        }

        public void ServerStopBroadcast()
        {
            NetworkTransport.StopBroadcastDiscovery();
            NetworkTransport.RemoveHost(this._serverHostID);
            this._serverHostID = -1;
            this.isServerBroadcasting = false;
        }

        private byte[] ServerTupleToByte(string ip, int port)
        {
            if (ip == null)
            {
                ip = "<ANY>";
            }
            string s = string.Format("{0}_{1}", ip, port);
            return Encoding.ASCII.GetBytes(s);
        }

        public void Shutdown()
        {
            if (this.isClientBroadcasting)
            {
                this.ClientStopListen();
            }
            if (this.isServerBroadcasting)
            {
                this.ServerStopBroadcast();
            }
            if (this._setupTeardownTransport)
            {
                NetworkTransport.Shutdown();
            }
        }

        public bool isClientBroadcasting { get; private set; }

        public bool isServerBroadcasting { get; private set; }

        public delegate void ClientDiscoveredServerDelegate(string serverIP, int serverPort);
    }
}

