namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoSignInRewardItemIconButton : MonoBehaviour
    {
        private ClickCallBack _clickCallBack;
        private bool _isTodayCanGet;
        private bool _rewardAlreadyGot;
        private RewardData _rewardData;
        private StorageDataItemBase _rewardItemData;
        private int _rewardItemNum;

        public ResourceType GetRewardType()
        {
            if (this._rewardData.RewardExp > 0)
            {
                this._rewardItemNum = this._rewardData.RewardExp;
                return ResourceType.PlayerExp;
            }
            if (this._rewardData.RewardFriendPoint > 0)
            {
                this._rewardItemNum = this._rewardData.RewardFriendPoint;
                return ResourceType.FriendPoint;
            }
            if (this._rewardData.RewardHCoin > 0)
            {
                this._rewardItemNum = this._rewardData.RewardHCoin;
                return ResourceType.Hcoin;
            }
            if (this._rewardData.RewardSCoin > 0)
            {
                this._rewardItemNum = this._rewardData.RewardSCoin;
                return ResourceType.Scoin;
            }
            if (this._rewardData.RewardStamina > 0)
            {
                this._rewardItemNum = this._rewardData.RewardStamina;
                return ResourceType.Stamina;
            }
            if (this._rewardData.RewardSkillPoint > 0)
            {
                this._rewardItemNum = this._rewardData.RewardSkillPoint;
                return ResourceType.SkillPoint;
            }
            return ResourceType.Item;
        }

        public void OnClick()
        {
            if (this._clickCallBack != null)
            {
                this._clickCallBack(this._rewardData);
            }
        }

        public void SetClickCallback(ClickCallBack callback)
        {
            this._clickCallBack = callback;
        }

        private void SetupRewardItemData()
        {
            if (this.GetRewardType() == ResourceType.Item)
            {
                if (this._rewardData.RewardItem1ID > 0)
                {
                    this._rewardItemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(this._rewardData.RewardItem1ID, this._rewardData.RewardItem1Level);
                    this._rewardItemData.number = this._rewardData.RewardItem1Num;
                    this._rewardItemNum = this._rewardData.RewardItem1Num;
                    return;
                }
                if (this._rewardData.RewardItem2ID > 0)
                {
                    this._rewardItemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(this._rewardData.RewardItem2ID, this._rewardData.RewardItem2Level);
                    this._rewardItemData.number = this._rewardData.RewardItem2Num;
                    this._rewardItemNum = this._rewardData.RewardItem2Num;
                    return;
                }
                if (this._rewardData.RewardItem3ID > 0)
                {
                    this._rewardItemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(this._rewardData.RewardItem3ID, this._rewardData.RewardItem3Level);
                    this._rewardItemData.number = this._rewardData.RewardItem3Num;
                    this._rewardItemNum = this._rewardData.RewardItem3Num;
                    return;
                }
                if (this._rewardData.RewardItem4ID > 0)
                {
                    this._rewardItemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(this._rewardData.RewardItem4ID, this._rewardData.RewardItem4Level);
                    this._rewardItemData.number = this._rewardData.RewardItem4Num;
                    this._rewardItemNum = this._rewardData.RewardItem4Num;
                    return;
                }
                if (this._rewardData.RewardItem5ID > 0)
                {
                    this._rewardItemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(this._rewardData.RewardItem5ID, this._rewardData.RewardItem5Level);
                    this._rewardItemData.number = this._rewardData.RewardItem5Num;
                    this._rewardItemNum = this._rewardData.RewardItem5Num;
                    return;
                }
            }
            this._rewardItemData = null;
        }

        private void SetupRewardItemIcon()
        {
            ResourceType rewardType = this.GetRewardType();
            this.SetupRewardItemData();
            Sprite resourceSprite = UIUtil.GetResourceSprite(rewardType, this._rewardItemData);
            base.transform.Find("ItemIcon/Icon").GetComponent<Image>().sprite = resourceSprite;
            base.transform.Find("ItemIcon").GetComponent<Image>().color = MiscData.GetColor("TotalWhite");
            Text component = base.transform.Find("Text").GetComponent<Text>();
            if (rewardType == ResourceType.Item)
            {
                if ((this._rewardItemData is WeaponDataItem) || (this._rewardItemData is StigmataDataItem))
                {
                    component.text = "Lv." + this._rewardItemData.level;
                }
                else
                {
                    component.text = "x" + this._rewardItemData.number;
                }
                base.transform.Find("FragmentIcon").gameObject.SetActive(this._rewardItemData is AvatarFragmentDataItem);
                base.transform.Find("StigmataType").gameObject.SetActive(this._rewardItemData is StigmataDataItem);
                if (this._rewardItemData is StigmataDataItem)
                {
                    base.transform.Find("StigmataType/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.StigmataTypeIconPath[this._rewardItemData.GetBaseType()]);
                    if (this._rewardAlreadyGot)
                    {
                        base.transform.Find("StigmataType/Image").GetComponent<Image>().color = MiscData.GetColor("SignInGotGrey");
                    }
                }
                base.transform.Find("ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[this._rewardItemData.rarity]);
            }
            else
            {
                component.text = "x" + this._rewardItemNum;
                base.transform.Find("ItemIcon").GetComponent<Image>().sprite = (rewardType != ResourceType.Hcoin) ? Miscs.GetSpriteByPrefab("SpriteOutput/ItemFrame/FrameComBlue") : Miscs.GetSpriteByPrefab("SpriteOutput/ItemFrame/FrameComPurple");
            }
            base.transform.Find("Received").gameObject.SetActive(this._rewardAlreadyGot);
            base.transform.GetComponent<Button>().interactable = !this._rewardAlreadyGot;
        }

        public void SetupView(RewardData rewardData, bool rewardAlreadyGot, bool todayCanGet)
        {
            this._rewardData = rewardData;
            this._rewardAlreadyGot = rewardAlreadyGot;
            this._isTodayCanGet = todayCanGet;
            base.transform.Find("Star").gameObject.SetActive(false);
            base.transform.Find("StigmataType").gameObject.SetActive(false);
            base.transform.Find("FragmentIcon").gameObject.SetActive(false);
            base.transform.Find("BG/Unselected").gameObject.SetActive(true);
            base.transform.Find("BG/Selected").gameObject.SetActive(false);
            this.SetupRewardItemIcon();
            if (this._isTodayCanGet)
            {
                base.transform.GetComponent<Animator>().SetTrigger("Play");
            }
        }

        public delegate void ClickCallBack(RewardData item);
    }
}

