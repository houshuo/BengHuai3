namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class GMTalkDialogContext : BaseDialogContext
    {
        private List<string> _showCommandList = new List<string> { "FETCH ALL", "FETCH ALLAVATAR", "CLEAR PACK", "CLEAR STAGE", "CLEAR ALL", "CLEAR GUIDE", "FSALL", "CLEAR SIGN", "CLEAR ACTIVITY" };
        private const string BUTTON_PREFAB_PATH = "UI/GMTalk/GMTalkButton";
        public const string NETWORK_DELAY_OFF = "模拟网络延迟<color=red>(已经关闭)</color>";
        public const string NETWORK_DELAY_ON = "模拟网络延迟<color=red>(已经开启)({0}秒)</color>";

        public GMTalkDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "GMTalkDialogContext",
                viewPrefabPath = "UI/GMTalk/GMTalkDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/SingleButton/Button").GetComponent<Button>(), new UnityAction(this.OnOkButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/ClearLocalData").GetComponent<Button>(), new UnityAction(this.OnClearLocalDataCallBack));
            base.BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/SkipAllTutorial").GetComponent<Button>(), new UnityAction(this.OnSkipAllTutorialCallBack));
            base.BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/SkipAllUITutorial").GetComponent<Button>(), new UnityAction(this.OnSkipAllUITutorialCallback));
            base.BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/SkipAllLevelTutorial").GetComponent<Button>(), new UnityAction(this.OnSkipAllLevelTutorialCallback));
            base.BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/UnlockAllCG").GetComponent<Button>(), new UnityAction(this.OnUnlockAllCGCallBack));
            base.BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/SkipAllPlot").GetComponent<Button>(), new UnityAction(this.OnSkipAllPlotCallback));
            base.BindViewCallback(base.view.transform.Find("GMBtnsPanel/Content/ShittyNetwork").GetComponent<Button>(), new UnityAction(this.OnNetworkDelayCallback));
            base.BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/TestAllEquip").GetComponent<Button>(), new UnityAction(this.OnTestAllEquipCallBack));
            base.BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/GoodFeel1").GetComponent<Button>(), new UnityAction(this.OnOriginGoodFeelCallback));
            base.BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/GoodFeel10").GetComponent<Button>(), new UnityAction(this.OnGoodFeel10Callback));
            base.BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/GoodFeel100").GetComponent<Button>(), new UnityAction(this.OnGoodFeel100Callback));
            base.BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/GoodFeel1000").GetComponent<Button>(), new UnityAction(this.OnGoodFeel1000Callback));
            base.BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/GoodFeel10000").GetComponent<Button>(), new UnityAction(this.OnGoodFeel10000Callback));
            base.BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/RestartGame").GetComponent<Button>(), new UnityAction(this.OnRestartGameCallBack));
            base.BindViewCallback(base.view.transform.Find("AutoTestPanel/Content/BenchmarkSwitches").GetComponent<Button>(), new UnityAction(this.OnToggleBenchmarkCallBack));
        }

        private void GMTalkButtonCallback(string command)
        {
            Singleton<TestModule>.Instance.RequestGMTalk(command);
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        public void OnClearLocalDataCallBack()
        {
            MiHoYoGameData.DeleteAllData();
            this.Destroy();
        }

        public void OnGoodFeel10000Callback()
        {
        }

        public void OnGoodFeel1000Callback()
        {
        }

        public void OnGoodFeel100Callback()
        {
        }

        public void OnGoodFeel10Callback()
        {
        }

        public void OnNetworkDelayCallback()
        {
            GlobalVars.DEBUG_NETWORK_DELAY_LEVEL = (GlobalVars.DEBUG_NETWORK_DELAY_LEVEL + 1) % 5;
            this.SyncNetworkDelayDisplay();
        }

        public void OnOkButtonCallBack()
        {
            string text = base.view.transform.Find("Dialog/InputField").GetComponent<InputField>().text;
            if (MonoMemoryProfiler.ParseCommand(text))
            {
                MonoMemoryProfiler.CreateMemoryProfiler();
            }
            char[] separator = new char[] { ";"[0] };
            string[] strArray = text.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                Singleton<TestModule>.Instance.RequestGMTalk(strArray[i].Trim());
            }
            this.Destroy();
        }

        public void OnOriginGoodFeelCallback()
        {
        }

        public void OnRestartGameCallBack()
        {
            this.Destroy();
            GeneralLogicManager.RestartGame();
        }

        public void OnSkipAllLevelTutorialCallback()
        {
            this.SkipAllLevelTutorial();
            this.Destroy();
        }

        public void OnSkipAllPlotCallback()
        {
            this.SkipAllPlot();
            this.Destroy();
        }

        public void OnSkipAllTutorialCallBack()
        {
            this.SkipAllUITutorial();
            this.SkipAllLevelTutorial();
            this.SkipAllPlot();
            this.Destroy();
        }

        public void OnSkipAllUITutorialCallback()
        {
            this.SkipAllUITutorial();
            this.Destroy();
        }

        public void OnTestAllEquipCallBack()
        {
            this.Destroy();
            Singleton<ApplicationManager>.Instance.StartCoroutine(this.TestAllEquip());
        }

        public void OnToggleBenchmarkCallBack()
        {
            this.Destroy();
            MonoBenchmarkSwitches switches = UnityEngine.Object.FindObjectOfType<MonoBenchmarkSwitches>();
            if (switches == null)
            {
                GameObject target = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(target);
                target.name = "__Benchmark";
                target.AddComponent<MonoBenchmarkSwitches>();
            }
            else
            {
                UnityEngine.Object.Destroy(switches.gameObject);
            }
        }

        public void OnUnlockAllCGCallBack()
        {
            this.UnlockAllCG();
            this.Destroy();
        }

        protected override bool SetupView()
        {
            EventSystem.current.SetSelectedGameObject(base.view.transform.Find("Dialog/InputField").gameObject);
            Transform parent = base.view.transform.Find("GMBtnsPanel/Content");
            foreach (string str in this._showCommandList)
            {
                GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/GMTalk/GMTalkButton", BundleType.RESOURCE_FILE));
                obj2.transform.SetParent(parent, false);
                obj2.GetComponent<MonoGMTalkButton>().SetupView(str, new MonoGMTalkButton.ButtonCallBack(this.GMTalkButtonCallback));
            }
            this.SyncNetworkDelayDisplay();
            return false;
        }

        private void SkipAllLevelTutorial()
        {
            foreach (LevelTutorialMetaData data in LevelTutorialMetaDataReader.GetItemList())
            {
                if ((data.tutorialId > LevelTutorialModule.BASE_LEVEL_TUTORIAL_ID) && (data.tutorialId < LevelPlotModule.BASE_LEVEL_PLOT_ID))
                {
                    Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint) data.tutorialId, true);
                }
            }
        }

        private void SkipAllPlot()
        {
            foreach (PlotMetaData data in PlotMetaDataReader.GetItemList())
            {
                if ((data.plotID > LevelPlotModule.BASE_LEVEL_PLOT_ID) && (data.plotID < CGModule.BASE_CG_ID))
                {
                    Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint) data.plotID, true);
                }
            }
        }

        private void SkipAllUITutorial()
        {
            foreach (TutorialData data in TutorialDataReader.GetItemList())
            {
                if (data.id < LevelTutorialModule.BASE_LEVEL_TUTORIAL_ID)
                {
                    Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint) data.id, true);
                }
            }
        }

        private void SyncNetworkDelayDisplay()
        {
            base.view.transform.Find("GMBtnsPanel/Content/ShittyNetwork/Text").GetComponent<Text>().text = (GlobalVars.DEBUG_NETWORK_DELAY_LEVEL <= 0) ? "模拟网络延迟<color=red>(已经关闭)</color>" : string.Format("模拟网络延迟<color=red>(已经开启)({0}秒)</color>", GlobalVars.DEBUG_NETWORK_DELAY_LEVEL);
        }

        [DebuggerHidden]
        private IEnumerator TestAllEquip()
        {
            return new <TestAllEquip>c__Iterator5E();
        }

        private void UnlockAllCG()
        {
            foreach (CgMetaData data in CgMetaDataReader.GetItemList())
            {
                if (data.CgID > CGModule.BASE_CG_ID)
                {
                    Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint) data.CgID, true);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <TestAllEquip>c__Iterator5E : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal List<StorageDataItemBase>.Enumerator <$s_1700>__1;
            internal StorageDataItemBase <item>__2;
            internal float <waitSec>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 3:
                    case 4:
                        try
                        {
                        }
                        finally
                        {
                            this.<$s_1700>__1.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.<waitSec>__0 = 0.35f;
                        if (!(Singleton<MainUIManager>.Instance.CurrentPageContext is MainPageContext))
                        {
                            Singleton<MainUIManager>.Instance.CurrentPageContext.BackToMainMenuPage();
                        }
                        this.$current = new WaitForSeconds(this.<waitSec>__0);
                        this.$PC = 1;
                        goto Label_01B1;

                    case 1:
                        Singleton<MainUIManager>.Instance.ShowPage(new StorageShowPageContext(), UIType.Page);
                        this.$current = new WaitForSeconds(this.<waitSec>__0);
                        this.$PC = 2;
                        goto Label_01B1;

                    case 2:
                        this.<$s_1700>__1 = Singleton<StorageModule>.Instance.UserStorageItemList.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 3:
                    case 4:
                        break;

                    default:
                        goto Label_01AF;
                }
                try
                {
                    switch (num)
                    {
                        case 3:
                            goto Label_0111;
                    }
                    while (this.<$s_1700>__1.MoveNext())
                    {
                        this.<item>__2 = this.<$s_1700>__1.Current;
                        UIUtil.ShowItemDetail(this.<item>__2, false, true);
                        this.$current = new WaitForSeconds(this.<waitSec>__0);
                        this.$PC = 3;
                        flag = true;
                        goto Label_01B1;
                    Label_0111:
                        if ((this.<item>__2 is WeaponDataItem) || (this.<item>__2 is StigmataDataItem))
                        {
                            Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
                        }
                        else
                        {
                            Singleton<MainUIManager>.Instance.CurrentPageContext.dialogContextList[0].Destroy();
                        }
                        this.$current = new WaitForSeconds(this.<waitSec>__0);
                        this.$PC = 4;
                        flag = true;
                        goto Label_01B1;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.<$s_1700>__1.Dispose();
                }
                this.$PC = -1;
            Label_01AF:
                return false;
            Label_01B1:
                return true;
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

