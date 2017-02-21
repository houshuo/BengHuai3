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

    public class RewardGotDialogContext : BaseDialogContext
    {
        private SequenceAnimationManager _animationManager;
        private string _completeIconPrefabPath;
        private List<DropItem> _dropItemList;
        private List<RewardUIData> _nonItemRewardList;
        private OnDialogDestroy _onDestroy;
        private int _playerLevelBefore;
        private RewardData _rewardData;
        private Dictionary<int, StorageDataItemBase> _rewardItemDict;
        private string _titleTextID;
        private const string DROP_ITEM_PREFAB_PATH = "UI/Menus/Widget/Map/DropItemButton";

        public RewardGotDialogContext(RewardData rewardData, int playerLevelBefore, List<DropItem> dropList = null, string titleTextID = "", string completeIconPrefabPath = "")
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "RewardGotDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/RewardGotDialog"
            };
            base.config = pattern;
            this._rewardData = rewardData;
            this._dropItemList = dropList;
            this._titleTextID = titleTextID;
            this._completeIconPrefabPath = completeIconPrefabPath;
            this._playerLevelBefore = playerLevelBefore;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Button").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
        }

        private void ClearViews()
        {
            base.view.transform.Find("Dialog/Content/TextList/line1").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/TextList/line2").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/TextList/line3").gameObject.SetActive(false);
        }

        private string GetDesc(string textID, int id)
        {
            if (textID == RewardUIData.ITEM_ICON_TEXT_ID)
            {
                return Singleton<StorageModule>.Instance.GetDummyStorageDataItem(id, 1).GetDisplayTitle();
            }
            return LocalizationGeneralLogic.GetText(textID, new object[0]);
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

        private Transform GetTextLine(int typeCount, params Transform[] lineTrans)
        {
            if (typeCount == 1)
            {
                return lineTrans[0];
            }
            if (typeCount == 2)
            {
                return lineTrans[1];
            }
            if (typeCount == 3)
            {
                return lineTrans[2];
            }
            return null;
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

        private void OnBGBtnClick()
        {
            this.Destroy();
            if (this._onDestroy != null)
            {
                this._onDestroy();
            }
            if (this._playerLevelBefore < Singleton<PlayerModule>.Instance.playerData.teamLevel)
            {
                PlayerLevelUpDialogContext dialogContext = new PlayerLevelUpDialogContext();
                dialogContext.SetLevelBeforeNoScoreManager(this._playerLevelBefore);
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
        }

        private void OnDropItemButtonClick(StorageDataItemBase itemData)
        {
            UIUtil.ShowItemDetail(itemData, true, true);
        }

        public void RegisterCallBack(OnDialogDestroy callback)
        {
            this._onDestroy = callback;
        }

        private void SetupCompleteIcon()
        {
            if (!string.IsNullOrEmpty(this._completeIconPrefabPath))
            {
                base.view.transform.Find("Dialog/Content/CompleteIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._completeIconPrefabPath);
            }
        }

        private void SetupIcon(Transform icon, RewardUIData data)
        {
            icon.gameObject.SetActive(true);
            icon.Find("ItemIcon/Icon").GetComponent<Image>().sprite = data.GetIconSprite();
            icon.Find("Text").GetComponent<Text>().text = string.Format("\x00d7{0}", data.value);
        }

        private void SetupLine(Transform line, RewardUIData data, StorageDataItemBase item = null)
        {
            line.gameObject.SetActive(true);
            line.Find("Image").GetComponent<Image>().sprite = data.GetIconSprite();
            line.Find("Desc").GetComponent<Text>().text = this.GetDesc(data.valueLabelTextID, data.itemID);
            line.Find("Number").GetComponent<Text>().text = string.Format("\x00d7{0}", data.value);
        }

        private void SetupRewardList()
        {
            Transform transform = base.view.transform.Find("Dialog/Content/TextList/line1");
            Transform transform2 = base.view.transform.Find("Dialog/Content/TextList/line2");
            Transform transform3 = base.view.transform.Find("Dialog/Content/TextList/line3");
            transform.gameObject.SetActive(false);
            transform2.gameObject.SetActive(false);
            transform3.gameObject.SetActive(false);
            int typeCount = 0;
            this._nonItemRewardList = new List<RewardUIData>();
            if (this._rewardData.get_exp() > 0)
            {
                RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int) this._rewardData.get_exp());
                this._nonItemRewardList.Add(playerExpData);
            }
            if (this._rewardData.get_scoin() > 0)
            {
                RewardUIData scoinData = RewardUIData.GetScoinData((int) this._rewardData.get_scoin());
                this._nonItemRewardList.Add(scoinData);
            }
            if (this._rewardData.get_hcoin() > 0)
            {
                RewardUIData hcoinData = RewardUIData.GetHcoinData((int) this._rewardData.get_hcoin());
                this._nonItemRewardList.Add(hcoinData);
            }
            if (this._rewardData.get_stamina() > 0)
            {
                RewardUIData staminaData = RewardUIData.GetStaminaData((int) this._rewardData.get_stamina());
                this._nonItemRewardList.Add(staminaData);
            }
            if (this._rewardData.get_skill_point() > 0)
            {
                RewardUIData skillPointData = RewardUIData.GetSkillPointData((int) this._rewardData.get_skill_point());
                this._nonItemRewardList.Add(skillPointData);
            }
            if (this._rewardData.get_friends_point() > 0)
            {
                RewardUIData friendPointData = RewardUIData.GetFriendPointData((int) this._rewardData.get_friends_point());
                this._nonItemRewardList.Add(friendPointData);
            }
            foreach (RewardUIData data7 in this._nonItemRewardList)
            {
                typeCount++;
                Transform[] lineTrans = new Transform[] { transform, transform2, transform3 };
                Transform textLine = this.GetTextLine(typeCount, lineTrans);
                if (textLine != null)
                {
                    this.SetupLine(textLine, data7, null);
                }
            }
            this._rewardItemDict = new Dictionary<int, StorageDataItemBase>();
            foreach (RewardItemData data8 in this._rewardData.get_item_list())
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) data8.get_id(), (int) data8.get_level());
                dummyStorageDataItem.number = (int) data8.get_num();
                if (this._rewardItemDict.ContainsKey(dummyStorageDataItem.ID))
                {
                    StorageDataItemBase local1 = this._rewardItemDict[dummyStorageDataItem.ID];
                    local1.number += dummyStorageDataItem.number;
                }
                else
                {
                    this._rewardItemDict[dummyStorageDataItem.ID] = dummyStorageDataItem;
                }
            }
            if (this._dropItemList != null)
            {
                foreach (DropItem item in this._dropItemList)
                {
                    StorageDataItemBase base3 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) item.get_item_id(), (int) item.get_level());
                    base3.number = (int) item.get_num();
                    if (this._rewardItemDict.ContainsKey(base3.ID))
                    {
                        StorageDataItemBase local2 = this._rewardItemDict[base3.ID];
                        local2.number += base3.number;
                    }
                    else
                    {
                        this._rewardItemDict[base3.ID] = base3;
                    }
                }
            }
            Transform trans = base.view.transform.Find("Dialog/Content/RewardList/Content");
            trans.DestroyChildren();
            foreach (RewardUIData data9 in this._nonItemRewardList)
            {
                Transform rewardTrans = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI/Menus/Widget/Map/DropItemButton")).transform;
                rewardTrans.SetParent(trans, false);
                this.HideRewardTransSomePart(rewardTrans);
                rewardTrans.GetComponent<MonoLevelDropIconButton>().Clear();
                rewardTrans.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().sprite = data9.GetIconSprite();
                rewardTrans.Find("BG/Desc").GetComponent<Text>().text = "\x00d7" + data9.value.ToString();
                rewardTrans.GetComponent<CanvasGroup>().alpha = 1f;
            }
            foreach (StorageDataItemBase base4 in this._rewardItemDict.Values)
            {
                Transform transform7 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI/Menus/Widget/Map/DropItemButton")).transform;
                transform7.SetParent(trans, false);
                transform7.GetComponent<MonoLevelDropIconButton>().SetupView(base4, new DropItemButtonClickCallBack(this.OnDropItemButtonClick), true, false, false, false);
                transform7.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }

        private void SetupTitle()
        {
            if (!string.IsNullOrEmpty(this._titleTextID))
            {
                base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(this._titleTextID, new object[0]);
            }
        }

        protected override bool SetupView()
        {
            this._animationManager = new SequenceAnimationManager(null, null);
            this.ClearViews();
            this.SetupTitle();
            this.SetupRewardList();
            this.SetupCompleteIcon();
            this._animationManager.AddAnimation(base.view.transform.Find("Dialog/Content/CompleteIcon").GetComponent<MonoAnimationinSequence>(), null);
            this._animationManager.StartPlay(0.5f, false);
            Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
            return false;
        }

        public delegate void OnDialogDestroy();
    }
}

