namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class ChatDialogContext : BaseDialogContext
    {
        private TabManager _friendTabManager;
        private bool _isFriendListShow;
        private Mode _mode;
        private TabManager _tabManager;
        private int _talkingFriendUid;
        private const float FRIEND_LIST_MAX_HEIGHT = 351.3f;
        private const float FRIEND_LIST_MIN_HEIGHT = 215.3f;
        private const float FRIEND_LIST_ROW_HEIGHT = 60f;
        private const int LINE_NUMBER_BETWEEN_HISTORY_MSG = 4;
        private const float LONG_INPUT_LENGTH = 946f;
        private const int MAX_LENGTH = 40;
        private const float SHORT_INPUT_LENGTH = 710f;

        public ChatDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ChatDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/Chat/ChatDialog"
            };
            base.config = pattern;
            this._tabManager = new TabManager();
            this._friendTabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
            this._friendTabManager.onSetActive += new TabManager.OnSetActive(this.OnFriendTabSetActive);
            if (false)
            {
                this._mode = Mode.Guild;
            }
            else
            {
                this._mode = Mode.World;
            }
            this._talkingFriendUid = 0;
        }

        public ChatDialogContext(int talkingUid)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ChatDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/Chat/ChatDialog"
            };
            base.config = pattern;
            this._tabManager = new TabManager();
            this._friendTabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
            this._friendTabManager.onSetActive += new TabManager.OnSetActive(this.OnFriendTabSetActive);
            this._mode = Mode.Friend;
            this._talkingFriendUid = !Singleton<FriendModule>.Instance.IsMyFriend(talkingUid) ? 0 : talkingUid;
            if (this._talkingFriendUid != 0)
            {
                Singleton<ChatModule>.Instance.SetFriendMsgRead(this._talkingFriendUid);
            }
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Room/ChangeBtn").GetComponent<Button>(), new UnityAction(this.OnChangeRoomBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Friend/ChangeBtn").GetComponent<Button>(), new UnityAction(this.OnChangeFriendBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/TabBtns/WorldBtn").GetComponent<Button>(), new UnityAction(this.OnWorldTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/TabBtns/GuildBtn").GetComponent<Button>(), new UnityAction(this.OnGuildTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/TabBtns/FriendBtn").GetComponent<Button>(), new UnityAction(this.OnFriendTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/SendMsgBtn").GetComponent<Button>(), new UnityAction(this.OnSendMsgBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        private void ClearInputFieldText()
        {
            InputField component = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>();
            component.textComponent.text = string.Empty;
            component.text = string.Empty;
        }

        private void Close()
        {
            this.Destroy();
        }

        private ChatMsgDataItem GenerateMsgSendByMe(string msg)
        {
            PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
            return new ChatMsgDataItem(playerData.userId, playerData.NickNameText, TimeUtil.Now, msg, ChatMsgDataItem.Type.MSG);
        }

        private int GetMsgHistoryIndex()
        {
            if (this._mode == Mode.World)
            {
                return Singleton<ChatModule>.Instance.worldHistoryMsgIndex;
            }
            if (this._mode == Mode.Guild)
            {
                return Singleton<ChatModule>.Instance.guildHistoryMsgIndex;
            }
            if (this._mode != Mode.Friend)
            {
                throw new Exception("Invalid Type or State!");
            }
            if (Singleton<ChatModule>.Instance.friendHistoryMsgIndexDic.ContainsKey(this._talkingFriendUid))
            {
                return Singleton<ChatModule>.Instance.friendHistoryMsgIndexDic[this._talkingFriendUid];
            }
            return 0;
        }

        private int GetScrollerCount()
        {
            if ((this.GetMsgHistoryIndex() > 0) && (this.GetShowMsgDataList().Count > 0))
            {
                return (this.GetShowMsgDataList().Count + 4);
            }
            return this.GetShowMsgDataList().Count;
        }

        private ChatMsgDataItem GetShowChatMsgDataItem(int index, int msgHistoryIndex)
        {
            List<ChatMsgDataItem> showMsgDataList = this.GetShowMsgDataList();
            int count = showMsgDataList.Count;
            ChatMsgDataItem item = null;
            if (count > 0)
            {
                if (msgHistoryIndex == 0)
                {
                    return this.GetShowMsgDataList()[index];
                }
                if (index < msgHistoryIndex)
                {
                    return showMsgDataList[index];
                }
                if ((index >= msgHistoryIndex) && (index < (msgHistoryIndex + 4)))
                {
                    return ((index != msgHistoryIndex) ? ChatMsgDataItem.EMPTY_MSG : ChatMsgDataItem.HISTORY_LINE_MSG);
                }
                if (index >= (msgHistoryIndex + 4))
                {
                    int num2 = index - 4;
                    if (num2 < count)
                    {
                        item = showMsgDataList[num2];
                    }
                }
            }
            return item;
        }

        private List<ChatMsgDataItem> GetShowMsgDataList()
        {
            if (this._mode == Mode.World)
            {
                return Singleton<ChatModule>.Instance.worldChatMsgList;
            }
            if (this._mode == Mode.Guild)
            {
                return Singleton<ChatModule>.Instance.guildChatMsgList;
            }
            if (this._mode != Mode.Friend)
            {
                throw new Exception("Invalid Type or State!");
            }
            return Singleton<ChatModule>.Instance.GetFriendChatMsgList(this._talkingFriendUid);
        }

        private bool OnAddFriendRsp(GetFriendListRsp rsp)
        {
            this.SetupView();
            return false;
        }

        private void OnChangeFriendBtnClick()
        {
            this._isFriendListShow = !this._isFriendListShow;
            this.UpdateFriendListOpenState();
            if (this._mode == Mode.Friend)
            {
                if (this._isFriendListShow)
                {
                    base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").GetComponent<MonoFriendChatList>().OpenFriendChatList();
                }
                else
                {
                    base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").GetComponent<MonoFriendChatList>().CloseFriendChatList();
                }
            }
        }

        private void OnChangeRoomBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new ChangeChatRoomDialogContext(), UIType.Any);
        }

        private bool OnChangeTalkingFriend(int uid)
        {
            this._talkingFriendUid = uid;
            this.SetupView();
            return false;
        }

        private void OnChangeTalkingFriendClick(FriendBriefDataItem data)
        {
            this._isFriendListShow = false;
            base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").GetComponent<MonoFriendChatList>().CloseFriendChatList();
            this._talkingFriendUid = data.uid;
            this._friendTabManager.ShowTab(data.uid.ToString());
            this.RefreshMode(Mode.Friend);
        }

        private bool OnDelFriendNotify(DelFriendNotify ntf)
        {
            return false;
        }

        private bool OnEnterWorldChatroomRsp(EnterWorldChatroomRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.SetupView();
            }
            else
            {
                string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput, 2f), UIType.Any);
            }
            return false;
        }

        private void OnFriendScrollChange(Transform trans, int index)
        {
            if (this._mode == Mode.Friend)
            {
                int targetUid = Singleton<ChatModule>.Instance.GetSortedChatFriendList()[index];
                FriendBriefDataItem friendBriefData = Singleton<FriendModule>.Instance.TryGetFriendBriefData(targetUid);
                trans.Find("FriendBtn/Name").GetComponent<Text>().text = friendBriefData.nickName;
                MonoFriendChatInfo component = trans.GetComponent<MonoFriendChatInfo>();
                bool friendMsgNewState = Singleton<ChatModule>.Instance.GetFriendMsgNewState(friendBriefData.uid);
                bool flag2 = friendBriefData.uid == this._talkingFriendUid;
                bool hasNewMessage = friendMsgNewState && !flag2;
                component.SetupView(friendBriefData, hasNewMessage, new ChangeTalkingFriend(this.OnChangeTalkingFriendClick));
                MonoGridScroller scroller = base.view.transform.Find("Dialog/Content/ChatList/ScrollView").GetComponent<MonoGridScroller>();
                Button btn = trans.Find("FriendBtn").GetComponent<Button>();
                this._friendTabManager.SetTab(friendBriefData.uid.ToString(), btn, scroller.gameObject);
            }
        }

        private void OnFriendTabBtnClick()
        {
            this._tabManager.ShowTab("FriendTab");
            this.RefreshMode(Mode.Friend);
        }

        private void OnFriendTabSetActive(bool active, GameObject go, Button btn)
        {
            MonoFriendChatInfo component = btn.transform.parent.GetComponent<MonoFriendChatInfo>();
            if (active)
            {
                FriendBriefDataItem friendData = component.GetFriendData();
                if (friendData != null)
                {
                    Singleton<ChatModule>.Instance.SetFriendMsgRead(friendData.uid);
                    component.SetNewMessageTipShow(false);
                }
            }
            component.RefreshNickName();
        }

        private void OnGuildTabBtnClick()
        {
            this._tabManager.ShowTab("GuildTab");
            this.RefreshMode(Mode.Guild);
        }

        private bool OnNicknameModifyRsp(NicknameModifyRsp rsp)
        {
            this.SetupView();
            return false;
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.ChangeTalkingFriend) && this.OnChangeTalkingFriend((int) ntf.body));
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x59:
                    return this.OnEnterWorldChatroomRsp(pkt.getData<EnterWorldChatroomRsp>());

                case 0x5b:
                    return this.OnRecvWorldChatMsgNotify(pkt.getData<RecvWorldChatMsgNotify>());

                case 0x5d:
                    return this.OnRecvFriendChatMsgNotify(pkt.getData<RecvFriendChatMsgNotify>());

                case 0x41:
                    return this.OnAddFriendRsp(pkt.getData<GetFriendListRsp>());

                case 80:
                    return this.OnDelFriendNotify(pkt.getData<DelFriendNotify>());

                case 0x15:
                    return this.OnNicknameModifyRsp(pkt.getData<NicknameModifyRsp>());
            }
            return false;
        }

        private bool OnRecvFriendChatMsgNotify(RecvFriendChatMsgNotify rsp)
        {
            if ((this._talkingFriendUid > 0) && (rsp.get_chat_msg().get_uid() == this._talkingFriendUid))
            {
                if (this._mode == Mode.Friend)
                {
                    this.UpdateChatList(1);
                }
                Singleton<ChatModule>.Instance.SetFriendMsgRead(this._talkingFriendUid);
            }
            else
            {
                this.SetupFriendList();
                this.UpdateNewMsgBtnTip();
            }
            return false;
        }

        private bool OnRecvWorldChatMsgNotify(RecvWorldChatMsgNotify rsp)
        {
            if ((this._mode == Mode.World) && (Singleton<ChatModule>.Instance.chatRoomId > 0))
            {
                this.UpdateChatList(1);
            }
            return false;
        }

        private void OnScrollChange(Transform trans, int index)
        {
            Text component = trans.Find("Nickname").GetComponent<Text>();
            Text text2 = trans.Find("Msg").GetComponent<Text>();
            int msgHistoryIndex = this.GetMsgHistoryIndex();
            ChatMsgDataItem showChatMsgDataItem = this.GetShowChatMsgDataItem(index, msgHistoryIndex);
            if (showChatMsgDataItem == null)
            {
                component.gameObject.SetActive(false);
                text2.gameObject.SetActive(false);
            }
            else
            {
                bool flag = Singleton<PlayerModule>.Instance.playerData.userId == showChatMsgDataItem.uid;
                bool flag2 = showChatMsgDataItem.isMsgDataItemBelongToType(ChatMsgDataItem.Type.HISTORY_LINE);
                component.gameObject.SetActive(!flag2);
                text2.gameObject.SetActive(!flag2);
                trans.Find("HistoryLine").gameObject.SetActive(flag2);
                if (showChatMsgDataItem.isMsgDataItemBelongToType(ChatMsgDataItem.Type.EMPTY))
                {
                    component.text = string.Empty;
                    text2.text = string.Empty;
                }
                else
                {
                    Color color = new Color(1f, 0.6f, 0f);
                    Color color2 = new Color(0f, 0.76f, 1f);
                    Color color3 = new Color(0.99f, 0.87f, 0.3f);
                    Color white = Color.white;
                    Color grey = Color.grey;
                    Color color6 = new Color(1f, 0.36f, 0.25f);
                    if (this._mode == Mode.World)
                    {
                        component.color = !flag ? color2 : color;
                        component.text = string.Format("[{0}]", showChatMsgDataItem.nickname);
                        text2.color = !flag ? white : color3;
                        text2.text = showChatMsgDataItem.msg;
                        if (!flag)
                        {
                        }
                    }
                    else if (this._mode == Mode.Guild)
                    {
                        component.text = this.WrapGuildTalkMsgRichText(showChatMsgDataItem);
                        text2.color = !flag ? white : color3;
                        text2.text = showChatMsgDataItem.msg;
                    }
                    else if (this._mode == Mode.Friend)
                    {
                        component.text = this.WrapFriendTalkMsgRichText(showChatMsgDataItem);
                        text2.text = showChatMsgDataItem.msg;
                        if (Singleton<FriendModule>.Instance.IsMyFriend(this._talkingFriendUid))
                        {
                            text2.color = !flag ? white : color3;
                        }
                        else
                        {
                            text2.color = grey;
                        }
                    }
                    if (showChatMsgDataItem.isMsgDataItemBelongToType(ChatMsgDataItem.Type.SYSTEM))
                    {
                        component.color = color6;
                        text2.color = color6;
                        component.text = string.Format("[{0}]", LocalizationGeneralLogic.GetText("Chat_Content_System", new object[0]));
                    }
                    bool flag4 = showChatMsgDataItem.isMsgDataItemBelongToType(ChatMsgDataItem.Type.LUCK_GECHA);
                    text2.supportRichText = flag4;
                    if (index < msgHistoryIndex)
                    {
                        text2.color = grey;
                    }
                }
            }
        }

        private void OnSendMsgBtnClick()
        {
            string str = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>().text.Trim();
            int length = Mathf.Min(40, str.Length);
            str = str.Substring(0, length);
            if (!string.IsNullOrEmpty(str) && ((this._mode != Mode.Friend) || (this._talkingFriendUid > 0)))
            {
                ChatMsgDataItem msgItem = this.GenerateMsgSendByMe(str);
                if (this._mode == Mode.World)
                {
                    if (Singleton<ChatModule>.Instance.IsWorldChatAllowed(msgItem))
                    {
                        Singleton<ChatModule>.Instance.worldChatMsgList.Add(msgItem);
                        Singleton<ChatModule>.Instance.lastWorldChatTime = msgItem.time;
                        Singleton<NetworkManager>.Instance.NotifySendWorldChatMsg(msgItem.msg);
                    }
                    else
                    {
                        Singleton<ChatModule>.Instance.worldChatMsgList.Add(ChatMsgDataItem.TALK_TOO_FAST_MSG);
                    }
                }
                else if (this._mode == Mode.Friend)
                {
                    if (Singleton<FriendModule>.Instance.IsMyFriend(this._talkingFriendUid))
                    {
                        Singleton<ChatModule>.Instance.AddFriendChatMsgByMySelf(msgItem, this._talkingFriendUid, true);
                        Singleton<NetworkManager>.Instance.NotifySendFriendChatMsg(this._talkingFriendUid, msgItem.msg);
                        this.SetupFriendList();
                    }
                    else
                    {
                        msgItem.msg = LocalizationGeneralLogic.GetText("Chat_Content_NotFriend", new object[0]);
                        Singleton<ChatModule>.Instance.AddFriendChatMsgByMySelf(msgItem, this._talkingFriendUid, false);
                    }
                }
                else if (this._mode == Mode.Guild)
                {
                    Singleton<ChatModule>.Instance.guildChatMsgList.Add(msgItem);
                    Singleton<NetworkManager>.Instance.NotifySendGuildChatMsg(msgItem.msg);
                }
                this.UpdateChatList(1);
                this.ClearInputFieldText();
            }
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
            btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.transform.Find("Image").GetComponent<Image>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.interactable = !active;
            if (!btn.enabled)
            {
                btn.GetComponent<Image>().color = Color.grey;
            }
            this.ClearInputFieldText();
        }

        private void OnWorldTabBtnClick()
        {
            this._tabManager.ShowTab("WorldTab");
            this.RefreshMode(Mode.World);
        }

        private void RefreshMode(Mode mode)
        {
            this._mode = mode;
            this.SetupView();
        }

        private void SetInputLength(float length)
        {
            RectTransform component = base.view.transform.Find("Dialog/Content/InputField").GetComponent<RectTransform>();
            component.sizeDelta = new Vector2(length, component.sizeDelta.y);
        }

        private void SetupChannelView()
        {
            if (this._mode == Mode.World)
            {
                this.SetupWorldView();
            }
            if (this._mode == Mode.Guild)
            {
                this.SetupGuildView();
            }
            else if (this._mode == Mode.Friend)
            {
                this.SetupFriendView();
            }
        }

        private void SetupChatList()
        {
            MonoGridScroller component = base.view.transform.Find("Dialog/Content/ChatList/ScrollView").GetComponent<MonoGridScroller>();
            component.Init(new MonoGridScroller.OnChange(this.OnScrollChange), this.GetScrollerCount(), null);
            component.ScrollToEnd();
        }

        private void SetupFriendList()
        {
            MonoGridScroller component = base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList/ScrollView").GetComponent<MonoGridScroller>();
            RectTransform transform = base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").GetComponent<RectTransform>();
            int friendChatCount = Singleton<ChatModule>.Instance.GetFriendChatCount();
            float size = Mathf.Clamp((float) ((friendChatCount * 60f) + 30f), (float) 215.3f, (float) 351.3f);
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            this._friendTabManager.Clear();
            component.Init(new MonoGridScroller.OnChange(this.OnFriendScrollChange), friendChatCount, null);
            component.ScrollToEnd();
        }

        private void SetupFriendView()
        {
            string text = LocalizationGeneralLogic.GetText("Chat_Content_FriendUnselected", new object[0]);
            base.view.transform.Find("Dialog/Content/Friend/ChangeBtn/Name").GetComponent<Text>().text = (this._talkingFriendUid != 0) ? Singleton<FriendModule>.Instance.TryGetPlayerNickName(this._talkingFriendUid) : text;
            this.SetInputLength(710f);
        }

        private void SetupGuildView()
        {
            this.SetInputLength(946f);
        }

        private void SetupTabView()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = showingTabKey;
            if (string.IsNullOrEmpty(showingTabKey))
            {
                if (this._mode == Mode.World)
                {
                    searchKey = "WorldTab";
                }
                else if (this._mode == Mode.Guild)
                {
                    searchKey = "GuildTab";
                }
                else if (this._mode == Mode.Friend)
                {
                    searchKey = "FriendTab";
                }
            }
            MonoGridScroller component = base.view.transform.Find("Dialog/Content/ChatList/ScrollView").GetComponent<MonoGridScroller>();
            this._tabManager.SetTab("WorldTab", base.view.transform.Find("Dialog/TabBtns/WorldBtn").GetComponent<Button>(), component.gameObject);
            this._tabManager.SetTab("FriendTab", base.view.transform.Find("Dialog/TabBtns/FriendBtn").GetComponent<Button>(), component.gameObject);
            this._tabManager.ShowTab(searchKey);
        }

        protected override bool SetupView()
        {
            InputField component = base.view.transform.Find("Dialog/Content/InputField").GetComponent<InputField>();
            component.characterLimit = 0;
            component.GetComponent<InputFieldHelper>().mCharacterlimit = 40;
            base.view.transform.Find("Dialog/Content/Room").gameObject.SetActive(this._mode == Mode.World);
            base.view.transform.Find("Dialog/Content/WorldModeBtn").gameObject.SetActive(this._mode == Mode.World);
            base.view.transform.Find("Dialog/Content/GuildModeBtn").gameObject.SetActive(this._mode == Mode.Guild);
            base.view.transform.Find("Dialog/Content/FriendModeBtn").gameObject.SetActive(this._mode == Mode.Friend);
            base.view.transform.Find("Dialog/Content/Friend").gameObject.SetActive(this._mode == Mode.Friend);
            base.view.transform.Find("Dialog/TabBtns/GuildBtn").GetComponent<Button>().enabled = false;
            this.UpdateFriendListOpenState();
            this.UpdateNewMsgBtnTip();
            this.SetupChannelView();
            this.SetupChatList();
            this.SetupFriendList();
            this.SetupTabView();
            return false;
        }

        private void SetupWorldView()
        {
            base.view.transform.Find("Dialog/Content/Room/ChangeBtn/Num").GetComponent<Text>().text = Singleton<ChatModule>.Instance.chatRoomId.ToString();
            this.SetInputLength(946f);
        }

        private void UpdateChatList(int addMsgCount)
        {
            MonoGridScroller component = base.view.transform.Find("Dialog/Content/ChatList/ScrollView").GetComponent<MonoGridScroller>();
            component.AddChildren(addMsgCount);
            component.ScrollToNextItem();
        }

        private void UpdateFriendList(int addFriendCount)
        {
            MonoGridScroller component = base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList/ScrollView").GetComponent<MonoGridScroller>();
            component.AddChildren(addFriendCount);
            component.ScrollToNextItem();
        }

        private void UpdateFriendListOpenState()
        {
            base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").gameObject.SetActive(this._mode == Mode.Friend);
            bool flag = base.view.transform.Find("Dialog/Content/FriendListPanel/FriendList").GetComponent<MonoFriendChatList>().status == MonoFriendChatList.Status.Open;
            base.view.transform.Find("Dialog/Content/FriendListPanel").GetComponent<CanvasGroup>().blocksRaycasts = (this._mode == Mode.Friend) && flag;
            base.view.transform.Find("Dialog/Content/Friend/ChangeBtn/Image").GetComponent<RectTransform>().SetLocalEulerAnglesZ(!this._isFriendListShow ? ((float) 180) : ((float) 0));
        }

        private void UpdateNewMsgBtnTip()
        {
            bool flag = Singleton<ChatModule>.Instance.IsFriendChatListHasNewMsg();
            base.view.transform.Find("Dialog/Content/Friend/ChangeBtn/Tip").gameObject.SetActive(flag);
        }

        private string WrapFriendTalkMsgRichText(ChatMsgDataItem msgData)
        {
            string text = LocalizationGeneralLogic.GetText("Chat_Content_I", new object[0]);
            string str2 = LocalizationGeneralLogic.GetText("Chat_Content_To", new object[0]);
            string str3 = LocalizationGeneralLogic.GetText("Chat_Content_Speak", new object[0]);
            if (Singleton<PlayerModule>.Instance.playerData.userId == msgData.uid)
            {
                object[] objArray1 = new object[] { text, str2, Singleton<FriendModule>.Instance.TryGetPlayerNickName(this._talkingFriendUid), str3 };
                return string.Format("<color=#ff9900>{0}{1}</color><color=#00c1ff>[{2}]</color><color=#ff9900>{3}</color>:", objArray1);
            }
            object[] args = new object[] { "<color=#00c1ff>", Singleton<FriendModule>.Instance.TryGetPlayerNickName(this._talkingFriendUid), "</color>", "<color=white>", str2, text, str3, "</color>" };
            return string.Format("{0}[{1}]{2}{3}{4}{5}{6}:{7}", args);
        }

        private string WrapGuildTalkMsgRichText(ChatMsgDataItem msgData)
        {
            if (Singleton<PlayerModule>.Instance.playerData.userId == msgData.uid)
            {
                object[] objArray1 = new object[] { "<color=#ff9900>", msgData.nickname, "</color>", "<color=#00c1ff>", msgData.guildTitle, "</color>" };
                return string.Format("{0}[{1}]{2}{3}[{4}]:{5}", objArray1);
            }
            object[] args = new object[] { "<color=#00c1ff>", msgData.nickname, "</color>", "<color=#00c1ff>", msgData.guildTitle, "</color>" };
            return string.Format("{0}[{1}]{2}{3}[{4}]:{5}", args);
        }

        public enum Mode
        {
            World,
            Guild,
            Friend
        }
    }
}

