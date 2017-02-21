namespace MoleMole
{
    using proto;
    using System;
    using System.Runtime.InteropServices;

    public class ChatMsgDataItem
    {
        public const int DIVIDE_UID = -2;
        public static ChatMsgDataItem EMPTY_MSG = CreateEmptyMsgDataItem();
        public const int EMPTY_UID = -3;
        public string guildTitle;
        public static ChatMsgDataItem HISTORY_LINE_MSG = CreateHistoryLineMsgDataItem();
        public string msg;
        public string nickname;
        public const int SYSTEM_UID = -1;
        public static ChatMsgDataItem TALK_TOO_FAST_MSG = CreateSystemFastTalkMsgDataItem();
        public DateTime time;
        public Type type;
        public int uid;

        public ChatMsgDataItem()
        {
        }

        public ChatMsgDataItem(ChatMsg chatMsg)
        {
            DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(chatMsg.get_time());
            string nickname = !string.IsNullOrEmpty(chatMsg.get_nickname()) ? chatMsg.get_nickname() : ("ID. " + chatMsg.get_uid());
            this.Init((int) chatMsg.get_uid(), nickname, dateTimeFromTimeStamp, chatMsg.get_msg(), Type.MSG);
        }

        public ChatMsgDataItem(SystemChatMsg sysChatMsg)
        {
            if (sysChatMsg.get_type() == 1)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) sysChatMsg.get_item_id(), 1);
                Type mSG = Type.MSG;
                object[] replaceParams = new object[] { string.Format("[{0}]", dummyStorageDataItem.GetDisplayTitle()) };
                string text = LocalizationGeneralLogic.GetText("ChatMsg_GachaGetItem", replaceParams);
                if (dummyStorageDataItem.rarity >= 4)
                {
                    string str2 = string.Format("{0}[{1}]{2}", "<color=#9b59b6>", dummyStorageDataItem.GetDisplayTitle(), "</color>");
                    string str3 = string.Format("{0}{1}{2}", "<color=#88c700ff>", LocalizationGeneralLogic.GetText("Chat_Content_Source_Egg", new object[0]), "</color>");
                    object[] objArray2 = new object[] { str3, str2 };
                    text = LocalizationGeneralLogic.GetText("ChatMsg_GachaGetItemFrom", objArray2);
                    mSG = Type.LUCK_GECHA;
                }
                DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(sysChatMsg.get_time());
                string nickname = !string.IsNullOrEmpty(sysChatMsg.get_nickname()) ? sysChatMsg.get_nickname() : ("ID. " + sysChatMsg.get_uid());
                this.Init((int) sysChatMsg.get_uid(), nickname, dateTimeFromTimeStamp, text, mSG);
            }
        }

        public ChatMsgDataItem(int uid, string nickname, DateTime time, string msg, Type type = 0)
        {
            this.Init(uid, nickname, time, msg, Type.MSG);
        }

        public static ChatMsgDataItem CreateEmptyMsgDataItem()
        {
            return new ChatMsgDataItem { uid = -3, type = Type.EMPTY };
        }

        public static ChatMsgDataItem CreateHistoryLineMsgDataItem()
        {
            return new ChatMsgDataItem { uid = -2, type = Type.HISTORY_LINE };
        }

        public static ChatMsgDataItem CreateSystemFastTalkMsgDataItem()
        {
            return new ChatMsgDataItem { uid = -1, type = Type.SYSTEM, msg = LocalizationGeneralLogic.GetText("Chat_Content_TalkTooFast", new object[0]) };
        }

        private void Init(int uid, string nickname, DateTime time, string msg, Type type = 0)
        {
            this.uid = uid;
            this.nickname = nickname;
            this.time = time;
            this.msg = msg;
            this.type = type;
        }

        public bool isMsgDataItemBelongToType(Type dataType)
        {
            if (dataType == Type.EMPTY)
            {
                return ((this.type == dataType) && (this.uid == -3));
            }
            if (dataType == Type.HISTORY_LINE)
            {
                return ((this.type == dataType) && (this.uid == -2));
            }
            if (dataType == Type.MSG)
            {
                return (this.type == dataType);
            }
            if (dataType == Type.SYSTEM)
            {
                return ((this.type == dataType) && (this.uid == -1));
            }
            return ((dataType == Type.LUCK_GECHA) && (this.type == Type.LUCK_GECHA));
        }

        public enum Type
        {
            MSG,
            EMPTY,
            HISTORY_LINE,
            SYSTEM,
            LUCK_GECHA
        }
    }
}

