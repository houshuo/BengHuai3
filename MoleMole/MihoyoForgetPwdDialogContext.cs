namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class MihoyoForgetPwdDialogContext : BaseDialogContext
    {
        public MihoyoForgetPwdDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "MihoyoForgetPwdDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/MiHoYoAccount/MiHoYoForgetPswDialog"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/UsePhoneBtn").GetComponent<Button>(), new UnityAction(this.OnUsePhoneBtnCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/UseEmailBtn").GetComponent<Button>(), new UnityAction(this.OnUseEmailBtnCallBack));
        }

        public void Close()
        {
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        public void OnUseEmailBtnCallBack()
        {
            TheOriginalAccountManager manager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
            if (manager != null)
            {
                WebViewGeneralLogic.LoadUrl(manager.ORIGINAL_EMAIL_PASSWORD_URL, false);
            }
        }

        public void OnUsePhoneBtnCallBack()
        {
            TheOriginalAccountManager manager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
            if (manager != null)
            {
                WebViewGeneralLogic.LoadUrl(manager.ORIGINAL_MOBILE_PASSWORD_URL, false);
            }
        }

        protected override bool SetupView()
        {
            return false;
        }
    }
}

