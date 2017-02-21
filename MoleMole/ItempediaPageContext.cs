namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class ItempediaPageContext : BasePageContext
    {
        private TabInfo _currentTabInfo;
        private TabInfo _itemTabInfo = new TabInfo();
        private Button _sortButton;
        private MonoItempediaSortButton _sortButtonCost;
        private MonoItempediaSortButton _sortButtonLevel;
        private MonoItempediaSortButton _sortButtonRarity;
        private MonoItempediaSortButton _sortButtonSuite;
        private MonoItempediaSortButton _sortButtonType;
        private GameObject _sortPanel;
        private TabInfo _stigmataTabInfo = new TabInfo();
        private TabManager _tabManager;
        private static MemorizeInfo _tabMemorizedInfo;
        private TabInfo _weaponTabInfo = new TabInfo();
        public string defaultTab = "WeaponTab";
        private const string ITEM_TAB = "ItemTab";
        private const string STIGMATA_TAB = "StigmataTab";
        private const string WEAPON_TAB = "WeaponTab";

        public ItempediaPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ItempediaPageContext",
                viewPrefabPath = "UI/Menus/Page/Itempedia/ItempediaPage"
            };
            base.config = pattern;
            this._tabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), new UnityAction(this.OnWeaponTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), new UnityAction(this.OnStigmataTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), new UnityAction(this.OnItemTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("SortBtn").GetComponent<Button>(), new UnityAction(this.OnSortBtnClick));
            base.BindViewCallback(base.view.transform.Find("SortPanel/BG").GetComponent<Button>(), new UnityAction(this.OnSortBGClick));
        }

        private bool IsItempediaDataEqual(RectTransform dataNew, RectTransform dataOld)
        {
            if ((dataNew == null) || (dataOld == null))
            {
                return false;
            }
            MonoItempediaIconButton component = dataOld.GetComponent<MonoItempediaIconButton>();
            return (dataNew.GetComponent<MonoItempediaIconButton>()._item == component._item);
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

        private void OnItemButtonClick(ItempediaDataAdapter item)
        {
            bool unlock = false;
            int[] allUnlockItems = Singleton<ItempediaModule>.Instance.GetAllUnlockItems();
            int index = 0;
            int length = allUnlockItems.Length;
            while (index < length)
            {
                if (item.ID == allUnlockItems[index])
                {
                    unlock = true;
                    break;
                }
                index++;
            }
            UIUtil.ShowItemDetail(item.GetDummyStorageItemData(), true, unlock);
        }

        private void OnItemTabBtnClick()
        {
            this._currentTabInfo = this._itemTabInfo;
            this._tabManager.ShowTab("ItemTab");
            this.SetMemorizeTabName("ItemTab");
            this.PlayCurrentTabAnimation();
            this.SetCollection("ItemTab");
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            base.view.GetComponent<MonoFadeInAnimManager>().Play("tab_btns_fade_in", false, null);
            this.PlayCurrentTabAnimation();
        }

        private void OnScrollerChange(List<int> list, Transform trans, int index)
        {
        }

        private void OnSortBGClick()
        {
            this.SetupSortView(false);
        }

        private void OnSortBtnClick()
        {
            this.SetupSortView(true);
        }

        private void OnSortByCostClicked()
        {
            if (this._currentTabInfo.sortType == StorageModule.StorageSortType.Cost_ASC)
            {
                this._currentTabInfo.sortType = StorageModule.StorageSortType.Cost_DESC;
            }
            else
            {
                this._currentTabInfo.sortType = StorageModule.StorageSortType.Cost_ASC;
            }
            this.SortItems(this._currentTabInfo);
            this.SetupTab(this._currentTabInfo);
            this.SetupSortView(true);
            this.PlayCurrentTabAnimation();
        }

        private void OnSortByLevelClicked()
        {
            if (this._currentTabInfo.sortType == StorageModule.StorageSortType.Level_ASC)
            {
                this._currentTabInfo.sortType = StorageModule.StorageSortType.Level_DESC;
            }
            else
            {
                this._currentTabInfo.sortType = StorageModule.StorageSortType.Level_ASC;
            }
            this.SortItems(this._currentTabInfo);
            this.SetupTab(this._currentTabInfo);
            this.SetupSortView(true);
            this.PlayCurrentTabAnimation();
        }

        private void OnSortByRarityClicked()
        {
            if (this._currentTabInfo.sortType == StorageModule.StorageSortType.Rarity_ASC)
            {
                this._currentTabInfo.sortType = StorageModule.StorageSortType.Rarity_DESC;
            }
            else
            {
                this._currentTabInfo.sortType = StorageModule.StorageSortType.Rarity_ASC;
            }
            this.SortItems(this._currentTabInfo);
            this.SetupTab(this._currentTabInfo);
            this.SetupSortView(true);
            this.PlayCurrentTabAnimation();
        }

        private void OnSortBySuiteClicked()
        {
            if (this._currentTabInfo.sortType == StorageModule.StorageSortType.Suite_ASC)
            {
                this._currentTabInfo.sortType = StorageModule.StorageSortType.Suite_DESC;
            }
            else
            {
                this._currentTabInfo.sortType = StorageModule.StorageSortType.Suite_ASC;
            }
            this.SortItems(this._currentTabInfo);
            this.SetupTab(this._currentTabInfo);
            this.SetupSortView(true);
            this.PlayCurrentTabAnimation();
        }

        private void OnSortByTypeClicked()
        {
            if (this._currentTabInfo.sortType == StorageModule.StorageSortType.BaseType_ASC)
            {
                this._currentTabInfo.sortType = StorageModule.StorageSortType.BaseType_DESC;
            }
            else
            {
                this._currentTabInfo.sortType = StorageModule.StorageSortType.BaseType_ASC;
            }
            this.SortItems(this._currentTabInfo);
            this.SetupTab(this._currentTabInfo);
            this.SetupSortView(true);
            this.PlayCurrentTabAnimation();
        }

        private void OnStigmataTabBtnClick()
        {
            this._currentTabInfo = this._stigmataTabInfo;
            this._tabManager.ShowTab("StigmataTab");
            this.SetMemorizeTabName("StigmataTab");
            this.PlayCurrentTabAnimation();
            this.SetCollection("StigmataTab");
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
            btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.transform.Find("Image").GetComponent<Image>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.interactable = !active;
            go.SetActive(active);
        }

        private void OnWeaponTabBtnClick()
        {
            this._currentTabInfo = this._weaponTabInfo;
            this._tabManager.ShowTab("WeaponTab");
            this.SetMemorizeTabName("WeaponTab");
            this.PlayCurrentTabAnimation();
            this.SetCollection("WeaponTab");
        }

        private void PlayCurrentTabAnimation()
        {
            if (this._currentTabInfo != null)
            {
                this._currentTabInfo.fadeManager.Init(this._currentTabInfo.scroller.GetItemDict(), null, new Func<RectTransform, RectTransform, bool>(this.IsStorageItemDataEqual));
                this._currentTabInfo.fadeManager.Play();
            }
        }

        private void SetCollection(string tabName)
        {
            GameObject gameObject = base.view.transform.Find("Collection").gameObject;
            Text component = base.view.transform.Find("Collection/Text").GetComponent<Text>();
            Text text2 = base.view.transform.Find("Collection/Title").GetComponent<Text>();
            int unlockCountWeapon = 0;
            int allWeaponCount = 0;
            if (tabName == "WeaponTab")
            {
                gameObject.SetActive(true);
                unlockCountWeapon = Singleton<ItempediaModule>.Instance.GetUnlockCountWeapon();
                allWeaponCount = Singleton<ItempediaModule>.Instance.GetAllWeaponCount();
                text2.text = LocalizationGeneralLogic.GetText("Menu_Desc_WeaponIndexCompleteness", new object[0]);
            }
            else if (tabName == "StigmataTab")
            {
                gameObject.SetActive(true);
                unlockCountWeapon = Singleton<ItempediaModule>.Instance.GetUnlockCountStigmata();
                allWeaponCount = Singleton<ItempediaModule>.Instance.GetAllStigmataCount();
                text2.text = LocalizationGeneralLogic.GetText("Menu_Desc_StigmataIndexCompleteness", new object[0]);
            }
            else if (tabName == "ItemTab")
            {
                gameObject.SetActive(false);
            }
            component.text = string.Format("{0}/{1}", unlockCountWeapon.ToString(), allWeaponCount.ToString());
        }

        private void SetMemorizeTabName(string name)
        {
            if (_tabMemorizedInfo == null)
            {
                _tabMemorizedInfo = new MemorizeInfo();
            }
            _tabMemorizedInfo.lastTabName = name;
        }

        private void SetupScrollView(List<ItempediaDataAdapter> dataList, MonoGridScroller scroller)
        {
            <SetupScrollView>c__AnonStoreyFF yff = new <SetupScrollView>c__AnonStoreyFF {
                dataList = dataList,
                <>f__this = this
            };
            scroller.Init(new MonoGridScroller.OnChange(yff.<>m__17F), yff.dataList.Count, null);
        }

        private void SetupSortView(bool sortActive)
        {
            this._sortPanel.SetActive(sortActive);
            this._sortButton.interactable = !sortActive;
            if (sortActive)
            {
                string showingTabKey = this._tabManager.GetShowingTabKey();
                bool flag = this.IsTabItemHeap(showingTabKey);
                this._sortButtonLevel.gameObject.SetActive(false);
                this._sortButtonCost.gameObject.SetActive(!flag);
                this._sortButtonSuite.gameObject.SetActive(showingTabKey == "StigmataTab");
                this._sortButtonRarity.SetupView((this._currentTabInfo.sortType == StorageModule.StorageSortType.Rarity_ASC) | (this._currentTabInfo.sortType == StorageModule.StorageSortType.Rarity_DESC), this._currentTabInfo.sortType == StorageModule.StorageSortType.Rarity_ASC);
                this._sortButtonType.SetupView((this._currentTabInfo.sortType == StorageModule.StorageSortType.BaseType_ASC) | (this._currentTabInfo.sortType == StorageModule.StorageSortType.BaseType_DESC), this._currentTabInfo.sortType == StorageModule.StorageSortType.BaseType_ASC);
                this._sortButtonLevel.SetupView((this._currentTabInfo.sortType == StorageModule.StorageSortType.Level_ASC) | (this._currentTabInfo.sortType == StorageModule.StorageSortType.Level_DESC), this._currentTabInfo.sortType == StorageModule.StorageSortType.Level_ASC);
                this._sortButtonCost.SetupView((this._currentTabInfo.sortType == StorageModule.StorageSortType.Cost_ASC) | (this._currentTabInfo.sortType == StorageModule.StorageSortType.Cost_DESC), this._currentTabInfo.sortType == StorageModule.StorageSortType.Cost_ASC);
                this._sortButtonSuite.SetupView((this._currentTabInfo.sortType == StorageModule.StorageSortType.Suite_ASC) | (this._currentTabInfo.sortType == StorageModule.StorageSortType.Suite_DESC), this._currentTabInfo.sortType == StorageModule.StorageSortType.Suite_ASC);
            }
        }

        private void SetupTab(TabInfo info)
        {
            if (info.itemList != null)
            {
                this.SetupScrollView(info.itemList, info.scroller);
            }
        }

        private void SetupTabDataList(TabInfo info, object[] metaDataList)
        {
            if (metaDataList != null)
            {
                info.itemList = new List<ItempediaDataAdapter>(metaDataList.Length);
                int index = 0;
                int length = metaDataList.Length;
                while (index < length)
                {
                    ItempediaDataAdapter item = new ItempediaDataAdapter(metaDataList[index]);
                    if (Singleton<ItempediaModule>.Instance.IsInItempedia(item.ID))
                    {
                        info.itemList.Add(item);
                    }
                    index++;
                }
            }
        }

        protected override bool SetupView()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = !string.IsNullOrEmpty(showingTabKey) ? showingTabKey : "WeaponTab";
            this._tabManager.Clear();
            GameObject content = null;
            Button btn = null;
            content = base.view.transform.Find("WeaponTab").gameObject;
            btn = base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>();
            this._tabManager.SetTab("WeaponTab", btn, content);
            content = base.view.transform.Find("StigmataTab").gameObject;
            btn = base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>();
            this._tabManager.SetTab("StigmataTab", btn, content);
            content = base.view.transform.Find("ItemTab").gameObject;
            btn = base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>();
            this._tabManager.SetTab("ItemTab", btn, content);
            this._tabManager.ShowTab(searchKey);
            base.view.GetComponent<MonoFadeInAnimManager>().Play("tab_btns_fade_in", false, null);
            this._sortButtonRarity = base.view.transform.Find("SortPanel/Content/SortFuncBtnRarity").GetComponent<MonoItempediaSortButton>();
            this._sortButtonType = base.view.transform.Find("SortPanel/Content/SortFuncBtnType").GetComponent<MonoItempediaSortButton>();
            this._sortButtonLevel = base.view.transform.Find("SortPanel/Content/SortFuncBtnLevel").GetComponent<MonoItempediaSortButton>();
            this._sortButtonCost = base.view.transform.Find("SortPanel/Content/SortFuncBtnCost").GetComponent<MonoItempediaSortButton>();
            this._sortButtonSuite = base.view.transform.Find("SortPanel/Content/SortFuncBtnSuite").GetComponent<MonoItempediaSortButton>();
            this._sortButton = base.view.transform.Find("SortBtn").GetComponent<Button>();
            this._sortPanel = base.view.transform.Find("SortPanel").gameObject;
            this._sortButtonRarity.SetClickCallback(new Action(this.OnSortByRarityClicked));
            this._sortButtonType.SetClickCallback(new Action(this.OnSortByTypeClicked));
            this._sortButtonLevel.SetClickCallback(new Action(this.OnSortByLevelClicked));
            this._sortButtonCost.SetClickCallback(new Action(this.OnSortByCostClicked));
            this._sortButtonSuite.SetClickCallback(new Action(this.OnSortBySuiteClicked));
            this._weaponTabInfo.scroller = base.view.transform.Find("WeaponTab/ScrollView").GetComponent<MonoGridScroller>();
            this._stigmataTabInfo.scroller = base.view.transform.Find("StigmataTab/ScrollView").GetComponent<MonoGridScroller>();
            this._itemTabInfo.scroller = base.view.transform.Find("ItemTab/ScrollView").GetComponent<MonoGridScroller>();
            this._weaponTabInfo.fadeManager = base.view.transform.Find("WeaponTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
            this._stigmataTabInfo.fadeManager = base.view.transform.Find("StigmataTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
            this._itemTabInfo.fadeManager = base.view.transform.Find("ItemTab/ScrollView").GetComponent<MonoScrollerFadeManager>();
            this._stigmataTabInfo.sortType = StorageModule.StorageSortType.Suite_ASC;
            this.SetupTabDataList(this._weaponTabInfo, WeaponMetaDataReader.GetItemList().ToArray());
            this.SetupTabDataList(this._stigmataTabInfo, StigmataMetaDataReader.GetItemList().ToArray());
            this.SetupTabDataList(this._itemTabInfo, ItemMetaDataReader.GetItemList().ToArray());
            this.SortItems(this._weaponTabInfo);
            this.SortItems(this._stigmataTabInfo);
            this.SortItems(this._itemTabInfo);
            this.SetupTab(this._weaponTabInfo);
            this.SetupTab(this._stigmataTabInfo);
            this.SetupTab(this._itemTabInfo);
            switch (((_tabMemorizedInfo != null) ? _tabMemorizedInfo.lastTabName : searchKey))
            {
                case "WeaponTab":
                    this.OnWeaponTabBtnClick();
                    break;

                case "StigmataTab":
                    this.OnStigmataTabBtnClick();
                    break;

                case "ItemTab":
                    this.OnItemTabBtnClick();
                    break;
            }
            return false;
        }

        private void SortItems(TabInfo tabInfo)
        {
            switch (tabInfo.sortType)
            {
                case StorageModule.StorageSortType.Rarity_ASC:
                    tabInfo.itemList.Sort(new Comparison<ItempediaDataAdapter>(ItempediaDataAdapter.CompareToRarityAsc));
                    break;

                case StorageModule.StorageSortType.Rarity_DESC:
                    tabInfo.itemList.Sort(new Comparison<ItempediaDataAdapter>(ItempediaDataAdapter.CompareToRarityDesc));
                    break;

                case StorageModule.StorageSortType.BaseType_ASC:
                    tabInfo.itemList.Sort(new Comparison<ItempediaDataAdapter>(ItempediaDataAdapter.CompareToBaseTypeAsc));
                    break;

                case StorageModule.StorageSortType.BaseType_DESC:
                    tabInfo.itemList.Sort(new Comparison<ItempediaDataAdapter>(ItempediaDataAdapter.CompareToBaseTypeDesc));
                    break;

                case StorageModule.StorageSortType.Level_ASC:
                    tabInfo.itemList.Sort(new Comparison<ItempediaDataAdapter>(ItempediaDataAdapter.CompareToLevelAsc));
                    break;

                case StorageModule.StorageSortType.Level_DESC:
                    tabInfo.itemList.Sort(new Comparison<ItempediaDataAdapter>(ItempediaDataAdapter.CompareToLevelDesc));
                    break;

                case StorageModule.StorageSortType.Cost_ASC:
                    tabInfo.itemList.Sort(new Comparison<ItempediaDataAdapter>(ItempediaDataAdapter.CompareToCostAsc));
                    break;

                case StorageModule.StorageSortType.Cost_DESC:
                    tabInfo.itemList.Sort(new Comparison<ItempediaDataAdapter>(ItempediaDataAdapter.CompareToCostDesc));
                    break;

                case StorageModule.StorageSortType.Suite_ASC:
                    tabInfo.itemList.Sort(new Comparison<ItempediaDataAdapter>(ItempediaDataAdapter.CompareToSuiteAsc));
                    break;

                case StorageModule.StorageSortType.Suite_DESC:
                    tabInfo.itemList.Sort(new Comparison<ItempediaDataAdapter>(ItempediaDataAdapter.CompareToSuiteDesc));
                    break;
            }
        }

        [CompilerGenerated]
        private sealed class <SetupScrollView>c__AnonStoreyFF
        {
            internal ItempediaPageContext <>f__this;
            internal List<ItempediaDataAdapter> dataList;

            internal void <>m__17F(Transform t, int i)
            {
                MonoItempediaIconButton component = t.GetComponent<MonoItempediaIconButton>();
                ItempediaDataAdapter item = this.dataList[i];
                int[] allUnlockItems = Singleton<ItempediaModule>.Instance.GetAllUnlockItems();
                bool active = false;
                int index = 0;
                int length = allUnlockItems.Length;
                while (index < length)
                {
                    if (allUnlockItems[index] == item.ID)
                    {
                        active = true;
                        break;
                    }
                    index++;
                }
                component.SetupView(item, active);
                component.SetClickCallback(new MonoItempediaIconButton.ClickCallBack(this.<>f__this.OnItemButtonClick));
            }
        }

        private class MemorizeInfo
        {
            public string lastTabName;
        }

        private class TabInfo
        {
            public MonoScrollerFadeManager fadeManager;
            public List<ItempediaDataAdapter> itemList;
            public MonoGridScroller scroller;
            public StorageModule.StorageSortType sortType = StorageModule.StorageSortType.Rarity_ASC;
        }
    }
}

