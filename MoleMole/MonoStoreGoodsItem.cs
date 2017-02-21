namespace MoleMole
{
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoStoreGoodsItem : MonoBehaviour
    {
        private Goods _goods;
        private bool _isMultiCurrency;
        private const string _salePanelDiscountSpritePath = "SpriteOutput/ShopIcons/SalePatternDiscount";
        private const string _salePanelGreySpritePath = "SpriteOutput/ShopIcons/SalePatternGrey";
        private const string _salePanelNewSpritePath = "SpriteOutput/ShopIcons/SalePatternNew";
        private const string _salePanelSuperWorthSpritePath = "SpriteOutput/ShopIcons/SalePatternSuperWorth";
        private int _ticketID;

        public void OnClick()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectStoreGoodsItem, this._goods));
            base.transform.Find("BG/Selected").gameObject.SetActive(true);
            base.transform.Find("BG/Unselected").gameObject.SetActive(false);
        }

        public void OnIconClick()
        {
            ShopGoodsMetaData shopGoodsMetaDataByKey;
            if (this._ticketID > 0)
            {
                int hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[this._ticketID];
                shopGoodsMetaDataByKey = new ShopGoodsMetaData(this._ticketID, this._ticketID, 1, 1, hCoinCost, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x7fffffff, 1, 0x2710, false);
                if (Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(this._ticketID / 10))
                {
                    hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[this._ticketID / 10];
                    shopGoodsMetaDataByKey = new ShopGoodsMetaData(this._ticketID, this._ticketID / 10, 1, 10, hCoinCost * 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x7fffffff, 1, 0x2710, false);
                }
            }
            else
            {
                shopGoodsMetaDataByKey = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int) this._goods.get_goods_id());
            }
            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(shopGoodsMetaDataByKey.ItemID, shopGoodsMetaDataByKey.ItemLevel);
            dummyStorageDataItem.number = shopGoodsMetaDataByKey.ItemNum;
            UIUtil.ShowItemDetail(dummyStorageDataItem, true, true);
            this.OnClick();
        }

        private void SetItemDefaultColor()
        {
            base.transform.Find("BG/Unselected/FrameTop").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedTop");
            base.transform.Find("BG/Unselected/FrameBottom").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedBottom");
            Transform transform = base.transform.Find("BG/Unselected/NowPrize");
            if (this._isMultiCurrency)
            {
                IEnumerator enumerator = transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        current.Find("Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
                        current.Find("Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedPrice");
                        current.Find("x").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedPriceX");
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
            else
            {
                transform.Find("Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
                transform.Find("Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedPrice");
                transform.Find("x").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultUnselectedPriceX");
            }
            base.transform.Find("BG/Unselected/FakePrize/Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultDiscountNum");
            base.transform.Find("BG/Unselected/FakePrize/Num/Line").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultDiscountLine");
            base.transform.Find("BG/Selected/FrameTop").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultSelectedTop");
            Transform transform3 = base.transform.Find("BG/Selected/NowPrize");
            if (this._isMultiCurrency)
            {
                IEnumerator enumerator2 = transform3.GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext())
                    {
                        Transform transform4 = (Transform) enumerator2.Current;
                        transform4.Find("Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
                        transform4.Find("Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultSelectedPrice");
                        transform4.Find("x").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultSelectedPriceX");
                    }
                }
                finally
                {
                    IDisposable disposable2 = enumerator2 as IDisposable;
                    if (disposable2 == null)
                    {
                    }
                    disposable2.Dispose();
                }
            }
            else
            {
                transform3.Find("Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
                transform3.Find("Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultSelectedPrice");
                transform3.Find("x").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultSelectedPriceX");
            }
            base.transform.Find("BG/Selected/FakePrize/Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultDiscountNum");
            base.transform.Find("BG/Selected/FakePrize/Num/Line").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultDiscountLine");
            base.transform.Find("ItemIcon").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
            for (int i = 1; i < 6; i++)
            {
                Transform transform5 = base.transform.Find("Star/" + i.ToString());
                if (transform5 != null)
                {
                    transform5.GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
                }
            }
            for (int j = 1; j < 6; j++)
            {
                Transform transform6 = base.transform.Find("AvatarStar/" + j.ToString());
                if (transform6 != null)
                {
                    transform6.GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
                }
            }
            base.transform.Find("StigmataType/Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
            base.transform.Find("FragmentIcon").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsDefaultIcon");
            base.transform.Find("NumPanel/Num/Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultNum");
            base.transform.Find("NumPanel/Num/x").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultNumX");
            base.transform.Find("LevelPanel/Num/Num").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultNum");
            base.transform.Find("LevelPanel/Num/Lv").GetComponent<Text>().color = MiscData.GetColor("ShopGoodsDefaultLevel");
        }

        private void SetItemGrey()
        {
            base.transform.Find("BG/Unselected/FrameTop").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyTop");
            base.transform.Find("BG/Unselected/FrameBottom").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyBG");
            base.transform.Find("BG/Unselected/NowPrize").gameObject.SetActive(false);
            base.transform.Find("BG/Unselected/FakePrize").gameObject.SetActive(false);
            base.transform.Find("BG/Unselected/Empty").gameObject.SetActive(true);
            Color color = base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color;
            color.a = 0.5f;
            base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color = color;
            base.transform.Find("BG/Selected/FrameTop").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyTop");
            base.transform.Find("BG/Selected/NowPrize").gameObject.SetActive(false);
            base.transform.Find("BG/Selected/FakePrize").gameObject.SetActive(false);
            base.transform.Find("BG/Selected/Empty").gameObject.SetActive(true);
            color = base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color;
            color.a = 0.5f;
            base.transform.Find("BG/Selected/Image").GetComponent<Image>().color = color;
            base.transform.Find("ItemIcon").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyIcon");
            base.transform.Find("ItemIcon/SellOut").gameObject.SetActive(true);
            base.transform.Find("StigmataType/Image").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyIcon");
            base.transform.Find("FragmentIcon").GetComponent<Image>().color = MiscData.GetColor("ShopGoodsGreyIcon");
            base.transform.Find("SaleLabel/Bg").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/ShopIcons/SalePatternGrey");
        }

        private void SetupAvatarStar(int starNum)
        {
            for (int i = 1; i < 7; i++)
            {
                string name = string.Format("AvatarStar/{0}", i);
                base.transform.Find(name).gameObject.SetActive(i == starNum);
            }
        }

        private void SetupCurrencyColor(bool isEnough, int index)
        {
            if (!isEnough)
            {
                if (this._isMultiCurrency)
                {
                    base.transform.Find(string.Format("BG/Unselected/NowPrize/{0}/Num", index)).GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                    base.transform.Find(string.Format("BG/Unselected/NowPrize/{0}/x", index)).GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                    base.transform.Find(string.Format("BG/Selected/NowPrize/{0}/Num", index)).GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                    base.transform.Find(string.Format("BG/Selected/NowPrize/{0}/x", index)).GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                }
                else
                {
                    base.transform.Find("BG/Unselected/NowPrize/Num").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                    base.transform.Find("BG/Unselected/NowPrize/x").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                    base.transform.Find("BG/Selected/NowPrize/Num").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                    base.transform.Find("BG/Selected/NowPrize/x").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                }
            }
            else if (this._isMultiCurrency)
            {
                base.transform.Find(string.Format("BG/Unselected/NowPrize/{0}/Num", index)).GetComponent<Text>().color = MiscData.GetColor("Black");
                base.transform.Find(string.Format("BG/Unselected/NowPrize/{0}/x", index)).GetComponent<Text>().color = MiscData.GetColor("Black");
                base.transform.Find(string.Format("BG/Selected/NowPrize/{0}/Num", index)).GetComponent<Text>().color = MiscData.GetColor("Black");
                base.transform.Find(string.Format("BG/Selected/NowPrize/{0}/x", index)).GetComponent<Text>().color = MiscData.GetColor("Black");
            }
            else
            {
                base.transform.Find("BG/Unselected/NowPrize/Num").GetComponent<Text>().color = MiscData.GetColor("Black");
                base.transform.Find("BG/Unselected/NowPrize/x").GetComponent<Text>().color = MiscData.GetColor("Black");
                base.transform.Find("BG/Selected/NowPrize/Num").GetComponent<Text>().color = MiscData.GetColor("Black");
                base.transform.Find("BG/Selected/NowPrize/x").GetComponent<Text>().color = MiscData.GetColor("Black");
            }
        }

        private void SetupDesc(StorageDataItemBase item)
        {
            if ((item is WeaponDataItem) || (item is StigmataDataItem))
            {
                base.transform.Find("NumPanel").gameObject.SetActive(false);
                base.transform.Find("LevelPanel").gameObject.SetActive(true);
                base.transform.Find("LevelPanel/Num/Num").GetComponent<Text>().text = item.level.ToString();
            }
            else if (item is AvatarCardDataItem)
            {
                base.transform.Find("NumPanel").gameObject.SetActive(false);
                base.transform.Find("LevelPanel").gameObject.SetActive(true);
                AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.ID).avatarID);
                base.transform.Find("LevelPanel/Num/Num").GetComponent<Text>().text = dummyAvatarDataItem.level.ToString();
            }
            else
            {
                base.transform.Find("NumPanel").gameObject.SetActive(true);
                base.transform.Find("LevelPanel").gameObject.SetActive(false);
                base.transform.Find("NumPanel/Num/Num").GetComponent<Text>().text = item.number.ToString();
            }
        }

        private void SetupPrice(ShopGoodsMetaData goodsItem)
        {
            List<int> list = (this._ticketID <= 0) ? UIUtil.GetGoodsRealPrice(this._goods) : new List<int> { goodsItem.HCoinCost };
            if (this._isMultiCurrency)
            {
                if (list.Count > 3)
                {
                    base.transform.GetComponent<LayoutElement>().preferredWidth = 320f;
                }
                else
                {
                    base.transform.GetComponent<LayoutElement>().preferredWidth = 220f;
                }
            }
            Transform transform = base.transform.Find("BG/Unselected/NowPrize");
            Transform transform2 = base.transform.Find("BG/Selected/NowPrize");
            int num = 1;
            if (goodsItem.HCoinCost > 0)
            {
                if (this._isMultiCurrency)
                {
                    transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Hcoin");
                    transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Hcoin");
                    transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                    transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                }
                else
                {
                    transform.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Hcoin");
                    transform2.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Hcoin");
                    transform.Find("Num").GetComponent<Text>().text = list[num - 1].ToString();
                    transform2.Find("Num").GetComponent<Text>().text = list[num - 1].ToString();
                }
                this.SetupCurrencyColor(list[num - 1] <= Singleton<PlayerModule>.Instance.playerData.hcoin, num);
                num++;
            }
            if (goodsItem.SCoinCost > 0)
            {
                if (this._isMultiCurrency)
                {
                    transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Scoin");
                    transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Scoin");
                    transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                    transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                }
                else
                {
                    transform.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Scoin");
                    transform2.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/RewardIcons/Scoin");
                    transform.Find("Num").GetComponent<Text>().text = list[num - 1].ToString();
                    transform2.Find("Num").GetComponent<Text>().text = list[num - 1].ToString();
                }
                this.SetupCurrencyColor(list[num - 1] <= Singleton<PlayerModule>.Instance.playerData.scoin, num);
                num++;
            }
            if (goodsItem.CostItemId > 0)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(goodsItem.CostItemId, 1);
                string currencyIconPath = MiscData.GetCurrencyIconPath(goodsItem.CostItemId);
                if (string.IsNullOrEmpty(currencyIconPath))
                {
                    currencyIconPath = dummyStorageDataItem.GetIconPath();
                }
                if (this._isMultiCurrency)
                {
                    transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(currencyIconPath);
                    transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(currencyIconPath);
                    transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                    transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                }
                else
                {
                    transform.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(currencyIconPath);
                    transform2.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(currencyIconPath);
                    transform.Find("Num").GetComponent<Text>().text = list[num - 1].ToString();
                    transform2.Find("Num").GetComponent<Text>().text = list[num - 1].ToString();
                }
                int number = 0;
                StorageDataItemBase base3 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(goodsItem.CostItemId);
                if (base3 != null)
                {
                    number = base3.number;
                }
                this.SetupCurrencyColor(list[num - 1] <= number, num);
                num++;
            }
            if (goodsItem.CostItemId2 > 0)
            {
                StorageDataItemBase base4 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(goodsItem.CostItemId2, 1);
                string iconPath = MiscData.GetCurrencyIconPath(goodsItem.CostItemId2);
                if (string.IsNullOrEmpty(iconPath))
                {
                    iconPath = base4.GetIconPath();
                }
                if (this._isMultiCurrency)
                {
                    transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(iconPath);
                    transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(iconPath);
                    transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                    transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                }
                int num3 = 0;
                StorageDataItemBase base5 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(goodsItem.CostItemId2);
                if (base5 != null)
                {
                    num3 = base5.number;
                }
                this.SetupCurrencyColor(list[num - 1] <= num3, num);
                num++;
            }
            if (goodsItem.CostItemId3 > 0)
            {
                StorageDataItemBase base6 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(goodsItem.CostItemId3, 1);
                string str3 = MiscData.GetCurrencyIconPath(goodsItem.CostItemId3);
                if (string.IsNullOrEmpty(str3))
                {
                    str3 = base6.GetIconPath();
                }
                if (this._isMultiCurrency)
                {
                    transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(str3);
                    transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(str3);
                    transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                    transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                }
                int num4 = 0;
                StorageDataItemBase base7 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(goodsItem.CostItemId3);
                if (base7 != null)
                {
                    num4 = base7.number;
                }
                this.SetupCurrencyColor(list[num - 1] <= num4, num);
                num++;
            }
            if (goodsItem.CostItemId4 > 0)
            {
                StorageDataItemBase base8 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(goodsItem.CostItemId4, 1);
                string str4 = MiscData.GetCurrencyIconPath(goodsItem.CostItemId4);
                if (string.IsNullOrEmpty(str4))
                {
                    str4 = base8.GetIconPath();
                }
                if (this._isMultiCurrency)
                {
                    transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(str4);
                    transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(str4);
                    transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                    transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                }
                int num5 = 0;
                StorageDataItemBase base9 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(goodsItem.CostItemId4);
                if (base9 != null)
                {
                    num5 = base9.number;
                }
                this.SetupCurrencyColor(list[num - 1] <= num5, num);
                num++;
            }
            if (goodsItem.CostItemId5 > 0)
            {
                StorageDataItemBase base10 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(goodsItem.CostItemId5, 1);
                string str5 = MiscData.GetCurrencyIconPath(goodsItem.CostItemId5);
                if (string.IsNullOrEmpty(str5))
                {
                    str5 = base10.GetIconPath();
                }
                if (this._isMultiCurrency)
                {
                    transform.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(str5);
                    transform2.Find(string.Format("{0}/Image", num)).GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(str5);
                    transform.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                    transform2.Find(string.Format("{0}/Num", num)).GetComponent<Text>().text = list[num - 1].ToString();
                }
                int num6 = 0;
                StorageDataItemBase base11 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(goodsItem.CostItemId5);
                if (base11 != null)
                {
                    num6 = base11.number;
                }
                this.SetupCurrencyColor(list[num - 1] <= num6, num);
                num++;
            }
            if (list.Count < transform.childCount)
            {
                for (int i = list.Count; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                    transform2.GetChild(i).gameObject.SetActive(false);
                }
            }
            if (goodsItem.Discount < 0x2710)
            {
                int num8 = UIUtil.GetGoodsOriginPrice(this._goods)[0];
                base.transform.Find("BG/Unselected/FakePrize").gameObject.SetActive(true);
                base.transform.Find("BG/Selected/FakePrize").gameObject.SetActive(true);
                base.transform.Find("BG/Unselected/FakePrize/Num").GetComponent<Text>().text = num8.ToString();
                base.transform.Find("BG/Selected/FakePrize/Num").GetComponent<Text>().text = num8.ToString();
            }
            else
            {
                base.transform.Find("BG/Unselected/FakePrize").gameObject.SetActive(false);
                base.transform.Find("BG/Selected/FakePrize").gameObject.SetActive(false);
            }
            if (!this._isMultiCurrency)
            {
                base.transform.Find("BG/Unselected/NowPrize/Image").gameObject.SetActive(true);
                base.transform.Find("BG/Unselected/NowPrize/x").gameObject.SetActive(true);
                base.transform.Find("BG/Unselected/NowPrize/Num").gameObject.SetActive(true);
                base.transform.Find("BG/Selected/NowPrize/Image").gameObject.SetActive(true);
                base.transform.Find("BG/Selected/NowPrize/x").gameObject.SetActive(true);
                base.transform.Find("BG/Selected/NowPrize/Num").gameObject.SetActive(true);
            }
        }

        private void SetupRarityView(StorageDataItemBase item)
        {
            base.transform.Find("AvatarStar").gameObject.SetActive(false);
            base.transform.Find("Star").gameObject.SetActive(false);
            string hexString = MiscData.Config.ItemRarityColorList[item.rarity];
            base.transform.Find("ItemIcon").GetComponent<Image>().color = Miscs.ParseColor(hexString);
            if (item is AvatarCardDataItem)
            {
                AvatarDataItem dummyAvatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.ID).avatarID);
                this.SetupAvatarStar(dummyAvatarDataItem.star);
                base.transform.Find("AvatarStar").gameObject.SetActive(true);
            }
            else if (!(item is AvatarFragmentDataItem))
            {
                base.transform.Find("Star").gameObject.SetActive(true);
                int rarity = item.rarity;
                if (item is WeaponDataItem)
                {
                    rarity = (item as WeaponDataItem).GetMaxRarity();
                }
                else if (item is StigmataDataItem)
                {
                    rarity = (item as StigmataDataItem).GetMaxRarity();
                }
                base.transform.Find("Star").GetComponent<MonoItemIconStar>().SetupView(item.rarity, rarity);
            }
        }

        private void SetupSaleLabel(ShopGoodsMetaData goodsItem)
        {
            if (goodsItem.IsSuperWorth)
            {
                base.transform.Find("SaleLabel").gameObject.SetActive(true);
                base.transform.Find("SaleLabel/Bg").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/ShopIcons/SalePatternSuperWorth");
                base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountSuperWorth", new object[0]);
            }
            else if (goodsItem.Discount < 0x2710)
            {
                base.transform.Find("SaleLabel").gameObject.SetActive(true);
                base.transform.Find("SaleLabel/Bg").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/ShopIcons/SalePatternDiscount");
                int num = Mathf.RoundToInt((float) (goodsItem.Discount / 0x3e8));
                switch (num)
                {
                    case 1:
                        base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountOne", new object[0]);
                        return;

                    case 2:
                        base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountTwo", new object[0]);
                        return;

                    case 3:
                        base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountThree", new object[0]);
                        return;

                    case 4:
                        base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountFour", new object[0]);
                        return;

                    case 5:
                        base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountFive", new object[0]);
                        return;

                    case 6:
                        base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountSix", new object[0]);
                        return;

                    case 7:
                        base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountSeven", new object[0]);
                        return;

                    case 8:
                        base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountEight", new object[0]);
                        return;
                }
                if (num == 9)
                {
                    base.transform.Find("SaleLabel/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopStoreDiscountNine", new object[0]);
                }
            }
            else
            {
                base.transform.Find("SaleLabel").gameObject.SetActive(false);
            }
        }

        private void SetupStigmataTypeIcon(StorageDataItemBase item)
        {
            base.transform.Find("StigmataType").gameObject.SetActive(item is StigmataDataItem);
            if (item is StigmataDataItem)
            {
                base.transform.Find("StigmataType/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.StigmataTypeIconPath[item.GetBaseType()]);
            }
        }

        public void SetupView(Goods goods, bool isSelected, int ticketID, bool isMultiCurrency = false)
        {
            ShopGoodsMetaData shopGoodsMetaDataByKey;
            this._goods = goods;
            this._ticketID = ticketID;
            this._isMultiCurrency = isMultiCurrency;
            this.SetItemDefaultColor();
            base.transform.Find("BG/Selected").gameObject.SetActive(isSelected);
            base.transform.Find("BG/Unselected").gameObject.SetActive(!isSelected);
            if (this._ticketID > 0)
            {
                int hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[this._ticketID];
                shopGoodsMetaDataByKey = new ShopGoodsMetaData(this._ticketID, this._ticketID, 1, 1, hCoinCost, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x7fffffff, 1, 0x2710, false);
                if (Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(this._ticketID / 10))
                {
                    hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[this._ticketID / 10];
                    shopGoodsMetaDataByKey = new ShopGoodsMetaData(this._ticketID, this._ticketID / 10, 1, 10, hCoinCost * 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x7fffffff, 1, 0x2710, false);
                }
            }
            else
            {
                shopGoodsMetaDataByKey = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int) goods.get_goods_id());
            }
            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(shopGoodsMetaDataByKey.ItemID, shopGoodsMetaDataByKey.ItemLevel);
            dummyStorageDataItem.number = shopGoodsMetaDataByKey.ItemNum;
            this.SetupSaleLabel(shopGoodsMetaDataByKey);
            base.transform.Find("FragmentIcon").gameObject.SetActive(dummyStorageDataItem is AvatarFragmentDataItem);
            Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetIconPath());
            base.transform.Find("ItemIcon/Icon").GetComponent<Image>().sprite = spriteByPrefab;
            base.transform.Find("ItemIcon/FrameBg").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[dummyStorageDataItem.rarity]);
            base.transform.Find("ItemIcon/FrameLight").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityLightImgPath[dummyStorageDataItem.rarity]);
            base.transform.Find("ItemIcon/FrameBg").gameObject.SetActive(true);
            base.transform.Find("ItemIcon/FrameLight").gameObject.SetActive(true);
            this.SetupDesc(dummyStorageDataItem);
            this.SetupRarityView(dummyStorageDataItem);
            this.SetupStigmataTypeIcon(dummyStorageDataItem);
            this.SetupPrice(shopGoodsMetaDataByKey);
            if (goods.get_buy_times() >= shopGoodsMetaDataByKey.MaxBuyTimes)
            {
                this.SetItemGrey();
            }
            else
            {
                base.transform.Find("BG/Unselected/NowPrize").gameObject.SetActive(true);
                base.transform.Find("BG/Unselected/Empty").gameObject.SetActive(false);
                Color color = base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color;
                color.a = 1f;
                base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color = color;
                base.transform.Find("BG/Selected/NowPrize").gameObject.SetActive(true);
                base.transform.Find("BG/Selected/Empty").gameObject.SetActive(false);
                color = base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color;
                color.a = 1f;
                base.transform.Find("BG/Unselected/Image").GetComponent<Image>().color = color;
                base.transform.Find("ItemIcon/SellOut").gameObject.SetActive(false);
            }
        }
    }
}

