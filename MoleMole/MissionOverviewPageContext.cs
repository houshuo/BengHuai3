namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UniRx;
    using UnityEngine;

    public class MissionOverviewPageContext : BasePageContext
    {
        private List<Tuple<StorageDataItemBase, bool>> _avatarGotList;
        private Dictionary<int, RectTransform> _dictBeforeFetch;
        private MonoScrollerFadeManager _fadeMgr;
        private int _levelBeforeReward;
        private List<MissionDataItem> _missionList = new List<MissionDataItem>();
        private MonoMissionUtil _util;
        private bool _waitingForIslandServerData;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, int> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, RectTransform> <>f__am$cache8;

        public MissionOverviewPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "MissionOverviewPageContext",
                viewPrefabPath = "UI/Menus/Page/Mission/MissionOverviewPage"
            };
            base.config = pattern;
            this._avatarGotList = new List<Tuple<StorageDataItemBase, bool>>();
        }

        protected override void BindViewCallbacks()
        {
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        private bool FilterMission(MissionDataItem mission)
        {
            LinearMissionData data = LinearMissionDataReader.TryGetLinearMissionDataByKey(mission.id);
            if ((data != null) && (data.IsAchievement == 1))
            {
                return true;
            }
            if ((data != null) && (data.PreMissionId > 0))
            {
                foreach (MissionDataItem item in Singleton<MissionModule>.Instance.GetMissionDict().Values)
                {
                    if ((item.id == data.PreMissionId) && ((item.status == 2) || (item.status == 3)))
                    {
                        return true;
                    }
                }
            }
            if (this.IsPreviewMission(mission))
            {
                TimeSpan span = (TimeSpan) (Miscs.GetDateTimeFromTimeStamp((uint) mission.beginTime) - TimeUtil.Now);
                if (span.TotalSeconds > mission.metaData.PreviewTime)
                {
                    return true;
                }
            }
            return false;
        }

        private void FilterMissions()
        {
            List<MissionDataItem> list = Enumerable.ToList<MissionDataItem>(Singleton<MissionModule>.Instance.GetMissionDict().Values);
            this._missionList.Clear();
            foreach (MissionDataItem item in list)
            {
                bool flag = false;
                if (item.status == 5)
                {
                    flag = true;
                }
                else
                {
                    LinearMissionData data = LinearMissionDataReader.TryGetLinearMissionDataByKey(item.id);
                    if ((data != null) && (data.PreMissionId > 0))
                    {
                        foreach (MissionDataItem item2 in list)
                        {
                            if ((item2.id == data.PreMissionId) && ((item2.status == 2) || (item2.status == 3)))
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (this.IsPreviewMission(item))
                    {
                        TimeSpan span = (TimeSpan) (Miscs.GetDateTimeFromTimeStamp((uint) item.beginTime) - TimeUtil.Now);
                        if (span.TotalSeconds > item.metaData.PreviewTime)
                        {
                            flag = true;
                            this._util.AddPreviewMission(item);
                        }
                    }
                    if ((data != null) && (data.IsAchievement == 1))
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    this._missionList.Add(item);
                }
            }
        }

        private MissionDataItem GetMissionDataItem(int id)
        {
            Dictionary<int, MissionDataItem> missionDict = Singleton<MissionModule>.Instance.GetMissionDict();
            if (missionDict.ContainsKey(id))
            {
                return missionDict[id];
            }
            return null;
        }

        private void GoToActivityPage(List<int> activityIDList)
        {
            int activityID = 0;
            WeekDayActivityDataItem weekDayActivity = null;
            foreach (int num2 in activityIDList)
            {
                if (Singleton<LevelModule>.Instance.GetWeekDayActivityByID(num2).GetStatus() == ActivityDataItemBase.Status.InProgress)
                {
                    activityID = num2;
                    break;
                }
            }
            if (activityID <= 0)
            {
                activityID = activityIDList[0];
            }
            weekDayActivity = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(activityID);
            Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(weekDayActivity), UIType.Page);
        }

        private void GoToActivityRootPage()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext("Event"), UIType.Page);
        }

        private void GoToAvatarPage()
        {
            AvatarOverviewPageContext context = new AvatarOverviewPageContext {
                type = AvatarOverviewPageContext.PageType.Show,
                selectedAvatarID = Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.lastSelectedAvatarID
            };
            Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
        }

        private void GoToChapterSelectPage()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(null), UIType.Page);
        }

        private void GoToEndlessPage()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext("Event"), UIType.Page);
        }

        private void GoToEquipPage()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new StorageShowPageContext(), UIType.Page);
        }

        private void GoToIsland()
        {
            this._waitingForIslandServerData = true;
            Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(0x9d, null), UIType.Any);
            Singleton<NetworkManager>.Instance.RequestGetIsland();
        }

        private void GoToMainStoryRootPage()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext("MainStory"), UIType.Page);
        }

        private void GoToSerialPage(int serialID)
        {
            WeekDayActivityDataItem weekDayActivity = null;
            List<WeekDayActivityDataItem> allWeekDayActivityList = Singleton<LevelModule>.Instance.AllWeekDayActivityList;
            for (int i = 0; i < allWeekDayActivityList.Count; i++)
            {
                if ((allWeekDayActivityList[i].GetSeriesID() == serialID) && (allWeekDayActivityList[i].GetStatus() == ActivityDataItemBase.Status.InProgress))
                {
                    weekDayActivity = allWeekDayActivityList[i];
                    break;
                }
            }
            if (weekDayActivity == null)
            {
                weekDayActivity = Singleton<LevelModule>.Instance.GetActivityListBySeriesID(serialID)[0];
            }
            Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(weekDayActivity), UIType.Page);
        }

        private void GoToStageDetailDialog(int stageID)
        {
            LevelDataItem levelData = Singleton<LevelModule>.Instance.TryGetLevelById(stageID);
            if (levelData == null)
            {
                levelData = new LevelDataItem(stageID);
            }
            if (levelData.UnlockPlayerLevel > Singleton<PlayerModule>.Instance.playerData.teamLevel)
            {
                object[] replaceParams = new object[] { levelData.UnlockPlayerLevel };
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ActivityLock", replaceParams), 2f), UIType.Any);
            }
            else
            {
                int totalFinishedChanllengeNum = Singleton<LevelModule>.Instance.GetChapterById(levelData.ChapterID).GetTotalFinishedChanllengeNum(levelData.Diffculty);
                if (levelData.UnlockChanllengeNum > totalFinishedChanllengeNum)
                {
                    object[] objArray2 = new object[] { levelData.UnlockChanllengeNum };
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ChallengeLackLock", objArray2), 2f), UIType.Any);
                }
                else
                {
                    Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(levelData), UIType.Page);
                }
            }
        }

        private void GoToSupplyPage()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new GachaMainPageContext(), UIType.Page);
        }

        private void HackAddMission()
        {
            Mission mission2 = new Mission();
            mission2.set_mission_id(0x2715);
            Mission mission = mission2;
            MissionDataItem item = new MissionDataItem(mission) {
                status = 2
            };
            this._missionList.Add(item);
            mission2 = new Mission();
            mission2.set_mission_id(0x59d9);
            mission = mission2;
            item = new MissionDataItem(mission) {
                status = 2
            };
            this._missionList.Add(item);
            mission2 = new Mission();
            mission2.set_mission_id(0x55f5);
            mission = mission2;
            item = new MissionDataItem(mission) {
                status = 2,
                progress = 20
            };
            this._missionList.Add(item);
            mission2 = new Mission();
            mission2.set_mission_id(0x55f3);
            mission = mission2;
            item = new MissionDataItem(mission) {
                status = 2,
                progress = 20
            };
            this._missionList.Add(item);
            mission2 = new Mission();
            mission2.set_mission_id(0x791a);
            mission = mission2;
            item = new MissionDataItem(mission) {
                status = 2,
                progress = 20
            };
            this._missionList.Add(item);
            mission2 = new Mission();
            mission2.set_mission_id(0x791d);
            mission = mission2;
            item = new MissionDataItem(mission) {
                status = 2,
                progress = 20
            };
            this._missionList.Add(item);
            mission2 = new Mission();
            mission2.set_mission_id(0x791c);
            mission = mission2;
            item = new MissionDataItem(mission) {
                status = 2,
                progress = 20
            };
            this._missionList.Add(item);
        }

        private bool IsMissionEqual(RectTransform missionNew, RectTransform missionOld)
        {
            if ((missionNew == null) || (missionOld == null))
            {
                return false;
            }
            MonoMissionInfo component = missionOld.GetComponent<MonoMissionInfo>();
            return (missionNew.GetComponent<MonoMissionInfo>().GetMissionData().id == component.GetMissionData().id);
        }

        private bool IsPreviewMission(MissionDataItem mission)
        {
            return (((mission.metaData.type == 3) && (mission.status == 1)) && (mission.metaData.PreviewTime > 0));
        }

        private void OnFetchRewardBtnClick(MissionDataItem missionData)
        {
            this.SaveDataBeforeReward();
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = entry => entry.Key;
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = entry => entry.Value;
            }
            this._dictBeforeFetch = Enumerable.ToDictionary<KeyValuePair<int, RectTransform>, int, RectTransform>(base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoGridScroller>().GetItemDict(), <>f__am$cache7, <>f__am$cache8);
            Singleton<NetworkManager>.Instance.RequestGetMissionReward((uint) missionData.id);
        }

        private bool OnGetIsLandRsp(GetIslandRsp rsp)
        {
            if (this._waitingForIslandServerData)
            {
                this._waitingForIslandServerData = false;
                Singleton<MainUIManager>.Instance.MoveToNextScene("Island", false, true, true, null, true);
            }
            return false;
        }

        private void OnGoMissionBtnClick(MissionDataItem missionData)
        {
            E_MissionLinkType linkType = (E_MissionLinkType) missionData.metaData.LinkType;
            int linkParam = missionData.metaData.LinkParam;
            switch (linkType)
            {
                case E_MissionLinkType.Stage:
                    this.GoToStageDetailDialog(linkParam);
                    break;

                case E_MissionLinkType.Activity:
                    this.GoToActivityPage(missionData.metaData.LinkParams);
                    break;

                case E_MissionLinkType.Serial:
                    this.GoToSerialPage(linkParam);
                    break;

                case E_MissionLinkType.Attack:
                    this.GoToChapterSelectPage();
                    break;

                case E_MissionLinkType.Valkyrja:
                    this.GoToAvatarPage();
                    break;

                case E_MissionLinkType.Equip:
                    this.GoToEquipPage();
                    break;

                case E_MissionLinkType.Supply:
                    this.GoToSupplyPage();
                    break;

                case E_MissionLinkType.ActivityRoot:
                    this.GoToActivityRootPage();
                    break;

                case E_MissionLinkType.MainStoryRoot:
                    this.GoToMainStoryRootPage();
                    break;

                case E_MissionLinkType.Endless:
                    this.GoToEndlessPage();
                    break;

                case E_MissionLinkType.Island:
                    this.GoToIsland();
                    break;
            }
        }

        private void OnMissionRewardGot(GetMissionRewardRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                MissionRewardGotDialogContext dialogContext = new MissionRewardGotDialogContext(rsp.get_reward_list(), null);
                dialogContext.RegisterCallBack(new MissionRewardGotDialogContext.OnDialogDestroy(this.OnRewardGotDialogClose));
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
        }

        private void OnMissionUpdated(uint id)
        {
            this.SetupView();
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.MissionUpdated)
            {
                MissionDataItem missionDataItem = this.GetMissionDataItem((int) ((uint) ntf.body));
                if ((missionDataItem != null) && !this.FilterMission(missionDataItem))
                {
                    this.SetupView();
                }
            }
            else if (ntf.type == NotifyTypes.MissionRewardGot)
            {
                this.OnMissionRewardGot((GetMissionRewardRsp) ntf.body);
                this._fadeMgr.Reset();
            }
            else if (ntf.type == NotifyTypes.MissionRewardAvatarGot)
            {
                this.ShowAvatarGot((AvatarCardDataItem) ntf.body);
            }
            else if (ntf.type == NotifyTypes.MissionDeleted)
            {
                this.SetupView();
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x9d) && this.OnGetIsLandRsp(pkt.getData<GetIslandRsp>()));
        }

        public void OnRewardGotDialogClose(AvatarCardDataItem avatarData)
        {
            if (this._levelBeforeReward < Singleton<PlayerModule>.Instance.playerData.teamLevel)
            {
                PlayerLevelUpDialogContext dialogContext = new PlayerLevelUpDialogContext();
                dialogContext.SetLevelBeforeNoScoreManager(this._levelBeforeReward);
                if (avatarData != null)
                {
                    dialogContext.SetNotifyWhenDestroy(avatarData);
                }
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else if (avatarData != null)
            {
                this.ShowAvatarGot(avatarData);
            }
        }

        private void OnScrollerChange(Transform trans, int index)
        {
            MissionDataItem missionData = this._missionList[index];
            MonoMissionInfo component = trans.GetComponent<MonoMissionInfo>();
            component.SetupView(missionData);
            component.RegisterCallBacks(new FetchRewardCallBack(this.OnFetchRewardBtnClick), new GoMissionCallBack(this.OnGoMissionBtnClick));
        }

        protected override void OnSetActive(bool enabled)
        {
            if (!enabled)
            {
                this._fadeMgr.Reset();
            }
        }

        private void SaveDataBeforeReward()
        {
            this._levelBeforeReward = Singleton<PlayerModule>.Instance.playerData.teamLevel;
        }

        protected override bool SetupView()
        {
            if ((base.view != null) && (base.view.transform != null))
            {
                this._util = base.view.GetComponent<MonoMissionUtil>();
                this._util.Init();
                this.FilterMissions();
                this._missionList.Sort(new Comparison<MissionDataItem>(MissionDataItem.CompareToMission));
                MonoGridScroller component = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoGridScroller>();
                component.Init(new MonoGridScroller.OnChange(this.OnScrollerChange), this._missionList.Count, null);
                this._fadeMgr = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoScrollerFadeManager>();
                this._fadeMgr.Init(component.GetItemDict(), this._dictBeforeFetch, new Func<RectTransform, RectTransform, bool>(this.IsMissionEqual));
                this._fadeMgr.Play();
                this._dictBeforeFetch = null;
            }
            return false;
        }

        private void ShowAvatarGot(AvatarCardDataItem avatarData)
        {
            this._avatarGotList.Clear();
            this._avatarGotList.Add(new Tuple<StorageDataItemBase, bool>(avatarData, false));
            Singleton<MainUIManager>.Instance.ShowDialog(new DropNewItemDialogContextV2(this._avatarGotList), UIType.Any);
        }

        public enum E_MissionLinkType
        {
            None,
            Stage,
            Activity,
            Serial,
            Attack,
            Valkyrja,
            Equip,
            Supply,
            ActivityRoot,
            MainStoryRoot,
            Endless,
            Island
        }
    }
}

