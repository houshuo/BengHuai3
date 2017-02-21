namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UniRx;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MainUIManager
    {
        private ViewCache _dialogViewCache;
        private EmptyPageContext _emtryPageContext;
        private int _loadPageDialogTimes;
        private Stack<BasePageContext> _pageContextStack;
        private Stack<BasePageContext> _pageContextStackStash;
        private ViewCache _pageViewCache;
        private Dictionary<string, Action> _sceneLoadCallBackDict;
        private Dictionary<Tuple<string, string>, GameObject> _uiEffectContainerDict;
        private bool _useViewCache;
        private Coroutine _waitMoveToSceneCoroutine;
        private ViewCache _widgetViewCache;
        public bool spaceShipVisibleOnPreviesPage;
        private const string UI_EFFECT_CONTAINER_TAG = "UIEffectContainer";

        private MainUIManager()
        {
            this.bDestroyUntilNotify = true;
            this.bShowLoadingTips = true;
            this._pageContextStack = new Stack<BasePageContext>();
            this._pageContextStackStash = new Stack<BasePageContext>();
            this._emtryPageContext = new EmptyPageContext();
            this._useViewCache = MainUIData.USE_VIEW_CACHING;
            if (this._useViewCache)
            {
                this._pageViewCache = new ViewCache(10);
                this._dialogViewCache = new ViewCache(10);
                this._widgetViewCache = new ViewCache(5);
            }
            this._uiEffectContainerDict = new Dictionary<Tuple<string, string>, GameObject>();
            this._sceneLoadCallBackDict = new Dictionary<string, Action>();
        }

        public void BackPage()
        {
            if ((this._pageContextStack.Count != 0) && ((this._pageContextStack.Count != 1) || !(this._pageContextStack.Peek() is MainPageContext)))
            {
                BasePageContext context = this._pageContextStack.Pop();
                this.spaceShipVisibleOnPreviesPage = context.spaceShipVisible();
                context.Destroy();
                if (this._pageContextStack.Count > 0)
                {
                    BasePageContext context2 = this._pageContextStack.Peek();
                    if (context2.view != null)
                    {
                        context2.SetActive(true);
                        context2.OnLandedFromBackPage();
                    }
                    else
                    {
                        context2.StartUp(this.SceneCanvas.transform, null);
                    }
                    if (context2.spaceShipVisible())
                    {
                        UIUtil.SpaceshipCheckWeather();
                    }
                }
                else
                {
                    this._emtryPageContext.SetActive(true);
                }
            }
        }

        public bool BackPageTo(string contextName)
        {
            if (this._pageContextStack.Count <= 0)
            {
                SuperDebug.VeryImportantError("The page stack is empty!!!");
                return false;
            }
            if (contextName != "MainPageContext")
            {
                bool flag = false;
                foreach (BasePageContext context in this._pageContextStack)
                {
                    if (context.config.contextName == contextName)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    SuperDebug.VeryImportantError("Can't find page in stack: " + contextName);
                    return false;
                }
            }
            this.spaceShipVisibleOnPreviesPage = this._pageContextStack.Peek().spaceShipVisible();
            while ((this._pageContextStack.Count > 0) && (this._pageContextStack.Peek().config.contextName != contextName))
            {
                this._pageContextStack.Pop().Destroy();
            }
            if (this._pageContextStack.Count > 0)
            {
                BasePageContext context4 = this._pageContextStack.Peek();
                if (context4.view != null)
                {
                    context4.SetActive(true);
                    context4.OnLandedFromBackPage();
                }
                else
                {
                    context4.StartUp(this.SceneCanvas.transform, null);
                }
            }
            else
            {
                this._pageContextStackStash.Clear();
                this._emtryPageContext.Destroy();
                this.ShowPage(new MainPageContext(), UIType.Page);
            }
            return true;
        }

        public void BackToMainMenuPage()
        {
            if (this.SceneCanvas is MonoIslandUICanvas)
            {
                this.MoveToNextScene("MainMenuWithSpaceship", false, true, true, null, true);
            }
            else if (!this.BackPageTo("MainPageContext"))
            {
                this._pageContextStack.Clear();
                this._pageContextStackStash.Clear();
                this._emtryPageContext.Destroy();
                this.ShowPage(new MainPageContext(), UIType.Page);
            }
        }

        public void CheckResouceBeforeLoad()
        {
            this._loadPageDialogTimes++;
            if (this._loadPageDialogTimes >= 10)
            {
                Resources.UnloadUnusedAssets();
                this._loadPageDialogTimes = 0;
            }
        }

        private void ClearAllContext()
        {
            foreach (BasePageContext context in this._pageContextStack)
            {
                context.Clear();
            }
            this._emtryPageContext.Clear();
            this.SceneCanvas.ClearAllWidgetContext();
        }

        public void ClearViewCache()
        {
            if (this._useViewCache)
            {
                this._pageViewCache.ClearLRUCache();
                this._dialogViewCache.ClearLRUCache();
                this._widgetViewCache.ClearLRUCache();
            }
        }

        public void CreateContextFromStash()
        {
            if (this.HasContextInStash())
            {
                this._pageContextStack = this._pageContextStackStash;
                this._pageContextStackStash = new Stack<BasePageContext>();
                this._pageContextStack.Peek().StartUp(this.SceneCanvas.transform, null);
            }
        }

        [DebuggerHidden]
        private IEnumerator DoMoveToNextScene(string sceneName, bool toKeepContextStack = false)
        {
            return new <DoMoveToNextScene>c__Iterator39 { toKeepContextStack = toKeepContextStack, sceneName = sceneName, <$>toKeepContextStack = toKeepContextStack, <$>sceneName = sceneName, <>f__this = this };
        }

        public string GetAllPageNamesInStack()
        {
            StringBuilder builder = new StringBuilder();
            foreach (BasePageContext context in this._pageContextStack)
            {
                builder.Append(context.ToString());
                builder.Append(" ");
            }
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        public Action GetCallBackWhenSceneLoaded(string sceneName)
        {
            Action action;
            this._sceneLoadCallBackDict.TryGetValue(sceneName, out action);
            return action;
        }

        public MonoInLevelUICanvas GetInLevelUICanvas()
        {
            return (this.SceneCanvas as MonoInLevelUICanvas);
        }

        public BaseMonoCanvas GetMainCanvas()
        {
            return this.SceneCanvas;
        }

        public Transform GetUIHolder(UIType uiType)
        {
            switch (uiType)
            {
                case UIType.Page:
                    return this.SceneCanvas.transform.Find("Pages");

                case UIType.SpecialDialog:
                    return this.SceneCanvas.transform.Find("SpecialDialogs");

                case UIType.SuspendBar:
                    return this.SceneCanvas.transform.Find("SuspendBars");

                case UIType.Dialog:
                    return this.SceneCanvas.transform.Find("Dialogs");

                case UIType.Root:
                    return null;

                case UIType.MostFront:
                    return this.SceneCanvas.transform;
            }
            return null;
        }

        public bool HasContextInStash()
        {
            return (this._pageContextStackStash.Count > 0);
        }

        private GameObject LoadAndInstantiateView(string path)
        {
            return UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(path, BundleType.RESOURCE_FILE));
        }

        public GameObject LoadInstancedView(BaseContext context)
        {
            ContextPattern config = context.config;
            if (!this._useViewCache)
            {
                return this.LoadAndInstantiateView(config.viewPrefabPath);
            }
            GameObject obj2 = null;
            switch (context.uiType)
            {
                case UIType.Page:
                    obj2 = this._pageViewCache.LoadInstancedView(config);
                    break;

                case UIType.Dialog:
                    obj2 = this._dialogViewCache.LoadInstancedView(config);
                    break;

                default:
                    obj2 = this._widgetViewCache.LoadInstancedView(config);
                    break;
            }
            SuperDebug.VeryImportantAssert(obj2 != null, "failed to create view for: " + config.viewPrefabPath);
            return obj2;
        }

        public void LockUI(bool toLock, float timeSpan = 3f)
        {
            if (this.SceneCanvas != null)
            {
                Transform transform = this.SceneCanvas.transform.Find("BlockPanel");
                if (transform != null)
                {
                    if (toLock)
                    {
                        transform.GetComponent<MonoBlockPanel>().SetTimeSpanTakeEffect(timeSpan);
                    }
                    else
                    {
                        transform.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void MoveToNextScene(string sceneName, bool toKeepContextStack = false, bool isAsync = false, bool destroyUntilNotify = true, Action onSceneLoadedCallBack = null, bool showLoadingTips = true)
        {
            this._sceneLoadCallBackDict[sceneName] = onSceneLoadedCallBack;
            if (this._waitMoveToSceneCoroutine == null)
            {
                if (isAsync)
                {
                    this.SceneAfterLoading = sceneName;
                    this.bDestroyUntilNotify = destroyUntilNotify;
                    this.bShowLoadingTips = showLoadingTips;
                    this._waitMoveToSceneCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.DoMoveToNextScene("Loading", toKeepContextStack));
                }
                else
                {
                    this._waitMoveToSceneCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.DoMoveToNextScene(sceneName, toKeepContextStack));
                }
            }
        }

        public void PopTopPageOnly()
        {
            if (this._pageContextStack.Count > 0)
            {
                BasePageContext context = this._pageContextStack.Pop();
                context.DestroyContextOnly();
                this.spaceShipVisibleOnPreviesPage = context.spaceShipVisible();
            }
        }

        public void ReleaseInstancedView(BaseContext context)
        {
            ContextPattern config = context.config;
            GameObject view = context.view;
            if (view != null)
            {
                if (!this._useViewCache)
                {
                    if (((view != null) && !string.IsNullOrEmpty(config.viewPrefabPath)) && !config.dontDestroyView)
                    {
                        UnityEngine.Object.Destroy(view);
                    }
                }
                else
                {
                    switch (context.uiType)
                    {
                        case UIType.Page:
                            this._pageViewCache.ReleaseInstancedView(view, config);
                            return;

                        case UIType.Dialog:
                            this._dialogViewCache.ReleaseInstancedView(view, config);
                            return;
                    }
                    this._widgetViewCache.ReleaseInstancedView(view, config);
                }
            }
        }

        public void ResetOnMoveToNextScene(string sceneName)
        {
            this.ResetViewCache();
            this._useViewCache = !(sceneName == "TestLevel01") ? MainUIData.USE_VIEW_CACHING : false;
            this._pageContextStack = new Stack<BasePageContext>();
            Singleton<NotifyManager>.Instance.ClearAllContext();
            this.SceneCanvas = null;
            if (Singleton<TutorialModule>.Instance != null)
            {
                Singleton<TutorialModule>.Instance.SetTutorialFlag(false);
            }
            this._loadPageDialogTimes = 0;
            Singleton<ApplicationManager>.Instance.StopAllCoroutines();
        }

        public void ResetSceneLoadedCallBack(string sceneName)
        {
            if (this._sceneLoadCallBackDict.ContainsKey(sceneName))
            {
                this._sceneLoadCallBackDict.Remove(sceneName);
            }
        }

        public void ResetViewCache()
        {
            if (this._useViewCache)
            {
                this._pageViewCache.Reset();
                this._dialogViewCache.Reset();
                this._widgetViewCache.Reset();
            }
        }

        public void SetMainCanvas(BaseMonoCanvas canvas)
        {
            this.SceneCanvas = canvas;
            this.ResetViewCache();
        }

        public void ShowDialog(BaseDialogContext dialogContext, UIType uiType = 0)
        {
            Transform uIHolder = this.GetUIHolder((uiType != UIType.Any) ? uiType : dialogContext.uiType);
            if (this._pageContextStack.Count > 0)
            {
                BasePageContext context = this._pageContextStack.Peek();
                context.dialogContextList.Add(dialogContext);
                dialogContext.pageContext = context;
            }
            else
            {
                this._emtryPageContext.dialogContextList.Add(dialogContext);
                dialogContext.pageContext = this._emtryPageContext;
            }
            dialogContext.StartUp(this.SceneCanvas.transform, uIHolder);
        }

        public void ShowPage(BasePageContext context, UIType uiType = 1)
        {
            if (this._pageContextStack.Count > 0)
            {
                BasePageContext context2 = this._pageContextStack.Peek();
                context2.SetActive(false);
                this.spaceShipVisibleOnPreviesPage = context2.spaceShipVisible();
            }
            else
            {
                this._emtryPageContext.SetActive(false);
            }
            this._pageContextStack.Push(context);
            context.StartUp(this.SceneCanvas.transform, this.GetUIHolder(uiType));
        }

        public void ShowUIEffect(string contextName, string effectPath)
        {
            Transform parent = GameObject.Find("UIEffectContainer").transform;
            Tuple<string, string> key = new Tuple<string, string>(contextName, effectPath);
            if (!this._uiEffectContainerDict.ContainsKey(key) || (this._uiEffectContainerDict[key] == null))
            {
                GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(effectPath));
                if (obj2 != null)
                {
                    obj2.transform.SetParent(parent, false);
                    this._uiEffectContainerDict[key] = obj2;
                }
            }
            else
            {
                foreach (ParticleSystem system in this._uiEffectContainerDict[key].GetComponentsInChildren<ParticleSystem>())
                {
                    system.Play();
                    GeneralGameObjectSound component = system.GetComponent<GeneralGameObjectSound>();
                    if (component != null)
                    {
                        Singleton<WwiseAudioManager>.Instance.Post(component.enterEventName, null, null, null);
                    }
                }
            }
        }

        public void ShowWidget(BaseWidgetContext widget, UIType uiType = 0)
        {
            widget.StartUp(this.SceneCanvas.transform, this.GetUIHolder((uiType != UIType.Any) ? uiType : widget.uiType));
        }

        public bool bDestroyUntilNotify { get; private set; }

        public bool bShowLoadingTips { get; private set; }

        public BasePageContext CurrentPageContext
        {
            get
            {
                if ((this._pageContextStack != null) && (this._pageContextStack.Count > 0))
                {
                    return this._pageContextStack.Peek();
                }
                return this._emtryPageContext;
            }
        }

        public string SceneAfterLoading { get; private set; }

        public BaseMonoCanvas SceneCanvas { get; private set; }

        [CompilerGenerated]
        private sealed class <DoMoveToNextScene>c__Iterator39 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal string <$>sceneName;
            internal bool <$>toKeepContextStack;
            internal MainUIManager <>f__this;
            internal string sceneName;
            internal bool toKeepContextStack;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (!(this.<>f__this.SceneAfterLoading == "TestLevel01"))
                        {
                            goto Label_005D;
                        }
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00B9;
                }
                if (GlobalDataManager.IsInRefreshDataAsync)
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
            Label_005D:
                if (this.toKeepContextStack)
                {
                    this.<>f__this._pageContextStackStash = this.<>f__this._pageContextStack;
                }
                this.<>f__this._waitMoveToSceneCoroutine = null;
                this.<>f__this.ResetOnMoveToNextScene(this.sceneName);
                this.<>f__this.spaceShipVisibleOnPreviesPage = false;
                SceneManager.LoadScene(this.sceneName);
                this.$PC = -1;
            Label_00B9:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

