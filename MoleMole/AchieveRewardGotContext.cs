namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class AchieveRewardGotContext : BaseDialogContext
    {
        private List<RewardUIData> _achieveRewardList = new List<RewardUIData>();
        private SequenceAnimationManager _animationManager;
        private OnDialogDestroy _onDestroy;
        private RewardData _rewardData;

        public AchieveRewardGotContext(List<RewardData> dataList)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AchieveRewardGotDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AchieveRewardGotDialog"
            };
            base.config = pattern;
            this._rewardData = dataList[0];
            this.InitRewardList();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
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
            this._achieveRewardList.Clear();
            if (this._rewardData.get_exp() > 0)
            {
                RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int) this._rewardData.get_exp());
                this._achieveRewardList.Add(playerExpData);
            }
            if (this._rewardData.get_scoin() > 0)
            {
                RewardUIData scoinData = RewardUIData.GetScoinData((int) this._rewardData.get_scoin());
                this._achieveRewardList.Add(scoinData);
            }
            if (this._rewardData.get_hcoin() > 0)
            {
                RewardUIData hcoinData = RewardUIData.GetHcoinData((int) this._rewardData.get_hcoin());
                this._achieveRewardList.Add(hcoinData);
            }
            if (this._rewardData.get_stamina() > 0)
            {
                RewardUIData staminaData = RewardUIData.GetStaminaData((int) this._rewardData.get_stamina());
                this._achieveRewardList.Add(staminaData);
            }
            if (this._rewardData.get_skill_point() > 0)
            {
                RewardUIData skillPointData = RewardUIData.GetSkillPointData((int) this._rewardData.get_skill_point());
                this._achieveRewardList.Add(skillPointData);
            }
            if (this._rewardData.get_friends_point() > 0)
            {
                RewardUIData friendPointData = RewardUIData.GetFriendPointData((int) this._rewardData.get_friends_point());
                this._achieveRewardList.Add(friendPointData);
            }
            foreach (RewardItemData data7 in this._rewardData.get_item_list())
            {
                RewardUIData item = new RewardUIData(ResourceType.Item, (int) data7.get_num(), RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int) data7.get_id(), (int) data7.get_level());
                this._achieveRewardList.Add(item);
            }
        }

        private void OnBGBtnClick()
        {
            this.Destroy();
            if (this._onDestroy != null)
            {
                this._onDestroy();
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

        private void SetupDetail()
        {
            for (int i = 0; i < 3; i++)
            {
                int num2 = i + 1;
                this.SetupDetailItem(base.view.transform.Find("Dialog/Content/RewardDetail/Lines/" + num2.ToString()).gameObject, (i >= this._achieveRewardList.Count) ? null : this._achieveRewardList[i]);
            }
        }

        private void SetupDetailItem(GameObject item, RewardUIData data)
        {
            if (data == null)
            {
                item.SetActive(false);
            }
            else
            {
                item.SetActive(true);
                Text component = item.transform.Find("Item").GetComponent<Text>();
                Text text2 = item.transform.Find("Number").GetComponent<Text>();
                if (data.rewardType == ResourceType.PlayerExp)
                {
                    component.text = LocalizationGeneralLogic.GetText("Menu_Level", new object[0]);
                }
                else if (data.rewardType == ResourceType.Scoin)
                {
                    component.text = LocalizationGeneralLogic.GetText("Menu_Scoin", new object[0]);
                }
                else if (data.rewardType == ResourceType.Hcoin)
                {
                    component.text = LocalizationGeneralLogic.GetText("Menu_Hcoin", new object[0]);
                }
                else if (data.rewardType == ResourceType.FriendPoint)
                {
                    component.text = LocalizationGeneralLogic.GetText("10119", new object[0]);
                }
                else if (data.rewardType == ResourceType.Item)
                {
                    component.text = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID, 1).GetDisplayTitle();
                }
                text2.text = data.value.ToString();
            }
        }

        private void SetupIconView(Transform trans, RewardUIData data)
        {
            if ((((data.rewardType == ResourceType.PlayerExp) || (data.rewardType == ResourceType.FriendPoint)) || ((data.rewardType == ResourceType.Hcoin) || (data.rewardType == ResourceType.Scoin))) || ((data.rewardType == ResourceType.SkillPoint) || (data.rewardType == ResourceType.Stamina)))
            {
                trans.Find("ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(data.iconPath);
                trans.Find("ItemIcon/Icon").GetComponent<Image>().SetNativeSize();
                trans.Find("QuestionMark").gameObject.SetActive(false);
                trans.Find("Star").gameObject.SetActive(false);
                trans.Find("Text").GetComponent<Text>().text = string.Format("\x00d7{0}", data.value.ToString());
                trans.Find("ItemIcon").GetComponent<Image>().color = Color.white;
            }
            else
            {
                MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
                if (component != null)
                {
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID, 1);
                    dummyStorageDataItem.number = data.value;
                    component.SetupView(dummyStorageDataItem, MonoItemIconButton.SelectMode.None, false, false, false);
                }
            }
        }

        private void SetupScrollView()
        {
            base.view.transform.Find("Dialog/Content/Rewards/ScrollView").GetComponent<MonoGridScroller>().Init(delegate (Transform trans, int index) {
                RewardUIData data = this._achieveRewardList[index];
                this.SetupIconView(trans, data);
            }, this._achieveRewardList.Count, null);
        }

        protected override bool SetupView()
        {
            this._animationManager = new SequenceAnimationManager(null, null);
            this.SetupScrollView();
            this.SetupDetail();
            this._animationManager.AddAnimation(base.view.transform.Find("Dialog/Content/CompleteIcon").GetComponent<MonoAnimationinSequence>(), null);
            this._animationManager.StartPlay(0.5f, false);
            return false;
        }

        public delegate void OnDialogDestroy();
    }
}

