namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public class StoreModule : BaseModule
    {
        private Dictionary<UIShopType, StoreDataItem> _shopDict;
        private List<StoreDataItem> _storeDataItemList;

        public StoreModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._storeDataItemList = new List<StoreDataItem>();
            this._shopDict = new Dictionary<UIShopType, StoreDataItem>();
        }

        private Goods GetGoods(int shopID, int goodsID)
        {
            foreach (StoreDataItem item in this._storeDataItemList)
            {
                if (item.shopID == shopID)
                {
                    foreach (Goods goods in item.goodsList)
                    {
                        if (goods.get_goods_id() == goodsID)
                        {
                            return goods;
                        }
                    }
                }
            }
            return null;
        }

        public StoreDataItem GetStoreDataByType(UIShopType shopType)
        {
            if (this._shopDict.ContainsKey(shopType))
            {
                return this._shopDict[shopType];
            }
            return null;
        }

        public List<StoreDataItem> GetStoreDataItemList()
        {
            return this._storeDataItemList;
        }

        public StoreDataItem GetStoreDateItemByID(int shopID)
        {
            foreach (StoreDataItem item in this._storeDataItemList)
            {
                if (item.shopID == shopID)
                {
                    return item;
                }
            }
            return null;
        }

        private void MergeStoreDataItemList(List<Shop> shopList)
        {
            foreach (Shop shop in shopList)
            {
                StoreDataItem storeDateItemByID = this.GetStoreDateItemByID((int) shop.get_shop_id());
                if (storeDateItemByID == null)
                {
                    StoreDataItem item = new StoreDataItem(shop);
                    this._storeDataItemList.Add(item);
                    this._shopDict[(UIShopType) shop.get_shop_type()] = item;
                    if ((item.shopType == UIShopType.SHOP_ACTIVITY) && item.isOpen)
                    {
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ActivtyShopScheduleChange, true));
                    }
                }
                else
                {
                    bool flag = false;
                    if ((storeDateItemByID.shopType == UIShopType.SHOP_ACTIVITY) && (storeDateItemByID.isOpen != shop.get_is_open()))
                    {
                        flag = true;
                    }
                    storeDateItemByID.UpdateFromShop(shop);
                    if (flag)
                    {
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ActivtyShopScheduleChange, shop.get_is_open()));
                    }
                }
            }
        }

        private bool OnBuyGoodsRsp(BuyGoodsRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                Goods goods = this.GetGoods((int) rsp.get_shop_id(), (int) rsp.get_goods_id());
                if (goods != null)
                {
                    goods.set_buy_times(rsp.get_goods_buy_times());
                }
                Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
            }
            return false;
        }

        private bool OnGetShopListRsp(GetShopListRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.MergeStoreDataItemList(rsp.get_shop_list());
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0xca:
                    return this.OnGetShopListRsp(pkt.getData<GetShopListRsp>());

                case 0xcc:
                    return this.OnBuyGoodsRsp(pkt.getData<BuyGoodsRsp>());
            }
            return false;
        }
    }
}

