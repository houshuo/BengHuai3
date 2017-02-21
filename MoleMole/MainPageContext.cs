namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class MainPageContext : BasePageContext
    {
        private MonoFadeImage _fadeImage;
        private SequenceDialogManager _firstLoginDialogManager;
        private bool _isMessagePlaying;
        private int _maxQuenedMessageCount = 10;
        private bool _optionalBtnsShowed;
        private Queue<PopShowMessage> _showMessageQueue;
        private bool _waitingForIslandServerData;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache8;
        [CompilerGenerated]
        private static Action <>f__am$cache9;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cacheA;
        [CompilerGenerated]
        private static Action <>f__am$cacheB;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cacheC;
        private const string BTNS_FADE_IN_STR = "MainMenuBtnFadeIn";
        private const string BTNS_FADE_OUT_STR = "MainMenuBtnFadeOut";
        private const string PAGE_FADE_IN_STR = "PageFadeIn";

        public MainPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "MainPageContext",
                viewPrefabPath = "UI/Menus/Page/MainPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            base.showSpaceShip = true;
            this._showMessageQueue = new Queue<PopShowMessage>();
            this._maxQuenedMessageCount = MiscData.Config.ChatConfig.MaxQuenedPopMessageAmount;
            this._isMessagePlaying = false;
        }

        private bool AllowEnqueneMessage()
        {
            return (this._showMessageQueue.Count < this._maxQuenedMessageCount);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("MainBtns/AvatarBtn").GetComponent<Button>(), new UnityAction(this.OnAvatarBtnClick));
            base.BindViewCallback(base.view.transform.Find("MainBtns/StorageBtn").GetComponent<Button>(), new UnityAction(this.OnStorageBtnClick));
            base.BindViewCallback(base.view.transform.Find("MainBtns/MapBtn").GetComponent<Button>(), new UnityAction(this.OnMapBtnClick));
            base.BindViewCallback(base.view.transform.Find("MainBtns/GachaBtn").GetComponent<Button>(), new UnityAction(this.OnGachaButtonClick));
            base.BindViewCallback(base.view.transform.Find("MainBtns/IslandBtn").GetComponent<Button>(), new UnityAction(this.OnIslandButtonClick));
            base.BindViewCallback(base.view.transform.Find("OptionMainBtn").GetComponent<Button>(), new UnityAction(this.OnOptionBtnClick));
            base.BindViewCallback(base.view.transform.Find("OptionBtns/Btns/FriendBtn").GetComponent<Button>(), new UnityAction(this.OnFriendButtonClick));
            base.BindViewCallback(base.view.transform.Find("OptionBtns/Btns/MailBtn").GetComponent<Button>(), new UnityAction(this.OnMailButtonClick));
            base.BindViewCallback(base.view.transform.Find("OptionBtns/Btns/ConfigBtn").GetComponent<Button>(), new UnityAction(this.OnConfigButtonClick));
            base.BindViewCallback(base.view.transform.Find("OptionBtns/Btns/MissionBtn").GetComponent<Button>(), new UnityAction(this.OnMissionButtonClick));
            base.BindViewCallback(base.view.transform.Find("OptionBtns/Btns/BulletinBoardBtn").GetComponent<Button>(), new UnityAction(this.OnBulletinBoardButtonClick));
            base.BindViewCallback(base.view.transform.Find("OptionBtns/Btns/FeedbackBtn").GetComponent<Button>(), new UnityAction(this.OnFeedbackButtonClick));
            base.BindViewCallback(base.view.transform.Find("Talk").GetComponent<Button>(), new UnityAction(this.OnTalkBtnClick));
        }

        private bool CanShowInviteHintDialog()
        {
            if (!MiscData.Config.BasicConfig.IsInviteFeatureEnable)
            {
                return false;
            }
            if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
            {
                return false;
            }
            if (!Singleton<AccountManager>.Instance.manager.IsAccountBind())
            {
                return false;
            }
            PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
            if (playerData.teamLevel >= playerData.maxLevelToAcceptInvite)
            {
                return false;
            }
            if (Singleton<MiHoYoGameData>.Instance.LocalData.HasShowInviteHintDialog)
            {
                return false;
            }
            return Singleton<TutorialModule>.Instance.IsTutorialIDFinish(0x439);
        }

        private bool CanShowStartUpDialogs()
        {
            if (Singleton<PlayerModule>.GetInstance().playerData.uiTempSaveData.hasShowedStartUpDialogs)
            {
                return false;
            }
            if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
            {
                return false;
            }
            return Singleton<TutorialModule>.GetInstance().IsTutorialIDFinish(0x439);
        }

        private void CheckBulletin()
        {
            if (TimeUtil.Now >= Singleton<BulletinModule>.Instance.LastCheckBulletinTime.AddSeconds((double) MiscData.Config.BasicConfig.CheckBulletinIntervalSecond))
            {
                Singleton<NetworkManager>.Instance.RequestGetBulletin();
            }
        }

        private void CheckDailyUpdateTime()
        {
            if (TimeUtil.AcrossDailyUpdateTime())
            {
                Singleton<NetworkManager>.Instance.SendPacketsOnLoginSuccess(true, 0);
            }
        }

        private void CheckInviteHint()
        {
            if (this.CanShowInviteHintDialog())
            {
                GeneralDialogContext context2 = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    title = LocalizationGeneralLogic.GetText("Title_InvitationCode_Input", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Invitation_Tutorial", new object[0]),
                    okBtnText = LocalizationGeneralLogic.GetText("Menu_OK", new object[0]),
                    cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Cancel", new object[0])
                };
                if (<>f__am$cacheB == null)
                {
                    <>f__am$cacheB = delegate {
                        Singleton<MiHoYoGameData>.Instance.LocalData.HasShowInviteHintDialog = true;
                        Singleton<MiHoYoGameData>.Instance.Save();
                    };
                }
                context2.destroyCallBack = <>f__am$cacheB;
                if (<>f__am$cacheC == null)
                {
                    <>f__am$cacheC = delegate (bool confirmed) {
                        Singleton<MiHoYoGameData>.Instance.LocalData.HasShowInviteHintDialog = true;
                        Singleton<MiHoYoGameData>.Instance.Save();
                        if (confirmed)
                        {
                            Singleton<MainUIManager>.Instance.ShowPage(new FriendOverviewPageContext(FriendOverviewPageContext.TAB_KEY[3], InviteTab.InviteeTab), UIType.Page);
                        }
                    };
                }
                context2.buttonCallBack = <>f__am$cacheC;
                GeneralDialogContext dialogContext = context2;
                if ((this._firstLoginDialogManager != null) && this._firstLoginDialogManager.IsPlaying())
                {
                    this._firstLoginDialogManager.AddDialog(dialogContext);
                }
                else
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
            }
        }

        private void CheckMessage()
        {
            if ((this._showMessageQueue.Count > 0) && !this._isMessagePlaying)
            {
                this._isMessagePlaying = true;
                base.view.transform.Find("NewMessages").GetComponent<MonoMainMessages>().ShowMessage(this._showMessageQueue.Peek(), new Action(this.ShowNextMessage));
            }
        }

        private void CheckWeather()
        {
            if (TimeUtil.Now < Singleton<MiHoYoGameData>.Instance.LocalData.EndThunderDateTime)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowThunderWeather, null));
            }
            else if (TimeUtil.Now > Singleton<MiHoYoGameData>.Instance.LocalData.NextRandomDateTime)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowRandomWeather, null));
                Singleton<MiHoYoGameData>.Instance.LocalData.NextRandomDateTime = TimeUtil.Now.AddHours(2.0);
            }
        }

        private void EnquenePopShowMessage(PopShowMessage message)
        {
            if ((message != null) && this.AllowEnqueneMessage())
            {
                this._showMessageQueue.Enqueue(message);
            }
        }

        public void FadeIn(float speed = 1f)
        {
            if (this._fadeImage != null)
            {
                this._fadeImage.FadeIn(speed);
            }
        }

        public void FadeOut(float speed = 1f)
        {
            if (this._fadeImage != null)
            {
                this._fadeImage.FadeOut(speed);
            }
        }

        private void InitFade()
        {
            this._fadeImage = base.view.GetComponentInChildren<MonoFadeImage>();
        }

        private void OnAvatarBtnClick()
        {
            AvatarOverviewPageContext context = new AvatarOverviewPageContext {
                type = AvatarOverviewPageContext.PageType.Show,
                selectedAvatarID = Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.lastSelectedAvatarID
            };
            Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
        }

        private void OnBulletinBoardButtonClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new BulletinBoardDialogContext(BulletinBoardDialogContext.ShowType.ShowEvent), UIType.Any);
        }

        private void OnConfigButtonClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new SettingPageContext("AudioTab"), UIType.Page);
        }

        private void OnFeedbackButtonClick()
        {
            int userId = Singleton<PlayerModule>.Instance.playerData.userId;
            WebViewGeneralLogic.LoadUrl(string.Format("http://webview.bh3.com/bug_feedback/index.php?uid={0}", userId), true);
        }

        private void OnFirstTimeLandedOnMainPage()
        {
            GeneralDialogContext context4;
            this._firstLoginDialogManager = new SequenceDialogManager(null);
            int offlineFriendsPoint = Singleton<PlayerModule>.Instance.playerData.offlineFriendsPoint;
            if (offlineFriendsPoint > 0)
            {
                context4 = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
                };
                object[] replaceParams = new object[] { offlineFriendsPoint };
                context4.desc = LocalizationGeneralLogic.GetText("Menu_Desc_OfflineFriendPointGet", replaceParams);
                this._firstLoginDialogManager.AddDialog(context4);
                Singleton<PlayerModule>.Instance.playerData.offlineFriendsPoint = 0;
            }
            if (this.CanShowStartUpDialogs())
            {
                if (!Singleton<AccountManager>.Instance.manager.IsAccountBind())
                {
                    if (TimeUtil.Now.Subtract(Singleton<MiHoYoGameData>.Instance.LocalData.LastShowBindAccountWarningTime).TotalSeconds >= MiscData.Config.BasicConfig.BindAccountWarningIntervalSecond)
                    {
                        context4 = new GeneralDialogContext {
                            type = GeneralDialogContext.ButtonType.DoubleButton,
                            title = LocalizationGeneralLogic.GetText("Menu_Title_BindAccount", new object[0]),
                            desc = LocalizationGeneralLogic.GetText("Menu_Desc_BindAccount", new object[0]),
                            okBtnText = LocalizationGeneralLogic.GetText("Menu_Action_DoBindAccount", new object[0]),
                            cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Action_CancelBindAccount", new object[0])
                        };
                        if (<>f__am$cache7 == null)
                        {
                            <>f__am$cache7 = delegate (bool confirmed) {
                                if (confirmed)
                                {
                                    Singleton<AccountManager>.Instance.manager.BindUI();
                                }
                            };
                        }
                        context4.buttonCallBack = <>f__am$cache7;
                        GeneralDialogContext dialogContext = context4;
                        this._firstLoginDialogManager.AddDialog(dialogContext);
                        Singleton<MiHoYoGameData>.Instance.LocalData.LastShowBindAccountWarningTime = TimeUtil.Now;
                        Singleton<MiHoYoGameData>.Instance.Save();
                    }
                }
                else if (((Singleton<AccountManager>.Instance.manager is TheOriginalAccountManager) && !(Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager).IsRealNameVerify) && (TimeUtil.Now.Subtract(Singleton<MiHoYoGameData>.Instance.LocalData.LastShowBindIdentityWarningTime).TotalSeconds >= MiscData.Config.BasicConfig.BindIdentityWarningIntervalSecond))
                {
                    context4 = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.DoubleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Title_BindIdentity", new object[0]),
                        desc = LocalizationGeneralLogic.GetText("Menu_Desc_BindIdentity", new object[0]),
                        okBtnText = LocalizationGeneralLogic.GetText("Menu_Action_DoBindIdentity", new object[0]),
                        cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Action_CancelBindIdentity", new object[0])
                    };
                    if (<>f__am$cache8 == null)
                    {
                        <>f__am$cache8 = delegate (bool confirmed) {
                            if (confirmed)
                            {
                                WebViewGeneralLogic.LoadUrl(string.Format((Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager).ORIGINAL_BIND_IDENTITY_URL + "?uid={0}&token={1}", Singleton<AccountManager>.Instance.manager.AccountUid, Singleton<AccountManager>.Instance.manager.AccountToken), false);
                            }
                        };
                    }
                    context4.buttonCallBack = <>f__am$cache8;
                    GeneralDialogContext context2 = context4;
                    this._firstLoginDialogManager.AddDialog(context2);
                    Singleton<MiHoYoGameData>.Instance.LocalData.LastShowBindIdentityWarningTime = TimeUtil.Now;
                    Singleton<MiHoYoGameData>.Instance.Save();
                }
                if (this.CanShowInviteHintDialog())
                {
                    context4 = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.DoubleButton,
                        title = LocalizationGeneralLogic.GetText("Title_InvitationCode_Input", new object[0]),
                        desc = LocalizationGeneralLogic.GetText("Invitation_Tutorial", new object[0]),
                        okBtnText = LocalizationGeneralLogic.GetText("Menu_OK", new object[0]),
                        cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Cancel", new object[0])
                    };
                    if (<>f__am$cache9 == null)
                    {
                        <>f__am$cache9 = delegate {
                            Singleton<MiHoYoGameData>.Instance.LocalData.HasShowInviteHintDialog = true;
                            Singleton<MiHoYoGameData>.Instance.Save();
                        };
                    }
                    context4.destroyCallBack = <>f__am$cache9;
                    if (<>f__am$cacheA == null)
                    {
                        <>f__am$cacheA = delegate (bool confirmed) {
                            Singleton<MiHoYoGameData>.Instance.LocalData.HasShowInviteHintDialog = true;
                            Singleton<MiHoYoGameData>.Instance.Save();
                            if (confirmed)
                            {
                                Singleton<MainUIManager>.Instance.ShowPage(new FriendOverviewPageContext(FriendOverviewPageContext.TAB_KEY[3], InviteTab.InviteeTab), UIType.Page);
                            }
                        };
                    }
                    context4.buttonCallBack = <>f__am$cacheA;
                    GeneralDialogContext context3 = context4;
                    this._firstLoginDialogManager.AddDialog(context3);
                }
            }
            if (this.CanShowStartUpDialogs())
            {
                this.ShowBulletin(false);
            }
            this._firstLoginDialogManager.StartShow(0f);
            if (this.CanShowStartUpDialogs())
            {
                Singleton<NetworkManager>.GetInstance().RequestGetSignInRewardStatus();
                Singleton<PlayerModule>.GetInstance().playerData.uiTempSaveData.hasShowedStartUpDialogs = true;
            }
            AntiCheatPlugin.Init(MiscData.Config.AntiCheat.Enable, MiscData.Config.AntiCheat.LibList, MiscData.Config.AntiCheat.ProcList);
            Singleton<ApplicationManager>.Instance.DetectCheat();
            AntiEmulatorPlugin.Init(MiscData.Config.AntiEmulator.Enable, MiscData.Config.AntiEmulator.DeviceModelList);
            Singleton<ApplicationManager>.Instance.DetectEmulator();
        }

        public void OnFriendButtonClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new FriendOverviewPageContext("FriendListTab", InviteTab.InviteeTab), UIType.Page);
        }

        private void OnGachaButtonClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new SupplyEntrancePageContext(), UIType.Page);
        }

        private bool OnGetAvatarReletedInfo()
        {
            this.UpdateAvatarPopUp();
            this.SetupAvatar3dModel();
            return false;
        }

        private bool OnGetIsLandRsp(GetIslandRsp rsp)
        {
            if (this._waitingForIslandServerData)
            {
                this._waitingForIslandServerData = false;
                Singleton<MainUIManager>.Instance.MoveToNextScene("Island", false, true, true, null, true);
            }
            return false;
        }

        private bool OnGetOfflineFriendsPointNotify(GetOfflineFriendsPointNotify rsp)
        {
            this.TryShowOfflineFriendsPointView();
            return false;
        }

        private bool OnGetSignInRewardStatusRsp(GetSignInRewardStatusRsp rsp)
        {
            if ((rsp.get_retcode() == null) && rsp.get_is_need_sign_in())
            {
                SignInDialogContext dialogContext = new SignInDialogContext(rsp);
                if ((this._firstLoginDialogManager != null) && this._firstLoginDialogManager.IsPlaying())
                {
                    this._firstLoginDialogManager.AddDialog(dialogContext);
                }
                else
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
            }
            return false;
        }

        private void OnIslandButtonClick()
        {
            this._waitingForIslandServerData = true;
            Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(0x9d, null), UIType.Any);
            Singleton<NetworkManager>.Instance.RequestGetIsland();
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            this.SetupAvatar3dModel();
            this.OnRecvFriendRelatedNotify();
            this.PlayPageFadeInAniamtion();
            this.SetRedPoints();
            this.CheckDailyUpdateTime();
            UIUtil.SpaceshipCheckWeather();
            this.SetGalTouchActive(true);
            Singleton<AssetBundleManager>.Instance.CheckSVNVersion();
            this.CheckBulletin();
            this.SetIslandEntry();
            this.CheckInviteHint();
        }

        public void OnMailButtonClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new MailOverviewPageContext(), UIType.Page);
        }

        private void OnMainBtnPress(Transform trans, bool isPress)
        {
            trans.Find("Unselect").gameObject.SetActive(!isPress);
            trans.Find("Select").gameObject.SetActive(isPress);
        }

        private void OnMapBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(null), UIType.Page);
        }

        private void OnMissionButtonClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new MissionOverviewPageContext(), UIType.Page);
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.BulletinPopUpUpdate) && this.OnRecvBulletinRelatedNotify());
        }

        private void OnOptionBtnClick()
        {
            this._optionalBtnsShowed = !this._optionalBtnsShowed;
            base.view.transform.Find("OptionMainBtn/BG/OnImg").gameObject.SetActive(this._optionalBtnsShowed);
            base.view.transform.Find("OptionMainBtn/BG/OffImg").gameObject.SetActive(!this._optionalBtnsShowed);
            if (this._optionalBtnsShowed)
            {
                base.view.transform.GetComponent<Animation>().Play("MainMenuBtnFadeIn");
                MonoMainCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoMainCanvas;
                SuperDebug.VeryImportantAssert((sceneCanvas != null) && (Singleton<MainUIManager>.Instance.SceneCanvas != null), "canvas is not maincanvas1 " + Singleton<MainUIManager>.Instance.SceneCanvas.ToString());
                if (sceneCanvas != null)
                {
                    Transform transform = sceneCanvas.playerBar.view.transform;
                    transform.Find("TeamBriefPanel").GetComponent<Animation>().Play("TeamBriefFadeIn");
                    transform.Find("RightPanel").GetComponent<Animation>().Play("RightPanelFadeIn");
                }
            }
            else
            {
                base.view.transform.GetComponent<Animation>().Play("MainMenuBtnFadeOut");
                MonoMainCanvas canvas2 = Singleton<MainUIManager>.Instance.SceneCanvas as MonoMainCanvas;
                SuperDebug.VeryImportantAssert((canvas2 != null) && (Singleton<MainUIManager>.Instance.SceneCanvas != null), "canvas is not maincanvas2 " + Singleton<MainUIManager>.Instance.SceneCanvas.ToString());
                if (canvas2 != null)
                {
                    Transform transform2 = canvas2.playerBar.view.transform;
                    transform2.Find("TeamBriefPanel").GetComponent<Animation>().Play("TeamBriefFadeOut");
                    transform2.Find("RightPanel").GetComponent<Animation>().Play("RightPanelFadeOut");
                }
            }
            this.SetOptionalBtnsInteractable(this._optionalBtnsShowed);
        }

        private void OnOptionBtnPress(Transform trans, bool isPress)
        {
            trans.Find("Text").GetComponent<Outline>().effectColor = !isPress ? MiscData.GetColor("Blue") : MiscData.GetColor("Orange");
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            ushort num = pkt.getCmdId();
            switch (num)
            {
                case 0x19:
                case 0x30:
                    return this.OnGetAvatarReletedInfo();

                case 0x61:
                    return this.OnRecvSystemChatMsgNotify(pkt.getData<RecvSystemChatMsgNotify>());

                case 0x51:
                    return this.OnGetOfflineFriendsPointNotify(pkt.getData<GetOfflineFriendsPointNotify>());

                case 0x5b:
                    return this.OnRecvWorldChatMsgNotify(pkt.getData<RecvWorldChatMsgNotify>());

                case 0x5d:
                    return this.OnRecvFriendChatMsgNotify(pkt.getData<RecvFriendChatMsgNotify>());

                case 0x5e:
                    return this.OnRecvFriendOfflineChatMsgNotify(pkt.getData<RecvFriendOfflineChatMsgNotify>());

                case 0x41:
                case 0x47:
                    return this.OnRecvFriendRelatedNotify();

                case 0x55:
                case 0x57:
                    return this.OnRecvMailRelatedNotify();

                case 0x71:
                case 0x74:
                    return this.OnRecvMissionNotify();

                case 0x7a:
                    return this.OnGetSignInRewardStatusRsp(pkt.getData<GetSignInRewardStatusRsp>());

                case 0x8a:
                    return this.OnRecvBulletinRelatedNotify();

                case 0x9d:
                    return this.OnGetIsLandRsp(pkt.getData<GetIslandRsp>());

                case 0xc6:
                    return this.SetupWelfareHint();
            }
            if ((num != 0x9d) && (num != 0xa9))
            {
                return false;
            }
            return this.SetupIslandHint();
        }

        private bool OnRecvBulletinRelatedNotify()
        {
            base.view.transform.Find("OptionBtns/Btns/BulletinBoardBtn/PopUp").gameObject.SetActive(Singleton<BulletinModule>.Instance.HasNewBulletins());
            return false;
        }

        private bool OnRecvFriendChatMsgNotify(RecvFriendChatMsgNotify rsp)
        {
            this.EnquenePopShowMessage(new PopShowMessage(rsp.get_chat_msg().get_msg(), rsp.get_chat_msg().get_uid()));
            this.CheckMessage();
            return false;
        }

        private bool OnRecvFriendOfflineChatMsgNotify(RecvFriendOfflineChatMsgNotify rsp)
        {
            foreach (ChatMsg msg in rsp.get_chat_msg_list())
            {
                this.EnquenePopShowMessage(new PopShowMessage(msg.get_msg(), msg.get_uid()));
            }
            this.CheckMessage();
            return false;
        }

        private bool OnRecvFriendRelatedNotify()
        {
            base.view.transform.Find("OptionBtns/Btns/FriendBtn/PopUp").gameObject.SetActive(Singleton<FriendModule>.Instance.HasNewFriend() || Singleton<FriendModule>.Instance.HasNewRequest());
            return false;
        }

        private bool OnRecvMailRelatedNotify()
        {
            base.view.transform.Find("OptionBtns/Btns/MailBtn/PopUp").gameObject.SetActive(Singleton<MailModule>.Instance.HasNewMail());
            return false;
        }

        private bool OnRecvMissionNotify()
        {
            base.view.transform.Find("OptionBtns/Btns/MissionBtn/PopUp").gameObject.SetActive(Singleton<MissionModule>.Instance.NeedNotify());
            return false;
        }

        private bool OnRecvSystemChatMsgNotify(RecvSystemChatMsgNotify rsp)
        {
            if (rsp.get_chat_msg().get_type() == 1)
            {
                ChatMsgDataItem item = new ChatMsgDataItem(rsp.get_chat_msg());
                string msgContent = string.Format("{0} {1}", item.nickname, item.msg);
                this.EnquenePopShowMessage(new PopShowMessage(msgContent, MessageSource.System));
            }
            this.CheckMessage();
            return false;
        }

        private bool OnRecvWorldChatMsgNotify(RecvWorldChatMsgNotify rsp)
        {
            this.EnquenePopShowMessage(new PopShowMessage(rsp.get_chat_msg().get_msg(), MessageSource.World));
            this.CheckMessage();
            return false;
        }

        protected override void OnSetActive(bool enabled)
        {
            if (enabled)
            {
                if (this.CanShowStartUpDialogs())
                {
                    this.ShowStartUpDialogs();
                }
            }
            else
            {
                this.SetGalTouchActive(false);
            }
        }

        private void OnStorageBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new StorageShowPageContext(), UIType.Page);
        }

        public void OnTalkBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new ChatDialogContext(), UIType.Any);
        }

        private void PlayPageFadeInAniamtion()
        {
            base.view.transform.GetComponent<Animation>().Play("MainMenuBtnFadeIn");
            MonoMainCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoMainCanvas;
            if (sceneCanvas != null)
            {
                Transform transform = sceneCanvas.playerBar.view.transform;
                transform.Find("TeamBriefPanel").GetComponent<Animation>().Play("TeamBriefFadeIn");
                transform.Find("RightPanel").GetComponent<Animation>().Play("RightPanelFadeIn");
            }
            this._optionalBtnsShowed = true;
            this.SetOptionalBtnsInteractable(this._optionalBtnsShowed);
        }

        public override void SetActive(bool enabled)
        {
            if (!enabled)
            {
                MonoGalTouchView view = UnityEngine.Object.FindObjectOfType<MonoGalTouchView>();
                if (view != null)
                {
                    view.gameObject.SetActive(false);
                }
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ExitMainPage, null));
            }
            else
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EnterMainPage, null));
            }
            base.SetActive(enabled);
        }

        private void SetGalTouchActive(bool active)
        {
            MonoMainCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas() as MonoMainCanvas;
            if (mainCanvas != null)
            {
                Avatar3dModelContext context = mainCanvas.avatar3dModelContext;
                if (context != null)
                {
                    if (active)
                    {
                        context.TriggerStartGalTouch();
                    }
                    else
                    {
                        context.TriggerStopGalTouch();
                    }
                }
            }
        }

        private void SetIslandEntry()
        {
            int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
            Transform transform = base.view.transform.Find("MainBtns/IslandBtn");
            if (!GlobalVars.ENABLE_ISLAND_ENTRY)
            {
                transform.gameObject.SetActive(false);
            }
            else
            {
                int playerLevelNeedShowIslandButton = MiscData.Config.BasicConfig.PlayerLevelNeedShowIslandButton;
                int playerLevelNeedEnterIsland = MiscData.Config.BasicConfig.PlayerLevelNeedEnterIsland;
                if (teamLevel < playerLevelNeedShowIslandButton)
                {
                    transform.gameObject.SetActive(false);
                }
                else if (teamLevel < playerLevelNeedEnterIsland)
                {
                    transform.gameObject.SetActive(true);
                    transform.Find("LockPanel").gameObject.SetActive(true);
                    transform.GetComponent<PressWithCallBack>().onPress = null;
                    transform.GetComponent<Button>().interactable = false;
                }
                else
                {
                    transform.gameObject.SetActive(true);
                    transform.Find("LockPanel").gameObject.SetActive(false);
                    transform.GetComponent<PressWithCallBack>().onPress = new PressWithCallBack.OnPress(this.OnMainBtnPress);
                    transform.GetComponent<Button>().interactable = true;
                }
            }
        }

        private void SetOptionalBtnsInteractable(bool interactable)
        {
            IEnumerator enumerator = base.view.transform.Find("OptionBtns/Btns").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    current.GetComponent<Button>().enabled = interactable;
                    current.GetComponent<MonoButtonWwiseEvent>().enabled = interactable;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        private void SetRedPoints()
        {
            this.OnRecvFriendRelatedNotify();
            this.OnRecvMailRelatedNotify();
            this.OnRecvBulletinRelatedNotify();
            this.OnRecvMissionNotify();
            this.UpdateAvatarPopUp();
        }

        private bool SetupAvatar3dModel()
        {
            int currentTouchAvatarID = Singleton<GalTouchModule>.Instance.GetCurrentTouchAvatarID();
            if (currentTouchAvatarID == 0)
            {
                currentTouchAvatarID = 0x65;
            }
            UIUtil.Create3DAvatarByPage(Singleton<AvatarModule>.Instance.GetAvatarByID(currentTouchAvatarID), MiscData.PageInfoKey.MainPage, "Default");
            return false;
        }

        private bool SetupIslandHint()
        {
            bool flag = Singleton<IslandModule>.Instance.HasCabinNeedToShowLevelUp() || (Singleton<IslandModule>.Instance.GetVentureDoneNum() > 0);
            base.view.transform.Find("MainBtns/IslandBtn/PopUp").gameObject.SetActive(flag);
            return false;
        }

        protected override bool SetupView()
        {
            this.SetupAvatar3dModel();
            this.OnRecvFriendRelatedNotify();
            this.OnRecvMissionNotify();
            this.OnRecvMailRelatedNotify();
            this.OnRecvBulletinRelatedNotify();
            foreach (PressWithCallBack back in base.view.transform.Find("MainBtns").GetComponentsInChildren<PressWithCallBack>(true))
            {
                back.onPress = new PressWithCallBack.OnPress(this.OnMainBtnPress);
            }
            this.PlayPageFadeInAniamtion();
            if (!Singleton<PlayerModule>.GetInstance().playerData.uiTempSaveData.hasLandedMainPage)
            {
                Singleton<PlayerModule>.GetInstance().playerData.uiTempSaveData.hasLandedMainPage = true;
                this.OnFirstTimeLandedOnMainPage();
            }
            else
            {
                this.CheckDailyUpdateTime();
                Singleton<AssetBundleManager>.GetInstance().CheckSVNVersion();
                if (this.CanShowStartUpDialogs())
                {
                    this.ShowStartUpDialogs();
                }
                else
                {
                    this.CheckBulletin();
                }
            }
            base.view.transform.Find("CloundDebugBtn").gameObject.SetActive(GlobalVars.DEBUG_FEATURE_ON);
            this.InitFade();
            this.UpdateAvatarPopUp();
            UIUtil.SpaceshipCheckWeather();
            this.SetIslandEntry();
            this.SetupWelfareHint();
            this.SetupIslandHint();
            return false;
        }

        private bool SetupWelfareHint()
        {
            bool flag = Singleton<ShopWelfareModule>.Instance.HasWelfareCanGet();
            base.view.transform.Find("MainBtns/GachaBtn/PopUp").gameObject.SetActive(flag);
            return false;
        }

        private void ShowBulletin(bool force = false)
        {
            TimeSpan span = TimeUtil.Now.Subtract(Singleton<MiHoYoGameData>.GetInstance().LocalData.LastShowBulletinTime);
            if (force || (span.TotalSeconds >= MiscData.Config.BasicConfig.ShowBulletinIntervalSecond))
            {
                if (this._firstLoginDialogManager != null)
                {
                    this._firstLoginDialogManager.AddDialog(new BulletinBoardDialogContext(BulletinBoardDialogContext.ShowType.ShowEvent));
                }
                Singleton<MiHoYoGameData>.GetInstance().LocalData.LastShowBulletinTime = TimeUtil.Now;
                Singleton<MiHoYoGameData>.GetInstance().Save();
            }
        }

        private void ShowNextMessage()
        {
            this._showMessageQueue.Dequeue();
            if (this._showMessageQueue.Count <= 0)
            {
                this._isMessagePlaying = false;
            }
            else
            {
                base.view.transform.Find("NewMessages").GetComponent<MonoMainMessages>().ShowMessage(this._showMessageQueue.Peek(), new Action(this.ShowNextMessage));
            }
        }

        private void ShowStartUpDialogs()
        {
            this.ShowBulletin(true);
            if (this._firstLoginDialogManager != null)
            {
                this._firstLoginDialogManager.StartShow(0f);
            }
            Singleton<NetworkManager>.GetInstance().RequestGetSignInRewardStatus();
            Singleton<PlayerModule>.GetInstance().playerData.uiTempSaveData.hasShowedStartUpDialogs = true;
        }

        public override void StartUp(Transform canvasTrans, Transform viewParent)
        {
            base.StartUp(canvasTrans, viewParent);
            this.SetGalTouchActive(true);
        }

        private void TryShowOfflineFriendsPointView()
        {
            int offlineFriendsPoint = Singleton<PlayerModule>.Instance.playerData.offlineFriendsPoint;
            if (offlineFriendsPoint > 0)
            {
                GeneralDialogContext context2 = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
                };
                object[] replaceParams = new object[] { offlineFriendsPoint };
                context2.desc = LocalizationGeneralLogic.GetText("Menu_Desc_OfflineFriendPointGet", replaceParams);
                GeneralDialogContext dialogContext = context2;
                if ((this._firstLoginDialogManager != null) && this._firstLoginDialogManager.IsPlaying())
                {
                    this._firstLoginDialogManager.AddDialog(dialogContext);
                }
                else
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
                Singleton<PlayerModule>.Instance.playerData.offlineFriendsPoint = 0;
            }
        }

        private void UpdateAvatarPopUp()
        {
            List<AvatarDataItem> userAvatarList = Singleton<AvatarModule>.Instance.UserAvatarList;
            bool flag = false;
            foreach (AvatarDataItem item in userAvatarList)
            {
                if (item.CanStarUp)
                {
                    flag = true;
                    break;
                }
            }
            base.view.transform.Find("MainBtns/AvatarBtn/PopUp").gameObject.SetActive(flag);
        }

        public enum MessageSource
        {
            None,
            System,
            World,
            Guild,
            Friend
        }

        public class PopShowMessage
        {
            public string message;
            public MainPageContext.MessageSource source;
            public uint talkingUid;

            public PopShowMessage(string msgContent, MainPageContext.MessageSource msgSource)
            {
                this.message = msgContent;
                this.source = msgSource;
            }

            public PopShowMessage(string msgContent, uint uid)
            {
                this.message = msgContent;
                this.talkingUid = uid;
                this.source = MainPageContext.MessageSource.Friend;
            }
        }
    }
}

