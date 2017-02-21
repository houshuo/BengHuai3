namespace MoleMole
{
    using System;
    using UnityEngine;

    public class TheBiliAccountManager : TheBaseAccountManager
    {
        private const int CANCEL_RET_CODE = 0x1771;
        private const bool DebugBuild = false;
        private const int LOGIN_ALERT_2_CANCEL_BUTTON_TEXT_ID = 0x2738;
        private const int LOGIN_ALERT_2_CONFIRM_BUTTON_TEXT_ID = 0x27fc;
        private const int LOGIN_ALERT_2_DESP_TEXT_ID = 0x27fb;
        private const int LOGIN_ALERT_2_TITLE_TEXT_ID = 0x27fa;
        private const int LOGIN_ALERT_3_CANCEL_BUTTON_TEXT_ID = 0x2738;
        private const int LOGIN_ALERT_3_CONFIRM_BUTTON_TEXT_ID = 0x27e7;
        private const int LOGIN_ALERT_3_DESP_TEXT_ID = 0x27fe;
        private const int LOGIN_ALERT_3_TITLE_TEXT_ID = 0x27fd;
        private const int LOGIN_ALERT_CANCEL_BUTTON_TEXT_ID = 0x2738;
        private const int LOGIN_ALERT_CONFIRM_BUTTON_TEXT_ID = 0x27e7;
        private const int LOGIN_ALERT_DESP_TEXT_ID = 0x27ea;
        private const int LOGIN_ALERT_TITLE_TEXT_ID = 0x27e9;
        private const int SUCCESS_RET_CODE = 0x271a;

        public TheBiliAccountManager()
        {
            base._accountDelegate = new TheBiliAccountDelegate();
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
            Application.Quit();
        }

        public override void HideToolBar()
        {
        }

        public override void Init()
        {
            base.AccountType = 2;
            base._accountDelegate.init(false, string.Empty, string.Empty, null);
        }

        public override void InitFinishedCallBack(string param)
        {
        }

        public override void LoginTestFinishedCallBack(string param)
        {
            int result = 0;
            if (int.TryParse(param, out result))
            {
                if (result == 0x271a)
                {
                    base.AccountUid = base._accountDelegate.getUid();
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SDKAccountLoginSuccess, null));
                }
                else
                {
                    this.LoginUI();
                }
            }
        }

        public override void LoginUI()
        {
            base._accountDelegate.login("AccountEventListener", "LoginTestFinishedCallBack", string.Empty, string.Empty, null);
        }

        public override bool Pay(string productID, string productName, float productPrice)
        {
            if (base.Pay(productID, productName, productPrice))
            {
                base._accountDelegate.pay(productID, productName, productPrice, Guid.NewGuid().ToString("N"), Singleton<PlayerModule>.Instance.playerData.userId.ToString(), base.ChannelRegion, "AccountEventListener", "PayFinishedCallBack", null);
            }
            return false;
        }

        public override void PayFinishedCallBack(string param)
        {
            SDKPayResult payResult = new SDKPayResult();
            int result = 0;
            if ((!int.TryParse(param, out result) || (result == 0x3e8)) || (result == 0x1771))
            {
                payResult.payRetCode = PayResult.PayRetcode.FAILED;
            }
            else
            {
                payResult.payRetCode = PayResult.PayRetcode.SUCCESS;
            }
            Singleton<ChannelPayModule>.Instance.OnPurchaseCallback(payResult);
        }

        public override void RegisterUI()
        {
            base._accountDelegate.register("AccountEventListener", "RegisterUIFinishedCallBack", string.Empty, string.Empty, string.Empty, string.Empty, null);
        }

        public override void RegisterUIFinishedCallBack(string arg1, string arg2, string arg3, string arg4)
        {
            int result = 0;
            if (int.TryParse(arg1, out result) && (result == 0x271a))
            {
                this.LoginUI();
            }
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

        public override void ShowPausePage()
        {
        }

        public override void ShowToolBar()
        {
        }
    }
}

