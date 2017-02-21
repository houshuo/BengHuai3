namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class LevelModule : BaseModule
    {
        private Dictionary<int, ChapterDataItem> _allChapterDict;
        private Dictionary<int, LevelDataItem> _allLevelDict;
        private Dictionary<int, SeriesDataItem> _allSeriesDict;
        private Dictionary<int, WeekDayActivityDataItem> _allWeekDayActivityDict;
        private bool _isFirstStageDataRsp;
        private bool _isInitialized;
        private Dictionary<int, List<int>> _levelDropItemsDict;
        private Dictionary<int, List<int>> _levelFirstDropItemsDict;
        [CompilerGenerated]
        private static Comparison<WeekDayActivityDataItem> <>f__am$cacheB;
        [CompilerGenerated]
        private static Comparison<LevelDataItem> <>f__am$cacheC;

        public LevelModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._isInitialized = false;
            this._isFirstStageDataRsp = true;
        }

        private void AddAllActivityFromMetaData()
        {
            this._allWeekDayActivityDict.Clear();
            this._allSeriesDict.Clear();
            foreach (WeekDayActivityMetaData data in WeekDayActivityMetaDataReader.GetItemList())
            {
                WeekDayActivityDataItem item = new WeekDayActivityDataItem(data.weekDayActivityID);
                this._allWeekDayActivityDict.Add(data.weekDayActivityID, item);
                int seriesID = item.GetSeriesID();
                if (!this._allSeriesDict.ContainsKey(seriesID))
                {
                    this._allSeriesDict.Add(seriesID, new SeriesDataItem(seriesID));
                }
                this._allSeriesDict[seriesID].weekActivityList.Add(item);
                foreach (int num2 in item.GetLevelIDList())
                {
                    if (!this._allLevelDict.ContainsKey(num2) && (LevelMetaDataReader.GetLevelMetaDataByKey(num2) != null))
                    {
                        LevelDataItem item2 = new LevelDataItem(num2) {
                            ActID = item.GetActivityID(),
                            ChapterID = seriesID
                        };
                        this._allLevelDict.Add(item2.levelId, item2);
                    }
                }
            }
        }

        private void AddAllChaptersFromMetaData()
        {
            this.AllChapterList.Clear();
            this._allChapterDict.Clear();
            foreach (ChapterMetaData data in ChapterMetaDataReader.GetItemList())
            {
                ChapterDataItem item = new ChapterDataItem(data.chapterId);
                this.AllChapterList.Add(item);
                this._allChapterDict.Add(data.chapterId, item);
            }
        }

        private void AddLevelDataItem(LevelDataItem level)
        {
            List<LevelDataItem> collection = this._allChapterDict[level.ChapterID].AddLevel(level);
            this.AllLevelList.AddRange(collection);
            foreach (LevelDataItem item in collection)
            {
                this._allLevelDict.Add(item.levelId, item);
            }
        }

        public void ClearLevelEndReqInfo()
        {
            if (MiscData.Config.BasicConfig.FeatureOnRetrySendLevelEndReq)
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.ProcessingStageEndReq = null;
                Singleton<MiHoYoGameData>.Instance.Save();
            }
        }

        public bool ContainLevelById(int levelId)
        {
            return ((this._allLevelDict != null) && this._allLevelDict.ContainsKey(levelId));
        }

        public List<WeekDayActivityDataItem> GetActivityListBySeriesID(int seriesID)
        {
            if (this._allSeriesDict.ContainsKey(seriesID))
            {
                return this._allSeriesDict[seriesID].weekActivityList;
            }
            return null;
        }

        public ChapterDataItem GetChapterById(int chapterId)
        {
            return this._allChapterDict[chapterId];
        }

        public LevelDataItem GetLevelById(int levelId)
        {
            return this._allLevelDict[levelId];
        }

        public List<int> GetLevelDropItemIDList(int levelID)
        {
            List<int> list;
            this._levelDropItemsDict.TryGetValue(levelID, out list);
            return list;
        }

        public List<int> GetLevelFirstDropItemIDList(int levelID)
        {
            List<int> list;
            this._levelFirstDropItemsDict.TryGetValue(levelID, out list);
            return list;
        }

        public int GetOneUnlockChapterID()
        {
            int num = 1;
            foreach (KeyValuePair<int, ChapterDataItem> pair in this._allChapterDict)
            {
                if (pair.Value.Unlocked)
                {
                    return num;
                }
            }
            return num;
        }

        public WeekDayActivityDataItem GetWeekDayActivityByID(int activityID)
        {
            WeekDayActivityDataItem item;
            this._allWeekDayActivityDict.TryGetValue(activityID, out item);
            return item;
        }

        public List<LevelDataItem> GetWeekDayActivityLevelsByID(int activityID)
        {
            WeekDayActivityDataItem item;
            this._allWeekDayActivityDict.TryGetValue(activityID, out item);
            if (item == null)
            {
                return null;
            }
            List<LevelDataItem> list = new List<LevelDataItem>();
            foreach (int num in item.GetLevelIDList())
            {
                if (this._allLevelDict.ContainsKey(num))
                {
                    list.Add(this._allLevelDict[num]);
                }
            }
            return list;
        }

        public SeriesDataItem GetWeekDaySeriesByActivityID(int activityID)
        {
            SeriesDataItem item2;
            WeekDayActivityDataItem weekDayActivityByID = this.GetWeekDayActivityByID(activityID);
            if (weekDayActivityByID == null)
            {
                return null;
            }
            this._allSeriesDict.TryGetValue(weekDayActivityByID.GetSeriesID(), out item2);
            return item2;
        }

        public void HandleStageEndRspForRetry(StageEndRsp rsp)
        {
            if (MiscData.Config.BasicConfig.FeatureOnRetrySendLevelEndReq)
            {
                GeneralConfirmDialogContext context;
                this.ClearLevelEndReqInfo();
                if (rsp.get_retcode() == null)
                {
                    context = new GeneralConfirmDialogContext {
                        type = GeneralConfirmDialogContext.ButtonType.SingleButton,
                        desc = LocalizationGeneralLogic.GetText("Menu_Desc_SuccRetrySendLevelEndReq", new object[0])
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else
                {
                    string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                    context = new GeneralConfirmDialogContext {
                        type = GeneralConfirmDialogContext.ButtonType.SingleButton
                    };
                    object[] replaceParams = new object[] { networkErrCodeOutput };
                    context.desc = LocalizationGeneralLogic.GetText("Menu_Desc_FailRetrySendLevelEndReq", replaceParams);
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
            }
        }

        public bool HasChapter(int chapterID)
        {
            ChapterDataItem item;
            this._allChapterDict.TryGetValue(chapterID, out item);
            return ((item != null) && item.Unlocked);
        }

        public bool HasPlot(int levelID)
        {
            <HasPlot>c__AnonStoreyCF ycf = new <HasPlot>c__AnonStoreyCF {
                levelID = levelID
            };
            return (PlotMetaDataReader.GetItemList().Find(new Predicate<PlotMetaData>(ycf.<>m__D7)) != null);
        }

        private void Init()
        {
            this._isInitialized = true;
            this.AllLevelList = new List<LevelDataItem>();
            this._allLevelDict = new Dictionary<int, LevelDataItem>();
            this.AllChapterList = new List<ChapterDataItem>();
            this._allChapterDict = new Dictionary<int, ChapterDataItem>();
            this.AllWeekDayActivityList = new List<WeekDayActivityDataItem>();
            this._allWeekDayActivityDict = new Dictionary<int, WeekDayActivityDataItem>();
            this._allSeriesDict = new Dictionary<int, SeriesDataItem>();
            this._levelDropItemsDict = new Dictionary<int, List<int>>();
            this._levelFirstDropItemsDict = new Dictionary<int, List<int>>();
            this.AddAllChaptersFromMetaData();
            this.AddAllActivityFromMetaData();
        }

        public bool IsLastLevelOfAct(LevelDataItem levelData)
        {
            if (levelData.LevelType != 1)
            {
                return false;
            }
            List<LevelDataItem> list = this.GetChapterById(levelData.ChapterID).GetLevelOfActs(1)[levelData.ActID];
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = (x, y) => y.SectionId - x.SectionId;
            }
            list.Sort(<>f__am$cacheC);
            return (levelData.SectionId == list[0].SectionId);
        }

        public bool IsLevelDone(int levelID)
        {
            LevelDataItem item = this.TryGetLevelById(levelID);
            return ((item != null) && (item.status == 3));
        }

        private bool OnGetStageDataRsp(GetStageDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if (!this._isInitialized)
                {
                    this.Init();
                }
                foreach (Stage stage in rsp.get_stage_list())
                {
                    int levelId = (int) stage.get_id();
                    if (LevelMetaDataReader.GetLevelMetaDataByKey(levelId) != null)
                    {
                        if (!this._allLevelDict.ContainsKey(levelId))
                        {
                            LevelDataItem level = new LevelDataItem(levelId);
                            if (level.LevelType == 1)
                            {
                                this.AddLevelDataItem(level);
                            }
                            else
                            {
                                this._allLevelDict[levelId] = level;
                                this.AllLevelList.Add(level);
                            }
                        }
                        LevelDataItem item2 = this._allLevelDict[levelId];
                        if (item2.LevelType == 1)
                        {
                            this._allChapterDict[item2.ChapterID].Unlocked = true;
                        }
                        if ((this._allLevelDict[levelId].LevelType == 1) && !this._allLevelDict[levelId].Initialized)
                        {
                            this._allLevelDict[levelId].Init();
                            if (!this._isFirstStageDataRsp)
                            {
                                ActDataItem item3 = new ActDataItem(this._allLevelDict[levelId].ActID);
                                if ((item3 != null) && (item3.actType != ActDataItem.ActType.Extra))
                                {
                                    Singleton<MiHoYoGameData>.Instance.LocalData.NeedPlayLevelAnimationSet.Add(levelId);
                                }
                                if (this.IsLastLevelOfAct(this._allLevelDict[levelId]))
                                {
                                    Singleton<MiHoYoGameData>.Instance.LocalData.EndThunderDateTime = TimeUtil.Now.AddHours(4.0);
                                    Singleton<MiHoYoGameData>.Instance.LocalData.NextRandomDateTime = TimeUtil.Now.AddDays(-1.0);
                                }
                                else
                                {
                                    Singleton<MiHoYoGameData>.Instance.LocalData.EndThunderDateTime = TimeUtil.Now.AddDays(-1.0);
                                }
                                Singleton<MiHoYoGameData>.Instance.Save();
                            }
                        }
                        LevelDataItem levelData = this._allLevelDict[levelId];
                        base.UpdateField<int>(stage.get_progressSpecified(), ref levelData.progress, (int) stage.get_progress(), new Action<int, int>(this._allLevelDict[levelId].OnProgressUpdate));
                        base.UpdateField<int>(stage.get_enter_timesSpecified(), ref levelData.enterTimes, (int) stage.get_enter_times(), null);
                        base.UpdateField<int>(stage.get_reset_timesSpecified(), ref levelData.resetTimes, (int) stage.get_reset_times(), null);
                        base.UpdateField<int>(stage.get_bonus_enter_timesSpecified(), ref levelData.dropActivityEnterTimes, (int) stage.get_bonus_enter_times(), null);
                        base.UpdateField<int>(stage.get_bonus_total_timesSpecified(), ref levelData.dropActivityMaxEnterTimes, (int) stage.get_bonus_total_times(), null);
                        if (((levelData.progress == 0) && (levelData.LevelType == 1)) && (Singleton<MainMenuBGM>.Instance != null))
                        {
                            Singleton<MainMenuBGM>.Instance.SetBGMSwitchByStage(this.IsLastLevelOfAct(levelData));
                        }
                        levelData.isDropActivityOpen = stage.get_bonus_end_timeSpecified();
                        levelData.dropActivityEndTime = Miscs.GetDateTimeFromTimeStamp(stage.get_bonus_end_time());
                        for (int i = 0; i < this._allLevelDict[levelId].challengeList.Count; i++)
                        {
                            LevelChallengeDataItem item5 = this._allLevelDict[levelId].challengeList[i];
                            if (stage.get_challenge_index_list().Contains((uint) i))
                            {
                                item5.Finished = true;
                            }
                        }
                    }
                }
                this._isFirstStageDataRsp = false;
            }
            return false;
        }

        private bool OnGetStageDropDisplayRsp(GetStageDropDisplayRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (StageDropDisplayInfo info in rsp.get_stage_drop_list())
                {
                    int levelId = (int) info.get_stage_id();
                    if (LevelMetaDataReader.GetLevelMetaDataByKey(levelId) != null)
                    {
                        if (!this._allLevelDict.ContainsKey(levelId))
                        {
                            LevelDataItem level = new LevelDataItem(levelId);
                            if (level.LevelType == 1)
                            {
                                this.AddLevelDataItem(level);
                            }
                            else
                            {
                                this._allLevelDict[levelId] = level;
                                this.AllLevelList.Add(level);
                            }
                        }
                        LevelDataItem item2 = this._allLevelDict[levelId];
                        item2.dropDisplayInfoReceived = true;
                        item2.displayDropList = base.ConvertList(info.get_drop_item_id_list());
                        item2.displayFirstDropList = base.ConvertList(info.get_first_drop_item_id_list());
                        item2.displayBonusDropList = base.ConvertList(info.get_bonus_drop_item_id_list());
                        item2.isDoubleDrop = info.get_double_drop();
                    }
                }
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.StageDropListUpdated, null));
            }
            return false;
        }

        private bool OnGetWeekDayActivityDataRsp(GetWeekDayActivityDataRsp rsp)
        {
            if (!this._isInitialized)
            {
                this.Init();
            }
            if (rsp.get_retcode() == null)
            {
                if (rsp.get_is_whole_dataSpecified() && rsp.get_is_whole_data())
                {
                    this.AllWeekDayActivityList.Clear();
                }
                List<uint> levelIDList = new List<uint>();
                foreach (WeekDayActivity activity in rsp.get_activity_list())
                {
                    WeekDayActivityDataItem item;
                    this._allWeekDayActivityDict.TryGetValue((int) activity.get_activity_id(), out item);
                    if (item == null)
                    {
                        Debug.LogError("The activity with id: " + activity.get_activity_id() + " , is wrong!!!");
                    }
                    else
                    {
                        if (this.AllWeekDayActivityList.Contains(item))
                        {
                            this.AllWeekDayActivityList.Remove(item);
                        }
                        item.enterTimes = (int) activity.get_enter_times();
                        item.beginTime = Miscs.GetDateTimeFromTimeStamp(activity.get_begin_time());
                        item.endTime = Miscs.GetDateTimeFromTimeStamp(activity.get_end_time());
                        item.InitStatusOnPacket();
                        this._allWeekDayActivityDict[item.GetActivityID()] = item;
                        this.AllWeekDayActivityList.Add(item);
                        foreach (uint num in activity.get_stage_id_list())
                        {
                            LevelDataItem item2;
                            this._allLevelDict.TryGetValue((int) num, out item2);
                            if (item2 != null)
                            {
                                item2.status = 2;
                            }
                        }
                        levelIDList.AddRange(activity.get_stage_id_list());
                    }
                }
                Singleton<NetworkManager>.Instance.RequestLevelDropList(levelIDList);
                if (<>f__am$cacheB == null)
                {
                    <>f__am$cacheB = (lobj, robj) => lobj.GetActivityID() - robj.GetActivityID();
                }
                this.AllWeekDayActivityList.Sort(<>f__am$cacheB);
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x2a:
                    return this.OnGetStageDataRsp(pkt.getData<GetStageDataRsp>());

                case 0x3d:
                    return this.OnGetStageDropDisplayRsp(pkt.getData<GetStageDropDisplayRsp>());

                case 0x7e:
                    return this.OnGetWeekDayActivityDataRsp(pkt.getData<GetWeekDayActivityDataRsp>());
            }
            return false;
        }

        public void RetrySendLevelEndReq()
        {
            if (MiscData.Config.BasicConfig.FeatureOnRetrySendLevelEndReq && (Singleton<MiHoYoGameData>.Instance.LocalData.ProcessingStageEndReq != null))
            {
                MemoryStream source = new MemoryStream(Singleton<MiHoYoGameData>.Instance.LocalData.ProcessingStageEndReq);
                StageEndReq data = (StageEndReq) Singleton<NetworkManager>.Instance.serializer.Deserialize(source, null, typeof(StageEndReq));
                if (data != null)
                {
                    Singleton<NetworkManager>.Instance.SendPacket<StageEndReq>(data);
                }
                else
                {
                    this.ClearLevelEndReqInfo();
                }
            }
        }

        public void SaveLevelEndReqInfo(StageEndReq stageEndReq)
        {
            if (MiscData.Config.BasicConfig.FeatureOnRetrySendLevelEndReq)
            {
                MemoryStream dest = new MemoryStream();
                dest.SetLength(0L);
                dest.Position = 0L;
                Singleton<NetworkManager>.Instance.serializer.Serialize(dest, stageEndReq);
                Singleton<MiHoYoGameData>.Instance.LocalData.ProcessingStageEndReq = dest.ToArray();
                Singleton<MiHoYoGameData>.Instance.Save();
            }
        }

        public LevelDataItem TryGetLevelById(int levelId)
        {
            LevelDataItem item = null;
            if (this._allLevelDict != null)
            {
                this._allLevelDict.TryGetValue(levelId, out item);
            }
            return item;
        }

        public WeekDayActivityDataItem TryGetWeekDayActivityByLevelID(int levelID)
        {
            foreach (WeekDayActivityDataItem item2 in this._allWeekDayActivityDict.Values)
            {
                if (item2.GetLevelIDList().Contains(levelID))
                {
                    return item2;
                }
            }
            return null;
        }

        public List<ChapterDataItem> AllChapterList { get; private set; }

        public List<LevelDataItem> AllLevelList { get; private set; }

        public List<WeekDayActivityDataItem> AllWeekDayActivityList { get; private set; }

        [CompilerGenerated]
        private sealed class <HasPlot>c__AnonStoreyCF
        {
            internal int levelID;

            internal bool <>m__D7(PlotMetaData x)
            {
                return (x.levelID == this.levelID);
            }
        }
    }
}

