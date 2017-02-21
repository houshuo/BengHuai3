namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.EventSystems;

    public class AnnouncementDialogContext : BaseWidgetContext
    {
        private string _announcement;

        public AnnouncementDialogContext(string announcement)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AnnouncementDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AnnouncementDialog"
            };
            base.config = pattern;
            base.uiType = UIType.SuspendBar;
            this._announcement = announcement;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
        }

        private void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Announcement").GetComponent<Text>().text = this._announcement;
            return false;
        }
    }
}

