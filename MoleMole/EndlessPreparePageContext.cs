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

    public class EndlessPreparePageContext : BasePageContext
    {
        private bool _enterBattleDirrectly;
        private EndlessGroupMetaData _groupMetaData;
        private LoadingWheelWidgetContext _loadingWheelDialogContext;
        private EndlessStageBeginRsp _stageBeginRsp;
        [CompilerGenerated]
        private static Comparison<EndlessDropMetaData> <>f__am$cache4;
        private const int MAX_TEAM_MEMBER_NUM = 3;

        public EndlessPreparePageContext(bool enterBattleDirrectly = false)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "EndlessPreparePageContext",
                viewPrefabPath = "UI/Menus/Page/EndlessActivity/EndlessPreparePage"
            };
            base.config = pattern;
            this._enterBattleDirrectly = enterBattleDirrectly;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Action/Btn").GetComponent<Button>(), new UnityAction(this.OnOkButtonCallBack));
        }

        private void DoBeginLevel()
        {
            Singleton<LevelScoreManager>.Create();
            int progress = !this._stageBeginRsp.get_progressSpecified() ? 1 : (((int) this._stageBeginRsp.get_progress()) + 1);
            int hardLevel = Mathf.FloorToInt(this._groupMetaData.baseHardLevel + ((progress - 1) * this._groupMetaData.deltaHardLevel));
            List<string> appliedToolList = new List<string>();
            foreach (uint num3 in this._stageBeginRsp.get_effect_item_id_list())
            {
                EndlessToolDataItem item = new EndlessToolDataItem((int) num3, 1);
                if (item.ParamString != null)
                {
                    appliedToolList.Add(item.ParamString);
                }
            }
            Singleton<LevelScoreManager>.Instance.SetEndlessLevelBeginIntent(progress, hardLevel, appliedToolList, this._stageBeginRsp, MiscData.Config.BasicConfig.EndlessInitTimer, 1);
            this.ResetWaitPacketData();
            this.BackPage();
            Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", true, true, true, null, true);
        }

        private EndlessDropMetaData GetDropDataList(int level)
        {
            int currentGroupLevel = Singleton<EndlessModule>.Instance.currentGroupLevel;
            int num2 = Singleton<EndlessModule>.Instance.CurrentFinishProgress + 1;
            List<EndlessDropMetaData> itemList = EndlessDropMetaDataReader.GetItemList();
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = delegate (EndlessDropMetaData left, EndlessDropMetaData right) {
                    if (left.group != right.group)
                    {
                        return left.group - right.group;
                    }
                    return left.level - right.level;
                };
            }
            itemList.Sort(<>f__am$cache4);
            for (int i = 0; i < (itemList.Count - 1); i++)
            {
                EndlessDropMetaData data = itemList[i];
                EndlessDropMetaData data2 = itemList[i + 1];
                if (data.group >= currentGroupLevel)
                {
                    if (data.group > currentGroupLevel)
                    {
                        return data;
                    }
                    if (data.level == num2)
                    {
                        return data;
                    }
                    if (data.level < num2)
                    {
                        if ((data2.group == currentGroupLevel) && (data2.level > num2))
                        {
                            return data;
                        }
                        if (data2.group > currentGroupLevel)
                        {
                            return data;
                        }
                    }
                }
            }
            return null;
        }

        private void InitTeam()
        {
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(4);
            HashSet<int> collection = new HashSet<int>(memberList);
            if (collection.Count != 0)
            {
                HashSet<int> set2 = new HashSet<int>();
                for (int i = 0; i < memberList.Count; i++)
                {
                    int avatarId = memberList[i];
                    EndlessAvatarHp endlessAvatarHPData = Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(avatarId);
                    if (endlessAvatarHPData.get_is_dieSpecified() && endlessAvatarHPData.get_is_die())
                    {
                        set2.Add(avatarId);
                    }
                }
                collection.ExceptWith(set2);
                Singleton<PlayerModule>.Instance.playerData.SetTeamMember(4, new List<int>(collection));
                Singleton<NetworkManager>.Instance.NotifyUpdateAvatarTeam(4);
            }
        }

        public bool OnGetEndlessAvatarHpRsp(GetEndlessAvatarHpRsp rsp)
        {
            this.SetupMyTeam();
            return false;
        }

        public override void OnLandedFromBackPage()
        {
            this.SetupMyTeam();
            this.SetupLeaderSkill();
            this.SetupInfoPanel();
            base.OnLandedFromBackPage();
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.TeamMemberChanged)
            {
                this.SetupMyTeam();
                this.SetupLeaderSkill();
                this.SetupFightBtnInterable();
            }
            if (((ntf.type == NotifyTypes.EndlessActivityEnd) || (ntf.type == NotifyTypes.EndlessActivitySettlement)) || (ntf.type == NotifyTypes.EndlessActivityBegin))
            {
                Singleton<MainUIManager>.Instance.BackPage();
            }
            return ((ntf.type == NotifyTypes.EndlessAppliedToolChange) && this.SetupInfoPanel());
        }

        public void OnOkButtonCallBack()
        {
            if (Singleton<EndlessModule>.Instance.SelfInvisible())
            {
                GeneralConfirmDialogContext dialogContext = new GeneralConfirmDialogContext {
                    type = GeneralConfirmDialogContext.ButtonType.DoubleButton,
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_InvisibleItemWillLoseEffectHint", new object[0]),
                    buttonCallBack = delegate (bool confirmed) {
                        if (confirmed)
                        {
                            this.RequestToEnterLevel();
                        }
                    }
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                this.RequestToEnterLevel();
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x8e:
                    return this.OnStageBeginRsp(pkt.getData<EndlessStageBeginRsp>());

                case 150:
                    return this.OnGetEndlessAvatarHpRsp(pkt.getData<GetEndlessAvatarHpRsp>());
            }
            return false;
        }

        private void OnRefreshTeammateUI(int num, bool bSelfSkill)
        {
            Transform transform = base.view.transform.Find("TeamPanel/Team");
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(4);
            AvatarDataItem avatarData = (num > memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(memberList[num - 1]);
            transform.Find(num.ToString()).gameObject.GetComponent<MonoTeamMember>().SetupView(4, num, base.view.GetComponent<MonoSwitchTeammateAnimPlugin>(), avatarData, base.view.GetComponent<RectTransform>());
            if (bSelfSkill)
            {
                this.SetupLeaderSkill();
            }
        }

        private void OnScrollerChange(Transform toolTrans, int index)
        {
            EndlessToolDataItem itemData = Singleton<EndlessModule>.Instance.GetAppliedToolDataList()[index];
            toolTrans.GetComponent<MonoAppliedEndlessItem>().SetupView(itemData);
        }

        public bool OnStageBeginRsp(EndlessStageBeginRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this._stageBeginRsp = rsp;
                this.DoBeginLevel();
            }
            else
            {
                this.ResetWaitPacketData();
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
                };
                dialogContext.desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        private void RequestToEnterLevel()
        {
            this._loadingWheelDialogContext = new LoadingWheelWidgetContext();
            this._loadingWheelDialogContext.ignoreMaxWaitTime = true;
            Singleton<MainUIManager>.Instance.ShowWidget(this._loadingWheelDialogContext, UIType.Any);
            Singleton<NetworkManager>.Instance.RequestEndlessLevelBeginReq();
        }

        private void ResetWaitPacketData()
        {
            this._stageBeginRsp = null;
            if (this._loadingWheelDialogContext != null)
            {
                this._loadingWheelDialogContext.Finish();
            }
        }

        private void SetActionButtonInteractable(bool interactable)
        {
            base.view.transform.Find("Action/Btn").GetComponent<Button>().interactable = interactable;
        }

        private void SetupDropList()
        {
            string text = string.Empty;
            int level = Singleton<EndlessModule>.Instance.CurrentFinishProgress + 1;
            EndlessDropMetaData dropDataList = this.GetDropDataList(level);
            if (dropDataList == null)
            {
                base.view.transform.Find("InfoPanel/Drop").gameObject.SetActive(false);
            }
            else
            {
                List<int> list;
                List<int> firstDropDisplayList = dropDataList.firstDropDisplayList;
                List<int> dropDisplayList = dropDataList.dropDisplayList;
                if ((Singleton<EndlessModule>.Instance.maxLevelEverReach < level) && (firstDropDisplayList != null))
                {
                    list = firstDropDisplayList;
                    text = LocalizationGeneralLogic.GetText("Menu_FirstDropList", new object[0]);
                }
                else
                {
                    list = dropDisplayList;
                    text = LocalizationGeneralLogic.GetText("Menu_DisplayDropList", new object[0]);
                }
                base.view.transform.Find("InfoPanel/Drop/Title/Text").GetComponent<Text>().text = text;
                Transform transform = base.view.transform.Find("InfoPanel/Drop/Drops/ScollerContent");
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    bool flag = i < list.Count;
                    child.gameObject.SetActive(flag);
                    if (flag)
                    {
                        StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(list[i], 1);
                        child.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, null, true, false, false, false);
                    }
                }
            }
        }

        private bool SetupFightBtnInterable()
        {
            bool flag = true;
            if (Singleton<PlayerModule>.Instance.playerData.GetMemberList(4).Count < 1)
            {
                flag = false;
            }
            if ((TimeUtil.Now < Singleton<EndlessModule>.Instance.BeginTime) || (TimeUtil.Now >= Singleton<EndlessModule>.Instance.EndTime))
            {
                flag = false;
            }
            base.view.transform.Find("Action/Btn").GetComponent<Button>().interactable = flag;
            return flag;
        }

        private bool SetupInfoPanel()
        {
            base.view.transform.Find("InfoPanel/Title/GroupName").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(this._groupMetaData.groupName, new object[0]);
            base.view.transform.Find("InfoPanel/Title/GroupName").GetComponent<Text>().color = Miscs.ParseColor(MiscData.Config.EndlessGroupUnSelectColor[this._groupMetaData.groupLevel]);
            base.view.transform.Find("InfoPanel/Title/FloorNum").GetComponent<Text>().text = (Singleton<EndlessModule>.Instance.CurrentFinishProgress + 1).ToString();
            base.view.transform.Find("InfoPanel/ApplyToolsScrollView/").GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(this.OnScrollerChange), Singleton<EndlessModule>.Instance.GetAppliedToolDataList().Count, null);
            base.view.transform.Find("InfoPanel/Rank/Rank").GetComponent<Text>().text = Singleton<EndlessModule>.Instance.CurrentRank.ToString();
            return false;
        }

        private void SetupLeaderSkill()
        {
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(4);
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
        }

        private void SetupMyTeam()
        {
            this.InitTeam();
            Transform transform = base.view.transform.Find("TeamPanel/Team");
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(4);
            MonoSwitchTeammateAnimPlugin component = base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>();
            for (int i = 1; i <= 3; i++)
            {
                AvatarDataItem avatarData = (i > memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(memberList[i - 1]);
                MonoTeamMember member = transform.Find(i.ToString()).gameObject.GetComponent<MonoTeamMember>();
                member.SetupView(4, i, component, avatarData, base.view.GetComponent<RectTransform>());
                member.RegisterCallback(new RefreshTeammateUI_Handler(this.OnRefreshTeammateUI), new MoleMole.StartSwitchAnim_Handler(this.StartSwitchAnim_Handler));
            }
        }

        protected override bool SetupView()
        {
            Singleton<EndlessModule>.Instance.CheckAllAvatarHPChanged();
            this._groupMetaData = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey(Singleton<EndlessModule>.Instance.currentGroupLevel);
            this.SetupMyTeam();
            this.SetupLeaderSkill();
            this.SetupInfoPanel();
            this.SetupDropList();
            this.SetupFightBtnInterable();
            base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>().RegisterCallback(new RefreshTeammateUI_Handler(this.OnRefreshTeammateUI));
            if (this._enterBattleDirrectly)
            {
                this.RequestToEnterLevel();
            }
            return false;
        }

        private void StartSwitchAnim_Handler(int dataIndex, int fromIndex, int toIndex)
        {
            base.view.transform.GetComponent<MonoSwitchTeammateAnimPlugin>().StartSwitchAnim(dataIndex, fromIndex, toIndex);
        }
    }
}

