namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class StorageShowPageContext : BasePageContext
    {
        private Dictionary<GameObject, MonoScrollerFadeManager> _fadeManagerDict;
        private Dictionary<GameObject, Dictionary<int, RectTransform>> _itemBeforeDict;
        private MonoStorageSelectForPowerUp _powerUpPanel;
        private Dictionary<GameObject, MonoGridScroller> _scrollerDict;
        private MonoStorageSelectForSellPanel _sellPanel;
        private Dictionary<string, List<StorageDataItemBase>> _tabItemList;
        private TabManager _tabManager;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, int> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, RectTransform> <>f__am$cache11;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, int> <>f__am$cache12;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, RectTransform> <>f__am$cache13;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, int> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, RectTransform> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, int> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, RectTransform> <>f__am$cacheF;
        public string defaultTab = string.Empty;
        public FeatureType featureType;
        public const string Fragment_TAB = "FragmentTab";
        public const string Item_TAB = "ItemTab";
        public StorageDataItemBase powerUpTarget;
        public List<StorageDataItemBase> selectedResources;
        public const string STIGMATA_TAB = "StigmataTab";
        public static readonly string[] TAB_KEY = new string[] { "WeaponTab", "StigmataTab", "ItemTab", "FragmentTab" };
        public const string WEAPON_TAB = "WeaponTab";

        public StorageShowPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "StorageShowPageContext",
                viewPrefabPath = "UI/Menus/Page/Storage/StorageShowPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            this._tabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
            this._tabItemList = new Dictionary<string, List<StorageDataItemBase>>();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), new UnityAction(this.OnWeaponTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), new UnityAction(this.OnStigmataTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), new UnityAction(this.OnItemTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_4").GetComponent<Button>(), new UnityAction(this.OnFragmentTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("SellBtn").GetComponent<Button>(), new UnityAction(this.OnSellBtnClick));
            base.BindViewCallback(base.view.transform.Find("SortBtn").GetComponent<Button>(), new UnityAction(this.OnSortBtnClick));
            base.BindViewCallback(base.view.transform.Find("SortPanel/BG").GetComponent<Button>(), new UnityAction(this.OnSortBGClick));
        }

        private void ClearOnChangedTab(string tabBefore, string tabAfter)
        {
            switch (this.featureType)
            {
                case FeatureType.Normal:
                    this.defaultTab = tabAfter;
                    Singleton<MiHoYoGameData>.Instance.LocalData.StorageShowTabName = tabAfter;
                    Singleton<MiHoYoGameData>.Instance.Save();
                    break;

                case FeatureType.SelectForSell:
                    this.featureType = FeatureType.Normal;
                    this.RefreshTabItemByKey(tabBefore);
                    this.SetupSellView();
                    break;

                case FeatureType.SelectForPowerUp:
                    if (this._powerUpPanel != null)
                    {
                        this._powerUpPanel.ClearModifyingItem();
                        bool isMulti = this.IsTabItemHeap(this._tabManager.GetShowingTabKey());
                        this._powerUpPanel.RefreshView(isMulti);
                        this.RefreshShowingTabItem();
                    }
                    break;
            }
        }

        private bool FilterForPowerUp(StorageDataItemBase item)
        {
            return (((!item.isProtected && (item.avatarID <= 0)) && ((item != this.powerUpTarget) && !Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(item.ID))) && (!(item is MaterialDataItem) || (((MaterialDataItem) item).GetGearExp() > 0f)));
        }

        private bool FilterForSell(StorageDataItemBase item)
        {
            return ((!item.isProtected && (item.avatarID <= 0)) && !Singleton<PlayerModule>.Instance.playerData.gachaTicketPriceDict.ContainsKey(item.ID));
        }

        private void InitScroller()
        {
            this._scrollerDict = new Dictionary<GameObject, MonoGridScroller>();
            this._scrollerDict[base.view.transform.Find("WeaponTab").gameObject] = base.view.transform.Find("WeaponTab/ScrollView").GetComponent<MonoGridScroller>();
            this._scrollerDict[base.view.transform.Find("StigmataTab").gameObject] = base.view.transform.Find("StigmataTab/ScrollView").GetComponent<MonoGridScroller>();
            this._scrollerDict[base.view.transform.Find("ItemTab").gameObject] = base.view.transform.Find("ItemTab/ScrollView").GetComponent<MonoGridScroller>();
            this._scrollerDict[base.view.transform.Find("FragmentTab").gameObject] = base.view.transform.Find("FragmentTab/ScrollView").GetComponent<MonoGridScroller>();
            this._fadeManagerDict = new Dictionary<GameObject, MonoScrollerFadeManager>();
            this._fadeManagerDict[base.view.transform.Find("WeaponTab").gameObject] = base.view.transform.Find("WeaponTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
            this._fadeManagerDict[base.view.transform.Find("StigmataTab").gameObject] = base.view.transform.Find("StigmataTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
            this._fadeManagerDict[base.view.transform.Find("ItemTab").gameObject] = base.view.transform.Find("ItemTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
            this._fadeManagerDict[base.view.transform.Find("FragmentTab").gameObject] = base.view.transform.Find("FragmentTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
            this._itemBeforeDict = new Dictionary<GameObject, Dictionary<int, RectTransform>>();
            this._itemBeforeDict[base.view.transform.Find("WeaponTab").gameObject] = null;
            this._itemBeforeDict[base.view.transform.Find("StigmataTab").gameObject] = null;
            this._itemBeforeDict[base.view.transform.Find("ItemTab").gameObject] = null;
            this._itemBeforeDict[base.view.transform.Find("FragmentTab").gameObject] = null;
        }

        private bool IsStorageItemDataEqual(RectTransform dataNew, RectTransform dataOld)
        {
            if ((dataNew == null) || (dataOld == null))
            {
                return false;
            }
            MonoItemIconButton component = dataOld.GetComponent<MonoItemIconButton>();
            return (dataNew.GetComponent<MonoItemIconButton>()._item == component._item);
        }

        private bool IsTabItemHeap(string tabKey)
        {
            return ((tabKey != "WeaponTab") && (tabKey != "StigmataTab"));
        }

        private void OnFragmentTabBtnClick()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = "FragmentTab";
            this._tabManager.ShowTab(searchKey);
            this.ClearOnChangedTab(showingTabKey, searchKey);
        }

        private void OnItemButonClick(StorageDataItemBase item, bool selected)
        {
            if (this.featureType == FeatureType.SelectForSell)
            {
                this._sellPanel.RefreshOnItemButonClick(item);
                this.RefreshShowingTabItem();
            }
            else if (this.featureType == FeatureType.SelectForPowerUp)
            {
                this._powerUpPanel.RefreshOnItemButonClick(item);
                this.RefreshShowingTabItem();
            }
            else
            {
                UIUtil.ShowItemDetail(item, false, true);
            }
        }

        private void OnItemMinusBtnClick(StorageDataItemBase dataItem)
        {
            if (this._powerUpPanel != null)
            {
                this._powerUpPanel.OnDecreaseBtnClick(dataItem);
            }
        }

        private void OnItemTabBtnClick()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = "ItemTab";
            this._tabManager.ShowTab(searchKey);
            this.ClearOnChangedTab(showingTabKey, searchKey);
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            GameObject showingTabContent = this._tabManager.GetShowingTabContent();
            if (this._fadeManagerDict.ContainsKey(showingTabContent))
            {
                this._fadeManagerDict[showingTabContent].Init(this._scrollerDict[showingTabContent].GetItemDict(), null, new Func<RectTransform, RectTransform, bool>(this.IsStorageItemDataEqual));
                this._fadeManagerDict[showingTabContent].Play();
                this._itemBeforeDict[showingTabContent] = null;
            }
            base.view.GetComponent<MonoFadeInAnimManager>().Play("tab_btns_fade_in", false, null);
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.SetSellViewActive)
            {
                return this.OnSetSellViewActive((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.SetStorageSortType)
            {
                return this.OnSetSortType((StorageModule.StorageSortType) ((int) ntf.body));
            }
            if (ntf.type == NotifyTypes.RefreshStorageShowing)
            {
                this.RefreshShowingTabItem();
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x1b:
                case 0x1c:
                    this.SetupView();
                    break;

                case 0x67:
                    this.SetupFragmentTab();
                    if (this._tabManager.GetShowingTabKey() == "FragmentTab")
                    {
                        GameObject showingTabContent = this._tabManager.GetShowingTabContent();
                        if (this._fadeManagerDict.ContainsKey(showingTabContent))
                        {
                            this._fadeManagerDict[showingTabContent].Init(this._scrollerDict[showingTabContent].GetItemDict(), null, new Func<RectTransform, RectTransform, bool>(this.IsStorageItemDataEqual));
                            this._fadeManagerDict[showingTabContent].Play();
                            this._itemBeforeDict[showingTabContent] = null;
                        }
                    }
                    break;

                case 40:
                case 0x88:
                    return this.SetupView();
            }
            return false;
        }

        private void OnScrollerChange(List<StorageDataItemBase> list, Transform trans, int index)
        {
            StorageDataItemBase item = list[index];
            MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
            component.showProtected = true;
            component.blockSelect = false;
            if (this.featureType == FeatureType.SelectForSell)
            {
                component.blockSelect = !this.FilterForSell(item);
            }
            else if (this.featureType == FeatureType.SelectForPowerUp)
            {
                component.blockSelect = !this.FilterForPowerUp(item);
            }
            bool flag = ((this.featureType == FeatureType.SelectForSell) && (this._sellPanel != null)) && this._sellPanel.IsItemInSelectedMap(item);
            bool flag2 = ((this.featureType == FeatureType.SelectForPowerUp) && (this._powerUpPanel != null)) && this._powerUpPanel.IsItemInSelectedMap(item);
            bool bUsed = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.avatarID) != null;
            component.SetupView(item, MonoItemIconButton.SelectMode.CheckWhenSelect, flag || flag2, false, bUsed);
            component.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnItemButonClick));
            if (this.IsTabItemHeap(this._tabManager.GetShowingTabKey()) && flag2)
            {
                component.SetMinusBtnCallBack(new Action<StorageDataItemBase>(this.OnItemMinusBtnClick));
                component.ShowSelectedNum(this._powerUpPanel.GetItemSelectNum(item));
            }
        }

        private void OnSellBtnClick()
        {
            this.featureType = FeatureType.SelectForSell;
            this.SetupSellView();
        }

        private bool OnSetSellViewActive(bool setActive)
        {
            this.featureType = !setActive ? FeatureType.Normal : FeatureType.SelectForSell;
            this.SetupSellView();
            return false;
        }

        private bool OnSetSortType(StorageModule.StorageSortType sortType)
        {
            this.SetupSortView(true);
            this.SortByTab(this._tabManager.GetShowingTabKey(), this._tabManager.GetShowingTabContent());
            this.PlayCurrentTabAnimation();
            return false;
        }

        private void OnSortBGClick()
        {
            this.SetupSortView(false);
        }

        private void OnSortBtnClick()
        {
            this.SetupSortView(true);
        }

        private void OnStigmataTabBtnClick()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = "StigmataTab";
            this._tabManager.ShowTab(searchKey);
            this.ClearOnChangedTab(showingTabKey, searchKey);
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
            btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.transform.Find("Image").GetComponent<Image>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.interactable = !active;
            go.SetActive(active);
            if (active && this._fadeManagerDict.ContainsKey(go))
            {
                this._fadeManagerDict[go].Init(this._scrollerDict[go].GetItemDict(), null, new Func<RectTransform, RectTransform, bool>(this.IsStorageItemDataEqual));
                this._fadeManagerDict[go].Play();
                this._itemBeforeDict[go] = null;
            }
        }

        private void OnWeaponTabBtnClick()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = "WeaponTab";
            this._tabManager.ShowTab(searchKey);
            this.ClearOnChangedTab(showingTabKey, searchKey);
        }

        private void PlayCurrentTabAnimation()
        {
            GameObject showingTabContent = this._tabManager.GetShowingTabContent();
            if (this._fadeManagerDict.ContainsKey(showingTabContent))
            {
                this._fadeManagerDict[showingTabContent].Init(this._scrollerDict[showingTabContent].GetItemDict(), null, new Func<RectTransform, RectTransform, bool>(this.IsStorageItemDataEqual));
                this._fadeManagerDict[showingTabContent].Play();
                this._itemBeforeDict[showingTabContent] = null;
            }
        }

        private void RefreshShowingTabItem()
        {
            this._tabManager.GetShowingTabContent().transform.Find("ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
        }

        private void RefreshTabItemByKey(string key)
        {
            this._tabManager.GetTabContent(key).transform.Find("ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
        }

        private void SetupFragmentTab()
        {
            GameObject gameObject = base.view.transform.Find("FragmentTab").gameObject;
            if (<>f__am$cache12 == null)
            {
                <>f__am$cache12 = entry => entry.Key;
            }
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = entry => entry.Value;
            }
            this._itemBeforeDict[gameObject] = Enumerable.ToDictionary<KeyValuePair<int, RectTransform>, int, RectTransform>(this._scrollerDict[gameObject].GetItemDict(), <>f__am$cache12, <>f__am$cache13);
            if (this.featureType == FeatureType.SelectForPowerUp)
            {
                base.view.transform.Find("TabBtns/TabBtn_4").gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
            else
            {
                base.view.transform.Find("TabBtns/TabBtn_4").gameObject.SetActive(true);
                this.SetupTab("FragmentTab", base.view.transform.Find("TabBtns/TabBtn_4").GetComponent<Button>(), base.view.transform.Find("FragmentTab").gameObject, Singleton<StorageModule>.Instance.GetFragmentList());
            }
        }

        private void SetupItemTab()
        {
            GameObject gameObject = base.view.transform.Find("ItemTab").gameObject;
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = entry => entry.Key;
            }
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = entry => entry.Value;
            }
            this._itemBeforeDict[gameObject] = Enumerable.ToDictionary<KeyValuePair<int, RectTransform>, int, RectTransform>(this._scrollerDict[gameObject].GetItemDict(), <>f__am$cache10, <>f__am$cache11);
            List<StorageDataItemBase> allUserMaterial = Singleton<StorageModule>.Instance.GetAllUserMaterial();
            if (this.featureType == FeatureType.SelectForPowerUp)
            {
                allUserMaterial = allUserMaterial.FindAll(new Predicate<StorageDataItemBase>(this.FilterForPowerUp));
            }
            else if (this.featureType == FeatureType.SelectForSell)
            {
                allUserMaterial = allUserMaterial.FindAll(new Predicate<StorageDataItemBase>(this.FilterForSell));
            }
            this.SetupTab("ItemTab", base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), base.view.transform.Find("ItemTab").gameObject, allUserMaterial);
        }

        private void SetupPowerUpView()
        {
            this._powerUpPanel = base.view.transform.Find("PowerUpPanel").GetComponent<MonoStorageSelectForPowerUp>();
            base.view.transform.Find("SellBtn").gameObject.SetActive(this.featureType == FeatureType.Normal);
            base.view.transform.Find("PowerUpPanel").gameObject.SetActive(this.featureType == FeatureType.SelectForPowerUp);
            if (this.featureType == FeatureType.SelectForPowerUp)
            {
                bool isMulti = this.IsTabItemHeap(this._tabManager.GetShowingTabKey());
                this._powerUpPanel.GetComponent<MonoStorageSelectForPowerUp>().SetupView(this.selectedResources, isMulti, this.powerUpTarget);
            }
            this.RefreshShowingTabItem();
            base.view.transform.Find("NoItemCanUse").gameObject.SetActive(false);
            if (this.featureType == FeatureType.SelectForPowerUp)
            {
                StorageModule instance = Singleton<StorageModule>.Instance;
                bool flag2 = (instance.GetAllUserWeapons().Count > 0) && (instance.GetAllUserStigmata().Count > 0);
                if (!flag2)
                {
                    foreach (StorageDataItemBase base2 in instance.GetAllUserMaterial())
                    {
                        if (base2.GetGearExp() > 0f)
                        {
                            flag2 = true;
                            break;
                        }
                    }
                }
                base.view.transform.Find("NoItemCanUse").gameObject.SetActive(!flag2);
            }
        }

        private void SetupSellView()
        {
            this._sellPanel = base.view.transform.Find("SellPanel").GetComponent<MonoStorageSelectForSellPanel>();
            base.view.transform.Find("SellBtn").gameObject.SetActive(this.featureType == FeatureType.Normal);
            base.view.transform.Find("SellPanel").gameObject.SetActive(this.featureType == FeatureType.SelectForSell);
            if (this.featureType == FeatureType.SelectForSell)
            {
                bool isMultiSell = this.IsTabItemHeap(this._tabManager.GetShowingTabKey());
                this._sellPanel.SetupView(isMultiSell);
            }
            this.RefreshShowingTabItem();
        }

        private void SetupSortView(bool sortActive)
        {
            base.view.transform.Find("SortPanel").gameObject.SetActive(sortActive);
            base.view.transform.Find("SortBtn").GetComponent<Button>().interactable = !sortActive;
            if (sortActive)
            {
                Transform transform = base.view.transform.Find("SortPanel/Content");
                bool flag = this.IsTabItemHeap(this._tabManager.GetShowingTabKey());
                transform.Find("SortFuncBtnLevel").gameObject.SetActive(!flag);
                transform.Find("SortFuncBtnCost").gameObject.SetActive(!flag);
                transform.Find("SortFuncBtnTime").gameObject.SetActive(!flag);
                foreach (MonoStorageSortButton button in transform.GetComponentsInChildren<MonoStorageSortButton>())
                {
                    if (button.gameObject.activeSelf)
                    {
                        button.SetupView(this._tabManager.GetShowingTabKey());
                    }
                }
            }
        }

        private void SetupStigmataTab()
        {
            GameObject gameObject = base.view.transform.Find("StigmataTab").gameObject;
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = entry => entry.Key;
            }
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = entry => entry.Value;
            }
            this._itemBeforeDict[gameObject] = Enumerable.ToDictionary<KeyValuePair<int, RectTransform>, int, RectTransform>(this._scrollerDict[gameObject].GetItemDict(), <>f__am$cacheE, <>f__am$cacheF);
            List<StorageDataItemBase> allUserStigmata = Singleton<StorageModule>.Instance.GetAllUserStigmata();
            if (this.featureType == FeatureType.SelectForPowerUp)
            {
                allUserStigmata = allUserStigmata.FindAll(new Predicate<StorageDataItemBase>(this.FilterForPowerUp));
            }
            else if (this.featureType == FeatureType.SelectForSell)
            {
                allUserStigmata = allUserStigmata.FindAll(new Predicate<StorageDataItemBase>(this.FilterForSell));
            }
            this.SetupTab("StigmataTab", base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), base.view.transform.Find("StigmataTab").gameObject, allUserStigmata);
        }

        private void SetupTab(string key, Button tabBtn, GameObject tabGo, List<StorageDataItemBase> list)
        {
            if (this._tabItemList.ContainsKey(key))
            {
                this._tabItemList[key] = list;
            }
            else
            {
                this._tabItemList.Add(key, list);
            }
            StorageModule.StorageSortType type = Singleton<StorageModule>.Instance.sortTypeMap[key];
            this._tabItemList[key].Sort(Singleton<StorageModule>.Instance.sortComparisionMap[type]);
            this.SortByTab(key, tabGo);
            this._tabManager.SetTab(key, tabBtn, tabGo);
        }

        protected override bool SetupView()
        {
            this.InitScroller();
            if (string.IsNullOrEmpty(this.defaultTab))
            {
                string storageShowTabName = Singleton<MiHoYoGameData>.Instance.LocalData.StorageShowTabName;
                if (string.IsNullOrEmpty(storageShowTabName))
                {
                    storageShowTabName = "WeaponTab";
                }
                this.defaultTab = storageShowTabName;
            }
            this.SetupWeaponTab();
            this.SetupStigmataTab();
            this.SetupItemTab();
            this.SetupFragmentTab();
            this._tabManager.ShowTab(this.defaultTab);
            this.SetupSortView(false);
            this.SetupSellView();
            this.SetupPowerUpView();
            base.view.GetComponent<MonoFadeInAnimManager>().Play("tab_btns_fade_in", false, null);
            return false;
        }

        private void SetupWeaponTab()
        {
            GameObject gameObject = base.view.transform.Find("WeaponTab").gameObject;
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = entry => entry.Key;
            }
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = entry => entry.Value;
            }
            this._itemBeforeDict[gameObject] = Enumerable.ToDictionary<KeyValuePair<int, RectTransform>, int, RectTransform>(this._scrollerDict[gameObject].GetItemDict(), <>f__am$cacheC, <>f__am$cacheD);
            List<StorageDataItemBase> allUserWeapons = Singleton<StorageModule>.Instance.GetAllUserWeapons();
            if (this.featureType == FeatureType.SelectForPowerUp)
            {
                allUserWeapons = allUserWeapons.FindAll(new Predicate<StorageDataItemBase>(this.FilterForPowerUp));
            }
            else if (this.featureType == FeatureType.SelectForSell)
            {
                allUserWeapons = allUserWeapons.FindAll(new Predicate<StorageDataItemBase>(this.FilterForSell));
            }
            this.SetupTab("WeaponTab", base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), base.view.transform.Find("WeaponTab").gameObject, allUserWeapons);
        }

        private void SortByTab(string key, GameObject tabGo)
        {
            <SortByTab>c__AnonStorey103 storey = new <SortByTab>c__AnonStorey103 {
                key = key,
                <>f__this = this
            };
            StorageModule.StorageSortType type = Singleton<StorageModule>.Instance.sortTypeMap[storey.key];
            this._tabItemList[storey.key].Sort(Singleton<StorageModule>.Instance.sortComparisionMap[type]);
            tabGo.transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(storey.<>m__1A3), this._tabItemList[storey.key].Count, null);
        }

        [CompilerGenerated]
        private sealed class <SortByTab>c__AnonStorey103
        {
            internal StorageShowPageContext <>f__this;
            internal string key;

            internal void <>m__1A3(Transform trans, int index)
            {
                this.<>f__this.OnScrollerChange(this.<>f__this._tabItemList[this.key], trans, index);
            }
        }

        public enum FeatureType
        {
            Normal,
            SelectForSell,
            SelectForPowerUp
        }
    }
}

