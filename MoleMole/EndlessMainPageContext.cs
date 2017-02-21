namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class EndlessMainPageContext : BasePageContext
    {
        private Color _groupColor;
        private ViewStatus _viewStatus;
        private const string BATTLE_REPORT_PREFAB_PATH = "UI/Menus/Widget/EndlessActivity/BattleReportRow";

        public EndlessMainPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "EndlessMainPageContext",
                viewPrefabPath = "UI/Menus/Page/EndlessActivity/EndlessMainPage"
            };
            base.config = pattern;
            this._viewStatus = ViewStatus.ShowCurrentGroup;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("ActionBtns/Prepare").GetComponent<Button>(), new UnityAction(this.OnPrepareBtnClick));
            base.BindViewCallback(base.view.transform.Find("ActionBtns/Tool").GetComponent<Button>(), new UnityAction(this.OnUseToolBtnClick));
            base.BindViewCallback(base.view.transform.Find("ActionBtns/Shop").GetComponent<Button>(), new UnityAction(this.OnShopBtnClick));
            base.BindViewCallback(base.view.transform.Find("GroupPanel/InfoBtn").GetComponent<Button>(), new UnityAction(this.OnInfoBtnClick));
        }

        private void CheckCurrentEndlessDataValid()
        {
            if (TimeUtil.Now > Singleton<EndlessModule>.Instance.SettlementTime)
            {
                Singleton<NetworkManager>.Instance.RequestEndlessData();
            }
        }

        private void CheckEndlessReward()
        {
            if (Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData != null)
            {
                this.ShowActivityRewardDialog(Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData);
            }
        }

        private bool CheckIfBombBurst()
        {
            EndlessToolDataItem justBurstBombData = Singleton<EndlessModule>.Instance.justBurstBombData;
            if (justBurstBombData != null)
            {
                Singleton<MainUIManager>.Instance.ShowUIEffect(base.config.contextName, justBurstBombData.EffectPrefatPath);
                object[] replaceParams = new object[] { justBurstBombData.GetDisplayTitle() };
                string text = LocalizationGeneralLogic.GetText("Menu_Desc_ExplodedByOther", replaceParams);
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(text, 2f), UIType.Any);
                Singleton<EndlessModule>.Instance.justBurstBombData = null;
            }
            return false;
        }

        private void InsertNewWarReport(EndlessWarInfo warInfo)
        {
            if (this._viewStatus != ViewStatus.ShowTopGroup)
            {
                Transform parent = base.view.transform.Find("BattlerReport/ReportList/Content");
                Transform transform = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI/Menus/Widget/EndlessActivity/BattleReportRow")).transform;
                transform.GetComponent<MonoBattleReportRow>().SetupView(warInfo, ViewStatus.ShowCurrentGroup);
                transform.SetParent(parent, false);
                transform.SetAsFirstSibling();
                base.view.transform.Find("BattlerReport/ReportList").GetComponent<MonoReportList>().Init();
            }
        }

        private void OnCurrentButtonClick()
        {
            this._viewStatus = ViewStatus.ShowCurrentGroup;
            this.SetupView();
        }

        private bool OnEndlessBegin()
        {
            this.SetAllButtonsInteractable(true);
            return false;
        }

        private bool OnEndlessEnd()
        {
            this.SetAllButtonsInteractable(false);
            return false;
        }

        private bool OnEndlessPlayerDataUpdateNotify(EndlessPlayerDataUpdateNotify rsp)
        {
            this.CheckIfBombBurst();
            this.SetupRank();
            return false;
        }

        private bool OnEndlessSettlement()
        {
            Singleton<NetworkManager>.Instance.RequestEndlessData();
            Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(140, new Action(this.BackPage)), UIType.Any);
            return false;
        }

        private bool OnEndlessWarInfoNotify(EndlessWarInfoNotify rsp)
        {
            this.InsertNewWarReport(rsp.get_war_info());
            return false;
        }

        private bool OnGetLastEndlessRewardDataRsp(GetLastEndlessRewardDataRsp rsp)
        {
            if ((rsp.get_retcode() == null) && (rsp.get_reward_list().Count > 0))
            {
                this.ShowActivityRewardDialog(rsp);
            }
            return false;
        }

        private void OnInfoBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new EndlessInfoDialogContext(), UIType.Any);
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            this.CheckCurrentEndlessDataValid();
            this.CheckEndlessReward();
            Singleton<AssetBundleManager>.Instance.CheckSVNVersion();
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.EndlessActivityEnd)
            {
                return this.OnEndlessEnd();
            }
            if (ntf.type == NotifyTypes.EndlessActivityEnd)
            {
                return this.OnEndlessSettlement();
            }
            if (ntf.type == NotifyTypes.EndlessActivityEnd)
            {
                return this.OnEndlessBegin();
            }
            return ((ntf.type == NotifyTypes.EndlessAppliedToolChange) && this.SetupRank());
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 140:
                case 220:
                    return this.SetupView();

                case 0x97:
                    return this.OnEndlessPlayerDataUpdateNotify(pkt.getData<EndlessPlayerDataUpdateNotify>());

                case 0x92:
                    return this.OnGetLastEndlessRewardDataRsp(pkt.getData<GetLastEndlessRewardDataRsp>());

                case 0x99:
                    return this.OnEndlessWarInfoNotify(pkt.getData<EndlessWarInfoNotify>());
            }
            return false;
        }

        private void OnPrepareBtnClick()
        {
            Singleton<EndlessModule>.Instance.CheckAllAvatarHPChanged();
            Singleton<MainUIManager>.Instance.ShowPage(new EndlessPreparePageContext(false), UIType.Page);
        }

        private void OnShopBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new ShopPageContext(UIShopType.SHOP_ENDLESS), UIType.Page);
        }

        private void OnTopGroupButtonClick()
        {
            this._viewStatus = ViewStatus.ShowTopGroup;
            this.SetupView();
        }

        private void OnUseToolBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new EndlessUseToolPageContext(), UIType.Page);
        }

        private void SetAllButtonsInteractable(bool interactable)
        {
            if (this._viewStatus == ViewStatus.ShowTopGroup)
            {
                base.view.transform.Find("ActionBtns").gameObject.SetActive(false);
            }
            else
            {
                base.view.transform.Find("ActionBtns").gameObject.SetActive(true);
                foreach (Button button in base.view.transform.Find("ActionBtns").GetComponentsInChildren<Button>())
                {
                    if (this._viewStatus == ViewStatus.ShowCurrentGroup)
                    {
                        button.interactable = interactable;
                    }
                }
            }
        }

        private void SetupGroupList()
        {
            EndlessModule instance = Singleton<EndlessModule>.Instance;
            int currentGroupLevel = instance.currentGroupLevel;
            List<EndlessGroupMetaData> itemList = EndlessGroupMetaDataReader.GetItemList();
            Transform transform = base.view.transform.Find("GroupPanel/GroupListPanel/GroupList");
            int num2 = (int) instance.endlessData.get_cur_top_group_level();
            bool flag = instance.endlessData.get_top_group_player_numSpecified() && instance.endlessData.get_top_group_promote_unlock_player_numSpecified();
            float num3 = instance.endlessData.get_top_group_player_num();
            float num4 = instance.endlessData.get_top_group_promote_unlock_player_num();
            bool flag2 = instance.CanSeeTopGroupInfo();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (i >= itemList.Count)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    string str;
                    EndlessGroupMetaData data = itemList[i];
                    bool flag3 = currentGroupLevel == data.groupLevel;
                    child.Find("CurrentGroup").gameObject.SetActive((this._viewStatus != ViewStatus.ShowCurrentGroup) ? (data.groupLevel == itemList.Count) : flag3);
                    if (data.groupLevel > num2)
                    {
                        str = MiscData.Config.EndlessGroupUnopenPrefabPath[data.groupLevel];
                    }
                    else if (data.groupLevel == currentGroupLevel)
                    {
                        str = MiscData.Config.EndlessGroupSelectPrefabPath[data.groupLevel];
                    }
                    else
                    {
                        str = MiscData.Config.EndlessGroupUnselectPrefabPath[data.groupLevel];
                    }
                    child.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(str);
                    if (flag2)
                    {
                        Button component = child.Find("Button").GetComponent<Button>();
                        if (flag3)
                        {
                            component.gameObject.SetActive(true);
                            base.BindViewCallback(component, new UnityAction(this.OnCurrentButtonClick));
                        }
                        else if (data.groupLevel == instance.TopGroupLevel)
                        {
                            child.Find("Button").GetComponent<Button>().gameObject.SetActive(true);
                            child.Find("Particle").GetComponent<ParticleSystem>().gameObject.SetActive(true);
                            base.BindViewCallback(component, new UnityAction(this.OnTopGroupButtonClick));
                        }
                    }
                    else if (data.groupLevel == (num2 + 1))
                    {
                        child.Find("Button").GetComponent<Button>().gameObject.SetActive(true);
                        base.BindViewCallback(child.Find("Button").GetComponent<Button>(), new UnityAction(this.ShowNextGroupHint));
                    }
                    else
                    {
                        child.Find("Button").GetComponent<Button>().gameObject.SetActive(false);
                    }
                    bool flag4 = data.groupLevel == (num2 + 1);
                    child.Find("Slider").gameObject.SetActive(flag4 && flag);
                    child.Find("Slider/Fill").GetComponent<Image>().fillAmount = num3 / num4;
                }
            }
            if (instance.CanRequestTopGroupInfo())
            {
                Singleton<NetworkManager>.Instance.RequestGetEndlessTopGroup();
            }
        }

        private bool SetupRank()
        {
            EndlessModule instance = Singleton<EndlessModule>.Instance;
            int groupLevel = (int) instance.endlessData.get_group_level();
            if (this._viewStatus == ViewStatus.ShowTopGroup)
            {
                groupLevel = instance.TopGroupLevel;
            }
            base.view.transform.gameObject.GetComponent<Animator>().SetBool("IsInTopGroup", groupLevel == instance.TopGroupLevel);
            EndlessGroupMetaData endlessGroupMetaDataByKey = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey(groupLevel);
            base.view.transform.Find("GroupPanel/RankPanel/GroupName/GroupName").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(endlessGroupMetaDataByKey.groupName, new object[0]);
            base.view.transform.Find("GroupPanel/RankPanel/GroupName/GroupName").GetComponent<Text>().color = this._groupColor;
            base.view.transform.Find("GroupPanel/RankPanel/BracketTop/Line").GetComponent<Image>().color = this._groupColor;
            base.view.transform.Find("GroupPanel/RankPanel/BracketTop/LineCur").GetComponent<Image>().color = this._groupColor;
            base.view.transform.Find("GroupPanel/RankPanel/BracketBottom/Line").GetComponent<Image>().color = this._groupColor;
            base.view.transform.Find("GroupPanel/RankPanel/BracketBottom/LineCur").GetComponent<Image>().color = this._groupColor;
            int num2 = 0;
            int count = 0;
            Transform transform = base.view.transform.Find("GroupPanel/RankPanel/RankList/Content/PromoteRank");
            if (endlessGroupMetaDataByKey.promoteRank == 0)
            {
                transform.gameObject.SetActive(false);
            }
            else
            {
                transform.gameObject.SetActive(true);
                List<int> promoteRank = instance.GetPromoteRank(this._viewStatus);
                transform.Find("RankRewardRow/Title").GetComponent<Text>().text = (instance.currentGroupLevel != instance.endlessData.get_cur_top_group_level()) ? LocalizationGeneralLogic.GetText("Menu_Title_EndlessPromoteArea", new object[0]) : LocalizationGeneralLogic.GetText("Menu_Title_EndlessStayArea", new object[0]);
                count = promoteRank.Count;
                transform.GetComponent<MonoRank>().SetupView(num2 + 1, endlessGroupMetaDataByKey.prototeRewardID, promoteRank, this._viewStatus);
                num2 += count;
            }
            transform = base.view.transform.Find("GroupPanel/RankPanel/RankList/Content/NormalRank");
            List<int> normalRank = instance.GetNormalRank(this._viewStatus);
            count = normalRank.Count;
            transform.GetComponent<MonoRank>().SetupView(num2 + 1, endlessGroupMetaDataByKey.normalRewardID, normalRank, this._viewStatus);
            num2 += count;
            transform = base.view.transform.Find("GroupPanel/RankPanel/RankList/Content/DemoteRank");
            if (endlessGroupMetaDataByKey.groupLevel > 1)
            {
                transform.gameObject.SetActive(true);
                List<int> demoteRank = instance.GetDemoteRank(this._viewStatus);
                count = demoteRank.Count;
                transform.GetComponent<MonoRank>().SetupView(num2 + 1, endlessGroupMetaDataByKey.demoteRewardID, demoteRank, this._viewStatus);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
            return false;
        }

        protected override bool SetupView()
        {
            this._groupColor = Miscs.ParseColor(MiscData.Config.EndlessGroupUnSelectColor[(int) Singleton<EndlessModule>.Instance.endlessData.get_group_level()]);
            this.SetupGroupList();
            this.SetupRank();
            this.SetupWarReportList();
            this.SetAllButtonsInteractable(Singleton<EndlessModule>.Instance.GetEndlessActivityStatus() == EndlessActivityStatus.InProgress);
            this.CheckEndlessReward();
            this.CheckIfBombBurst();
            Singleton<EndlessModule>.Instance.CheckAllAvatarHPChanged();
            Singleton<AssetBundleManager>.Instance.CheckSVNVersion();
            return false;
        }

        private void SetupWarReportList()
        {
            List<EndlessWarInfo> list;
            Transform trans = base.view.transform.Find("BattlerReport/ReportList/Content");
            trans.DestroyChildren();
            if (this._viewStatus == ViewStatus.ShowCurrentGroup)
            {
                list = new List<EndlessWarInfo>(Singleton<EndlessModule>.Instance.warInfoList);
            }
            else
            {
                list = Singleton<EndlessModule>.Instance.topGroupData.get_war_info_list();
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (i >= 20)
                {
                    break;
                }
                EndlessWarInfo battleInfo = list[i];
                Transform transform = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI/Menus/Widget/EndlessActivity/BattleReportRow")).transform;
                transform.GetComponent<MonoBattleReportRow>().SetupView(battleInfo, this._viewStatus);
                transform.SetParent(trans, false);
            }
            base.view.transform.Find("BattlerReport/ReportList").GetComponent<MonoReportList>().Init();
        }

        private void ShowActivityRewardDialog(GetLastEndlessRewardDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if (rsp.get_reward_list().Count > 0)
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new EndlessSettlementDialogContext(rsp), UIType.Any);
                }
                Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData = null;
                Singleton<MiHoYoGameData>.Instance.Save();
            }
        }

        private void ShowNextGroupHint()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_EndlessNextGroupOpenHint", new object[0]), 2f), UIType.Any);
        }

        public enum ViewStatus
        {
            ShowCurrentGroup,
            ShowTopGroup
        }
    }
}

