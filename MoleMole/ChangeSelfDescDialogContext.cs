namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class ChangeSelfDescDialogContext : BaseDialogContext
    {
        private const int MAX_LENGTH = 40;

        public ChangeSelfDescDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ChangeSelfDescDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/ChangeSelfDescDialog"
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
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        public void OnOKButtonCallBack()
        {
            string str = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>().text.Trim();
            int length = Mathf.Min(40, str.Length);
            str = str.Substring(0, length);
            if (!string.IsNullOrEmpty(str))
            {
                Singleton<NetworkManager>.Instance.RequestSelfDescChange(str);
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x4f) && this.OnSetSelfDescRsp(pkt.getData<SetSelfDescRsp>()));
        }

        public bool OnSetSelfDescRsp(SetSelfDescRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.Close();
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        protected override bool SetupView()
        {
            InputField component = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>();
            component.characterLimit = 0;
            component.GetComponent<InputFieldHelper>().mCharacterlimit = 40;
            component.text = Singleton<PlayerModule>.Instance.playerData.SelfDescText;
            return false;
        }
    }
}

