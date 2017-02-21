namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class ChangeChatRoomDialogContext : BaseDialogContext
    {
        public ChangeChatRoomDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ChangeChatRoomDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/Chat/ChangeChatRoomDialog"
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

        public bool OnEnterWorldChatroomRsp(EnterWorldChatroomRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.Close();
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]), 2f), UIType.Any);
            }
            return false;
        }

        public void OnOKButtonCallBack()
        {
            string str = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>().text.Trim();
            if (!string.IsNullOrEmpty(str))
            {
                int result = 0;
                if (!int.TryParse(str, out result))
                {
                    this.ShowErrMsg(LocalizationGeneralLogic.GetText("Err_ChatRoomIdWrong", new object[0]));
                }
                else if ((result <= 0) || (result > MiscData.Config.ChatConfig.ChatRoomMaxNum))
                {
                    this.ShowErrMsg(LocalizationGeneralLogic.GetText("Err_ChatRoomIdWrong", new object[0]));
                }
                else if (result == Singleton<ChatModule>.Instance.chatRoomId)
                {
                    this.ShowErrMsg(LocalizationGeneralLogic.GetText("Err_ChatRoomIdTheSame", new object[0]));
                }
                else
                {
                    Singleton<NetworkManager>.Instance.RequestEnterWorldChatroom(result);
                }
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x59) && this.OnEnterWorldChatroomRsp(pkt.getData<EnterWorldChatroomRsp>()));
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>().text = Singleton<ChatModule>.Instance.chatRoomId.ToString();
            Text component = base.view.transform.Find("Dialog/Content/RoomNumText").GetComponent<Text>();
            if (component != null)
            {
                int chatRoomMinNum = MiscData.Config.ChatConfig.ChatRoomMinNum;
                int chatRoomMaxNum = MiscData.Config.ChatConfig.ChatRoomMaxNum;
                component.text = string.Format("[{0}-{1}]", chatRoomMinNum, chatRoomMaxNum);
            }
            return false;
        }

        private void ShowErrMsg(string msg)
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(msg, 2f), UIType.Any);
        }
    }
}

