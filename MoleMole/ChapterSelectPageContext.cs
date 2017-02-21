namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class ChapterSelectPageContext : BasePageContext
    {
        private MonoActScroller _actScroller;
        private float _actScrollerLerpSpeed;
        private float _actScrollerSpeedDownRatio;
        private float _actScrollerStopLerpThreshold;
        private StageType _chapterType;
        private bool _difficultyPopUpActive;
        private bool _justShowLevelDetail;
        private LevelDetailDialogContextV2 _levelDetailContext;
        private bool _needLerpAfterInitLevels;
        private CanvasTimer _newUnlockLevelActDelayTimer;
        private CanvasTimer _newUnlockLevelAnimationTimer;
        private List<LevelDataItem> _newUnlockLevelDataList;
        private bool _shouldChangeActIndex;
        private int _showActIndex;
        private LevelDataItem _toShowLevelData;
        private WeekDayActivityDataItem _weekDayActivityData;
        [CompilerGenerated]
        private static Predicate<WeekDayActivityDataItem> <>f__am$cache13;
        [CompilerGenerated]
        private static Comparison<LevelDataItem> <>f__am$cache14;
        [CompilerGenerated]
        private static Predicate<LevelDataItem> <>f__am$cache15;
        [CompilerGenerated]
        private static Comparison<WeekDayActivityDataItem> <>f__am$cache16;
        private const float ACT_DELAY_TIMER_SPAN = 0.4f;
        private const string ACT_PREFAB_PATH = "UI/Menus/Widget/Map/Act";
        private ChapterDataItem chapter;
        private LevelDiffculty difficulty;
        private const int FIRST_LEVEL_ID = 0x2775;
        private const string LEVEL_PANEL_PREFAB_PATH = "UI/Menus/Widget/Map/LevelPanel";
        public Dictionary<LevelDataItem, Transform> levelTransDict;
        private const string PAGE_FADE_IN_ANI_STR = "PageFadeIn";
        private const float TIMER_SPAN = 0.5f;

        public ChapterSelectPageContext(ChapterDataItem chapter = null)
        {
            this._actScrollerSpeedDownRatio = 5f;
            ContextPattern pattern = new ContextPattern {
                contextName = "ChapterSelectPageContext",
                viewPrefabPath = "UI/Menus/Page/Map/ChapterSelectPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            base.findViewSavedInScene = true;
            this.InitData(chapter);
            this.SetNewUnlockLevelData();
        }

        public ChapterSelectPageContext(LevelDataItem levelData)
        {
            this._actScrollerSpeedDownRatio = 5f;
            ContextPattern pattern = new ContextPattern {
                contextName = "ChapterSelectPageContext",
                viewPrefabPath = "UI/Menus/Page/Map/ChapterSelectPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            base.findViewSavedInScene = true;
            this._toShowLevelData = levelData;
            this._justShowLevelDetail = this._toShowLevelData != null;
            this._chapterType = levelData.LevelType;
            switch (this._chapterType)
            {
                case 1:
                    this.chapter = Singleton<LevelModule>.Instance.GetChapterById(levelData.ChapterID);
                    this.difficulty = levelData.Diffculty;
                    this._showActIndex = new ActDataItem(levelData.ActID).actIndex;
                    break;

                case 2:
                case 3:
                    this._weekDayActivityData = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(levelData.ActID);
                    break;
            }
        }

        public ChapterSelectPageContext(WeekDayActivityDataItem weekDayActivity)
        {
            this._actScrollerSpeedDownRatio = 5f;
            ContextPattern pattern = new ContextPattern {
                contextName = "ChapterSelectPageContext",
                viewPrefabPath = "UI/Menus/Page/Map/ChapterSelectPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            base.findViewSavedInScene = true;
            this._chapterType = 2;
            this._weekDayActivityData = weekDayActivity;
        }

        private void ActDelayTimerTimeUpCallBack()
        {
            if (this._newUnlockLevelDataList.Count >= 1)
            {
                this._showActIndex = new ActDataItem(this._newUnlockLevelDataList[0].ActID).actIndex;
                this.SetupLevels();
                this.SetRectMaskDirty();
            }
        }

        public override void BackPage()
        {
            if ((this._levelDetailContext != null) && !this._justShowLevelDetail)
            {
                this._levelDetailContext.Destroy();
                this._levelDetailContext = null;
                base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn", false, null);
            }
            else
            {
                base.BackPage();
            }
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("WorldMapBtn").GetComponent<Button>(), new UnityAction(this.OnWorldMapBtnClick));
            base.BindViewCallback(base.view.transform.Find("Title/DescPanel/IconEx/Btn").GetComponent<Button>(), new UnityAction(this.JumpToNuclearActivity));
            base.BindViewCallback(base.view.transform.Find("EventShopBtn").GetComponent<Button>(), new UnityAction(this.OnActivityShopBtnClick));
        }

        private void CheckActIndex()
        {
            if ((this._chapterType == 1) && ((this.chapter.chapterId != Singleton<MiHoYoGameData>.Instance.LocalData.LastChapterID) || (this.difficulty != Singleton<MiHoYoGameData>.Instance.LocalData.LastDifficulty)))
            {
                this._showActIndex = 0;
            }
        }

        private void ClearActivityContent()
        {
            this.ClearChildItems(base.view.transform.Find("LevelPanel/ScrollerView/Content"));
            this.ClearChildItems(base.view.transform.Find("ActPanel/ScrollerView/Content"));
        }

        private void ClearChildItems(Transform target)
        {
            if (target != null)
            {
                IEnumerator enumerator = target.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        if (current != null)
                        {
                            MonoItemStatus component = current.GetComponent<MonoItemStatus>();
                            if (component != null)
                            {
                                component.isValid = false;
                            }
                            UnityEngine.Object.Destroy(current.gameObject);
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
        }

        private void ClearLevelsContent()
        {
            this.ClearChildItems(base.view.transform.Find("LevelPanel/ScrollerView/Content"));
            this.ClearChildItems(base.view.transform.Find("ActPanel/ScrollerView/Content"));
        }

        private void CreateNuclearActivityTips()
        {
            GeneralConfirmDialogContext dialogContext = new GeneralConfirmDialogContext {
                type = GeneralConfirmDialogContext.ButtonType.SingleButton,
                desc = LocalizationGeneralLogic.GetText("Menu_NuclearOpen", new object[0]),
                buttonCallBack = delegate (bool confirmed) {
                    if (confirmed)
                    {
                        this.JumpToNuclearActivity();
                    }
                }
            };
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        public override void Destroy()
        {
            if (this._newUnlockLevelActDelayTimer != null)
            {
                this._newUnlockLevelActDelayTimer.Destroy();
            }
            if (this._newUnlockLevelAnimationTimer != null)
            {
                this._newUnlockLevelAnimationTimer.Destroy();
            }
            if (base.view != null)
            {
                this.ClearLevelsContent();
                this.ClearActivityContent();
            }
            base.Destroy();
        }

        private List<WeekDayActivityDataItem> GetShowActivityListBySeries(SeriesDataItem seriesData, out bool hasInProgressNuclearActivity)
        {
            hasInProgressNuclearActivity = false;
            List<WeekDayActivityDataItem> list = new List<WeekDayActivityDataItem>();
            if (seriesData.weekActivityList != null)
            {
                int num = 0;
                int count = seriesData.weekActivityList.Count;
                while (num < count)
                {
                    if (seriesData.weekActivityList[num].GetActivityType() == 3)
                    {
                        if (seriesData.weekActivityList[num].GetStatus() != ActivityDataItemBase.Status.InProgress)
                        {
                            goto Label_0071;
                        }
                        hasInProgressNuclearActivity = true;
                    }
                    list.Add(seriesData.weekActivityList[num]);
                Label_0071:
                    num++;
                }
                if (<>f__am$cache16 == null)
                {
                    <>f__am$cache16 = (lob, robj) => lob.GetActivityID() - robj.GetActivityID();
                }
                list.Sort(<>f__am$cache16);
            }
            return list;
        }

        private Dictionary<int, List<LevelDataItem>> GetShowLevelOfActs()
        {
            Dictionary<int, List<LevelDataItem>> levelOfActs = this.chapter.GetLevelOfActs(this.difficulty);
            foreach (int num in levelOfActs.Keys.ToArray<int>())
            {
                ActDataItem item = new ActDataItem(num);
                if (item.actType == ActDataItem.ActType.Extra)
                {
                    if (<>f__am$cache15 == null)
                    {
                        <>f__am$cache15 = x => x.status == 1;
                    }
                    if (levelOfActs[num].TrueForAll(<>f__am$cache15))
                    {
                        levelOfActs.Remove(num);
                    }
                }
            }
            return levelOfActs;
        }

        private void InitData(ChapterDataItem chapter)
        {
            this._chapterType = 1;
            if (chapter == null)
            {
                int lastChapterID = Singleton<MiHoYoGameData>.Instance.LocalData.LastChapterID;
                this.chapter = Singleton<LevelModule>.Instance.GetChapterById(lastChapterID);
                this.difficulty = Singleton<MiHoYoGameData>.Instance.LocalData.LastDifficulty;
                this._showActIndex = Singleton<MiHoYoGameData>.Instance.LocalData.LastActIndex;
            }
            else
            {
                this.chapter = chapter;
                this.difficulty = Singleton<MiHoYoGameData>.Instance.LocalData.LastDifficulty;
                if (this.chapter.chapterId != Singleton<MiHoYoGameData>.Instance.LocalData.LastChapterID)
                {
                    this._showActIndex = 0;
                }
                else
                {
                    this._showActIndex = Singleton<MiHoYoGameData>.Instance.LocalData.LastActIndex;
                }
            }
        }

        private void JumpToNuclearActivity()
        {
            if (this._weekDayActivityData != null)
            {
                if (<>f__am$cache13 == null)
                {
                    <>f__am$cache13 = x => (x.GetActivityType() == 3) && (x.GetStatus() == ActivityDataItemBase.Status.InProgress);
                }
                WeekDayActivityDataItem item2 = Singleton<LevelModule>.Instance.GetWeekDaySeriesByActivityID(this._weekDayActivityData.GetActivityID()).weekActivityList.Find(<>f__am$cache13);
                if (item2 != null)
                {
                    this._weekDayActivityData = item2;
                    this.SetupView();
                }
            }
        }

        private void OnActivityShopBtnClick()
        {
            StoreDataItem storeDataByType = Singleton<StoreModule>.Instance.GetStoreDataByType(UIShopType.SHOP_ACTIVITY);
            if ((storeDataByType != null) && storeDataByType.isOpen)
            {
                Singleton<MainUIManager>.Instance.ShowPage(new ShopPageContext(UIShopType.SHOP_ACTIVITY), UIType.Page);
            }
        }

        private void OnActLerpOnWhenNeedPlayNewUnlockLevel()
        {
            MonoActScroller component = base.view.transform.Find("ActPanel/ScrollerView").GetComponent<MonoActScroller>();
            component.lerpSpeed = this._actScrollerLerpSpeed;
            component.stopLerpThreshold = this._actScrollerStopLerpThreshold;
            if ((this._chapterType == 1) && ((this._newUnlockLevelDataList != null) && (this._newUnlockLevelDataList.Count >= 1)))
            {
                if (this._newUnlockLevelAnimationTimer != null)
                {
                    this._newUnlockLevelAnimationTimer.Destroy();
                }
                if (this._shouldChangeActIndex)
                {
                    this.PlayNewUnlockLevelAnimation();
                    this._shouldChangeActIndex = false;
                }
                else
                {
                    this._newUnlockLevelAnimationTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.5f, 0f);
                    this._newUnlockLevelAnimationTimer.timeUpCallback = new Action(this.PlayNewUnlockLevelAnimation);
                    this._newUnlockLevelAnimationTimer.StartRun(false);
                }
            }
        }

        public void OnDoLevelBegin()
        {
            if (this._levelDetailContext != null)
            {
                this._levelDetailContext.Destroy();
                this._levelDetailContext = null;
            }
            this._toShowLevelData = null;
            this._justShowLevelDetail = false;
        }

        private bool OnGetWeekDayActivityDataRsp(GetWeekDayActivityDataRsp rsp)
        {
            if (this._weekDayActivityData != null)
            {
                this.SetupActivityView();
            }
            return false;
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn", false, null);
        }

        private void OnLevelClick(LevelDataItem levelData)
        {
            if (levelData.status != 1)
            {
                LevelDetailDialogContextV2 dialogContext = new LevelDetailDialogContextV2(levelData, levelData.Diffculty);
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.SpecialDialog);
                this._levelDetailContext = dialogContext;
                if (levelData.LevelType == 1)
                {
                    this._showActIndex = new ActDataItem(levelData.ActID).actIndex;
                }
                levelData.isNewLevel = false;
                if (levelData.LevelType == 1)
                {
                    Singleton<MiHoYoGameData>.Instance.LocalData.NeedPlayLevelAnimationSet.Remove(levelData.levelId);
                    Singleton<MiHoYoGameData>.Instance.Save();
                }
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.SetLevelDifficulty)
            {
                return this.OnSetupDifficultyNotify((LevelDiffculty) ((int) ntf.body));
            }
            if (ntf.type == NotifyTypes.StageEnd)
            {
                return this.OnStageEndNotify((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.RefreshChapterSelectPage)
            {
                return this.OnRefreshChapterSelectPage();
            }
            return ((ntf.type == NotifyTypes.ActivtyShopScheduleChange) && this.ShowActivityShopEntry());
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x2e:
                    return this.OnStageEndRsp(pkt.getData<StageEndRsp>());

                case 0x7e:
                    return this.OnGetWeekDayActivityDataRsp(pkt.getData<GetWeekDayActivityDataRsp>());
            }
            return false;
        }

        private bool OnRefreshChapterSelectPage()
        {
            this.SetupView();
            return false;
        }

        private bool OnSetupDifficultyNotify(LevelDiffculty difficulty)
        {
            if (this._chapterType == 1)
            {
                this.difficulty = difficulty;
                this.CheckActIndex();
                this.SetupLevels();
                this.SetupChallengeNum();
                if (this._justShowLevelDetail)
                {
                    this.OnLevelClick(this._toShowLevelData);
                }
                this.SetRectMaskDirty();
            }
            return false;
        }

        private bool OnStageEndNotify(bool shouldCreateNuclearActivityTips)
        {
            this.SetNewUnlockLevelData();
            if ((this._chapterType == 1) && (this._newUnlockLevelDataList.Count > 0))
            {
                MonoActScroller component = base.view.transform.Find("ActPanel/ScrollerView").GetComponent<MonoActScroller>();
                component.onLerpEndCallBack = new Action(this.OnActLerpOnWhenNeedPlayNewUnlockLevel);
                component.lerpSpeed /= this._actScrollerSpeedDownRatio;
                component.stopLerpThreshold *= this._actScrollerSpeedDownRatio * 2f;
                int actIndex = new ActDataItem(this._newUnlockLevelDataList[0].ActID).actIndex;
                this._shouldChangeActIndex = actIndex != this._showActIndex;
                if (actIndex != this._showActIndex)
                {
                    if (this._newUnlockLevelActDelayTimer != null)
                    {
                        this._newUnlockLevelActDelayTimer.Destroy();
                    }
                    this._newUnlockLevelActDelayTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.4f, 0f);
                    this._newUnlockLevelActDelayTimer.timeUpCallback = new Action(this.ActDelayTimerTimeUpCallBack);
                    this._needLerpAfterInitLevels = true;
                }
                else
                {
                    this._needLerpAfterInitLevels = false;
                    this.ActDelayTimerTimeUpCallBack();
                }
            }
            if (shouldCreateNuclearActivityTips)
            {
                this.CreateNuclearActivityTips();
            }
            this.SetRectMaskDirty();
            return false;
        }

        private bool OnStageEndRsp(StageEndRsp rsp)
        {
            Singleton<LevelModule>.Instance.HandleStageEndRspForRetry(rsp);
            this.SetupView();
            return false;
        }

        private void OnWorldMapBtnClick()
        {
            switch (this._chapterType)
            {
                case 1:
                    Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext(this.chapter), UIType.Page);
                    break;

                case 2:
                case 3:
                    Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext(this._weekDayActivityData), UIType.Page);
                    break;
            }
        }

        private void PlayNewUnlockLevelAnimation()
        {
            if (this._newUnlockLevelDataList != null)
            {
                foreach (LevelDataItem item in this._newUnlockLevelDataList)
                {
                    if (((this.levelTransDict != null) && this.levelTransDict.ContainsKey(item)) && (this.levelTransDict[item] != null))
                    {
                        MonoLevelView component = this.levelTransDict[item].GetComponent<MonoLevelView>();
                        if (component != null)
                        {
                            component.PlayNewUnlockAnimation(0.5f);
                        }
                        Singleton<MiHoYoGameData>.Instance.LocalData.NeedPlayLevelAnimationSet.Remove(item.levelId);
                    }
                }
                Singleton<MiHoYoGameData>.Instance.Save();
                this._newUnlockLevelDataList.Clear();
            }
            base.view.transform.Find("ActPanel/ScrollerView").GetComponent<MonoLevelScroller>().onLerpEndCallBack = null;
        }

        private void SaveChapterStatus()
        {
            if (this._chapterType == 1)
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.LastChapterID = this.chapter.chapterId;
                Singleton<MiHoYoGameData>.Instance.LocalData.LastActIndex = this._actScroller.centerIndex;
                Singleton<MiHoYoGameData>.Instance.LocalData.LastDifficulty = this.difficulty;
                Singleton<MiHoYoGameData>.Instance.Save();
            }
            else if (((this._chapterType == 2) || (this._chapterType == 3)) || (this._chapterType == 5))
            {
                this._weekDayActivityData = this._actScroller.GetCenterTransform().GetComponent<MonoActButton>().GetWeekDayActivityData();
                this.ShowActivityShopEntry();
            }
        }

        private void SetNewUnlockLevelData()
        {
            if (this._chapterType == 1)
            {
                this._newUnlockLevelDataList = new List<LevelDataItem>();
                List<LevelDataItem> levelList = this.chapter.GetLevelList(this.difficulty);
                if (<>f__am$cache14 == null)
                {
                    <>f__am$cache14 = (lo, ro) => lo.levelId - ro.levelId;
                }
                levelList.Sort(<>f__am$cache14);
                foreach (LevelDataItem item in levelList)
                {
                    if (((item.levelId != 0x2775) && (item.status != 1)) && Singleton<MiHoYoGameData>.Instance.LocalData.NeedPlayLevelAnimationSet.Contains(item.levelId))
                    {
                        this._newUnlockLevelDataList.Add(item);
                    }
                }
            }
        }

        private void SetRectMaskDirty()
        {
            base.view.transform.Find("LevelPanel/ScrollerView").GetComponent<RectMask>().SetGraphicDirty();
            base.view.transform.Find("ActPanel/ScrollerView").GetComponent<RectMask>().SetGraphicDirty();
        }

        private void SetupActivityView()
        {
            this.levelTransDict = new Dictionary<LevelDataItem, Transform>();
            bool hasInProgressNuclearActivity = false;
            SeriesDataItem weekDaySeriesByActivityID = Singleton<LevelModule>.Instance.GetWeekDaySeriesByActivityID(this._weekDayActivityData.GetActivityID());
            List<WeekDayActivityDataItem> showActivityListBySeries = this.GetShowActivityListBySeries(weekDaySeriesByActivityID, out hasInProgressNuclearActivity);
            for (int i = 0; i < showActivityListBySeries.Count; i++)
            {
                if (showActivityListBySeries[i] == this._weekDayActivityData)
                {
                    this._showActIndex = i;
                    break;
                }
            }
            this._showActIndex = Mathf.Clamp(this._showActIndex, 0, showActivityListBySeries.Count - 1);
            base.view.transform.Find("Path/2").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_Event", new object[0]) + " >";
            base.view.transform.Find("Path/3").GetComponent<Text>().text = weekDaySeriesByActivityID.title;
            base.view.transform.Find("DifficultySelect").gameObject.SetActive(false);
            base.view.transform.Find("InfoPanel").gameObject.SetActive(true);
            base.view.transform.Find("Title/DescPanel/Desc").GetComponent<Text>().text = weekDaySeriesByActivityID.title;
            base.view.transform.Find("Title/DescPanel/Desc").GetComponent<TypewriterEffect>().RestartRead();
            base.view.transform.Find("Title/DescPanel/IconEx").gameObject.SetActive(hasInProgressNuclearActivity);
            base.view.transform.Find("ActivityBG/ExBG").gameObject.SetActive(false);
            Transform levelScrollTrans = base.view.transform.Find("LevelPanel/ScrollerView");
            Transform transform2 = base.view.transform.Find("ActPanel/ScrollerView");
            MonoActScroller component = transform2.GetComponent<MonoActScroller>();
            this._actScroller = component;
            this.ClearActivityContent();
            if (showActivityListBySeries.Count <= 0)
            {
                levelScrollTrans.gameObject.SetActive(false);
                transform2.gameObject.SetActive(false);
            }
            else
            {
                levelScrollTrans.gameObject.SetActive(true);
                transform2.gameObject.SetActive(true);
                foreach (WeekDayActivityDataItem item2 in showActivityListBySeries)
                {
                    Transform transform = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/Act", BundleType.RESOURCE_FILE)).transform;
                    transform.SetParent(transform2.Find("Content"), false);
                    List<LevelDataItem> weekDayActivityLevelsByID = Singleton<LevelModule>.Instance.GetWeekDayActivityLevelsByID(item2.GetActivityID());
                    transform.GetComponent<MonoActButton>().SetupActivityView(item2, base.view.transform.Find("InfoPanel").GetComponent<MonoActivityInfoPanel>(), weekDayActivityLevelsByID, levelScrollTrans, new LevelBtnClickCallBack(this.OnLevelClick), base.view.transform.Find("ActivityBG"), this.levelTransDict);
                }
                levelScrollTrans.GetComponent<MonoLevelScroller>().InitLevelPanels(this._showActIndex, showActivityListBySeries.Count, null, false);
                transform2.GetComponent<MonoActScroller>().InitActs(this._showActIndex, showActivityListBySeries.Count, new Action(this.SaveChapterStatus), false);
                if (this._justShowLevelDetail)
                {
                    this.OnLevelClick(this._toShowLevelData);
                }
                this.SetRectMaskDirty();
                this.ShowActivityShopEntry();
            }
        }

        private void SetupChallengeNum()
        {
            base.view.transform.Find("ChallengeNum/Num").GetComponent<Text>().text = "x" + this.chapter.GetTotalFinishedChanllengeNum(this.difficulty);
        }

        private void SetupLevels()
        {
            this.levelTransDict = new Dictionary<LevelDataItem, Transform>();
            Transform levelScrollTrans = base.view.transform.Find("LevelPanel/ScrollerView");
            Transform transform2 = base.view.transform.Find("ActPanel/ScrollerView");
            MonoActScroller component = transform2.GetComponent<MonoActScroller>();
            this._actScroller = component;
            this.ClearLevelsContent();
            Dictionary<int, List<LevelDataItem>> showLevelOfActs = this.GetShowLevelOfActs();
            if (showLevelOfActs.Count <= 0)
            {
                levelScrollTrans.gameObject.SetActive(false);
                transform2.gameObject.SetActive(false);
            }
            else
            {
                levelScrollTrans.gameObject.SetActive(true);
                transform2.gameObject.SetActive(true);
                foreach (LevelDataItem item in this.chapter.GetAllLevelList())
                {
                    if (Singleton<LevelModule>.Instance.GetLevelDropItemIDList(item.levelId) == null)
                    {
                        Singleton<NetworkManager>.Instance.RequestChapterDropList(this.chapter);
                        break;
                    }
                }
                int totalFinishedChanllengeNum = this.chapter.GetTotalFinishedChanllengeNum(this.difficulty);
                List<int> list = Enumerable.ToList<int>(showLevelOfActs.Keys);
                list.Sort();
                foreach (int num2 in list)
                {
                    ActDataItem actData = new ActDataItem(num2);
                    Transform transform = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/Act", BundleType.RESOURCE_FILE)).transform;
                    transform.SetParent(transform2.Find("Content"), false);
                    List<LevelDataItem> levels = showLevelOfActs[num2];
                    transform.GetComponent<MonoActButton>().SetupActView(actData, levels, levelScrollTrans, new LevelBtnClickCallBack(this.OnLevelClick), base.view.transform.Find("ActivityBG"), this.levelTransDict, totalFinishedChanllengeNum);
                }
                if (this._newUnlockLevelDataList != null)
                {
                    transform2.GetComponent<MonoLevelScroller>().onLerpEndCallBack = new Action(this.OnActLerpOnWhenNeedPlayNewUnlockLevel);
                }
                levelScrollTrans.GetComponent<MonoLevelScroller>().InitLevelPanels(this._showActIndex, list.Count, null, this._needLerpAfterInitLevels);
                transform2.GetComponent<MonoActScroller>().InitActs(this._showActIndex, list.Count, new Action(this.SaveChapterStatus), this._needLerpAfterInitLevels);
                this._needLerpAfterInitLevels = false;
            }
        }

        protected override bool SetupView()
        {
            if (Mathf.Approximately(this._actScrollerLerpSpeed, 0f))
            {
                this._actScrollerLerpSpeed = base.view.transform.Find("ActPanel/ScrollerView").GetComponent<MonoActScroller>().lerpSpeed;
                this._actScrollerStopLerpThreshold = base.view.transform.Find("ActPanel/ScrollerView").GetComponent<MonoActScroller>().stopLerpThreshold;
            }
            switch (this._chapterType)
            {
                case 1:
                    base.view.transform.Find("Path/2").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_Story", new object[0]) + " >";
                    base.view.transform.Find("Path/3").GetComponent<Text>().text = this.chapter.Title;
                    base.view.transform.Find("Title/DescPanel/Desc").GetComponent<Text>().text = this.chapter.Title;
                    base.view.transform.Find("Title/DescPanel/Desc").GetComponent<TypewriterEffect>().RestartRead();
                    base.view.transform.Find("DifficultySelect").gameObject.SetActive(true);
                    base.view.transform.Find("DifficultySelect").GetComponent<MonoLevelDifficultyPanel>().Init(this.difficulty, this.chapter);
                    base.view.transform.Find("InfoPanel").gameObject.SetActive(false);
                    base.view.transform.Find("Title/DescPanel/IconEx").gameObject.SetActive(false);
                    base.view.transform.Find("EventShopBtn").gameObject.SetActive(false);
                    this.SetupChallengeNum();
                    if (!this._justShowLevelDetail)
                    {
                        this.SaveChapterStatus();
                        break;
                    }
                    break;

                case 2:
                case 3:
                    this.SetupActivityView();
                    break;
            }
            base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn", false, null);
            base.view.transform.Find("ChallengeNum").gameObject.SetActive(this._chapterType == 1);
            this.SetRectMaskDirty();
            Singleton<LevelModule>.Instance.RetrySendLevelEndReq();
            return false;
        }

        private bool ShowActivityShopEntry()
        {
            if (this._weekDayActivityData != null)
            {
                bool flag = false;
                StoreDataItem storeDataByType = Singleton<StoreModule>.Instance.GetStoreDataByType(UIShopType.SHOP_ACTIVITY);
                if ((storeDataByType != null) && storeDataByType.isOpen)
                {
                    flag = true;
                }
                base.view.transform.Find("EventShopBtn").gameObject.SetActive(this._weekDayActivityData.ShowActivityShopEntry() && flag);
            }
            return false;
        }
    }
}

