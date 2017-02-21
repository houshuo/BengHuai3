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

    public class MissionRewardGotDialogContext : BaseDialogContext
    {
        private SequenceAnimationManager _animationManager;
        private AvatarCardDataItem _avatarData;
        private List<DropItem> _dropItemList;
        private List<RewardUIData> _missionRewardList = new List<RewardUIData>();
        private OnDialogDestroy _onDestroy;
        private List<RewardData> _rewardDataList;

        public MissionRewardGotDialogContext(List<RewardData> rewardDataList, List<DropItem> dropList = null)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "MissionRewardGotDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/MissionRewardGotDialog"
            };
            base.config = pattern;
            this._rewardDataList = rewardDataList;
            this._dropItemList = dropList;
            this._avatarData = null;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
        }

        private void ClearViews()
        {
            base.view.transform.Find("Dialog/Content/RewardList/center").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/RewardList/left").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/RewardList/right").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/TextList/line1").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/TextList/line2").gameObject.SetActive(false);
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
            RewardData data = this._rewardDataList[0];
            this._missionRewardList.Clear();
            if (data.get_exp() > 0)
            {
                RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int) data.get_exp());
                this._missionRewardList.Add(playerExpData);
            }
            if (data.get_scoin() > 0)
            {
                RewardUIData scoinData = RewardUIData.GetScoinData((int) data.get_scoin());
                this._missionRewardList.Add(scoinData);
            }
            if (data.get_hcoin() > 0)
            {
                RewardUIData hcoinData = RewardUIData.GetHcoinData((int) data.get_hcoin());
                this._missionRewardList.Add(hcoinData);
            }
            if (data.get_stamina() > 0)
            {
                RewardUIData staminaData = RewardUIData.GetStaminaData((int) data.get_stamina());
                this._missionRewardList.Add(staminaData);
            }
            if (data.get_skill_point() > 0)
            {
                RewardUIData skillPointData = RewardUIData.GetSkillPointData((int) data.get_skill_point());
                this._missionRewardList.Add(skillPointData);
            }
            if (data.get_friends_point() > 0)
            {
                RewardUIData friendPointData = RewardUIData.GetFriendPointData((int) data.get_friends_point());
                this._missionRewardList.Add(friendPointData);
            }
            foreach (RewardItemData data8 in data.get_item_list())
            {
                RewardUIData data9 = new RewardUIData(ResourceType.Item, (int) data8.get_num(), RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int) data8.get_id(), (int) data8.get_level());
                this._missionRewardList.Add(data9);
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) data8.get_id(), (int) data8.get_level());
                if (dummyStorageDataItem is AvatarCardDataItem)
                {
                    this._avatarData = dummyStorageDataItem as AvatarCardDataItem;
                }
            }
            if (this._dropItemList != null)
            {
                foreach (DropItem item in this._dropItemList)
                {
                    RewardUIData data10 = new RewardUIData(ResourceType.Item, (int) item.get_num(), RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int) item.get_item_id(), (int) item.get_level());
                    this._missionRewardList.Add(data10);
                }
            }
        }

        private void OnBGBtnClick()
        {
            this.Destroy();
            if (this._onDestroy != null)
            {
                this._onDestroy(this._avatarData);
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.UnlockAvatar)
            {
                int body = (int) ntf.body;
                Singleton<MainUIManager>.Instance.ShowDialog(new AvatarUnlockDialogContext(body, false), UIType.Any);
            }
            return false;
        }

        public void RegisterCallBack(OnDialogDestroy callback)
        {
            this._onDestroy = callback;
        }

        private void SetupContents()
        {
            int count = this._missionRewardList.Count;
            switch (count)
            {
                case 0:
                    break;

                case 1:
                {
                    RewardUIData data = this._missionRewardList[0];
                    Transform icon = base.view.transform.Find("Dialog/Content/RewardList/center");
                    this.SetupIcon(icon, data);
                    Transform line = base.view.transform.Find("Dialog/Content/TextList/line1");
                    this.SetupLine(line, data);
                    break;
                }
                default:
                    if (count >= 2)
                    {
                        RewardUIData data2 = this._missionRewardList[0];
                        Transform transform3 = base.view.transform.Find("Dialog/Content/RewardList/left");
                        this.SetupIcon(transform3, data2);
                        data2 = this._missionRewardList[1];
                        Transform transform4 = base.view.transform.Find("Dialog/Content/RewardList/right");
                        this.SetupIcon(transform4, data2);
                        data2 = this._missionRewardList[0];
                        Transform transform5 = base.view.transform.Find("Dialog/Content/TextList/line1");
                        this.SetupLine(transform5, data2);
                        data2 = this._missionRewardList[1];
                        Transform transform6 = base.view.transform.Find("Dialog/Content/TextList/line2");
                        this.SetupLine(transform6, data2);
                        if (count <= 2)
                        {
                        }
                    }
                    break;
            }
        }

        private void SetupIcon(Transform icon, RewardUIData data)
        {
            icon.gameObject.SetActive(true);
            icon.Find("ItemIcon/Icon").GetComponent<Image>().sprite = data.GetIconSprite();
            icon.Find("Text").GetComponent<Text>().text = string.Format("\x00d7{0}", data.value);
        }

        private void SetupLine(Transform line, RewardUIData data)
        {
            line.gameObject.SetActive(true);
            line.Find("Image").GetComponent<Image>().sprite = data.GetIconSprite();
            line.Find("Desc").GetComponent<Text>().text = this.GetDesc(data.valueLabelTextID, data.itemID);
            line.Find("Number").GetComponent<Text>().text = string.Format("\x00d7{0}", data.value);
        }

        protected override bool SetupView()
        {
            this._animationManager = new SequenceAnimationManager(null, null);
            this.ClearViews();
            this.InitRewardList();
            this.SetupContents();
            this._animationManager.AddAnimation(base.view.transform.Find("Dialog/Content/CompleteIcon").GetComponent<MonoAnimationinSequence>(), null);
            this._animationManager.StartPlay(0.5f, false);
            return false;
        }

        public delegate void OnDialogDestroy(AvatarCardDataItem data);
    }
}

