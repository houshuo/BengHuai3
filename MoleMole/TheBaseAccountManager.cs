namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public abstract class TheBaseAccountManager
    {
        protected string _accountArg1 = string.Empty;
        protected string _accountArg2 = string.Empty;
        protected TheBaseAccountDelegate _accountDelegate;
        public const string ACCOUNT_CALLBACK_LISTENER = "AccountEventListener";

        protected TheBaseAccountManager()
        {
        }

        protected void BindAccount()
        {
        }

        protected virtual void BindTest()
        {
        }

        public virtual void BindTestFinishedCallBack(string param)
        {
        }

        public virtual void BindUI()
        {
        }

        public virtual void BindUIFinishedCallBack(string arg1, string arg2)
        {
        }

        public virtual bool DestroyCurrentBeforeShowStorePage()
        {
            return true;
        }

        public virtual void DoExit()
        {
        }

        public virtual string GetAccountName()
        {
            return string.Empty;
        }

        public List<RechargeDataItem> GetRechargeItemList()
        {
            return Singleton<ChannelPayModule>.Instance.GetRechargeItemList();
        }

        public RechargeDataItem GetStoreItemByProductID(string productID)
        {
            return Singleton<ChannelPayModule>.Instance.GetStoreItemByProductID(productID);
        }

        public virtual void HideToolBar()
        {
        }

        public abstract void Init();
        public virtual void InitFinishedCallBack(string param)
        {
        }

        public bool IsAccountBind()
        {
            return !string.IsNullOrEmpty(this.AccountUid);
        }

        public bool IsForceLoginType()
        {
            return false;
        }

        protected void LoginAccount()
        {
        }

        protected virtual void LoginTest()
        {
        }

        public virtual void LoginTestFinishedCallBack(string param)
        {
        }

        public abstract void LoginUI();
        public virtual void LoginUIFinishedCallBack(string arg1, string arg2)
        {
        }

        public virtual void OnLogin()
        {
        }

        public virtual void OnRegister()
        {
        }

        public virtual bool Pay(string productID, string productName, float productPrice)
        {
            if (!this.GetStoreItemByProductID(productID).CanPurchase())
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("IAPTPurchaseLimit", new object[0]), 2f), UIType.Any);
                return false;
            }
            if (!Singleton<ChannelPayModule>.Instance.PreparePurchaseProduct(productID))
            {
                return false;
            }
            return true;
        }

        public virtual void PayFinishedCallBack(string param)
        {
        }

        protected virtual void RegisterTest()
        {
        }

        public virtual void RegisterTestFinishedCallBack(string param)
        {
        }

        public virtual void RegisterUI()
        {
        }

        public virtual void RegisterUIFinishedCallBack(string arg1, string arg2, string arg3, string arg4)
        {
        }

        public virtual void Reset()
        {
            this._accountArg1 = string.Empty;
            this._accountArg2 = string.Empty;
            this.AccountUid = string.Empty;
            this.AccountToken = string.Empty;
            this.AccountExt = string.Empty;
        }

        public virtual void SaveAccountToLocal()
        {
        }

        public virtual void SetupByDispatchServerData()
        {
        }

        public virtual void ShowAccountPage()
        {
        }

        public void ShowAllProducts()
        {
            Singleton<ApplicationManager>.Instance.StartCoroutine(Singleton<ChannelPayModule>.Instance.ShowAllProductsAsync());
        }

        public virtual void ShowExitUI()
        {
        }

        public virtual void ShowPausePage()
        {
        }

        public virtual void ShowStorePage(int scanior)
        {
        }

        public virtual void ShowToolBar()
        {
        }

        public virtual void SwitchAccountFinishedCallBack(string param)
        {
        }

        public virtual void VerifyEmailApply()
        {
        }

        public string AccountExt { get; protected set; }

        public string AccountToken { get; protected set; }

        public proto.AccountType AccountType { get; protected set; }

        public string AccountUid { get; protected set; }

        public string ChannelRegion { get; set; }
    }
}

