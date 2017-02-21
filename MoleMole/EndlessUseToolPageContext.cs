namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class EndlessUseToolPageContext : BasePageContext
    {
        private Animator _animator;
        private EndlessItem _selectItem;
        private EndlessPlayerData _selectPlayer;
        private EndlessToolDataItem _selectToolData;
        private int ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID = Animator.StringToHash("CanSelectPlayer");
        private const string BATTLE_REPORT_PREFAB_PATH = "UI/Menus/Widget/EndlessActivity/BattleReportRow";
        private const string ENDLESS_ITEM_PREFAB_PATH = "UI/Menus/Widget/EndlessActivity/EndlessItem";
        private const string RANK_ROW_BUTTON_PREFAB_PATH = "UI/Menus/Widget/EndlessActivity/RankInfoButton";

        public EndlessUseToolPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "EndlessUseToolPageContext",
                viewPrefabPath = "UI/Menus/Page/EndlessActivity/EndlessUseToolPage"
            };
            base.config = pattern;
        }

        private void CheckCurrentEndlessDataValid()
        {
            if (TimeUtil.Now > Singleton<EndlessModule>.Instance.SettlementTime)
            {
                Singleton<NetworkManager>.Instance.RequestEndlessData();
                this.BackPage();
            }
        }

        private void CheckItemListEmpty()
        {
            base.view.transform.Find("ItemPanel/NoItemHint").gameObject.SetActive(Singleton<EndlessModule>.Instance.GetPlayerEndlessItemList().Count <= 0);
        }

        private void CheckSelectItemForPlayerPanel()
        {
            if (((this._selectItem == null) || this._selectToolData.ApplyToSelf) || (this._selectPlayer == null))
            {
                this._animator.SetBool(this.ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID, false);
            }
            else
            {
                this._animator.SetBool(this.ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID, true);
            }
        }

        private void ClearSelectPlayer()
        {
            this._selectPlayer = null;
            this.SetupRank();
            this.CheckSelectItemForPlayerPanel();
        }

        private void OnConfirmUseTool(bool confirmed)
        {
            if (confirmed)
            {
                this.RequestUseTool();
            }
            else
            {
                this.ClearSelectPlayer();
            }
        }

        private bool OnEndlessItemDataUpdateNotify(EndlessItemDataUpdateNotify rsp)
        {
            this.SetupItemList();
            return false;
        }

        private bool OnEndlessPlayerDataUpdateNotify(EndlessPlayerDataUpdateNotify rsp)
        {
            this.SetupRank();
            return false;
        }

        private bool OnGetLastEndlessRewardDataRsp(GetLastEndlessRewardDataRsp rsp)
        {
            if ((rsp.get_retcode() == null) && (rsp.get_reward_list().Count > 0))
            {
                this.BackPage();
            }
            return false;
        }

        private void OnItemButtonClick(EndlessItem itemData)
        {
            if (this._selectItem == itemData)
            {
                this._selectItem = null;
                this._selectToolData = null;
                base.view.transform.Find("GroupPanel/Title/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectEndlessToolFirst", new object[0]);
                this._animator.SetBool(this.ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID, false);
            }
            else
            {
                this._selectItem = itemData;
                this._selectToolData = new EndlessToolDataItem((int) itemData.get_item_id(), 1);
                if (this._selectToolData.ApplyToSelf || (this._selectToolData.ToolType == 3))
                {
                    this._animator.SetBool(this.ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID, false);
                }
            }
            this._selectPlayer = null;
            this.SetRankListTitle();
            this.SetupRank();
            this.SetupItemList();
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            this.CheckCurrentEndlessDataValid();
        }

        public override bool OnNotify(Notify ntf)
        {
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 140:
                    return this.SetupView();

                case 0x94:
                    return this.OnUseEndlessItemRsp(pkt.getData<UseEndlessItemRsp>());

                case 0x97:
                    return this.OnEndlessPlayerDataUpdateNotify(pkt.getData<EndlessPlayerDataUpdateNotify>());

                case 0x92:
                    return this.OnGetLastEndlessRewardDataRsp(pkt.getData<GetLastEndlessRewardDataRsp>());

                case 0x98:
                    return this.OnEndlessItemDataUpdateNotify(pkt.getData<EndlessItemDataUpdateNotify>());
            }
            return false;
        }

        private void OnRankRowButtonClick(EndlessPlayerData playerEndlessData)
        {
            if (this._selectItem == null)
            {
                base.view.transform.Find("GroupPanel/Title/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectEndlessToolFirst", new object[0]);
            }
            else
            {
                if (this._selectPlayer == playerEndlessData)
                {
                    this._selectPlayer = null;
                }
                else
                {
                    this._selectPlayer = playerEndlessData;
                    this.SetupUseToolConfirmDialog(this._selectToolData);
                }
                this.SetupRank();
                this.SetupItemList();
            }
        }

        private void OnScrollerChange(Transform childTrans, int index)
        {
            List<int> rankListSorted = Singleton<EndlessModule>.Instance.GetRankListSorted();
            EndlessPlayerData playerEndlessData = Singleton<EndlessModule>.Instance.GetPlayerEndlessData(rankListSorted[index]);
            PlayerFriendBriefData playerBriefData = Singleton<EndlessModule>.Instance.GetPlayerBriefData(rankListSorted[index]);
            childTrans.GetComponent<MonoRankButton>().SetupView(index + 1, playerEndlessData, UIUtil.GetPlayerNickname(playerBriefData), this._selectPlayer == playerEndlessData, new Action<EndlessPlayerData>(this.OnRankRowButtonClick), this._selectToolData);
        }

        private void OnUseBtnClick()
        {
            if (this._selectToolData.ApplyToSelf || (this._selectToolData.ToolType == 3))
            {
                this.SetupUseToolConfirmDialog(this._selectToolData);
            }
            else
            {
                this._animator.SetBool(this.ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID, true);
            }
        }

        private bool OnUseEndlessItemRsp(UseEndlessItemRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.ShowEndlessToolEffect(this._selectToolData, rsp);
                this._selectItem = null;
                this._selectToolData = null;
                this.SetupView();
            }
            return false;
        }

        private void RequestUseTool()
        {
            Singleton<NetworkManager>.Instance.RequestUseEndlessItem(this._selectItem.get_item_id(), (this._selectPlayer != null) ? ((int) this._selectPlayer.get_uid()) : -1);
            this._selectPlayer = null;
        }

        private void SetRankListTitle()
        {
            if (this._selectItem == null)
            {
                base.view.transform.Find("GroupPanel/Title/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectEndlessToolFirst", new object[0]);
            }
            else
            {
                base.view.transform.Find("GroupPanel/Title/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectEndlessTarget", new object[0]);
            }
        }

        private void SetupItemList()
        {
            Transform parent = base.view.transform.Find("ItemPanel/ItemList/Content");
            List<EndlessItem> playerEndlessItemList = Singleton<EndlessModule>.Instance.GetPlayerEndlessItemList();
            if (parent.childCount < playerEndlessItemList.Count)
            {
                int num = playerEndlessItemList.Count - parent.childCount;
                for (int j = 0; j < num; j++)
                {
                    UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI/Menus/Widget/EndlessActivity/EndlessItem")).transform.SetParent(parent, false);
                }
            }
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (i >= playerEndlessItemList.Count)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    EndlessItem itemData = playerEndlessItemList[i];
                    child.GetComponent<MonoEndlessItemButton>().SetupView(itemData, itemData == this._selectItem, this._selectPlayer, new Action<EndlessItem>(this.OnItemButtonClick), new Action(this.OnUseBtnClick));
                }
            }
            this.CheckItemListEmpty();
        }

        private void SetupRank()
        {
            List<int> rankListSorted = Singleton<EndlessModule>.Instance.GetRankListSorted();
            base.view.transform.Find("GroupPanel/RankPanel/RankList").GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(this.OnScrollerChange), rankListSorted.Count, null);
        }

        private void SetupUseToolConfirmDialog(EndlessToolDataItem toolData)
        {
            object[] replaceParams = new object[] { toolData.GetDisplayTitle() };
            string text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessComfirmUse", replaceParams);
            if (toolData.ShowIcon)
            {
                if (toolData.ApplyToSelf)
                {
                    List<EndlessToolDataItem> appliedToolDataList = Singleton<EndlessModule>.Instance.GetAppliedToolDataList();
                    for (int i = 0; i < appliedToolDataList.Count; i++)
                    {
                        if (appliedToolDataList[i].ID == toolData.ID)
                        {
                            object[] objArray2 = new object[] { toolData.GetDisplayTitle() };
                            text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessComfirmReuse", objArray2);
                            break;
                        }
                    }
                }
                else if ((toolData.ToolType != 3) && (this._selectPlayer != null))
                {
                    List<EndlessWaitEffectItem> list2 = this._selectPlayer.get_wait_effect_item_list();
                    for (int j = 0; j < list2.Count; j++)
                    {
                        if (list2[j].get_item_id() == toolData.ID)
                        {
                            object[] objArray3 = new object[] { toolData.GetDisplayTitle() };
                            text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessComfirmReuse", objArray3);
                            break;
                        }
                    }
                }
            }
            if (Singleton<EndlessModule>.Instance.SelfInvisible())
            {
                text = text + Environment.NewLine + LocalizationGeneralLogic.GetText("Menu_Desc_InvisibleItemWillLoseEffectHint", new object[0]);
            }
            GeneralDialogContext dialogContext = new GeneralDialogContext {
                type = GeneralDialogContext.ButtonType.DoubleButton,
                title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                desc = text,
                buttonCallBack = new Action<bool>(this.OnConfirmUseTool),
                notDestroyAfterTouchBG = true,
                destroyCallBack = new Action(this.ClearSelectPlayer)
            };
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("GroupPanel/Title/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectEndlessToolFirst", new object[0]);
            this._animator = base.view.transform.GetComponent<Animator>();
            this.SetupRank();
            this.SetupItemList();
            this.SetRankListTitle();
            this.CheckSelectItemForPlayerPanel();
            Singleton<EndlessModule>.Instance.CheckAllAvatarHPChanged();
            return false;
        }

        private void ShowActivityRewardDialog(GetLastEndlessRewardDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if (rsp.get_reward_list().Count > 0)
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new MissionRewardGotDialogContext(rsp.get_reward_list(), null), UIType.Any);
                }
                Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData = null;
                Singleton<MiHoYoGameData>.Instance.Save();
            }
        }

        private void ShowEndlessToolEffect(EndlessToolDataItem toolData, UseEndlessItemRsp rsp)
        {
            Singleton<MainUIManager>.Instance.ShowUIEffect(base.config.contextName, toolData.EffectPrefatPath);
            object[] replaceParams = new object[] { toolData.GetDisplayTitle() };
            string text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessUseSuccess", replaceParams);
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(text, 2f), UIType.Any);
        }
    }
}

