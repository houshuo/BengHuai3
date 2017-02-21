namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class FriendOverviewPageContext : BasePageContext
    {
        private InviteTab _currentInviteTab;
        private string _defaultTabKey;
        private Dictionary<GameObject, MonoScrollerFadeManager> _fadeManagerDict;
        private GetInviteeFriendRsp _inviteeInfo;
        private GetInviteFriendRsp _inviterInfo;
        private Dictionary<GameObject, Dictionary<int, RectTransform>> _itemBeforeDict;
        private int _playerUidToShow;
        private Dictionary<GameObject, MonoGridScroller> _scrollerDict;
        private bool _shouldMarkAllFriendsAsOld;
        private Dictionary<string, List<FriendBriefDataItem>> _tabItemList;
        private TabManager _tabManager;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, int> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, RectTransform> <>f__am$cache11;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, int> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, RectTransform> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, int> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, RectTransform> <>f__am$cacheF;
        public static readonly string[] TAB_KEY = new string[] { "FriendListTab", "AddFriendTab", "RequestListTab", "InviteCodeTab" };

        public FriendOverviewPageContext(string tabKey = "FriendListTab", InviteTab inviteTab = 1)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "FriendOverviewPageContext",
                viewPrefabPath = "UI/Menus/Page/Friend/FriendOverviewPage"
            };
            base.config = pattern;
            this._tabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
            this._defaultTabKey = tabKey;
            this._currentInviteTab = inviteTab;
            this._tabItemList = new Dictionary<string, List<FriendBriefDataItem>>();
            this._playerUidToShow = -1;
            this._shouldMarkAllFriendsAsOld = false;
            this._currentInviteTab = InviteTab.InviteeTab;
        }

        public override void BackPage()
        {
            if (this._shouldMarkAllFriendsAsOld)
            {
                Singleton<FriendModule>.Instance.MarkAllFriendsAsOld();
            }
            base.BackPage();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), new UnityAction(this.OnFriendListTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), new UnityAction(this.OnAddFriendTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), new UnityAction(this.OnRequestListTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_4").GetComponent<Button>(), new UnityAction(this.OnInviteCodeBtnClick));
            base.BindViewCallback(base.view.transform.Find("InviteCodeTab/Tab/Tab_1").GetComponent<Button>(), new UnityAction(this.OnInviteeTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("InviteCodeTab/Tab/Tab_2").GetComponent<Button>(), new UnityAction(this.OnInviterTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("FriendListTab/SortBtn").GetComponent<Button>(), new UnityAction(this.OnSortBtnClick));
            base.BindViewCallback(base.view.transform.Find("FriendListTab/SortPanel/BG").GetComponent<Button>(), new UnityAction(this.OnSortBGClick));
            base.BindViewCallback(base.view.transform.Find("AddFriendTab/RefreshBtn").GetComponent<Button>(), new UnityAction(this.OnAddRefreshBtnClick));
            base.BindViewCallback(base.view.transform.Find("AddFriendTab/SearchBtn").GetComponent<Button>(), new UnityAction(this.OnSearchBtnClick));
            base.BindViewCallback(base.view.transform.Find("InviteCodeTab/MyInvitationCode/InviteCode/BtnCopy").GetComponent<Button>(), new UnityAction(this.OnMyInviteCopyBtnClick));
            base.BindViewCallback(base.view.transform.Find("InviteCodeTab/InputInvitationCode/InviteCode/BtnOk").GetComponent<Button>(), new UnityAction(this.OnAcceptInviteBtnClick));
        }

        public override void Destroy()
        {
            if (this._shouldMarkAllFriendsAsOld)
            {
                Singleton<FriendModule>.Instance.MarkAllFriendsAsOld();
            }
            base.Destroy();
        }

        private void InitScroller()
        {
            this._scrollerDict = new Dictionary<GameObject, MonoGridScroller>();
            this._scrollerDict[base.view.transform.Find("FriendListTab").gameObject] = base.view.transform.Find("FriendListTab/ScrollView").GetComponent<MonoGridScroller>();
            this._scrollerDict[base.view.transform.Find("AddFriendTab").gameObject] = base.view.transform.Find("AddFriendTab/ScrollView").GetComponent<MonoGridScroller>();
            this._scrollerDict[base.view.transform.Find("RequestListTab").gameObject] = base.view.transform.Find("RequestListTab/ScrollView").GetComponent<MonoGridScroller>();
            this._fadeManagerDict = new Dictionary<GameObject, MonoScrollerFadeManager>();
            this._fadeManagerDict[base.view.transform.Find("FriendListTab").gameObject] = base.view.transform.Find("FriendListTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
            this._fadeManagerDict[base.view.transform.Find("AddFriendTab").gameObject] = base.view.transform.Find("AddFriendTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
            this._fadeManagerDict[base.view.transform.Find("RequestListTab").gameObject] = base.view.transform.Find("RequestListTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
            this._itemBeforeDict = new Dictionary<GameObject, Dictionary<int, RectTransform>>();
            this._itemBeforeDict[base.view.transform.Find("FriendListTab").gameObject] = null;
            this._itemBeforeDict[base.view.transform.Find("AddFriendTab").gameObject] = null;
            this._itemBeforeDict[base.view.transform.Find("RequestListTab").gameObject] = null;
        }

        private bool IsFriendDataEqual(RectTransform friendNew, RectTransform friendOld)
        {
            if ((friendNew == null) || (friendOld == null))
            {
                return false;
            }
            MonoFriendInfo component = friendOld.GetComponent<MonoFriendInfo>();
            return (friendNew.GetComponent<MonoFriendInfo>().GetFriendUID() == component.GetFriendUID());
        }

        private void OnAcceptBtnClick(FriendBriefDataItem friendBriefData)
        {
            Singleton<FriendModule>.Instance.RecordRequestAddFriend(friendBriefData.uid);
            Singleton<FriendModule>.Instance.RemoveFriendInfo(FriendTab.RequestListTab, friendBriefData.uid);
            this.SetupRequestListTab();
            this.PlayCurrentTabAnim();
            Singleton<NetworkManager>.Instance.RequestAgreeFriend(friendBriefData.uid);
        }

        private void OnAcceptInviteBtnClick()
        {
            string str = base.view.transform.Find("InviteCodeTab/InputInvitationCode/InviteCode/InputField").GetComponent<InputField>().text.Trim();
            if (!string.IsNullOrEmpty(str))
            {
                str = str.ToUpper();
                Singleton<NetworkManager>.Instance.RequestGetAcceptFriendInvite(str);
            }
        }

        private bool OnAddFriendRsp(AddFriendRsp rsp)
        {
            int targetUid = (int) rsp.get_target_uid();
            string desc = string.Empty;
            switch (rsp.get_retcode())
            {
                case 0:
                    switch (rsp.get_action())
                    {
                        case 1:
                        {
                            object[] replaceParams = new object[] { Singleton<FriendModule>.Instance.TryGetPlayerNickName(targetUid) };
                            desc = LocalizationGeneralLogic.GetText("Menu_Desc_RequestAddFriend", replaceParams);
                            break;
                        }
                        case 2:
                        {
                            object[] objArray1 = new object[] { Singleton<FriendModule>.Instance.TryGetPlayerNickName(targetUid) };
                            desc = LocalizationGeneralLogic.GetText("Menu_Desc_AgreeFriend", objArray1);
                            break;
                        }
                        case 3:
                        {
                            object[] objArray2 = new object[] { Singleton<FriendModule>.Instance.TryGetPlayerNickName(targetUid) };
                            desc = LocalizationGeneralLogic.GetText("Menu_Desc_RejectFriend", objArray2);
                            break;
                        }
                    }
                    goto Label_019E;

                case 1:
                    desc = LocalizationGeneralLogic.GetText("Err_FailToAddFriend", new object[0]);
                    goto Label_019E;

                case 3:
                    desc = LocalizationGeneralLogic.GetText("Err_FriendFull", new object[0]);
                    goto Label_019E;

                case 4:
                    desc = LocalizationGeneralLogic.GetText("Err_TargetFriendFull", new object[0]);
                    goto Label_019E;

                case 5:
                    desc = LocalizationGeneralLogic.GetText("Err_IsSelf", new object[0]);
                    goto Label_019E;

                case 6:
                    desc = LocalizationGeneralLogic.GetText("Err_IsFriend", new object[0]);
                    goto Label_019E;

                case 7:
                    desc = LocalizationGeneralLogic.GetText("Err_AskTooOften", new object[0]);
                    goto Label_019E;

                case 8:
                    desc = LocalizationGeneralLogic.GetText("Err_TargetAskListFull", new object[0]);
                    goto Label_019E;

                case 9:
                    desc = LocalizationGeneralLogic.GetText("Err_TargetInAskList", new object[0]);
                    goto Label_019E;
            }
            desc = LocalizationGeneralLogic.GetText("Err_FailToAddFriend", new object[0]);
        Label_019E:
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(desc, 2f), UIType.Any);
            return false;
        }

        private void OnAddFriendTabBtnClick()
        {
            if (this._shouldMarkAllFriendsAsOld)
            {
                Singleton<FriendModule>.Instance.MarkAllFriendsAsOld();
            }
            this._tabManager.ShowTab("AddFriendTab");
        }

        private void OnAddRefreshBtnClick()
        {
            Singleton<NetworkManager>.Instance.RequestRecommandFriendList();
        }

        private bool OnDelFriendRsp(DelFriendRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                object[] replaceParams = new object[] { Singleton<FriendModule>.Instance.TryGetPlayerNickName((int) rsp.get_target_uid()) };
                string text = LocalizationGeneralLogic.GetText("Menu_Desc_DeleteFriend", replaceParams);
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(text, 2f), UIType.Any);
                this.SetupFriendListTab();
            }
            return false;
        }

        private void OnDetailBtnClick(FriendBriefDataItem friendBriefData)
        {
            Singleton<FriendModule>.Instance.MarkFriendAsOld(friendBriefData.uid);
            FriendDetailDataItem detailData = Singleton<FriendModule>.Instance.TryGetFriendDetailData(friendBriefData.uid);
            if (detailData == null)
            {
                this._playerUidToShow = friendBriefData.uid;
                Singleton<NetworkManager>.Instance.RequestFriendDetailInfo(friendBriefData.uid);
            }
            else
            {
                this.ShowFriendDetailInfo(detailData);
            }
        }

        private void OnFriendListTabBtnClick()
        {
            if (!this._shouldMarkAllFriendsAsOld)
            {
                this._shouldMarkAllFriendsAsOld = true;
            }
            base.view.transform.Find("TabBtns/TabBtn_1/PopUp").gameObject.SetActive(false);
            this._tabManager.ShowTab("FriendListTab");
        }

        private bool OnGetAcceptFriendInviteRsp(AcceptFriendInviteRsp rsp)
        {
            if (MiscData.Config.BasicConfig.IsInviteFeatureEnable)
            {
                if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
                {
                    return false;
                }
                if (!Singleton<AccountManager>.Instance.manager.IsAccountBind())
                {
                    return false;
                }
                if (rsp.get_retcode() == null)
                {
                    Singleton<NetworkManager>.Instance.RequestGetInviteeFriend();
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_InviteInputSuccess", new object[0]), 2f), UIType.Any);
                }
                else
                {
                    GeneralDialogContext dialogContext = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.SingleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                        desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]),
                        notDestroyAfterTouchBG = true
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
            }
            return false;
        }

        private bool OnGetInviteeFriendRsp(GetInviteeFriendRsp rsp)
        {
            if (MiscData.Config.BasicConfig.IsInviteFeatureEnable)
            {
                if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
                {
                    return false;
                }
                if (!Singleton<AccountManager>.Instance.manager.IsAccountBind())
                {
                    return false;
                }
                if (rsp.get_retcode() == null)
                {
                    this._inviteeInfo = rsp;
                    this.SetupInviteeTabUI();
                    Transform transform = base.view.transform.Find("InviteCodeTab/InputInvitationCode");
                    transform.gameObject.SetActive(true);
                    base.view.transform.Find("InviteCodeTab/MyInvitationCode").gameObject.SetActive(false);
                    base.view.transform.Find("InviteCodeTab/Tab/Tab_1").gameObject.SetActive(true);
                    InputField component = transform.Find("InviteCode/InputField").GetComponent<InputField>();
                    int maxLevelToAcceptInvite = Singleton<PlayerModule>.Instance.playerData.maxLevelToAcceptInvite;
                    object[] replaceParams = new object[] { maxLevelToAcceptInvite };
                    component.transform.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_InvitationCode_effective", replaceParams);
                    if (rsp.get_invitee_codeSpecified())
                    {
                        component.text = rsp.get_invitee_code();
                        component.interactable = false;
                        transform.Find("InviteCode/BtnOk").gameObject.SetActive(false);
                        transform.Find("InviteCode/Used").gameObject.SetActive(true);
                    }
                    else if (Singleton<PlayerModule>.Instance.playerData.teamLevel >= Singleton<PlayerModule>.Instance.playerData.maxLevelToAcceptInvite)
                    {
                        base.view.transform.Find("InviteCodeTab/Tab/Tab_1").gameObject.SetActive(false);
                        this._currentInviteTab = InviteTab.InviterTab;
                        transform.gameObject.SetActive(false);
                    }
                    else
                    {
                        component.interactable = true;
                        transform.Find("InviteCode/BtnOk").gameObject.SetActive(true);
                        transform.Find("InviteCode/Used").gameObject.SetActive(false);
                    }
                    transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(this.OnInviteeRewardScrollChange), rsp.get_invitee_reward_list().Count, new Vector2(0f, 1f));
                }
                else
                {
                    base.view.transform.Find("InviteCodeTab/Tab/Tab_1").gameObject.SetActive(false);
                    this._currentInviteTab = InviteTab.InviterTab;
                    base.view.transform.Find("InviteCodeTab/InputInvitationCode").gameObject.SetActive(false);
                }
            }
            return false;
        }

        private bool OnGetInviteFriendRsp(GetInviteFriendRsp rsp)
        {
            if (MiscData.Config.BasicConfig.IsInviteFeatureEnable)
            {
                if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
                {
                    return false;
                }
                if (!Singleton<AccountManager>.Instance.manager.IsAccountBind())
                {
                    return false;
                }
                if (rsp.get_retcode() == null)
                {
                    this._inviterInfo = rsp;
                    this.SetupInviterTabUI();
                    base.view.transform.Find("InviteCodeTab/InputInvitationCode").gameObject.SetActive(false);
                    base.view.transform.Find("InviteCodeTab/MyInvitationCode").gameObject.SetActive(true);
                    base.view.transform.Find("InviteCodeTab/Tab/Tab_2").gameObject.SetActive(true);
                    Transform transform = base.view.transform.Find("InviteCodeTab/MyInvitationCode");
                    transform.Find("InviteCode/InputField/Text").GetComponent<Text>().text = rsp.get_my_invite_code().ToString();
                    transform.Find("InviteCode/HaveInvited/Num").GetComponent<Text>().text = rsp.get_has_invite_num().ToString();
                    transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(this.OnInviterRewardScrollChange), rsp.get_my_invite_reward_list().Count, new Vector2(0f, 1f));
                }
                else
                {
                    base.view.transform.Find("InviteCodeTab/Tab/Tab_2").gameObject.SetActive(false);
                    this._currentInviteTab = InviteTab.InviteeTab;
                    base.view.transform.Find("InviteCodeTab/MyInvitationCode").gameObject.SetActive(false);
                }
            }
            return false;
        }

        private bool OnGetRecommandListRsp(GetRecommendFriendListRsp rsp)
        {
            switch (rsp.get_retcode())
            {
                case 0:
                    this.SetupAddFriendTab();
                    break;

                case 2:
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_RefreshToOften", new object[0]), 2f), UIType.Any);
                    break;
            }
            return false;
        }

        private void OnInviteCodeBtnClick()
        {
            if (this._shouldMarkAllFriendsAsOld)
            {
                Singleton<FriendModule>.Instance.MarkAllFriendsAsOld();
            }
            this._tabManager.ShowTab("InviteCodeTab");
        }

        private void OnInviteeRewardScrollChange(Transform itemTrans, int index)
        {
            InviteeFriendRewardData rewardData = this._inviteeInfo.get_invitee_reward_list()[index];
            itemTrans.GetComponent<MonoInviteRewardRow>().SetupView(this._inviteeInfo.get_invitee_codeSpecified(), rewardData);
        }

        private void OnInviteeTabBtnClick()
        {
            this._currentInviteTab = InviteTab.InviteeTab;
            this.SetupInviteeTabUI();
            Singleton<NetworkManager>.Instance.RequestGetInviteeFriend();
        }

        private void OnInviterRewardScrollChange(Transform itemTrans, int index)
        {
            InviteFriendRewardData rewardData = this._inviterInfo.get_my_invite_reward_list()[index];
            itemTrans.GetComponent<MonoInviteRewardRow>().SetupView(rewardData);
        }

        private void OnInviterTabBtnClick()
        {
            this._currentInviteTab = InviteTab.InviteeTab;
            this.SetupInviterTabUI();
            Singleton<NetworkManager>.Instance.RequestGetInviteFriend();
        }

        public override void OnLandedFromBackPage()
        {
            this.SetupView();
            base.OnLandedFromBackPage();
        }

        private void OnMyInviteCopyBtnClick()
        {
            ClipboardManager.CopyToClipboard(base.view.transform.Find("InviteCodeTab/MyInvitationCode/InviteCode/InputField/Text").GetComponent<Text>().text.Trim());
            this.ShowMyInviteCodeCopySuccessHint();
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.SetFriendSortType) && this.OnSetSortType((FriendModule.FriendSortType) ((int) ntf.body)));
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x41:
                case 80:
                    this.SetupFriendListTab();
                    break;

                case 0x4d:
                    this.OnGetRecommandListRsp(pkt.getData<GetRecommendFriendListRsp>());
                    this.PlayCurrentTabAnim();
                    break;

                case 0x47:
                    this.SetupRequestListTab();
                    this.PlayCurrentTabAnim();
                    break;

                case 0x49:
                    this.OnPlayerDetailRsp(pkt.getData<GetPlayerDetailDataRsp>());
                    break;

                case 0x43:
                    this.OnAddFriendRsp(pkt.getData<AddFriendRsp>());
                    break;

                case 0x45:
                    this.OnDelFriendRsp(pkt.getData<DelFriendRsp>());
                    break;

                case 0xe2:
                    this.OnGetInviteeFriendRsp(pkt.getData<GetInviteeFriendRsp>());
                    break;

                case 0xe0:
                    this.OnGetInviteFriendRsp(pkt.getData<GetInviteFriendRsp>());
                    break;

                case 0xe4:
                    this.OnGetAcceptFriendInviteRsp(pkt.getData<AcceptFriendInviteRsp>());
                    break;
            }
            return false;
        }

        private bool OnPlayerDetailRsp(GetPlayerDetailDataRsp rsp)
        {
            if ((rsp.get_retcode() == null) && (this._playerUidToShow == rsp.get_detail().get_uid()))
            {
                this._playerUidToShow = -1;
                FriendDetailDataItem detailData = new FriendDetailDataItem(rsp.get_detail());
                return this.ShowFriendDetailInfo(detailData);
            }
            return false;
        }

        private void OnRejectBtnClick(FriendBriefDataItem friendBriefData)
        {
            Singleton<FriendModule>.Instance.RemoveFriendInfo(FriendTab.RequestListTab, friendBriefData.uid);
            this.SetupRequestListTab();
            this.PlayCurrentTabAnim();
            Singleton<NetworkManager>.Instance.RequestRejectFriend(friendBriefData.uid);
        }

        private void OnRequestBtnClick(FriendBriefDataItem friendBriefData)
        {
            Singleton<FriendModule>.Instance.RecordRequestAddFriend(friendBriefData.uid);
            Singleton<FriendModule>.Instance.RemoveFriendInfo(FriendTab.AddFriendTab, friendBriefData.uid);
            this.SetupAddFriendTab();
            this.PlayCurrentTabAnim();
            Singleton<NetworkManager>.Instance.RequestAddFriend(friendBriefData.uid);
        }

        private void OnRequestListTabBtnClick()
        {
            if (this._shouldMarkAllFriendsAsOld)
            {
                Singleton<FriendModule>.Instance.MarkAllFriendsAsOld();
            }
            Singleton<FriendModule>.Instance.MarkAllRequestsAsOld();
            base.view.transform.Find("TabBtns/TabBtn_3/PopUp").gameObject.SetActive(false);
            this._tabManager.ShowTab("RequestListTab");
        }

        private void OnScrollerChange(string key, List<FriendBriefDataItem> list, Transform trans, int index)
        {
            FriendBriefDataItem friendBriefData = list[index];
            trans.GetComponent<MonoFriendInfo>().SetupView(friendBriefData, key.ToEnum<FriendTab>(FriendTab.FriendListTab), new RequestCallBack(this.OnRequestBtnClick), new AcceptCallBack(this.OnAcceptBtnClick), new RejectCallBack(this.OnRejectBtnClick), new DetailCallBack(this.OnDetailBtnClick));
        }

        private void OnSearchBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new SearchFriendDialogContext(), UIType.Any);
        }

        private bool OnSetSortType(FriendModule.FriendSortType sortType)
        {
            this.SetupSortView(true);
            this.SetupScrollView(this._tabManager.GetShowingTabKey(), this._tabManager.GetShowingTabContent());
            this.PlayCurrentTabAnim();
            return false;
        }

        private void OnSortBGClick()
        {
            this.SetupSortView(false);
        }

        private void OnSortBtnClick()
        {
            this.SetupSortView(true);
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
            btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.transform.Find("Image").GetComponent<Image>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.interactable = !active;
            go.SetActive(active);
            if (active && this._fadeManagerDict.ContainsKey(go))
            {
                this._fadeManagerDict[go].Init(this._scrollerDict[go].GetItemDict(), this._itemBeforeDict[go], new Func<RectTransform, RectTransform, bool>(this.IsFriendDataEqual));
                this._fadeManagerDict[go].Play();
                this._itemBeforeDict[go] = null;
            }
        }

        private void PlayCurrentTabAnim()
        {
            GameObject showingTabContent = this._tabManager.GetShowingTabContent();
            if (this._scrollerDict.ContainsKey(showingTabContent))
            {
                this._fadeManagerDict[showingTabContent].Init(this._scrollerDict[showingTabContent].GetItemDict(), this._itemBeforeDict[showingTabContent], new Func<RectTransform, RectTransform, bool>(this.IsFriendDataEqual));
                this._fadeManagerDict[showingTabContent].Play();
                this._itemBeforeDict[showingTabContent] = null;
            }
        }

        private void SetupAddFriendTab()
        {
            GameObject gameObject = base.view.transform.Find("AddFriendTab").gameObject;
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = entry => entry.Key;
            }
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = entry => entry.Value;
            }
            this._itemBeforeDict[gameObject] = Enumerable.ToDictionary<KeyValuePair<int, RectTransform>, int, RectTransform>(this._scrollerDict[gameObject].GetItemDict(), <>f__am$cacheE, <>f__am$cacheF);
            List<FriendBriefDataItem> list = new List<FriendBriefDataItem>();
            list.AddRange(Singleton<FriendModule>.Instance.GetRecommandFriendList());
            this.SetupTab("AddFriendTab", base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), base.view.transform.Find("AddFriendTab").gameObject, list);
        }

        private void SetupFriendListTab()
        {
            GameObject gameObject = base.view.transform.Find("RequestListTab").gameObject;
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = entry => entry.Key;
            }
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = entry => entry.Value;
            }
            this._itemBeforeDict[gameObject] = Enumerable.ToDictionary<KeyValuePair<int, RectTransform>, int, RectTransform>(this._scrollerDict[gameObject].GetItemDict(), <>f__am$cacheC, <>f__am$cacheD);
            List<FriendBriefDataItem> list = new List<FriendBriefDataItem>();
            list.AddRange(Singleton<FriendModule>.Instance.friendsList);
            base.view.transform.Find("TabBtns/TabBtn_1/PopUp").gameObject.SetActive(Singleton<FriendModule>.Instance.HasNewFriend());
            this.SetupTab("FriendListTab", base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), base.view.transform.Find("FriendListTab").gameObject, list);
            this.SetupFriendNumView();
            this.SetupSortView(false);
        }

        private void SetupFriendNumView()
        {
            int count = Singleton<FriendModule>.Instance.friendsList.Count;
            int maxFriendFinal = Singleton<PlayerModule>.Instance.playerData.maxFriendFinal;
            base.view.transform.Find("FriendListTab/FriendNum/CurNum").GetComponent<Text>().text = count.ToString();
            Text component = base.view.transform.Find("FriendListTab/FriendNum/MaxNum").GetComponent<Text>();
            component.text = maxFriendFinal.ToString();
            component.color = (count <= maxFriendFinal) ? Color.white : Color.red;
        }

        private void SetupInviteCodeTab()
        {
            bool isInviteFeatureEnable = MiscData.Config.BasicConfig.IsInviteFeatureEnable;
            bool isReview = Singleton<NetworkManager>.Instance.DispatchSeverData.isReview;
            bool flag3 = Singleton<AccountManager>.Instance.manager.IsAccountBind();
            bool flag4 = (isInviteFeatureEnable && !isReview) && flag3;
            base.view.transform.Find("InviteCodeTab").gameObject.SetActive(flag4);
            base.view.transform.Find("TabBtns/TabBtn_4").gameObject.SetActive(flag4);
            if (!isReview && flag3)
            {
                base.view.transform.Find("InviteCodeTab/Tab/Tab_1").gameObject.SetActive(false);
                base.view.transform.Find("InviteCodeTab/Tab/Tab_2").gameObject.SetActive(false);
                base.view.transform.Find("InviteCodeTab/InputInvitationCode").gameObject.SetActive(false);
                base.view.transform.Find("InviteCodeTab/MyInvitationCode").gameObject.SetActive(false);
                this.SetupInviteCodeTab("InviteCodeTab", base.view.transform.Find("TabBtns/TabBtn_4").GetComponent<Button>(), base.view.transform.Find("InviteCodeTab").gameObject, "some string as invite code");
                Singleton<NetworkManager>.Instance.RequestGetInviteeFriend();
                Singleton<NetworkManager>.Instance.RequestGetInviteFriend();
            }
        }

        private void SetupInviteCodeTab(string key, Button tabBtn, GameObject tabGo, string inviteCode)
        {
            this._tabManager.SetTab(key, tabBtn, tabGo);
            bool flag = Singleton<PlayerModule>.Instance.playerData.teamLevel < 10;
            base.view.transform.Find("InviteCodeTab/Tab/Tab_2").gameObject.SetActive(flag);
        }

        private void SetupInviteeTabUI()
        {
            GameObject gameObject = base.view.transform.Find("InviteCodeTab").gameObject;
            gameObject.transform.Find("Tab/Tab_1").GetComponent<Button>().interactable = false;
            gameObject.transform.Find("Tab/Tab_2").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("Tab/Tab_1/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
            gameObject.transform.Find("Tab/Tab_2/Text").GetComponent<Text>().color = Color.white;
            gameObject.transform.Find("InputInvitationCode").gameObject.SetActive(false);
            gameObject.transform.Find("MyInvitationCode").gameObject.SetActive(false);
        }

        private void SetupInviterTabUI()
        {
            GameObject gameObject = base.view.transform.Find("InviteCodeTab").gameObject;
            gameObject.transform.Find("Tab/Tab_1").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("Tab/Tab_2").GetComponent<Button>().interactable = false;
            gameObject.transform.Find("Tab/Tab_1/Text").GetComponent<Text>().color = Color.white;
            gameObject.transform.Find("Tab/Tab_2/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
            gameObject.transform.Find("InputInvitationCode").gameObject.SetActive(false);
            gameObject.transform.Find("MyInvitationCode").gameObject.SetActive(false);
        }

        private void SetupRequestListTab()
        {
            GameObject gameObject = base.view.transform.Find("RequestListTab").gameObject;
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = entry => entry.Key;
            }
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = entry => entry.Value;
            }
            this._itemBeforeDict[gameObject] = Enumerable.ToDictionary<KeyValuePair<int, RectTransform>, int, RectTransform>(this._scrollerDict[gameObject].GetItemDict(), <>f__am$cache10, <>f__am$cache11);
            List<FriendBriefDataItem> list = new List<FriendBriefDataItem>();
            list.AddRange(Singleton<FriendModule>.Instance.askingList);
            base.view.transform.Find("TabBtns/TabBtn_3/PopUp").gameObject.SetActive(Singleton<FriendModule>.Instance.HasNewRequest());
            this.SetupTab("RequestListTab", base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), base.view.transform.Find("RequestListTab").gameObject, list);
        }

        private void SetupScrollView(string key, GameObject tabGo)
        {
            <SetupScrollView>c__AnonStoreyE9 ye = new <SetupScrollView>c__AnonStoreyE9 {
                key = key,
                <>f__this = this
            };
            FriendModule.FriendSortType type = Singleton<FriendModule>.Instance.sortTypeMap[ye.key];
            this._tabItemList[ye.key].Sort(Singleton<FriendModule>.Instance.sortComparisionMap[type]);
            tabGo.transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(ye.<>m__12C), this._tabItemList[ye.key].Count, null);
        }

        private void SetupSortView(bool sortActive)
        {
            base.view.transform.Find("FriendListTab/SortPanel").gameObject.SetActive(sortActive);
            base.view.transform.Find("FriendListTab/SortBtn").GetComponent<Button>().interactable = !sortActive;
            if (sortActive)
            {
                foreach (MonoFriendSortButton button in base.view.transform.Find("FriendListTab/SortPanel/Content").GetComponentsInChildren<MonoFriendSortButton>())
                {
                    if (button.gameObject.activeSelf)
                    {
                        button.SetupView(this._tabManager.GetShowingTabKey());
                    }
                }
            }
        }

        private void SetupTab(string key, Button tabBtn, GameObject tabGo, List<FriendBriefDataItem> list)
        {
            if (this._tabItemList.ContainsKey(key))
            {
                this._tabItemList[key] = list;
            }
            else
            {
                this._tabItemList.Add(key, list);
            }
            this.SetupScrollView(key, tabGo);
            this._tabManager.SetTab(key, tabBtn, tabGo);
        }

        protected override bool SetupView()
        {
            this.InitScroller();
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = !string.IsNullOrEmpty(showingTabKey) ? showingTabKey : this._defaultTabKey;
            this.SetupFriendListTab();
            this.SetupAddFriendTab();
            this.SetupRequestListTab();
            this.SetupInviteCodeTab();
            this._tabManager.ShowTab(searchKey);
            if ((searchKey == "FriendListTab") && !this._shouldMarkAllFriendsAsOld)
            {
                this._shouldMarkAllFriendsAsOld = true;
            }
            return false;
        }

        private bool ShowFriendDetailInfo(FriendDetailDataItem detailData)
        {
            RemoteAvatarDetailPageContext context = new RemoteAvatarDetailPageContext(detailData, false, null);
            Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            return false;
        }

        private void ShowMyInviteCodeCopySuccessHint()
        {
            GeneralHintDialogContext dialogContext = new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_InvitationCode_CopySuccess", new object[0]), 2f);
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        [CompilerGenerated]
        private sealed class <SetupScrollView>c__AnonStoreyE9
        {
            internal FriendOverviewPageContext <>f__this;
            internal string key;

            internal void <>m__12C(Transform trans, int index)
            {
                this.<>f__this.OnScrollerChange(this.key, this.<>f__this._tabItemList[this.key], trans, index);
            }
        }

        public enum FriendTab
        {
            FriendListTab,
            AddFriendTab,
            RequestListTab,
            InviteCodeTab
        }
    }
}

