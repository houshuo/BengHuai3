namespace MoleMole
{
    using System;

    public class TheUCAccountManager : TheBaseAccountManager
    {
        private const bool DebugBuild = false;
        private const string RESPONSE_FAIL_CODE = "2";
        private const string RESPONSE_LOGIN_OUT = "3";
        private const string RESPONSE_PAY_CANCEL = "6";
        private const string RESPONSE_PAY_QUIT = "4";
        private const string RESPONSE_SUCCESS_CODE = "1";
        private const string RESPONSE_UNINIT = "5";

        public TheUCAccountManager()
        {
            base._accountDelegate = new TheUCAccountDelegate();
        }

        protected override void BindTest()
        {
        }

        public override void BindTestFinishedCallBack(string param)
        {
        }

        public override void BindUI()
        {
        }

        public override void BindUIFinishedCallBack(string arg1, string arg2)
        {
        }

        public override void DoExit()
        {
            base._accountDelegate.exit();
        }

        public override void HideToolBar()
        {
            base._accountDelegate.hideToolBar();
        }

        public override void Init()
        {
            base.AccountType = 3;
            base._accountDelegate.init(false, "AccountEventListener", "InitFinishedCallBack", null);
        }

        public override void InitFinishedCallBack(string param)
        {
        }

        protected override void LoginTest()
        {
        }

        public override void LoginTestFinishedCallBack(string param)
        {
            if (param == "1")
            {
                base.AccountUid = base._accountDelegate.getUid();
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SDKAccountLoginSuccess, null));
            }
            else
            {
                this.LoginUI();
            }
        }

        public override void LoginUI()
        {
            base._accountDelegate.login("AccountEventListener", "LoginTestFinishedCallBack", string.Empty, string.Empty, null);
        }

        public override void LoginUIFinishedCallBack(string arg1, string arg2)
        {
        }

        public override bool Pay(string productID, string productName, float productPrice)
        {
            if (base.Pay(productID, productName, productPrice))
            {
                base._accountDelegate.pay(productID, productName, productPrice, Guid.NewGuid().ToString("N"), Singleton<PlayerModule>.Instance.playerData.userId.ToString(), Singleton<NetworkManager>.Instance.DispatchSeverData.oaServerUrl + "/callback/uc", "AccountEventListener", "PayFinishedCallBack", null);
            }
            return false;
        }

        public override void PayFinishedCallBack(string param)
        {
            SDKPayResult payResult = new SDKPayResult();
            if (param == "4")
            {
                payResult.payRetCode = PayResult.PayRetcode.CANCELED;
            }
            else if (param == "2")
            {
                payResult.payRetCode = PayResult.PayRetcode.FAILED;
            }
            else
            {
                payResult.payRetCode = PayResult.PayRetcode.SUCCESS;
            }
            Singleton<ChannelPayModule>.Instance.OnPurchaseCallback(payResult);
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

        public override void ShowAccountPage()
        {
        }

        public override void ShowExitUI()
        {
            this.DoExit();
        }

        public override void ShowPausePage()
        {
            base._accountDelegate.showPausePage();
        }

        public override void ShowToolBar()
        {
            base._accountDelegate.showToolBar();
        }
    }
}

