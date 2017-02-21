namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class PlayerProfilePageContext : BasePageContext
    {
        private MonoAvatarStar _avatarStar;
        private string _currentTab;
        private string _redeemCode;
        private TabManager _tabManager;
        [CompilerGenerated]
        private static Predicate<MissionDataItem> <>f__am$cache4;
        [CompilerGenerated]
        private static Predicate<LinearMissionData> <>f__am$cache5;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache6;
        private const string ACCOUNT_TAB_NAME = "Account";
        private const string PLAYER_TAB_NAME = "Player";
        private const int REDEEM_CODE_LENGTH = 10;

        public PlayerProfilePageContext(TabType tabType = 0)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "PlayerProfilePageContext",
                viewPrefabPath = "UI/Menus/Page/PlayerProfile/PlayerProfilePage"
            };
            base.config = pattern;
            this._tabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
            switch (tabType)
            {
                case TabType.PlayerTab:
                    this._currentTab = "Player";
                    break;

                case TabType.AccountTab:
                    this._currentTab = "Account";
                    break;
            }
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("ProfilePanel/Buttons/ChangeDescBtn").GetComponent<Button>(), new UnityAction(this.OnChangeDescBtnClick));
            base.BindViewCallback(base.view.transform.Find("ProfilePanel/Buttons/ChangeNickNameBtn").GetComponent<Button>(), new UnityAction(this.OnChangeNickNameBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabButtons/PlayerButton").GetComponent<Button>(), new UnityAction(this.OnPlayerBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabButtons/AccountButton").GetComponent<Button>(), new UnityAction(this.OnAccountBtnClick));
            base.BindViewCallback(base.view.transform.Find("AccountPanel/LogoutBtn").GetComponent<Button>(), new UnityAction(this.OnLogoutBtnClick));
            base.BindViewCallback(base.view.transform.Find("AccountPanel/BindingBtn").GetComponent<Button>(), new UnityAction(this.OnAccountBindingBtnClick));
            base.BindViewCallback(base.view.transform.Find("AccountPanel/SecurityState/Mail/VerifyBtn").GetComponent<Button>(), new UnityAction(this.OnMailVerifyBtnClick));
            base.BindViewCallback(base.view.transform.Find("AccountPanel/SecurityState/Mail/BindingBtn").GetComponent<Button>(), new UnityAction(this.OnMailBindingBtnClick));
            base.BindViewCallback(base.view.transform.Find("AccountPanel/SecurityState/Mobile/BindingBtn").GetComponent<Button>(), new UnityAction(this.OnMobileBindingBtnClick));
            base.BindViewCallback(base.view.transform.Find("AccountPanel/SecurityState/Identity/BindingBtn").GetComponent<Button>(), new UnityAction(this.OnIdentityBindingBtnClick));
            base.BindViewCallback(base.view.transform.Find("EntryButtons/ItempediaButton").GetComponent<Button>(), new UnityAction(this.OnItempediaBtnClick));
            base.BindViewCallback(base.view.transform.Find("EntryButtons/AchievementButton").GetComponent<Button>(), new UnityAction(this.OnAchieveBtnClick));
            base.BindViewCallback(base.view.transform.Find("EntryButtons/CGReplayButton").GetComponent<Button>(), new UnityAction(this.OnCGReplayBtnClick));
            base.BindViewCallback(base.view.transform.Find("AccountPanel/Award/GetAward").GetComponent<Button>(), new UnityAction(this.OnGetRewardBtnClick));
        }

        public void Close()
        {
            this.Destroy();
        }

        public void OnAccountBindingBtnClick()
        {
            Singleton<AccountManager>.Instance.manager.BindUI();
        }

        public void OnAccountBtnClick()
        {
            this._currentTab = "Account";
            this._tabManager.ShowTab(this._currentTab);
        }

        public void OnAchieveBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new AchieveOverviewPageContext(), UIType.Page);
        }

        public void OnCGReplayBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new CGReplayPageContext(), UIType.Page);
        }

        public void OnChangeDescBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new ChangeSelfDescDialogContext(), UIType.Any);
        }

        public void OnChangeNickNameBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new ChangeNicknameDialogContext(), UIType.Any);
        }

        private bool OnGetRedeemCodeInfoRsp(GetRedeemCodeInfoRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new RedeemDialogContext(this._redeemCode, RedeemDialogContext.RedeemStatus.ShowInfo, rsp, null), UIType.Any);
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new RedeemDialogContext(this._redeemCode, RedeemDialogContext.RedeemStatus.Error, rsp, LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])), UIType.Any);
            }
            return false;
        }

        public void OnGetRewardBtnClick()
        {
            this._redeemCode = base.view.transform.Find("AccountPanel/Award/InputField/Text").GetComponent<Text>().text;
            if (this._redeemCode.Length != 10)
            {
                string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput((GetRedeemCodeInfoRsp.Retcode) 2, new object[0]);
                Singleton<MainUIManager>.Instance.ShowDialog(new RedeemDialogContext(this._redeemCode, RedeemDialogContext.RedeemStatus.Error, null, networkErrCodeOutput), UIType.Any);
            }
            else
            {
                Singleton<NetworkManager>.Instance.RequestGetRedeemCodeInfo(this._redeemCode);
                Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(0xd4, null), UIType.Any);
            }
        }

        public void OnIdentityBindingBtnClick()
        {
            string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
            string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
            TheOriginalAccountManager manager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
            if (manager != null)
            {
                WebViewGeneralLogic.LoadUrl(string.Format(manager.ORIGINAL_BIND_IDENTITY_URL + "?uid={0}&token={1}", accountUid, accountToken), false);
            }
        }

        public void OnItempediaBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new ItempediaPageContext(), UIType.Page);
        }

        public void OnLogoutBtnClick()
        {
            GeneralDialogContext dialogContext = new GeneralDialogContext {
                type = GeneralDialogContext.ButtonType.DoubleButton,
                title = LocalizationGeneralLogic.GetText("Menu_Title_Logout", new object[0]),
                desc = LocalizationGeneralLogic.GetText("Menu_Desc_Logout", new object[0])
            };
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = delegate (bool confirmed) {
                    if (confirmed)
                    {
                        Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(), UIType.Any);
                        Singleton<MiHoYoGameData>.Instance.GeneralLocalData.ClearLastLoginUser();
                        Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
                        GeneralLogicManager.RestartGame();
                    }
                };
            }
            dialogContext.buttonCallBack = <>f__am$cache6;
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        public void OnMailBindingBtnClick()
        {
            string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
            string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
            TheOriginalAccountManager manager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
            if (manager != null)
            {
                WebViewGeneralLogic.LoadUrl(string.Format(manager.ORIGINAL_BIND_EMAIL_URL + "?uid={0}&token={1}", accountUid, accountToken), false);
            }
        }

        public void OnMailVerifyBtnClick()
        {
            Singleton<AccountManager>.Instance.manager.VerifyEmailApply();
        }

        public void OnMobileBindingBtnClick()
        {
            string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
            string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
            TheOriginalAccountManager manager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
            if (manager != null)
            {
                WebViewGeneralLogic.LoadUrl(string.Format(manager.ORIGINAL_BIND_MOBILE_URL + "?uid={0}&token={1}", accountUid, accountToken), false);
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.MihoyoAccountInfoChanged)
            {
                this.SetupAccount();
            }
            if (ntf.type == NotifyTypes.MissionUpdated)
            {
                this.SetupCollection();
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 11:
                    return this.SetupView();

                case 120:
                    this.SetupAccount();
                    break;

                case 0xd4:
                    this.OnGetRedeemCodeInfoRsp(pkt.getData<GetRedeemCodeInfoRsp>());
                    break;
            }
            return false;
        }

        public void OnPlayerBtnClick()
        {
            this._currentTab = "Player";
            this._tabManager.ShowTab(this._currentTab);
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            btn.interactable = !active;
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
            btn.transform.Find("Label").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.transform.Find("Icon").GetComponent<Image>().color = !active ? Color.white : MiscData.GetColor("Black");
            go.SetActive(active);
        }

        private bool PopupNoticeAcitve()
        {
            foreach (MissionDataItem item in Singleton<MissionModule>.Instance.GetMissionDict().Values)
            {
                LinearMissionData linearMissionDataByKey = LinearMissionDataReader.GetLinearMissionDataByKey(item.id);
                if (((linearMissionDataByKey != null) && (linearMissionDataByKey.IsAchievement == 1)) && (item.status == 3))
                {
                    return true;
                }
            }
            return false;
        }

        private void SetPopupVisible(bool visible)
        {
            Transform transform = base.view.transform.Find("EntryButtons/AchievementButton/PopUp");
            if (transform != null)
            {
                transform.gameObject.SetActive(visible);
            }
        }

        private void SetupAccount()
        {
            GameObject gameObject = base.view.transform.Find("AccountPanel/SecurityState").gameObject;
            if (Singleton<AccountManager>.Instance.manager is TheOriginalAccountManager)
            {
                gameObject.SetActive(true);
                this.SetupSecurityState();
            }
            else
            {
                gameObject.SetActive(false);
            }
            GameObject obj3 = base.view.transform.Find("ProfilePanel/Credit/Description/Visitor").gameObject;
            GameObject obj4 = base.view.transform.Find("ProfilePanel/Credit/Description/User").gameObject;
            if (!Singleton<AccountManager>.Instance.manager.IsAccountBind())
            {
                obj3.SetActive(true);
                obj4.SetActive(false);
            }
            else
            {
                obj3.SetActive(false);
                obj4.SetActive(true);
            }
            GameObject obj5 = base.view.transform.Find("AccountPanel/LogoutBtn").gameObject;
            GameObject obj6 = base.view.transform.Find("AccountPanel/BindingBtn").gameObject;
            bool flag = !Singleton<AccountManager>.Instance.manager.IsAccountBind();
            obj5.SetActive(!flag);
            obj6.SetActive(flag);
            base.view.transform.Find("AccountPanel/Award/InputField").GetComponent<InputField>().characterLimit = 10;
            base.view.transform.Find("AccountPanel/Award").gameObject.SetActive(!Singleton<NetworkManager>.Instance.DispatchSeverData.isReview);
        }

        private void SetupCollection()
        {
            Text component = base.view.transform.Find("EntryButtons/ItempediaButton/Progress/Current").GetComponent<Text>();
            Text text2 = base.view.transform.Find("EntryButtons/ItempediaButton/Progress/Max").GetComponent<Text>();
            Text text3 = base.view.transform.Find("EntryButtons/AchievementButton/Progress/Current").GetComponent<Text>();
            Text text4 = base.view.transform.Find("EntryButtons/AchievementButton/Progress/Max").GetComponent<Text>();
            Text text5 = base.view.transform.Find("EntryButtons/CGReplayButton/Progress/Current").GetComponent<Text>();
            Text text6 = base.view.transform.Find("EntryButtons/CGReplayButton/Progress/Max").GetComponent<Text>();
            component.text = Singleton<ItempediaModule>.Instance.GetUnlockCountTotal().ToString();
            text2.text = Singleton<ItempediaModule>.Instance.GetItempediaTotalCount().ToString();
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = x => (x.status == 5) || (x.status == 3);
            }
            List<MissionDataItem> list2 = Singleton<MissionModule>.Instance.GetAchievements().FindAll(<>f__am$cache4);
            text3.text = list2.Count.ToString();
            List<int> finishedCGIDList = Singleton<CGModule>.Instance.GetFinishedCGIDList();
            List<int> allCGIDList = Singleton<CGModule>.Instance.GetAllCGIDList();
            text5.text = finishedCGIDList.Count.ToString();
            text6.text = allCGIDList.Count.ToString();
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = delegate (LinearMissionData x) {
                    MissionData missionDataByKey = MissionDataReader.GetMissionDataByKey(x.MissionID);
                    MissionDataItem missionDataItem = Singleton<MissionModule>.Instance.GetMissionDataItem(x.MissionID);
                    bool flag = x.IsAchievement == 1;
                    bool flag2 = (missionDataByKey != null) && (missionDataByKey.NoDisplay == 0);
                    bool flag3 = (missionDataItem != null) && ((missionDataItem.status == 3) || (missionDataItem.status == 5));
                    return flag && (flag2 || flag3);
                };
            }
            List<LinearMissionData> list5 = LinearMissionDataReader.GetItemList().FindAll(<>f__am$cache5);
            text4.text = list5.Count.ToString();
        }

        private void SetupPlayer()
        {
            PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
            int avatarID = 0;
            if (playerData.GetMemberList(1).Count > 0)
            {
                avatarID = playerData.GetMemberList(1)[0];
                AvatarDataItem item2 = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarID);
                base.view.transform.Find("ProfilePanel/Credit/Photo/Avatar").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(item2.IconPath);
                base.view.transform.Find("ProfilePanel/Credit/Photo").GetComponent<Image>().sprite = item2.GetBGSprite();
            }
            base.view.transform.Find("ProfilePanel/Credit/Description/NickName").GetComponent<Text>().text = playerData.NickNameText;
            base.view.transform.Find("ProfilePanel/Credit/Header/IDValue").GetComponent<Text>().text = playerData.userId.ToString();
            base.view.transform.Find("ProfilePanel/Credit/Header/LevelText").GetComponent<Text>().text = "Lv." + playerData.teamLevel.ToString();
            base.view.transform.Find("ProfilePanel/Exp/ExpText").GetComponent<Text>().text = playerData.teamExp + " / " + playerData.TeamMaxExp;
            base.view.transform.Find("ProfilePanel/Credit/Description/Intro").GetComponent<Text>().text = playerData.SelfDescText;
            base.view.transform.Find("ProfilePanel/Exp/ExpBar").GetComponent<MonoSliderGroup>().UpdateValue((float) playerData.teamExp, (float) playerData.TeamMaxExp, 0f);
            AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarID);
            base.view.transform.Find("AvatarCard/Avatar").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(Singleton<StorageModule>.Instance.GetDummyStorageDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(avatarByID.avatarID).avatarCardID, 1).GetImagePath());
            for (int i = 1; i <= 3; i++)
            {
                base.view.transform.Find("ProfilePanel/Credit/Bottom/Attr" + i.ToString()).gameObject.SetActive(i == avatarByID.Attribute);
            }
            this._avatarStar = base.view.transform.Find("ProfilePanel/Credit/Bottom/Stars").GetComponent<MonoAvatarStar>();
            if (this._avatarStar != null)
            {
                this._avatarStar.SetupView(avatarByID.star);
            }
        }

        private void SetupSecurityState()
        {
            GeneralLocalDataItem.AccountData account = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account;
            GameObject gameObject = base.view.transform.Find("AccountPanel/SecurityState/Mail/Content").gameObject;
            GameObject obj3 = base.view.transform.Find("AccountPanel/SecurityState/Mail/ContentWait").gameObject;
            GameObject obj4 = base.view.transform.Find("AccountPanel/SecurityState/Mail/ContentNo").gameObject;
            GameObject obj5 = base.view.transform.Find("AccountPanel/SecurityState/Mail/MarkYes").gameObject;
            GameObject obj6 = base.view.transform.Find("AccountPanel/SecurityState/Mail/MarkNo").gameObject;
            GameObject obj7 = base.view.transform.Find("AccountPanel/SecurityState/Mail/MarkCautious").gameObject;
            GameObject obj8 = base.view.transform.Find("AccountPanel/SecurityState/Mail/VerifyBtn").gameObject;
            GameObject obj9 = base.view.transform.Find("AccountPanel/SecurityState/Mail/BindingBtn").gameObject;
            bool flag = Singleton<AccountManager>.Instance.manager.IsAccountBind() && string.IsNullOrEmpty(account.email);
            bool flag2 = Singleton<AccountManager>.Instance.manager.IsAccountBind() && string.IsNullOrEmpty(account.email);
            obj8.SetActive(flag);
            obj9.SetActive(flag2);
            if (string.IsNullOrEmpty(account.email))
            {
                gameObject.SetActive(false);
                obj3.SetActive(false);
                obj4.SetActive(true);
                obj5.SetActive(false);
                obj6.SetActive(true);
                obj7.SetActive(false);
            }
            else if (account.isEmailVerify)
            {
                gameObject.SetActive(true);
                obj3.SetActive(false);
                obj4.SetActive(false);
                obj5.SetActive(true);
                obj6.SetActive(false);
                obj7.SetActive(false);
                gameObject.GetComponent<Text>().text = account.email;
            }
            else
            {
                gameObject.SetActive(false);
                obj3.SetActive(true);
                obj4.SetActive(false);
                obj5.SetActive(false);
                obj6.SetActive(false);
                obj7.SetActive(true);
                obj8.SetActive(true);
                obj9.SetActive(false);
            }
            if (obj8.activeSelf)
            {
                obj8.GetComponent<Button>().interactable = !Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasSendVerifyEmailApply;
            }
            gameObject = base.view.transform.Find("AccountPanel/SecurityState/Mobile/Content").gameObject;
            obj3 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/ContentWait").gameObject;
            obj4 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/ContentNo").gameObject;
            obj5 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/MarkYes").gameObject;
            obj6 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/MarkNo").gameObject;
            obj7 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/MarkCautious").gameObject;
            obj9 = base.view.transform.Find("AccountPanel/SecurityState/Mobile/BindingBtn").gameObject;
            obj3.SetActive(false);
            flag2 = Singleton<AccountManager>.Instance.manager.IsAccountBind() && string.IsNullOrEmpty(account.mobile);
            obj9.SetActive(flag2);
            if (string.IsNullOrEmpty(account.mobile))
            {
                gameObject.SetActive(false);
                obj4.SetActive(true);
                obj5.SetActive(false);
                obj6.SetActive(true);
                obj7.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                obj4.SetActive(false);
                obj5.SetActive(true);
                obj6.SetActive(false);
                obj7.SetActive(false);
                gameObject.GetComponent<Text>().text = account.mobile;
            }
            gameObject = base.view.transform.Find("AccountPanel/SecurityState/Identity/Content").gameObject;
            obj3 = base.view.transform.Find("AccountPanel/SecurityState/Identity/ContentWait").gameObject;
            obj4 = base.view.transform.Find("AccountPanel/SecurityState/Identity/ContentNo").gameObject;
            obj5 = base.view.transform.Find("AccountPanel/SecurityState/Identity/MarkYes").gameObject;
            obj6 = base.view.transform.Find("AccountPanel/SecurityState/Identity/MarkNo").gameObject;
            obj7 = base.view.transform.Find("AccountPanel/SecurityState/Identity/MarkCautious").gameObject;
            obj9 = base.view.transform.Find("AccountPanel/SecurityState/Identity/BindingBtn").gameObject;
            obj3.SetActive(false);
            obj7.SetActive(false);
            flag2 = Singleton<AccountManager>.Instance.manager.IsAccountBind() && !account.isRealNameVerify;
            obj9.SetActive(flag2);
            if (!account.isRealNameVerify)
            {
                gameObject.SetActive(false);
                obj4.SetActive(true);
                obj5.SetActive(false);
                obj6.SetActive(true);
            }
            else
            {
                gameObject.SetActive(true);
                obj4.SetActive(false);
                obj5.SetActive(true);
                obj6.SetActive(false);
            }
        }

        private void SetupTabs()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = !string.IsNullOrEmpty(showingTabKey) ? showingTabKey : this._currentTab;
            this._tabManager.Clear();
            GameObject content = null;
            Button btn = null;
            content = base.view.transform.Find("ProfilePanel").gameObject;
            btn = base.view.transform.Find("TabButtons/PlayerButton").GetComponent<Button>();
            this._tabManager.SetTab("Player", btn, content);
            content = base.view.transform.Find("AccountPanel").gameObject;
            btn = base.view.transform.Find("TabButtons/AccountButton").GetComponent<Button>();
            this._tabManager.SetTab("Account", btn, content);
            this._tabManager.ShowTab(searchKey);
        }

        protected override bool SetupView()
        {
            this.SetupPlayer();
            this.SetupAccount();
            this.SetupTabs();
            this.SetupCollection();
            this.SetPopupVisible(this.PopupNoticeAcitve());
            return false;
        }

        public enum TabType
        {
            PlayerTab,
            AccountTab
        }
    }
}

