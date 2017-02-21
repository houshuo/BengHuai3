namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class MihoyoRegisterDialogContext : BaseDialogContext
    {
        public MihoyoRegisterDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "MihoyoRegisterDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/MiHoYoAccount/MihoyoRegisterDialog"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/EmailBtn").GetComponent<Button>(), new UnityAction(this.OnEmailBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/PhoneBtn").GetComponent<Button>(), new UnityAction(this.OnPhoneBtnClick));
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
            this.Close();
            if (rsp.get_retcode() == null)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_BindSuccess", new object[0]), 2f), UIType.Any);
            }
            else
            {
                string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput, 2f), UIType.Any);
            }
            return false;
        }

        public void OnEmailBtnClick()
        {
            TheOriginalAccountManager manager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
            if (manager != null)
            {
                WebViewGeneralLogic.LoadUrl(manager.ORIGINAL_EMAIL_REGISTER_URL, false);
            }
        }

        private bool OnMihoyoAccountRegisterSuccess(Tuple<string, string> registerData)
        {
            if (Singleton<PlayerModule>.Instance.playerData.userId > 0)
            {
                Singleton<AccountManager>.Instance.manager.BindUIFinishedCallBack(registerData.Item1, registerData.Item2);
            }
            else
            {
                Singleton<AccountManager>.Instance.manager.LoginUIFinishedCallBack(registerData.Item1, registerData.Item2);
            }
            return false;
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.MihoyoAccountRegisterSuccess)
            {
                return this.OnMihoyoAccountRegisterSuccess((Tuple<string, string>) ntf.body);
            }
            if (ntf.type == NotifyTypes.MihoyoAccountLoginSuccess)
            {
                this.Close();
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 120) && this.OnBindAccountRsp(pkt.getData<BindAccountRsp>()));
        }

        public void OnPhoneBtnClick()
        {
            TheOriginalAccountManager manager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
            if (manager != null)
            {
                WebViewGeneralLogic.LoadUrl(manager.ORIGINAL_MOBILE_REGISTER_URL, false);
            }
        }

        protected override bool SetupView()
        {
            return false;
        }
    }
}

