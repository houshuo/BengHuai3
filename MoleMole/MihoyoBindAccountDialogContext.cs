namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class MihoyoBindAccountDialogContext : BaseDialogContext
    {
        private const int MAX_LENGTH = 50;

        public MihoyoBindAccountDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "MihoyoBindAccountDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/MiHoYoAccount/MiHoYoBindAccountDialog"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), new UnityAction(this.OnOKButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ForgetPswLink").GetComponent<Button>(), new UnityAction(this.OnForgetPswLinkCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ModifyPswLink").GetComponent<Button>(), new UnityAction(this.OnModifyPswLinkCallBack));
        }

        public void Close()
        {
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        public bool OnBindAccountRsp(BindAccountRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_BindSuccess", new object[0]), 2f), UIType.Any);
                this.Close();
            }
            else
            {
                string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput, 2f), UIType.Any);
            }
            return false;
        }

        public void OnForgetPswLinkCallBack()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new MihoyoForgetPwdDialogContext(), UIType.Any);
        }

        public void OnModifyPswLinkCallBack()
        {
            TheOriginalAccountManager manager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
            if (manager != null)
            {
                WebViewGeneralLogic.LoadUrl(manager.ORIGINAL_CHANGE_PASSWORD_URL, false);
            }
        }

        public void OnOKButtonCallBack()
        {
            string text = base.view.transform.Find("Dialog/Content/AccountInputField").GetComponent<InputField>().text;
            string str2 = base.view.transform.Find("Dialog/Content/PswInputField").GetComponent<InputField>().text;
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(str2))
            {
                Singleton<AccountManager>.Instance.manager.BindUIFinishedCallBack(text, str2);
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 120) && this.OnBindAccountRsp(pkt.getData<BindAccountRsp>()));
        }

        protected override bool SetupView()
        {
            InputField component = base.view.transform.Find("Dialog/Content/AccountInputField").GetComponent<InputField>();
            InputField field2 = base.view.transform.Find("Dialog/Content/PswInputField").GetComponent<InputField>();
            component.characterLimit = 50;
            field2.characterLimit = 50;
            return false;
        }
    }
}

