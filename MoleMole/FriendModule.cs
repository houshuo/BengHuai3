namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class FriendModule : BaseModule
    {
        private Dictionary<int, FriendBriefDataItem> _askingDict;
        private Dictionary<int, FriendBriefDataItem> _friendsBriefInfoDict;
        private Dictionary<int, DateTime> _helperFrozenMap;
        private Dictionary<int, CacheData<FriendBriefDataItem>> _playerBriefInfoCacheDict;
        private Dictionary<int, CacheData<FriendDetailDataItem>> _playerDetialInfoCacheDict;
        private HashSet<int> _requestAddPlayerUIDSet;
        public List<FriendBriefDataItem> askingList;
        public List<FriendBriefDataItem> friendsList;
        public List<FriendBriefDataItem> helperStrangerList;
        public List<FriendBriefDataItem> recommandedPlayerList;
        public Dictionary<FriendSortType, Comparison<FriendBriefDataItem>> sortComparisionMap;
        public Dictionary<string, FriendSortType> sortTypeMap;

        public FriendModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this.friendsList = new List<FriendBriefDataItem>();
            this._friendsBriefInfoDict = new Dictionary<int, FriendBriefDataItem>();
            this._playerBriefInfoCacheDict = new Dictionary<int, CacheData<FriendBriefDataItem>>();
            this._playerDetialInfoCacheDict = new Dictionary<int, CacheData<FriendDetailDataItem>>();
            this.askingList = new List<FriendBriefDataItem>();
            this._askingDict = new Dictionary<int, FriendBriefDataItem>();
            this.recommandedPlayerList = new List<FriendBriefDataItem>();
            this.helperStrangerList = new List<FriendBriefDataItem>();
            this._helperFrozenMap = new Dictionary<int, DateTime>();
            this._requestAddPlayerUIDSet = new HashSet<int>();
            this.InitForSort();
        }

        public DateTime GetHelperNextAvaliableTime(int uid)
        {
            if (this._helperFrozenMap.ContainsKey(uid))
            {
                return this._helperFrozenMap[uid];
            }
            return TimeUtil.Now;
        }

        public FriendBriefDataItem GetOneStrangeHelper()
        {
            FriendBriefDataItem item = this.helperStrangerList.Find(x => !this.isHelperFrozen(x.uid) && !this._friendsBriefInfoDict.ContainsKey(x.uid));
            if (item == null)
            {
                Singleton<NetworkManager>.Instance.RequestRecommandFriendList();
            }
            return item;
        }

        public List<FriendBriefDataItem> GetRecommandFriendList()
        {
            List<FriendBriefDataItem> list = new List<FriendBriefDataItem>();
            foreach (FriendBriefDataItem item in this.recommandedPlayerList)
            {
                if (!this._friendsBriefInfoDict.ContainsKey(item.uid) && !this._requestAddPlayerUIDSet.Contains(item.uid))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public bool HasNewFriend()
        {
            foreach (FriendBriefDataItem item in this.friendsList)
            {
                if (!this.IsOldFriend(item.uid))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasNewRequest()
        {
            foreach (FriendBriefDataItem item in this.askingList)
            {
                if (!this.IsOldRequest(item.uid))
                {
                    return true;
                }
            }
            return false;
        }

        private void InitForSort()
        {
            this.sortTypeMap = new Dictionary<string, FriendSortType>();
            this.sortComparisionMap = new Dictionary<FriendSortType, Comparison<FriendBriefDataItem>>();
            foreach (string str in FriendOverviewPageContext.TAB_KEY)
            {
                if (str == "FriendListTab")
                {
                    this.sortTypeMap.Add(str, FriendSortType.Friend_NEW);
                }
                else if (str == "RequestListTab")
                {
                    this.sortTypeMap.Add(str, FriendSortType.Request_NEW);
                }
                else
                {
                    this.sortTypeMap.Add(str, FriendSortType.Level_DESC);
                }
            }
            this.sortComparisionMap.Add(FriendSortType.Friend_NEW, new Comparison<FriendBriefDataItem>(FriendBriefDataItem.CompareToFriendNew));
            this.sortComparisionMap.Add(FriendSortType.Request_NEW, new Comparison<FriendBriefDataItem>(FriendBriefDataItem.CompareToRequestNew));
            this.sortComparisionMap.Add(FriendSortType.Level_DESC, new Comparison<FriendBriefDataItem>(FriendBriefDataItem.CompareToLevelDesc));
            this.sortComparisionMap.Add(FriendSortType.Level_ASC, new Comparison<FriendBriefDataItem>(FriendBriefDataItem.CompareToLevelAsc));
            this.sortComparisionMap.Add(FriendSortType.Star_DESC, new Comparison<FriendBriefDataItem>(FriendBriefDataItem.CompareToAvatarStarDesc));
            this.sortComparisionMap.Add(FriendSortType.Star_ASC, new Comparison<FriendBriefDataItem>(FriendBriefDataItem.CompareToAvatarStarAsc));
            this.sortComparisionMap.Add(FriendSortType.Combat_DESC, new Comparison<FriendBriefDataItem>(FriendBriefDataItem.CompareToAvatarCombatDesc));
            this.sortComparisionMap.Add(FriendSortType.Combat_ASC, new Comparison<FriendBriefDataItem>(FriendBriefDataItem.CompareToAvatarCombatAsc));
        }

        public bool isHelperFrozen(int uid)
        {
            return (this.GetHelperNextAvaliableTime(uid) > TimeUtil.Now);
        }

        public bool IsMyFriend(int targetUid)
        {
            return this._friendsBriefInfoDict.ContainsKey(targetUid);
        }

        public bool IsOldFriend(int friendUID)
        {
            return Singleton<MiHoYoGameData>.Instance.LocalData.OldFriendUIDSet.Contains(friendUID);
        }

        public bool IsOldRequest(int friendUID)
        {
            return Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet.Contains(friendUID);
        }

        public bool IsRequestAddOnce(int friendUID)
        {
            return this._requestAddPlayerUIDSet.Contains(friendUID);
        }

        public void MarkAllFriendsAsOld()
        {
            this.MarkFriendsAsOld(this.friendsList);
        }

        public void MarkAllRequestsAsOld()
        {
            this.MarkRequestsAsOld(this.askingList);
        }

        public void MarkFriendAsOld(int friendUID)
        {
            Singleton<MiHoYoGameData>.Instance.LocalData.OldFriendUIDSet.Add(friendUID);
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        public void MarkFriendsAsOld(List<FriendBriefDataItem> friends)
        {
            foreach (FriendBriefDataItem item in friends)
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.OldFriendUIDSet.Add(item.uid);
            }
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        public void MarkRequestAsOld(int friendUID)
        {
            Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet.Add(friendUID);
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        public void MarkRequestsAsOld(List<FriendBriefDataItem> requests)
        {
            foreach (FriendBriefDataItem item in requests)
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet.Add(item.uid);
            }
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        private bool OnAddFriendRsp(AddFriendRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                int item = (int) rsp.get_target_uid();
                Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet.Remove(item);
                Singleton<MiHoYoGameData>.Instance.Save();
            }
            return false;
        }

        private bool OnDelFriendNotify(DelFriendNotify rsp)
        {
            int num = (int) rsp.get_target_uid();
            FriendBriefDataItem item = null;
            foreach (FriendBriefDataItem item2 in this.friendsList)
            {
                if (item2.uid == num)
                {
                    item = item2;
                    break;
                }
            }
            if (item != null)
            {
                this.friendsList.Remove(item);
            }
            this._friendsBriefInfoDict.Remove((int) rsp.get_target_uid());
            return false;
        }

        private bool OnDelFriendRsp(DelFriendRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
            }
            return false;
        }

        private bool OnGetAskAddFriendListRsp(GetAskAddFriendListRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                bool flag = this._askingDict.Count <= 0;
                for (int i = 0; i < rsp.get_ask_list().Count; i++)
                {
                    PlayerFriendBriefData briefData = rsp.get_ask_list()[i];
                    FriendBriefDataItem item = new FriendBriefDataItem(briefData);
                    if (this._askingDict.ContainsKey(item.uid))
                    {
                        this.askingList.Remove(this._askingDict[item.uid]);
                        this.askingList.Insert(0, item);
                    }
                    else
                    {
                        this.askingList.Add(item);
                    }
                    this._askingDict[item.uid] = item;
                    this._playerBriefInfoCacheDict[item.uid] = new CacheData<FriendBriefDataItem>(item);
                }
                if (flag)
                {
                    HashSet<int> set = new HashSet<int>();
                    foreach (int num2 in Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet)
                    {
                        if (this._askingDict.ContainsKey(num2))
                        {
                            set.Add(num2);
                        }
                    }
                    if (set.Count > 0)
                    {
                        Singleton<MiHoYoGameData>.Instance.LocalData.OldRequestUIDSet.ExceptWith(set);
                        Singleton<MiHoYoGameData>.Instance.Save();
                    }
                }
            }
            return false;
        }

        private bool OnGetAssistantFrozenListRsp(GetAssistantFrozenListRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (AssistantFrozen frozen in rsp.get_frozen_list())
                {
                    int num = (int) frozen.get_uid();
                    DateTime time = TimeUtil.Now.AddSeconds((double) frozen.get_left_frozen_time());
                    this._helperFrozenMap[num] = time;
                }
            }
            return false;
        }

        private bool OnGetFriendListRsp(GetFriendListRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if (rsp.get_is_whole_dataSpecified() && rsp.get_is_whole_data())
                {
                    this._friendsBriefInfoDict.Clear();
                    this.friendsList.Clear();
                }
                for (int i = 0; i < rsp.get_friend_list().Count; i++)
                {
                    PlayerFriendBriefData briefData = rsp.get_friend_list()[i];
                    FriendBriefDataItem item = new FriendBriefDataItem(briefData);
                    if (this._friendsBriefInfoDict.ContainsKey(item.uid))
                    {
                        this.friendsList.Remove(this._friendsBriefInfoDict[item.uid]);
                        this.friendsList.Insert(0, item);
                    }
                    else
                    {
                        this.friendsList.Add(item);
                    }
                    this._friendsBriefInfoDict[item.uid] = item;
                    this._playerBriefInfoCacheDict[item.uid] = new CacheData<FriendBriefDataItem>(item);
                }
            }
            return false;
        }

        private bool OnGetPlayerDetailDataRsp(GetPlayerDetailDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                PlayerDetailData playerDetailData = rsp.get_detail();
                FriendDetailDataItem item = new FriendDetailDataItem(playerDetailData);
                this._playerDetialInfoCacheDict[(int) playerDetailData.get_uid()] = new CacheData<FriendDetailDataItem>(item);
            }
            return false;
        }

        private bool OnGetRecommandFriendListRsp(GetRecommendFriendListRsp rsp)
        {
            this.recommandedPlayerList.Clear();
            this.helperStrangerList.Clear();
            for (int i = 0; i < rsp.get_recommend_list().Count; i++)
            {
                PlayerFriendBriefData briefData = rsp.get_recommend_list()[i];
                if (!this._friendsBriefInfoDict.ContainsKey((int) briefData.get_uid()))
                {
                    FriendBriefDataItem item = new FriendBriefDataItem(briefData);
                    if (i < MiscData.Config.BasicConfig.RecommendFriendListNum)
                    {
                        this.recommandedPlayerList.Add(item);
                    }
                    else
                    {
                        this.helperStrangerList.Add(item);
                    }
                    this._playerBriefInfoCacheDict[item.uid] = new CacheData<FriendBriefDataItem>(item);
                }
            }
            if (this.helperStrangerList.Count == 0)
            {
                this.helperStrangerList.AddRange(this.recommandedPlayerList);
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x49:
                    return this.OnGetPlayerDetailDataRsp(pkt.getData<GetPlayerDetailDataRsp>());

                case 0x41:
                    return this.OnGetFriendListRsp(pkt.getData<GetFriendListRsp>());

                case 0x43:
                    return this.OnAddFriendRsp(pkt.getData<AddFriendRsp>());

                case 0x47:
                    return this.OnGetAskAddFriendListRsp(pkt.getData<GetAskAddFriendListRsp>());

                case 0x4d:
                    return this.OnGetRecommandFriendListRsp(pkt.getData<GetRecommendFriendListRsp>());

                case 80:
                    return this.OnDelFriendNotify(pkt.getData<DelFriendNotify>());

                case 0x65:
                    return this.OnGetAssistantFrozenListRsp(pkt.getData<GetAssistantFrozenListRsp>());
            }
            return false;
        }

        public void RecordRequestAddFriend(int friendUID)
        {
            this._requestAddPlayerUIDSet.Add(friendUID);
        }

        public void RemoveFriendInfo(FriendOverviewPageContext.FriendTab friendType, int targetUid)
        {
            <RemoveFriendInfo>c__AnonStoreyCA yca = new <RemoveFriendInfo>c__AnonStoreyCA {
                targetUid = targetUid
            };
            switch (friendType)
            {
                case FriendOverviewPageContext.FriendTab.FriendListTab:
                    this._friendsBriefInfoDict.Remove(yca.targetUid);
                    this.friendsList.RemoveAll(new Predicate<FriendBriefDataItem>(yca.<>m__C9));
                    break;

                case FriendOverviewPageContext.FriendTab.AddFriendTab:
                    this.recommandedPlayerList.RemoveAll(new Predicate<FriendBriefDataItem>(yca.<>m__CA));
                    break;

                case FriendOverviewPageContext.FriendTab.RequestListTab:
                    this._askingDict.Remove(yca.targetUid);
                    this.askingList.RemoveAll(new Predicate<FriendBriefDataItem>(yca.<>m__CB));
                    break;
            }
        }

        public FriendBriefDataItem TryGetFriendBriefData(int targetUid)
        {
            if (this._playerBriefInfoCacheDict.ContainsKey(targetUid) && this._playerBriefInfoCacheDict[targetUid].CacheValid)
            {
                return this._playerBriefInfoCacheDict[targetUid].Value;
            }
            foreach (FriendBriefDataItem item in this.friendsList)
            {
                if (item.uid == targetUid)
                {
                    return item;
                }
            }
            return null;
        }

        public FriendDetailDataItem TryGetFriendDetailData(int targetUid)
        {
            if (this._playerDetialInfoCacheDict.ContainsKey(targetUid) && this._playerDetialInfoCacheDict[targetUid].CacheValid)
            {
                return this._playerDetialInfoCacheDict[targetUid].Value;
            }
            return null;
        }

        public FriendDetailDataItem TryGetFriendDetailInfo(int uid)
        {
            CacheData<FriendDetailDataItem> data;
            this._playerDetialInfoCacheDict.TryGetValue(uid, out data);
            if ((data != null) && data.CacheValid)
            {
                return data.Value;
            }
            return null;
        }

        public string TryGetPlayerNickName(int targetUid)
        {
            <TryGetPlayerNickName>c__AnonStoreyCB ycb = new <TryGetPlayerNickName>c__AnonStoreyCB {
                targetUid = targetUid
            };
            if (this._playerBriefInfoCacheDict.ContainsKey(ycb.targetUid) && this._playerBriefInfoCacheDict[ycb.targetUid].CacheValid)
            {
                return this._playerBriefInfoCacheDict[ycb.targetUid].Value.nickName;
            }
            if (this._playerDetialInfoCacheDict.ContainsKey(ycb.targetUid) && this._playerDetialInfoCacheDict[ycb.targetUid].CacheValid)
            {
                return this._playerDetialInfoCacheDict[ycb.targetUid].Value.nickName;
            }
            FriendBriefDataItem item = this.recommandedPlayerList.Find(new Predicate<FriendBriefDataItem>(ycb.<>m__CC));
            if (item != null)
            {
                return item.nickName;
            }
            return ycb.targetUid.ToString();
        }

        [CompilerGenerated]
        private sealed class <RemoveFriendInfo>c__AnonStoreyCA
        {
            internal int targetUid;

            internal bool <>m__C9(FriendBriefDataItem x)
            {
                return (x.uid == this.targetUid);
            }

            internal bool <>m__CA(FriendBriefDataItem x)
            {
                return (x.uid == this.targetUid);
            }

            internal bool <>m__CB(FriendBriefDataItem x)
            {
                return (x.uid == this.targetUid);
            }
        }

        [CompilerGenerated]
        private sealed class <TryGetPlayerNickName>c__AnonStoreyCB
        {
            internal int targetUid;

            internal bool <>m__CC(FriendBriefDataItem x)
            {
                return (x.uid == this.targetUid);
            }
        }

        public enum FriendSortType
        {
            Friend_NEW,
            Request_NEW,
            Level_DESC,
            Level_ASC,
            Star_DESC,
            Star_ASC,
            Combat_DESC,
            Combat_ASC
        }
    }
}

