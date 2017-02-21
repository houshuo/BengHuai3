namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoRewardIconButton : MonoBehaviour
    {
        private StorageDataItemBase _itemData;
        protected int _num;
        private RewardType _rewardType;

        private string GetIconPath()
        {
            string str = "SpriteOutput/SpecialIcons/";
            switch (this._rewardType)
            {
                case RewardType.HCoin:
                    return (str + "HCoinIcon");

                case RewardType.SCoin:
                    return (str + "SCoinIcon");

                case RewardType.Stamina:
                    return (str + "StaminaIcon");

                case RewardType.SkillPoint:
                    return (str + "SkillPointIcon");

                case RewardType.Item:
                    return this._itemData.GetIconPath();
            }
            throw new Exception("Invalid Type or State!");
        }

        private string GetName()
        {
            switch (this._rewardType)
            {
                case RewardType.HCoin:
                    return LocalizationGeneralLogic.GetText("Menu_Hcoin", new object[0]);

                case RewardType.SCoin:
                    return LocalizationGeneralLogic.GetText("Menu_Scoin", new object[0]);

                case RewardType.Stamina:
                    return LocalizationGeneralLogic.GetText("Menu_Stamina", new object[0]);

                case RewardType.SkillPoint:
                    return LocalizationGeneralLogic.GetText("Menu_SkillPtNum", new object[0]);

                case RewardType.Item:
                    return this._itemData.GetDisplayTitle();
            }
            throw new Exception("Invalid Type or State!");
        }

        public void OnButtonClick()
        {
            if (this._itemData != null)
            {
                UIUtil.ShowItemDetail(this._itemData, false, true);
            }
        }

        public void SetupView(RewardType rewardType, int num, StorageDataItemBase itemData)
        {
            this._rewardType = rewardType;
            this._num = num;
            this._itemData = itemData;
            GameObject obj2 = Miscs.LoadResource<GameObject>(this.GetIconPath(), BundleType.RESOURCE_FILE);
            base.transform.Find("Icon").GetComponent<Image>().sprite = obj2.GetComponent<SpriteRenderer>().sprite;
            string name = this.GetName();
            if (num > 1)
            {
                name = name + " x" + num;
            }
            base.transform.Find("Desc").GetComponent<Text>().text = name;
        }

        public enum RewardType
        {
            HCoin,
            SCoin,
            Stamina,
            SkillPoint,
            Item
        }
    }
}

