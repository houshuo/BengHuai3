namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class LevelPreparePageContext : BasePageContext
    {
        private FriendDetailDataItem _helperDetailData;
        private bool _isWaitingLevelBegin;
        private LoadingWheelWidgetContext _loadingWheelDialogContext;
        private int _playerUidToShow;
        private FriendBriefDataItem _selectedHelper;
        private List<FriendBriefDataItem> _showDataList;
        private StageBeginRsp _stageBeginRsp;
        [CompilerGenerated]
        private static Comparison<FriendBriefDataItem> <>f__am$cache8;
        [CompilerGenerated]
        private static Comparison<FriendBriefDataItem> <>f__am$cache9;
        public readonly LevelDataItem level;
        private const int MAX_TEAM_MEMBER_NUM = 3;

        public LevelPreparePageContext(LevelDataItem level)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "LevelPreparePageContext",
                viewPrefabPath = "UI/Menus/Page/Map/LevelPreparePage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            this.level = level;
            this._playerUidToShow = -1;
            this._isWaitingLevelBegin = false;
            this._showDataList = new List<FriendBriefDataItem>();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Btn").GetComponent<Button>(), new UnityAction(this.OnOkButtonCallBack));
        }

        public override void Destroy()
        {
            if (this._loadingWheelDialogContext != null)
            {
                this._loadingWheelDialogContext.Finish();
            }
            base.Destroy();
        }

        private void DoBeginLevel()
        {
            if (Singleton<LevelScoreManager>.Instance == null)
            {
                Singleton<LevelScoreManager>.Create();
            }
            LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
            Transform transform = base.view.transform.Find("LevelDebugPanel");
            if (transform != null)
            {
                instance.isDebugDynamicLevel = transform.GetComponent<MonoLevelDebug>().useDynamicLevel;
            }
            instance.collectAntiCheatData = this._stageBeginRsp.get_is_collect_cheat_data();
            instance.signKey = this._stageBeginRsp.get_sign_key();
            int progress = !this._stageBeginRsp.get_progressSpecified() ? 0 : ((int) this._stageBeginRsp.get_progress());
            LevelDataItem level = !this._stageBeginRsp.get_stage_idSpecified() ? this.level : Singleton<LevelModule>.Instance.GetLevelById((int) this._stageBeginRsp.get_stage_id());
            instance.SetLevelBeginIntent(level, progress, this._stageBeginRsp.get_drop_item_list(), this.level.BattleType, this._helperDetailData);
            this.ResetWaitPacketData();
            Singleton<MainUIManager>.Instance.PopTopPageOnly();
            ChapterSelectPageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext as ChapterSelectPageContext;
            if (currentPageContext != null)
            {
                currentPageContext.OnDoLevelBegin();
            }
            bool toKeepContextStack = Singleton<MainUIManager>.Instance.SceneCanvas is MonoMainCanvas;
            Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", toKeepContextStack, true, true, null, true);
        }

        private List<FriendBriefDataItem> GetHelperList()
        {
            List<FriendBriefDataItem> list = new List<FriendBriefDataItem>();
            FriendBriefDataItem oneStrangeHelper = Singleton<FriendModule>.Instance.GetOneStrangeHelper();
            List<FriendBriefDataItem> friendsList = Singleton<FriendModule>.Instance.friendsList;
            List<FriendBriefDataItem> collection = new List<FriendBriefDataItem>();
            List<FriendBriefDataItem> list4 = new List<FriendBriefDataItem>();
            foreach (FriendBriefDataItem item2 in friendsList)
            {
                if (Singleton<FriendModule>.Instance.isHelperFrozen(item2.uid))
                {
                    list4.Add(item2);
                }
                else
                {
                    collection.Add(item2);
                }
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = (o1, o2) => o2.level - o1.level;
            }
            collection.Sort(<>f__am$cache8);
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = (o1, o2) => Singleton<FriendModule>.Instance.GetHelperNextAvaliableTime(o1.uid).CompareTo(Singleton<FriendModule>.Instance.GetHelperNextAvaliableTime(o2.uid));
            }
            list4.Sort(<>f__am$cache9);
            if (oneStrangeHelper != null)
            {
                list.Add(oneStrangeHelper);
            }
            list.AddRange(collection);
            list.AddRange(list4);
            return list;
        }

        private void InitTeam()
        {
            if ((this.level.LevelType != 1) && (Singleton<PlayerModule>.Instance.playerData.GetMemberList(this.level.LevelType).Count == 0))
            {
                List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(1);
                Singleton<PlayerModule>.Instance.playerData.SetTeamMember(this.level.LevelType, memberList);
                Singleton<NetworkManager>.Instance.NotifyUpdateAvatarTeam(this.level.LevelType);
            }
        }

        private bool IsFriendNumOver()
        {
            int count = Singleton<FriendModule>.Instance.friendsList.Count;
            int maxFriendFinal = Singleton<PlayerModule>.Instance.playerData.maxFriendFinal;
            return (count > maxFriendFinal);
        }

        private void OnChange(Transform trans, int index)
        {
            bool selected = this._showDataList[index] == this._selectedHelper;
            trans.GetComponent<MonoHelperFrameRow>().SetupView(this._showDataList[index], selected, new Action<FriendBriefDataItem>(this.OnFrameBtnClick), new Action<FriendBriefDataItem>(this.OnIconBtnClick));
        }

        private void OnFrameBtnClick(FriendBriefDataItem friendBriefData)
        {
            bool flag = Singleton<FriendModule>.Instance.IsMyFriend(friendBriefData.uid);
            if (this.IsFriendNumOver() && flag)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new FriendNumOverDialogContext(), UIType.Any);
            }
            else
            {
                if ((this._selectedHelper != null) && (this._selectedHelper.uid == friendBriefData.uid))
                {
                    this._selectedHelper = null;
                }
                else
                {
                    this._selectedHelper = friendBriefData;
                }
                this.SetupHelperSkill();
                base.view.transform.Find("FriendsPanel/FriendScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
            }
        }

        private void OnIconBtnClick(FriendBriefDataItem friendBriefData)
        {
            FriendDetailDataItem detailData = Singleton<FriendModule>.Instance.TryGetFriendDetailData(friendBriefData.uid);
            if (detailData == null)
            {
                this._playerUidToShow = friendBriefData.uid;
                Singleton<NetworkManager>.Instance.RequestFriendDetailInfo(friendBriefData.uid);
            }
            else
            {
                UIUtil.ShowFriendDetailInfo(detailData, false, null);
            }
        }

        public override void OnLandedFromBackPage()
        {
            this.SetupMyTeam();
            this.SetupLeaderSkill();
            base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn", false, null);
            base.OnLandedFromBackPage();
        }

        private void OnMulModeHintDialogButtonClick(bool isOk)
        {
            if (isOk)
            {
                this.RequestToEnterLevel();
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.TeamMemberChanged)
            {
                this.SetupMyTeam();
                this.SetupLeaderSkill();
                return false;
            }
            return ((ntf.type == NotifyTypes.ShowStaminaExchangeInfo2) && this.ShowStaminaExchangeDialog());
        }

        public void OnOkButtonCallBack()
        {
            GeneralDialogContext context;
            if (Singleton<PlayerModule>.Instance.playerData.teamLevel < this.level.UnlockPlayerLevel)
            {
                context = new GeneralDialogContext {
                    title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0])
                };
                object[] replaceParams = new object[] { this.level.UnlockPlayerLevel };
                context.desc = LocalizationGeneralLogic.GetText("Err_PlayerLvLack", replaceParams);
                context.type = GeneralDialogContext.ButtonType.SingleButton;
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else
            {
                bool flag = false;
                foreach (int num in Singleton<PlayerModule>.Instance.playerData.GetMemberList(this.level.LevelType))
                {
                    if (Singleton<IslandModule>.Instance.IsAvatarDispatched(num))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    context = new GeneralDialogContext {
                        title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0]),
                        desc = LocalizationGeneralLogic.GetText("Menu_Desc_AvatarDispatchedCannotFight", new object[0]),
                        type = GeneralDialogContext.ButtonType.SingleButton
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else
                {
                    if (this.level.IsMultMode)
                    {
                        int count = Singleton<PlayerModule>.Instance.playerData.GetMemberList(this.level.LevelType).Count;
                        if (count < this.level.MinEnterAvatarNum)
                        {
                            context = new GeneralDialogContext {
                                title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0]),
                                desc = LocalizationGeneralLogic.GetText("Menu_Desc_TeamMemberLackHint", new object[0]),
                                type = GeneralDialogContext.ButtonType.SingleButton
                            };
                            Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                            return;
                        }
                        if ((count < 3) && (count < Singleton<AvatarModule>.Instance.UserAvatarList.Count))
                        {
                            context = new GeneralDialogContext {
                                title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0]),
                                desc = LocalizationGeneralLogic.GetText("Menu_Desc_TeamMemberCanAddHint", new object[0]),
                                type = GeneralDialogContext.ButtonType.DoubleButton,
                                buttonCallBack = new Action<bool>(this.OnMulModeHintDialogButtonClick)
                            };
                            Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                            return;
                        }
                    }
                    this.RequestToEnterLevel();
                }
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x2c:
                    return this.OnStageBeginRsp(pkt.getData<StageBeginRsp>());

                case 0x49:
                    return this.OnPlayerDetailRsp(pkt.getData<GetPlayerDetailDataRsp>());
            }
            return false;
        }

        private bool OnPlayerDetailRsp(GetPlayerDetailDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if (!this._isWaitingLevelBegin)
                {
                    if (this._playerUidToShow == rsp.get_detail().get_uid())
                    {
                        this._playerUidToShow = -1;
                        FriendDetailDataItem detailData = new FriendDetailDataItem(rsp.get_detail());
                        return UIUtil.ShowFriendDetailInfo(detailData, false, null);
                    }
                }
                else
                {
                    this._helperDetailData = new FriendDetailDataItem(rsp.get_detail());
                    if (this._stageBeginRsp != null)
                    {
                        this.DoBeginLevel();
                    }
                }
            }
            else if (this._isWaitingLevelBegin)
            {
                this.ResetWaitPacketData();
            }
            return false;
        }

        private void OnRefreshTeammateUI(int num, bool bSelfSkill)
        {
            Transform transform = base.view.transform.Find("TeamPanel/Team");
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(this.level.LevelType);
            AvatarDataItem avatarData = (num > memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(memberList[num - 1]);
            transform.Find(num.ToString()).gameObject.GetComponent<MonoTeamMember>().SetupView(this.level.LevelType, num, base.view.GetComponent<MonoSwitchTeammateAnimPlugin>(), avatarData, base.view.GetComponent<RectTransform>());
            if (bSelfSkill)
            {
                this.SetupLeaderSkill();
            }
        }

        public bool OnStageBeginRsp(StageBeginRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this._stageBeginRsp = rsp;
                if ((this._selectedHelper == null) || (this._helperDetailData != null))
                {
                    this.DoBeginLevel();
                }
            }
            else
            {
                this.ResetWaitPacketData();
                if (rsp.get_retcode() == 4)
                {
                    Singleton<PlayerModule>.Instance.playerData._cacheDataUtil.CheckCacheValidAndGo<PlayerStaminaExchangeInfo>(ECacheData.Stamina, NotifyTypes.ShowStaminaExchangeInfo2);
                }
                else
                {
                    GeneralDialogContext dialogContext = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.SingleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
                    };
                    dialogContext.desc = (rsp.get_retcode() != 3) ? LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]) : LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[] { this.level.UnlockPlayerLevel });
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
            }
            return false;
        }

        private void RequestToEnterLevel()
        {
            if (this._loadingWheelDialogContext == null)
            {
                LoadingWheelWidgetContext context = new LoadingWheelWidgetContext {
                    ignoreMaxWaitTime = true
                };
                this._loadingWheelDialogContext = context;
                Singleton<MainUIManager>.Instance.ShowWidget(this._loadingWheelDialogContext, UIType.Any);
            }
            this._isWaitingLevelBegin = true;
            int uid = (this._selectedHelper == null) ? 0 : this._selectedHelper.uid;
            if (this._selectedHelper != null)
            {
                this._helperDetailData = Singleton<FriendModule>.Instance.TryGetFriendDetailData(this._selectedHelper.uid);
                if (this._helperDetailData == null)
                {
                    Singleton<NetworkManager>.Instance.RequestFriendDetailInfo(uid);
                }
            }
            Singleton<NetworkManager>.Instance.RequestLevelBeginReq(this.level, uid);
        }

        private void ResetWaitPacketData()
        {
            this._isWaitingLevelBegin = false;
            this._stageBeginRsp = null;
            this._helperDetailData = null;
            if (this._loadingWheelDialogContext != null)
            {
                this._loadingWheelDialogContext.Finish();
            }
        }

        private void SetupHelpers()
        {
            base.view.transform.Find("FriendsPanel/MulModeHint").gameObject.SetActive(this.level.IsMultMode);
            this._showDataList = this.GetHelperList();
            base.view.transform.Find("FriendsPanel/FriendScrollView").GetComponent<MonoGridScroller>().Init(new MoleMole.MonoGridScroller.OnChange(this.OnChange), this._showDataList.Count, null);
        }

        private void SetupHelperSkill()
        {
            Transform transform = base.view.transform.Find("TeamPanel/Skills/Friend");
            if ((this._selectedHelper == null) || (this._selectedHelper.AvatarLeaderSkill == null))
            {
                transform.gameObject.SetActive(false);
            }
            else
            {
                AvatarSkillDataItem avatarLeaderSkill = this._selectedHelper.AvatarLeaderSkill;
                if (!avatarLeaderSkill.UnLocked)
                {
                    transform.gameObject.SetActive(false);
                }
                else
                {
                    Color color;
                    transform.gameObject.SetActive(true);
                    bool flag = Singleton<FriendModule>.Instance.IsMyFriend(this._selectedHelper.uid);
                    if (flag)
                    {
                        UIUtil.TryParseHexString("#88c700FF", out color);
                    }
                    else
                    {
                        UIUtil.TryParseHexString("#a0a0a0FF", out color);
                    }
                    transform.Find("BG/Select").gameObject.SetActive(flag);
                    transform.Find("BG/Unable").gameObject.SetActive(!flag);
                    Text component = transform.Find("SkillName").GetComponent<Text>();
                    component.text = avatarLeaderSkill.SkillName;
                    component.color = color;
                    transform.Find("Desc").GetComponent<Text>().text = avatarLeaderSkill.SkillShortInfo;
                    string str = !flag ? LocalizationGeneralLogic.GetText("LevelPreparePage_FriendSkillHint_NotFriend", new object[0]) : LocalizationGeneralLogic.GetText("LevelPreparePage_FriendSkillHint_IsFriend", new object[0]);
                    Text text2 = transform.Find("Hint").GetComponent<Text>();
                    text2.text = str;
                    text2.color = color;
                    transform.Find("Line").GetComponent<Image>().color = color;
                }
            }
        }

        private void SetupLeaderSkill()
        {
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(this.level.LevelType);
            if (memberList.Count > 0)
            {
                int avatarID = memberList[0];
                AvatarSkillDataItem leaderSkill = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarID).GetLeaderSkill();
                if (leaderSkill.UnLocked)
                {
                    base.view.transform.Find("TeamPanel/Skills/Self").gameObject.SetActive(true);
                    base.view.transform.Find("TeamPanel/Skills/Self/SkillName").GetComponent<Text>().text = leaderSkill.SkillName;
                    base.view.transform.Find("TeamPanel/Skills/Self/Desc").GetComponent<Text>().text = leaderSkill.SkillShortInfo;
                }
                else
                {
                    base.view.transform.Find("TeamPanel/Skills/Self").gameObject.SetActive(false);
                }
            }
            else
            {
                base.view.transform.Find("TeamPanel/Skills/Self").gameObject.SetActive(false);
            }
            this.SetupHelperSkill();
        }

        private void SetupLevelTips()
        {
            int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
            if (teamLevel < MiscData.Config.BasicConfig.MinPlayerPunishLevel)
            {
                base.view.transform.Find("LvTips").gameObject.SetActive(false);
            }
            else
            {
                int num2 = Mathf.Clamp(this.level.RecommandLv - teamLevel, 0, 10);
                int playerPunishLevelDifferenceStepOne = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepOne;
                int playerPunishLevelDifferenceStepTwo = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepTwo;
                int playerPunishLevelDifferenceStepThree = MiscData.Config.BasicConfig.PlayerPunishLevelDifferenceStepThree;
                if ((num2 >= playerPunishLevelDifferenceStepOne) && (num2 < playerPunishLevelDifferenceStepTwo))
                {
                    base.view.transform.Find("LvTips").gameObject.SetActive(false);
                }
                else if ((num2 >= playerPunishLevelDifferenceStepTwo) && (num2 < playerPunishLevelDifferenceStepThree))
                {
                    base.view.transform.Find("LvTips").gameObject.SetActive(true);
                    base.view.transform.Find("LvTips").GetComponent<Image>().color = MiscData.GetColor("PrepareLevelPunishTipYellowBgColor");
                    base.view.transform.Find("LvTips/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Level_Tips1", new object[0]);
                    base.view.transform.Find("LvTips/Text").GetComponent<Text>().color = MiscData.GetColor("PrepareLevelPunishTipYellowTxtColor");
                    base.view.transform.Find("LvTips/Arrow").GetComponent<Image>().color = MiscData.GetColor("PrepareLevelPunishTipYellowBgColor");
                }
                else
                {
                    base.view.transform.Find("LvTips").gameObject.SetActive(true);
                    base.view.transform.Find("LvTips").GetComponent<Image>().color = MiscData.GetColor("PrepareLevelPunishTipRedBgColor");
                    base.view.transform.Find("LvTips/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Level_Tips2", new object[0]);
                    base.view.transform.Find("LvTips/Text").GetComponent<Text>().color = MiscData.GetColor("PrepareLevelPunishTipRedTxtColor");
                    base.view.transform.Find("LvTips/Arrow").GetComponent<Image>().color = MiscData.GetColor("PrepareLevelPunishTipRedBgColor");
                }
            }
        }

        private void SetupMyTeam()
        {
            this.InitTeam();
            Transform transform = base.view.transform.Find("TeamPanel/Team");
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(this.level.LevelType);
            MonoSwitchTeammateAnimPlugin component = base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>();
            for (int i = 1; i <= 3; i++)
            {
                AvatarDataItem avatarData = (i > memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(memberList[i - 1]);
                MonoTeamMember member = transform.Find(i.ToString()).gameObject.GetComponent<MonoTeamMember>();
                member.SetupView(this.level.LevelType, i, component, avatarData, base.view.GetComponent<RectTransform>());
                member.RegisterCallback(new RefreshTeammateUI_Handler(this.OnRefreshTeammateUI), new MoleMole.StartSwitchAnim_Handler(this.StartSwitchAnim_Handler));
            }
            base.view.transform.Find("TeamPanel/MulModeHint").gameObject.SetActive(this.level.IsMultMode);
            if (this.level.IsMultMode)
            {
                base.view.transform.Find("TeamPanel/MulModeHint/Num").GetComponent<Text>().text = this.level.MinEnterAvatarNum.ToString();
            }
            this.SetupLevelTips();
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Cost/Stamina").GetComponent<Text>().text = this.level.StaminaCost.ToString();
            base.view.transform.Find("BG/Pic").GetComponent<Image>().sprite = this.level.GetDetailPicSprite();
            this.SetupMyTeam();
            this.SetupHelpers();
            this.SetupLeaderSkill();
            base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn", false, null);
            base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>().RegisterCallback(new RefreshTeammateUI_Handler(this.OnRefreshTeammateUI));
            base.view.transform.Find("DynamicLv").gameObject.SetActive(GlobalVars.DEBUG_FEATURE_ON);
            base.view.transform.Find("LevelDebugButton").gameObject.SetActive(GlobalVars.DEBUG_FEATURE_ON);
            return false;
        }

        public bool ShowStaminaExchangeDialog()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new StaminaExchangeDialogContext("Menu_Desc_StaminaExchange2"), UIType.Any);
            return false;
        }

        private void StartSwitchAnim_Handler(int dataIndex, int fromIndex, int toIndex)
        {
            base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>().StartSwitchAnim(dataIndex, fromIndex, toIndex);
        }
    }
}

