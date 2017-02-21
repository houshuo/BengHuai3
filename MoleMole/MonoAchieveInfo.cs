namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class MonoAchieveInfo : MonoBehaviour
    {
        public UnityAction<MissionDataItem> _fetchRewardClicked;
        private MissionDataItem _missionDataItem;
        private List<RewardUIData> achieveRewardList = new List<RewardUIData>();
        public Image background;
        public Text description;
        public Button fetchButton;
        public GameObject fetchIcon;
        public GameObject iconGameObject;
        public Text progressPercentageText;
        public MonoMaskSlider progressSlider;
        public GameObject[] rewardObjects;
        public GameObject succIcon;
        public Text title;

        private void BindViewCallback(Button button, UnityAction callback)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }

        private void OnDestroy()
        {
            this.background = null;
            this.iconGameObject = null;
            this.title = null;
            this.description = null;
            this.rewardObjects = null;
            this.fetchButton = null;
            this.progressPercentageText = null;
            this.progressSlider = null;
            this.succIcon = null;
            this.fetchIcon = null;
        }

        public void OnFetchButtonClicked()
        {
            if (this._fetchRewardClicked != null)
            {
                this._fetchRewardClicked(this._missionDataItem);
            }
        }

        private void RefreshRewardList(MissionDataItem item)
        {
            this.achieveRewardList.Clear();
            RewardData rewardDataByKey = RewardDataReader.GetRewardDataByKey(item.metaData.rewardId);
            if (rewardDataByKey.RewardExp > 0)
            {
                RewardUIData playerExpData = RewardUIData.GetPlayerExpData(rewardDataByKey.RewardExp);
                playerExpData.itemID = rewardDataByKey.RewardID;
                this.achieveRewardList.Add(playerExpData);
            }
            if (rewardDataByKey.RewardSCoin > 0)
            {
                RewardUIData scoinData = RewardUIData.GetScoinData(rewardDataByKey.RewardSCoin);
                scoinData.itemID = rewardDataByKey.RewardID;
                this.achieveRewardList.Add(scoinData);
            }
            if (rewardDataByKey.RewardHCoin > 0)
            {
                RewardUIData hcoinData = RewardUIData.GetHcoinData(rewardDataByKey.RewardHCoin);
                hcoinData.itemID = rewardDataByKey.RewardID;
                this.achieveRewardList.Add(hcoinData);
            }
            if (rewardDataByKey.RewardStamina > 0)
            {
                RewardUIData staminaData = RewardUIData.GetStaminaData(rewardDataByKey.RewardStamina);
                staminaData.itemID = rewardDataByKey.RewardID;
                this.achieveRewardList.Add(staminaData);
            }
            if (rewardDataByKey.RewardSkillPoint > 0)
            {
                RewardUIData skillPointData = RewardUIData.GetSkillPointData(rewardDataByKey.RewardSkillPoint);
                skillPointData.itemID = rewardDataByKey.RewardID;
                this.achieveRewardList.Add(skillPointData);
            }
            if (rewardDataByKey.RewardFriendPoint > 0)
            {
                RewardUIData friendPointData = RewardUIData.GetFriendPointData(rewardDataByKey.RewardFriendPoint);
                friendPointData.itemID = rewardDataByKey.RewardID;
                this.achieveRewardList.Add(friendPointData);
            }
            if (rewardDataByKey.RewardItem1ID > 0)
            {
                RewardUIData data8 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem1Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem1ID, rewardDataByKey.RewardItem1Level);
                this.achieveRewardList.Add(data8);
            }
            if (rewardDataByKey.RewardItem2ID > 0)
            {
                RewardUIData data9 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem2Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem2ID, rewardDataByKey.RewardItem2Level);
                this.achieveRewardList.Add(data9);
            }
            if (rewardDataByKey.RewardItem3ID > 0)
            {
                RewardUIData data10 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem3Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem3ID, rewardDataByKey.RewardItem3Level);
                this.achieveRewardList.Add(data10);
            }
            if (rewardDataByKey.RewardItem4ID > 0)
            {
                RewardUIData data11 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem4Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem4ID, rewardDataByKey.RewardItem4Level);
                this.achieveRewardList.Add(data11);
            }
            if (rewardDataByKey.RewardItem5ID > 0)
            {
                RewardUIData data12 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem5Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem5ID, rewardDataByKey.RewardItem5Level);
                this.achieveRewardList.Add(data12);
            }
        }

        public void SetupFetchRewardButtonClickCallback(UnityAction<MissionDataItem> callback)
        {
            this._fetchRewardClicked = callback;
        }

        public void SetupRewardView(RewardUIData data, Transform rewardTrans)
        {
            <SetupRewardView>c__AnonStorey109 storey = new <SetupRewardView>c__AnonStorey109 {
                data = data,
                <>f__this = this
            };
            Text component = rewardTrans.Find("Num/Number").GetComponent<Text>();
            Image image = rewardTrans.Find("Icon").GetComponent<Image>();
            Image image2 = rewardTrans.Find("BG").GetComponent<Image>();
            MonoItemIconStar star = rewardTrans.Find("Stars").GetComponent<MonoItemIconStar>();
            image.sprite = storey.data.GetIconSprite();
            component.text = storey.data.value.ToString();
            bool flag = storey.data.rewardType == ResourceType.Item;
            star.gameObject.SetActive(flag);
            if (flag)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(storey.data.itemID, 1);
                string hexString = MiscData.Config.ItemRarityColorList[dummyStorageDataItem.rarity];
                image2.color = Miscs.ParseColor(hexString);
            }
            this.BindViewCallback(rewardTrans.Find("Button").GetComponent<Button>(), new UnityAction(storey.<>m__1BC));
        }

        public void SetupView(MissionDataItem item)
        {
            this._missionDataItem = item;
            this.title.text = LocalizationGeneralLogic.GetText(item.metaData.title, new object[0]);
            this.description.text = LocalizationGeneralLogic.GetText(item.metaData.description, new object[0]);
            this.fetchButton.gameObject.SetActive(item.status == 3);
            this.background.color = (item.status != 5) ? Color.white : Color.gray;
            this.fetchIcon.SetActive(item.status == 5);
            int num = ((item.status != 3) && (item.status != 5)) ? ((int) ((item.progress * 100f) / ((float) item.metaData.totalProgress))) : 100;
            this.progressPercentageText.text = num.ToString() + "%";
            this.progressSlider.UpdateValue(num * 0.01f, 1f, 0f);
            this.RefreshRewardList(item);
            if (!string.IsNullOrEmpty(item.metaData.thumb) && (this.iconGameObject != null))
            {
                GameObject original = Resources.Load<GameObject>(item.metaData.thumb);
                if (original != null)
                {
                    GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
                    obj3.transform.SetParent(this.iconGameObject.transform.parent);
                    RectTransform transform = obj3.transform as RectTransform;
                    RectTransform transform2 = this.iconGameObject.transform as RectTransform;
                    if ((transform != null) && (transform2 != null))
                    {
                        transform.localPosition = transform2.localPosition;
                        transform.localRotation = transform2.localRotation;
                        transform.localScale = transform2.localScale;
                    }
                    UnityEngine.Object.DestroyImmediate(this.iconGameObject);
                    this.iconGameObject = obj3;
                }
            }
            int index = 0;
            int length = this.rewardObjects.Length;
            while (index < length)
            {
                if (index < this.achieveRewardList.Count)
                {
                    this.rewardObjects[index].SetActive(true);
                    this.SetupRewardView(this.achieveRewardList[index], this.rewardObjects[index].transform);
                }
                else
                {
                    this.rewardObjects[index].SetActive(false);
                }
                index++;
            }
        }

        private void ShowRewardDetail(RewardUIData data)
        {
            UIUtil.ShowResourceDetail(data);
        }

        public int id
        {
            get
            {
                if (this._missionDataItem == null)
                {
                    return 0;
                }
                return this._missionDataItem.id;
            }
        }

        [CompilerGenerated]
        private sealed class <SetupRewardView>c__AnonStorey109
        {
            internal MonoAchieveInfo <>f__this;
            internal RewardUIData data;

            internal void <>m__1BC()
            {
                this.<>f__this.ShowRewardDetail(this.data);
            }
        }
    }
}

