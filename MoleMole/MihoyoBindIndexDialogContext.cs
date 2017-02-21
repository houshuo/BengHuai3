namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class MihoyoBindIndexDialogContext : BaseDialogContext
    {
        public MihoyoBindIndexDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "MihoyoBindIndexDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/MiHoYoAccount/MiHoYoBindIndexDialog"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/BindAccountBtn").GetComponent<Button>(), new UnityAction(this.OnBindAccountBtnCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/RegisterBtn").GetComponent<Button>(), new UnityAction(this.OnRegisterBtnCallBack));
        }

        public void Close()
        {
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        public void OnBindAccountBtnCallBack()
        {
            this.Close();
            Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoBindAccountDialogContext(), UIType.Any);
        }

        public void OnRegisterBtnCallBack()
        {
            this.Close();
            Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoRegisterDialogContext(), UIType.Any);
        }

        protected override bool SetupView()
        {
            return false;
        }
    }
}

