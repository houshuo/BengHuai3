namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class ShopPageContext : BasePageContext
    {
        private UIShopType _currentShopType;
        private const string StoreTab = "ShopTab";
        private const string TAB_SELECT_BG_PATH = "SpriteOutput/GeneralUI/TabDialogSelected";
        private const string TAB_UNSELECT_BG_PATH = "SpriteOutput/GeneralUI/TabDialogUnselected";

        public ShopPageContext(UIShopType shopType = 1)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ShopPageContext",
                viewPrefabPath = "UI/Menus/Page/Shop/ShopOverviewPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            base.showSpaceShip = false;
            this._currentShopType = shopType;
        }

        public override void BackPage()
        {
            base.BackPage();
        }

        public override void BackToMainMenuPage()
        {
            base.BackToMainMenuPage();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("CartInfoPanel/BuyBtn").GetComponent<Button>(), new UnityAction(this.OnBuyBtnClick));
            base.BindViewCallback(base.view.transform.Find("ShopTab/SystemInfoPanel/TitleTab/TabBtns/TabBtn_1").GetComponent<Button>(), new UnityAction(this.SetupNormalStoreTab));
            base.BindViewCallback(base.view.transform.Find("ShopTab/SystemInfoPanel/TitleTab/TabBtns/TabBtn_2").GetComponent<Button>(), new UnityAction(this.SetupEndlessStoreTab));
            base.BindViewCallback(base.view.transform.Find("ShopTab/SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").GetComponent<Button>(), new UnityAction(this.SetupGachaTicketStoreTab));
            base.BindViewCallback(base.view.transform.Find("ShopTab/SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity").GetComponent<Button>(), new UnityAction(this.SetupActivityStoreTab));
        }

        public void OnBuyBtnClick()
        {
            base.view.transform.Find("ShopTab").gameObject.GetComponent<MonoShopStoreTab>().OnBuyGoods();
        }

        private bool OnBuyGachaTicket(BuyGachaTicketRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) rsp.get_material_id(), 1);
                object[] replaceParams = new object[] { dummyStorageDataItem.GetDisplayTitle(), (int) rsp.get_num() };
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_BuyGachaTicketSuccess", replaceParams), 2f), UIType.Any);
                Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
                return this.SetupView();
            }
            return false;
        }

        private bool OnBuyGoodsRsp(BuyGoodsRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.SetupView();
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopBuyGoodsSuccess", new object[0]), 2f), UIType.Any);
                base.view.transform.Find("ShopTab").gameObject.GetComponent<MonoShopStoreTab>().OnBuyGoodsRsp(rsp);
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
                };
                dialogContext.desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                if (!string.IsNullOrEmpty(dialogContext.desc))
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
                if ((rsp.get_retcode() == 6) || (rsp.get_retcode() == 7))
                {
                    Singleton<NetworkManager>.Instance.RequestGetShopList();
                }
            }
            return false;
        }

        private void OnEnterRechargeTab()
        {
        }

        private bool OnGetMainDataRsp(GetMainDataRsp rsp)
        {
            if (rsp.get_hcoinSpecified() || rsp.get_scoinSpecified())
            {
                this.SetupView();
            }
            return false;
        }

        private bool OnGetShopListRsp(GetShopListRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.SetupView();
            }
            return false;
        }

        private bool OnManualRefreshShopRsp(ManualRefreshShopRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopManualRefreshSuccess", new object[0]), 2f), UIType.Any);
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
                };
                dialogContext.desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                if (!string.IsNullOrEmpty(dialogContext.desc))
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
            }
            return false;
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.SelectStoreGoodsItem) && this.OnSelectStoreGoodsItem((Goods) ntf.body));
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0xca:
                    return this.OnGetShopListRsp(pkt.getData<GetShopListRsp>());

                case 0xcc:
                    return this.OnBuyGoodsRsp(pkt.getData<BuyGoodsRsp>());

                case 0xce:
                    return this.OnManualRefreshShopRsp(pkt.getData<ManualRefreshShopRsp>());

                case 11:
                    return this.OnGetMainDataRsp(pkt.getData<GetMainDataRsp>());

                case 0xd8:
                    return this.OnBuyGachaTicket(pkt.getData<BuyGachaTicketRsp>());
            }
            return false;
        }

        private bool OnSelectStoreGoodsItem(Goods goods)
        {
            Transform transform = base.view.transform.Find("ShopTab");
            if (transform != null)
            {
                MonoShopStoreTab component = transform.GetComponent<MonoShopStoreTab>();
                if (component != null)
                {
                    component.OnSelectGoods(goods);
                }
            }
            return false;
        }

        private void SetupActivityStoreTab()
        {
            this._currentShopType = UIShopType.SHOP_ACTIVITY;
            GameObject gameObject = base.view.transform.Find("ShopTab").gameObject;
            StoreDataItem storeDataByType = Singleton<StoreModule>.Instance.GetStoreDataByType(UIShopType.SHOP_ACTIVITY);
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity").GetComponent<Button>().interactable = false;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1/Text").GetComponent<Text>().color = Color.white;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2/Text").GetComponent<Text>().color = Color.white;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket/Text").GetComponent<Text>().color = Color.white;
            if (storeDataByType != null)
            {
                base.view.transform.Find("ShopTab").GetComponent<MonoShopStoreTab>().SetupView(UIShopType.SHOP_ACTIVITY, storeDataByType, gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity/Text").GetComponent<Text>(), true, true);
            }
        }

        private void SetupEndlessStoreTab()
        {
            this._currentShopType = UIShopType.SHOP_ENDLESS;
            GameObject gameObject = base.view.transform.Find("ShopTab").gameObject;
            StoreDataItem storeDataByType = Singleton<StoreModule>.Instance.GetStoreDataByType(UIShopType.SHOP_ENDLESS);
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2").GetComponent<Button>().interactable = false;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1/Text").GetComponent<Text>().color = Color.white;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket/Text").GetComponent<Text>().color = Color.white;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity/Text").GetComponent<Text>().color = Color.white;
            if (storeDataByType != null)
            {
                base.view.transform.Find("ShopTab").GetComponent<MonoShopStoreTab>().SetupView(UIShopType.SHOP_ENDLESS, storeDataByType, gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2/Text").GetComponent<Text>(), true, true);
            }
        }

        private void SetupGachaTicketStoreTab()
        {
            this._currentShopType = UIShopType.SHOP_GACHATICKET;
            GameObject gameObject = base.view.transform.Find("ShopTab").gameObject;
            List<Goods> goodsList = new List<Goods>();
            foreach (int num in Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.Keys)
            {
                Goods item = new Goods();
                item.set_goods_id((uint) num);
                goodsList.Add(item);
            }
            StoreDataItem storeDataItem = new StoreDataItem(true, "Menu_Label_GachaTicket", "Menu_Label_GachaTicket", goodsList);
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").GetComponent<Button>().interactable = false;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1/Text").GetComponent<Text>().color = Color.white;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2/Text").GetComponent<Text>().color = Color.white;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity/Text").GetComponent<Text>().color = Color.white;
            if (storeDataItem != null)
            {
                base.view.transform.Find("ShopTab").GetComponent<MonoShopStoreTab>().SetupView(UIShopType.SHOP_GACHATICKET, storeDataItem, gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket/Text").GetComponent<Text>(), true, true);
            }
        }

        private void SetupNormalStoreTab()
        {
            this._currentShopType = UIShopType.SHOP_NORMAL;
            GameObject gameObject = base.view.transform.Find("ShopTab").gameObject;
            StoreDataItem storeDataByType = Singleton<StoreModule>.Instance.GetStoreDataByType(UIShopType.SHOP_NORMAL);
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1").GetComponent<Button>().interactable = false;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity").GetComponent<Button>().interactable = true;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1/Text").GetComponent<Text>().color = MiscData.GetColor("Black");
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_2/Text").GetComponent<Text>().color = Color.white;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket/Text").GetComponent<Text>().color = Color.white;
            gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_Activity/Text").GetComponent<Text>().color = Color.white;
            if (storeDataByType != null)
            {
                base.view.transform.Find("ShopTab").GetComponent<MonoShopStoreTab>().SetupView(UIShopType.SHOP_NORMAL, storeDataByType, gameObject.transform.Find("SystemInfoPanel/TitleTab/TabBtns/TabBtn_1/Text").GetComponent<Text>(), true, true);
            }
        }

        protected override bool SetupView()
        {
            bool flag = Singleton<PlayerModule>.Instance.playerData.teamLevel >= MiscData.Config.BasicConfig.GachaUnlockNeedPlayerLevel;
            if (!flag && (this._currentShopType == UIShopType.SHOP_GACHATICKET))
            {
                this._currentShopType = UIShopType.SHOP_NORMAL;
            }
            switch (this._currentShopType)
            {
                case UIShopType.SHOP_NORMAL:
                    this.SetupNormalStoreTab();
                    break;

                case UIShopType.SHOP_ENDLESS:
                    this.SetupEndlessStoreTab();
                    break;

                case UIShopType.SHOP_ACTIVITY:
                    this.SetupActivityStoreTab();
                    break;

                case UIShopType.SHOP_GACHATICKET:
                    this.SetupGachaTicketStoreTab();
                    break;
            }
            base.view.transform.Find("ShopTab/SystemInfoPanel/TitleTab/TabBtns/TabBtn_GachaTicket").gameObject.SetActive(flag);
            this.TrySetCartInfoPanel();
            return false;
        }

        private void TrySetCartInfoPanel()
        {
            base.view.transform.Find("CartInfoPanel").gameObject.SetActive(true);
            base.view.transform.Find("CartInfoPanel/Info").gameObject.SetActive(false);
        }
    }
}

