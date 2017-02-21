namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoRankRewardRow : MonoBehaviour
    {
        private List<RewardUIData> _rankRewardDataList = new List<RewardUIData>();
        private int _rewardID;
        private const string REWARD_ITEM_PREFAB_PATH = "UI/Menus/Widget/EndlessActivity/RewardItem";

        private void InitRewardList()
        {
            this._rankRewardDataList.Clear();
            RewardData rewardDataByKey = RewardDataReader.GetRewardDataByKey(this._rewardID);
            if (rewardDataByKey != null)
            {
                if (rewardDataByKey.RewardExp > 0)
                {
                    RewardUIData playerExpData = RewardUIData.GetPlayerExpData(rewardDataByKey.RewardExp);
                    playerExpData.itemID = rewardDataByKey.RewardID;
                    this._rankRewardDataList.Add(playerExpData);
                }
                if (rewardDataByKey.RewardSCoin > 0)
                {
                    RewardUIData scoinData = RewardUIData.GetScoinData(rewardDataByKey.RewardSCoin);
                    scoinData.itemID = rewardDataByKey.RewardID;
                    this._rankRewardDataList.Add(scoinData);
                }
                if (rewardDataByKey.RewardHCoin > 0)
                {
                    RewardUIData hcoinData = RewardUIData.GetHcoinData(rewardDataByKey.RewardHCoin);
                    hcoinData.itemID = rewardDataByKey.RewardID;
                    this._rankRewardDataList.Add(hcoinData);
                }
                if (rewardDataByKey.RewardStamina > 0)
                {
                    RewardUIData staminaData = RewardUIData.GetStaminaData(rewardDataByKey.RewardStamina);
                    staminaData.itemID = rewardDataByKey.RewardID;
                    this._rankRewardDataList.Add(staminaData);
                }
                if (rewardDataByKey.RewardSkillPoint > 0)
                {
                    RewardUIData skillPointData = RewardUIData.GetSkillPointData(rewardDataByKey.RewardSkillPoint);
                    skillPointData.itemID = rewardDataByKey.RewardID;
                    this._rankRewardDataList.Add(skillPointData);
                }
                if (rewardDataByKey.RewardFriendPoint > 0)
                {
                    RewardUIData friendPointData = RewardUIData.GetFriendPointData(rewardDataByKey.RewardFriendPoint);
                    friendPointData.itemID = rewardDataByKey.RewardID;
                    this._rankRewardDataList.Add(friendPointData);
                }
                if (rewardDataByKey.RewardItem1ID > 0)
                {
                    RewardUIData item = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem1Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem1ID, rewardDataByKey.RewardItem1Level);
                    this._rankRewardDataList.Add(item);
                }
                if (rewardDataByKey.RewardItem2ID > 0)
                {
                    RewardUIData data9 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem2Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem2ID, rewardDataByKey.RewardItem2Level);
                    this._rankRewardDataList.Add(data9);
                }
                if (rewardDataByKey.RewardItem3ID > 0)
                {
                    RewardUIData data10 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem3Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem3ID, rewardDataByKey.RewardItem3Level);
                    this._rankRewardDataList.Add(data10);
                }
                if (rewardDataByKey.RewardItem4ID > 0)
                {
                    RewardUIData data11 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem4Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem4ID, rewardDataByKey.RewardItem4Level);
                    this._rankRewardDataList.Add(data11);
                }
                if (rewardDataByKey.RewardItem5ID > 0)
                {
                    RewardUIData data12 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem5Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem5ID, rewardDataByKey.RewardItem5Level);
                    this._rankRewardDataList.Add(data12);
                }
            }
        }

        public void SetupView(int rewardID)
        {
            this._rewardID = rewardID;
            this.InitRewardList();
            Transform trans = base.transform.Find("RewardList/RewardContainer");
            if (this._rankRewardDataList.Count <= 0)
            {
                trans.gameObject.SetActive(false);
                base.transform.Find("RewardList/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Label_EndlessNoReward", new object[0]);
            }
            else
            {
                trans.gameObject.SetActive(true);
                base.transform.Find("RewardList/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Label_Reward", new object[0]);
                trans.DestroyChildren();
                foreach (RewardUIData data in this._rankRewardDataList)
                {
                    Transform transform = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI/Menus/Widget/EndlessActivity/RewardItem")).transform;
                    transform.SetParent(trans, false);
                    transform.GetComponent<MonoRewardItem>().SetupView(data);
                }
            }
        }
    }
}

