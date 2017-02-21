namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class ChapterOverviewPageContext : BasePageContext
    {
        private bool _noSpecialStory;
        private WeekDayActivityDataItem _selectedActivityData;
        private ChapterDataItem _selectedChapterData;
        private string _showingTab;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Predicate<ChapterDataItem> <>f__am$cache5;
        [CompilerGenerated]
        private static Predicate<ChapterDataItem> <>f__am$cache6;
        [CompilerGenerated]
        private static Predicate<ChapterDataItem> <>f__am$cache7;
        private const string ACTIVITY_BUTTON_PREFAB = "UI/Menus/Widget/Map/ActivityButton";
        private const string CHAPTER_BUTTON_PREFAB = "UI/Menus/Widget/Map/ChapterButton";
        public const string EVENT_TAB = "Event";
        public const string MAIN_STORY_TAB = "MainStory";
        public const string SPECIAL_STORY_TAB = "SpecialStory";

        public ChapterOverviewPageContext(ChapterDataItem chapterData)
        {
            this._noSpecialStory = true;
            this.InitChapterOverviewPageContext();
            this._selectedChapterData = chapterData;
            this._showingTab = "MainStory";
        }

        public ChapterOverviewPageContext(WeekDayActivityDataItem acitivtyData)
        {
            this._noSpecialStory = true;
            this.InitChapterOverviewPageContext();
            this._selectedActivityData = acitivtyData;
            this._showingTab = "Event";
        }

        public ChapterOverviewPageContext(string tab = "")
        {
            this._noSpecialStory = true;
            this.InitChapterOverviewPageContext();
            this._showingTab = !string.IsNullOrEmpty(tab) ? tab : "MainStory";
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Content/Event/Packed/PackBtn").GetComponent<Button>(), new UnityAction(this.OnEventPackBtnClick));
            base.BindViewCallback(base.view.transform.Find("Content/SpecialStory/Packed/PackBtn").GetComponent<Button>(), new UnityAction(this.OnSpecialStoryPackBtnClick));
            base.BindViewCallback(base.view.transform.Find("Content/MainStory/Packed/PackBtn").GetComponent<Button>(), new UnityAction(this.OnMainStoryPackBtnClick));
        }

        private void InitChapterOverviewPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ChapterOverviewPageContext",
                viewPrefabPath = "UI/Menus/Page/Map/ChapterOverviewPage",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
        }

        private void OnChangeSelectActivity(int index)
        {
            List<WeekDayActivityDataItem> list = new List<WeekDayActivityDataItem>();
            foreach (WeekDayActivityDataItem item in Singleton<LevelModule>.Instance.AllWeekDayActivityList)
            {
                if (item.GetStatus() != ActivityDataItemBase.Status.Unavailable)
                {
                    list.Add(item);
                }
            }
            if (index < list.Count)
            {
                this._selectedActivityData = list[index];
                this._selectedChapterData = null;
            }
        }

        private void OnChangeSelectChapter(int index)
        {
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = x => x.ChapterType == ChapterDataItem.ChpaterType.MainStory;
            }
            List<ChapterDataItem> list = Singleton<LevelModule>.Instance.AllChapterList.FindAll(<>f__am$cache7);
            if (index < list.Count)
            {
                this._selectedChapterData = list[index];
                this._selectedActivityData = null;
            }
        }

        public void OnEventPackBtnClick()
        {
            this.Show("Event");
        }

        private bool OnGetEndlessDataRsp(GetEndlessDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if ((rsp.get_cur_progress_avatar_id_list().Count > 0) || (rsp.get_cur_progress_item_id_list().Count > 0))
                {
                    GeneralDialogContext dialogContext = new GeneralDialogContext {
                        title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                        desc = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessGoToBattleDirrectly", new object[0])
                    };
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = delegate (bool confirmed) {
                            if (confirmed)
                            {
                                Singleton<MainUIManager>.Instance.ShowPage(new EndlessMainPageContext(), UIType.Page);
                                Singleton<MainUIManager>.Instance.ShowPage(new EndlessPreparePageContext(true), UIType.Page);
                            }
                        };
                    }
                    dialogContext.buttonCallBack = <>f__am$cache4;
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
                else
                {
                    Singleton<MainUIManager>.Instance.ShowPage(new EndlessMainPageContext(), UIType.Page);
                }
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]), 2f), UIType.Any);
            }
            return false;
        }

        public bool OnGetStageDataRsp(GetStageDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if (this._selectedChapterData != null)
                {
                    this.SetupMainStory();
                }
                else if (this._selectedActivityData != null)
                {
                    this.SetupEvent();
                }
                else
                {
                    Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext(string.Empty), UIType.Page);
                }
            }
            return false;
        }

        public override void OnLandedFromBackPage()
        {
            this.Show(this._showingTab);
            base.OnLandedFromBackPage();
        }

        public void OnMainStoryPackBtnClick()
        {
            this.Show("MainStory");
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.RequestEnterEndlessActivity) && this.OnRequestEnterEndlessActivity());
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x2a:
                    return this.OnGetStageDataRsp(pkt.getData<GetStageDataRsp>());

                case 140:
                    return this.OnGetEndlessDataRsp(pkt.getData<GetEndlessDataRsp>());
            }
            return false;
        }

        private bool OnRequestEnterEndlessActivity()
        {
            if (Singleton<EndlessModule>.Instance == null)
            {
                Singleton<EndlessModule>.Create();
            }
            Singleton<NetworkManager>.Instance.RequestEndlessData();
            Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(140, null), UIType.Any);
            return false;
        }

        public void OnSpecialStoryPackBtnClick()
        {
            if (this._noSpecialStory)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SpecialStoryLock", new object[0]), 2f), UIType.Any);
            }
            else
            {
                this.Show("SpecialStory");
            }
        }

        private void SetupEvent()
        {
            MonoChapterScroller component = base.view.transform.Find("Content/Event/Expanded/ScrollerView").GetComponent<MonoChapterScroller>();
            component.transform.Find("Content").GetComponent<GridLayoutGroup>().enabled = true;
            List<ActivityDataItemBase> list = new List<ActivityDataItemBase>();
            foreach (WeekDayActivityDataItem item in Singleton<LevelModule>.Instance.AllWeekDayActivityList)
            {
                if ((item.GetStatus() != ActivityDataItemBase.Status.Unavailable) && ((item.GetActivityType() != 3) || (item.GetStatus() == ActivityDataItemBase.Status.InProgress)))
                {
                    list.Add(item);
                }
            }
            list.Add(EndlessActivityDataItem.GetInstance());
            base.view.transform.Find("Content/Event/Packed/PackBtn").GetComponent<Button>().interactable = list.Count > 0;
            this.SetupEventContent(component, list);
        }

        private void SetupEventContent(MonoChapterScroller scroller, List<ActivityDataItemBase> list)
        {
            Transform content = scroller.content;
            content.DestroyChildren();
            if (list.Count != 0)
            {
                int initCenterIndex = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/ActivityButton", BundleType.RESOURCE_FILE));
                    obj2.name = "ChapterButton_" + (i + 1);
                    obj2.GetComponent<MonoActivityEntryButton>().SetupView(list[i]);
                    obj2.transform.SetParent(content, false);
                    if (this._selectedActivityData != null)
                    {
                        if (this._selectedActivityData == list[i])
                        {
                            initCenterIndex = i;
                        }
                    }
                    else if (list[i].GetStatus() == ActivityDataItemBase.Status.InProgress)
                    {
                        initCenterIndex = i;
                    }
                }
                scroller.Init(initCenterIndex, list.Count, new Action<int>(this.OnChangeSelectActivity));
            }
        }

        private void SetupMainStory()
        {
            MonoChapterScroller component = base.view.transform.Find("Content/MainStory/Expanded/ScrollerView").GetComponent<MonoChapterScroller>();
            component.transform.Find("Content").GetComponent<GridLayoutGroup>().enabled = true;
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = x => x.ChapterType == ChapterDataItem.ChpaterType.MainStory;
            }
            List<ChapterDataItem> list = Singleton<LevelModule>.Instance.AllChapterList.FindAll(<>f__am$cache5);
            base.view.transform.Find("Content/MainStory/Packed/PackBtn").GetComponent<Button>().interactable = list.Count > 0;
            this.SetupMainStoryContent(component, list);
        }

        private void SetupMainStoryContent(MonoChapterScroller scroller, List<ChapterDataItem> list)
        {
            Transform content = scroller.content;
            content.DestroyChildren();
            if (list.Count != 0)
            {
                int initCenterIndex = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/ChapterButton", BundleType.RESOURCE_FILE));
                    obj2.name = "ChapterButton_" + (i + 1);
                    obj2.GetComponent<MonoChapterButton>().SetupView(list[i]);
                    obj2.transform.SetParent(content, false);
                    if (this._selectedChapterData != null)
                    {
                        if (list[i] == this._selectedChapterData)
                        {
                            initCenterIndex = i;
                        }
                    }
                    else if (list[i].Unlocked)
                    {
                        initCenterIndex = i;
                    }
                }
                scroller.Init(initCenterIndex, list.Count, new Action<int>(this.OnChangeSelectChapter));
            }
        }

        private void SetupSpecialStory()
        {
            MonoChapterScroller component = base.view.transform.Find("Content/SpecialStory/Expanded/ScrollerView").GetComponent<MonoChapterScroller>();
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = x => x.ChapterType == ChapterDataItem.ChpaterType.SpecialStory;
            }
            List<ChapterDataItem> list = Singleton<LevelModule>.Instance.AllChapterList.FindAll(<>f__am$cache6);
            this._noSpecialStory = list.Count == 0;
            base.view.transform.Find("Content/SpecialStory/Packed/PackBtn").GetComponent<Button>().interactable = true;
            this.SetupMainStoryContent(component, list);
        }

        protected override bool SetupView()
        {
            if (Singleton<LevelModule>.Instance.AllChapterList.Count != 0)
            {
                this.SetupMainStory();
                this.SetupSpecialStory();
                this.SetupEvent();
                this.Show(this._showingTab);
            }
            return false;
        }

        private void Show(string showTab)
        {
            string[] strArray = new string[] { "Event", "SpecialStory", "MainStory" };
            foreach (string str in strArray)
            {
                GameObject gameObject = base.view.transform.Find("Content/" + str).gameObject;
                if (showTab == str)
                {
                    gameObject.GetComponent<Animator>().SetTrigger("ExpandTrigger");
                }
                else if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ExpandedAnim"))
                {
                    gameObject.GetComponent<Animator>().SetTrigger("PackTrigger");
                }
            }
            this._showingTab = showTab;
        }
    }
}

