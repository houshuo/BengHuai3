namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoInviteRewardRow : MonoBehaviour
    {
        private InviteeFriendRewardData _inviteeRewardData;
        private InviteFriendRewardData _inviterRewardData;
        private InviteTab _inviteType;
        private bool _isAcceptInvitation;
        private List<RewardUIData> _rewardUIDataList;

        private void DoSetupView()
        {
            Text component = base.transform.Find("Label/Text").GetComponent<Text>();
            if (this._inviteType == InviteTab.InviteeTab)
            {
                component.gameObject.SetActive(this._inviteeRewardData.get_levelSpecified());
                object[] replaceParams = new object[] { this._inviteeRewardData.get_level() };
                component.text = LocalizationGeneralLogic.GetText("InviteReward_Limit", replaceParams);
            }
            else
            {
                component.gameObject.SetActive(this._inviterRewardData.get_levelSpecified());
                object[] objArray2 = new object[] { this._inviterRewardData.get_level() };
                component.text = LocalizationGeneralLogic.GetText("InvitateeReward_Limit", objArray2);
            }
            if (this._inviteType == InviteTab.InviterTab)
            {
                base.transform.Find("Label/Current").gameObject.SetActive(true);
                base.transform.Find("Label/DivisionSign").gameObject.SetActive(true);
                base.transform.Find("Label/Max").gameObject.SetActive(true);
                base.transform.Find("Label/Current").GetComponent<Text>().text = this._inviterRewardData.get_cur_num().ToString();
                base.transform.Find("Label/Current").GetComponent<Text>().color = (this._inviterRewardData.get_cur_num() < this._inviterRewardData.get_max_num()) ? MiscData.GetColor("WarningRed") : MiscData.GetColor("Blue");
                base.transform.Find("Label/Max").GetComponent<Text>().text = this._inviterRewardData.get_max_num().ToString();
            }
            else
            {
                base.transform.Find("Label/Current").gameObject.SetActive(false);
                base.transform.Find("Label/DivisionSign").gameObject.SetActive(false);
                base.transform.Find("Label/Max").gameObject.SetActive(false);
            }
            this.SetupRewardList();
            Transform transform = base.transform.Find("Content");
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(i < this._rewardUIDataList.Count);
                if (i < this._rewardUIDataList.Count)
                {
                    RewardUIData data = this._rewardUIDataList[i];
                    Image image = child.Find("ItemIcon").GetComponent<Image>();
                    Image image2 = child.Find("ItemIcon/Icon").GetComponent<Image>();
                    image.gameObject.SetActive(true);
                    child.Find("SelectedMark").gameObject.SetActive(false);
                    child.Find("ProtectedMark").gameObject.SetActive(false);
                    child.Find("InteractiveMask").gameObject.SetActive(false);
                    child.Find("NotEnough").gameObject.SetActive(false);
                    child.Find("Star").gameObject.SetActive(false);
                    child.Find("StigmataType").gameObject.SetActive(false);
                    child.Find("UnidentifyText").gameObject.SetActive(false);
                    child.Find("QuestionMark").gameObject.SetActive(false);
                    child.Find("ItemIcon").GetComponent<Image>().color = Color.white;
                    child.Find("ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[0]);
                    if (data.rewardType == ResourceType.Item)
                    {
                        StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID, data.level);
                        dummyStorageDataItem.number = data.value;
                        MonoItemIconButton button = child.GetComponent<MonoItemIconButton>();
                        button.SetupView(dummyStorageDataItem, MonoItemIconButton.SelectMode.None, false, false, false);
                        button.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnItemClick));
                    }
                    else
                    {
                        image2.sprite = data.GetIconSprite();
                        child.Find("Text").GetComponent<Text>().text = "\x00d7" + data.value;
                    }
                }
            }
            if (this._inviteType == InviteTab.InviteeTab)
            {
                base.transform.Find("AlreadyIssued").gameObject.SetActive(this._isAcceptInvitation && (Singleton<PlayerModule>.Instance.playerData.teamLevel >= this._inviteeRewardData.get_level()));
            }
            else
            {
                base.transform.Find("AlreadyIssued").gameObject.SetActive(this._inviterRewardData.get_cur_num() >= this._inviterRewardData.get_max_num());
            }
        }

        private void OnItemClick(StorageDataItemBase itemData, bool selected)
        {
            UIUtil.ShowItemDetail(itemData, false, true);
        }

        private void SetupRewardList()
        {
            this._rewardUIDataList = new List<RewardUIData>();
            if (((this._inviteType != InviteTab.InviteeTab) || (this._inviteeRewardData.get_reward_list().Count >= 1)) && ((this._inviteType != InviteTab.InviterTab) || (this._inviterRewardData.get_reward_list().Count >= 1)))
            {
                RewardData data = (this._inviteType != InviteTab.InviteeTab) ? this._inviterRewardData.get_reward_list()[0] : this._inviteeRewardData.get_reward_list()[0];
                if (data.get_exp() > 0)
                {
                    RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int) data.get_exp());
                    this._rewardUIDataList.Add(playerExpData);
                }
                if (data.get_scoin() > 0)
                {
                    RewardUIData scoinData = RewardUIData.GetScoinData((int) data.get_scoin());
                    this._rewardUIDataList.Add(scoinData);
                }
                if (data.get_hcoin() > 0)
                {
                    RewardUIData hcoinData = RewardUIData.GetHcoinData((int) data.get_hcoin());
                    this._rewardUIDataList.Add(hcoinData);
                }
                if (data.get_stamina() > 0)
                {
                    RewardUIData staminaData = RewardUIData.GetStaminaData((int) data.get_stamina());
                    this._rewardUIDataList.Add(staminaData);
                }
                if (data.get_skill_point() > 0)
                {
                    RewardUIData skillPointData = RewardUIData.GetSkillPointData((int) data.get_skill_point());
                    this._rewardUIDataList.Add(skillPointData);
                }
                if (data.get_friends_point() > 0)
                {
                    RewardUIData friendPointData = RewardUIData.GetFriendPointData((int) data.get_friends_point());
                    this._rewardUIDataList.Add(friendPointData);
                }
                foreach (RewardItemData data8 in data.get_item_list())
                {
                    RewardUIData item = new RewardUIData(ResourceType.Item, (int) data8.get_num(), RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int) data8.get_id(), (int) data8.get_level());
                    this._rewardUIDataList.Add(item);
                }
            }
        }

        public void SetupView(InviteFriendRewardData rewardData)
        {
            this._inviteType = InviteTab.InviterTab;
            this._inviteeRewardData = null;
            this._inviterRewardData = rewardData;
            this.DoSetupView();
        }

        public void SetupView(bool isAcceptInvitation, InviteeFriendRewardData rewardData)
        {
            this._inviteType = InviteTab.InviteeTab;
            this._isAcceptInvitation = isAcceptInvitation;
            this._inviteeRewardData = rewardData;
            this._inviterRewardData = null;
            this.DoSetupView();
        }
    }
}

