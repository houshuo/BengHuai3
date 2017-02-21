namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoShopRechargeTab : MonoBehaviour
    {
        private string _currentSelectedProductID = string.Empty;
        private bool _hasPlayAnim;
        private List<RechargeDataItem> _rechargeDataItemList;
        private Transform _scrollViewTrans;
        private bool _waitRefreshAfterSetupView;

        private void OnDestroy()
        {
            this._currentSelectedProductID = string.Empty;
        }

        private void OnDisable()
        {
        }

        public void OnPucharseProduct()
        {
            if (Singleton<NetworkManager>.Instance.DispatchSeverData.forbidRecharge)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ForbidRecharge", new object[0]), 2f), UIType.Any);
            }
            else if ((Singleton<NetworkManager>.Instance.DispatchSeverData.rechargeMaxLimit > 0) && (Singleton<ShopWelfareModule>.Instance.totalPayHCoin >= Singleton<NetworkManager>.Instance.DispatchSeverData.rechargeMaxLimit))
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopRechargeHCoinLimit", new object[0]), 2f), UIType.Any);
            }
            else if (string.IsNullOrEmpty(this._currentSelectedProductID))
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ShopNoSelect", new object[0]), 2f), UIType.Any);
            }
            else
            {
                RechargeDataItem storeItemByProductID = Singleton<AccountManager>.Instance.manager.GetStoreItemByProductID(this._currentSelectedProductID);
                if (storeItemByProductID != null)
                {
                    if (!storeItemByProductID.CanPurchase())
                    {
                        Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTPurchaseLimit", new object[0]), 2f), UIType.Any);
                    }
                    else
                    {
                        Singleton<AccountManager>.Instance.manager.Pay(this._currentSelectedProductID, storeItemByProductID.productName, ((float) storeItemByProductID.serverPrice) / 100f);
                    }
                }
                else
                {
                    this._currentSelectedProductID = string.Empty;
                }
            }
        }

        public void OnRefreshRechargeTab()
        {
            this._rechargeDataItemList = Singleton<AccountManager>.Instance.manager.GetRechargeItemList();
            foreach (RechargeDataItem item in this._rechargeDataItemList)
            {
            }
            this._currentSelectedProductID = string.Empty;
            this._scrollViewTrans.GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(this.OnScrollChange), this._rechargeDataItemList.Count, new Vector2(0f, 1f));
            this._scrollViewTrans.gameObject.SetActive(true);
            if (this._waitRefreshAfterSetupView && !this._hasPlayAnim)
            {
                base.transform.Find("ScrollView/Content").GetComponent<Animation>().Play("RechargeItemsFadeIn");
                this._waitRefreshAfterSetupView = false;
                this._hasPlayAnim = true;
            }
        }

        private void OnScrollChange(Transform trans, int index)
        {
            RechargeDataItem rechargeDataItem = this._rechargeDataItemList[index];
            trans.GetComponent<MonoRechargeItem>().SetupView(rechargeDataItem, rechargeDataItem.productID == this._currentSelectedProductID);
        }

        public void OnSelectProduct(string productID)
        {
            this._currentSelectedProductID = productID;
            this._scrollViewTrans.GetComponent<MonoGridScroller>().RefreshCurrent();
            RechargeDataItem storeItemByProductID = Singleton<AccountManager>.Instance.manager.GetStoreItemByProductID(productID);
            if (storeItemByProductID != null)
            {
                base.transform.parent.Find("CartInfoPanel/BuyBtn").GetComponent<Button>().interactable = true;
                base.transform.parent.Find("CartInfoPanel/Info").gameObject.SetActive(true);
                object[] replaceParams = new object[] { storeItemByProductID.formattedPrice, storeItemByProductID.productName };
                base.transform.parent.Find("CartInfoPanel/Info/Desc").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_ShopRechargeBuyDesc", replaceParams);
                this.SetupPayMethodView();
            }
        }

        public void SetPayMethodId(ChannelPayModule.PayMethod payMethodId)
        {
            Singleton<ChannelPayModule>.Instance.SetPayMethodId(payMethodId);
            this.SetupPayMethodView();
        }

        private void SetupPayMethodView()
        {
            bool flag = Singleton<AccountManager>.Instance.accountConfig.paymentBranch == ConfigAccount.PaymentBranch.ORIGINAL_ANDROID_PAY;
            base.transform.parent.Find("CartInfoPanel/Info/AndroidPayBtns").gameObject.SetActive(flag);
            if (flag)
            {
                bool flag2 = Singleton<ChannelPayModule>.Instance.GetPayMethodId() == ChannelPayModule.PayMethod.ALIPAY;
                base.transform.parent.Find("CartInfoPanel/Info/AndroidPayBtns/AilpayBtn/ActiveImage").gameObject.SetActive(flag2);
                base.transform.parent.Find("CartInfoPanel/Info/AndroidPayBtns/AilpayBtn/UnactiveImage").gameObject.SetActive(!flag2);
                base.transform.parent.Find("CartInfoPanel/Info/AndroidPayBtns/WechatBtn/ActiveImage").gameObject.SetActive(!flag2);
                base.transform.parent.Find("CartInfoPanel/Info/AndroidPayBtns/WechatBtn/UnactiveImage").gameObject.SetActive(flag2);
            }
        }

        public void SetupView()
        {
            this._currentSelectedProductID = string.Empty;
            this._scrollViewTrans = base.transform.Find("ScrollView");
            this._scrollViewTrans.gameObject.SetActive(false);
            Singleton<AccountManager>.Instance.manager.ShowAllProducts();
            this._waitRefreshAfterSetupView = true;
        }
    }
}

