namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class StaminaExchangeDialogContext : BaseDialogContext
    {
        private string _descText;
        private Text _nextRecoverTimeText;
        private CanvasTimer _timer;

        public StaminaExchangeDialogContext(string desc)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "StaminaExchangeDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/StaminaExchangeDialog"
            };
            base.config = pattern;
            this._descText = desc;
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
            this.DestroyTimerAndSelf();
        }

        public override void Destroy()
        {
            if (this._timer != null)
            {
                this._timer.Destroy();
            }
            base.Destroy();
        }

        private void DestroyTimerAndSelf()
        {
            if (this._timer != null)
            {
                this._timer.Destroy();
            }
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.DestroyTimerAndSelf();
        }

        public void OnOKButtonCallBack()
        {
            Singleton<NetworkManager>.Instance.RequestStaminaExchange();
            this.DestroyTimerAndSelf();
        }

        private void SetupStaminaInfo()
        {
            PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
            base.view.transform.Find("Dialog/Content/StaminaInfo/NumText").GetComponent<Text>().text = playerData.stamina + "/" + playerData.MaxStamina;
            if (!Singleton<PlayerModule>.Instance.playerData.IsStaminaFull())
            {
                DateTime now = TimeUtil.Now;
                TimeSpan span = Singleton<PlayerModule>.Instance.playerData.nextStaminaRecoverDatetime.Subtract(now);
                if (span.TotalSeconds > 0.0)
                {
                    this._nextRecoverTimeText.gameObject.SetActive(true);
                    this._nextRecoverTimeText.text = string.Format("( {0:D2} : {1:D2} )", span.Minutes, span.Seconds);
                }
            }
            else
            {
                this._nextRecoverTimeText.gameObject.SetActive(false);
            }
        }

        protected override bool SetupView()
        {
            PlayerStaminaExchangeInfo info = Singleton<PlayerModule>.Instance.playerData.staminaExchangeCache.Value;
            this._nextRecoverTimeText = base.view.transform.Find("Dialog/Content/StaminaInfo/RecoverTimeText").GetComponent<Text>();
            this.SetupStaminaInfo();
            object[] replaceParams = new object[] { info.usableTimes };
            base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(this._descText, replaceParams);
            base.view.transform.Find("Dialog/Content/Exchange/HCoinNumText").GetComponent<Text>().text = info.hcoinCost.ToString();
            base.view.transform.Find("Dialog/Content/Exchange/StaminaNumText").GetComponent<Text>().text = info.staminaGet.ToString();
            return false;
        }

        public override void StartUp(Transform canvasTrans, Transform viewParent = null)
        {
            base.StartUp(canvasTrans, viewParent);
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateInfiniteTimer(1f);
            this._timer.timeTriggerCallback = new Action(this.SetupStaminaInfo);
        }
    }
}

