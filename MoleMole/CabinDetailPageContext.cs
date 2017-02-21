namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class CabinDetailPageContext : BasePageContext
    {
        private Animator _animator;
        private int _animatorCollectTrigger = Animator.StringToHash("CollectTab");
        private int _animatorEnhanceTrigger = Animator.StringToHash("EnhanceTab");
        private int _animatorMiscOverviewTrigger = Animator.StringToHash("MiscOverviewTab");
        private int _animatorMiscTrigger = Animator.StringToHash("MiscTab");
        private int _animatorPowerTrigger = Animator.StringToHash("PowerTab");
        private int _animatorTreeTrigger = Animator.StringToHash("TreeTab");
        private Dictionary<string, int> _animatorTriggerDict;
        private int _animatorVentureTrigger = Animator.StringToHash("VentureTab");
        private bool _bCacheSpawn;
        private CabinDataItemBase _cabinData;
        private CanvasTimer _collectCabinTimer;
        private MiscSubTab _currentMiscSubTab;
        private Transform _iconEffect;
        private float _iconEffectDuration = 2.5f;
        private Vector2 _infoPos;
        private int _playerLevelBefore;
        private StorageDataItemBase _selectedItem;
        private List<StorageDataItemBase> _showItemList;
        private Dictionary<MiscSubTab, Transform> _subTabDict = new Dictionary<MiscSubTab, Transform>();
        private TabManager _tabManager;
        private CanvasTimer _triggerCameraTimer;
        private List<VentureDataItem> _ventureList;
        [CompilerGenerated]
        private static Action <>f__am$cache1A;
        private const string AVATAR_ENHANCE_INFO_PREFAB_PATH = "UI/Menus/Widget/Island/AvatarEnhanceInfo";
        public const string COLLECT_TAB = "CollectTab";
        private const string COLLECT_UI_BG_PREFAB_PATH = "SpriteOutput/CabinBG/CabinCollectBG";
        private const string COLLECT_UI_BLACK_PREFAB_PATH = "SpriteOutput/CabinBG/CabinCollectBlack";
        public string defaultTab = string.Empty;
        public const string ENHANCE_TAB = "EnhanceTab";
        private const string ENHANCE_UI_BG_PREFAB_PATH = "SpriteOutput/CabinBG/CabinEnhanceBG";
        private const string ENHANCE_UI_BLACK_PREFAB_PATH = "SpriteOutput/CabinBG/CabinEnhanceBlack";
        private float FETCH_SCOIN_MISSION_RATIO_TOTAL = 200f;
        public const string MISC_OVERVIEW_TAB = "MiscOverviewTab";
        public const string MISC_TAB = "MiscTab";
        private const string MISC_TAB_PREFAB_PATH = "UI/Menus/Page/Island/MiscTab";
        private const string Misc_UI_BG_PREFAB_PATH = "SpriteOutput/CabinBG/CabinMiscBG";
        private const string Misc_UI_BLACK_PREFAB_PATH = "SpriteOutput/CabinBG/CabinMiscBlack";
        public const string POWER_TAB = "PowerTab";
        private const string POWER_UI_BG_PREFAB_PATH = "SpriteOutput/CabinBG/CabinPowerBG";
        private const string POWER_UI_BLACK_PREFAB_PATH = "SpriteOutput/CabinBG/CabinPowerBlack";
        public static readonly string[] TAB_KEY = new string[] { "PowerTab", "TreeTab", "EnhanceTab", "VentureTab", "MiscTab", "MiscOverviewTab", "CollectTab" };
        public const string TREE_TAB = "TreeTab";
        private const string TREE_TAB_PREFAB_PATH = "UI/Menus/Page/Island/TreeTab";
        public const string VENTURE_TAB = "VentureTab";
        private const string VENTURE_UI_BG_PREFAB_PATH = "SpriteOutput/CabinBG/CabinVentureBG";
        private const string VENTURE_UI_BLACK_PREFAB_PATH = "SpriteOutput/CabinBG/CabinVentureBlack";

        public CabinDetailPageContext(CabinDataItemBase cabinData, bool bCacheSpawn = false)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "CabinDetailPageContext",
                viewPrefabPath = "UI/Menus/Page/Island/CabinDetailPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            this._cabinData = cabinData;
            this._tabManager = new TabManager();
            this._tabManager.onSetActive += new MoleMole.TabManager.OnSetActive(this.OnTabSetActive);
            this._bCacheSpawn = bCacheSpawn;
        }

        public override void BackPage()
        {
            Singleton<IslandModule>.Instance.RegisterVentureInProgress();
            base.view.transform.Find("TreeTab/ScrollView").GetComponent<MonoTechTreeUI>().ClearNodes();
            if (this._triggerCameraTimer != null)
            {
                this._triggerCameraTimer.Destroy();
            }
            this.TriggerSceneCamera(true);
            foreach (string str in TAB_KEY)
            {
                base.view.transform.Find(str).gameObject.SetActive(false);
            }
            base.BackPage();
            base.BackPage();
        }

        public override void BackToMainMenuPage()
        {
            Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true, true, null, true);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtnPower").GetComponent<Button>(), new UnityAction(this.OnPowerTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtnTree").GetComponent<Button>(), new UnityAction(this.OnTreeTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtnEnhance").GetComponent<Button>(), new UnityAction(this.OnEnhanceTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtnVenture").GetComponent<Button>(), new UnityAction(this.OnVentureTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtnMisc").GetComponent<Button>(), new UnityAction(this.OnMiscTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtnMiscOverview").GetComponent<Button>(), new UnityAction(this.OnMiscOverviewTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtnCollect").GetComponent<Button>(), new UnityAction(this.OnCollectTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("VentureTab/InfoPanel/Refresh").GetComponent<Button>(), new UnityAction(this.OnRefreshVentureBtnClick));
            base.BindViewCallback(base.view.transform.Find("CollectTab/FetchBtn/Btn").GetComponent<Button>(), new UnityAction(this.OnFetchScoinBtnClick));
            base.BindViewCallback(base.view.transform.Find("TreeTab/ResetBtn").GetComponent<Button>(), new UnityAction(this.OnResetPowerBtnClick));
            base.BindViewCallback(base.view.transform.Find("MiscTab/FirstSubTab/AddBtn").GetComponent<Button>(), new UnityAction(this.OnMiscAddBtnClick));
            base.BindViewCallback(base.view.transform.Find("MiscTab/SelectSubTab/DisjointBtn").GetComponent<Button>(), new UnityAction(this.OnMiscDisjointBtnClick));
            base.BindViewCallback(base.view.transform.Find("MiscTab/PreviewSubTab/DisjointBtn").GetComponent<Button>(), new UnityAction(this.OnMiscDisjointFinalBtnClick));
            base.BindViewCallback(base.view.transform.Find("MiscTab/SelectSubTab/CancelBtn").GetComponent<Button>(), new UnityAction(this.OnMiscSelectCancelBtnClick));
            base.BindViewCallback(base.view.transform.Find("MiscTab/PreviewSubTab/CancelBtn").GetComponent<Button>(), new UnityAction(this.OnMiscPreviewCancelBtnClick));
        }

        private void ClearOnChangedTab(string tabBefore, string tabAfter)
        {
            int id = this._animatorTriggerDict[tabAfter];
            this._animator.ResetTrigger(this._animatorTriggerDict[tabBefore]);
            this._animator.ResetTrigger(id);
            this._animator.SetTrigger(id);
        }

        private bool Filter(StorageDataItemBase item)
        {
            bool flag = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.avatarID) != null;
            bool flag2 = CabinDisjointEquipmentMetaDataReader.TryGetCabinDisjointEquipmentMetaDataByKey(item.ID) != null;
            return ((!flag && flag2) && !item.isProtected);
        }

        private List<StorageDataItemBase> GetFilterList()
        {
            List<StorageDataItemBase> list = Singleton<StorageModule>.Instance.GetAllUserWeapons().FindAll(x => this.Filter(x));
            list.Sort(new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToRarityDesc));
            return list;
        }

        private void HackDropItems()
        {
            CabinCollectDataItem item = this._cabinData as CabinCollectDataItem;
            item.dropItems = new List<DropItem>();
            DropItem item2 = new DropItem();
            item2.set_item_id(0xfa1);
            item.dropItems.Add(item2);
            item2 = new DropItem();
            item2.set_item_id(0x3ef);
            item.dropItems.Add(item2);
        }

        private bool OnAddCabinTechRsp(AddCabinTechRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.RefreshTreeUI();
            }
            return false;
        }

        private void OnChange(Transform trans, int index)
        {
            StorageDataItemBase item = this._showItemList[index];
            MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
            component.showProtected = true;
            bool isSelected = item == this._selectedItem;
            bool bUsed = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.avatarID) != null;
            component.SetupView(item, MonoItemIconButton.SelectMode.SmallWhenUnSelect, isSelected, false, bUsed);
            component.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnItemClick));
        }

        private void OnCollectTabBtnClick()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = "CollectTab";
            this.defaultTab = searchKey;
            this._tabManager.ShowTab(searchKey);
            this.ClearOnChangedTab(showingTabKey, searchKey);
        }

        private void OnEnhanceTabBtnClick()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = "EnhanceTab";
            this.defaultTab = searchKey;
            this._tabManager.ShowTab(searchKey);
            this.ClearOnChangedTab(showingTabKey, searchKey);
        }

        private void OnFetchScoinBtnClick()
        {
            CabinCollectDataItem item = this._cabinData as CabinCollectDataItem;
            if (item.CanFetchScoin())
            {
                Singleton<NetworkManager>.Instance.RequestIslandCollect();
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = "No scoin can fetch"
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
        }

        private bool OnGetCollectCabinRsp(GetCollectCabinRsp rsp)
        {
            if ((this._cabinData is CabinCollectDataItem) && (rsp.get_retcode() == null))
            {
                this.SetupCollectTab();
            }
            return false;
        }

        private bool OnGetIslandDisjoinEquipmentRsp(IslandDisjoinEquipmentRsp rsp)
        {
            this.SwitchMiscSubTab(MiscSubTab.First);
            Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Decompose_Item", null, null, null);
            this._iconEffect.gameObject.SetActive(true);
            this._iconEffect.GetComponent<MonoItemIconButton>().SetupView(this._selectedItem, MonoItemIconButton.SelectMode.None, false, false, false);
            Singleton<ApplicationManager>.Instance.StartCoroutine(this.ShowDisjoinRsp(rsp));
            return false;
        }

        private bool OnGetIslandVentureRewardRsp(GetIslandVentureRewardRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new RewardGotDialogContext(rsp.get_reward_list()[0], this._playerLevelBefore, rsp.get_drop_item_list(), "Menu_Title_GotVentureReward", string.Empty), UIType.Any);
            }
            return false;
        }

        private bool OnGetIslandVentureRsp(GetIslandVentureRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.RefreshVentureTabContent();
            }
            return false;
        }

        private bool OnGetMainDataRsp(GetMainDataRsp rsp)
        {
            if ((rsp.get_scoinSpecified() && (this._tabManager.GetShowingTabKey() == "MiscTab")) && (this._currentMiscSubTab == MiscSubTab.Preview))
            {
                this.RefreshSubTab_Preview();
            }
            return false;
        }

        private bool OnGetRefreshIslandVentureInfoRsp(GetRefreshIslandVentureInfoRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                GeneralDialogContext context;
                int num = (int) rsp.get_refresh_price();
                if (num > Singleton<PlayerModule>.Instance.playerData.hcoin)
                {
                    context = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.SingleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                        desc = LocalizationGeneralLogic.GetText("10029", new object[0])
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else
                {
                    context = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.DoubleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
                    };
                    object[] replaceParams = new object[] { num };
                    context.desc = LocalizationGeneralLogic.GetText("Menu_Desc_RefreshVentureListHint", replaceParams);
                    context.buttonCallBack = new Action<bool>(this.OnRefreshVentureListConfirmed);
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
            }
            return false;
        }

        private bool OnIslandCollectRsp(IslandCollectRsp rsp)
        {
            <OnIslandCollectRsp>c__AnonStoreyFA yfa = new <OnIslandCollectRsp>c__AnonStoreyFA {
                rsp = rsp,
                <>f__this = this
            };
            if (yfa.rsp.get_retcode() == null)
            {
                <OnIslandCollectRsp>c__AnonStoreyF9 yf = new <OnIslandCollectRsp>c__AnonStoreyF9 {
                    <>f__ref$250 = yfa,
                    <>f__this = this
                };
                Transform effectTrans = base.view.transform.Find("EffectContainer/IslandCollectionGoldCoin");
                yf.fetchScoin = (int) yfa.rsp.get_add_scoin();
                CabinCollectDataItem item = this._cabinData as CabinCollectDataItem;
                yf.burstRate = (!yfa.rsp.get_is_extraSpecified() || !yfa.rsp.get_is_extra()) ? 1f : item.crtExtraRatio;
                ParticleSystem.EmissionModule emission = effectTrans.GetComponent<ParticleSystem>().emission;
                ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve {
                    constantMax = Mathf.Clamp((float) (((float) yf.fetchScoin) / item.topLimit), (float) 0.1f, (float) 1f) * this.FETCH_SCOIN_MISSION_RATIO_TOTAL
                };
                emission.rate = curve;
                Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Collect_Gold", null, null, null);
                this.PlayEffect(effectTrans);
                Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.2f, 0f).timeUpCallback = new Action(yf.<>m__17A);
            }
            return false;
        }

        private void OnItemClick(StorageDataItemBase item, bool selected)
        {
            if (!selected)
            {
                this._selectedItem = item;
                this.UpdateSelectInfo();
                this._subTabDict[MiscSubTab.Select].Find("SelectPanel/Info/Content/ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
            }
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            IEnumerator enumerator = base.view.transform.Find("EffectContainer").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    foreach (ParticleSystem system in current.GetComponentsInChildren<ParticleSystem>())
                    {
                        system.Stop();
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
            this._animator.ResetTrigger(this._animatorTriggerDict[this.defaultTab]);
            this._animator.SetTrigger(this._animatorTriggerDict[this.defaultTab]);
        }

        private void OnMiscAddBtnClick()
        {
            this._showItemList = this.GetFilterList();
            if (this._showItemList.Count <= 0)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_CabinNoDisjointWeapon", new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                this.SwitchMiscSubTab(MiscSubTab.Select);
            }
        }

        private void OnMiscDisjointBtnClick()
        {
            this.SwitchMiscSubTab(MiscSubTab.Preview);
        }

        private void OnMiscDisjointFinalBtnClick()
        {
            int scoin = Singleton<PlayerModule>.Instance.playerData.scoin;
            int needSCoin = CabinDisjointEquipmentMetaDataReader.GetCabinDisjointEquipmentMetaDataByKey(this._selectedItem.ID).NeedSCoin;
            if (scoin < needSCoin)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new SCoinExchangeDialogContext(), UIType.Any);
            }
            else
            {
                Singleton<NetworkManager>.Instance.RequestIslandDisjoinEquipment(3, (uint) this._selectedItem.uid);
            }
        }

        private void OnMiscOverviewTabBtnClick()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = "MiscOverviewTab";
            this.defaultTab = searchKey;
            this._tabManager.ShowTab(searchKey);
            this.ClearOnChangedTab(showingTabKey, searchKey);
        }

        private void OnMiscPreviewCancelBtnClick()
        {
            this.SwitchMiscSubTab(MiscSubTab.Select);
        }

        private void OnMiscSelectCancelBtnClick()
        {
            this.SwitchMiscSubTab(MiscSubTab.First);
        }

        private void OnMiscTabBtnClick()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = "MiscTab";
            this.defaultTab = searchKey;
            this._tabManager.ShowTab(searchKey);
            this.ClearOnChangedTab(showingTabKey, searchKey);
            this.SwitchMiscSubTab(MiscSubTab.First);
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.SetSellViewActive)
            {
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x9d:
                    this.SetupView();
                    break;

                case 0xa7:
                    this.OnResetCabinTechRsp(pkt.getData<ResetCabinTechRsp>());
                    break;

                case 0xa5:
                    this.OnAddCabinTechRsp(pkt.getData<AddCabinTechRsp>());
                    break;

                case 0xa9:
                    this.OnGetIslandVentureRsp(pkt.getData<GetIslandVentureRsp>());
                    break;

                case 11:
                    this.OnGetMainDataRsp(pkt.getData<GetMainDataRsp>());
                    break;

                case 0xb0:
                    this.OnGetIslandVentureRewardRsp(pkt.getData<GetIslandVentureRewardRsp>());
                    break;

                case 170:
                    this.RefreshVentureTabContent();
                    break;

                case 180:
                    this.OnGetIslandDisjoinEquipmentRsp(pkt.getData<IslandDisjoinEquipmentRsp>());
                    break;

                case 0xac:
                    this.OnRefreshIslandVentureRsp(pkt.getData<RefreshIslandVentureRsp>());
                    break;

                case 0xb8:
                    this.OnGetCollectCabinRsp(pkt.getData<GetCollectCabinRsp>());
                    break;

                case 0xb6:
                    this.OnIslandCollectRsp(pkt.getData<IslandCollectRsp>());
                    break;

                case 210:
                    this.OnSpeedUpIslandVentureRsp(pkt.getData<SpeedUpIslandVentureRsp>());
                    break;

                case 0xde:
                    this.OnGetRefreshIslandVentureInfoRsp(pkt.getData<GetRefreshIslandVentureInfoRsp>());
                    break;
            }
            return false;
        }

        private void OnPowerTabBtnClick()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = "PowerTab";
            this.defaultTab = searchKey;
            this._tabManager.ShowTab(searchKey);
            this.ClearOnChangedTab(showingTabKey, searchKey);
        }

        private bool OnRefreshIslandVentureRsp(RefreshIslandVentureRsp rsp)
        {
            if (rsp.get_retcode() != null)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0]),
                    desc = rsp.get_retcode().ToString()
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        private void OnRefreshVentureBtnClick()
        {
            int times = Singleton<IslandModule>.Instance.VentureRefreshTimes + 1;
            if (!(this._cabinData as CabinVentureDataItem).GetRefreshCost(times))
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_ReachRefreshVentureMaxTimes", new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                Singleton<NetworkManager>.Instance.RequestGetRefreshIslandVentureInfo();
                Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(0xde, null), UIType.Any);
            }
        }

        private void OnRefreshVentureListConfirmed(bool isConfirmed)
        {
            if (isConfirmed)
            {
                Singleton<NetworkManager>.Instance.RequestRefreshIslandVenture();
            }
        }

        private bool OnResetCabinTechRsp(ResetCabinTechRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.RefreshTreeUI();
            }
            return false;
        }

        private void OnResetPowerBtnClick()
        {
            if ((this._cabinData.cabinType == 5) && (Singleton<IslandModule>.Instance.GetVentureInProgressNum() > 0))
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_CanNotResetTechTreeWhenVentureInProgress", new object[0]),
                    type = GeneralDialogContext.ButtonType.SingleButton
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new ResetTechTreeDialogContext(this._cabinData), UIType.Any);
            }
        }

        protected override void OnSetActive(bool enabled)
        {
            base.OnSetActive(enabled);
            if (!enabled)
            {
                foreach (string str in TAB_KEY)
                {
                    base.view.transform.Find(str).gameObject.SetActive(false);
                }
            }
        }

        private bool OnSpeedUpIslandVentureRsp(SpeedUpIslandVentureRsp rsp)
        {
            if ((rsp.get_retcode() == null) && (this._cabinData.cabinType == 5))
            {
                this.SetupVentureTabContent();
            }
            return false;
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            if (!this._bCacheSpawn)
            {
                btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
                btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
                btn.transform.Find("Image").GetComponent<Image>().color = !active ? Color.white : MiscData.GetColor("Black");
                btn.interactable = !active;
                go.SetActive(active);
            }
        }

        private void OnTreeTabBtnClick()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = "TreeTab";
            this.defaultTab = searchKey;
            this._tabManager.ShowTab(searchKey);
            this.ClearOnChangedTab(showingTabKey, searchKey);
            base.view.transform.Find("TreeTab/ScrollView").GetComponent<MonoTechTreeUI>().SetOriginPosition();
        }

        private void OnVentureCancelBtnClick(VentureDataItem ventureData)
        {
            <OnVentureCancelBtnClick>c__AnonStoreyFB yfb = new <OnVentureCancelBtnClick>c__AnonStoreyFB {
                ventureData = ventureData
            };
            GeneralDialogContext dialogContext = new GeneralDialogContext {
                title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0])
            };
            object[] replaceParams = new object[] { yfb.ventureData.GetStaminaReturnOnCancel() };
            dialogContext.desc = LocalizationGeneralLogic.GetText("Menu_Desc_CancelVentureHint", replaceParams);
            dialogContext.buttonCallBack = new Action<bool>(yfb.<>m__17C);
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        private void OnVentureFetchBtnClick(VentureDataItem ventureData)
        {
            this._playerLevelBefore = Singleton<PlayerModule>.Instance.playerData.teamLevel;
            Singleton<NetworkManager>.Instance.RequestGetIslandVentureReward(ventureData.VentureID);
        }

        private void OnVentureGoBtnClick(VentureDataItem ventureData)
        {
            Singleton<MainUIManager>.Instance.ShowPage(new VentureDispatchPageContext(ventureData), UIType.Page);
        }

        private void OnVentureScrollChange(Transform ventureTrans, int index)
        {
            VentureDataItem ventureData = this._ventureList[index];
            ventureTrans.GetComponent<MonoVentureInfoRow>().SetupView(ventureData, new Action<VentureDataItem>(this.OnVentureFetchBtnClick), new Action<VentureDataItem>(this.OnVentureGoBtnClick), new Action<VentureDataItem>(this.OnVentureCancelBtnClick), new Action<VentureDataItem>(this.OnVentureSpeedUpBtnClick));
        }

        private void OnVentureSpeedUpBtnClick(VentureDataItem ventureData)
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new VentureSpeedUpDialogContext(ventureData), UIType.Any);
        }

        private void OnVentureTabBtnClick()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = "VentureTab";
            this.defaultTab = searchKey;
            this._tabManager.ShowTab(searchKey);
            this.ClearOnChangedTab(showingTabKey, searchKey);
        }

        private void PlayEffect(Transform effectTrans)
        {
            foreach (ParticleSystem system in effectTrans.GetComponentsInChildren<ParticleSystem>())
            {
                system.Play();
            }
        }

        private void RefreshPowerInfo()
        {
            int maxPowerCost = Singleton<IslandModule>.Instance.GetMaxPowerCost();
            int usedPowerCost = Singleton<IslandModule>.Instance.GetUsedPowerCost();
            int usedPower = this._cabinData.GetUsedPower();
            float num4 = ((float) usedPowerCost) / ((float) maxPowerCost);
            float num5 = ((float) usedPower) / ((float) maxPowerCost);
            base.view.transform.Find("TreeTab/PowerInfo/PowerCircle/AllUsed").GetComponent<Image>().fillAmount = num4;
            base.view.transform.Find("TreeTab/PowerInfo/PowerCircle/ThisUsed").GetComponent<Image>().fillAmount = num5;
            base.view.transform.Find("TreeTab/PowerInfo/Line1/Used").GetComponent<Text>().text = usedPowerCost.ToString();
            base.view.transform.Find("TreeTab/PowerInfo/Line1/Max").GetComponent<Text>().text = maxPowerCost.ToString();
            base.view.transform.Find("TreeTab/PowerInfo/Line2/Used").GetComponent<Text>().text = usedPower.ToString();
        }

        private void RefreshSubTab_First()
        {
            bool flag = this._cabinData._techTree.AbilityUnLock(6);
            string str = !flag ? LocalizationGeneralLogic.GetText("Menu_Desc_CabinMiscDisjointDisable", new object[0]) : LocalizationGeneralLogic.GetText("Menu_Desc_CabinMiscDisjointEnable", new object[0]);
            base.view.transform.Find("MiscTab/FirstSubTab/Title").GetComponent<Text>().text = str;
            base.view.transform.Find("MiscTab/FirstSubTab/AddBtn").gameObject.SetActive(flag);
        }

        private void RefreshSubTab_Preview()
        {
            this._subTabDict[MiscSubTab.Preview].Find("Input").GetComponent<MonoItemIconButton>().SetupView(this._selectedItem, MonoItemIconButton.SelectMode.None, false, false, false);
            this._subTabDict[MiscSubTab.Preview].Find("ScrollView/Content").GetComponent<MonoMiscDisjointOutputUI>().SetupView(this._selectedItem);
            int scoin = Singleton<PlayerModule>.Instance.playerData.scoin;
            int needSCoin = CabinDisjointEquipmentMetaDataReader.GetCabinDisjointEquipmentMetaDataByKey(this._selectedItem.ID).NeedSCoin;
            Text component = this._subTabDict[MiscSubTab.Preview].Find("DisjointBtn/Cost/Content/Text").GetComponent<Text>();
            component.text = needSCoin.ToString();
            Color color = (scoin >= needSCoin) ? Color.white : Color.red;
            component.color = color;
        }

        private void RefreshSubTab_Select()
        {
            if (this._showItemList.Count > 0)
            {
                this._selectedItem = this._showItemList[0];
                this._subTabDict[MiscSubTab.Select].Find("SelectPanel/Info/Content/ScrollView").GetComponent<MonoGridScroller>().Init(new MoleMole.MonoGridScroller.OnChange(this.OnChange), this._showItemList.Count, null);
                this.UpdateSelectInfo();
            }
        }

        private void RefreshTreeUI()
        {
            base.view.transform.Find("TreeTab/ScrollView").GetComponent<MonoTechTreeUI>().RefreshUI();
            this.RefreshPowerInfo();
        }

        private void RefreshVentureTabContent()
        {
            this.SetupVentureTabContent();
            bool flag = Singleton<IslandModule>.Instance.GetVentureDoneNum() > 0;
            base.view.transform.Find("TabBtns/TabBtnVenture/PopUp").gameObject.SetActive(flag);
        }

        private void SetAllTabVisible(bool isVisible)
        {
            foreach (string str in TAB_KEY)
            {
                base.view.transform.Find(str).gameObject.SetActive(isVisible);
            }
            IEnumerator enumerator = base.view.transform.Find("TabBtns").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    current.gameObject.SetActive(isVisible);
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

        private void SetupCollectTab()
        {
            GameObject gameObject = base.view.transform.Find("CollectTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/TabBtnCollect").GetComponent<Button>();
            component.gameObject.SetActive(true);
            string key = "CollectTab";
            this._tabManager.SetTab(key, component, gameObject);
            this.SetupCollectTabContent();
        }

        private void SetupCollectTabContent()
        {
            CabinCollectDataItem item = this._cabinData as CabinCollectDataItem;
            Transform transform = base.view.transform.Find("CollectTab/InfoPanel");
            transform.Find("Speed/Num").GetComponent<Text>().text = Mathf.FloorToInt(item.speed).ToString();
            transform.Find("TopLimit/Num").GetComponent<Text>().text = item.topLimit.ToString();
            transform.Find("CrtRatio/Num").GetComponent<Text>().text = string.Format("{0:0%}", item.crtRatio);
            transform.Find("ExtraRatio/Num").GetComponent<Text>().text = string.Format("{0:0%}", item.crtExtraRatio);
            base.view.transform.Find("CollectTab/Scoin/Fill").GetComponent<Image>().fillAmount = Mathf.Clamp01(((float) item.currentScoinAmount) / item.topLimit);
            base.view.transform.Find("CollectTab/Scoin/Num").GetComponent<Text>().text = item.currentScoinAmount.ToString();
            if (this._collectCabinTimer != null)
            {
                this._collectCabinTimer.Destroy();
            }
            if (item.canUpdateScoinLate)
            {
                TimeSpan span = (TimeSpan) (item.nextScoinUpdateTime - TimeUtil.Now);
                this._collectCabinTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer((float) span.TotalSeconds, 0f);
                if (<>f__am$cache1A == null)
                {
                    <>f__am$cache1A = () => Singleton<NetworkManager>.Instance.RequestGetCollectCabin();
                }
                this._collectCabinTimer.timeUpCallback = <>f__am$cache1A;
            }
            base.view.transform.Find("CollectTab/FetchBtn/Btn").GetComponent<Button>().interactable = item.CanFetchScoin();
            int num = 5;
            Transform transform2 = base.view.transform.Find("CollectTab/InfoPanel");
            Transform transform3 = base.view.transform.Find("CollectTab/InfoPanel/Accessory");
            int dropMaterialPackageNum = Singleton<IslandModule>.Instance.GetDropMaterialPackageNum();
            if (dropMaterialPackageNum > num)
            {
                dropMaterialPackageNum = num;
            }
            if ((dropMaterialPackageNum > 0) || (item.dropItems.Count > 0))
            {
                transform3.gameObject.SetActive(true);
                this._infoPos.x = transform2.GetComponent<RectTransform>().anchoredPosition.x;
                this._infoPos.y = -40f;
                transform2.GetComponent<RectTransform>().anchoredPosition = this._infoPos;
            }
            else
            {
                transform3.gameObject.SetActive(false);
                this._infoPos.x = transform2.GetComponent<RectTransform>().anchoredPosition.x;
                this._infoPos.y = -112f;
                transform2.GetComponent<RectTransform>().anchoredPosition = this._infoPos;
            }
            for (int i = 0; i < num; i++)
            {
                int num8 = i + 1;
                Transform transform4 = transform3.Find(string.Format("MaterialList/{0}", num8.ToString()));
                if (i < item.dropItems.Count)
                {
                    <SetupCollectTabContent>c__AnonStoreyF8 yf = new <SetupCollectTabContent>c__AnonStoreyF8 {
                        <>f__this = this
                    };
                    transform4.gameObject.SetActive(true);
                    int metaId = (int) item.dropItems[i].get_item_id();
                    int level = (int) item.dropItems[i].get_level();
                    yf.itemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(metaId, level);
                    if (yf.itemData != null)
                    {
                        transform4.Find("Empty").gameObject.SetActive(false);
                        transform4.Find("BG").gameObject.SetActive(true);
                        transform4.Find("BG/Unselected/FrameBottom").gameObject.SetActive(true);
                        transform4.Find("Text").gameObject.SetActive(true);
                        transform4.Find("Text").GetComponent<Text>().text = string.Format("x{0}", (int) item.dropItems[i].get_num());
                        transform4.Find("ItemIcon").gameObject.SetActive(true);
                        transform4.Find("Star").gameObject.SetActive(true);
                        transform4.Find("ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(yf.itemData.GetIconPath());
                        transform4.Find("Star").GetComponent<MonoItemIconStar>().SetupView(yf.itemData.rarity, yf.itemData.rarity);
                        if (yf.itemData is EndlessToolDataItem)
                        {
                            transform4.Find("ShowDetailBtn").GetComponent<Button>().onClick.RemoveAllListeners();
                        }
                        else
                        {
                            base.BindViewCallback(transform4.Find("ShowDetailBtn").GetComponent<Button>(), new UnityAction(yf.<>m__179));
                        }
                    }
                }
                else if (i < dropMaterialPackageNum)
                {
                    transform4.gameObject.SetActive(true);
                    transform4.Find("Empty").gameObject.SetActive(true);
                    transform4.Find("BG").gameObject.SetActive(false);
                    transform4.Find("BG/Unselected/FrameBottom").gameObject.SetActive(false);
                    transform4.Find("Text").gameObject.SetActive(false);
                    transform4.Find("ItemIcon").gameObject.SetActive(false);
                    transform4.Find("Star").gameObject.SetActive(false);
                    transform4.Find("ShowDetailBtn").GetComponent<Button>().onClick.RemoveAllListeners();
                }
                else
                {
                    transform4.gameObject.SetActive(false);
                    transform4.Find("ShowDetailBtn").GetComponent<Button>().onClick.RemoveAllListeners();
                }
            }
        }

        private void SetupEnhanceTab()
        {
            GameObject gameObject = base.view.transform.Find("EnhanceTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/TabBtnEnhance").GetComponent<Button>();
            component.gameObject.SetActive(true);
            string key = "EnhanceTab";
            this._tabManager.SetTab(key, component, gameObject);
            this.SetupEnhanceTabContent();
        }

        private void SetupEnhanceTabContent()
        {
            Transform trans = base.view.transform.Find("EnhanceTab/EnhanceList/Content");
            trans.DestroyChildren();
            Transform transform = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI/Menus/Widget/Island/AvatarEnhanceInfo")).transform;
            transform.SetParent(trans, false);
            CabinAvatarEnhanceDataItem item = this._cabinData as CabinAvatarEnhanceDataItem;
            int avatarClassID = item._classType;
            transform.GetComponent<MonoAvatarEnhance>().SetupView(avatarClassID);
        }

        private void SetupMiscOverviewTab()
        {
            GameObject gameObject = base.view.transform.Find("MiscOverviewTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/TabBtnMiscOverview").GetComponent<Button>();
            component.gameObject.SetActive(true);
            string key = "MiscOverviewTab";
            this._tabManager.SetTab(key, component, gameObject);
            this.SetupMiscOverviewTabContent();
        }

        private void SetupMiscOverviewTabContent()
        {
            int count = Singleton<FriendModule>.Instance.friendsList.Count;
            int maxFriendAdd = Singleton<IslandModule>.Instance.GetMaxFriendAdd();
            int num3 = Singleton<PlayerModule>.Instance.playerData.maxFriend + maxFriendAdd;
            base.view.transform.Find("MiscOverviewTab/Content/1/Friend/Fill").GetComponent<Image>().fillAmount = ((float) count) / ((float) num3);
            base.view.transform.Find("MiscOverviewTab/Content/1/Friend/Num").GetComponent<Text>().text = count.ToString();
            base.view.transform.Find("MiscOverviewTab/Content/1/Plus").GetComponent<Text>().text = string.Format("+{0}", maxFriendAdd);
            int skillPoint = Singleton<PlayerModule>.Instance.playerData.skillPoint;
            int skillPointLimit = Singleton<PlayerModule>.Instance.playerData.skillPointLimit;
            int skillPointAdd = Singleton<IslandModule>.Instance.GetSkillPointAdd();
            base.view.transform.Find("MiscOverviewTab/Content/2/Skill/Fill").GetComponent<Image>().fillAmount = ((float) skillPoint) / ((float) skillPointLimit);
            base.view.transform.Find("MiscOverviewTab/Content/2/Skill/Num").GetComponent<Text>().text = skillPoint.ToString();
            base.view.transform.Find("MiscOverviewTab/Content/2/Plus").GetComponent<Text>().text = string.Format("+{0}", skillPointAdd);
        }

        private void SetupMiscTab()
        {
            GameObject gameObject = base.view.transform.Find("MiscTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/TabBtnMisc").GetComponent<Button>();
            component.gameObject.SetActive(true);
            string key = "MiscTab";
            this._tabManager.SetTab(key, component, gameObject);
            this._iconEffect = base.view.transform.Find("MiscTab/IconEffect");
            this.SetupMiscTabContent();
        }

        private void SetupMiscTabContent()
        {
            this._subTabDict[MiscSubTab.First] = base.view.transform.Find("MiscTab/FirstSubTab");
            this._subTabDict[MiscSubTab.Select] = base.view.transform.Find("MiscTab/SelectSubTab");
            this._subTabDict[MiscSubTab.Preview] = base.view.transform.Find("MiscTab/PreviewSubTab");
            this.SwitchMiscSubTab(MiscSubTab.First);
        }

        private void SetupPowerTab()
        {
            GameObject gameObject = base.view.transform.Find("PowerTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/TabBtnPower").GetComponent<Button>();
            component.gameObject.SetActive(true);
            string key = "PowerTab";
            this._tabManager.SetTab(key, component, gameObject);
            this.SetupPowerTabContent();
        }

        private void SetupPowerTabContent()
        {
            Transform transform = base.view.transform.Find("PowerTab/PowerInfo");
            int usedPowerCost = Singleton<IslandModule>.Instance.GetUsedPowerCost();
            int maxPowerCost = Singleton<IslandModule>.Instance.GetMaxPowerCost();
            transform.Find("CurrentPower/PowerCost/Current").GetComponent<Text>().text = usedPowerCost.ToString();
            transform.Find("CurrentPower/PowerCost/Max").GetComponent<Text>().text = maxPowerCost.ToString();
            base.view.transform.Find("PowerTab/Power/Fill").GetComponent<Image>().fillAmount = ((float) usedPowerCost) / ((float) maxPowerCost);
            transform.Find("MaxInfo/Current/Num").GetComponent<Text>().text = Singleton<IslandModule>.Instance.GetMaxPowerCost().ToString();
            int nextLevelMaxPowerCost = Singleton<IslandModule>.Instance.GetNextLevelMaxPowerCost();
            bool flag = nextLevelMaxPowerCost > 0;
            transform.Find("MaxInfo/Next").gameObject.SetActive(flag);
            if (flag)
            {
                transform.Find("MaxInfo/Next/Num").GetComponent<Text>().text = nextLevelMaxPowerCost.ToString();
            }
        }

        private void SetupTreeTab()
        {
            GameObject gameObject = base.view.transform.Find("TreeTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/TabBtnTree").GetComponent<Button>();
            component.gameObject.SetActive(true);
            string key = "TreeTab";
            this._tabManager.SetTab(key, component, gameObject);
            MonoTechTreeUI eui = gameObject.transform.Find("ScrollView").GetComponent<MonoTechTreeUI>();
            if (string.IsNullOrEmpty(this.defaultTab))
            {
                eui.ClearNodes();
            }
            if (!eui.HasChildren())
            {
                eui.InitNodes(this._cabinData._techTree);
            }
            this.RefreshPowerInfo();
            eui.SetOriginPosition();
        }

        private void SetupUIBG()
        {
            Image component = base.view.transform.Find("BG/BG").GetComponent<Image>();
            Image image2 = base.view.transform.Find("BG/Black").GetComponent<Image>();
            string prefabPath = string.Empty;
            string str2 = string.Empty;
            switch (this._cabinData.cabinType)
            {
                case 1:
                    prefabPath = "SpriteOutput/CabinBG/CabinPowerBG";
                    str2 = "SpriteOutput/CabinBG/CabinPowerBlack";
                    break;

                case 2:
                    prefabPath = "SpriteOutput/CabinBG/CabinEnhanceBG";
                    str2 = "SpriteOutput/CabinBG/CabinEnhanceBlack";
                    break;

                case 3:
                    prefabPath = "SpriteOutput/CabinBG/CabinCollectBG";
                    str2 = "SpriteOutput/CabinBG/CabinCollectBlack";
                    break;

                case 4:
                    prefabPath = "SpriteOutput/CabinBG/CabinMiscBG";
                    str2 = "SpriteOutput/CabinBG/CabinMiscBlack";
                    break;

                case 5:
                    prefabPath = "SpriteOutput/CabinBG/CabinVentureBG";
                    str2 = "SpriteOutput/CabinBG/CabinVentureBlack";
                    break;

                case 6:
                    prefabPath = "SpriteOutput/CabinBG/CabinEnhanceBG";
                    str2 = "SpriteOutput/CabinBG/CabinEnhanceBlack";
                    break;

                case 7:
                    prefabPath = "SpriteOutput/CabinBG/CabinEnhanceBG";
                    str2 = "SpriteOutput/CabinBG/CabinEnhanceBlack";
                    break;
            }
            component.sprite = Miscs.GetSpriteByPrefab(prefabPath);
            image2.sprite = Miscs.GetSpriteByPrefab(str2);
        }

        private void SetupVentureTab()
        {
            GameObject gameObject = base.view.transform.Find("VentureTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/TabBtnVenture").GetComponent<Button>();
            component.gameObject.SetActive(true);
            bool flag = Singleton<IslandModule>.Instance.GetVentureDoneNum() > 0;
            base.view.transform.Find("TabBtns/TabBtnVenture/PopUp").gameObject.SetActive(flag);
            string key = "VentureTab";
            this._tabManager.SetTab(key, component, gameObject);
            this.SetupVentureTabContent();
            Singleton<IslandModule>.Instance.UnRegisterVentureInProgress();
        }

        private void SetupVentureTabContent()
        {
            this._ventureList = Singleton<IslandModule>.Instance.GetVentureList();
            base.view.transform.Find("VentureTab/ScrollView").GetComponent<MonoGridScroller>().Init(new MoleMole.MonoGridScroller.OnChange(this.OnVentureScrollChange), this._ventureList.Count, null);
            base.view.transform.Find("VentureTab/InfoPanel/InProgress/Num").GetComponent<Text>().text = string.Format("{0}/{1}", Singleton<IslandModule>.Instance.GetVentureInProgressNum(), (Singleton<IslandModule>.Instance.GetCabinDataByType(5) as CabinVentureDataItem).GetMaxVentureNumInProgress());
            base.view.transform.Find("VentureTab/InfoPanel/AutoRefresh/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(TimeUtil.DailyUpdateTime, null, null, false);
        }

        protected override bool SetupView()
        {
            this._animator = base.view.transform.GetComponent<Animator>();
            this._animatorTriggerDict = new Dictionary<string, int>();
            this._animatorTriggerDict["CollectTab"] = this._animatorCollectTrigger;
            this._animatorTriggerDict["PowerTab"] = this._animatorPowerTrigger;
            this._animatorTriggerDict["MiscTab"] = this._animatorMiscTrigger;
            this._animatorTriggerDict["MiscOverviewTab"] = this._animatorMiscOverviewTrigger;
            this._animatorTriggerDict["VentureTab"] = this._animatorVentureTrigger;
            this._animatorTriggerDict["EnhanceTab"] = this._animatorEnhanceTrigger;
            this._animatorTriggerDict["TreeTab"] = this._animatorTreeTrigger;
            this.SetupUIBG();
            IEnumerator enumerator = base.view.transform.Find("EffectContainer").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    foreach (ParticleSystem system in current.GetComponentsInChildren<ParticleSystem>())
                    {
                        system.Stop();
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
            Transform transform2 = base.view.transform.Find("InfoPanel/Info/");
            transform2.Find("CabinName").GetComponent<Text>().text = this._cabinData.GetCabinName();
            transform2.Find("Lv/Lv").GetComponent<Text>().text = "Lv." + this._cabinData.level;
            transform2.Find("ExtendGrade").GetComponent<MonoCabinExtendGrade>().SetupView(this._cabinData.extendGrade);
            this.SetAllTabVisible(false);
            if (this._cabinData.cabinType != 1)
            {
                this.SetupTreeTab();
            }
            switch (this._cabinData.cabinType)
            {
                case 1:
                    this.SetupPowerTab();
                    if (string.IsNullOrEmpty(this.defaultTab))
                    {
                        this.defaultTab = "PowerTab";
                    }
                    break;

                case 2:
                    this.SetupEnhanceTab();
                    if (string.IsNullOrEmpty(this.defaultTab))
                    {
                        this.defaultTab = "EnhanceTab";
                    }
                    break;

                case 3:
                    this.SetupCollectTab();
                    if (string.IsNullOrEmpty(this.defaultTab))
                    {
                        this.defaultTab = "CollectTab";
                    }
                    break;

                case 4:
                    this.SetupMiscTab();
                    this.SetupMiscOverviewTab();
                    if (string.IsNullOrEmpty(this.defaultTab))
                    {
                        this.defaultTab = "MiscOverviewTab";
                    }
                    break;

                case 5:
                    this.SetupVentureTab();
                    if (string.IsNullOrEmpty(this.defaultTab))
                    {
                        this.defaultTab = "VentureTab";
                    }
                    break;

                case 6:
                    this.SetupEnhanceTab();
                    if (string.IsNullOrEmpty(this.defaultTab))
                    {
                        this.defaultTab = "EnhanceTab";
                    }
                    break;

                case 7:
                    this.SetupEnhanceTab();
                    if (string.IsNullOrEmpty(this.defaultTab))
                    {
                        this.defaultTab = "EnhanceTab";
                    }
                    break;
            }
            if (string.IsNullOrEmpty(this.defaultTab))
            {
                this.defaultTab = "TreeTab";
            }
            this._tabManager.ShowTab(this.defaultTab);
            this._animator.ResetTrigger(this._animatorTriggerDict[this.defaultTab]);
            this._animator.SetTrigger(this._animatorTriggerDict[this.defaultTab]);
            if (!this._bCacheSpawn)
            {
                this._triggerCameraTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.5f, 0f);
                this._triggerCameraTimer.timeUpCallback = delegate {
                    this.TriggerSceneCamera(false);
                };
            }
            return false;
        }

        private void ShowDetailDialog(StorageDataItemBase item)
        {
            UIUtil.ShowItemDetail(item, true, true);
        }

        [DebuggerHidden]
        private IEnumerator ShowDisjoinRsp(IslandDisjoinEquipmentRsp rsp)
        {
            return new <ShowDisjoinRsp>c__Iterator66 { rsp = rsp, <$>rsp = rsp, <>f__this = this };
        }

        private void ShowGetScoinHintDialog(int scoinNum, float burstRate, List<DropItem> dropItems)
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new IslandCollectGotDialogContext(scoinNum, burstRate, dropItems), UIType.Any);
        }

        private void SwitchMiscSubTab(MiscSubTab tab)
        {
            this._currentMiscSubTab = tab;
            IEnumerator enumerator = Enum.GetValues(typeof(MiscSubTab)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    MiscSubTab current = (MiscSubTab) ((int) enumerator.Current);
                    this._subTabDict[current].gameObject.SetActive(current == tab);
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
            if (tab == MiscSubTab.First)
            {
                this.RefreshSubTab_First();
            }
            else if (tab == MiscSubTab.Select)
            {
                this.RefreshSubTab_Select();
            }
            else if (tab == MiscSubTab.Preview)
            {
                this.RefreshSubTab_Preview();
            }
        }

        private void TriggerSceneCamera(bool enable)
        {
            GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>().TriggerCameraObj(enable);
        }

        private void UpdateSelectInfo()
        {
            this._subTabDict[MiscSubTab.Select].Find("SelectPanel/Info/Content/SelectedEquip/Name").GetComponent<Text>().text = this._selectedItem.GetDisplayTitle();
            this._subTabDict[MiscSubTab.Select].Find("SelectPanel/Info/Content/SelectedEquip/Lv").GetComponent<Text>().text = "LV." + this._selectedItem.level;
        }

        [CompilerGenerated]
        private sealed class <OnIslandCollectRsp>c__AnonStoreyF9
        {
            internal CabinDetailPageContext.<OnIslandCollectRsp>c__AnonStoreyFA <>f__ref$250;
            internal CabinDetailPageContext <>f__this;
            internal float burstRate;
            internal int fetchScoin;

            internal void <>m__17A()
            {
                this.<>f__this.ShowGetScoinHintDialog(this.fetchScoin, this.burstRate, this.<>f__ref$250.rsp.get_drop_item_list());
            }
        }

        [CompilerGenerated]
        private sealed class <OnIslandCollectRsp>c__AnonStoreyFA
        {
            internal CabinDetailPageContext <>f__this;
            internal IslandCollectRsp rsp;
        }

        [CompilerGenerated]
        private sealed class <OnVentureCancelBtnClick>c__AnonStoreyFB
        {
            internal VentureDataItem ventureData;

            internal void <>m__17C(bool confirmed)
            {
                if (confirmed)
                {
                    Singleton<NetworkManager>.Instance.RequestCancelDispatchIslandVenture(this.ventureData.VentureID);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <SetupCollectTabContent>c__AnonStoreyF8
        {
            internal CabinDetailPageContext <>f__this;
            internal StorageDataItemBase itemData;

            internal void <>m__179()
            {
                this.<>f__this.ShowDetailDialog(this.itemData);
            }
        }

        [CompilerGenerated]
        private sealed class <ShowDisjoinRsp>c__Iterator66 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal IslandDisjoinEquipmentRsp <$>rsp;
            internal CabinDetailPageContext <>f__this;
            internal IslandDisjoinEquipmentRsp rsp;

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
                        this.$current = new WaitForSeconds(this.<>f__this._iconEffectDuration);
                        this.$PC = 1;
                        return true;

                    case 1:
                    {
                        this.<>f__this._iconEffect.gameObject.SetActive(false);
                        GeneralDialogContext dialogContext = new GeneralDialogContext {
                            type = GeneralDialogContext.ButtonType.SingleButton,
                            title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0]),
                            desc = (this.rsp.get_retcode() != null) ? this.rsp.get_retcode().ToString() : LocalizationGeneralLogic.GetText("Menu_Title_IslandDisjoinSucc", new object[0])
                        };
                        Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                        this.$PC = -1;
                        break;
                    }
                }
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

