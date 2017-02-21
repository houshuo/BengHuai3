namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class ResetTechTreeDialogContext : BaseSequenceDialogContext
    {
        private CabinDataItemBase _cabinData;
        private int _scoin_need;

        public ResetTechTreeDialogContext(CabinDataItemBase data)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ResetTechTreeDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/ResetTechTreeDialog"
            };
            base.config = pattern;
            this._cabinData = data;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ResetBtn").GetComponent<Button>(), new UnityAction(this.OnReset));
        }

        private void Close()
        {
            this.Destroy();
        }

        private void InitView()
        {
            base.view.transform.Find("Dialog/Content/Error").gameObject.SetActive(false);
        }

        private void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        private void OnReset()
        {
            if (this._scoin_need > 0)
            {
                Singleton<NetworkManager>.Instance.RequestResetCabinTech(this._cabinData.cabinType);
            }
            this.Close();
        }

        protected override bool SetupView()
        {
            this._scoin_need = this._cabinData.GetResetScoin();
            this.InitView();
            base.view.transform.Find("Dialog/Content/Info/Scoin").GetComponent<Text>().text = this._scoin_need.ToString();
            if (Singleton<PlayerModule>.Instance.playerData.scoin >= this._scoin_need)
            {
                base.view.transform.Find("Dialog/Content/ResetBtn").GetComponent<Button>().interactable = true;
            }
            else
            {
                base.view.transform.Find("Dialog/Content/ResetBtn").GetComponent<Button>().interactable = false;
                base.view.transform.Find("Dialog/Content/Error").gameObject.SetActive(true);
            }
            return false;
        }
    }
}

