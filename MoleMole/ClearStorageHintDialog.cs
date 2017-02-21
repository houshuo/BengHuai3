namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class ClearStorageHintDialog : BaseDialogContext
    {
        private const float TIMER_SPAN = 1.5f;

        public ClearStorageHintDialog(float timerSpan = 1.5f)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ClearStorageHintDialog",
                viewPrefabPath = "UI/Menus/Dialog/ClearStorageHintDialog"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Destroy));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/ReturnBtn").GetComponent<Button>(), new UnityAction(this.Destroy));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/ClearBtn").GetComponent<Button>(), new UnityAction(this.OnClearBtnClick));
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        private void OnClearBtnClick()
        {
            this.Destroy();
            Singleton<MainUIManager>.Instance.ShowPage(new StorageShowPageContext(), UIType.Page);
        }
    }
}

