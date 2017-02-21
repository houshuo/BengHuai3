namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class SkillPointExchangeDialogContext : BaseDialogContext
    {
        private Text _nextRecoverTimeText;
        private CanvasTimer _timer;

        public SkillPointExchangeDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "StaminaExchangeDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/SkillPointExchangeDialog"
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
            Singleton<NetworkManager>.Instance.RequestSkillPointExchange();
            this.DestroyTimerAndSelf();
        }

        private void SetupSkillPtInfo()
        {
            PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
            base.view.transform.Find("Dialog/Content/SkillPtInfo/NumText").GetComponent<Text>().text = playerData.skillPoint + "/" + playerData.skillPointLimit;
            if (!Singleton<PlayerModule>.Instance.playerData.IsSkillPointFull())
            {
                DateTime now = TimeUtil.Now;
                TimeSpan span = Singleton<PlayerModule>.Instance.playerData.nextSkillPtRecoverDatetime.Subtract(now);
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
            PlayerSkillPointExchangeInfo info = Singleton<PlayerModule>.Instance.playerData.skillPointExchangeCache.Value;
            this._nextRecoverTimeText = base.view.transform.Find("Dialog/Content/SkillPtInfo/RecoverTimeText").GetComponent<Text>();
            this.SetupSkillPtInfo();
            object[] replaceParams = new object[] { info.usableTimes };
            base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_SkillPtExchange", replaceParams);
            base.view.transform.Find("Dialog/Content/Exchange/HCoinNumText").GetComponent<Text>().text = info.hcoinCost.ToString();
            base.view.transform.Find("Dialog/Content/Exchange/SkillPtNumText").GetComponent<Text>().text = info.skillPointGet.ToString();
            return false;
        }

        public override void StartUp(Transform canvasTrans, Transform viewParent = null)
        {
            base.StartUp(canvasTrans, viewParent);
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateInfiniteTimer(1f);
            this._timer.timeTriggerCallback = new Action(this.SetupSkillPtInfo);
        }
    }
}

