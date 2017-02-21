namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class EndlessModule : BaseModule
    {
        private List<int> _demoteRankList;
        private Dictionary<int, EndlessItemFrozenInfo> _itemFrozenInfoDict;
        private DateTime _lastTimeGetTopGroupData = DateTime.Now.AddDays(-1.0);
        private List<int> _normalRankList;
        private Dictionary<int, EndlessItem> _playerItemDict;
        private List<int> _playerRankList;
        private List<int> _promoteRankList;
        private List<EndlessToolDataItem> _waitToEffectToolDataList;
        [CompilerGenerated]
        private static Comparison<EndlessItem> <>f__am$cache10;
        private Dictionary<int, EndlessAvatarHp> avatarHPDict;
        public GetEndlessDataRsp endlessData;
        private Dictionary<int, EndlessPlayerData> endlessPlayerDataDict;
        private const int INVISIBLE_ITEM_ID = 0x1117f;
        public EndlessToolDataItem justBurstBombData;
        private Dictionary<int, PlayerFriendBriefData> playerDataDict;
        public GetEndlessTopGroupRsp topGroupData;
        private int UID;
        public Stack<EndlessWarInfo> warInfoList;

        private EndlessModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this.UID = Singleton<PlayerModule>.Instance.playerData.userId;
            this.endlessData = null;
            this.endlessPlayerDataDict = new Dictionary<int, EndlessPlayerData>();
            this.playerDataDict = new Dictionary<int, PlayerFriendBriefData>();
            this.warInfoList = new Stack<EndlessWarInfo>();
            this._playerItemDict = new Dictionary<int, EndlessItem>();
            this._itemFrozenInfoDict = new Dictionary<int, EndlessItemFrozenInfo>();
            this._promoteRankList = new List<int>();
            this._normalRankList = new List<int>();
            this._demoteRankList = new List<int>();
            this._waitToEffectToolDataList = new List<EndlessToolDataItem>();
            this._playerRankList = new List<int>();
            this.InitAvatarHPDict();
        }

        public bool CanRequestTopGroupInfo()
        {
            if ((this.endlessData.get_cur_top_group_level() < this.TopGroupLevel) || (this.endlessData.get_group_level() == this.TopGroupLevel))
            {
                return false;
            }
            return ((this.topGroupData == null) || (this._lastTimeGetTopGroupData.AddHours(1.0) < TimeUtil.Now));
        }

        public bool CanSeeTopGroupInfo()
        {
            return ((this.endlessData.get_cur_top_group_level() == this.TopGroupLevel) && (this.endlessData.get_group_level() != this.TopGroupLevel));
        }

        public void CheckAllAvatarHPChanged()
        {
            foreach (EndlessAvatarHp hp in this.avatarHPDict.Values)
            {
                this.CheckAvatarHPChanged(hp);
            }
        }

        public DateTime CheckAvatarHPChanged(EndlessAvatarHp avatarHPData)
        {
            if ((avatarHPData.get_hp_percent() >= 100) || !avatarHPData.get_next_recover_timeSpecified())
            {
                return DateTime.MinValue;
            }
            DateTime dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(avatarHPData.get_next_recover_time());
            if (TimeUtil.Now >= dateTimeFromTimeStamp)
            {
                Singleton<NetworkManager>.Instance.RequestEndlessAvatarHp();
            }
            return dateTimeFromTimeStamp;
        }

        private void ClearData()
        {
            this.endlessPlayerDataDict.Clear();
            this.playerDataDict.Clear();
            this.warInfoList.Clear();
            this._playerItemDict.Clear();
            this._itemFrozenInfoDict.Clear();
            this._promoteRankList.Clear();
            this._normalRankList.Clear();
            this._demoteRankList.Clear();
            this._waitToEffectToolDataList.Clear();
            this._playerRankList.Clear();
            this.endlessData = null;
        }

        public List<EndlessToolDataItem> GetAppliedToolDataList()
        {
            this._waitToEffectToolDataList.Clear();
            List<EndlessWaitEffectItem> list = this.endlessPlayerDataDict[this.UID].get_wait_effect_item_list();
            List<EndlessWaitBurstBomb> list2 = this.endlessPlayerDataDict[this.UID].get_wait_burst_bomb_list();
            for (int i = 0; i < list.Count; i++)
            {
                EndlessWaitEffectItem item = list[i];
                EndlessToolDataItem item2 = new EndlessToolDataItem((int) item.get_item_id(), 1);
                this._waitToEffectToolDataList.Add(item2);
            }
            foreach (EndlessWaitBurstBomb bomb in list2)
            {
                EndlessToolDataItem item3 = new EndlessToolDataItem((int) bomb.get_item_id(), 1);
                this._waitToEffectToolDataList.Add(item3);
            }
            if (this.SelfInvisible())
            {
                this._waitToEffectToolDataList.Add(new EndlessToolDataItem(0x1117f, 1));
            }
            return this._waitToEffectToolDataList;
        }

        public int GetAvatarRemainHP(int avatarId)
        {
            EndlessAvatarHp hp;
            this.avatarHPDict.TryGetValue(avatarId, out hp);
            if (hp == null)
            {
                return -1;
            }
            return (int) hp.get_hp_percent();
        }

        public List<int> GetDemoteRank(EndlessMainPageContext.ViewStatus viewStatus = 0)
        {
            if (viewStatus == EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
            {
                this.SetupRankList();
            }
            else if (viewStatus == EndlessMainPageContext.ViewStatus.ShowTopGroup)
            {
                this.SetupTopGroupRankList();
            }
            return this._demoteRankList;
        }

        public EndlessActivityStatus GetEndlessActivityStatus()
        {
            if (TimeUtil.Now < this.BeginTime)
            {
                return EndlessActivityStatus.WaitToStart;
            }
            if (TimeUtil.Now < this.EndTime)
            {
                return EndlessActivityStatus.InProgress;
            }
            if (TimeUtil.Now < this.SettlementTime)
            {
                return EndlessActivityStatus.WaitToSettlement;
            }
            if (TimeUtil.Now >= this.SettlementTime)
            {
                Singleton<NetworkManager>.Instance.RequestEndlessData();
                return EndlessActivityStatus.None;
            }
            return EndlessActivityStatus.None;
        }

        public EndlessAvatarHp GetEndlessAvatarHPData(int avatarId)
        {
            EndlessAvatarHp hp;
            this.avatarHPDict.TryGetValue(avatarId, out hp);
            if (hp == null)
            {
                return null;
            }
            return hp;
        }

        public DateTime GetFrozenEndTime(int targetUid)
        {
            EndlessItemFrozenInfo info;
            this._itemFrozenInfoDict.TryGetValue(targetUid, out info);
            if (info == null)
            {
                return TimeUtil.Now.AddDays(-1.0);
            }
            return Miscs.GetDateTimeFromTimeStamp(info.get_frozen_time());
        }

        public List<int> GetNormalRank(EndlessMainPageContext.ViewStatus viewStatus = 0)
        {
            if (viewStatus == EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
            {
                this.SetupRankList();
            }
            else if (viewStatus == EndlessMainPageContext.ViewStatus.ShowTopGroup)
            {
                this.SetupTopGroupRankList();
            }
            return this._normalRankList;
        }

        public PlayerFriendBriefData GetPlayerBriefData(int playerUid)
        {
            if (!this.playerDataDict.ContainsKey(playerUid))
            {
                PlayerFriendBriefData data = new PlayerFriendBriefData();
                object[] replaceParams = new object[] { playerUid };
                data.set_nickname(LocalizationGeneralLogic.GetText("Menu_DefaultNickname", replaceParams));
                this.playerDataDict.Add(playerUid, data);
            }
            return this.playerDataDict[playerUid];
        }

        public EndlessPlayerData GetPlayerEndlessData(int playerUid)
        {
            return this.endlessPlayerDataDict[playerUid];
        }

        public List<EndlessItem> GetPlayerEndlessItemList()
        {
            List<EndlessItem> list = new List<EndlessItem>(this._playerItemDict.Values);
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = (left, right) => (int) (left.get_item_id() - right.get_item_id());
            }
            list.Sort(<>f__am$cache10);
            return list;
        }

        public List<int> GetPromoteRank(EndlessMainPageContext.ViewStatus viewStatus = 0)
        {
            if (viewStatus == EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
            {
                this.SetupRankList();
            }
            else if (viewStatus == EndlessMainPageContext.ViewStatus.ShowTopGroup)
            {
                this.SetupTopGroupRankList();
            }
            return this._promoteRankList;
        }

        public List<int> GetRankListSorted()
        {
            this._playerRankList.Clear();
            foreach (EndlessPlayerData data in this.endlessPlayerDataDict.Values)
            {
                this._playerRankList.Add((int) data.get_uid());
            }
            this._playerRankList.Sort(new Comparison<int>(this.GroupRankSortComparor));
            return this._playerRankList;
        }

        public EndlessPlayerData GetSelfEndlessData()
        {
            return this.endlessPlayerDataDict[this.UID];
        }

        public PlayerFriendBriefData GetTopGroupPlayerBriefData(int playerUid)
        {
            foreach (PlayerFriendBriefData data in this.topGroupData.get_brief_data_list())
            {
                if (data.get_uid() == playerUid)
                {
                    return data;
                }
            }
            PlayerFriendBriefData data2 = new PlayerFriendBriefData();
            object[] replaceParams = new object[] { playerUid };
            data2.set_nickname(LocalizationGeneralLogic.GetText("Menu_DefaultNickname", replaceParams));
            return data2;
        }

        public EndlessPlayerData GetTopGroupPlayerEndlessData(int playerUid)
        {
            foreach (EndlessPlayerData data in this.topGroupData.get_endless_data_list())
            {
                if (data.get_uid() == playerUid)
                {
                    return data;
                }
            }
            return null;
        }

        private int GroupRankSortComparor(int palyerAUid, int PlayerBUid)
        {
            EndlessPlayerData data = this.endlessPlayerDataDict[palyerAUid];
            EndlessPlayerData data2 = this.endlessPlayerDataDict[PlayerBUid];
            if (data.get_progress() != data2.get_progress())
            {
                return (int) (data2.get_progress() - data.get_progress());
            }
            if (data.get_progress_time() != data2.get_progress_time())
            {
                return (int) (data.get_progress_time() - data2.get_progress_time());
            }
            return (int) (data.get_uid() - data2.get_uid());
        }

        private void InitAvatarHPDict()
        {
            this.avatarHPDict = new Dictionary<int, EndlessAvatarHp>();
            foreach (AvatarDataItem item in Singleton<AvatarModule>.Instance.UserAvatarList)
            {
                EndlessAvatarHp hp = new EndlessAvatarHp();
                hp.set_avatar_id((uint) item.avatarID);
                hp.set_hp_percent(100);
                hp.set_next_recover_time((uint) TimeUtil.Now.AddDays(1.0).Ticks);
                hp.set_is_die(false);
                this.avatarHPDict[item.avatarID] = hp;
            }
        }

        private bool OnEndlessItemDataUpdateNotify(EndlessItemDataUpdateNotify rsp)
        {
            foreach (EndlessItem item in rsp.get_item_list())
            {
                if (EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey((int) item.get_item_id()) != null)
                {
                    if (item.get_num() < 1)
                    {
                        this._playerItemDict.Remove((int) item.get_item_id());
                    }
                    else
                    {
                        this._playerItemDict[(int) item.get_item_id()] = item;
                    }
                }
            }
            return false;
        }

        private bool OnEndlessPlayerDataUpdateNotify(EndlessPlayerDataUpdateNotify rsp)
        {
            if ((rsp.get_player_data().get_uid() != this.UID) || !rsp.get_player_data().get_is_just_bomb_burstSpecified())
            {
                goto Label_00B5;
            }
            EndlessPlayerData selfEndlessData = this.GetSelfEndlessData();
            EndlessWaitBurstBomb bombData = null;
            <OnEndlessPlayerDataUpdateNotify>c__AnonStoreyC9 yc = new <OnEndlessPlayerDataUpdateNotify>c__AnonStoreyC9();
            using (List<EndlessWaitBurstBomb>.Enumerator enumerator = selfEndlessData.get_wait_burst_bomb_list().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yc.bombData = enumerator.Current;
                    if (rsp.get_player_data().get_wait_burst_bomb_list().FindLast(new Predicate<EndlessWaitBurstBomb>(yc.<>m__C7)) == null)
                    {
                        bombData = yc.bombData;
                        goto Label_00A3;
                    }
                }
            }
        Label_00A3:
            this.justBurstBombData = new EndlessToolDataItem((int) bombData.get_item_id(), 1);
        Label_00B5:
            this.endlessPlayerDataDict[(int) rsp.get_player_data().get_uid()] = rsp.get_player_data();
            foreach (PlayerFriendBriefData data2 in rsp.get_brief_data_list())
            {
                this.playerDataDict[(int) data2.get_uid()] = data2;
            }
            return false;
        }

        private bool OnEndlessStageBeginRsp(EndlessStageBeginRsp rsp)
        {
            if ((rsp.get_retcode() == null) && rsp.get_progressSpecified())
            {
                this.endlessPlayerDataDict[this.UID].set_progress(rsp.get_progress());
            }
            return false;
        }

        private bool OnEndlessStageEndRsp(EndlessStageEndRsp rsp)
        {
            if ((rsp.get_retcode() == null) && rsp.get_progressSpecified())
            {
                this.endlessPlayerDataDict[this.UID].set_progress(rsp.get_progress());
            }
            return false;
        }

        private bool OnEndlessWarInfoNotify(EndlessWarInfoNotify rsp)
        {
            this.warInfoList.Push(rsp.get_war_info());
            return false;
        }

        private bool OnGetEndlessAvatarHpRsp(GetEndlessAvatarHpRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                Dictionary<int, EndlessAvatarHp> dictionary = new Dictionary<int, EndlessAvatarHp>();
                foreach (EndlessAvatarHp hp in rsp.get_avatar_hp_list())
                {
                    dictionary[(int) hp.get_avatar_id()] = hp;
                }
                foreach (AvatarDataItem item in Singleton<AvatarModule>.Instance.UserAvatarList)
                {
                    int avatarID = item.avatarID;
                    if (dictionary.ContainsKey(avatarID))
                    {
                        this.avatarHPDict[avatarID] = dictionary[avatarID];
                    }
                    else
                    {
                        EndlessAvatarHp hp2 = this.avatarHPDict[avatarID];
                        hp2.set_hp_percent(100);
                        hp2.set_is_die(false);
                    }
                }
            }
            return false;
        }

        private bool OnGetEndlessDataRsp(GetEndlessDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.ClearData();
                this.endlessData = rsp;
                foreach (EndlessPlayerData data in rsp.get_endless_data_list())
                {
                    if ((data.get_uid() != this.UID) || !data.get_is_just_bomb_burstSpecified())
                    {
                        goto Label_00D9;
                    }
                    EndlessPlayerData selfEndlessData = this.GetSelfEndlessData();
                    EndlessWaitBurstBomb bombData = null;
                    <OnGetEndlessDataRsp>c__AnonStoreyC8 yc = new <OnGetEndlessDataRsp>c__AnonStoreyC8();
                    using (List<EndlessWaitBurstBomb>.Enumerator enumerator2 = selfEndlessData.get_wait_burst_bomb_list().GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            yc.bombData = enumerator2.Current;
                            if (data.get_wait_burst_bomb_list().FindLast(new Predicate<EndlessWaitBurstBomb>(yc.<>m__C6)) == null)
                            {
                                bombData = yc.bombData;
                                goto Label_00C7;
                            }
                        }
                    }
                Label_00C7:
                    this.justBurstBombData = new EndlessToolDataItem((int) bombData.get_item_id(), 1);
                Label_00D9:
                    this.endlessPlayerDataDict[(int) data.get_uid()] = data;
                }
                foreach (PlayerFriendBriefData data3 in rsp.get_brief_data_list())
                {
                    this.playerDataDict[(int) data3.get_uid()] = data3;
                }
                foreach (EndlessWarInfo info in rsp.get_war_info_list())
                {
                    this.warInfoList.Push(info);
                }
                foreach (EndlessItemFrozenInfo info2 in rsp.get_item_frozen_list())
                {
                    this._itemFrozenInfoDict[(int) info2.get_target_uid()] = info2;
                }
                this._playerItemDict.Clear();
                foreach (EndlessItem item in rsp.get_item_list())
                {
                    if (EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey((int) item.get_item_id()) != null)
                    {
                        this._playerItemDict[(int) item.get_item_id()] = item;
                    }
                }
            }
            return false;
        }

        private bool OnGetEndlessTopGroupRsp(GetEndlessTopGroupRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this._lastTimeGetTopGroupData = TimeUtil.Now;
                this.topGroupData = rsp;
            }
            return false;
        }

        private bool OnGetLastEndlessRewardDataRsp(GetLastEndlessRewardDataRsp rsp)
        {
            if ((rsp.get_retcode() == null) && (rsp.get_reward_list().Count > 0))
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData = rsp;
                Singleton<MiHoYoGameData>.Instance.Save();
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 140:
                    return this.OnGetEndlessDataRsp(pkt.getData<GetEndlessDataRsp>());

                case 150:
                    return this.OnGetEndlessAvatarHpRsp(pkt.getData<GetEndlessAvatarHpRsp>());

                case 0x97:
                    return this.OnEndlessPlayerDataUpdateNotify(pkt.getData<EndlessPlayerDataUpdateNotify>());

                case 0x8e:
                    return this.OnEndlessStageBeginRsp(pkt.getData<EndlessStageBeginRsp>());

                case 0x90:
                    return this.OnEndlessStageEndRsp(pkt.getData<EndlessStageEndRsp>());

                case 0x92:
                    return this.OnGetLastEndlessRewardDataRsp(pkt.getData<GetLastEndlessRewardDataRsp>());

                case 0x99:
                    return this.OnEndlessWarInfoNotify(pkt.getData<EndlessWarInfoNotify>());

                case 0x94:
                    return this.OnUseEndlessItemRsp(pkt.getData<UseEndlessItemRsp>());

                case 0x98:
                    return this.OnEndlessItemDataUpdateNotify(pkt.getData<EndlessItemDataUpdateNotify>());

                case 220:
                    return this.OnGetEndlessTopGroupRsp(pkt.getData<GetEndlessTopGroupRsp>());
            }
            return false;
        }

        private bool OnUseEndlessItemRsp(UseEndlessItemRsp rsp)
        {
            if ((rsp.get_retcode() == null) && rsp.get_target_uidSpecified())
            {
                EndlessItemFrozenInfo info = new EndlessItemFrozenInfo();
                info.set_target_uid(rsp.get_target_uid());
                info.set_frozen_time(rsp.get_item_frozen_time());
                this._itemFrozenInfoDict[(int) rsp.get_target_uid()] = info;
            }
            return false;
        }

        public bool PlayerInvisible(int uid)
        {
            EndlessPlayerData data = this.endlessPlayerDataDict[uid];
            return (data.get_hidden_expire_timeSpecified() && (Miscs.GetDateTimeFromTimeStamp(data.get_hidden_expire_time()) > TimeUtil.Now));
        }

        public bool SelfInvisible()
        {
            return this.PlayerInvisible(this.UID);
        }

        public void SetAvatarHP(int avatarHPPercent, int avatarId)
        {
            EndlessAvatarHp hp;
            this.avatarHPDict.TryGetValue(avatarId, out hp);
            if (hp != null)
            {
                hp.set_hp_percent((uint) avatarHPPercent);
            }
        }

        private void SetupRankList()
        {
            EndlessGroupMetaData endlessGroupMetaDataByKey = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey((int) this.endlessData.get_group_level());
            int num = 0;
            int num2 = 0;
            List<int> rankListSorted = this.GetRankListSorted();
            num2 = 0;
            while ((num + num2) < rankListSorted.Count)
            {
                if ((this.GetPlayerEndlessData(rankListSorted[num + num2]).get_progress() <= 0) || ((num + num2) >= endlessGroupMetaDataByKey.promoteRank))
                {
                    break;
                }
                num2++;
            }
            this._promoteRankList = rankListSorted.GetRange(num, num2);
            num += num2;
            num2 = 0;
            while ((num + num2) < rankListSorted.Count)
            {
                if (((endlessGroupMetaDataByKey.groupLevel > 1) && (this.GetPlayerEndlessData(rankListSorted[num + num2]).get_progress() <= 0)) || ((num + num2) >= (endlessGroupMetaDataByKey.demoteRank - 1)))
                {
                    break;
                }
                num2++;
            }
            this._normalRankList = rankListSorted.GetRange(num, num2);
            num += num2;
            num2 = 0;
            while ((num + num2) < rankListSorted.Count)
            {
                num2++;
            }
            this._demoteRankList = rankListSorted.GetRange(num, num2);
        }

        private void SetupTopGroupRankList()
        {
            if (this.CanSeeTopGroupInfo())
            {
                EndlessGroupMetaData endlessGroupMetaDataByKey = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey(this.TopGroupLevel);
                int num = 0;
                int num2 = 0;
                List<EndlessPlayerData> list = this.topGroupData.get_endless_data_list();
                list.Sort(new Comparison<EndlessPlayerData>(this.TopGroupRankSortComparor));
                num2 = 0;
                while ((num + num2) < list.Count)
                {
                    if ((list[num + num2].get_progress() <= 0) || ((num + num2) >= endlessGroupMetaDataByKey.promoteRank))
                    {
                        break;
                    }
                    num2++;
                }
                this._promoteRankList.Clear();
                for (int i = 0; i < num2; i++)
                {
                    this._promoteRankList.Add((int) list[num + i].get_uid());
                }
                num += num2;
                num2 = 0;
                while ((num + num2) < list.Count)
                {
                    if (((endlessGroupMetaDataByKey.groupLevel > 1) && (list[num + num2].get_progress() <= 0)) || ((num + num2) >= (endlessGroupMetaDataByKey.demoteRank - 1)))
                    {
                        break;
                    }
                    num2++;
                }
                this._normalRankList.Clear();
                for (int j = 0; j < num2; j++)
                {
                    this._normalRankList.Add((int) list[num + j].get_uid());
                }
                num += num2;
                num2 = 0;
                while ((num + num2) < list.Count)
                {
                    num2++;
                }
                this._demoteRankList.Clear();
                for (int k = 0; k < num2; k++)
                {
                    this._demoteRankList.Add((int) list[num + k].get_uid());
                }
            }
        }

        private int TopGroupRankSortComparor(EndlessPlayerData playerA, EndlessPlayerData playerB)
        {
            if (playerA.get_progress() != playerB.get_progress())
            {
                return (int) (playerB.get_progress() - playerA.get_progress());
            }
            if (playerA.get_progress_time() != playerB.get_progress_time())
            {
                return (int) (playerA.get_progress_time() - playerB.get_progress_time());
            }
            return (int) (playerA.get_uid() - playerB.get_uid());
        }

        public DateTime BeginTime
        {
            get
            {
                return Miscs.GetDateTimeFromTimeStamp(this.endlessData.get_begin_time());
            }
        }

        public int CurrentFinishProgress
        {
            get
            {
                return (int) this.endlessPlayerDataDict[this.UID].get_progress();
            }
        }

        public int currentGroupLevel
        {
            get
            {
                return (int) this.endlessData.get_group_level();
            }
        }

        public int CurrentRank
        {
            get
            {
                this.SetupRankList();
                List<int> rankListSorted = this.GetRankListSorted();
                for (int i = 0; i < rankListSorted.Count; i++)
                {
                    if (this.UID == rankListSorted[i])
                    {
                        return (i + 1);
                    }
                }
                return 100;
            }
        }

        public RankUpDownStatus CurrentRankStatus
        {
            get
            {
                this.SetupRankList();
                foreach (int num in this._promoteRankList)
                {
                    if (this.UID == num)
                    {
                        return RankUpDownStatus.Up;
                    }
                }
                foreach (int num2 in this._normalRankList)
                {
                    if (this.UID == num2)
                    {
                        return RankUpDownStatus.Stay;
                    }
                }
                foreach (int num3 in this._demoteRankList)
                {
                    if (this.UID == num3)
                    {
                        return RankUpDownStatus.Down;
                    }
                }
                return RankUpDownStatus.Stay;
            }
        }

        public int CurrentRewardID
        {
            get
            {
                this.SetupRankList();
                EndlessGroupMetaData endlessGroupMetaDataByKey = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey((int) this.endlessData.get_group_level());
                foreach (int num in this._promoteRankList)
                {
                    if (this.UID == num)
                    {
                        return endlessGroupMetaDataByKey.prototeRewardID;
                    }
                }
                foreach (int num2 in this._normalRankList)
                {
                    if (this.UID == num2)
                    {
                        return endlessGroupMetaDataByKey.normalRewardID;
                    }
                }
                foreach (int num3 in this._demoteRankList)
                {
                    if (this.UID == num3)
                    {
                        return endlessGroupMetaDataByKey.demoteRewardID;
                    }
                }
                return -1;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return Miscs.GetDateTimeFromTimeStamp(this.endlessData.get_end_time());
            }
        }

        public int maxLevelEverReach
        {
            get
            {
                return (int) this.endlessPlayerDataDict[this.UID].get_max_progress();
            }
        }

        public int randomSeed
        {
            get
            {
                if (this.endlessData.get_random_seedSpecified())
                {
                    return (int) this.endlessData.get_random_seed();
                }
                return 0;
            }
        }

        public DateTime SettlementTime
        {
            get
            {
                return Miscs.GetDateTimeFromTimeStamp(this.endlessData.get_close_time());
            }
        }

        public int TopGroupLevel
        {
            get
            {
                return EndlessGroupMetaDataReader.GetItemList().Count;
            }
        }

        [CompilerGenerated]
        private sealed class <OnEndlessPlayerDataUpdateNotify>c__AnonStoreyC9
        {
            internal EndlessWaitBurstBomb bombData;

            internal bool <>m__C7(EndlessWaitBurstBomb data)
            {
                return (data.get_item_id() == this.bombData.get_item_id());
            }
        }

        [CompilerGenerated]
        private sealed class <OnGetEndlessDataRsp>c__AnonStoreyC8
        {
            internal EndlessWaitBurstBomb bombData;

            internal bool <>m__C6(EndlessWaitBurstBomb data)
            {
                return (data.get_item_id() == this.bombData.get_item_id());
            }
        }
    }
}

