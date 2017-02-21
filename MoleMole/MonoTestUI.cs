namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoTestUI : BaseMonoCanvas
    {
        public Avatar3dModelContext avatar3dModelContext;
        public bool disableNetWork;
        public bool isBenchmark;
        public GameObject MainCamera;
        public GameObject MainMenu_SpaceShip;
        public bool testLocalization;

        public void Awake()
        {
            GlobalVars.DISABLE_NETWORK_DEBUG = this.disableNetWork;
            SuperDebug.DEBUG_SWITCH[6] = true;
            MainUIData.USE_VIEW_CACHING = false;
            GeneralLogicManager.InitAll();
            Singleton<MainUIManager>.Instance.SetMainCanvas(this);
            string[] soundBankNames = new string[] { "All_In_One_Bank", "BK_Global", "BK_Events" };
            Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(soundBankNames);
            Singleton<IslandModule>.Instance.InitTechTree();
        }

        private void CreateDoubleButtonTemplate()
        {
            GeneralDialogContext context2 = new GeneralDialogContext {
                type = GeneralDialogContext.ButtonType.DoubleButton,
                title = "TestUI",
                desc = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                notDestroyAfterTouchBG = true
            };
            BaseDialogContext dialogContext = context2;
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        private void CreateSingleButtonTemplate()
        {
            GeneralDialogContext context2 = new GeneralDialogContext {
                type = GeneralDialogContext.ButtonType.SingleButton,
                title = "TestUI",
                desc = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
            };
            BaseDialogContext dialogContext = context2;
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        public override GameObject GetSpaceShipObj()
        {
            return this.MainMenu_SpaceShip;
        }

        private void Login()
        {
            Singleton<NetworkManager>.Instance.QuickLogin();
        }

        private void PostStartHandleBenchmark()
        {
            if (GlobalVars.IS_BENCHMARK || this.isBenchmark)
            {
                Screen.sleepTimeout = -1;
                SuperDebug.CloseAllDebugs();
                GameObject target = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(target);
                target.name = "__Benchmark";
                target.AddComponent<MonoBenchmarkSwitches>();
            }
        }

        public override void Start()
        {
            TestUIContext widget = new TestUIContext(base.gameObject);
            Singleton<MainUIManager>.Instance.ShowWidget(widget, UIType.Any);
            this.TestLocalization();
            if (!this.disableNetWork)
            {
                base.StartCoroutine(Singleton<NetworkManager>.Instance.ConnectGlobalDispatchServer(new Action(this.Login)));
            }
            else
            {
                this.Login();
            }
            this.PostStartHandleBenchmark();
            if (Singleton<EffectManager>.Instance == null)
            {
                Singleton<EffectManager>.Create();
                Singleton<EffectManager>.Instance.InitAtAwake();
                Singleton<EffectManager>.Instance.InitAtStart();
            }
            base.Start();
        }

        public void TestLocalization()
        {
            if (this.testLocalization)
            {
                foreach (Text text in base.GetComponentsInChildren<Text>())
                {
                    if (text.GetComponent<LocalizedText>() == null)
                    {
                        text.text = "XXXXX";
                    }
                }
            }
        }
    }
}

