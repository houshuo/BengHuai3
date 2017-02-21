namespace MoleMole
{
    using RenderHeads.Media.AVProVideo;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Events;

    public class MonoGameEntry : BaseMonoCanvas
    {
        private ElevatorModelContext _elevatorModelContext;
        private GameEntryPageContext _pageContext;
        private float _retryAssetBundleWaitTimer;
        private SpaceShipModelContext _spaceshipModelContext;
        private bool _startFirstLevel;
        private Status _status;
        private DisplayIMGUI _videoDisplay;
        private MediaPlayer _videoPlayer;
        private Coroutine _waitAvatarModelCoroutine;
        private Coroutine _waitBeforeLoginCoroutine;
        private Coroutine _waitLoadSpaceshipCoroutine;
        private Coroutine _waitSplashFadeoutCoroutine;
        [CompilerGenerated]
        private static Action <>f__am$cache11;
        public int assetbundleRetryTimes;
        public Avatar3dModelContext avatar3dModelContext;
        public GameObject GameEntryPage;
        public const int MAX_ASSET_BUNDLE_RETRY_TIMES = 4;
        public const float RETRY_ASSET_BUNDLE_WAIT_SECONDS = 2f;

        private void AfterDataAssetReady()
        {
            if (GlobalVars.ResourceUseAssetBundle)
            {
                this.CheckAndLoadEventAsset();
            }
            else
            {
                this.OnEventAssetReady();
            }
        }

        private void AudioVolumeParamSetting(float vol)
        {
            Singleton<WwiseAudioManager>.Instance.SetParam("Vol_BGM", vol);
            Singleton<WwiseAudioManager>.Instance.SetParam("Vol_SE", vol);
            Singleton<WwiseAudioManager>.Instance.SetParam("Vol_Voice", vol);
        }

        public void Awake()
        {
            GraphicsSettingUtil.SetTargetFrameRate(60);
            GeneralLogicManager.InitOnGameStart();
            Singleton<MainUIManager>.Instance.SetMainCanvas(this);
            this.assetbundleRetryTimes = 0;
            this._status = Status.Default;
            this._retryAssetBundleWaitTimer = 0f;
            this._videoPlayer = base.transform.Find("Video/VideoPlayer").GetComponent<MediaPlayer>();
            this._videoPlayer.Events.AddListener(new UnityAction<MediaPlayer, MediaPlayerEvent.EventType, ErrorCode>(this.OnVideoEvent));
            this._videoPlayer.gameObject.SetActive(false);
            this._videoDisplay = base.transform.Find("Video/VideoDisplay").GetComponent<DisplayIMGUI>();
            this._videoDisplay.gameObject.SetActive(false);
            base.transform.Find("Video/BlackPanel").gameObject.SetActive(false);
            base.transform.Find("Video").gameObject.SetActive(false);
        }

        private void CheckAndLoadDataAsset()
        {
            Singleton<AssetBundleManager>.Instance.Loader.LoadVersionFile(BundleType.DATA_FILE, new Action<long, long, long, float>(this.OnLoadingDataVersionProgress), delegate (bool success) {
                if (success)
                {
                    base.StartCoroutine(Singleton<AssetBundleManager>.Instance.Loader.StartDownloadAssetBundle(BundleType.DATA_FILE, new Action<long, long, long, float>(this.OnLoadingDataAssetBundleProgress), null, new Action<bool>(this.OnLoadingDataAssetBundleComplete)));
                }
                else
                {
                    this.OnLoadingDataAssetBundleComplete(false);
                }
            });
        }

        private void CheckAndLoadEventAsset()
        {
            Singleton<AssetBundleManager>.Instance.Loader.LoadVersionFile(BundleType.RESOURCE_FILE, new Action<long, long, long, float>(this.OnLoadingEventVersionProgress), delegate (bool success) {
                if (success)
                {
                    base.StartCoroutine(Singleton<AssetBundleManager>.Instance.Loader.StartDownloadAssetBundle(BundleType.RESOURCE_FILE, new Action<long, long, long, float>(this.OnLoadingEventAssetBundleProgress), null, new Action<bool>(this.OnLoadingEventAssetBundleComplete)));
                }
                else
                {
                    this.OnLoadingEventAssetBundleComplete(false);
                }
            });
        }

        public void ConnectDispatch()
        {
            base.StartCoroutine(Singleton<NetworkManager>.Instance.ConnectDispatchServer(new Action(this.DispatchConnectCallback)));
        }

        public void ConnentGlobalDispatch()
        {
            base.StartCoroutine(Singleton<NetworkManager>.Instance.ConnectGlobalDispatchServer(new Action(this.ConnectDispatch)));
        }

        private void DisableVideo()
        {
            this._videoPlayer.Events.RemoveAllListeners();
            this._videoPlayer.gameObject.SetActive(false);
            this._videoDisplay.gameObject.SetActive(false);
            base.transform.Find("Video").gameObject.SetActive(false);
        }

        private void DispatchConnectCallback()
        {
            Singleton<AccountManager>.Instance.manager.SetupByDispatchServerData();
            if (GlobalVars.DataUseAssetBundle)
            {
                this.CheckAndLoadDataAsset();
            }
            else
            {
                this.OnDataAssetReady();
            }
            if (Singleton<NetworkManager>.Instance.DispatchSeverData.showVersionText)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowVersionText, null));
            }
        }

        public void OnCameraBeforeEnterSpaceshipAnimOver()
        {
            this._elevatorModelContext.SetDescImage(ElevatorModelContext.DescImageType.Confirmed);
            this._elevatorModelContext.PlayDoorAnimation();
            Singleton<WwiseAudioManager>.Instance.Post("GameEntry_Elevator_Door_Open", null, null, null);
        }

        public void OnCameraEnterSpaceshipAnimEvent(int phase)
        {
            if (phase == 1)
            {
                Singleton<WwiseAudioManager>.Instance.Post("VO_M_Con_12_CPT_On_Bridge", null, null, null);
            }
            if (phase == 2)
            {
                this.TriggerAvatarModelTurnAround();
            }
            else if (phase == 3)
            {
                Singleton<WwiseAudioManager>.Instance.ClearManualPrepareBank();
                Singleton<MainMenuBGM>.Instance.TryEnterMainMenu();
                Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithoutSpaceship", false, false, true, null, true);
            }
        }

        private void OnDataAssetReady()
        {
            GeneralLogicManager.InitOnDataAssetReady(true, new Action(this.AfterDataAssetReady));
        }

        public override void OnDestroy()
        {
            this.StopWaitLoadSpaceship();
            this.StopWaitAvatarModel();
            this.StopWaitSplashFadeout();
            this.StopWaitBeforeLogin();
            if (this._startFirstLevel)
            {
                if ((this._spaceshipModelContext != null) && (this._spaceshipModelContext.view != null))
                {
                    UnityEngine.Object.Destroy(this._spaceshipModelContext.view);
                }
                if ((this.avatar3dModelContext != null) && (this.avatar3dModelContext.view != null))
                {
                    UnityEngine.Object.Destroy(this.avatar3dModelContext.view);
                }
            }
            base.OnDestroy();
        }

        public void OnElevatorDoorAnimOver()
        {
            Camera.main.gameObject.GetComponent<Animation>().Play("EnterSpaceship");
            this._elevatorModelContext.PlayBackAnimation();
        }

        public void OnElevatorFloorAnimEvent(int phase)
        {
            if (phase == 1)
            {
                GraphicsSettingUtil.EnableUIAvatarsDynamicBone(false);
            }
        }

        public void OnElevatorFloorPhase1AnimOver()
        {
            this._elevatorModelContext.HideSomeParts();
            this.spaceshipGO.transform.Find("Warship").GetComponent<Animation>().Play("WarshipFall");
            this._elevatorModelContext.PlayFloorPhase2Animation();
            this.SetUIAvatarStandOnSpaceship();
        }

        public void OnElevatorFloorPhase2AnimOver()
        {
            GraphicsSettingUtil.EnableUIAvatarsDynamicBone(true);
            Camera.main.gameObject.GetComponent<Animation>().Play("BeforeEnterSpaceship");
        }

        private void OnEventAssetReady()
        {
            Singleton<AssetBundleManager>.Instance.UpdateEventSVNVersion(null);
            this._waitBeforeLoginCoroutine = base.StartCoroutine(this.WaitBeforeLogin());
        }

        private void OnLoadingDataAssetBundleComplete(bool result)
        {
            if (result)
            {
                this.assetbundleRetryTimes = 0;
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowLoadAssetText, false));
                this.OnDataAssetReady();
            }
            else
            {
                this.TriggerRetryCheckAndLoadDataAsset();
            }
        }

        private void OnLoadingDataAssetBundleProgress(long current, long total, long delta, float speed)
        {
            float num = ((float) current) / ((float) total);
            string text = LocalizationGeneralLogic.GetText("Menu_DownloadDataAsset", new object[0]);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, text, true, num)));
        }

        private void OnLoadingDataVersionProgress(long current, long total, long delta, float speed)
        {
            string text = LocalizationGeneralLogic.GetText("Menu_CheckDataAsset", new object[0]);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, text, false, 0f)));
        }

        private void OnLoadingEventAssetBundleComplete(bool result)
        {
            if (result)
            {
                this.assetbundleRetryTimes = 0;
                this._status = Status.Default;
                this._retryAssetBundleWaitTimer = 0f;
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowLoadAssetText, false));
                this.OnEventAssetReady();
            }
            else
            {
                this.TriggerRetryCheckAndLoadEventAsset();
            }
        }

        private void OnLoadingEventAssetBundleProgress(long current, long total, long delta, float speed)
        {
            float num = ((float) current) / ((float) total);
            string text = LocalizationGeneralLogic.GetText("Menu_DownloadEventAsset", new object[0]);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, text, true, num)));
        }

        private void OnLoadingEventVersionProgress(long current, long total, long delta, float speed)
        {
            string text = LocalizationGeneralLogic.GetText("Menu_CheckEventAsset", new object[0]);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, text, false, 0f)));
        }

        public void OnPlayerLogin(bool isFirstTime)
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(false, string.Empty, false, 0f)));
            UIUtil.SpaceshipCheckWeather();
            Singleton<IslandModule>.Instance.InitTechTree();
            if (isFirstTime)
            {
                this.PlayVideo();
            }
            else
            {
                this._waitAvatarModelCoroutine = base.StartCoroutine(this.WaitCreateAvatarModel());
                this.DisableVideo();
            }
            if (Singleton<RealTimeWeatherManager>.Instance.Available)
            {
                this.ProcessRealtimeWeatherUpdate();
            }
            Singleton<MiHoYoGameData>.Instance.GeneralLocalData.ReportUIStatistics();
        }

        public void OnRestartGame()
        {
            this.StopWaitLoadSpaceship();
            this.StopWaitAvatarModel();
            this.StopWaitSplashFadeout();
            this.StopWaitBeforeLogin();
            if ((this._spaceshipModelContext != null) && (this._spaceshipModelContext.view != null))
            {
                UnityEngine.Object.Destroy(this._spaceshipModelContext.view);
            }
            if ((this.avatar3dModelContext != null) && (this.avatar3dModelContext.view != null))
            {
                UnityEngine.Object.Destroy(this.avatar3dModelContext.view);
            }
        }

        private void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode ec)
        {
            switch (et)
            {
                case MediaPlayerEvent.EventType.ReadyToPlay:
                    this.OnVideoReady();
                    break;

                case MediaPlayerEvent.EventType.FirstFrameReady:
                    this.OnVideoStarted();
                    break;

                case MediaPlayerEvent.EventType.FinishedPlaying:
                case MediaPlayerEvent.EventType.Error:
                    this.OnVideoFinished();
                    break;
            }
        }

        private void OnVideoFinished()
        {
            try
            {
                this.AudioVolumeParamSetting(3f);
                this.DisableVideo();
                Screen.sleepTimeout = -2;
            }
            catch (Exception exception)
            {
                SuperDebug.VeryImportantError(exception.ToString());
            }
            finally
            {
                this.StartFirstLevel();
            }
        }

        private void OnVideoReady()
        {
            try
            {
                this.AudioVolumeParamSetting(0f);
                Singleton<WwiseAudioManager>.Instance.Post("GameEntry_Elevator_End", null, null, null);
                this._videoPlayer.Play();
                Singleton<MainUIManager>.Instance.LockUI(true, float.MaxValue);
                Screen.sleepTimeout = -1;
            }
            catch (Exception exception)
            {
                SuperDebug.VeryImportantError(exception.ToString());
                this.OnVideoFinished();
            }
        }

        private void OnVideoStarted()
        {
            this._videoDisplay.gameObject.SetActive(true);
            base.transform.Find("Video/BlackPanel").gameObject.SetActive(true);
        }

        private void PlayVideo()
        {
            this._pageContext.FadeOutVideo();
            base.transform.Find("Video").gameObject.SetActive(true);
            this._videoPlayer.gameObject.SetActive(true);
            try
            {
                CgDataItem item = Singleton<CGModule>.Instance.GetCgDataItemList()[0];
                string path = string.Format("Video/{0}.mp4", item.cgPath);
                if (this._videoPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, path, false))
                {
                    if (<>f__am$cache11 == null)
                    {
                        <>f__am$cache11 = () => Singleton<MainUIManager>.Instance.LockUI(true, float.MaxValue);
                    }
                    Singleton<ApplicationManager>.Instance.InvokeNextFrame(<>f__am$cache11);
                }
                else
                {
                    SuperDebug.VeryImportantError("open video failed! path=" + path);
                    this.OnVideoFinished();
                }
            }
            catch (Exception exception)
            {
                SuperDebug.VeryImportantError(exception.ToString());
                this.OnVideoFinished();
            }
        }

        private void PostStartHandleBenchmark()
        {
            if (GlobalVars.IS_BENCHMARK)
            {
                Screen.sleepTimeout = -1;
                SuperDebug.CloseAllDebugs();
                GameObject target = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(target);
                target.name = "__Benchmark";
                target.AddComponent<MonoBenchmarkSwitches>();
            }
        }

        private void ProcessRealtimeWeatherUpdate()
        {
            if (Singleton<RealTimeWeatherManager>.Instance.IsWeatherInfoExpired())
            {
                Singleton<RealTimeWeatherManager>.Instance.QueryWeatherInfo(null);
            }
        }

        private void RetryCheckAndLoadDataAsset()
        {
            this.assetbundleRetryTimes++;
            if (this.assetbundleRetryTimes > 4)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_NetError", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_DownloadDataAssetErr", new object[0]),
                    notDestroyAfterTouchBG = true,
                    buttonCallBack = delegate (bool confirmed) {
                        this.CheckAndLoadDataAsset();
                        this.TriggerRetryCheckAndLoadDataAsset();
                    }
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                this.CheckAndLoadDataAsset();
            }
        }

        private void RetryCheckAndLoadEventAsset()
        {
            this.assetbundleRetryTimes++;
            if (this.assetbundleRetryTimes > 4)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_NetError", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_DownloadEventAssetErr", new object[0]),
                    notDestroyAfterTouchBG = true,
                    buttonCallBack = delegate (bool confirmed) {
                        this.CheckAndLoadEventAsset();
                        this.TriggerRetryCheckAndLoadEventAsset();
                    }
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                this.CheckAndLoadEventAsset();
            }
        }

        private void SetUIAvatarStandOnSpaceship()
        {
            int readyToTouchAvatarID = Singleton<GalTouchModule>.Instance.GetReadyToTouchAvatarID();
            AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(readyToTouchAvatarID);
            this.avatar3dModelContext.SetStandOnSpaceship(avatarByID.avatarID);
        }

        public override void Start()
        {
            GameObject view = GameObject.Find("StartLoading_Model");
            this._elevatorModelContext = new ElevatorModelContext(view);
            Singleton<MainUIManager>.Instance.ShowWidget(this._elevatorModelContext, UIType.Any);
            this.avatar3dModelContext = new Avatar3dModelContext(null);
            Singleton<MainUIManager>.Instance.ShowWidget(this.avatar3dModelContext, UIType.Root);
            this._pageContext = new GameEntryPageContext(this.GameEntryPage);
            Singleton<MainUIManager>.Instance.ShowPage(this._pageContext, UIType.Page);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.FadeOutGameEntrySplash, null));
            string[] soundBankNames = new string[] { "All_In_One_Bank", "BK_Global", "BK_Events" };
            Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(soundBankNames);
            Singleton<WwiseAudioManager>.Instance.ManualPrepareBank("BK_GameEntry");
            Singleton<WwiseAudioManager>.Instance.Post("GameEntry_Elevator_Start_Alarm", null, null, null);
            string text = LocalizationGeneralLogic.GetText("Menu_ConnectGlobalDispatch", new object[0]);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, text, false, 0f)));
            this._waitSplashFadeoutCoroutine = base.StartCoroutine(this.WaitSplashFadeout());
            this.PostStartHandleBenchmark();
            AudioSettingData.ApplySettingConfig();
            if (Singleton<EffectManager>.Instance == null)
            {
                Singleton<EffectManager>.Create();
                Singleton<EffectManager>.Instance.InitAtAwake();
                Singleton<EffectManager>.Instance.InitAtStart();
            }
            base.Start();
        }

        private void StartFirstLevel()
        {
            try
            {
                int cgID = Singleton<CGModule>.Instance.GetCgDataItemList()[0].cgID;
                Singleton<CGModule>.Instance.MarkCGIDFinish(cgID);
            }
            catch (Exception exception)
            {
                SuperDebug.VeryImportantError(exception.ToString());
            }
            this._startFirstLevel = this._pageContext.StartFirstLevel();
            if (!this._startFirstLevel)
            {
                this.DisableVideo();
                this._pageContext.DisableVideoFadeOut();
                this._waitAvatarModelCoroutine = base.StartCoroutine(this.WaitCreateAvatarModel());
            }
        }

        private void StopWaitAvatarModel()
        {
            if (this._waitAvatarModelCoroutine != null)
            {
                base.StopCoroutine(this._waitAvatarModelCoroutine);
                this._waitAvatarModelCoroutine = null;
            }
        }

        private void StopWaitBeforeLogin()
        {
            if (this._waitBeforeLoginCoroutine != null)
            {
                base.StopCoroutine(this._waitBeforeLoginCoroutine);
                this._waitBeforeLoginCoroutine = null;
            }
        }

        private void StopWaitLoadSpaceship()
        {
            if (this._waitLoadSpaceshipCoroutine != null)
            {
                base.StopCoroutine(this._waitLoadSpaceshipCoroutine);
                this._waitLoadSpaceshipCoroutine = null;
            }
        }

        private void StopWaitSplashFadeout()
        {
            if (this._waitSplashFadeoutCoroutine != null)
            {
                base.StopCoroutine(this._waitSplashFadeoutCoroutine);
                this._waitSplashFadeoutCoroutine = null;
            }
        }

        private void TriggerAvatarModelTurnAround()
        {
            int readyToTouchAvatarID = Singleton<GalTouchModule>.Instance.GetReadyToTouchAvatarID();
            AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(readyToTouchAvatarID);
            this.avatar3dModelContext.TriggerAvatarTurnAround(avatarByID.avatarID);
        }

        private void TriggerRetryCheckAndLoadDataAsset()
        {
            this._status = Status.WaitingRetryLoadDataAsset;
            this._retryAssetBundleWaitTimer = 0f;
        }

        private void TriggerRetryCheckAndLoadEventAsset()
        {
            this._status = Status.WaitingRetryLoadEventAsset;
            this._retryAssetBundleWaitTimer = 0f;
        }

        public override void Update()
        {
            base.Update();
            if (this._status == Status.WaitingRetryLoadDataAsset)
            {
                this._retryAssetBundleWaitTimer += Time.fixedDeltaTime;
                if (this._retryAssetBundleWaitTimer >= 2f)
                {
                    this._status = Status.Default;
                    this._retryAssetBundleWaitTimer = 0f;
                    this.RetryCheckAndLoadDataAsset();
                }
            }
            else if (this._status == Status.WaitingRetryLoadEventAsset)
            {
                this._retryAssetBundleWaitTimer += Time.fixedDeltaTime;
                if (this._retryAssetBundleWaitTimer >= 2f)
                {
                    this._status = Status.Default;
                    this._retryAssetBundleWaitTimer = 0f;
                    this.RetryCheckAndLoadEventAsset();
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator WaitBeforeLogin()
        {
            return new <WaitBeforeLogin>c__Iterator52 { <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator WaitCreateAvatarModel()
        {
            return new <WaitCreateAvatarModel>c__Iterator54 { <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator WaitLoadSpaceShip()
        {
            return new <WaitLoadSpaceShip>c__Iterator53 { <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator WaitSplashFadeout()
        {
            return new <WaitSplashFadeout>c__Iterator51 { <>f__this = this };
        }

        public GameObject spaceshipGO { get; set; }

        public float warshipDefaultYPos { get; set; }

        [CompilerGenerated]
        private sealed class <WaitBeforeLogin>c__Iterator52 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoGameEntry <>f__this;
            internal bool <hasLastLoginUser>__0;
            internal bool <isLoginByAccount>__1;
            internal string <text>__2;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this._waitLoadSpaceshipCoroutine = this.<>f__this.StartCoroutine(this.<>f__this.WaitLoadSpaceShip());
                        this.$current = this.<>f__this._waitLoadSpaceshipCoroutine;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<hasLastLoginUser>__0 = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.LastLoginUserId != 0;
                        this.<isLoginByAccount>__1 = !string.IsNullOrEmpty(Singleton<AccountManager>.Instance.manager.AccountUid);
                        this.<text>__2 = (this.<hasLastLoginUser>__0 || this.<isLoginByAccount>__1) ? "ENTRY_PREPARED" : "ENTRY_PREPARED_LOGIN";
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, LocalizationGeneralLogic.GetText(this.<text>__2, new object[0]), true, 1f)));
                        this.<>f__this._elevatorModelContext.SetDescImage(ElevatorModelContext.DescImageType.Identifying);
                        Singleton<AccountManager>.Instance.manager.LoginUI();
                        this.<>f__this._waitBeforeLoginCoroutine = null;
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitCreateAvatarModel>c__Iterator54 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoGameEntry <>f__this;
            internal List<Transform> <avatars>__0;
            internal float <timer>__1;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<avatars>__0 = this.<>f__this.avatar3dModelContext.GetAllAvatars();
                        break;

                    case 1:
                        this.<avatars>__0 = this.<>f__this.avatar3dModelContext.GetAllAvatars();
                        break;

                    case 2:
                        goto Label_009C;

                    default:
                        goto Label_0121;
                }
                if (this.<avatars>__0.Count == 0)
                {
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_0123;
                }
                this.<timer>__1 = 0f;
                while ((Singleton<MissionModule>.Instance == null) || !Singleton<MissionModule>.Instance.missionDataReceived)
                {
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_0123;
                Label_009C:
                    this.<timer>__1 += Time.deltaTime;
                    if (this.<timer>__1 > 3f)
                    {
                        Singleton<NetworkManager>.Instance.RequestGetMissionData();
                        this.<timer>__1 = 0f;
                    }
                }
                this.<>f__this._elevatorModelContext.PlayFloorPhase1Animation();
                Singleton<WwiseAudioManager>.Instance.Post("GameEntry_Elevator_End", null, null, null);
                this.<>f__this._waitAvatarModelCoroutine = null;
                this.$PC = -1;
            Label_0121:
                return false;
            Label_0123:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitLoadSpaceShip>c__Iterator53 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoGameEntry <>f__this;
            internal Camera <mainCamera>__1;
            internal AsyncAssetRequst <resReq>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<resReq>__0 = Miscs.LoadResourceAsync("Stage/MainMenu_SpaceShip/MainMenu_SpaceShip", BundleType.RESOURCE_FILE);
                        this.$current = this.<resReq>__0.operation;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.spaceshipGO = UnityEngine.Object.Instantiate<GameObject>((GameObject) this.<resReq>__0.asset);
                        this.<>f__this.spaceshipGO.name = "MainMenu_SpaceShip";
                        this.<>f__this.warshipDefaultYPos = this.<>f__this.spaceshipGO.transform.Find("Warship").position.y;
                        this.<>f__this.spaceshipGO.transform.Find("Warship").localPosition = new Vector3(0f, 10f, 0f);
                        UnityEngine.Object.DontDestroyOnLoad(this.<>f__this.spaceshipGO);
                        this.<mainCamera>__1 = Camera.main;
                        this.<>f__this._spaceshipModelContext = new SpaceShipModelContext(this.<>f__this.spaceshipGO, this.<mainCamera>__1.gameObject);
                        Singleton<MainUIManager>.Instance.ShowWidget(this.<>f__this._spaceshipModelContext, UIType.Any);
                        this.<>f__this._waitLoadSpaceshipCoroutine = null;
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitSplashFadeout>c__Iterator51 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoGameEntry <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                    case 1:
                        if (!this.<>f__this._pageContext.IsSplashFadeOut)
                        {
                            this.$current = null;
                            this.$PC = 1;
                            return true;
                        }
                        this.<>f__this._waitSplashFadeoutCoroutine = null;
                        if (!GlobalVars.DISABLE_NETWORK_DEBUG)
                        {
                            this.<>f__this.ConnentGlobalDispatch();
                        }
                        else
                        {
                            FakePacketHelper.FakeConnectDispatch();
                            this.<>f__this.OnDataAssetReady();
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        private enum Status
        {
            Default,
            WaitingRetryLoadDataAsset,
            WaitingRetryLoadEventAsset
        }
    }
}

