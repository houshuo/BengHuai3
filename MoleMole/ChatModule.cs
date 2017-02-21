namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UniRx;

    public class ChatModule : BaseModule
    {
        private Dictionary<int, List<ChatMsgDataItem>> _friendChatMsgMap;
        private Dictionary<int, Tuple<bool, int>> _friendNewMsgWeightDic;
        private int _weight;
        public int chatRoomId;
        public Dictionary<int, int> friendHistoryMsgIndexDic;
        public List<ChatMsgDataItem> guildChatMsgList;
        public int guildHistoryMsgIndex;
        public DateTime lastWorldChatTime;
        public List<ChatMsgDataItem> worldChatMsgList;
        public int worldHistoryMsgIndex;

        private ChatModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this.worldChatMsgList = new List<ChatMsgDataItem>();
            this.guildChatMsgList = new List<ChatMsgDataItem>();
            this._friendChatMsgMap = new Dictionary<int, List<ChatMsgDataItem>>();
            this._friendNewMsgWeightDic = new Dictionary<int, Tuple<bool, int>>();
            this.friendHistoryMsgIndexDic = new Dictionary<int, int>();
            this.chatRoomId = 0;
            this._weight = 0;
        }

        private void AddFriendChatMsg(ChatMsgDataItem msgData)
        {
            if (!this._friendChatMsgMap.ContainsKey(msgData.uid))
            {
                this._friendChatMsgMap.Add(msgData.uid, new List<ChatMsgDataItem>());
            }
            this._friendChatMsgMap[msgData.uid].Add(msgData);
            this.SaveLocalFriendChatMsg();
            this.SetFriendMsgNew(msgData);
        }

        public void AddFriendChatMsgByMySelf(ChatMsgDataItem msgData, int friendId, bool needSave = true)
        {
            if (!this._friendChatMsgMap.ContainsKey(friendId))
            {
                this._friendChatMsgMap.Add(friendId, new List<ChatMsgDataItem>());
            }
            this._friendChatMsgMap[friendId].Add(msgData);
            if (!this._friendNewMsgWeightDic.ContainsKey(friendId))
            {
                this._friendNewMsgWeightDic.Add(friendId, Tuple.Create<bool, int>(false, 0));
            }
            else
            {
                this._friendNewMsgWeightDic[friendId] = Tuple.Create<bool, int>(false, this._friendNewMsgWeightDic[friendId].Item2);
            }
            if (needSave)
            {
                this.SaveLocalFriendChatMsg();
            }
        }

        public int GetFriendChatCount()
        {
            int num = 0;
            foreach (int num2 in this._friendNewMsgWeightDic.Keys)
            {
                if (Singleton<FriendModule>.Instance.IsMyFriend(num2))
                {
                    num++;
                }
            }
            return num;
        }

        public List<ChatMsgDataItem> GetFriendChatMsgList(int friendId)
        {
            List<ChatMsgDataItem> list = new List<ChatMsgDataItem>();
            if ((friendId > 0) && this._friendChatMsgMap.ContainsKey(friendId))
            {
                return this._friendChatMsgMap[friendId];
            }
            return list;
        }

        public bool GetFriendMsgNewState(int uid)
        {
            if (!this._friendNewMsgWeightDic.ContainsKey(uid))
            {
                return false;
            }
            return this._friendNewMsgWeightDic[uid].Item1;
        }

        public List<int> GetSortedChatFriendList()
        {
            List<int> list = new List<int>();
            foreach (int num in this._friendNewMsgWeightDic.Keys)
            {
                if (!list.Contains(num))
                {
                    list.Add(num);
                }
            }
            list.Sort(new Comparison<int>(this.MostRecentOrderCompare));
            return list;
        }

        public bool IsFriendChatListHasNewMsg()
        {
            foreach (KeyValuePair<int, Tuple<bool, int>> pair in this._friendNewMsgWeightDic)
            {
                if (pair.Value.Item1)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsInWorldChatCD(DateTime chatTime)
        {
            TimeSpan span = (TimeSpan) (chatTime - this.lastWorldChatTime);
            return (span.TotalSeconds >= MiscData.Config.ChatConfig.WorldChatInterval);
        }

        private bool IsInWorldChatLevel()
        {
            return (Singleton<PlayerModule>.Instance.playerData.teamLevel >= MiscData.Config.ChatConfig.WorldChatLevelRequirment);
        }

        public bool IsWorldChatAllowed(ChatMsgDataItem msgItem)
        {
            return (this.IsInWorldChatCD(msgItem.time) && this.IsInWorldChatLevel());
        }

        public int MostRecentOrderCompare(int uidA, int uidB)
        {
            int num = this._friendNewMsgWeightDic[uidA].Item2;
            return (this._friendNewMsgWeightDic[uidB].Item2 - num);
        }

        private bool OnEnterWorldChatroomRsp(EnterWorldChatroomRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.chatRoomId = (int) rsp.get_chatroom_id();
                this.worldChatMsgList.Clear();
                foreach (ChatMsg msg in rsp.get_his_chat_msg_list())
                {
                    this.worldChatMsgList.Add(new ChatMsgDataItem(msg));
                }
                this.worldHistoryMsgIndex = this.worldChatMsgList.Count;
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 7:
                    return this.OnPlayerLoginRsp(pkt.getData<PlayerLoginRsp>());

                case 0x59:
                    return this.OnEnterWorldChatroomRsp(pkt.getData<EnterWorldChatroomRsp>());

                case 0x5b:
                    return this.OnRecvWorldChatMsgNotify(pkt.getData<RecvWorldChatMsgNotify>());

                case 0x5d:
                    return this.OnRecvFriendChatMsgNotify(pkt.getData<RecvFriendChatMsgNotify>());

                case 0x5e:
                    return this.OnRecvFriendOfflineChatMsgNotify(pkt.getData<RecvFriendOfflineChatMsgNotify>());

                case 0x61:
                    return this.OnRecvSystemChatMsgNotify(pkt.getData<RecvSystemChatMsgNotify>());
            }
            return false;
        }

        private bool OnPlayerLoginRsp(PlayerLoginRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (KeyValuePair<int, List<ChatMsgDataItem>> pair in Singleton<MiHoYoGameData>.Instance.LocalData.FriendChatMsgMap)
                {
                    if (!this._friendChatMsgMap.ContainsKey(pair.Key))
                    {
                        this._friendChatMsgMap.Add(pair.Key, new List<ChatMsgDataItem>());
                    }
                    this._friendChatMsgMap[pair.Key].AddRange(pair.Value);
                }
            }
            foreach (int num in this._friendChatMsgMap.Keys)
            {
                if (!this.friendHistoryMsgIndexDic.ContainsKey(num))
                {
                    this.friendHistoryMsgIndexDic.Add(num, this._friendChatMsgMap[num].Count);
                }
            }
            return false;
        }

        private bool OnRecvFriendChatMsgNotify(RecvFriendChatMsgNotify rsp)
        {
            this.AddFriendChatMsg(new ChatMsgDataItem(rsp.get_chat_msg()));
            return false;
        }

        private bool OnRecvFriendOfflineChatMsgNotify(RecvFriendOfflineChatMsgNotify rsp)
        {
            foreach (ChatMsg msg in rsp.get_chat_msg_list())
            {
                this.AddFriendChatMsg(new ChatMsgDataItem(msg));
            }
            return false;
        }

        private bool OnRecvSystemChatMsgNotify(RecvSystemChatMsgNotify rsp)
        {
            if (rsp.get_chat_msg().get_type() == 1)
            {
                this.worldChatMsgList.Add(new ChatMsgDataItem(rsp.get_chat_msg()));
            }
            return false;
        }

        private bool OnRecvWorldChatMsgNotify(RecvWorldChatMsgNotify rsp)
        {
            this.worldChatMsgList.Add(new ChatMsgDataItem(rsp.get_chat_msg()));
            return false;
        }

        private void SaveLocalFriendChatMsg()
        {
            int cacheOfflineChatMsgMaxNum = MiscData.Config.ChatConfig.CacheOfflineChatMsgMaxNum;
            Dictionary<int, List<ChatMsgDataItem>> friendChatMsgMap = Singleton<MiHoYoGameData>.Instance.LocalData.FriendChatMsgMap;
            foreach (KeyValuePair<int, List<ChatMsgDataItem>> pair in this._friendChatMsgMap)
            {
                List<ChatMsgDataItem> list = (pair.Value.Count <= cacheOfflineChatMsgMaxNum) ? pair.Value : pair.Value.GetRange(pair.Value.Count - cacheOfflineChatMsgMaxNum, cacheOfflineChatMsgMaxNum);
                friendChatMsgMap[pair.Key] = list;
            }
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        private void SetFriendMsgNew(ChatMsgDataItem msgData)
        {
            if (!this._friendNewMsgWeightDic.ContainsKey(msgData.uid))
            {
                this._friendNewMsgWeightDic.Add(msgData.uid, Tuple.Create<bool, int>(true, this._weight++));
            }
            this._friendNewMsgWeightDic[msgData.uid] = Tuple.Create<bool, int>(true, this._weight++);
        }

        public void SetFriendMsgRead(int uid)
        {
            if (this._friendNewMsgWeightDic.ContainsKey(uid))
            {
                this._friendNewMsgWeightDic[uid] = Tuple.Create<bool, int>(false, this._friendNewMsgWeightDic[uid].Item2);
            }
        }
    }
}

