namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class RechargePageContext : BasePageContext
    {
        private ShopType _currentShopType;
        private int _playerLevelBefore;
        private TabManager _tabManager;
        public readonly string defaultTab;
        public const string RechargeTab = "RechargeTab";
        private const string TAB_SELECT_BG_PATH = "SpriteOutput/GeneralUI/TabDialogSelected";
        private const string TAB_UNSELECT_BG_PATH = "SpriteOutput/GeneralUI/TabDialogUnselected";
        public const string WelfareTab = "RewardTab";

        public RechargePageContext(string defaultTab = "RechargeTab")
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "RechargePageContext",
                viewPrefabPath = "UI/Menus/Page/Shop/RechargePagePage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            base.showSpaceShip = false;
            this.defaultTab = defaultTab;
            this._tabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
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
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_Recharge").GetComponent<Button>(), new UnityAction(this.OnRechargeTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_Reward").GetComponent<Button>(), new UnityAction(this.OnWelfareTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("CartInfoPanel/BuyBtn").GetComponent<Button>(), new UnityAction(this.OnBuyBtnClick));
            base.BindViewCallback(base.view.transform.Find("CartInfoPanel/Info/AndroidPayBtns/AilpayBtn").GetComponent<Button>(), new UnityAction(this.OnAilpayBtnClick));
            base.BindViewCallback(base.view.transform.Find("CartInfoPanel/Info/AndroidPayBtns/WechatBtn").GetComponent<Button>(), new UnityAction(this.OnWechatBtnClick));
        }

        private void DoBuyHcoin()
        {
            this.RecordPlayerLevel();
            if (this._tabManager.GetShowingTabKey() == "RechargeTab")
            {
                Transform transform = base.view.transform.Find("RechargeTab");
                if (transform != null)
                {
                    MonoShopRechargeTab component = transform.GetComponent<MonoShopRechargeTab>();
                    if (component != null)
                    {
                        component.OnPucharseProduct();
                    }
                }
            }
        }

        public void OnAilpayBtnClick()
        {
            base.view.transform.Find("RechargeTab").GetComponent<MonoShopRechargeTab>().SetPayMethodId(ChannelPayModule.PayMethod.ALIPAY);
        }

        public void OnBuyBtnClick()
        {
            if (Singleton<AccountManager>.Instance.manager.IsAccountBind() || Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
            {
                this.DoBuyHcoin();
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_Action_TouristeCharge", new object[0]),
                    okBtnText = LocalizationGeneralLogic.GetText("Menu_Action_DoBindAccount", new object[0]),
                    cancelBtnText = LocalizationGeneralLogic.GetText("Menu_Action_ContinueRecharge", new object[0]),
                    notDestroyAfterTouchBG = true,
                    buttonCallBack = delegate (bool confirmed) {
                        if (confirmed)
                        {
                            Singleton<MainUIManager>.Instance.ShowPage(new PlayerProfilePageContext(PlayerProfilePageContext.TabType.AccountTab), UIType.Page);
                        }
                        else
                        {
                            this.DoBuyHcoin();
                        }
                    }
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
        }

        private void OnEnterRechargeTab()
        {
        }

        private bool OnGetVipRewardDataRsp(GetVipRewardDataRsp rsp)
        {
            base.view.transform.Find("RewardTab").gameObject.GetComponent<MonoShopWelfareTab>().SetupView(new Action(this.RecordPlayerLevel));
            return false;
        }

        private bool OnGetVipRewardRsp(GetVipRewardRsp rsp)
        {
            if ((rsp.get_retcode() == null) && (rsp.get_reward_list().Count > 0))
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new RewardGotDialogContext(rsp.get_reward_list()[0], this._playerLevelBefore, null, "Menu_ShopWelfareRewardDlgTitle", "SpriteOutput/ShopIcons/WelfareIcon"), UIType.Any);
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
            if (ntf.type == NotifyTypes.RefreshRechargeTab)
            {
                return this.OnRefreshRechargeTab();
            }
            return ((ntf.type == NotifyTypes.SelectRechargeItem) && this.OnSelectRechargeItem((string) ntf.body));
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0xc6:
                    this.SetupWelfareHint();
                    return this.OnGetVipRewardDataRsp(pkt.getData<GetVipRewardDataRsp>());

                case 200:
                    this.SetupWelfareHint();
                    return this.OnGetVipRewardRsp(pkt.getData<GetVipRewardRsp>());

                case 0xce:
                    return this.OnManualRefreshShopRsp(pkt.getData<ManualRefreshShopRsp>());

                case 0x53:
                    return this.OnRechargeSuccNotify(pkt.getData<RechargeFinishNotify>());
            }
            return false;
        }

        private bool OnRechargeSuccNotify(RechargeFinishNotify rsp)
        {
            if (rsp.get_retcode() == null)
            {
            }
            return false;
        }

        public void OnRechargeTabBtnClick()
        {
            this._tabManager.ShowTab("RechargeTab");
            base.view.transform.Find("RechargeTab").GetComponent<MonoShopRechargeTab>().SetupView();
            this.OnEnterRechargeTab();
            base.view.transform.Find("CartInfoPanel").gameObject.SetActive(true);
            base.view.transform.Find("ChargeHCoinPanel").gameObject.SetActive(false);
            this.TrySetInfoPanel();
        }

        private bool OnRefreshRechargeTab()
        {
            base.view.transform.Find("RechargeTab").gameObject.GetComponent<MonoShopRechargeTab>().OnRefreshRechargeTab();
            return false;
        }

        private bool OnSelectRechargeItem(string productID)
        {
            Transform transform = base.view.transform.Find("RechargeTab");
            if (transform != null)
            {
                MonoShopRechargeTab component = transform.GetComponent<MonoShopRechargeTab>();
                if (component != null)
                {
                    component.OnSelectProduct(productID);
                }
            }
            return false;
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
            btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.transform.Find("Image").GetComponent<Image>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.interactable = !active;
            go.SetActive(active);
        }

        public void OnWechatBtnClick()
        {
            base.view.transform.Find("RechargeTab").GetComponent<MonoShopRechargeTab>().SetPayMethodId(ChannelPayModule.PayMethod.WEIXIN_PAY);
        }

        public void OnWelfareTabBtnClick()
        {
            this._tabManager.ShowTab("RewardTab");
            base.view.transform.Find("RewardTab").GetComponent<MonoShopWelfareTab>().SetupView(new Action(this.RecordPlayerLevel));
            base.view.transform.Find("CartInfoPanel").gameObject.SetActive(false);
            base.view.transform.Find("ChargeHCoinPanel").gameObject.SetActive(true);
            this.TrySetInfoPanel();
        }

        private void RecordPlayerLevel()
        {
            this._playerLevelBefore = Singleton<PlayerModule>.Instance.playerData.teamLevel;
        }

        private void SetupRechargeTab()
        {
            GameObject gameObject = base.view.transform.Find("RechargeTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/TabBtn_Recharge").GetComponent<Button>();
            this._tabManager.SetTab("RechargeTab", component, gameObject);
            if (this.defaultTab == "RechargeTab")
            {
                gameObject.GetComponent<MonoShopRechargeTab>().SetupView();
                this.OnEnterRechargeTab();
            }
        }

        protected override bool SetupView()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = !string.IsNullOrEmpty(showingTabKey) ? showingTabKey : this.defaultTab;
            this._tabManager.Clear();
            this.SetupRechargeTab();
            this.SetupWelfareTab();
            this._tabManager.ShowTab(searchKey);
            this.TrySetInfoPanel();
            base.view.transform.Find("ChargeHCoinPanel").gameObject.SetActive(false);
            if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
            {
                base.view.GetComponent<MonoFadeInAnimManager>().Play("tab_btns_fade_in", false, null);
            }
            return false;
        }

        private bool SetupWelfareHint()
        {
            bool flag = Singleton<ShopWelfareModule>.Instance.HasWelfareCanGet();
            base.view.transform.Find("TabBtns/TabBtn_Reward/PopUp").gameObject.SetActive(flag);
            return false;
        }

        private void SetupWelfareTab()
        {
            GameObject gameObject = base.view.transform.Find("RewardTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/TabBtn_Reward").GetComponent<Button>();
            this._tabManager.SetTab("RewardTab", component, gameObject);
            if (this.defaultTab == "RewardTab")
            {
                gameObject.GetComponent<MonoShopWelfareTab>().SetupView(new Action(this.RecordPlayerLevel));
            }
            this.SetupWelfareHint();
        }

        private void TrySetInfoPanel()
        {
            bool flag = this._tabManager.GetShowingTabKey() == "RechargeTab";
            base.view.transform.Find("CartInfoPanel").gameObject.SetActive(flag);
            base.view.transform.Find("CartInfoPanel/Info").gameObject.SetActive(false);
            base.view.transform.Find("ChargeHCoinPanel/HCoin/Num").GetComponent<Text>().text = Singleton<ShopWelfareModule>.Instance.totalPayHCoin.ToString();
        }
    }
}

