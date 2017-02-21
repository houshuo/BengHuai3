namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class GeneralHintDialogContext : BaseDialogContext
    {
        private CanvasTimer _timer;
        public string desc;
        private const float TIMER_SPAN = 2f;

        public GeneralHintDialogContext(string desc, float timerSpan = 2f)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "GeneralHintDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/GeneralHintDialog"
            };
            base.config = pattern;
            this.desc = desc;
            if (this._timer != null)
            {
                this._timer.Destroy();
            }
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(timerSpan, 0f);
            this._timer.timeUpCallback = new Action(this.OnTimerUp);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Btn").GetComponent<Button>(), new UnityAction(this.Destroy));
        }

        public override void Destroy()
        {
            this._timer.Destroy();
            base.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        private void OnTimerUp()
        {
            this.Destroy();
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = this.desc;
            return false;
        }
    }
}

