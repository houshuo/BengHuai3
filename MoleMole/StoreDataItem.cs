namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public class StoreDataItem
    {
        public List<int> currencyIDList;
        public List<Goods> goodsList;
        public bool isOpen;
        public int manualRefreshTimes;
        public int maxManualRefreshTimes;
        public uint nextAutoRefreshTime;
        public int nextRefreshCost;
        public int refreshItemID;
        public uint scheduleChangeTime;
        public string shopButtonNameTextID;
        public int shopID;
        public string shopNameTextID;
        public UIShopType shopType;

        public StoreDataItem(Shop shopDataItem)
        {
            this.shopType = UIShopType.SHOP_GACHATICKET;
            this.shopID = (int) shopDataItem.get_shop_id();
            this.isOpen = shopDataItem.get_is_open();
            if (shopDataItem.get_shop_typeSpecified())
            {
                this.shopType = (UIShopType) shopDataItem.get_shop_type();
            }
            this.shopNameTextID = shopDataItem.get_text_map_name();
            this.shopButtonNameTextID = shopDataItem.get_text_map_button_name();
            this.goodsList = shopDataItem.get_goods_list();
            this.nextAutoRefreshTime = shopDataItem.get_next_auto_refresh_time();
            this.scheduleChangeTime = shopDataItem.get_schedule_change_time();
            this.manualRefreshTimes = (int) shopDataItem.get_manual_refresh_times();
            this.refreshItemID = (int) shopDataItem.get_refresh_item();
            this.nextRefreshCost = (int) shopDataItem.get_next_refresh_cost();
            this.maxManualRefreshTimes = (int) shopDataItem.get_max_manual_refresh_times();
            this.currencyIDList = new List<int>();
            foreach (uint num in shopDataItem.get_currency_list())
            {
                this.currencyIDList.Add((int) num);
            }
        }

        public StoreDataItem(bool isOpen, string shopNameTextID, string shopButtonNameTextID, List<Goods> goodsList)
        {
            this.shopType = UIShopType.SHOP_GACHATICKET;
            this.isOpen = isOpen;
            this.shopNameTextID = shopNameTextID;
            this.shopButtonNameTextID = shopButtonNameTextID;
            this.goodsList = goodsList;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.shopID, this.isOpen, this.shopNameTextID, this.nextAutoRefreshTime, this.manualRefreshTimes, this.nextRefreshCost, this.maxManualRefreshTimes };
            string str = string.Format("<StoreDataItem>\nID: {0}\nisOpen: {1}\nnameID: {2}\nnextAutoRefreshTime: {3}\nmanualRefreshTimes: {4}\nnextRefreshHCoinCost: {5}\nmaxManualRefreshTimes: {6}", args) + "\ngoodsList: " + this.goodsList.Count.ToString() + "\n";
            foreach (Goods goods in this.goodsList)
            {
                ShopGoodsMetaData shopGoodsMetaDataByKey = ShopGoodsMetaDataReader.GetShopGoodsMetaDataByKey((int) goods.get_goods_id());
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(shopGoodsMetaDataByKey.ItemID, shopGoodsMetaDataByKey.ItemLevel);
                dummyStorageDataItem.number = shopGoodsMetaDataByKey.ItemNum;
                string str2 = str;
                object[] objArray2 = new object[] { 
                    str2, "ID: ", goods.get_goods_id(), " name: ", dummyStorageDataItem.GetDisplayTitle(), " level: ", shopGoodsMetaDataByKey.ItemLevel.ToString(), " number: ", shopGoodsMetaDataByKey.ItemNum.ToString(), " hcoinCost: ", shopGoodsMetaDataByKey.HCoinCost.ToString(), " scoinCost: ", shopGoodsMetaDataByKey.SCoinCost.ToString(), " maxBuyTimes: ", shopGoodsMetaDataByKey.MaxBuyTimes.ToString(), " buyTimes: ", 
                    goods.get_buy_times().ToString(), "\n"
                 };
                str = string.Concat(objArray2);
            }
            return str;
        }

        public void UpdateFromShop(Shop shopDataItem)
        {
            this.shopID = (int) shopDataItem.get_shop_id();
            this.isOpen = shopDataItem.get_is_open();
            this.shopNameTextID = shopDataItem.get_text_map_name();
            this.shopButtonNameTextID = shopDataItem.get_text_map_button_name();
            this.goodsList = shopDataItem.get_goods_list();
            this.nextAutoRefreshTime = shopDataItem.get_next_auto_refresh_time();
            this.scheduleChangeTime = shopDataItem.get_schedule_change_time();
            this.manualRefreshTimes = (int) shopDataItem.get_manual_refresh_times();
            this.refreshItemID = (int) shopDataItem.get_refresh_item();
            this.nextRefreshCost = (int) shopDataItem.get_next_refresh_cost();
            this.maxManualRefreshTimes = (int) shopDataItem.get_max_manual_refresh_times();
            this.currencyIDList = new List<int>();
            foreach (uint num in shopDataItem.get_currency_list())
            {
                this.currencyIDList.Add((int) num);
            }
        }
    }
}

