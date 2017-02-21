namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoClientPacketConsumer : MonoBehaviour
    {
        private MiClient _client;
        private GeneralDialogContext _errorDialogContext;
        private LoadingWheelWidgetContext _loadingWheelDialogContext;
        private int _reconnectTimes;
        public Status _status;
        private float _timer;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cacheC;
        public uint clientPacketId;
        public string host;
        public uint lastServerPacketId;
        public NetworkReachability netReach;
        public bool netReachAlreadyInit;
        public ushort port;
        private const float RECONNECT_INTERVAL = 5f;

        private void Awake()
        {
            UnityEngine.Object.DontDestroyOnLoad(this);
        }

        private bool DispatchPacket(NetPacketV1 pkt)
        {
            if (pkt.getTime() > 0)
            {
                this.lastServerPacketId = pkt.getTime();
            }
            return Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.NetwrokPacket, pkt));
        }

        public void Init(MiClient client)
        {
            this._client = client;
        }

        public void OnApplicationQuit()
        {
            this._client.disconnect();
        }

        private void OnConnectNormal()
        {
            if (this._status != Status.Normal)
            {
                this._timer = 0f;
                this._reconnectTimes = 0;
                this._status = Status.Normal;
                if (this.ShouldShowLoadingWheelWhenDisconnect())
                {
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnSocketConnect, null));
                }
            }
            if ((this._loadingWheelDialogContext != null) && (this._loadingWheelDialogContext.view != null))
            {
                this._loadingWheelDialogContext.Finish();
            }
        }

        private void Reconnect()
        {
            this._reconnectTimes++;
            this._timer = 0f;
            this._status = Status.WaitingConnect;
            Singleton<NetworkManager>.Instance.QuickLogin();
        }

        public void SetRepeatLogin()
        {
            this._status = Status.RepeatLogin;
        }

        private bool ShouldShowLoadingWheelWhenDisconnect()
        {
            return (!(Singleton<MainUIManager>.Instance.SceneCanvas is MonoInLevelUICanvas) || MiscData.Config.BasicConfig.ShouldShowLoadingWheelWhenDisconnectInLevel);
        }

        private void ShowLoadingWheel()
        {
            if (this.ShouldShowLoadingWheelWhenDisconnect())
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnSocketDisconnect, null));
                Singleton<MainUIManager>.Instance.ShowWidget(this._loadingWheelDialogContext, UIType.Any);
            }
        }

        private void Start()
        {
            this.host = this._client.Host;
            this.port = this._client.Port;
            this._loadingWheelDialogContext = new LoadingWheelWidgetContext();
            this._status = Status.Normal;
        }

        public void TryShowErrorDialog()
        {
            if ((this._errorDialogContext == null) || (this._errorDialogContext.view == null))
            {
                GeneralDialogContext context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Err_PlayerRepeatLogin", new object[0]),
                    notDestroyAfterTouchBG = true,
                    hideCloseBtn = true
                };
                if (<>f__am$cacheC == null)
                {
                    <>f__am$cacheC = confirmed => GeneralLogicManager.RestartGame();
                }
                context.buttonCallBack = <>f__am$cacheC;
                this._errorDialogContext = context;
                Singleton<MainUIManager>.Instance.ShowDialog(this._errorDialogContext, UIType.Any);
            }
        }

        private void Update()
        {
            NetPacketV1 pkt = this._client.recv(0);
            if (pkt != null)
            {
                this.OnConnectNormal();
                if (Singleton<CommandMap>.Instance.GetTypeByCmdID(pkt.getCmdId()) != null)
                {
                    this.DispatchPacket(pkt);
                }
            }
            else if (Singleton<NetworkManager>.Instance.alreadyLogin)
            {
                NetworkReachability internetReachability = Application.internetReachability;
                if (!this.netReachAlreadyInit)
                {
                    this.netReachAlreadyInit = true;
                    this.netReach = internetReachability;
                }
                else if (this.netReach != internetReachability)
                {
                    this.netReach = internetReachability;
                    if (this._client.isConnected())
                    {
                        this._client.disconnect();
                    }
                }
                if (!this._client.isConnected() && ((Singleton<MainUIManager>.Instance != null) && (Singleton<MainUIManager>.Instance.SceneCanvas != null)))
                {
                    if (this._status == Status.Normal)
                    {
                        this._status = Status.WaitingConnect;
                        this.Reconnect();
                    }
                    else if (this._status == Status.WaitingConnect)
                    {
                        this._timer += Time.deltaTime;
                        if (this._timer > 5f)
                        {
                            this.ShowLoadingWheel();
                            this.Reconnect();
                        }
                    }
                    else if (this._status == Status.RepeatLogin)
                    {
                        this.TryShowErrorDialog();
                    }
                }
            }
        }

        public enum Status
        {
            Normal,
            WaitingConnect,
            RepeatLogin
        }
    }
}

