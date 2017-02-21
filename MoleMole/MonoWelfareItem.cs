namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoWelfareItem : MonoBehaviour
    {
        private Action _onGetBtnClick;
        private WelfareDataItem _welfareDataItem;
        private const string expPrefabPath = "SpriteOutput/RewardGotIcons/Exp";
        private const string friendPointPrefabPath = "SpriteOutput/RewardGotIcons/FriendPoint";
        private const string hcoinPrefabPath = "SpriteOutput/RewardGotIcons/HCoin";
        private const string scoinPrefabPath = "SpriteOutput/RewardGotIcons/SCoin";
        private const string skillPointPrefabPath = "SpriteOutput/RewardGotIcons/SkillPoint";
        private const string staminaPrefabPath = "SpriteOutput/RewardGotIcons/Stamina";

        private bool CanGetVipReward()
        {
            return true;
        }

        private Transform GetRewardTrans(int typeCount, params Transform[] rewardTrans)
        {
            if (typeCount == 1)
            {
                return rewardTrans[0];
            }
            if (typeCount == 2)
            {
                return rewardTrans[1];
            }
            if (typeCount == 3)
            {
                return rewardTrans[2];
            }
            return null;
        }

        public WelfareDataItem GetWelfareDataItem()
        {
            return this._welfareDataItem;
        }

        private void HideRewardTransSomePart(Transform rewardTrans)
        {
            rewardTrans.Find("BG/UnidentifyText").gameObject.SetActive(false);
            rewardTrans.Find("NewMark").gameObject.SetActive(false);
            rewardTrans.Find("AvatarStar").gameObject.SetActive(false);
            rewardTrans.Find("Star").gameObject.SetActive(false);
            rewardTrans.Find("StigmataType").gameObject.SetActive(false);
            rewardTrans.Find("FragmentIcon").gameObject.SetActive(false);
        }

        public void OnClickRequestVipReward()
        {
            if (this.CanGetVipReward())
            {
                if (this._onGetBtnClick != null)
                {
                    this._onGetBtnClick();
                }
                Singleton<NetworkManager>.Instance.RequestGetVipReward(this._welfareDataItem.vipLevel);
            }
        }

        private void OnItemBtnClick(StorageDataItemBase itemData)
        {
            UIUtil.ShowItemDetail(itemData, true, true);
        }

        private void SetItemDefaultMaterialAndColor(Transform rewardTrans)
        {
            rewardTrans.Find("BG/Unselected").GetComponent<Image>().material = null;
            rewardTrans.Find("BG/Unselected").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
            rewardTrans.Find("BG/Image").GetComponent<Image>().material = null;
            rewardTrans.Find("BG/Image").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
            rewardTrans.Find("ItemIcon/ItemIcon").GetComponent<Image>().material = null;
            rewardTrans.Find("ItemIcon/ItemIcon").GetComponent<Image>().color = MiscData.GetColor("DropItemIconDefaultColor");
            rewardTrans.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().material = null;
            rewardTrans.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
        }

        private void SetRewardItemGrey(Transform rewardTrans)
        {
            rewardTrans.Find("BG/Unselected").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
            rewardTrans.Find("BG/Image").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
            rewardTrans.Find("ItemIcon/ItemIcon").GetComponent<Image>().color = MiscData.GetColor("DropItemIconGrey");
            Image component = rewardTrans.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>();
            if (component.material != component.defaultMaterial)
            {
                component.color = MiscData.GetColor("DropItemIconFullGrey");
            }
            else
            {
                component.color = MiscData.GetColor("DropItemIconGrey");
            }
        }

        private void SetupRewardList()
        {
            RewardData rewardDataByKey = RewardDataReader.GetRewardDataByKey(this._welfareDataItem.rewardID);
            Transform transform = base.transform.Find("InnerPanel/RewardList/right");
            Transform transform2 = base.transform.Find("InnerPanel/RewardList/center");
            Transform transform3 = base.transform.Find("InnerPanel/RewardList/left");
            transform.gameObject.SetActive(false);
            transform2.gameObject.SetActive(false);
            transform3.gameObject.SetActive(false);
            int typeCount = 0;
            List<Tuple<string, int>> list = new List<Tuple<string, int>>();
            if (rewardDataByKey.RewardExp > 0)
            {
                list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/Exp", rewardDataByKey.RewardExp));
            }
            if (rewardDataByKey.RewardSCoin > 0)
            {
                list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/SCoin", rewardDataByKey.RewardSCoin));
            }
            if (rewardDataByKey.RewardHCoin > 0)
            {
                list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/HCoin", rewardDataByKey.RewardHCoin));
            }
            if (rewardDataByKey.RewardStamina > 0)
            {
                list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/Stamina", rewardDataByKey.RewardStamina));
            }
            if (rewardDataByKey.RewardSkillPoint > 0)
            {
                list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/SkillPoint", rewardDataByKey.RewardSkillPoint));
            }
            if (rewardDataByKey.RewardFriendPoint > 0)
            {
                list.Add(new Tuple<string, int>("SpriteOutput/RewardGotIcons/FriendPoint", rewardDataByKey.RewardFriendPoint));
            }
            foreach (Tuple<string, int> tuple in list)
            {
                typeCount++;
                Transform[] rewardTrans = new Transform[] { transform3, transform2, transform };
                Transform transform4 = this.GetRewardTrans(typeCount, rewardTrans);
                if (rewardDataByKey != null)
                {
                    transform4.gameObject.SetActive(true);
                    this.HideRewardTransSomePart(transform4);
                    transform4.GetComponent<MonoLevelDropIconButton>().Clear();
                    transform4.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(tuple.Item1);
                    transform4.Find("BG/Desc").GetComponent<Text>().text = "\x00d7" + tuple.Item2.ToString();
                    if (this._welfareDataItem.rewardStatus == 3)
                    {
                        this.SetRewardItemGrey(transform4);
                    }
                    else
                    {
                        this.SetItemDefaultMaterialAndColor(transform4);
                    }
                }
            }
            List<Tuple<int, int, int>> list2 = new List<Tuple<int, int, int>>();
            if (rewardDataByKey.RewardItem1ID > 0)
            {
                list2.Add(new Tuple<int, int, int>(rewardDataByKey.RewardItem1ID, rewardDataByKey.RewardItem1Level, rewardDataByKey.RewardItem1Num));
            }
            if (rewardDataByKey.RewardItem2ID > 0)
            {
                list2.Add(new Tuple<int, int, int>(rewardDataByKey.RewardItem2ID, rewardDataByKey.RewardItem2Level, rewardDataByKey.RewardItem2Num));
            }
            if (rewardDataByKey.RewardItem3ID > 0)
            {
                list2.Add(new Tuple<int, int, int>(rewardDataByKey.RewardItem3ID, rewardDataByKey.RewardItem3Level, rewardDataByKey.RewardItem3Num));
            }
            if (rewardDataByKey.RewardItem4ID > 0)
            {
                list2.Add(new Tuple<int, int, int>(rewardDataByKey.RewardItem4ID, rewardDataByKey.RewardItem4Level, rewardDataByKey.RewardItem4Num));
            }
            if (rewardDataByKey.RewardItem5ID > 0)
            {
                list2.Add(new Tuple<int, int, int>(rewardDataByKey.RewardItem5ID, rewardDataByKey.RewardItem5Level, rewardDataByKey.RewardItem5Num));
            }
            foreach (Tuple<int, int, int> tuple2 in list2)
            {
                typeCount++;
                Transform[] transformArray2 = new Transform[] { transform3, transform2, transform };
                Transform transform5 = this.GetRewardTrans(typeCount, transformArray2);
                if (rewardDataByKey != null)
                {
                    transform5.gameObject.SetActive(true);
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(tuple2.Item1, tuple2.Item2);
                    dummyStorageDataItem.number = tuple2.Item3;
                    bool isGrey = this._welfareDataItem.rewardStatus == 3;
                    transform5.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, new DropItemButtonClickCallBack(this.OnItemBtnClick), true, false, isGrey, false);
                }
            }
        }

        public void SetupView(WelfareDataItem welfareDataItem, Action onGetBtnClick = null)
        {
            this._welfareDataItem = welfareDataItem;
            this._onGetBtnClick = onGetBtnClick;
            if (welfareDataItem.rewardStatus == 2)
            {
                base.transform.Find("InnerPanel/BG/Get").gameObject.SetActive(true);
                base.transform.Find("InnerPanel/BG/Unget").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/BG/Over").gameObject.SetActive(false);
                object[] replaceParams = new object[] { welfareDataItem.payHCoin };
                base.transform.Find("InnerPanel/BG/Get/Desc/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopWelfareItemPayDesc", replaceParams);
                base.transform.Find("InnerPanel/Reward/RewardNo1").gameObject.SetActive(true);
                base.transform.Find("InnerPanel/Reward/RewardNo2").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/Reward/RewardNo3").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/Reward/RewardNo1/Num/Num/num").GetComponent<Text>().text = welfareDataItem.vipLevel.ToString();
                base.transform.Find("InnerPanel/GetBtn").gameObject.SetActive(true);
                base.transform.Find("InnerPanel/ProgressPanel").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/CompletePanel").gameObject.SetActive(false);
            }
            else if (welfareDataItem.rewardStatus == 1)
            {
                base.transform.Find("InnerPanel/BG/Unget").gameObject.SetActive(true);
                base.transform.Find("InnerPanel/BG/Get").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/BG/Over").gameObject.SetActive(false);
                object[] objArray2 = new object[] { welfareDataItem.payHCoin };
                base.transform.Find("InnerPanel/BG/Unget/Desc/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopWelfareItemPayDesc", objArray2);
                base.transform.Find("InnerPanel/Reward/RewardNo2").gameObject.SetActive(true);
                base.transform.Find("InnerPanel/Reward/RewardNo1").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/Reward/RewardNo3").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/Reward/RewardNo2/Num/Num/num").GetComponent<Text>().text = welfareDataItem.vipLevel.ToString();
                base.transform.Find("InnerPanel/ProgressPanel").gameObject.SetActive(true);
                base.transform.Find("InnerPanel/GetBtn").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/CompletePanel").gameObject.SetActive(false);
                int totalPayHCoin = Singleton<ShopWelfareModule>.Instance.totalPayHCoin;
                base.transform.Find("InnerPanel/ProgressPanel/HCoin/HCoin/Num").GetComponent<Text>().text = (welfareDataItem.payHCoin - totalPayHCoin).ToString();
                base.transform.Find("InnerPanel/ProgressPanel/ProgressBar").GetComponent<MonoMaskSlider>().UpdateValue((float) totalPayHCoin, (float) welfareDataItem.payHCoin, 0f);
            }
            if (welfareDataItem.rewardStatus == 3)
            {
                base.transform.Find("InnerPanel/BG/Over").gameObject.SetActive(true);
                base.transform.Find("InnerPanel/BG/Unget").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/BG/Get").gameObject.SetActive(false);
                object[] objArray3 = new object[] { welfareDataItem.payHCoin };
                base.transform.Find("InnerPanel/BG/Over/Desc/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopWelfareItemPayDescHasGot", objArray3);
                base.transform.Find("InnerPanel/Reward/RewardNo3").gameObject.SetActive(true);
                base.transform.Find("InnerPanel/Reward/RewardNo1").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/Reward/RewardNo2").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/Reward/RewardNo3/Num/Num/num").GetComponent<Text>().text = welfareDataItem.vipLevel.ToString();
                base.transform.Find("InnerPanel/CompletePanel").gameObject.SetActive(true);
                base.transform.Find("InnerPanel/GetBtn").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/ProgressPanel").gameObject.SetActive(false);
            }
            this.SetupRewardList();
        }
    }
}

