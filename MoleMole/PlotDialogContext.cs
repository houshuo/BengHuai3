namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class PlotDialogContext : BaseDialogContext
    {
        public PlotDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "PlotDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/PlotDialog"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("SkipButton").GetComponent<Button>(), new UnityAction(this.Destroy));
            base.BindViewCallback(base.view.transform.Find("Button").GetComponent<Button>(), new UnityAction(this.ButtonClickTest));
        }

        public void ButtonClickTest()
        {
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        protected override bool SetupView()
        {
            base.view.transform.gameObject.SetActive(true);
            return false;
        }
    }
}

