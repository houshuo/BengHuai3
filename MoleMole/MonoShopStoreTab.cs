namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoShopStoreTab : MonoBehaviour
    {
        private int _currentSelectedGoodsID;
        private bool _hasPlayAnim;
        private Transform _scrollViewTrans;
        private UIShopType _shopType;
        private StoreDataItem _storeDataItem;
        private const string ACTIVITY_SHOP_ITEM_BUTTON_PREFAB_PATH = "UI/Menus/Widget/Shop/ActivityItemButton";

        private bool CanBuyGoods()
        {
            ShopGoodsMetaData shopGoodsMetaDataByKey;
            if (this._currentSelectedGoodsID == 0)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopNoSelect", new object[0]), 2f), UIType.Any);
                return false;
            }
            Goods goodsByID = this.GetGoodsByID(this._currentSelectedGoodsID);
            if (goodsByID == null)
            {
                return false;
            }
            if (this._shopType == UIShopType.SHOP_GACHATICKET)
            {
                int hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[(int) goodsByID.get_goods_id()];
                shopGoodsMetaDataByKey = new ShopGoodsMetaData((int) goodsByID.get_goods_id(), (int) goodsByID.get_goods_id(), 1, 1, hCoinCost, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x7fffffff, 1, 0x2710, false);
            }
            else
            {
                shopGoodsMetaDataByKey = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int) goodsByID.get_goods_id());
            }
            if (goodsByID.get_buy_times() >= shopGoodsMetaDataByKey.MaxBuyTimes)
            {
                return false;
            }
            List<int> list = (this._shopType != UIShopType.SHOP_GACHATICKET) ? UIUtil.GetGoodsRealPrice(goodsByID) : new List<int> { Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[(int) goodsByID.get_goods_id()] };
            int num2 = 0;
            if (shopGoodsMetaDataByKey.HCoinCost > 0)
            {
                if (list[num2] > Singleton<PlayerModule>.Instance.playerData.hcoin)
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackHCoin", new object[0]), 2f), UIType.Any);
                    return false;
                }
                num2++;
            }
            if (shopGoodsMetaDataByKey.SCoinCost > 0)
            {
                if (list[num2] > Singleton<PlayerModule>.Instance.playerData.scoin)
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin", new object[0]), 2f), UIType.Any);
                    return false;
                }
                num2++;
            }
            if (shopGoodsMetaDataByKey.CostItemNum > 0)
            {
                StorageDataItemBase storageItemByTypeAndID = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), shopGoodsMetaDataByKey.CostItemId);
                if ((storageItemByTypeAndID == null) || (list[num2] > storageItemByTypeAndID.number))
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin", new object[0]), 2f), UIType.Any);
                    return false;
                }
                num2++;
            }
            if (shopGoodsMetaDataByKey.CostItemNum2 > 0)
            {
                StorageDataItemBase base3 = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), shopGoodsMetaDataByKey.CostItemId2);
                if ((base3 == null) || (list[num2] > base3.number))
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin", new object[0]), 2f), UIType.Any);
                    return false;
                }
                num2++;
            }
            if (shopGoodsMetaDataByKey.CostItemNum3 > 0)
            {
                StorageDataItemBase base4 = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), shopGoodsMetaDataByKey.CostItemId3);
                if ((base4 == null) || (list[num2] > base4.number))
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin", new object[0]), 2f), UIType.Any);
                    return false;
                }
                num2++;
            }
            if (shopGoodsMetaDataByKey.CostItemNum4 > 0)
            {
                StorageDataItemBase base5 = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), shopGoodsMetaDataByKey.CostItemId4);
                if ((base5 == null) || (list[num2] > base5.number))
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin", new object[0]), 2f), UIType.Any);
                    return false;
                }
                num2++;
            }
            if (shopGoodsMetaDataByKey.CostItemNum5 > 0)
            {
                StorageDataItemBase base6 = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), shopGoodsMetaDataByKey.CostItemId5);
                if ((base6 == null) || (list[num2] > base6.number))
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsLackSCoin", new object[0]), 2f), UIType.Any);
                    return false;
                }
                num2++;
            }
            if (Singleton<StorageModule>.Instance.IsFull())
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsBagFull", new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                return false;
            }
            return true;
        }

        private void DoRequestToRefresh(bool confirmed)
        {
            if (confirmed)
            {
                Singleton<NetworkManager>.Instance.RequestManualRefreshShop(this._storeDataItem.shopID);
            }
        }

        private Goods GetGoodsByID(int goodsID)
        {
            foreach (Goods goods in this._storeDataItem.goodsList)
            {
                if (goods.get_goods_id() == goodsID)
                {
                    return goods;
                }
            }
            return null;
        }

        private void OnAutoRefreshTimeOutCallback()
        {
            if (Singleton<NetworkManager>.Instance != null)
            {
                Singleton<NetworkManager>.Instance.RequestGetShopList();
            }
        }

        public void OnBuyGoods()
        {
            if (this.CanBuyGoods())
            {
                if (this._shopType == UIShopType.SHOP_GACHATICKET)
                {
                    int ticketID = this._currentSelectedGoodsID;
                    int num = 1;
                    if (Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(this._currentSelectedGoodsID / 10))
                    {
                        ticketID = this._currentSelectedGoodsID / 10;
                        num = 10;
                    }
                    Singleton<NetworkManager>.Instance.RequestBuyGachaTicket(ticketID, num);
                }
                else
                {
                    Singleton<NetworkManager>.Instance.RequestBuyGoods(this._storeDataItem.shopID, this._currentSelectedGoodsID);
                }
            }
        }

        public void OnBuyGoodsRsp(BuyGoodsRsp rsp)
        {
            this._storeDataItem = Singleton<StoreModule>.Instance.GetStoreDateItemByID(this._storeDataItem.shopID);
            if (rsp.get_retcode() == null)
            {
                if (this._shopType == UIShopType.SHOP_ACTIVITY)
                {
                    this.RefreshActivityScroller();
                }
                else
                {
                    this._scrollViewTrans.GetComponent<MonoGridScroller>().RefreshCurrent();
                }
                if (this._currentSelectedGoodsID != 0)
                {
                    Goods goodsByID = this.GetGoodsByID(this._currentSelectedGoodsID);
                    if (goodsByID != null)
                    {
                        this.OnSelectGoods(goodsByID);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            this._currentSelectedGoodsID = 0;
        }

        private void OnDisable()
        {
            this._currentSelectedGoodsID = 0;
        }

        public void OnManualRefresh()
        {
            if (this._storeDataItem.manualRefreshTimes < this._storeDataItem.maxManualRefreshTimes)
            {
                GeneralDialogContext context;
                string text = LocalizationGeneralLogic.GetText("Menu_Hcoin", new object[0]);
                if (this._storeDataItem.refreshItemID == 0)
                {
                    if (this._storeDataItem.nextRefreshCost > Singleton<PlayerModule>.Instance.playerData.hcoin)
                    {
                        object[] replaceParams = new object[] { text };
                        Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopManualRefreshLackHCoin", replaceParams), 2f), UIType.Any);
                    }
                    else
                    {
                        context = new GeneralDialogContext {
                            type = GeneralDialogContext.ButtonType.DoubleButton,
                            title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
                        };
                        object[] objArray2 = new object[] { this._storeDataItem.nextRefreshCost, text };
                        context.desc = LocalizationGeneralLogic.GetText("Menu_Desc_RefreshShopHint", objArray2);
                        context.buttonCallBack = new Action<bool>(this.DoRequestToRefresh);
                        Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                    }
                }
                else
                {
                    int number = 0;
                    StorageDataItemBase base2 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(this._storeDataItem.refreshItemID);
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(this._storeDataItem.refreshItemID, 1);
                    if (base2 != null)
                    {
                        number = base2.number;
                    }
                    if (this._storeDataItem.nextRefreshCost > number)
                    {
                        object[] objArray3 = new object[] { dummyStorageDataItem.GetDisplayTitle() };
                        Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopManualRefreshLackHCoin", objArray3), 2f), UIType.Any);
                    }
                    else
                    {
                        context = new GeneralDialogContext {
                            type = GeneralDialogContext.ButtonType.DoubleButton,
                            title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
                        };
                        object[] objArray4 = new object[] { this._storeDataItem.nextRefreshCost, dummyStorageDataItem.GetDisplayTitle() };
                        context.desc = LocalizationGeneralLogic.GetText("Menu_Desc_RefreshShopHint", objArray4);
                        context.buttonCallBack = new Action<bool>(this.DoRequestToRefresh);
                        Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                    }
                }
            }
        }

        private void OnScrollChange(Transform trans, int index)
        {
            Goods goods = this._storeDataItem.goodsList[index];
            trans.GetComponent<MonoStoreGoodsItem>().SetupView(goods, goods.get_goods_id() == this._currentSelectedGoodsID, (this._shopType != UIShopType.SHOP_GACHATICKET) ? 0 : ((int) goods.get_goods_id()), false);
        }

        public void OnSelectGoods(Goods goods)
        {
            ShopGoodsMetaData shopGoodsMetaDataByKey;
            if (this._shopType == UIShopType.SHOP_GACHATICKET)
            {
                int iD = (int) goods.get_goods_id();
                int hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[(int) goods.get_goods_id()];
                shopGoodsMetaDataByKey = new ShopGoodsMetaData((int) goods.get_goods_id(), (int) goods.get_goods_id(), 1, 1, hCoinCost, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x7fffffff, 1, 0x2710, false);
                if (Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(iD / 10))
                {
                    hCoinCost = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[(int) (goods.get_goods_id() / 10)];
                    shopGoodsMetaDataByKey = new ShopGoodsMetaData(iD, iD / 10, 1, 10, hCoinCost * 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x7fffffff, 1, 0x2710, false);
                }
            }
            else
            {
                shopGoodsMetaDataByKey = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int) goods.get_goods_id());
            }
            if (shopGoodsMetaDataByKey != null)
            {
                this._currentSelectedGoodsID = (int) goods.get_goods_id();
                if (this._shopType == UIShopType.SHOP_ACTIVITY)
                {
                    this.RefreshActivityScroller();
                }
                else
                {
                    this._scrollViewTrans.GetComponent<MonoGridScroller>().RefreshCurrent();
                }
                base.transform.parent.Find("CartInfoPanel/Info").gameObject.SetActive(true);
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(shopGoodsMetaDataByKey.ItemID, shopGoodsMetaDataByKey.ItemLevel);
                string text = string.Empty;
                if (goods.get_buy_times() >= shopGoodsMetaDataByKey.MaxBuyTimes)
                {
                    text = LocalizationGeneralLogic.GetText("Menu_ShopStoreBuyCostLackMoney", new object[0]);
                    base.transform.parent.Find("CartInfoPanel/BuyBtn").GetComponent<Button>().interactable = false;
                }
                else
                {
                    base.transform.parent.Find("CartInfoPanel/BuyBtn").GetComponent<Button>().interactable = true;
                    if (shopGoodsMetaDataByKey.ItemNum > 1)
                    {
                        object[] replaceParams = new object[] { dummyStorageDataItem.GetDisplayTitle(), shopGoodsMetaDataByKey.ItemNum };
                        text = LocalizationGeneralLogic.GetText("Menu_ShopStoreBuyCostDescMulti", replaceParams);
                    }
                    else
                    {
                        object[] objArray2 = new object[] { dummyStorageDataItem.GetDisplayTitle() };
                        text = LocalizationGeneralLogic.GetText("Menu_ShopStoreBuyCostDescOne", objArray2);
                    }
                    if (this._shopType != UIShopType.SHOP_GACHATICKET)
                    {
                        int num3 = shopGoodsMetaDataByKey.MaxBuyTimes - ((int) goods.get_buy_times());
                        if (goods.get_can_be_refreshSpecified() && goods.get_can_be_refresh())
                        {
                            object[] objArray3 = new object[] { num3 };
                            text = text + LocalizationGeneralLogic.GetText("Menu_ShopStoreBuyCostRemainToday", objArray3);
                        }
                        else
                        {
                            object[] objArray4 = new object[] { num3 };
                            text = text + LocalizationGeneralLogic.GetText("Menu_ShopStoreBuyCostRemainTotal", objArray4);
                        }
                    }
                }
                base.transform.parent.Find("CartInfoPanel/Info/Desc").GetComponent<Text>().text = text;
            }
        }

        private void RefreshActivityScroller()
        {
            Transform transform = base.transform.Find("ScrollViewActivity/Content");
            for (int i = 0; i < this._storeDataItem.goodsList.Count; i++)
            {
                Transform child = transform.GetChild(i);
                Goods goods = this._storeDataItem.goodsList[i];
                child.GetComponent<MonoStoreGoodsItem>().SetupView(goods, goods.get_goods_id() == this._currentSelectedGoodsID, 0, true);
            }
        }

        private void SetShopIsOpen()
        {
            bool isOpen = this._storeDataItem.isOpen;
            base.transform.Find("ShopUnOpen").gameObject.SetActive(!isOpen);
            base.transform.Find("ArrowLeft").gameObject.SetActive(isOpen);
            base.transform.Find("ArrowRight").gameObject.SetActive(isOpen);
            base.transform.Find("ScrollView").gameObject.SetActive(isOpen);
            base.transform.Find("ScrollViewActivity").gameObject.SetActive(isOpen);
            base.transform.Find("RefreshTab").gameObject.SetActive(isOpen);
            base.transform.Find("SpecialDesc").gameObject.SetActive(isOpen);
        }

        private void SetupAutoRefreshInfo()
        {
            base.transform.Find("SystemInfoPanel/SystemRefresh").gameObject.SetActive(this._shopType != UIShopType.SHOP_GACHATICKET);
            if (this._shopType != UIShopType.SHOP_GACHATICKET)
            {
                bool flag = true;
                DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(this._storeDataItem.nextAutoRefreshTime);
                if (this._shopType == UIShopType.SHOP_ACTIVITY)
                {
                    if (this._storeDataItem.scheduleChangeTime != 0)
                    {
                        dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(this._storeDataItem.scheduleChangeTime);
                    }
                    else
                    {
                        flag = false;
                    }
                }
                base.transform.Find("SystemInfoPanel/SystemRefresh").gameObject.SetActive(flag);
                if (flag)
                {
                    base.transform.Find("SystemInfoPanel/SystemRefresh/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(dateTimeFromTimeStamp, null, new Action(this.OnAutoRefreshTimeOutCallback), false);
                }
            }
        }

        private void SetupGoodsItem(bool playAnim)
        {
            if (this._shopType == UIShopType.SHOP_ACTIVITY)
            {
                Transform transform = base.transform.Find("ScrollViewActivity");
                Transform parent = base.transform.Find("ScrollViewActivity/Content");
                transform.gameObject.SetActive(true);
                base.transform.Find("ScrollView").gameObject.SetActive(false);
                if (!this._storeDataItem.isOpen)
                {
                    transform.gameObject.SetActive(false);
                }
                else
                {
                    int num = this._storeDataItem.goodsList.Count - parent.childCount;
                    if (num > 0)
                    {
                        for (int i = 0; i < num; i++)
                        {
                            UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Shop/ActivityItemButton", BundleType.RESOURCE_FILE)).transform.SetParent(parent, false);
                        }
                    }
                    else if (num < 0)
                    {
                        num = -num;
                        for (int j = 0; j < num; j++)
                        {
                            UnityEngine.Object.Destroy(parent.GetChild(j));
                        }
                    }
                    this.RefreshActivityScroller();
                }
            }
            else
            {
                base.transform.Find("ScrollViewActivity").gameObject.SetActive(false);
                base.transform.Find("ScrollView").gameObject.SetActive(true);
                this._scrollViewTrans.GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(this.OnScrollChange), this._storeDataItem.goodsList.Count, new Vector2(0f, 0f));
                this._scrollViewTrans.gameObject.SetActive(true);
                if (playAnim && !this._hasPlayAnim)
                {
                    base.transform.Find("ScrollView/Content").GetComponent<Animation>().Play("GoodsItemsFadeIn");
                    this._hasPlayAnim = true;
                }
            }
        }

        private void SetupManualRefreshInfo()
        {
            base.transform.Find("RefreshTab").gameObject.SetActive((this._shopType == UIShopType.SHOP_NORMAL) || (this._shopType == UIShopType.SHOP_ACTIVITY));
            if ((this._shopType != UIShopType.SHOP_GACHATICKET) && (this._shopType != UIShopType.SHOP_ENDLESS))
            {
                if (this._shopType == UIShopType.SHOP_ACTIVITY)
                {
                    base.transform.Find("RefreshTab").gameObject.SetActive(this._storeDataItem.isOpen);
                    if (!this._storeDataItem.isOpen)
                    {
                        return;
                    }
                }
                base.transform.Find("RefreshTab/RefreshTime/Num").GetComponent<Text>().text = (this._storeDataItem.maxManualRefreshTimes - this._storeDataItem.manualRefreshTimes).ToString();
                base.transform.Find("RefreshTab/MetalNum/Num").GetComponent<Text>().text = this._storeDataItem.nextRefreshCost.ToString();
                base.transform.Find("RefreshTab/MetalNum/Num").GetComponent<Text>().color = MiscData.GetColor("TotalWhite");
                base.transform.Find("RefreshTab/MetalNum/x").GetComponent<Text>().color = MiscData.GetColor("TotalWhite");
                if (this._storeDataItem.refreshItemID == 0)
                {
                    base.transform.Find("RefreshTab/MetalNum/ImgMetal").GetComponent<Image>().sprite = UIUtil.GetResourceSprite(ResourceType.Hcoin, null);
                    if (Singleton<PlayerModule>.Instance.playerData.hcoin < this._storeDataItem.nextRefreshCost)
                    {
                        base.transform.Find("RefreshTab/MetalNum/Num").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                        base.transform.Find("RefreshTab/MetalNum/x").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                    }
                }
                else
                {
                    string iconPath = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(this._storeDataItem.refreshItemID, 1).GetIconPath();
                    if (MiscData.GetCurrencyIconPath(this._storeDataItem.refreshItemID) != null)
                    {
                        iconPath = MiscData.GetCurrencyIconPath(this._storeDataItem.refreshItemID);
                    }
                    base.transform.Find("RefreshTab/MetalNum/ImgMetal").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(iconPath);
                    int number = 0;
                    StorageDataItemBase base3 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(this._storeDataItem.refreshItemID);
                    if (base3 != null)
                    {
                        number = base3.number;
                    }
                    if (number < this._storeDataItem.nextRefreshCost)
                    {
                        base.transform.Find("RefreshTab/MetalNum/Num").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                        base.transform.Find("RefreshTab/MetalNum/x").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                    }
                }
                if (this._storeDataItem.manualRefreshTimes >= this._storeDataItem.maxManualRefreshTimes)
                {
                    base.transform.Find("RefreshTab/RefreshBtn").GetComponent<Button>().interactable = false;
                }
                else
                {
                    base.transform.Find("RefreshTab/RefreshBtn").GetComponent<Button>().interactable = true;
                }
            }
        }

        private void SetupMetalNum()
        {
            base.transform.Find("SpecialDesc").gameObject.SetActive(true);
            Transform transform = base.transform.Find("SystemInfoPanel/Currency");
            switch (this._shopType)
            {
                case UIShopType.SHOP_ACTIVITY:
                    base.transform.Find("SpecialDesc").gameObject.SetActive(false);
                    base.transform.Find("SystemInfoPanel/Currency").gameObject.SetActive(false);
                    transform = base.transform.Find("SystemInfoPanel/ActivityCurrency");
                    transform.gameObject.SetActive(true);
                    break;

                case UIShopType.SHOP_GACHATICKET:
                    base.transform.Find("SystemInfoPanel/Currency").gameObject.SetActive(false);
                    base.transform.Find("SystemInfoPanel/ActivityCurrency").gameObject.SetActive(false);
                    return;

                default:
                    base.transform.Find("SystemInfoPanel/ActivityCurrency").gameObject.SetActive(false);
                    transform.gameObject.SetActive(true);
                    break;
            }
            transform.gameObject.SetActive(this._storeDataItem.currencyIDList.Count > 0);
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (i >= this._storeDataItem.currencyIDList.Count)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    child.gameObject.SetActive(true);
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(this._storeDataItem.currencyIDList[i], 1);
                    string currencyIconPath = MiscData.GetCurrencyIconPath(dummyStorageDataItem.ID);
                    if (string.IsNullOrEmpty(currencyIconPath))
                    {
                        currencyIconPath = dummyStorageDataItem.GetIconPath();
                    }
                    child.Find("ImgMetal").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(currencyIconPath);
                    List<StorageDataItemBase> list = Singleton<StorageModule>.Instance.TryGetStorageDataItemByMetaId(this._storeDataItem.currencyIDList[i], 1);
                    int number = 0;
                    if (list.Count > 0)
                    {
                        if (dummyStorageDataItem is MaterialDataItem)
                        {
                            number = list[0].number;
                        }
                        else
                        {
                            number = list.Count;
                        }
                    }
                    child.Find("Num").GetComponent<Text>().text = number.ToString();
                }
            }
        }

        public void SetupView(UIShopType shopType, StoreDataItem storeDataItem, Text tabText, bool playAnim = true, bool clearCurrentSelectGoods = true)
        {
            this._shopType = shopType;
            if (clearCurrentSelectGoods)
            {
                this._currentSelectedGoodsID = 0;
                base.transform.parent.Find("CartInfoPanel/Info").gameObject.SetActive(false);
            }
            this._scrollViewTrans = base.transform.Find("ScrollView");
            this._storeDataItem = storeDataItem;
            this.SetShopIsOpen();
            tabText.text = LocalizationGeneralLogic.GetText(this._storeDataItem.shopNameTextID, new object[0]);
            this.SetupMetalNum();
            this.SetupAutoRefreshInfo();
            this.SetupManualRefreshInfo();
            this.SetupGoodsItem(playAnim);
            base.transform.Find("SpecialDesc").gameObject.SetActive(false);
        }
    }
}

