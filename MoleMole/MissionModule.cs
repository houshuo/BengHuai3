namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class MissionModule : BaseModule
    {
        private int _enemyKilledCount;
        private Dictionary<int, MissionDataItem> _missionDict;
        private Dictionary<uint, int> _monsterKilledByAnimEventIDCount;
        private Dictionary<string, int> _monsterKilledByAttackCategoryTagCount;
        private Dictionary<string, int> _monsterWithCategoryKilledCount;
        private Dictionary<uint, int> _triggerAbilityActionCount;
        private Dictionary<uint, int> _uniqueMonsterKilledCount;
        [CompilerGenerated]
        private static Predicate<MissionDataItem> <>f__am$cache9;
        public bool missionDataReceived;
        private Dictionary<int, int> monsterKilledCount;

        public MissionModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._missionDict = new Dictionary<int, MissionDataItem>();
            this.monsterKilledCount = new Dictionary<int, int>();
            this._uniqueMonsterKilledCount = new Dictionary<uint, int>();
            this._monsterWithCategoryKilledCount = new Dictionary<string, int>();
            this._monsterKilledByAnimEventIDCount = new Dictionary<uint, int>();
            this._monsterKilledByAttackCategoryTagCount = new Dictionary<string, int>();
            this._triggerAbilityActionCount = new Dictionary<uint, int>();
            this.missionDataReceived = false;
        }

        private void FlushKillAnyEnemyProgressToServer()
        {
            foreach (MissionDataItem item in this._missionDict.Values)
            {
                if (((item.metaData != null) && (item.status == 2)) && (item.metaData.finishWay == 8))
                {
                    Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(8, 0, string.Empty, (uint) this._enemyKilledCount);
                    break;
                }
            }
            this._enemyKilledCount = 0;
        }

        private void FlushKillByAttackCategoryTagProgressToServer()
        {
            foreach (string str in this._monsterKilledByAttackCategoryTagCount.Keys)
            {
                foreach (MissionDataItem item in this._missionDict.Values)
                {
                    if (((item.metaData != null) && (item.status == 2)) && ((item.metaData.finishWay == 0x17) && (item.metaData.finishParaStr == str)))
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(0x17, 0, str, this._monsterKilledByAttackCategoryTagCount[str]);
                        break;
                    }
                }
            }
            this._monsterKilledByAttackCategoryTagCount.Clear();
        }

        private void FlushKillMonsterWithCategoryProgressToServer()
        {
            foreach (string str in this._monsterWithCategoryKilledCount.Keys)
            {
                foreach (MissionDataItem item in this._missionDict.Values)
                {
                    if (((item.metaData != null) && (item.status == 2)) && ((item.metaData.finishWay == 9) && (item.metaData.finishParaStr == str)))
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(9, 0, str, this._monsterWithCategoryKilledCount[str]);
                        break;
                    }
                }
            }
            this._monsterWithCategoryKilledCount.Clear();
        }

        private void FlushKillWithAnimEventIDProgressToServer()
        {
            foreach (uint num in this._monsterKilledByAnimEventIDCount.Keys)
            {
                foreach (MissionDataItem item in this._missionDict.Values)
                {
                    if (((item.metaData != null) && (item.status == 2)) && ((item.metaData.finishWay == 11) && (item.metaData.finishParaInt == num)))
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(11, num, string.Empty, this._monsterKilledByAnimEventIDCount[num]);
                        break;
                    }
                }
            }
            this._monsterKilledByAnimEventIDCount.Clear();
        }

        public void FlushMissionDataToServer()
        {
            this.FlushMonsterMissionProgressToServer();
            this.FlushUniqueMonsterMissionProgressToServer();
            this.FlushKillAnyEnemyProgressToServer();
            this.FlushKillMonsterWithCategoryProgressToServer();
            this.FlushTriggerAbilityActionProgressToServer();
            this.FlushKillWithAnimEventIDProgressToServer();
            this.FlushKillByAttackCategoryTagProgressToServer();
        }

        private void FlushMonsterMissionProgressToServer()
        {
            foreach (int num in this.monsterKilledCount.Keys)
            {
                foreach (MissionDataItem item in this._missionDict.Values)
                {
                    if (((item.metaData != null) && (item.status == 2)) && ((item.metaData.finishWay == 6) && (item.metaData.finishParaInt == num)))
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(6, (uint) num, string.Empty, this.monsterKilledCount[num]);
                        break;
                    }
                }
            }
            this.monsterKilledCount.Clear();
        }

        private void FlushTriggerAbilityActionProgressToServer()
        {
            foreach (uint num in this._triggerAbilityActionCount.Keys)
            {
                foreach (MissionDataItem item in this._missionDict.Values)
                {
                    if (((item.metaData != null) && (item.status == 2)) && ((item.metaData.finishWay == 10) && (item.metaData.finishParaInt == num)))
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(10, num, string.Empty, this._triggerAbilityActionCount[num]);
                        break;
                    }
                }
            }
            this._triggerAbilityActionCount.Clear();
        }

        private void FlushUniqueMonsterMissionProgressToServer()
        {
            foreach (uint num in this._uniqueMonsterKilledCount.Keys)
            {
                foreach (MissionDataItem item in this._missionDict.Values)
                {
                    if (((item.metaData != null) && (item.status == 2)) && ((item.metaData.finishWay == 7) && (item.metaData.finishParaInt == num)))
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(7, num, string.Empty, this._uniqueMonsterKilledCount[num]);
                        break;
                    }
                }
            }
            this._uniqueMonsterKilledCount.Clear();
        }

        public List<MissionDataItem> GetAchievements()
        {
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = delegate (MissionDataItem x) {
                    LinearMissionData linearMissionDataByKey = LinearMissionDataReader.GetLinearMissionDataByKey(x.id);
                    if (linearMissionDataByKey == null)
                    {
                        return false;
                    }
                    if (linearMissionDataByKey.IsAchievement == 0)
                    {
                        return false;
                    }
                    return true;
                };
            }
            return Enumerable.ToList<MissionDataItem>(this._missionDict.Values).FindAll(<>f__am$cache9);
        }

        public MissionDataItem GetMissionDataItem(int missionID)
        {
            MissionDataItem item;
            this._missionDict.TryGetValue(missionID, out item);
            return item;
        }

        public Dictionary<int, MissionDataItem> GetMissionDict()
        {
            return this._missionDict;
        }

        public bool NeedNotify()
        {
            foreach (MissionDataItem item in this._missionDict.Values)
            {
                if (item.status == 3)
                {
                    LinearMissionData linearMissionDataByKey = LinearMissionDataReader.GetLinearMissionDataByKey(item.id);
                    if ((linearMissionDataByKey == null) || (linearMissionDataByKey.IsAchievement == 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool OnDelMissionNotify(DelMissionNotify rsp)
        {
            this._missionDict.Remove((int) rsp.get_mission_id());
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionDeleted, rsp.get_mission_id()));
            return false;
        }

        private bool OnGetMissionDataRsp(GetMissionDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (Mission mission in rsp.get_mission_list())
                {
                    if (this._missionDict.ContainsKey((int) mission.get_mission_id()))
                    {
                        MissionDataItem item = this._missionDict[(int) mission.get_mission_id()];
                        if (!item.IsMissionEqual(mission))
                        {
                            item.UpdateFromMission(mission);
                            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionUpdated, mission.get_mission_id()));
                        }
                    }
                    else
                    {
                        MissionDataItem item2 = new MissionDataItem(mission);
                        this._missionDict[(int) mission.get_mission_id()] = item2;
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionUpdated, mission.get_mission_id()));
                    }
                    if ((((mission.get_status() == 2) || (mission.get_status() == 3)) || (mission.get_status() == 5)) && (Singleton<TutorialModule>.Instance != null))
                    {
                        Singleton<TutorialModule>.Instance.TryToDoTutoialWhenUpdateMissionStatus(mission);
                    }
                }
                this.missionDataReceived = true;
            }
            return false;
        }

        private bool OnGetMissionRewardRsp(GetMissionRewardRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionRewardGot, rsp));
                Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x71:
                    return this.OnGetMissionDataRsp(pkt.getData<GetMissionDataRsp>());

                case 0x73:
                    return this.OnGetMissionRewardRsp(pkt.getData<GetMissionRewardRsp>());

                case 0x74:
                    return this.OnDelMissionNotify(pkt.getData<DelMissionNotify>());

                case 0x76:
                    return this.OnUpdateMissionProgressRsp(pkt.getData<UpdateMissionProgressRsp>());
            }
            return false;
        }

        private bool OnUpdateMissionProgressRsp(UpdateMissionProgressRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
            }
            return false;
        }

        public void TryToUpdateKillAnyEnemy()
        {
            foreach (MissionDataItem item in this._missionDict.Values)
            {
                if (((item.metaData != null) && (item.status == 2)) && (item.metaData.finishWay == 8))
                {
                    this._enemyKilledCount++;
                    if ((item.progress + this._enemyKilledCount) >= item.metaData.totalProgress)
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(8, 0, string.Empty, (uint) this._enemyKilledCount);
                        this._enemyKilledCount = 0;
                    }
                    break;
                }
            }
        }

        public void TryToUpdateKillByAnimEventID(uint finishParaInt)
        {
            foreach (MissionDataItem item in this._missionDict.Values)
            {
                if (((item.metaData != null) && (item.status == 2)) && ((item.metaData.finishWay == 11) && (item.metaData.finishParaInt == finishParaInt)))
                {
                    if (this._monsterKilledByAnimEventIDCount.ContainsKey(finishParaInt))
                    {
                        Dictionary<uint, int> dictionary;
                        uint num;
                        int num2 = dictionary[num];
                        (dictionary = this._monsterKilledByAnimEventIDCount)[num = finishParaInt] = num2 + 1;
                    }
                    else
                    {
                        this._monsterKilledByAnimEventIDCount[finishParaInt] = 1;
                    }
                    if ((item.progress + this._monsterKilledByAnimEventIDCount[finishParaInt]) >= item.metaData.totalProgress)
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(11, finishParaInt, string.Empty, this._monsterKilledByAnimEventIDCount[finishParaInt]);
                        this._monsterKilledByAnimEventIDCount.Remove(finishParaInt);
                    }
                    break;
                }
            }
        }

        public void TryToUpdateKillByAttackCategoryTag(AttackResult.AttackCategoryTag[] categoryTags)
        {
            foreach (MissionDataItem item in this._missionDict.Values)
            {
                if (((item.metaData != null) && (item.status == 2)) && (item.metaData.finishWay == 0x17))
                {
                    foreach (AttackResult.AttackCategoryTag tag in categoryTags)
                    {
                        string key = tag.ToString();
                        if (item.metaData.finishParaStr == key)
                        {
                            if (this._monsterKilledByAttackCategoryTagCount.ContainsKey(key))
                            {
                                Dictionary<string, int> dictionary;
                                string str2;
                                int num2 = dictionary[str2];
                                (dictionary = this._monsterKilledByAttackCategoryTagCount)[str2 = key] = num2 + 1;
                            }
                            else
                            {
                                this._monsterKilledByAttackCategoryTagCount[key] = 1;
                            }
                            if ((item.progress + this._monsterKilledByAttackCategoryTagCount[key]) >= item.metaData.totalProgress)
                            {
                                Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(0x17, 0, key, this._monsterKilledByAttackCategoryTagCount[key]);
                                this._monsterKilledByAttackCategoryTagCount.Remove(key);
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void TryToUpdateKillMonsterMission(int monsterId)
        {
            foreach (MissionDataItem item in this._missionDict.Values)
            {
                if (((item.metaData != null) && (item.status == 2)) && ((item.metaData.finishWay == 6) && (item.metaData.finishParaInt == monsterId)))
                {
                    if (this.monsterKilledCount.ContainsKey(monsterId))
                    {
                        Dictionary<int, int> dictionary;
                        int num;
                        num = dictionary[num];
                        (dictionary = this.monsterKilledCount)[num = monsterId] = num + 1;
                    }
                    else
                    {
                        this.monsterKilledCount[monsterId] = 1;
                    }
                    if ((item.progress + this.monsterKilledCount[monsterId]) >= item.metaData.totalProgress)
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(6, (uint) monsterId, string.Empty, this.monsterKilledCount[monsterId]);
                        this.monsterKilledCount.Remove(monsterId);
                    }
                    break;
                }
            }
        }

        public void TryToUpdateKillMonsterWithCategoryName(string categoryName)
        {
            foreach (MissionDataItem item in this._missionDict.Values)
            {
                if (((item.metaData != null) && (item.status == 2)) && ((item.metaData.finishWay == 9) && (item.metaData.finishParaStr == categoryName)))
                {
                    if (this._monsterWithCategoryKilledCount.ContainsKey(categoryName))
                    {
                        Dictionary<string, int> dictionary;
                        string str;
                        int num = dictionary[str];
                        (dictionary = this._monsterWithCategoryKilledCount)[str = categoryName] = num + 1;
                    }
                    else
                    {
                        this._monsterWithCategoryKilledCount[categoryName] = 1;
                    }
                    if ((item.progress + this._monsterWithCategoryKilledCount[categoryName]) >= item.metaData.totalProgress)
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(9, 0, categoryName, this._monsterWithCategoryKilledCount[categoryName]);
                        this._monsterWithCategoryKilledCount.Remove(categoryName);
                    }
                    break;
                }
            }
        }

        public void TryToUpdateKillUniqueMonsterMission(uint uniqueMonsterId)
        {
            foreach (MissionDataItem item in this._missionDict.Values)
            {
                if (((item.metaData != null) && (item.status == 2)) && ((item.metaData.finishWay == 7) && (item.metaData.finishParaInt == uniqueMonsterId)))
                {
                    if (this._uniqueMonsterKilledCount.ContainsKey(uniqueMonsterId))
                    {
                        Dictionary<uint, int> dictionary;
                        uint num;
                        int num2 = dictionary[num];
                        (dictionary = this._uniqueMonsterKilledCount)[num = uniqueMonsterId] = num2 + 1;
                    }
                    else
                    {
                        this._uniqueMonsterKilledCount[uniqueMonsterId] = 1;
                    }
                    if ((item.progress + this._uniqueMonsterKilledCount[uniqueMonsterId]) >= item.metaData.totalProgress)
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(7, uniqueMonsterId, string.Empty, this._uniqueMonsterKilledCount[uniqueMonsterId]);
                        this._uniqueMonsterKilledCount.Remove(uniqueMonsterId);
                    }
                    break;
                }
            }
        }

        public void TryToUpdateTriggerAbilityAction(uint finishParaInt)
        {
            foreach (MissionDataItem item in this._missionDict.Values)
            {
                if (((item.metaData != null) && (item.status == 2)) && ((item.metaData.finishWay == 10) && (item.metaData.finishParaInt == finishParaInt)))
                {
                    if (this._triggerAbilityActionCount.ContainsKey(finishParaInt))
                    {
                        Dictionary<uint, int> dictionary;
                        uint num;
                        int num2 = dictionary[num];
                        (dictionary = this._triggerAbilityActionCount)[num = finishParaInt] = num2 + 1;
                    }
                    else
                    {
                        this._triggerAbilityActionCount[finishParaInt] = 1;
                    }
                    if ((item.progress + this._triggerAbilityActionCount[finishParaInt]) >= item.metaData.totalProgress)
                    {
                        Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress(10, finishParaInt, string.Empty, this._triggerAbilityActionCount[finishParaInt]);
                        this._triggerAbilityActionCount.Remove(finishParaInt);
                    }
                    break;
                }
            }
        }
    }
}

