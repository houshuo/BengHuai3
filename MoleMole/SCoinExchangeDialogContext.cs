namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class SCoinExchangeDialogContext : BaseDialogContext
    {
        public SCoinExchangeDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "SCoinExchangeDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/ScoinExchangeDialog"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), new UnityAction(this.OnOKButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        public void OnOKButtonCallBack()
        {
            Singleton<NetworkManager>.Instance.RequestScoinExchange();
            this.Destroy();
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 13) && this.SetupView());
        }

        protected override bool SetupView()
        {
            PlayerScoinExchangeInfo info = Singleton<PlayerModule>.Instance.playerData.scoinExchangeCache.Value;
            if (info != null)
            {
                base.view.transform.Find("Dialog/Content/Times/NumText").GetComponent<Text>().text = string.Format("({0}/{1})", info.usableTimes, info.totalTimes);
                base.view.transform.Find("Dialog/Content/Exchange/HCoinNumText").GetComponent<Text>().text = info.hcoinCost.ToString();
                base.view.transform.Find("Dialog/Content/Exchange/SCoinNumText").GetComponent<Text>().text = info.scoinGet.ToString();
            }
            return false;
        }
    }
}

