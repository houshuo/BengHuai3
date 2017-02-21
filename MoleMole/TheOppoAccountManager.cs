namespace MoleMole
{
    using System;

    public class TheOppoAccountManager : TheBaseAccountManager
    {
        private const bool DebugBuild = false;
        private const float PRICE_UNIT_RATE = 1f;

        public TheOppoAccountManager()
        {
            base._accountDelegate = new TheOppoAccountDelegate();
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
            base.AccountType = 8;
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
            int result = 0;
            if (int.TryParse(param, out result) && (result == 1))
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
                base._accountDelegate.pay(productID, productName, productPrice, Guid.NewGuid().ToString("N"), Singleton<PlayerModule>.Instance.playerData.userId.ToString(), Singleton<NetworkManager>.Instance.DispatchSeverData.oaServerUrl + "/callback/oppo", "AccountEventListener", "PayFinishedCallBack", null);
            }
            return false;
        }

        public override void PayFinishedCallBack(string param)
        {
            SDKPayResult payResult = new SDKPayResult();
            int result = 0;
            if (!int.TryParse(param, out result) || (result == 0))
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

