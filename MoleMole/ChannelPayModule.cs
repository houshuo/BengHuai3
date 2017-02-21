namespace MoleMole
{
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class ChannelPayModule : BaseModule
    {
        private Coroutine _getProductListFromMarketCoroutine;
        private Coroutine _getProductListFromServerCoroutine;
        private LoadingWheelWidgetContext _loadingWheelDialogContext;
        private List<RechargeDataItem> _rechargeItemList;
        private List<RechargeDataItem> _rechargeItemListFromMarket;
        private List<RechargeDataItem> _rechargeItemListFromServer;
        private Coroutine _showAllProductsCoroutine;
        private const float _showAllProductsMaxWaitTime = 10f;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache8;
        private readonly string[] INVALID_TOKEN_LIST = new string[] { "ewoJInNpZ25hdHVyZSIgPSAiQXBkeEpkdE53UFUyckE1L2NuM2tJTzFPVGsyNWZlREthMGFhZ3l5UnZlV2xjRmxnbHY2UkY2em5raUJTM3VtOVVjN3BWb2IrUHFaUjJUOHd5VnJITnBsb2YzRFgzSXFET2xXcSs5MGE3WWwrcXJSN0E3ald3dml3NzA4UFMrNjdQeUhSbmhPL0c3YlZxZ1JwRXI2RXVGeWJpVTFGWEFpWEpjNmxzMVlBc3NReEFBQURWekNDQTFNd2dnSTdvQU1DQVFJQ0NHVVVrVTNaV0FTMU1BMEdDU3FHU0liM0RRRUJCUVVBTUg4eEN6QUpCZ05WQkFZVEFsVlRNUk13RVFZRFZRUUtEQXBCY0hCc1pTQkpibU11TVNZd0pBWURWUVFMREIxQmNIQnNaU0JEWlhKMGFXWnBZMkYwYVc5dUlFRjFkR2h2Y21sMGVURXpNREVHQTFVRUF3d3FRWEJ3YkdVZ2FWUjFibVZ6SUZOMGIzSmxJRU5sY25ScFptbGpZWFJwYjI0Z1FYVjBhRzl5YVhSNU1CNFhEVEE1TURZeE5USXlNRFUxTmxvWERURTBNRFl4TkRJeU1EVTFObG93WkRFak1DRUdBMVVFQXd3YVVIVnlZMmhoYzJWU1pXTmxhWEIwUTJWeWRHbG1hV05oZEdVeEd6QVpCZ05WQkFzTUVrRndjR3hsSUdsVWRXNWxjeUJUZEc5eVpURVRNQkVHQTFVRUNnd0tRWEJ3YkdVZ1NXNWpMakVMTUFrR0ExVUVCaE1DVlZNd2daOHdEUVlKS29aSWh2Y05BUUVCQlFBRGdZMEFNSUdKQW9HQkFNclJqRjJjdDRJclNkaVRDaGFJMGc4cHd2L2NtSHM4cC9Sd1YvcnQvOTFYS1ZoTmw0WElCaW1LalFRTmZnSHNEczZ5anUrK0RyS0pFN3VLc3BoTWRkS1lmRkU1ckdYc0FkQkVqQndSSXhleFRldngzSExFRkdBdDFtb0t4NTA5ZGh4dGlJZERnSnYyWWFWczQ5QjB1SnZOZHk2U01xTk5MSHNETHpEUzlvWkhBZ01CQUFHamNqQndNQXdHQTFVZEV3RUIvd1FDTUFBd0h3WURWUjBqQkJnd0ZvQVVOaDNvNHAyQzBnRVl0VEpyRHRkREM1RllRem93RGdZRFZSMFBBUUgvQkFRREFnZUFNQjBHQTFVZERnUVdCQlNwZzRQeUdVakZQaEpYQ0JUTXphTittVjhrOVRBUUJnb3Foa2lHOTJOa0JnVUJCQUlGQURBTkJna3Foa2lHOXcwQkFRVUZBQU9DQVFFQUVhU2JQanRtTjRDL0lCM1FFcEszMlJ4YWNDRFhkVlhBZVZSZVM1RmFaeGMrdDg4cFFQOTNCaUF4dmRXLzNlVFNNR1k1RmJlQVlMM2V0cVA1Z204d3JGb2pYMGlreVZSU3RRKy9BUTBLRWp0cUIwN2tMczlRVWU4Y3pSOFVHZmRNMUV1bVYvVWd2RGQ0TndOWXhMUU1nNFdUUWZna1FRVnk4R1had1ZIZ2JFL1VDNlk3MDUzcEdYQms1MU5QTTN3b3hoZDNnU1JMdlhqK2xvSHNTdGNURXFlOXBCRHBtRzUrc2s0dHcrR0szR01lRU41LytlMVFUOW5wL0tsMW5qK2FCdzdDMHhzeTBiRm5hQWQxY1NTNnhkb3J5L0NVdk02Z3RLc21uT09kcVRlc2JwMGJzOHNuNldxczBDOWRnY3hSSHVPTVoydG04bnBMVW03YXJnT1N6UT09IjsKCSJwdXJjaGFzZS1pbmZvIiA9ICJld29KSW05eWFXZHBibUZzTFhCMWNtTm9ZWE5sTFdSaGRHVXRjSE4wSWlBOUlDSXlNREV5TFRBM0xURXlJREExT2pVME9qTTFJRUZ0WlhKcFkyRXZURzl6WDBGdVoyVnNaWE1pT3dvSkluQjFjbU5vWVhObExXUmhkR1V0YlhNaUlEMGdJakV6TkRJd09UYzJOelU0T0RJaU93b0pJbTl5YVdkcGJtRnNMWFJ5WVc1ellXTjBhVzl1TFdsa0lpQTlJQ0l4TnpBd01EQXdNamswTkRrME1qQWlPd29KSW1KMmNuTWlJRDBnSWpFdU5DSTdDZ2tpWVhCd0xXbDBaVzB0YVdRaUlEMGdJalExTURVME1qSXpNeUk3Q2draWRISmhibk5oWTNScGIyNHRhV1FpSUQwZ0lqRTNNREF3TURBeU9UUTBPVFF5TUNJN0Nna2ljWFZoYm5ScGRIa2lJRDBnSWpFaU93b0pJbTl5YVdkcGJtRnNMWEIxY21Ob1lYTmxMV1JoZEdVdGJYTWlJRDBnSWpFek5ESXdPVGMyTnpVNE9ESWlPd29KSW1sMFpXMHRhV1FpSUQwZ0lqVXpOREU0TlRBME1pSTdDZ2tpZG1WeWMybHZiaTFsZUhSbGNtNWhiQzFwWkdWdWRHbG1hV1Z5SWlBOUlDSTVNRFV4TWpNMklqc0tDU0p3Y205a2RXTjBMV2xrSWlBOUlDSmpiMjB1ZW1Wd2RHOXNZV0l1WTNSeVltOXVkWE11YzNWd1pYSndiM2RsY2pFaU93b0pJbkIxY21Ob1lYTmxMV1JoZEdVaUlEMGdJakl3TVRJdE1EY3RNVElnTVRJNk5UUTZNelVnUlhSakwwZE5WQ0k3Q2draWIzSnBaMmx1WVd3dGNIVnlZMmhoYzJVdFpHRjBaU0lnUFNBaU1qQXhNaTB3TnkweE1pQXhNam8xTkRvek5TQkZkR012UjAxVUlqc0tDU0ppYVdRaUlEMGdJbU52YlM1NlpYQjBiMnhoWWk1amRISmxlSEJsY21sdFpXNTBjeUk3Q2draWNIVnlZMmhoYzJVdFpHRjBaUzF3YzNRaUlEMGdJakl3TVRJdE1EY3RNVElnTURVNk5UUTZNelVnUVcxbGNtbGpZUzlNYjNOZlFXNW5aV3hsY3lJN0NuMD0iOwoJInBvZCIgPSAiMTciOwoJInNpZ25pbmctc3RhdHVzIiA9ICIwIjsKfQ==" };

        public ChannelPayModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._rechargeItemListFromServer = null;
            this._rechargeItemListFromMarket = null;
            this._rechargeItemList = new List<RechargeDataItem>();
        }

        private void AddReceipt(string receipt)
        {
            Singleton<MiHoYoGameData>.Instance.LocalData.Receipt = receipt;
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        private bool CheckNeedGetProductListFromMarket()
        {
            if (Singleton<AccountManager>.Instance.accountConfig.paymentBranch == ConfigAccount.PaymentBranch.APPSTORE_CN)
            {
                if (this._rechargeItemListFromMarket == null)
                {
                    return true;
                }
                for (int i = 0; i < this._rechargeItemListFromServer.Count; i++)
                {
                    RechargeDataItem item = new RechargeDataItem(this._rechargeItemListFromServer[i]);
                    bool flag = false;
                    for (int j = 0; j < this._rechargeItemListFromMarket.Count; j++)
                    {
                        if (item.productID == this._rechargeItemListFromMarket[j].productID)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void CheckReceipt()
        {
            if (!string.IsNullOrEmpty(this.GetReceipt()) && (this._loadingWheelDialogContext == null))
            {
                LoadingWheelWidgetContext context = new LoadingWheelWidgetContext {
                    ignoreMaxWaitTime = true
                };
                this._loadingWheelDialogContext = context;
                Singleton<MainUIManager>.Instance.ShowWidget(this._loadingWheelDialogContext, UIType.Any);
            }
        }

        private string GetBase64StringByKey(string receipt, string key)
        {
            int startIndex = (receipt.IndexOf("\"" + key + "\" = \"") + key.Length) + 6;
            int index = receipt.IndexOf("\";", startIndex);
            if (((startIndex != -1) && (index != -1)) && (index > startIndex))
            {
                return receipt.Substring(startIndex, index - startIndex);
            }
            return string.Empty;
        }

        public PayMethod GetPayMethodId()
        {
            return (PayMethod) Singleton<MiHoYoGameData>.Instance.LocalData.PayMethod;
        }

        public string GetReceipt()
        {
            return Singleton<MiHoYoGameData>.Instance.LocalData.Receipt;
        }

        public List<RechargeDataItem> GetRechargeItemList()
        {
            return this._rechargeItemList;
        }

        public RechargeDataItem GetStoreItemByProductID(string productID)
        {
            if (this._rechargeItemList != null)
            {
                for (int i = 0; i < this._rechargeItemList.Count; i++)
                {
                    if ((this._rechargeItemList[i] != null) && (this._rechargeItemList[i].productID == productID))
                    {
                        return this._rechargeItemList[i];
                    }
                }
            }
            return null;
        }

        private void MergeStoreItemList()
        {
            this._rechargeItemList.Clear();
            for (int i = 0; i < this._rechargeItemListFromServer.Count; i++)
            {
                RechargeDataItem item = new RechargeDataItem(this._rechargeItemListFromServer[i]);
                if (this._rechargeItemListFromMarket != null)
                {
                    for (int j = 0; j < this._rechargeItemListFromMarket.Count; j++)
                    {
                        if (item.productID == this._rechargeItemListFromMarket[j].productID)
                        {
                            item.formattedPrice = this._rechargeItemListFromMarket[j].formattedPrice;
                            this._rechargeItemList.Add(item);
                            break;
                        }
                    }
                }
                else
                {
                    this._rechargeItemList.Add(item);
                }
            }
        }

        private bool OnCreateWeiXinOrderRsp(CreateWeiXinOrderRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if (Singleton<AccountManager>.Instance.accountConfig.paymentBranch == ConfigAccount.PaymentBranch.ORIGINAL_ANDROID_PAY)
                {
                    TheOriginalAccountManager manager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
                    if (manager != null)
                    {
                        manager.PayByWeiXinOrder(rsp.get_partner_id(), rsp.get_prepay_id(), rsp.get_nonce_str(), rsp.get_timestamp(), rsp.get_sign());
                    }
                }
            }
            else
            {
                this.OnPurchaseCallback(new WeixinPayResult("ErrFailCreateWeiXinOrder"));
            }
            return false;
        }

        private bool OnGetProductsListRsp(GetProductListRsp rsp)
        {
            this._rechargeItemListFromServer = new List<RechargeDataItem>();
            if (rsp.get_retcode() == null)
            {
                foreach (Product product in rsp.get_product_list())
                {
                    RechargeDataItem item = new RechargeDataItem(product);
                    this._rechargeItemListFromServer.Add(item);
                }
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x63:
                    return this.OnGetProductsListRsp(pkt.getData<GetProductListRsp>());

                case 0x53:
                    return this.OnRechargeSuccNotify(pkt.getData<RechargeFinishNotify>());

                case 0xd0:
                    return this.OnCreateWeiXinOrderRsp(pkt.getData<CreateWeiXinOrderRsp>());
            }
            return false;
        }

        public void OnPurchaseCallback(PayResult payResult)
        {
            bool flag = false;
            if (Singleton<AccountManager>.Instance.accountConfig.paymentBranch == ConfigAccount.PaymentBranch.APPSTORE_CN)
            {
                MonoStoreKitEventListener.IAP_PURCHASE_CALLBACK = null;
                ApplePayResult result = (ApplePayResult) payResult;
                if (result.payRetCode == PayResult.PayRetcode.CANCELED)
                {
                    flag = true;
                }
                else if (result.payRetCode == PayResult.PayRetcode.FAILED)
                {
                    flag = true;
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTransitionFailed", new object[0]), 2f), UIType.Any);
                }
                else if (result.payRetCode == PayResult.PayRetcode.CONFIRMING)
                {
                    flag = true;
                }
                else if (result.payRetCode != PayResult.PayRetcode.SUCCESS)
                {
                    flag = true;
                }
            }
            else if ((payResult.payRetCode != PayResult.PayRetcode.SUCCESS) && (payResult.payRetCode != PayResult.PayRetcode.CONFIRMING))
            {
                if (payResult.payRetCode == PayResult.PayRetcode.CANCELED)
                {
                    flag = true;
                }
                else if (payResult.payRetCode == PayResult.PayRetcode.FAILED)
                {
                    flag = true;
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(payResult.GetResultShowText(), 2f), UIType.Any);
                }
                else
                {
                    flag = true;
                }
            }
            if (flag && (this._loadingWheelDialogContext != null))
            {
                this._loadingWheelDialogContext.Finish();
                this._loadingWheelDialogContext = null;
            }
        }

        private bool OnRechargeSuccNotify(RechargeFinishNotify rsp)
        {
            if (this._loadingWheelDialogContext != null)
            {
                this._loadingWheelDialogContext.Finish();
                this._loadingWheelDialogContext = null;
            }
            if ((rsp.get_retcode() == null) || ((rsp.get_retcode() == 2) && !string.IsNullOrEmpty(this.GetReceipt())))
            {
                RechargeDataItem storeItemByProductID = this.GetStoreItemByProductID(rsp.get_product_name());
                string str = (storeItemByProductID == null) ? string.Empty : storeItemByProductID.productName;
                int num = (int) (rsp.get_pay_hcoin() + rsp.get_free_hcoin());
                GeneralConfirmDialogContext dialogContext = new GeneralConfirmDialogContext {
                    type = GeneralConfirmDialogContext.ButtonType.SingleButton
                };
                object[] replaceParams = new object[] { str, num };
                dialogContext.desc = LocalizationGeneralLogic.GetText("IAPTransitionSuccess", replaceParams);
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = delegate (bool ok) {
                        Singleton<ShopWelfareModule>.Instance.TryHintNewWelfare();
                    };
                }
                dialogContext.buttonCallBack = <>f__am$cache8;
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                this.RemoveReceipt();
                this.StopAllShowProductsCoroutines();
                this._showAllProductsCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.ShowAllProductsAsync());
                Singleton<NetworkManager>.Instance.RequestGetVipRewardData();
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTransitionFailed", new object[0]), 2f), UIType.Any);
                this.RemoveReceipt();
            }
            return false;
        }

        public bool PreparePurchaseProduct(string productID)
        {
            if (this._loadingWheelDialogContext != null)
            {
                this._loadingWheelDialogContext.Finish();
                this._loadingWheelDialogContext = null;
            }
            if (!string.IsNullOrEmpty(this.GetReceipt()))
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTransitionContinued", new object[0]), 2f), UIType.Any);
                this.CheckReceipt();
                return false;
            }
            return true;
        }

        private void RemoveReceipt()
        {
            Singleton<MiHoYoGameData>.Instance.LocalData.Receipt = string.Empty;
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        [DebuggerHidden]
        private IEnumerator ReqStoreItemListFromMarket()
        {
            return new <ReqStoreItemListFromMarket>c__Iterator43 { <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator ReqStoreItemListFromServer()
        {
            return new <ReqStoreItemListFromServer>c__Iterator42 { <>f__this = this };
        }

        private void RetryShowAllProducts()
        {
            this.StopAllShowProductsCoroutines();
            this._showAllProductsCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.ShowAllProductsAsync());
        }

        public void SetPayMethodId(PayMethod payMethodId)
        {
            Singleton<MiHoYoGameData>.Instance.LocalData.PayMethod = (int) payMethodId;
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        [DebuggerHidden]
        public IEnumerator ShowAllProductsAsync()
        {
            return new <ShowAllProductsAsync>c__Iterator44 { <>f__this = this };
        }

        private void StopAllShowProductsCoroutines()
        {
            if (this._showAllProductsCoroutine != null)
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._showAllProductsCoroutine);
                this._showAllProductsCoroutine = null;
            }
            if (this._getProductListFromServerCoroutine != null)
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._getProductListFromServerCoroutine);
                this._getProductListFromServerCoroutine = null;
            }
            if (this._getProductListFromMarketCoroutine != null)
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._getProductListFromMarketCoroutine);
                this._getProductListFromMarketCoroutine = null;
            }
        }

        private bool VerifyInvalidToken(string token)
        {
            for (int i = 0; i < this.INVALID_TOKEN_LIST.Length; i++)
            {
                if (token == this.INVALID_TOKEN_LIST[i])
                {
                    return true;
                }
            }
            return false;
        }

        private bool VerifyReceiptLocal(string base64Receipt)
        {
            if ((base64Receipt == null) || ((base64Receipt != null) && (base64Receipt.Length < 0x400)))
            {
                return false;
            }
            if (this.VerifyInvalidToken(base64Receipt))
            {
                return false;
            }
            string receipt = SecurityUtil.Base64Decoder(base64Receipt);
            if (receipt == string.Empty)
            {
                return false;
            }
            string strToDecode = this.GetBase64StringByKey(receipt, "purchase-info");
            if (strToDecode == string.Empty)
            {
                return false;
            }
            strToDecode = SecurityUtil.Base64Decoder(strToDecode);
            if (strToDecode == string.Empty)
            {
                return false;
            }
            if (this.GetBase64StringByKey(strToDecode, "bid") != Singleton<NetworkManager>.Instance.channelConfig.BundleIdentifier)
            {
                return false;
            }
            return true;
        }

        [CompilerGenerated]
        private sealed class <ReqStoreItemListFromMarket>c__Iterator43 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal ChannelPayModule <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this._rechargeItemListFromMarket = null;
                        if (Singleton<AccountManager>.Instance.accountConfig.paymentBranch != ConfigAccount.PaymentBranch.APPSTORE_CN)
                        {
                            this.<>f__this._rechargeItemListFromMarket = new List<RechargeDataItem>();
                            break;
                        }
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_008B;
                }
                while (this.<>f__this._rechargeItemListFromMarket == null)
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                goto Label_008B;
                this.$PC = -1;
            Label_008B:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ReqStoreItemListFromServer>c__Iterator42 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal ChannelPayModule <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this._rechargeItemListFromServer = null;
                        Singleton<NetworkManager>.Instance.RequestProductList();
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_006B;
                }
                if (this.<>f__this._rechargeItemListFromServer == null)
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                goto Label_006B;
                this.$PC = -1;
            Label_006B:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ShowAllProductsAsync>c__Iterator44 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal ChannelPayModule <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (string.IsNullOrEmpty(this.<>f__this.GetReceipt()))
                        {
                            if (((this.<>f__this._showAllProductsCoroutine != null) || (this.<>f__this._getProductListFromServerCoroutine != null)) || (this.<>f__this._getProductListFromMarketCoroutine != null))
                            {
                                goto Label_021C;
                            }
                            if (this.<>f__this._loadingWheelDialogContext != null)
                            {
                                this.<>f__this._loadingWheelDialogContext.Finish();
                                this.<>f__this._loadingWheelDialogContext = null;
                            }
                            this.<>f__this._loadingWheelDialogContext = new LoadingWheelWidgetContext(0, new Action(this.<>f__this.RetryShowAllProducts));
                            this.<>f__this._loadingWheelDialogContext.SetMaxWaitTime(10f);
                            Singleton<MainUIManager>.Instance.ShowWidget(this.<>f__this._loadingWheelDialogContext, UIType.Any);
                            this.<>f__this._getProductListFromServerCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.<>f__this.ReqStoreItemListFromServer());
                            this.$current = this.<>f__this._getProductListFromServerCoroutine;
                            this.$PC = 1;
                            goto Label_021E;
                        }
                        Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTransitionContinued", new object[0]), 2f), UIType.Any);
                        this.<>f__this.CheckReceipt();
                        goto Label_021C;

                    case 1:
                        this.<>f__this._getProductListFromServerCoroutine = null;
                        if (!this.<>f__this.CheckNeedGetProductListFromMarket())
                        {
                            break;
                        }
                        this.<>f__this._getProductListFromMarketCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.<>f__this.ReqStoreItemListFromMarket());
                        this.$current = this.<>f__this._getProductListFromMarketCoroutine;
                        this.$PC = 2;
                        goto Label_021E;

                    case 2:
                        this.<>f__this._getProductListFromMarketCoroutine = null;
                        break;

                    default:
                        goto Label_021C;
                }
                this.<>f__this.MergeStoreItemList();
                this.<>f__this._showAllProductsCoroutine = null;
                if (this.<>f__this._loadingWheelDialogContext != null)
                {
                    this.<>f__this._loadingWheelDialogContext.Finish();
                    this.<>f__this._loadingWheelDialogContext = null;
                }
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.RefreshRechargeTab, null));
                this.$PC = -1;
            Label_021C:
                return false;
            Label_021E:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public enum PayMethod
        {
            ALIPAY,
            WEIXIN_PAY
        }
    }
}

