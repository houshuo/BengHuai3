namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class MonoMissionInfo : MonoBehaviour
    {
        private static Material _grayMat;
        private UnityEventBase _leftClickEvent;
        private RewardUIData _leftData;
        private MissionDataItem _missionData;
        [SerializeField]
        private Sprite _normalBG;
        private FetchRewardCallBack _onFetchRewardBtnClick;
        private GoMissionCallBack _onGoMissionBtnClick;
        [SerializeField]
        private Sprite _readyBG;
        private UnityEventBase _rightClickEvent;
        private RewardUIData _rightData;
        private Transform _time_root;
        private Text _timeLable_comp;
        private Text _timeNumber_comp;
        public const string ITEM_ICON_PREFAB_PATH = "ItemIconPrefabPath";
        private const string MATERIAL_GRAY_PATH = "Material/ImageGrayscale";
        private List<RewardUIData> missionRewardList = new List<RewardUIData>();

        private void ClearAllViews()
        {
            base.transform.Find("Buttons/Fetch").gameObject.SetActive(false);
            base.transform.Find("Buttons/Go").gameObject.SetActive(false);
            base.transform.Find("ProgressBar").gameObject.SetActive(false);
            base.transform.Find("ProgressText").gameObject.SetActive(false);
            base.transform.Find("StatusInfo/Finish").gameObject.SetActive(false);
            base.transform.Find("StatusInfo/Closed").gameObject.SetActive(false);
            base.transform.Find("StatusInfo/NotBegin").gameObject.SetActive(false);
            base.transform.Find("TypeInfo/Linear").gameObject.SetActive(false);
            base.transform.Find("TypeInfo/Branch").gameObject.SetActive(false);
            base.transform.Find("TypeInfo/Bounty").gameObject.SetActive(false);
            base.transform.Find("TypeInfo/Timed").gameObject.SetActive(false);
            base.transform.Find("TypeInfo/Touch").gameObject.SetActive(false);
            base.transform.Find("Rewards/Left").gameObject.SetActive(false);
            base.transform.Find("Rewards/Right").gameObject.SetActive(false);
            base.transform.Find("Rewards/Left/RewardItem/Star").gameObject.SetActive(false);
            base.transform.Find("Rewards/Left/RewardItem/AvatarStar").gameObject.SetActive(false);
            base.transform.Find("Rewards/Right/RewardItem/Star").gameObject.SetActive(false);
            base.transform.Find("Rewards/Right/RewardItem/AvatarStar").gameObject.SetActive(false);
            this.Time_root.gameObject.SetActive(false);
            base.transform.Find("Rewards/Left/RewardItem/x").gameObject.SetActive(true);
            base.transform.Find("Rewards/Left/RewardItem/Number").gameObject.SetActive(true);
            base.transform.Find("Rewards/Right/RewardItem/x").gameObject.SetActive(true);
            base.transform.Find("Rewards/Right/RewardItem/Number").gameObject.SetActive(true);
            this.ClearEvents();
        }

        private void ClearEvents()
        {
            if (this._leftClickEvent != null)
            {
                this._leftClickEvent.RemoveAllListeners();
                this._leftClickEvent = null;
            }
            if (this._rightClickEvent != null)
            {
                this._rightClickEvent.RemoveAllListeners();
                this._rightClickEvent = null;
            }
        }

        public MissionDataItem GetMissionData()
        {
            return this._missionData;
        }

        private bool IsMissionActive()
        {
            return ((this._missionData.status == 2) || (this._missionData.status == 3));
        }

        private bool IsTimedMissionNotBegin()
        {
            return ((this._missionData.metaData.type == 3) && (this._missionData.status == 1));
        }

        private void OnDestroy()
        {
            this.ClearEvents();
        }

        public void OnFetchRewardBtnClick()
        {
            if (this._onFetchRewardBtnClick != null)
            {
                this._onFetchRewardBtnClick(this._missionData);
            }
        }

        public void OnGoMissionBtnClick()
        {
            if (this._onGoMissionBtnClick != null)
            {
                this._onGoMissionBtnClick(this._missionData);
            }
        }

        private void RefreshRewardList()
        {
            this.missionRewardList.Clear();
            RewardData rewardDataByKey = RewardDataReader.GetRewardDataByKey(this._missionData.metaData.rewardId);
            if (rewardDataByKey.RewardExp > 0)
            {
                RewardUIData playerExpData = RewardUIData.GetPlayerExpData(rewardDataByKey.RewardExp);
                playerExpData.itemID = rewardDataByKey.RewardID;
                this.missionRewardList.Add(playerExpData);
            }
            if (rewardDataByKey.RewardSCoin > 0)
            {
                RewardUIData scoinData = RewardUIData.GetScoinData(rewardDataByKey.RewardSCoin);
                scoinData.itemID = rewardDataByKey.RewardID;
                this.missionRewardList.Add(scoinData);
            }
            if (rewardDataByKey.RewardHCoin > 0)
            {
                RewardUIData hcoinData = RewardUIData.GetHcoinData(rewardDataByKey.RewardHCoin);
                hcoinData.itemID = rewardDataByKey.RewardID;
                this.missionRewardList.Add(hcoinData);
            }
            if (rewardDataByKey.RewardStamina > 0)
            {
                RewardUIData staminaData = RewardUIData.GetStaminaData(rewardDataByKey.RewardStamina);
                staminaData.itemID = rewardDataByKey.RewardID;
                this.missionRewardList.Add(staminaData);
            }
            if (rewardDataByKey.RewardSkillPoint > 0)
            {
                RewardUIData skillPointData = RewardUIData.GetSkillPointData(rewardDataByKey.RewardSkillPoint);
                skillPointData.itemID = rewardDataByKey.RewardID;
                this.missionRewardList.Add(skillPointData);
            }
            if (rewardDataByKey.RewardFriendPoint > 0)
            {
                RewardUIData friendPointData = RewardUIData.GetFriendPointData(rewardDataByKey.RewardFriendPoint);
                friendPointData.itemID = rewardDataByKey.RewardID;
                this.missionRewardList.Add(friendPointData);
            }
            if (rewardDataByKey.RewardItem1ID > 0)
            {
                RewardUIData item = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem1Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem1ID, rewardDataByKey.RewardItem1Level);
                this.missionRewardList.Add(item);
            }
            if (rewardDataByKey.RewardItem2ID > 0)
            {
                RewardUIData data9 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem2Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem2ID, rewardDataByKey.RewardItem2Level);
                this.missionRewardList.Add(data9);
            }
            if (rewardDataByKey.RewardItem3ID > 0)
            {
                RewardUIData data10 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem3Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem3ID, rewardDataByKey.RewardItem3Level);
                this.missionRewardList.Add(data10);
            }
            if (rewardDataByKey.RewardItem4ID > 0)
            {
                RewardUIData data11 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem4Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem4ID, rewardDataByKey.RewardItem4Level);
                this.missionRewardList.Add(data11);
            }
            if (rewardDataByKey.RewardItem5ID > 0)
            {
                RewardUIData data12 = new RewardUIData(ResourceType.Item, rewardDataByKey.RewardItem5Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardDataByKey.RewardItem5ID, rewardDataByKey.RewardItem5Level);
                this.missionRewardList.Add(data12);
            }
        }

        public void RegisterCallBacks(FetchRewardCallBack onFetch, GoMissionCallBack onGo)
        {
            this._onFetchRewardBtnClick = onFetch;
            this._onGoMissionBtnClick = onGo;
        }

        private void SetLeftTimeUI(DateTime from, DateTime to)
        {
            string str;
            int num = Miscs.GetDiffTimeToShow(from, to, out str);
            if (num <= 0)
            {
                num = 1;
            }
            this.TimeNumber_comp.text = num.ToString();
            this.TimeLable_comp.text = str;
        }

        private void SetRarity(Transform tran, RewardUIData data)
        {
            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID, data.level);
            if (!(dummyStorageDataItem is AvatarFragmentDataItem))
            {
                if (dummyStorageDataItem is AvatarCardDataItem)
                {
                    tran.Find("x").gameObject.SetActive(false);
                    tran.Find("Number").gameObject.SetActive(false);
                    Transform transform = tran.Find("AvatarStar");
                    transform.gameObject.SetActive(true);
                    AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(data.itemID).avatarID);
                    transform.GetComponent<MonoAvatarStar>().SetupView(dummyAvatarDataItem.star);
                }
                else
                {
                    Transform transform2 = tran.Find("Star");
                    transform2.gameObject.SetActive(true);
                    int rarity = dummyStorageDataItem.rarity;
                    if (dummyStorageDataItem is WeaponDataItem)
                    {
                        rarity = (dummyStorageDataItem as WeaponDataItem).GetMaxRarity();
                    }
                    else if (dummyStorageDataItem is StigmataDataItem)
                    {
                        rarity = (dummyStorageDataItem as StigmataDataItem).GetMaxRarity();
                    }
                    transform2.GetComponent<MonoItemIconStar>().SetupView(dummyStorageDataItem.rarity, rarity);
                }
            }
        }

        private void SetupBGView()
        {
            Image component = base.GetComponent<Image>();
            if (this.IsMissionActive())
            {
                if (this._missionData.status == 2)
                {
                    component.sprite = this._normalBG;
                }
                else if (this._missionData.status == 3)
                {
                    component.sprite = this._readyBG;
                }
                component.material = null;
            }
            else
            {
                component.sprite = this._normalBG;
                component.material = _grayMat;
            }
        }

        private void SetupButtonsView()
        {
            if (this._missionData.status == 3)
            {
                if (this.missionRewardList.Count > 0)
                {
                    base.transform.Find("Buttons/Fetch").gameObject.SetActive(true);
                }
            }
            else if ((this._missionData.status == 2) && (this._missionData.metaData.LinkType != 0))
            {
                base.transform.Find("Buttons/Go").gameObject.SetActive(true);
            }
        }

        private void SetupItemFrame(Image frameImage, RewardUIData data)
        {
            ResourceType rewardType = data.rewardType;
            if (rewardType == ResourceType.Hcoin)
            {
                frameImage.sprite = Miscs.GetSpriteByPrefab("SpriteOutput/ItemFrame/FrameComPurple");
            }
            else if (rewardType == ResourceType.Item)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID, 1);
                frameImage.sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[dummyStorageDataItem.rarity]);
            }
            else
            {
                frameImage.sprite = Miscs.GetSpriteByPrefab("SpriteOutput/ItemFrame/FrameComBlue");
            }
        }

        private void SetupMissionIconView()
        {
            Image component = base.transform.Find("MissionIcon/Color").GetComponent<Image>();
            component.sprite = Miscs.GetSpriteByPrefab(this._missionData.metaData.thumb);
            Material material = !this.IsMissionActive() ? _grayMat : null;
            component.material = material;
        }

        private void SetupProgressView()
        {
            if (this._missionData.status == 2)
            {
                int progress = this._missionData.progress;
                int totalProgress = this._missionData.metaData.totalProgress;
                Transform transform = base.transform.Find("ProgressBar");
                transform.gameObject.SetActive(true);
                transform.GetComponent<MonoMaskSlider>().UpdateValue((float) progress, (float) totalProgress, 0f);
                Transform transform2 = base.transform.Find("ProgressText");
                transform2.gameObject.SetActive(true);
                transform2.Find("current").GetComponent<Text>().text = progress.ToString();
                transform2.Find("total").GetComponent<Text>().text = totalProgress.ToString();
            }
        }

        private void SetupRewardView()
        {
            this.RefreshRewardList();
            if (this.missionRewardList.Count != 0)
            {
                RewardUIData data;
                RewardUIData data2 = null;
                this._leftData = null;
                this._rightData = null;
                if (this.missionRewardList.Count == 1)
                {
                    data = null;
                    data2 = this.missionRewardList[0];
                }
                else
                {
                    data = this.missionRewardList[0];
                    data2 = this.missionRewardList[1];
                }
                this._leftData = data;
                this._rightData = data2;
                if (data != null)
                {
                    Color color;
                    base.transform.Find("Rewards/Left").gameObject.SetActive(true);
                    base.transform.Find("Rewards/Left/RewardItem/Icon").GetComponent<Image>().sprite = data.GetIconSprite();
                    base.transform.Find("Rewards/Left/RewardItem/Number").GetComponent<Text>().text = string.Format("{0}", data.value);
                    Text component = base.transform.Find("Rewards/Left/RewardItem/x").GetComponent<Text>();
                    string str = !this.IsMissionActive() ? "##96b1c0FF" : "43C6FCFF";
                    UIUtil.TryParseHexString(str, out color);
                    component.color = color;
                    base.transform.Find("Rewards/Left/RewardItem/Icon").GetComponent<Image>().material = !this.IsMissionActive() ? _grayMat : null;
                    if (data.rewardType == ResourceType.Item)
                    {
                        this.SetRarity(base.transform.Find("Rewards/Left/RewardItem"), data);
                    }
                    Button button = base.transform.Find("Rewards/Left/ShowDetailBtn").GetComponent<Button>();
                    this._leftClickEvent = button.onClick;
                    this._leftClickEvent.RemoveAllListeners();
                    button.onClick.AddListener(new UnityAction(this.ShowDetailDialog_Left));
                }
                if (data2 != null)
                {
                    Color color2;
                    base.transform.Find("Rewards/Right").gameObject.SetActive(true);
                    base.transform.Find("Rewards/Right/RewardItem/Icon").GetComponent<Image>().sprite = data2.GetIconSprite();
                    base.transform.Find("Rewards/Right/RewardItem/Number").GetComponent<Text>().text = string.Format("{0}", data2.value);
                    Text text4 = base.transform.Find("Rewards/Right/RewardItem/x").GetComponent<Text>();
                    string str2 = !this.IsMissionActive() ? "##96b1c0FF" : "43C6FCFF";
                    UIUtil.TryParseHexString(str2, out color2);
                    text4.color = color2;
                    base.transform.Find("Rewards/Right/RewardItem/Icon").GetComponent<Image>().material = !this.IsMissionActive() ? _grayMat : null;
                    if (data2.rewardType == ResourceType.Item)
                    {
                        this.SetRarity(base.transform.Find("Rewards/Right/RewardItem"), data2);
                    }
                    Button button2 = base.transform.Find("Rewards/Right/ShowDetailBtn").GetComponent<Button>();
                    this._rightClickEvent = button2.onClick;
                    this._rightClickEvent.RemoveAllListeners();
                    button2.onClick.AddListener(new UnityAction(this.ShowDetailDialog_Right));
                }
            }
        }

        private void SetupStatusInfoView()
        {
            if (this._missionData.status == 3)
            {
                base.transform.Find("StatusInfo/Finish").gameObject.SetActive(true);
            }
            else if (this._missionData.status == 5)
            {
                base.transform.Find("StatusInfo/Closed").gameObject.SetActive(true);
            }
            else if (this._missionData.status == 1)
            {
                base.transform.Find("StatusInfo/NotBegin").gameObject.SetActive(true);
            }
        }

        private void SetupTimeView()
        {
            if (this._missionData.metaData.type == 3)
            {
                if (this._missionData.status == 1)
                {
                    DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp((uint) this._missionData.beginTime);
                    this.Time_root.gameObject.SetActive(true);
                    this.SetLeftTimeUI(TimeUtil.Now, dateTimeFromTimeStamp);
                }
                else if ((this._missionData.status == 2) || (this._missionData.status == 3))
                {
                    DateTime to = Miscs.GetDateTimeFromTimeStamp((uint) this._missionData.endTime);
                    this.Time_root.gameObject.SetActive(true);
                    this.SetLeftTimeUI(TimeUtil.Now, to);
                }
                else
                {
                    this.Time_root.gameObject.SetActive(false);
                }
            }
        }

        private void SetupTitleView()
        {
            Color color;
            base.transform.Find("Title/title").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(this._missionData.metaData.title, new object[0]);
            base.transform.Find("Title/splash").GetComponent<Image>().material = !this.IsMissionActive() ? _grayMat : null;
            Text component = base.transform.Find("Title/description").GetComponent<Text>();
            component.text = LocalizationGeneralLogic.GetText(this._missionData.metaData.description, new object[0]);
            string str = !this.IsMissionActive() ? "#a0a0a0FF" : "00CAFFFF";
            UIUtil.TryParseHexString(str, out color);
            component.color = color;
        }

        private void SetupTypeView()
        {
            if (this._missionData.metaData.type == 1)
            {
                if (this._missionData.metaData.subType == 1)
                {
                    base.transform.Find("TypeInfo/Branch").gameObject.SetActive(true);
                }
                else
                {
                    base.transform.Find("TypeInfo/Linear").gameObject.SetActive(true);
                }
            }
            else if (this._missionData.metaData.type == 2)
            {
                base.transform.Find("TypeInfo/Bounty").gameObject.SetActive(true);
            }
            else if (this._missionData.metaData.type == 3)
            {
                base.transform.Find("TypeInfo/Timed").gameObject.SetActive(true);
            }
            else if (this._missionData.metaData.type == 4)
            {
                base.transform.Find("TypeInfo/Touch").gameObject.SetActive(true);
            }
        }

        public void SetupView(MissionDataItem missionData)
        {
            if (_grayMat == null)
            {
                _grayMat = Miscs.LoadResource<Material>("Material/ImageGrayscale", BundleType.RESOURCE_FILE);
            }
            this._missionData = missionData;
            this.ClearAllViews();
            this.SetupBGView();
            this.SetupTypeView();
            this.SetupMissionIconView();
            this.SetupStatusInfoView();
            this.SetupProgressView();
            this.SetupTitleView();
            this.SetupRewardView();
            this.SetupButtonsView();
            this.SetupTimeView();
        }

        private void ShowDetailDialog(RewardUIData data)
        {
            UIUtil.ShowResourceDetail(data);
        }

        private void ShowDetailDialog_Left()
        {
            if (this._leftData != null)
            {
                this.ShowDetailDialog(this._leftData);
            }
        }

        private void ShowDetailDialog_Right()
        {
            if (this._rightData != null)
            {
                this.ShowDetailDialog(this._rightData);
            }
        }

        private void Update()
        {
            if (this._missionData.metaData.type == 3)
            {
                if (this._missionData.status == 1)
                {
                    if (DateTime.Compare(TimeUtil.Now, Miscs.GetDateTimeFromTimeStamp((uint) this._missionData.beginTime)) < 0)
                    {
                        DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp((uint) this._missionData.beginTime);
                        this.SetLeftTimeUI(TimeUtil.Now, dateTimeFromTimeStamp);
                    }
                }
                else if (((this._missionData.status == 2) || (this._missionData.status == 3)) && (DateTime.Compare(TimeUtil.Now, Miscs.GetDateTimeFromTimeStamp((uint) this._missionData.endTime)) < 0))
                {
                    DateTime to = Miscs.GetDateTimeFromTimeStamp((uint) this._missionData.endTime);
                    this.SetLeftTimeUI(TimeUtil.Now, to);
                }
            }
        }

        public Transform Time_root
        {
            get
            {
                if (this._time_root == null)
                {
                    this._time_root = base.transform.Find("LeftTime");
                }
                return this._time_root;
            }
        }

        public Text TimeLable_comp
        {
            get
            {
                if (this._timeLable_comp == null)
                {
                    this._timeLable_comp = base.transform.Find("LeftTime/Label").GetComponent<Text>();
                }
                return this._timeLable_comp;
            }
        }

        public Text TimeNumber_comp
        {
            get
            {
                if (this._timeNumber_comp == null)
                {
                    this._timeNumber_comp = base.transform.Find("LeftTime/TimeValue").GetComponent<Text>();
                }
                return this._timeNumber_comp;
            }
        }
    }
}

