namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine.Events;

    public class SupplyEntrancePageContext : BasePageContext
    {
        public SupplyEntrancePageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "SupplyEntrancePageContext",
                viewPrefabPath = "UI/Menus/Page/SupplyEntrancePage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Btn/BtnSupply/Button").GetComponent<Button>(), new UnityAction(this.OnGachaButtonClick));
            base.BindViewCallback(base.view.transform.Find("Btn/BtnShop/Button").GetComponent<Button>(), new UnityAction(this.OnShopButtonClick));
            base.BindViewCallback(base.view.transform.Find("Btn/BtnRecharge/Button").GetComponent<Button>(), new UnityAction(this.OnRechargeButtonClick));
        }

        private void OnGachaButtonClick()
        {
            Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
            Singleton<MainUIManager>.Instance.ShowPage(new GachaMainPageContext(), UIType.Page);
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0xc6) && this.SetupWelfareHint());
        }

        private void OnRechargeButtonClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext("RechargeTab"), UIType.Page);
        }

        private void OnShopButtonClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new ShopPageContext(UIShopType.SHOP_NORMAL), UIType.Page);
        }

        private void SetupGacha()
        {
            int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
            int gachaUnlockNeedPlayerLevel = MiscData.Config.BasicConfig.GachaUnlockNeedPlayerLevel;
            bool flag = teamLevel < gachaUnlockNeedPlayerLevel;
            base.view.transform.Find("Btn/BtnSupply").gameObject.SetActive(!flag);
            base.view.transform.Find("Btn/BtnSupply/Locked").gameObject.SetActive(false);
        }

        protected override bool SetupView()
        {
            this.SetupGacha();
            this.SetupWelfareHint();
            return false;
        }

        private bool SetupWelfareHint()
        {
            bool flag = Singleton<ShopWelfareModule>.Instance.HasWelfareCanGet();
            base.view.transform.Find("Btn/BtnRecharge/PopUp").gameObject.SetActive(flag);
            return false;
        }
    }
}

