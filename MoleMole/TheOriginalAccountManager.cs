namespace MoleMole
{
    using SimpleJSON;
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;

    public class TheOriginalAccountManager : TheBaseAccountManager
    {
        private string ORIGINAL_ACCOUNT_SEVER;
        public string ORIGINAL_BIND_EMAIL_URL;
        public string ORIGINAL_BIND_IDENTITY_URL;
        public string ORIGINAL_BIND_MOBILE_URL;
        public string ORIGINAL_CHANGE_PASSWORD_URL;
        public string ORIGINAL_EMAIL_PASSWORD_URL;
        public string ORIGINAL_EMAIL_REGISTER_URL;
        private string ORIGINAL_GET_USER_INFO_BY_TOKEN_URL;
        private string ORIGINAL_LOGIN_URL;
        public string ORIGINAL_MOBILE_PASSWORD_URL;
        public string ORIGINAL_MOBILE_REGISTER_URL;
        private string ORIGINAL_TOKEN_LOGIN_URL;
        private string ORIGINAL_VERIFY_EMAIL_APPLY_URL;

        public TheOriginalAccountManager()
        {
            base.AccountType = 1;
            base._accountDelegate = new TheOriginalAccountDelegate();
        }

        protected override void BindTest()
        {
            string url = string.Format(this.ORIGINAL_LOGIN_URL + "?account={0}&password={1}", WWW.EscapeURL(base._accountArg1), WWW.EscapeURL(base._accountArg2));
            Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(url, new Action<string>(this.BindTestFinishedCallBack), () => this.WWWTimeOut(new Action(this.BindTest)), 5f, 3, null, null));
        }

        public override void BindTestFinishedCallBack(string param)
        {
            JSONNode node = JSON.Parse(param);
            if (node["retcode"].AsInt >= 0)
            {
                base.AccountUid = (string) node["data"]["uid"];
                base.AccountToken = (string) node["data"]["token"];
                this.Name = (string) node["data"]["name"];
                this.Email = (string) node["data"]["email"];
                this.Mobile = (string) node["data"]["mobile"];
                this.IsEmailVerify = node["data"]["is_email_verify"].AsInt > 0;
                this.IsRealNameVerify = !string.IsNullOrEmpty(node["data"]["realname"].Value) && !string.IsNullOrEmpty(node["data"]["identity_card"].Value);
                Singleton<NetworkManager>.Instance.RequestBindAccount();
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext((string) node["msg"], 2f), UIType.Any);
            }
        }

        public override void BindUI()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoBindIndexDialogContext(), UIType.Any);
        }

        public override void BindUIFinishedCallBack(string arg1, string arg2)
        {
            base._accountArg1 = arg1;
            base._accountArg2 = arg2;
            this.BindTest();
        }

        public override void DoExit()
        {
            Application.Quit();
        }

        public override string GetAccountName()
        {
            if (!string.IsNullOrEmpty(this.Name))
            {
                return this.Name;
            }
            if (!string.IsNullOrEmpty(this.Email))
            {
                return this.Email;
            }
            if (!string.IsNullOrEmpty(this.Mobile))
            {
                return this.Mobile;
            }
            return string.Empty;
        }

        private string GetHiddenEmail()
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                return string.Empty;
            }
            string email = this.Email;
            int length = 2;
            char[] separator = new char[] { "@"[0] };
            string[] strArray = email.Split(separator);
            if ((strArray == null) || (strArray.Length != 2))
            {
                return email;
            }
            if (strArray[0].Length >= length)
            {
                return (strArray[0].Substring(0, length) + "***@" + strArray[1]);
            }
            return (strArray[0] + "***" + strArray[1]);
        }

        private string GetHiddenMobile()
        {
            if (string.IsNullOrEmpty(this.Mobile))
            {
                return string.Empty;
            }
            string mobile = this.Mobile;
            if (mobile.Length <= 4)
            {
                return mobile;
            }
            StringBuilder builder = new StringBuilder(this.Mobile);
            for (int i = 3; i < (mobile.Length - 1); i++)
            {
                builder[i] = "*"[0];
            }
            return builder.ToString();
        }

        private string GetHiddenName()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return string.Empty;
            }
            string name = this.Name;
            int length = 2;
            if (name.Length >= length)
            {
                return (name.Substring(0, length) + "***");
            }
            return (name + "***");
        }

        public void GetUserInfoByToken()
        {
            string url = string.Format(this.ORIGINAL_GET_USER_INFO_BY_TOKEN_URL + "?uid={0}&token={1}", base.AccountUid, base.AccountToken);
            Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(url, new Action<string>(this.GetUserInfoByTokenFinishedCallBack), () => this.WWWTimeOut(new Action(this.GetUserInfoByToken)), 5f, 3, null, null));
        }

        private void GetUserInfoByTokenFinishedCallBack(string param)
        {
            JSONNode node = JSON.Parse(param);
            if (node["retcode"].AsInt >= 0)
            {
                this.Name = (string) node["data"]["name"];
                this.Email = (string) node["data"]["email"];
                this.Mobile = (string) node["data"]["mobile"];
                this.IsEmailVerify = node["data"]["is_email_verify"].AsInt > 0;
                this.IsRealNameVerify = !string.IsNullOrEmpty(node["data"]["realname"].Value) && !string.IsNullOrEmpty(node["data"]["identity_card"].Value);
                this.SaveAccountToLocal();
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountInfoChanged, null));
            }
        }

        public override void Init()
        {
        }

        public override void InitFinishedCallBack(string param)
        {
        }

        private void LoginAlertCallBack()
        {
        }

        private void LoginAlertCallBackDoubleAlert()
        {
        }

        protected override void LoginTest()
        {
            string url = string.Format(this.ORIGINAL_LOGIN_URL + "?account={0}&password={1}", WWW.EscapeURL(base._accountArg1), WWW.EscapeURL(base._accountArg2));
            Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(url, new Action<string>(this.LoginTestFinishedCallBack), () => this.WWWTimeOut(new Action(this.LoginTest)), 5f, 3, null, null));
        }

        public override void LoginTestFinishedCallBack(string param)
        {
            JSONNode node = JSON.Parse(param);
            if (node["retcode"].AsInt >= 0)
            {
                base.AccountUid = (string) node["data"]["uid"];
                base.AccountToken = (string) node["data"]["token"];
                this.Name = (string) node["data"]["name"];
                this.Email = (string) node["data"]["email"];
                this.Mobile = (string) node["data"]["mobile"];
                this.IsEmailVerify = node["data"]["is_email_verify"].AsInt > 0;
                this.IsRealNameVerify = !string.IsNullOrEmpty(node["data"]["realname"].Value) && !string.IsNullOrEmpty(node["data"]["identity_card"].Value);
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountLoginSuccess, null));
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext((string) node["msg"], 2f), UIType.Any);
            }
        }

        public override void LoginUI()
        {
            if (!string.IsNullOrEmpty(Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.uid))
            {
                this.TokenLoginTest();
            }
            else
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowMihoyoLoginUI, null));
            }
        }

        public override void LoginUIFinishedCallBack(string arg1, string arg2)
        {
            base._accountArg1 = arg1;
            base._accountArg2 = arg2;
            this.LoginTest();
        }

        public override bool Pay(string productID, string productName, float productPrice)
        {
            if (!base.Pay(productID, productName, productPrice))
            {
                return false;
            }
            switch (Singleton<AccountManager>.Instance.accountConfig.paymentBranch)
            {
                case ConfigAccount.PaymentBranch.APPSTORE_CN:
                    base._accountDelegate.pay(productID, productName, productPrice, Guid.NewGuid().ToString("N"), Singleton<PlayerModule>.Instance.playerData.userId.ToString(), string.Empty, string.Empty, string.Empty, null);
                    break;

                case ConfigAccount.PaymentBranch.ORIGINAL_ANDROID_PAY:
                    switch (Singleton<ChannelPayModule>.Instance.GetPayMethodId())
                    {
                        case ChannelPayModule.PayMethod.ALIPAY:
                            base._accountDelegate.pay(productID, productName, productPrice, Guid.NewGuid().ToString("N"), Singleton<PlayerModule>.Instance.playerData.userId.ToString(), Singleton<NetworkManager>.Instance.DispatchSeverData.oaServerUrl + "/callback/alipay", string.Empty, string.Empty, null);
                            break;

                        case ChannelPayModule.PayMethod.WEIXIN_PAY:
                        {
                            TheOriginalAccountDelegate.WeixinPrepayOrderInfo info = new TheOriginalAccountDelegate.WeixinPrepayOrderInfo {
                                productID = productID,
                                productName = productName,
                                productPrice = productPrice
                            };
                            (base._accountDelegate as TheOriginalAccountDelegate).weixinPrepayOrderInfo = info;
                            Singleton<NetworkManager>.Instance.RequestCreateWeiXinOrder(productID, productName, productPrice);
                            break;
                        }
                    }
                    break;
            }
            return true;
        }

        public bool PayByWeiXinOrder(string partnerID, string prepayID, string nonceStr, string timestamp, string sign)
        {
            TheOriginalAccountDelegate.WeixinPrepayOrderInfo weixinPrepayOrderInfo = (base._accountDelegate as TheOriginalAccountDelegate).weixinPrepayOrderInfo;
            weixinPrepayOrderInfo.partnerID = partnerID;
            weixinPrepayOrderInfo.prepayID = prepayID;
            weixinPrepayOrderInfo.partnerID = partnerID;
            weixinPrepayOrderInfo.nonceStr = nonceStr;
            weixinPrepayOrderInfo.timestamp = timestamp;
            weixinPrepayOrderInfo.sign = sign;
            base._accountDelegate.pay(weixinPrepayOrderInfo.productID, weixinPrepayOrderInfo.productName, weixinPrepayOrderInfo.productPrice, string.Empty, Singleton<PlayerModule>.Instance.playerData.userId.ToString(), Singleton<NetworkManager>.Instance.DispatchSeverData.oaServerUrl + "/callback/weixin", string.Empty, string.Empty, null);
            return true;
        }

        public override void PayFinishedCallBack(string param)
        {
            if (Singleton<AccountManager>.Instance.accountConfig.paymentBranch == ConfigAccount.PaymentBranch.ORIGINAL_ANDROID_PAY)
            {
                PayResult payResult = null;
                switch (Singleton<ChannelPayModule>.Instance.GetPayMethodId())
                {
                    case ChannelPayModule.PayMethod.ALIPAY:
                        payResult = new AliPayResult(param);
                        break;

                    case ChannelPayModule.PayMethod.WEIXIN_PAY:
                        payResult = new WeixinPayResult(param);
                        break;
                }
                if (payResult != null)
                {
                    Singleton<ChannelPayModule>.Instance.OnPurchaseCallback(payResult);
                }
            }
        }

        protected override void RegisterTest()
        {
        }

        public override void RegisterTestFinishedCallBack(string param)
        {
        }

        public override void RegisterUI()
        {
        }

        public override void RegisterUIFinishedCallBack(string arg1, string arg2, string arg3, string arg4)
        {
        }

        public override void Reset()
        {
            this.Name = string.Empty;
            this.Email = string.Empty;
            this.Mobile = string.Empty;
            this.IsEmailVerify = false;
            this.IsRealNameVerify = false;
            base.Reset();
        }

        public override void SaveAccountToLocal()
        {
            GeneralLocalDataItem.AccountData data = new GeneralLocalDataItem.AccountData {
                uid = base.AccountUid,
                token = base.AccountToken,
                name = this.Name,
                email = this.Email,
                mobile = this.Mobile,
                isEmailVerify = this.IsEmailVerify,
                isRealNameVerify = this.IsRealNameVerify
            };
            Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account = data;
            Singleton<MiHoYoGameData>.Instance.GeneralLocalData.LastLoginAccountName = data.GetAccountName();
            Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
        }

        public override void SetupByDispatchServerData()
        {
            this.ORIGINAL_ACCOUNT_SEVER = Singleton<NetworkManager>.Instance.DispatchSeverData.accountUrl;
            this.ORIGINAL_LOGIN_URL = this.ORIGINAL_ACCOUNT_SEVER + "/sdk/login";
            this.ORIGINAL_TOKEN_LOGIN_URL = this.ORIGINAL_ACCOUNT_SEVER + "/sdk/token_login";
            this.ORIGINAL_GET_USER_INFO_BY_TOKEN_URL = this.ORIGINAL_ACCOUNT_SEVER + "/sdk/get_user_info_by_token";
            this.ORIGINAL_VERIFY_EMAIL_APPLY_URL = this.ORIGINAL_ACCOUNT_SEVER + "/sdk/verify_email_apply";
            this.ORIGINAL_MOBILE_REGISTER_URL = this.ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/mobile_register";
            this.ORIGINAL_EMAIL_REGISTER_URL = this.ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/email_register";
            this.ORIGINAL_MOBILE_PASSWORD_URL = this.ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/change_password_by_mobile";
            this.ORIGINAL_EMAIL_PASSWORD_URL = this.ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/change_password_by_email_apply";
            this.ORIGINAL_CHANGE_PASSWORD_URL = this.ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/change_password";
            this.ORIGINAL_BIND_EMAIL_URL = this.ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/bind_email_apply";
            this.ORIGINAL_BIND_MOBILE_URL = this.ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/bind_mobile";
            this.ORIGINAL_BIND_IDENTITY_URL = this.ORIGINAL_ACCOUNT_SEVER + "/bh3rd_webview/bind_realname";
        }

        public override void ShowAccountPage()
        {
        }

        public override void ShowExitUI()
        {
            if (Singleton<MainUIManager>.Instance.SceneCanvas != null)
            {
                Singleton<MainUIManager>.Instance.SceneCanvas.ShowQuitGameDialog();
            }
        }

        public void TokenLoginTest()
        {
            GeneralLocalDataItem generalLocalData = Singleton<MiHoYoGameData>.Instance.GeneralLocalData;
            string url = string.Format(this.ORIGINAL_TOKEN_LOGIN_URL + "?uid={0}&token={1}", generalLocalData.Account.uid, generalLocalData.Account.token);
            Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(url, new Action<string>(this.TokenLoginTestFinishedCallBack), () => this.WWWTimeOut(new Action(this.TokenLoginTest)), 5f, 3, null, null));
        }

        public void TokenLoginTestFinishedCallBack(string param)
        {
            JSONNode node = JSON.Parse(param);
            GeneralLocalDataItem generalLocalData = Singleton<MiHoYoGameData>.Instance.GeneralLocalData;
            if (node["retcode"].AsInt >= 0)
            {
                base.AccountUid = generalLocalData.Account.uid;
                base.AccountToken = generalLocalData.Account.token;
                base.AccountExt = generalLocalData.Account.ext;
                this.Name = generalLocalData.Account.name;
                this.Email = generalLocalData.Account.email;
                this.Mobile = generalLocalData.Account.mobile;
                this.IsEmailVerify = generalLocalData.Account.isEmailVerify;
                this.IsRealNameVerify = generalLocalData.Account.isRealNameVerify;
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountLoginSuccess, null));
                if (base.IsAccountBind() && (!this.IsEmailVerify || !this.IsRealNameVerify))
                {
                    this.GetUserInfoByToken();
                }
            }
            else
            {
                generalLocalData.ClearLastLoginUser();
                Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_MihoyoAccountTokenError", new object[0]), 2f), UIType.Any);
                this.LoginUI();
            }
        }

        public override void VerifyEmailApply()
        {
            string url = string.Format(this.ORIGINAL_VERIFY_EMAIL_APPLY_URL + "?uid={0}&token={1}", base.AccountUid, base.AccountToken);
            Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(url, new Action<string>(this.VerifyEmailApplyFinishedCallBack), () => this.WWWTimeOut(new Action(this.VerifyEmailApply)), 5f, 3, null, null));
        }

        private void VerifyEmailApplyFinishedCallBack(string param)
        {
            JSONNode node = JSON.Parse(param);
            if (node["retcode"].AsInt >= 0)
            {
                Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasSendVerifyEmailApply = true;
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext((string) node["msg"], 2f), UIType.Any);
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountInfoChanged, null));
                this.GetUserInfoByToken();
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext((string) node["msg"], 2f), UIType.Any);
            }
        }

        private void WWWTimeOut(Action OkBtnCallback)
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(LocalizationGeneralLogic.GetText("Menu_MihoyoAccountTimeOut", new object[0]), null, OkBtnCallback, LocalizationGeneralLogic.GetText("Menu_Tips", new object[0])), UIType.Any);
        }

        public string Email { get; private set; }

        public bool IsEmailVerify { get; private set; }

        public bool IsRealNameVerify { get; private set; }

        public string Mobile { get; private set; }

        public string Name { get; private set; }
    }
}

