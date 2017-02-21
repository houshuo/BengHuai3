namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class GachaTicketLackDialogContext : BaseSequenceDialogContext
    {
        private int _currentTicketNum;
        private bool _hcoinEnough;
        private int _lackTicketNum;
        private int _ticketID;
        private int _ticketPrice;
        private int _wantedNum;

        public GachaTicketLackDialogContext(int ticketID, int totalNum)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "GachaTicketLackDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/GachaTicketLackDialog"
            };
            base.config = pattern;
            this._ticketID = ticketID;
            this._wantedNum = totalNum;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), new UnityAction(this.OnCancelButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), new UnityAction(this.OnOKButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Destroy));
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        public bool OnBuyGachaTicketRsp(BuyGachaTicketRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(this._ticketID, 1);
                object[] replaceParams = new object[] { this._lackTicketNum, dummyStorageDataItem.GetDisplayTitle() };
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_BuyGachaTicketSuccess", replaceParams), 2f), UIType.Any);
            }
            this.Destroy();
            return false;
        }

        public void OnCancelButtonCallBack()
        {
            this.Destroy();
        }

        public void OnOKButtonCallBack()
        {
            if (this._hcoinEnough)
            {
                Singleton<NetworkManager>.Instance.RequestBuyGachaTicket(this._ticketID, this._lackTicketNum);
                Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(0xd8, null), UIType.Any);
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext("RechargeTab"), UIType.Page);
                this.Destroy();
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            if (pkt.getCmdId() == 0xd8)
            {
                this.OnBuyGachaTicketRsp(pkt.getData<BuyGachaTicketRsp>());
            }
            return false;
        }

        protected override bool SetupView()
        {
            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(this._ticketID, 1);
            StorageDataItemBase base3 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(this._ticketID);
            this._currentTicketNum = (base3 != null) ? base3.number : 0;
            this._ticketPrice = Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict[this._ticketID];
            this._lackTicketNum = this._wantedNum - this._currentTicketNum;
            this._hcoinEnough = Singleton<PlayerModule>.Instance.playerData.hcoin >= (this._lackTicketNum * this._ticketPrice);
            string str = !this._hcoinEnough ? LocalizationGeneralLogic.GetText("Menu_GoToRecharge", new object[0]) : LocalizationGeneralLogic.GetText("Menu_Buy", new object[0]);
            string str2 = !this._hcoinEnough ? LocalizationGeneralLogic.GetText("Menu_GoToRechargeDesc", new object[0]) : LocalizationGeneralLogic.GetText("Menu_Desc_GachaTicketLack", new object[] { dummyStorageDataItem.GetDisplayTitle(), this._lackTicketNum * this._ticketPrice, this._lackTicketNum, dummyStorageDataItem.GetDisplayTitle() });
            base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn/Text").GetComponent<Text>().text = str;
            base.view.transform.Find("Dialog/Content/Desc/DescText").GetComponent<Text>().text = str2;
            string iconPath = dummyStorageDataItem.GetIconPath();
            base.view.transform.Find("Dialog/Content/TicketIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(iconPath);
            base.view.transform.Find("Dialog/Content/CurretnTickets/Content/TicketLabel").GetComponent<Text>().text = dummyStorageDataItem.GetDisplayTitle();
            base.view.transform.Find("Dialog/Content/CurretnTickets/Content/Num").GetComponent<Text>().text = this._currentTicketNum.ToString();
            object[] replaceParams = new object[] { dummyStorageDataItem.GetDisplayTitle() };
            base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_GachaTitcketLack", replaceParams);
            return false;
        }
    }
}

