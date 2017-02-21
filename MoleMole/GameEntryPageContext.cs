namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class GameEntryPageContext : BasePageContext
    {
        public bool _accountReady;
        private bool _agreementShowed;
        private bool _hasLogin;
        private LoadingWheelWidgetContext _loadingWheelDialogContext;
        private Coroutine _splashFadeOutCoroutine;
        private int _splashFadeOutFrameCount = 30;
        private int _transitBlackFadeOutFrameCount = 60;
        private int _transitBlackVideoFadeOutFrameCount = 15;
        [CompilerGenerated]
        private static Predicate<AvatarDataItem> <>f__am$cache9;

        public GameEntryPageContext(GameObject view)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "GameEntryContext"
            };
            base.config = pattern;
            base.showSpaceShip = true;
            base.view = view;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("DebugBtn").GetComponent<Button>(), new UnityAction(this.OnDebugBtnClick));
            base.BindViewCallback(base.view.transform.Find("FullScreenLogin"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("LoginPanel/EmptyUser/Normal/LoginBtn").GetComponent<Button>(), new UnityAction(this.OnLoginBtnClick));
            base.BindViewCallback(base.view.transform.Find("LoginPanel/EmptyUser/Normal/RigsterBtn").GetComponent<Button>(), new UnityAction(this.OnRigsterBtnClick));
            base.BindViewCallback(base.view.transform.Find("LoginPanel/EmptyUser/Normal/TryUserBtn").GetComponent<Button>(), new UnityAction(this.OnTryUserBtnClick));
            base.BindViewCallback(base.view.transform.Find("LoginPanel/LastUser/LogoutBtn").GetComponent<Button>(), new UnityAction(this.OnLogoutBtnClick));
            base.BindViewCallback(base.view.transform.Find("LoginPanel/EmptyUser/ForbidNewUser/LoginBtn").GetComponent<Button>(), new UnityAction(this.OnLoginBtnClick));
        }

        public override void Destroy()
        {
            this.StopSplashFadeOut();
            base.Destroy();
        }

        public void DisableVideoFadeOut()
        {
            base.view.transform.Find("TransitBlack").gameObject.SetActive(false);
        }

        private void DoBeginLevel(StageBeginRsp rsp)
        {
            if (Singleton<LevelScoreManager>.Instance == null)
            {
                Singleton<LevelScoreManager>.Create();
            }
            Singleton<LevelScoreManager>.Instance.collectAntiCheatData = rsp.get_is_collect_cheat_data();
            Singleton<LevelScoreManager>.Instance.signKey = rsp.get_sign_key();
            LevelDataItem level = Singleton<LevelModule>.Instance.TryGetLevelById(0x2775);
            Singleton<LevelScoreManager>.Instance.SetLevelBeginIntent(level, 0, rsp.get_drop_item_list(), level.BattleType, null);
            if (this._loadingWheelDialogContext != null)
            {
                this._loadingWheelDialogContext.Finish();
            }
            Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", false, true, true, null, false);
        }

        private bool FadeOutSplash()
        {
            this._splashFadeOutCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.FadeOutSplashImp());
            return false;
        }

        [DebuggerHidden]
        private IEnumerator FadeOutSplashImp()
        {
            return new <FadeOutSplashImp>c__Iterator62 { <>f__this = this };
        }

        public bool FadeOutVideo()
        {
            Singleton<ApplicationManager>.Instance.StartCoroutine(this.FadeOutVideoImp());
            return false;
        }

        [DebuggerHidden]
        private IEnumerator FadeOutVideoImp()
        {
            return new <FadeOutVideoImp>c__Iterator63 { <>f__this = this };
        }

        public void OnBGClick(BaseEventData data = null)
        {
            if (!this._hasLogin && this._accountReady)
            {
                Singleton<NetworkManager>.Instance.LoginGameServer();
            }
        }

        public void OnDebugBtnClick()
        {
            GlobalVars.DISABLE_NETWORK_DEBUG = true;
            SuperDebug.DEBUG_SWITCH[6] = true;
            FakePacketHelper.LoadFromFile();
            Singleton<NetworkManager>.Instance.LoginGameServer();
        }

        private bool OnGetAvatarReletedInfo()
        {
            this.SetupAvatar3dModel();
            return false;
        }

        private bool OnGetPlayerTokenRsp(GetPlayerTokenRsp rsp)
        {
            if (rsp.get_retcode() != null)
            {
                string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput, 2f), UIType.Any);
                if (rsp.get_retcode() == 3)
                {
                    Singleton<MiHoYoGameData>.Instance.GeneralLocalData.ClearLastLoginUser();
                    Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
                    Singleton<AccountManager>.Instance.manager.Reset();
                    this.ShowMihoyoLoginUI();
                }
            }
            return false;
        }

        public void OnLoginBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoLoginDialogContext(), UIType.Any);
        }

        public void OnLogoutBtnClick()
        {
            base.view.transform.Find("LoginPanel/LastUser").gameObject.SetActive(false);
            base.view.transform.Find("LoginPanel/EmptyUser").gameObject.SetActive(true);
            base.view.transform.Find("LoadingPanel/Label/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("ENTRY_PREPARED_LOGIN", new object[0]);
            this._accountReady = false;
        }

        public bool OnMihoyoAccountLoginSuccess()
        {
            string accountName = Singleton<AccountManager>.Instance.manager.GetAccountName();
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_WelcomeBack", new object[0]) + " " + accountName, 2f), UIType.Any);
            this.ShowMihoyoLoginUI();
            return false;
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.ShowMihoyoLoginUI)
            {
                return this.ShowMihoyoLoginUI();
            }
            if (ntf.type == NotifyTypes.MihoyoAccountLoginSuccess)
            {
                return this.OnMihoyoAccountLoginSuccess();
            }
            if (ntf.type == NotifyTypes.ShowLoadAssetText)
            {
                return this.ShowLoadAssetText((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.SetLoadAssetText)
            {
                return this.SetLoadAssetText((Tuple<bool, string, bool, float>) ntf.body);
            }
            if (ntf.type == NotifyTypes.FadeOutGameEntrySplash)
            {
                return this.FadeOutSplash();
            }
            if (ntf.type == NotifyTypes.SDKAccountLoginSuccess)
            {
                return this.OnSDKAccountLoginSuccess();
            }
            return ((ntf.type == NotifyTypes.ShowVersionText) && this.OnShowVersionText());
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x19:
                case 0x30:
                    return this.OnGetAvatarReletedInfo();

                case 5:
                    return this.OnGetPlayerTokenRsp(pkt.getData<GetPlayerTokenRsp>());

                case 7:
                    return this.OnPlayerLoginRsp(pkt.getData<PlayerLoginRsp>());

                case 0x2c:
                    return this.OnStageBeginRsp(pkt.getData<StageBeginRsp>());
            }
            return false;
        }

        private bool OnPlayerLoginRsp(PlayerLoginRsp rsp)
        {
            <OnPlayerLoginRsp>c__AnonStoreyEB yeb = new <OnPlayerLoginRsp>c__AnonStoreyEB {
                rsp = rsp
            };
            if (!this._hasLogin)
            {
                if (yeb.rsp.get_retcode() == null)
                {
                    <OnPlayerLoginRsp>c__AnonStoreyEA yea = new <OnPlayerLoginRsp>c__AnonStoreyEA {
                        <>f__ref$235 = yeb
                    };
                    base.view.transform.Find("LoginPanel/LastUser").gameObject.SetActive(false);
                    base.view.transform.Find("LoginPanel/EmptyUser").gameObject.SetActive(false);
                    yea.gameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
                    if (yeb.rsp.get_is_first_login() && !this._agreementShowed)
                    {
                        AgreementDialogContext dialogContext = new AgreementDialogContext {
                            buttonCallBack = new Action<bool>(yea.<>m__134)
                        };
                        Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                    }
                    else
                    {
                        yea.gameEntry.OnPlayerLogin(yeb.rsp.get_is_first_login());
                    }
                    this._hasLogin = true;
                }
                else if (yeb.rsp.get_retcode() != 4)
                {
                    string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(yeb.rsp.get_retcode(), new object[0]);
                    if (yeb.rsp.get_retcode() == 3)
                    {
                        DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(yeb.rsp.get_black_list_end_time());
                        object[] replaceParams = new object[] { Singleton<PlayerModule>.Instance.playerData.userId, yeb.rsp.get_msg(), dateTimeFromTimeStamp.ToString("yyyy-MM-dd HH:mm:ss") };
                        networkErrCodeOutput = LocalizationGeneralLogic.GetText("Menu_BlackList", replaceParams);
                    }
                    GeneralDialogContext context = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.SingleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                        desc = networkErrCodeOutput,
                        notDestroyAfterTouchBG = true
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
            }
            return false;
        }

        public void OnRigsterBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoRegisterDialogContext(), UIType.Any);
        }

        public bool OnSDKAccountLoginSuccess()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_WelcomeBack", new object[0]), 2f), UIType.Any);
            this._accountReady = true;
            return false;
        }

        private bool OnShowVersionText()
        {
            base.view.transform.Find("VersionText").gameObject.SetActive(true);
            string str = "v" + Singleton<NetworkManager>.Instance.GetGameVersion();
            if (GlobalVars.DataUseAssetBundle)
            {
                str = str + "-asb";
            }
            base.view.transform.Find("VersionText").GetComponent<Text>().text = str;
            return false;
        }

        public bool OnStageBeginRsp(StageBeginRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.DoBeginLevel(rsp);
            }
            return false;
        }

        public void OnTryUserBtnClick()
        {
            bool flag = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.LastLoginUserId != 0;
            bool flag2 = !string.IsNullOrEmpty(Singleton<AccountManager>.Instance.manager.AccountUid);
            if (!flag && !flag2)
            {
                GeneralDialogContext context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_GuestLogin", new object[0]),
                    notDestroyAfterTouchBG = true,
                    buttonCallBack = delegate (bool isOK) {
                        if (isOK)
                        {
                            this._agreementShowed = true;
                            AgreementDialogContext dialogContext = new AgreementDialogContext {
                                buttonCallBack = delegate (bool agree) {
                                    if (agree)
                                    {
                                        this.TryUserLogin();
                                    }
                                }
                            };
                            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                        }
                    }
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else
            {
                this.TryUserLogin();
            }
        }

        private bool SetLoadAssetText(Tuple<bool, string, bool, float> loadingInfo)
        {
            base.view.transform.Find("LoadingPanel").gameObject.SetActive(loadingInfo.Item1);
            base.view.transform.Find("LoadingPanel/Desc").GetComponent<Text>().text = loadingInfo.Item2;
            base.view.transform.Find("LoadingPanel/Progress").gameObject.SetActive(loadingInfo.Item3);
            base.view.transform.Find("LoadingPanel/Progress").GetComponent<Slider>().value = loadingInfo.Item4;
            if ((loadingInfo.Item2 == LocalizationGeneralLogic.GetText("ENTRY_PREPARED_LOGIN", new object[0])) || (loadingInfo.Item2 == LocalizationGeneralLogic.GetText("ENTRY_PREPARED", new object[0])))
            {
                base.view.transform.Find("LoadingPanel/Label/Text").GetComponent<Text>().text = loadingInfo.Item2;
                base.view.transform.Find("LoadingPanel").GetComponent<Animation>().Play();
            }
            return false;
        }

        private bool SetupAvatar3dModel()
        {
            int readyToTouchAvatarID = Singleton<GalTouchModule>.Instance.GetReadyToTouchAvatarID();
            AvatarDataItem avatar = Singleton<AvatarModule>.Instance.TryGetAvatarByID(readyToTouchAvatarID);
            if ((avatar == null) || !avatar.UnLocked)
            {
                if (<>f__am$cache9 == null)
                {
                    <>f__am$cache9 = x => x.UnLocked;
                }
                avatar = Singleton<AvatarModule>.Instance.UserAvatarList.Find(<>f__am$cache9);
            }
            UIUtil.Create3DAvatarByPage(avatar, MiscData.PageInfoKey.GameEntryPage, "Default");
            Singleton<GalTouchModule>.Instance.ChangeAvatar(avatar.avatarID);
            return false;
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("LoginPanel").gameObject.SetActive(false);
            base.view.transform.Find("DebugBtn").gameObject.SetActive(false);
            return false;
        }

        private bool ShowLoadAssetText(bool active)
        {
            return false;
        }

        public bool ShowMihoyoLoginUI()
        {
            base.view.transform.Find("LoginPanel").gameObject.SetActive(true);
            GeneralLocalDataItem generalLocalData = Singleton<MiHoYoGameData>.Instance.GeneralLocalData;
            bool flag = generalLocalData.LastLoginUserId != 0;
            bool flag2 = !string.IsNullOrEmpty(Singleton<AccountManager>.Instance.manager.AccountUid);
            this._accountReady = flag || flag2;
            base.view.transform.Find("LoginPanel/LastUser").gameObject.SetActive(flag || flag2);
            base.view.transform.Find("LoginPanel/EmptyUser").gameObject.SetActive(!flag && !flag2);
            string textID = (flag || flag2) ? "ENTRY_PREPARED" : "ENTRY_PREPARED_LOGIN";
            base.view.transform.Find("LoadingPanel/Label/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID, new object[0]);
            if (flag2)
            {
                string accountName = Singleton<AccountManager>.Instance.manager.GetAccountName();
                base.view.transform.Find("LoginPanel/LastUser/Info/Name").GetComponent<Text>().text = accountName;
                base.view.transform.Find("LoginPanel/LastUser/Info/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_User", new object[0]);
            }
            else if (flag)
            {
                string str3 = generalLocalData.LastLoginUserId.ToString();
                base.view.transform.Find("LoginPanel/LastUser/Info/Name").GetComponent<Text>().text = str3;
                base.view.transform.Find("LoginPanel/LastUser/Info/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_TryUser", new object[0]);
            }
            bool forbidNewUser = Singleton<NetworkManager>.Instance.DispatchSeverData.forbidNewUser;
            base.view.transform.Find("LoginPanel/EmptyUser/ForbidNewUser").gameObject.SetActive(forbidNewUser);
            base.view.transform.Find("LoginPanel/EmptyUser/Normal").gameObject.SetActive(!forbidNewUser);
            return false;
        }

        public bool StartFirstLevel()
        {
            try
            {
                LevelDataItem level = Singleton<LevelModule>.Instance.TryGetLevelById(0x2775);
                if (level == null)
                {
                    return false;
                }
                this._loadingWheelDialogContext = new LoadingWheelWidgetContext();
                Singleton<MainUIManager>.Instance.ShowWidget(this._loadingWheelDialogContext, UIType.Any);
                Singleton<NetworkManager>.Instance.RequestLevelBeginReq(level, 0);
                return true;
            }
            catch (Exception exception)
            {
                SuperDebug.VeryImportantError(exception.ToString());
                return false;
            }
        }

        private void StopSplashFadeOut()
        {
            if (this._splashFadeOutCoroutine != null)
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._splashFadeOutCoroutine);
                this._splashFadeOutCoroutine = null;
            }
        }

        private void TryUserLogin()
        {
            Singleton<MiHoYoGameData>.Instance.GeneralLocalData.ClearLastLoginUser();
            Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
            Singleton<AccountManager>.Instance.manager.Reset();
            Singleton<NetworkManager>.Instance.LoginGameServer();
        }

        public bool IsSplashFadeOut { get; private set; }

        [CompilerGenerated]
        private sealed class <FadeOutSplashImp>c__Iterator62 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameEntryPageContext <>f__this;
            internal float <alpha>__6;
            internal float <alpha>__8;
            internal int <count>__4;
            internal Image <image>__2;
            internal float <lerpBegin>__0;
            internal float <lerpEnd>__1;
            internal float <t>__5;
            internal float <t>__7;
            internal Color <temp>__3;

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
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_03C4;

                    case 1:
                        this.<lerpBegin>__0 = 1f;
                        this.<lerpEnd>__1 = 0f;
                        this.<image>__2 = this.<>f__this.view.transform.Find("Splash").GetComponent<Image>();
                        this.<temp>__3 = Color.white;
                        this.<count>__4 = 0;
                        break;

                    case 2:
                        break;

                    case 3:
                    case 4:
                    case 5:
                        goto Label_0343;

                    default:
                        goto Label_03C2;
                }
                if (this.<count>__4 <= this.<>f__this._splashFadeOutFrameCount)
                {
                    this.<t>__5 = ((float) this.<count>__4) / ((float) this.<>f__this._splashFadeOutFrameCount);
                    this.<alpha>__6 = Mathf.Lerp(this.<lerpBegin>__0, this.<lerpEnd>__1, Mathf.Clamp01(this.<t>__5));
                    this.<temp>__3.r = this.<image>__2.color.r;
                    this.<temp>__3.g = this.<image>__2.color.g;
                    this.<temp>__3.b = this.<image>__2.color.b;
                    this.<temp>__3.a = this.<alpha>__6;
                    this.<image>__2.color = this.<temp>__3;
                    this.<count>__4++;
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_03C4;
                }
                this.<image>__2.gameObject.SetActive(false);
                this.<count>__4 = 0;
                Singleton<WwiseAudioManager>.Instance.Post("GameEntry_Elevator_Start", null, null, null);
                this.<image>__2 = this.<>f__this.view.transform.Find("TransitBlack").GetComponent<Image>();
            Label_0343:
                while (this.<count>__4 <= this.<>f__this._transitBlackFadeOutFrameCount)
                {
                    if ((this.<count>__4 == 0) | (this.<count>__4 == 2))
                    {
                        this.<count>__4++;
                        this.$current = null;
                        this.$PC = 3;
                    }
                    else if (this.<count>__4 == 1)
                    {
                        GlobalDataManager.metaConfig = ConfigUtil.LoadConfig<ConfigMetaConfig>("Common/MetaConfig");
                        LocalDataVersion.LoadFromFile();
                        GraphicsSettingData.ReloadFromFile();
                        GraphicsSettingData.ApplySettingConfig();
                        this.<count>__4++;
                        this.$current = null;
                        this.$PC = 4;
                    }
                    else
                    {
                        this.<t>__7 = ((float) this.<count>__4) / ((float) this.<>f__this._transitBlackFadeOutFrameCount);
                        this.<alpha>__8 = Mathf.Lerp(this.<lerpBegin>__0, this.<lerpEnd>__1, Mathf.Clamp01(this.<t>__7));
                        this.<temp>__3.r = this.<image>__2.color.r;
                        this.<temp>__3.g = this.<image>__2.color.g;
                        this.<temp>__3.b = this.<image>__2.color.b;
                        this.<temp>__3.a = this.<alpha>__8;
                        this.<image>__2.color = this.<temp>__3;
                        this.<count>__4++;
                        this.$current = null;
                        this.$PC = 5;
                    }
                    goto Label_03C4;
                }
                this.<>f__this.view.transform.Find("Splash").gameObject.SetActive(false);
                this.<>f__this.view.transform.Find("TransitBlack").gameObject.SetActive(false);
                this.<>f__this.IsSplashFadeOut = true;
                this.<>f__this._splashFadeOutCoroutine = null;
                this.$PC = -1;
            Label_03C2:
                return false;
            Label_03C4:
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
        private sealed class <FadeOutVideoImp>c__Iterator63 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameEntryPageContext <>f__this;
            internal float <a>__4;
            internal Color <color>__1;
            internal int <count>__2;
            internal Image <image>__0;
            internal float <t>__3;

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
                        this.<>f__this.view.transform.Find("TransitBlack").gameObject.SetActive(true);
                        this.<image>__0 = this.<>f__this.view.transform.Find("TransitBlack").GetComponent<Image>();
                        this.<color>__1 = Color.white;
                        this.<count>__2 = 0;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0176;
                }
                if (this.<count>__2 <= this.<>f__this._transitBlackVideoFadeOutFrameCount)
                {
                    this.<t>__3 = ((float) this.<count>__2) / ((float) this.<>f__this._transitBlackVideoFadeOutFrameCount);
                    this.<a>__4 = Mathf.Lerp(0f, 1f, Mathf.Clamp01(this.<t>__3));
                    this.<color>__1.r = this.<image>__0.color.r;
                    this.<color>__1.g = this.<image>__0.color.g;
                    this.<color>__1.b = this.<image>__0.color.b;
                    this.<color>__1.a = this.<a>__4;
                    this.<image>__0.color = this.<color>__1;
                    this.<count>__2++;
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.$PC = -1;
            Label_0176:
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
        private sealed class <OnPlayerLoginRsp>c__AnonStoreyEA
        {
            internal GameEntryPageContext.<OnPlayerLoginRsp>c__AnonStoreyEB <>f__ref$235;
            internal MonoGameEntry gameEntry;

            internal void <>m__134(bool agree)
            {
                if (agree)
                {
                    this.gameEntry.OnPlayerLogin(this.<>f__ref$235.rsp.get_is_first_login());
                }
                else
                {
                    ApplicationManager.Quit();
                }
            }
        }

        [CompilerGenerated]
        private sealed class <OnPlayerLoginRsp>c__AnonStoreyEB
        {
            internal PlayerLoginRsp rsp;
        }

        public enum DescImageType
        {
            None,
            Loading,
            Identifying,
            Confirmed
        }
    }
}

