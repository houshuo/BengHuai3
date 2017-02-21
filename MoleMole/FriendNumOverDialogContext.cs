namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class FriendNumOverDialogContext : BaseSequenceDialogContext
    {
        public FriendNumOverDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "FriendNumOverDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/FriendNumOverDialog"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), new UnityAction(this.OnGoBtnClick));
        }

        private void Close()
        {
            this.Destroy();
        }

        private void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        private void OnGoBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new FriendOverviewPageContext("FriendListTab", InviteTab.InviteeTab), UIType.Page);
            this.Destroy();
        }

        protected override bool SetupView()
        {
            return false;
        }
    }
}

