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

    public class ChangeNicknameDialogContext : BaseDialogContext
    {
        private const int MAX_LENGTH = 8;

        public ChangeNicknameDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ChangeNicknameDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/ChangeNicknameDialog"
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

        public bool OnNicknameModifyRsp(NicknameModifyRsp rsp)
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

        public void OnOKButtonCallBack()
        {
            string str = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>().text.Trim();
            int length = Mathf.Min(8, str.Length);
            str = str.Substring(0, length);
            if (!string.IsNullOrEmpty(str))
            {
                Singleton<NetworkManager>.Instance.RequestNicknameChange(str);
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x15) && this.OnNicknameModifyRsp(pkt.getData<NicknameModifyRsp>()));
        }

        protected override bool SetupView()
        {
            InputField component = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>();
            component.characterLimit = 0;
            component.GetComponent<InputFieldHelper>().mCharacterlimit = 8;
            component.text = Singleton<PlayerModule>.Instance.playerData.nickname;
            return false;
        }
    }
}

