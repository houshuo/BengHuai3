namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using UnityEngine.Events;

    public class InLevelRechargeDialogContext : BaseDialogContext
    {
        private MonoShopRechargeTab _rechargeTab;
        private InLevelReviveDialogContext _reviveContext;

        public InLevelRechargeDialogContext(InLevelReviveDialogContext reviveContext)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "InLevelRechargeDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/InLevelShopDialog"
            };
            base.config = pattern;
            this._reviveContext = reviveContext;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnCancelBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/ActionBtns/CancelBtn").GetComponent<Button>(), new UnityAction(this.OnCancelBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CartInfoPanel/BuyBtn").GetComponent<Button>(), new UnityAction(this.OnBuyBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CartInfoPanel/Info/AndroidPayBtns/AilpayBtn").GetComponent<Button>(), new UnityAction(this.OnAilpayBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CartInfoPanel/Info/AndroidPayBtns/WechatBtn").GetComponent<Button>(), new UnityAction(this.OnWechatBtnClick));
        }

        public void OnAilpayBtnClick()
        {
            this._rechargeTab.SetPayMethodId(ChannelPayModule.PayMethod.ALIPAY);
        }

        private void OnBuyBtnClick()
        {
            this._rechargeTab.OnPucharseProduct();
        }

        private void OnCancelBtnClick()
        {
            this._reviveContext.SetActive(true);
            this.Destroy();
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.RefreshRechargeTab)
            {
                return this.OnRefreshRechargeTab();
            }
            return ((ntf.type == NotifyTypes.SelectRechargeItem) && this.OnSelectProduct((string) ntf.body));
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x53) && this.OnRechargeSuccNotify(pkt.getData<RechargeFinishNotify>()));
        }

        private bool OnRechargeSuccNotify(RechargeFinishNotify rsp)
        {
            if ((rsp.get_retcode() == null) || (rsp.get_retcode() == 2))
            {
                this._reviveContext.SetActive(true);
                this._reviveContext.RefreshView();
                this.Destroy();
            }
            return false;
        }

        public bool OnRefreshRechargeTab()
        {
            this._rechargeTab.OnRefreshRechargeTab();
            return false;
        }

        public bool OnSelectProduct(string productID)
        {
            this._rechargeTab.OnSelectProduct(productID);
            return false;
        }

        public void OnWechatBtnClick()
        {
            this._rechargeTab.SetPayMethodId(ChannelPayModule.PayMethod.WEIXIN_PAY);
        }

        protected override bool SetupView()
        {
            this._reviveContext.SetActive(false);
            this._rechargeTab = base.view.transform.Find("Dialog/RechargeTab").GetComponent<MonoShopRechargeTab>();
            this._rechargeTab.SetupView();
            base.view.transform.Find("Dialog/CartInfoPanel/Info").gameObject.SetActive(false);
            return false;
        }
    }
}

