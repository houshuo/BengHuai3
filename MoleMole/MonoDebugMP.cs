namespace MoleMole
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using UnityEngine;

    public class MonoDebugMP : MonoBehaviour
    {
        private int _connectedPeerCount;
        private UNetDiscovery _discovery;
        private ulong _lastRecvCount;
        private ulong _lastRecvCountWindowed;
        private float _lastRecvRate;
        private ulong _lastRecvTotal;
        private ulong _lastSendCount;
        private ulong _lastSendCountWindowed;
        private float _lastSendRate;
        private ulong _lastSendTotal;
        private MPPeer _peer;
        private State _state;
        private float _statTimer;
        private GUIStyle _style;
        private const int DEBUG_MP_BOX_HEIGHT = 120;
        private const int DEBUG_MP_BOX_WIDTH = 380;
        [Header("Discovery Key")]
        public int DiscoveryKey = 0x91d;
        [Header("Discovery Sub Version")]
        public int DiscoverySubVersion = 1;
        [Header("Discovery Version")]
        public int DiscoveryVersion = 1;
        [Header("Server Listen Port")]
        public int gameServerPort = 0xd1d0;
        [Header("uNet simulator, doesn't really work very well really")]
        public bool isDelaySimulator;
        public Action<MPPeer> onPeerReady;
        [Header("Which net lib to use")]
        public MPPeerType peerType;
        private const float STAT_WINDOW = 0.5f;
        [Header("Server wait for n player to start")]
        public int WaitForPlayerCount = 2;

        private float AvgWithLast(float lhs, float rhs)
        {
            return ((lhs + rhs) / 2f);
        }

        private void Awake()
        {
            this._style = new GUIStyle();
            this._style.margin = new RectOffset();
            this._style.padding = new RectOffset();
            this._style.normal.textColor = Color.magenta;
            this._style.hover.textColor = Color.magenta;
            this._style.focused.textColor = Color.magenta;
            this._style.active.textColor = Color.magenta;
            this._style.onNormal.textColor = Color.magenta;
            this._style.onHover.textColor = Color.magenta;
            this._style.onFocused.textColor = Color.magenta;
            this._style.onActive.textColor = Color.magenta;
            Texture2D textured = new Texture2D(4, 4);
            Color[] colors = new Color[0x10];
            Color black = Color.black;
            black.a = 0.5f;
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = black;
            }
            textured.SetPixels(colors);
            textured.Apply();
            this._style.normal.background = textured;
            if (this.peerType == MPPeerType.uNet)
            {
                this._discovery = new UNetDiscovery(this.DiscoveryKey, this.DiscoveryVersion, this.DiscoverySubVersion, 0x3e8, false);
                this._peer = new UNetMPPeer(this.isDelaySimulator);
            }
            else if (this.peerType == MPPeerType.Lidgren)
            {
                this._discovery = new UNetDiscovery(this.DiscoveryKey, this.DiscoveryVersion, this.DiscoverySubVersion, 0x3e8, true);
                this._peer = new LidgrenMPPeer();
            }
            this._peer.Init();
            this._discovery.Init();
            this._peer.onConnected = new MPPeer.ConnectedHandler(this.OnClientConnected);
            this._discovery.onClientDiscoveredServer = new UNetDiscovery.ClientDiscoveredServerDelegate(this.OnClientFoundServer);
            this._peer.onEstablished = new MPPeer.EstablishedHandler(this.OnEstablished);
            this._connectedPeerCount = 0;
        }

        private void DrawAndCalcStats()
        {
            ulong num;
            ulong num2;
            ulong num3;
            ulong num4;
            string str;
            this._peer.GetPeerStats(out num, out num2, out num3, out num4);
            object[] args = new object[] { num / ((ulong) 0x3e8L), num2 / ((ulong) 0x3e8L), num3, num4 };
            GUILayout.Label(string.Format("s(kb):{0,10}, r(kb):{1,10}, s(cnt):{2,10}, r(cnt):{3,10}", args), this._style, new GUILayoutOption[0]);
            object[] objArray2 = new object[] { this._lastSendRate, this._lastRecvRate, this._lastSendCountWindowed, this._lastRecvCountWindowed };
            GUILayout.Label(string.Format("s(kb/m):{0,5:00.00},r(kb/m):{1,5:00.00},s(cnt/m):{2,5}, r(cnt/m):{3,5}", objArray2), this._style, new GUILayoutOption[0]);
            this._peer.GetPeerStats2(out str);
            GUILayout.Label(str, new GUILayoutOption[0]);
            this._statTimer += Time.unscaledDeltaTime;
            if (this._statTimer > 0.5f)
            {
                float num5 = num - this._lastSendTotal;
                float num6 = num2 - this._lastRecvTotal;
                float num7 = num3 - this._lastSendCount;
                float num8 = num4 - this._lastRecvCount;
                this._lastSendRate = this.AvgWithLast(this._lastSendRate, (num5 / this._statTimer) * 0.06f);
                this._lastRecvRate = this.AvgWithLast(this._lastRecvRate, (num6 / this._statTimer) * 0.06f);
                this._lastSendCountWindowed = (ulong) this.AvgWithLast((float) this._lastSendCountWindowed, (num7 / this._statTimer) * 60f);
                this._lastRecvCountWindowed = (ulong) this.AvgWithLast((float) this._lastRecvCountWindowed, (num8 / this._statTimer) * 60f);
                PlotManager.Instance.PlotAdd("SendByteRate", this._lastSendRate);
                PlotManager.Instance.PlotAdd("RecvByteRate", this._lastRecvRate);
                this._statTimer = 0f;
                this._lastSendTotal = num;
                this._lastRecvTotal = num2;
                this._lastSendCount = num3;
                this._lastRecvCount = num4;
            }
        }

        private void InitPlotting()
        {
            PlotManager.Instance.PlotCreate("SendByteRate", 0f, 150f, Color.red, Vector2.zero);
            PlotManager.Instance.PlotCreate("RecvByteRate", Color.green, "SendByteRate");
        }

        private string MapToIPV4Literal(string serverIP)
        {
            IPAddress address;
            IPAddress.TryParse(serverIP, out address);
            if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                byte[] addressBytes = address.GetAddressBytes();
                object[] args = new object[] { addressBytes[12], addressBytes[13], addressBytes[14], addressBytes[15] };
                return string.Format("{0}.{1}.{2}.{3}", args);
            }
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                return serverIP;
            }
            return null;
        }

        private void OnClientConnected(int peerID)
        {
            if (this._state == State.DiscoveryServer)
            {
                this._connectedPeerCount++;
                if ((this._connectedPeerCount + 1) == this.WaitForPlayerCount)
                {
                    if (this._discovery.isServerBroadcasting)
                    {
                        this._discovery.Shutdown();
                    }
                    this._peer.ServerReady();
                }
            }
            else if (this._state == State.DiscoveryClient)
            {
                this._state = State.StandByClient;
            }
        }

        private void OnClientFoundServer(string serverIP, int serverPort)
        {
            if (this._state == State.DiscoveryClient)
            {
                if (this.peerType == MPPeerType.Lidgren)
                {
                    serverIP = this.MapToIPV4Literal(serverIP);
                }
                this._peer.Connect(serverIP, serverPort);
                if (this._discovery.isClientBroadcasting)
                {
                    this._discovery.ClientStopListen();
                }
            }
        }

        private void OnDestroy()
        {
            this._discovery.Shutdown();
            this._peer.Shutdown();
            UnityEngine.Object.Destroy(this._style.normal.background);
        }

        private void OnEstablished()
        {
            this._state = State.Connected;
            this.InitPlotting();
            if (this.onPeerReady != null)
            {
                this.onPeerReady(this._peer);
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect((float) (Screen.width - 380), 0f, 380f, 120f), this._style);
            this._peer.OnGUI();
            if (this._state == State.Idle)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(30f) };
                if (GUILayout.Button("Start Discovery Server", options) && !this._discovery.isServerBroadcasting)
                {
                    this._peer.Listen(this.gameServerPort, null);
                    this._discovery.ServerStartBroadcast(this.gameServerPort, null);
                    this._state = State.DiscoveryServer;
                }
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Height(30f) };
                if (GUILayout.Button("Start Discovery Client", optionArray2) && !this._discovery.isClientBroadcasting)
                {
                    this._discovery.ClientStartListen();
                    this._state = State.DiscoveryClient;
                }
            }
            else if (this._state == State.DiscoveryServer)
            {
                GUILayout.Label("Discovery Server Broadcasting on port: " + this.gameServerPort, new GUILayoutOption[0]);
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Height(30f) };
                if (GUILayout.Button("Stop", optionArray3) && this._discovery.isServerBroadcasting)
                {
                    this._discovery.ServerStopBroadcast();
                    this._peer.StopListen();
                    this._state = State.Idle;
                }
            }
            else if (this._state == State.DiscoveryClient)
            {
                GUILayout.Label("Discovery Client Listenting on port: " + this.gameServerPort, new GUILayoutOption[0]);
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Height(30f) };
                if (GUILayout.Button("Stop", optionArray4))
                {
                    this._discovery.ClientStopListen();
                    this._state = State.Idle;
                }
            }
            else if (this._state == State.StandByClient)
            {
                GUILayout.Label("client standby, waiting for start.", new GUILayoutOption[0]);
            }
            else if (this._state == State.Connected)
            {
                this.DrawAndCalcStats();
            }
            GUILayout.EndArea();
        }

        private void Update()
        {
            this._discovery.Core();
            this._peer.Core();
        }

        private enum State
        {
            Idle,
            DiscoveryClient,
            DiscoveryServer,
            StandByClient,
            Connected
        }
    }
}

