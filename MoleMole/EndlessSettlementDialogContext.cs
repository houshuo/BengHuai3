namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class EndlessSettlementDialogContext : BaseDialogContext
    {
        private List<RewardUIData> _gotRewardList = new List<RewardUIData>();
        private Color _groupBGColor;
        private GetLastEndlessRewardDataRsp _rewardData;
        private const string DEMOTE_EFFECT_PREFAT_PATH = "UI/Menus/Widget/Storage/UpgradingBigSuccess";
        private const string DEMOTE_LABEL_ID = "Menu_Title_EndlessDemote";
        private const string DEMOTE_SUBTITLE_ID = "Menu_Desc_EndlessDemoteSubtitle";
        private const string DEMOTE_TITLE_ID = "Menu_Desc_EndlessDemoteTitle";
        private const string PROMOTE_EFFECT_PREFAT_PATH = "UI/Menus/Widget/Storage/UpgradingBigSuccess";
        private const string PROMOTE_LABEL_ID = "Menu_Title_EndlessPromote";
        private const string PROMOTE_SUBTITLE_ID = "Menu_Desc_EndlessPromoteSubtitle";
        private const string PROMOTE_TITLE_ID = "Menu_Desc_EndlessPromoteTitle";
        private const string PROMOTE_TO_MAX_EFFECT_PREFAT_PATH = "UI/Menus/Widget/Storage/UpgradingLargeSuccess";
        private const string STAY_EFFECT_PREFAT_PATH = "UI/Menus/Widget/Storage/UpgradingBigSuccess";
        private const string STAY_LABEL_ID = "Menu_Title_EndlessStay";
        private const string STAY_SUBTITLE_ID = "Menu_Desc_EndlessStaySubtitle";
        private const string STAY_TITLE_ID = "Menu_Desc_EndlessStayTitle";

        public EndlessSettlementDialogContext(GetLastEndlessRewardDataRsp rewardData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "EndlessSettlementDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/EndlessSettlement"
            };
            base.config = pattern;
            this._rewardData = rewardData;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Btn").GetComponent<Button>(), new UnityAction(this.OnBGClick));
        }

        private string GetDesc(string textID, int id)
        {
            if (textID == RewardUIData.ITEM_ICON_TEXT_ID)
            {
                return Singleton<StorageModule>.Instance.GetDummyStorageDataItem(id, 1).GetDisplayTitle();
            }
            return LocalizationGeneralLogic.GetText(textID, new object[0]);
        }

        private void InitRewardList()
        {
            RewardData data = this._rewardData.get_reward_list()[0];
            this._gotRewardList.Clear();
            if (data.get_exp() > 0)
            {
                RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int) data.get_exp());
                this._gotRewardList.Add(playerExpData);
            }
            if (data.get_scoin() > 0)
            {
                RewardUIData scoinData = RewardUIData.GetScoinData((int) data.get_scoin());
                this._gotRewardList.Add(scoinData);
            }
            if (data.get_hcoin() > 0)
            {
                RewardUIData hcoinData = RewardUIData.GetHcoinData((int) data.get_hcoin());
                this._gotRewardList.Add(hcoinData);
            }
            if (data.get_stamina() > 0)
            {
                RewardUIData staminaData = RewardUIData.GetStaminaData((int) data.get_stamina());
                this._gotRewardList.Add(staminaData);
            }
            if (data.get_skill_point() > 0)
            {
                RewardUIData skillPointData = RewardUIData.GetSkillPointData((int) data.get_skill_point());
                this._gotRewardList.Add(skillPointData);
            }
            if (data.get_friends_point() > 0)
            {
                RewardUIData friendPointData = RewardUIData.GetFriendPointData((int) data.get_friends_point());
                this._gotRewardList.Add(friendPointData);
            }
            foreach (RewardItemData data8 in data.get_item_list())
            {
                RewardUIData item = new RewardUIData(ResourceType.Item, (int) data8.get_num(), RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int) data8.get_id(), (int) data8.get_level());
                this._gotRewardList.Add(item);
            }
            foreach (RewardUIData data10 in this._gotRewardList)
            {
            }
        }

        public void OnBGClick()
        {
            this.Destroy();
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.EndlessSettlementAnimationEnd) && this.PlayEffect());
        }

        private bool PlayEffect()
        {
            string path = string.Empty;
            int num = (int) this._rewardData.get_group_level();
            switch (this._rewardData.get_reward_type())
            {
                case 1:
                {
                    int count = EndlessGroupMetaDataReader.GetItemList().Count;
                    if (this._rewardData.get_group_level() < (count - 1))
                    {
                        path = "UI/Menus/Widget/Storage/UpgradingBigSuccess";
                        num++;
                        break;
                    }
                    path = "UI/Menus/Widget/Storage/UpgradingLargeSuccess";
                    num = count;
                    break;
                }
                case 2:
                    path = "UI/Menus/Widget/Storage/UpgradingBigSuccess";
                    break;

                case 3:
                    path = "UI/Menus/Widget/Storage/UpgradingBigSuccess";
                    if (num > 1)
                    {
                        num--;
                    }
                    break;
            }
            UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(path)).transform.SetParent(base.view.transform, false);
            base.view.transform.Find("Dialog/Content/GroupPanel/GroupBGL/GroupIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.EndlessGroupSelectPrefabPath[num]);
            base.view.transform.Find("Btn").gameObject.SetActive(true);
            return false;
        }

        private void SetupRewardTilte()
        {
            base.view.transform.Find("Dialog/Content/Title/GradientMask/Gradient").GetComponent<Gradient>().topColor = this._groupBGColor;
            base.view.transform.Find("Dialog/Content/GroupPanel/GroupBGL").GetComponent<Image>().color = this._groupBGColor;
            base.view.transform.Find("Dialog/Content/GroupPanel/GroupName").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey((int) this._rewardData.get_group_level()).groupName, new object[0]);
            switch (this._rewardData.get_reward_type())
            {
                case 1:
                    base.view.transform.Find("Dialog/Content/Title/MainTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessPromoteTitle", new object[0]);
                    base.view.transform.Find("Dialog/Content/Title/SubTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessPromoteSubtitle", new object[0]);
                    base.view.transform.Find("Dialog/Content/GroupPanel/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_EndlessPromote", new object[0]);
                    break;

                case 2:
                    base.view.transform.Find("Dialog/Content/Title/MainTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessStayTitle", new object[0]);
                    base.view.transform.Find("Dialog/Content/Title/SubTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessStaySubtitle", new object[0]);
                    base.view.transform.Find("Dialog/Content/GroupPanel/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_EndlessStay", new object[0]);
                    break;

                case 3:
                    base.view.transform.Find("Dialog/Content/Title/MainTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessDemoteTitle", new object[0]);
                    base.view.transform.Find("Dialog/Content/Title/SubTitle").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessDemoteSubtitle", new object[0]);
                    base.view.transform.Find("Dialog/Content/GroupPanel/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_EndlessDemote", new object[0]);
                    break;
            }
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Btn").gameObject.SetActive(false);
            this._groupBGColor = Miscs.ParseColor(MiscData.Config.EndlessGroupBGColor[(int) this._rewardData.get_group_level()]);
            this.SetupRewardTilte();
            base.view.transform.Find("Dialog/Content/GroupPanel/GroupBGL/GroupIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.EndlessGroupSelectPrefabPath[(int) this._rewardData.get_group_level()]);
            Transform transform = base.view.transform.Find("Dialog/Content/GetProps");
            this.InitRewardList();
            for (int i = 1; i <= 3; i++)
            {
                Transform transform2 = transform.Find(i.ToString());
                if (i > this._gotRewardList.Count)
                {
                    transform2.gameObject.SetActive(false);
                }
                else
                {
                    RewardUIData data = this._gotRewardList[i - 1];
                    transform2.Find("Image").GetComponent<Image>().sprite = data.GetIconSprite();
                    transform2.Find("Num").GetComponent<Text>().text = data.value.ToString();
                }
            }
            Transform transform3 = base.view.transform.Find("Dialog/Content/GroupPanel/Ranking");
            transform3.Find("Up").gameObject.SetActive(false);
            transform3.Find("Flat").gameObject.SetActive(false);
            transform3.Find("Down").gameObject.SetActive(false);
            switch (this._rewardData.get_reward_type())
            {
                case 1:
                    transform3.Find("Up").gameObject.SetActive(true);
                    break;

                case 2:
                    transform3.Find("Flat").gameObject.SetActive(true);
                    break;

                case 3:
                    transform3.Find("Down").gameObject.SetActive(true);
                    break;
            }
            base.view.transform.Find("Dialog/Content/GroupPanel/Label").gameObject.SetActive(false);
            return false;
        }
    }
}

