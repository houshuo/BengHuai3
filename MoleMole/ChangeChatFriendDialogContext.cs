namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class ChangeChatFriendDialogContext : BaseDialogContext
    {
        public ChangeChatFriendDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ChangeChatFriendDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/Chat/ChangeChatFriendDialog"
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
            if (!string.IsNullOrEmpty(str))
            {
                int result = 0;
                if (int.TryParse(str, out result) && (result > 0))
                {
                    this.Close();
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ChangeTalkingFriend, result));
                }
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return false;
        }

        protected override bool SetupView()
        {
            return false;
        }

        private void ShowErrMsg(string msg)
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(msg, 2f), UIType.Any);
        }
    }
}

