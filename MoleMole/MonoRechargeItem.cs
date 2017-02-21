namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoRechargeItem : MonoBehaviour
    {
        private string _iconPathPrex = "SpriteOutput/ShopIcons/";
        private RechargeDataItem _storeDataItem;

        public void OnClick()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectRechargeItem, this._storeDataItem.productID));
            base.transform.Find("InnerPanel/BG/Selected").gameObject.SetActive(true);
            base.transform.Find("InnerPanel/BG/Unselected").gameObject.SetActive(false);
        }

        public void SetupView(RechargeDataItem rechargeDataItem, bool isSelected)
        {
            this._storeDataItem = rechargeDataItem;
            base.transform.Find("InnerPanel/BG/Selected").gameObject.SetActive(isSelected);
            base.transform.Find("InnerPanel/BG/Unselected").gameObject.SetActive(!isSelected);
            base.transform.Find("InnerPanel/BG/Unselected/NowPrize/Num").GetComponent<Text>().text = rechargeDataItem.formattedPrice;
            base.transform.Find("InnerPanel/BG/Selected/NowPrize/Num").GetComponent<Text>().text = rechargeDataItem.formattedPrice;
            base.transform.Find("InnerPanel/NumPanel/Num/Num").GetComponent<Text>().text = rechargeDataItem.payHardCoin.ToString();
            if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview && (rechargeDataItem.productType == 3))
            {
                rechargeDataItem.productType = 1;
            }
            if (rechargeDataItem.productType == 3)
            {
                if (rechargeDataItem.cardLeftDays > 0)
                {
                    base.transform.Find("InnerPanel/MonthCardLeftDaysPanel").gameObject.SetActive(true);
                    base.transform.Find("InnerPanel/MonthCardDescPanel").gameObject.SetActive(false);
                    object[] replaceParams = new object[] { rechargeDataItem.cardLeftDays };
                    base.transform.Find("InnerPanel/MonthCardLeftDaysPanel/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_GiftHardCoinDaysLeft", replaceParams);
                }
                else
                {
                    base.transform.Find("InnerPanel/MonthCardLeftDaysPanel").gameObject.SetActive(false);
                    base.transform.Find("InnerPanel/MonthCardDescPanel").gameObject.SetActive(true);
                    object[] objArray2 = new object[] { rechargeDataItem.cardDailyHardCoin };
                    base.transform.Find("InnerPanel/MonthCardDescPanel/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_GifthardCoinDesc", objArray2);
                }
                base.transform.Find("InnerPanel/LimitBuyPanel").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/MonthCardLimitBuyPanel").gameObject.SetActive(rechargeDataItem.cardLeftDays < 180);
            }
            else
            {
                base.transform.Find("InnerPanel/MonthCardLeftDaysPanel").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/MonthCardDescPanel").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/MonthCardLimitBuyPanel").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/LimitBuyPanel").gameObject.SetActive(true);
                if (rechargeDataItem.productType == 2)
                {
                    base.transform.Find("InnerPanel/LimitBuyPanel/addition/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_FirstRechargeBonus", new object[0]);
                    object[] objArray3 = new object[] { rechargeDataItem.leftBuyTimes };
                    base.transform.Find("InnerPanel/LimitBuyPanel/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_FirstHardCoinDesc", objArray3);
                }
                else
                {
                    base.transform.Find("InnerPanel/LimitBuyPanel/addition/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("MenuRechargeBonus", new object[0]);
                    base.transform.Find("InnerPanel/LimitBuyPanel/Desc").gameObject.SetActive(false);
                }
            }
            if (rechargeDataItem.productType == 3)
            {
                base.transform.Find("InnerPanel/SaleLabel/SalePatten5").gameObject.SetActive(true);
                base.transform.Find("InnerPanel/SaleLabel/SalePatten4").gameObject.SetActive(false);
            }
            else if (rechargeDataItem.productType == 2)
            {
                base.transform.Find("InnerPanel/SaleLabel/SalePatten4").gameObject.SetActive(true);
                base.transform.Find("InnerPanel/SaleLabel/SalePatten5").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/LimitBuyPanel/addition/Num").GetComponent<Text>().text = rechargeDataItem.freeHardCoin.ToString();
            }
            else
            {
                if (rechargeDataItem.freeHardCoin > 0)
                {
                    base.transform.Find("InnerPanel/LimitBuyPanel/addition/Num").GetComponent<Text>().text = rechargeDataItem.freeHardCoin.ToString();
                }
                else
                {
                    base.transform.Find("InnerPanel/LimitBuyPanel/").gameObject.SetActive(false);
                }
                base.transform.Find("InnerPanel/SaleLabel/SalePatten4").gameObject.SetActive(false);
                base.transform.Find("InnerPanel/SaleLabel/SalePatten5").gameObject.SetActive(false);
            }
            string str = string.Empty;
            if (rechargeDataItem.productID.StartsWith("Bh3First"))
            {
                str = str + rechargeDataItem.productID.Substring(8);
            }
            else
            {
                str = str + rechargeDataItem.productID.Substring(3);
            }
            if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview && (str == "GiftHardCoinTier5"))
            {
                str = "HardCoinTier5";
            }
            base.transform.Find("InnerPanel/ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._iconPathPrex + str);
            if (rechargeDataItem.productType == 3)
            {
                RectTransform component = base.transform.Find("InnerPanel/ItemIcon/Icon").GetComponent<RectTransform>();
                component.anchoredPosition = new Vector2(component.anchoredPosition.x, 35f);
            }
        }
    }
}

