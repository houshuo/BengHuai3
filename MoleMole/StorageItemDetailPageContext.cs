namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class StorageItemDetailPageContext : BasePageContext
    {
        private TabManager _stigmataTabManager;
        public readonly bool hideActionBtns;
        public bool showIdentifyBtnOnly;
        public const string SIGMATA_AFFIX_SKILL_TAB = "S_Affix_Skill_Tab";
        public const string SIGMATA_SET_SKILL_TAB = "S_Suit_Skill_Tab";
        public const string SIGMATA_SKILL_TAB = "S_Skill_Tab";
        public StorageDataItemBase storageItem;
        public AvatarDataItem uiEquipOwner;
        public readonly bool unlock;

        public StorageItemDetailPageContext(StorageDataItemBase storageItem, bool hideActionBtns = false, bool unlock = true)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "StorageItemDetailPageContext",
                viewPrefabPath = "UI/Menus/Page/Storage/WeaponDetailPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            if (storageItem is StigmataDataItem)
            {
                base.config.viewPrefabPath = "UI/Menus/Page/Storage/StigmataDetailPage";
            }
            this.storageItem = storageItem;
            this.hideActionBtns = hideActionBtns;
            this.unlock = unlock;
            this._stigmataTabManager = new TabManager();
            this._stigmataTabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("ActionBtns/PowerUpBtn").GetComponent<Button>(), new UnityAction(this.OnPowerUpButtonClick));
            base.BindViewCallback(base.view.transform.Find("ActionBtns/RarityUpBtn").GetComponent<Button>(), new UnityAction(this.OnUpRarityButtonClick));
            if (this.storageItem is StigmataDataItem)
            {
                base.BindViewCallback(base.view.transform.Find("Info/Content/LockButton").GetComponent<Button>(), new UnityAction(this.OnLockButtonClick));
                base.BindViewCallback(base.view.transform.Find("Info/IdentifyBtn").GetComponent<Button>(), new UnityAction(this.OnIdentifyBtnClick));
                base.BindViewCallback(base.view.transform.Find("Info/Content/BtnInEquip").GetComponent<Button>(), new UnityAction(this.OnOwnerAvatarBtnClick));
            }
            else
            {
                base.BindViewCallback(base.view.transform.Find("Info/Info/Content/Equipment/LockButton").GetComponent<Button>(), new UnityAction(this.OnLockButtonClick));
                base.BindViewCallback(base.view.transform.Find("Info/Info/Content/Equipment/BtnInEquip").GetComponent<Button>(), new UnityAction(this.OnOwnerAvatarBtnClick));
            }
            if (this.storageItem is StigmataDataItem)
            {
                base.BindViewCallback(base.view.transform.Find("Skills/TabBtns/TabBtn_1").GetComponent<Button>(), new UnityAction(this.OnNaturalSkillTabButtonClick));
                base.BindViewCallback(base.view.transform.Find("Skills/TabBtns/TabBtn_2").GetComponent<Button>(), new UnityAction(this.OnSuitSkillTabButtonClick));
                base.BindViewCallback(base.view.transform.Find("Skills/TabBtns/TabBtn_3").GetComponent<Button>(), new UnityAction(this.OnAffixSkillTabButtonClick));
                base.BindViewCallback(base.view.transform.Find("ActionBtns/NewAffixBtn").GetComponent<Button>(), new UnityAction(this.OnNewAffixBtnClick));
            }
        }

        public override void Destroy()
        {
            if (((this.storageItem != null) && (this.storageItem is WeaponDataItem)) && (base.view != null))
            {
                base.view.transform.Find("Info/Info/Content/Equipment/3dModel").GetComponent<MonoWeaponRenderImage>().CleanUp();
            }
            base.Destroy();
        }

        public void OnAffixSkillTabButtonClick()
        {
            this._stigmataTabManager.ShowTab("S_Affix_Skill_Tab");
        }

        public void OnCloseBtnClick()
        {
            this.ShowStigmataInfo(false);
        }

        public void OnIdentifyBtnClick()
        {
        }

        public bool OnIdentifyStigmataAffixRsp(IdentifyStigmataAffixRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                base.view.transform.Find("Info/IdentifyBtn").GetComponent<DragObject>().OnIdentifyStigmataAffixSucc();
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]), 2f), UIType.Any);
            }
            return false;
        }

        public void OnLockButtonClick()
        {
            Singleton<NetworkManager>.Instance.RequestChangeEquipmentProtectdStatus(this.storageItem);
        }

        public void OnNaturalSkillTabButtonClick()
        {
            this._stigmataTabManager.ShowTab("S_Skill_Tab");
        }

        public void OnNewAffixBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new StigmataNewAffixPageContext(this.storageItem as StigmataDataItem), UIType.Page);
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.EquipPowerupOrEvo)
            {
                this.SetupItemData((StorageDataItemBase) ntf.body);
            }
            else if (ntf.type == NotifyTypes.RefreshStigmataDetailView)
            {
                this.SetupView();
                this._stigmataTabManager.ShowTab("S_Affix_Skill_Tab");
                base.TryToDoTutorial();
            }
            else if (ntf.type == NotifyTypes.StigmataNewAffix)
            {
                this.SetupView();
            }
            return false;
        }

        public void OnOpenBtnClick()
        {
            this.ShowStigmataInfo(true);
        }

        public void OnOwnerAvatarBtnClick()
        {
            AvatarDataItem avatarData = Singleton<AvatarModule>.Instance.TryGetAvatarByID(this.storageItem.avatarID);
            string defaultTab = "WeaponTab";
            if (this.storageItem is StigmataDataItem)
            {
                defaultTab = "StigmataTab";
            }
            Singleton<MainUIManager>.Instance.ShowPage(new AvatarDetailPageContext(avatarData, defaultTab), UIType.Page);
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x4b:
                    this.SetupItemProtectedStatus();
                    break;

                case 0xc0:
                    return this.OnIdentifyStigmataAffixRsp(pkt.getData<IdentifyStigmataAffixRsp>());

                case 40:
                case 0x88:
                    return this.SetupView();
            }
            return false;
        }

        public void OnPowerUpButtonClick()
        {
            if (this.storageItem.level < this.storageItem.GetMaxLevel())
            {
                StoragePowerUpPageContext context = new StoragePowerUpPageContext(this.storageItem) {
                    uiEquipOwner = this.uiEquipOwner
                };
                Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            }
        }

        public void OnSellButtonClick()
        {
            StorageItemSellDialogContext dialogContext = new StorageItemSellDialogContext {
                storageDataItem = this.storageItem
            };
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        public void OnSuitSkillTabButtonClick()
        {
            this._stigmataTabManager.ShowTab("S_Suit_Skill_Tab");
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
            btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.interactable = !active;
            go.SetActive(active);
        }

        public void OnUpRarityButtonClick()
        {
            StorageEvoPageContext context = new StorageEvoPageContext(this.storageItem) {
                uiEquipOwner = this.uiEquipOwner
            };
            Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
        }

        private void SetupAffixSkillsTab(StigmataDataItem stigmataData)
        {
            List<StigmataDataItem.AffixSkillData> affixSkillList = stigmataData.GetAffixSkillList();
            if (!stigmataData.IsAffixIdentify || (affixSkillList.Count > 0))
            {
                base.view.transform.Find("Skills/Info/AffixSkills/Content").GetComponent<MonoStigmataAffixSkillPanel>().SetupView(stigmataData, affixSkillList);
                this._stigmataTabManager.SetTab("S_Affix_Skill_Tab", base.view.transform.Find("Skills/TabBtns/TabBtn_3").GetComponent<Button>(), base.view.transform.Find("Skills/Info/AffixSkills").gameObject);
                base.view.transform.Find("Skills/TabBtns/TabBtn_3").gameObject.SetActive(true);
            }
            else
            {
                base.view.transform.Find("Skills/TabBtns/TabBtn_3").gameObject.SetActive(false);
                base.view.transform.Find("Skills/Info/AffixSkills").gameObject.SetActive(false);
            }
        }

        private void SetupAttributesPanel()
        {
            base.view.transform.Find("Attributes/InfoPanel/BasicStatus").GetComponent<MonoAttributeDisplay>().SetupView(this.storageItem, this.uiEquipOwner);
        }

        private void SetupInfoPanel()
        {
            if (this.storageItem is WeaponDataItem)
            {
                this.SetupWeapon();
            }
            else if (this.storageItem is StigmataDataItem)
            {
                this.SetupStigmata();
            }
        }

        public void SetupItemData(StorageDataItemBase itemData)
        {
            this.storageItem = itemData;
            this.SetupView();
        }

        private void SetupItemProtectedStatus()
        {
            Transform transform;
            if (this.storageItem is StigmataDataItem)
            {
                transform = base.view.transform.Find("Info/Content/LockButton");
            }
            else
            {
                transform = base.view.transform.Find("Info/Info/Content/Equipment/LockButton");
            }
            bool isProtected = this.storageItem.isProtected;
            transform.Find("LockedImage").gameObject.SetActive(isProtected);
            transform.Find("UnlockedImage").gameObject.SetActive(!isProtected);
        }

        private void SetupLvInfoPanel()
        {
            base.view.transform.Find("Lv/InfoRowLv/Lv/CurrentLevelNum").GetComponent<Text>().text = this.storageItem.level.ToString();
            base.view.transform.Find("Lv/InfoRowLv/Lv/MaxLevelNum").GetComponent<Text>().text = this.storageItem.GetMaxLevel().ToString();
            if (this.storageItem.level == this.storageItem.GetMaxLevel())
            {
                base.view.transform.Find("Lv/InfoRowLv/Lv/MaxLevelNum").GetComponent<Text>().color = MiscData.GetColor("Yellow");
            }
            base.view.transform.Find("Lv/InfoRowLv/Exp/NumText").GetComponent<Text>().text = this.storageItem.exp.ToString();
            base.view.transform.Find("Lv/InfoRowLv/Exp/MaxNumText").GetComponent<Text>().text = this.storageItem.GetMaxExp().ToString();
            base.view.transform.Find("Lv/InfoRowLv/Exp/TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue((float) this.storageItem.exp, (float) this.storageItem.GetMaxExp(), 0f);
        }

        private void SetupNaturalSkillsTab(StigmataDataItem stigmataData)
        {
            List<EquipSkillDataItem> skills = stigmataData.skills;
            if (skills.Count > 0)
            {
                base.view.transform.Find("Skills/Info/NaturalSkills/Content").GetComponent<MonoEquipSkillPanel>().SetupView(skills, stigmataData.level);
                this._stigmataTabManager.SetTab("S_Skill_Tab", base.view.transform.Find("Skills/TabBtns/TabBtn_1").GetComponent<Button>(), base.view.transform.Find("Skills/Info/NaturalSkills").gameObject);
                base.view.transform.Find("Skills/TabBtns/TabBtn_1").gameObject.SetActive(true);
            }
            else
            {
                base.view.transform.Find("Skills/TabBtns/TabBtn_1").gameObject.SetActive(false);
                base.view.transform.Find("Skills/Info/NaturalSkills").gameObject.SetActive(false);
            }
        }

        private void SetupSkillsPanel()
        {
            if (this.storageItem is WeaponDataItem)
            {
                this.SetupWeaponSkills();
            }
            else if (this.storageItem is StigmataDataItem)
            {
                this.SetupStigmataSkills();
            }
        }

        private void SetupStigmata()
        {
            StigmataDataItem storageItem = this.storageItem as StigmataDataItem;
            base.view.transform.Find("Info/Content/Equipment/Title/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(storageItem.GetSmallIconPath());
            string displayTitle = this.storageItem.GetDisplayTitle();
            if (storageItem.IsAffixIdentify)
            {
                string affixName = storageItem.GetAffixName();
                if (!string.IsNullOrEmpty(affixName))
                {
                    displayTitle = MiscData.AddColor("Blue", affixName) + " " + displayTitle;
                }
            }
            else
            {
                displayTitle = MiscData.AddColor("WarningRed", storageItem.GetAffixName()) + " " + displayTitle;
            }
            base.view.transform.Find("Info/Content/Equipment/Title/Name").GetComponent<Text>().text = displayTitle;
            base.view.transform.Find("Info/Content/Equipment/Cost/Num").GetComponent<Text>().text = this.storageItem.GetCost().ToString();
            this.SetupItemProtectedStatus();
            base.view.transform.Find("Info/Content/Equipment/Star/EquipStar").GetComponent<MonoEquipSubStar>().SetupView(this.storageItem.rarity, this.storageItem.GetMaxRarity());
            base.view.transform.Find("Info/Content/Equipment/Star/EquipSubStar").GetComponent<MonoEquipSubStar>().SetupView(this.storageItem.GetSubRarity(), this.storageItem.GetMaxSubRarity() - 1);
            base.view.transform.Find("Info/Figure").GetComponent<MonoStigmataFigure>().SetupViewWithIdentifyStatus(storageItem, this.hideActionBtns && !this.unlock);
            if (storageItem.IsAffixIdentify)
            {
                bool flag = (storageItem.CanRefine && (Singleton<TutorialModule>.Instance != null)) && UnlockUIDataReaderExtend.UnlockByTutorial(5);
                base.view.transform.Find("ActionBtns/NewAffixBtn").GetComponent<Button>().interactable = flag;
                bool flag2 = Singleton<StorageModule>.Instance.GetStigmatasCanUseForNewAffix(storageItem).Count > 0;
                base.view.transform.Find("ActionBtns/NewAffixBtn/PopUp").gameObject.SetActive(flag2 && flag);
            }
            else
            {
                base.view.transform.Find("ActionBtns/PowerUpBtn").GetComponent<Button>().interactable = false;
                base.view.transform.Find("ActionBtns/RarityUpBtn").GetComponent<Button>().interactable = false;
                base.view.transform.Find("ActionBtns/NewAffixBtn").GetComponent<Button>().interactable = false;
                base.view.transform.Find("ActionBtns/NewAffixBtn/PopUp").gameObject.SetActive(false);
            }
            base.view.transform.Find("Info/Figure/InfoMark").gameObject.SetActive(storageItem.IsAffixIdentify && this.unlock);
            base.view.transform.Find("Info/IdentifyBtn").gameObject.SetActive((!this.hideActionBtns || this.showIdentifyBtnOnly) && !storageItem.IsAffixIdentify);
            base.view.transform.Find("Info/IdentifyBtn").GetComponent<DragObject>().Init(this.storageItem);
            base.view.transform.Find("Info/Content/Equipment/Desc").GetComponent<Text>().text = this.storageItem.GetDescription();
            base.view.transform.Find("Info/Content").gameObject.SetActive(true);
            AvatarDataItem item2 = Singleton<AvatarModule>.Instance.TryGetAvatarByID(this.storageItem.avatarID);
            base.view.transform.Find("Info/Content/BtnInEquip").gameObject.SetActive(item2 != null);
            if (item2 != null)
            {
                base.view.transform.Find("Info/Content/BtnInEquip/EquipChara").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(item2.IconPath);
            }
        }

        private void SetupStigmataSkills()
        {
            StigmataDataItem storageItem = this.storageItem as StigmataDataItem;
            this._stigmataTabManager.Clear();
            this.SetupNaturalSkillsTab(storageItem);
            this.SetupAffixSkillsTab(storageItem);
            this.SetupSuitSkillsTab(storageItem);
            List<string> keys = this._stigmataTabManager.GetKeys();
            if (keys.Count > 0)
            {
                this._stigmataTabManager.ShowTab(keys[0]);
            }
        }

        private void SetupSuitSkillsTab(StigmataDataItem stigmataData)
        {
            SortedDictionary<int, EquipSkillDataItem> allSetSkills = stigmataData.GetAllSetSkills();
            if (allSetSkills.Count > 0)
            {
                base.view.transform.Find("Skills/Info/SuitSkills/Content").GetComponent<MonoStigmataSetSkillPanel>().SetupView(stigmataData, allSetSkills);
                this._stigmataTabManager.SetTab("S_Suit_Skill_Tab", base.view.transform.Find("Skills/TabBtns/TabBtn_2").GetComponent<Button>(), base.view.transform.Find("Skills/Info/SuitSkills").gameObject);
                base.view.transform.Find("Skills/TabBtns/TabBtn_2").gameObject.SetActive(true);
            }
            else
            {
                base.view.transform.Find("Skills/TabBtns/TabBtn_2").gameObject.SetActive(false);
                base.view.transform.Find("Skills/Info/SuitSkills").gameObject.SetActive(false);
            }
        }

        protected override bool SetupView()
        {
            Transform transform;
            base.view.transform.Find("ActionBtns/PowerUpBtn").GetComponent<Button>().interactable = this.storageItem.level < this.storageItem.GetMaxLevel();
            base.view.transform.Find("ActionBtns/RarityUpBtn").GetComponent<Button>().interactable = this.storageItem.GetEvoStorageItem() != null;
            this.SetupInfoPanel();
            this.SetupAttributesPanel();
            this.SetupSkillsPanel();
            this.SetupLvInfoPanel();
            if (this.storageItem is StigmataDataItem)
            {
                transform = base.view.transform.Find("Info/Content/LockButton");
            }
            else
            {
                transform = base.view.transform.Find("Info/Info/Content/Equipment/LockButton");
            }
            transform.gameObject.SetActive(!this.hideActionBtns);
            foreach (LetterSpacing spacing in base.view.transform.GetComponentsInChildren<LetterSpacing>())
            {
                if (spacing.autoFixLine)
                {
                    spacing.AccommodateText();
                }
            }
            base.view.transform.Find("ActionBtns").gameObject.SetActive(!this.hideActionBtns);
            return false;
        }

        private void SetupWeapon()
        {
            string prefabPath = MiscData.Config.PrefabPath.WeaponBaseTypeIcon[this.storageItem.GetBaseType()];
            base.view.transform.Find("Info/Info/Title/Equipment/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
            base.view.transform.Find("Info/Info/Title/Equipment/Name").GetComponent<Text>().text = this.storageItem.GetDisplayTitle();
            base.view.transform.Find("Info/Info/Content/Equipment/Cost/Num").GetComponent<Text>().text = this.storageItem.GetCost().ToString();
            base.view.transform.Find("Info/Info/Content/Equipment/Star/EquipStar").GetComponent<MonoEquipSubStar>().SetupView(this.storageItem.rarity, this.storageItem.GetMaxRarity());
            base.view.transform.Find("Info/Info/Content/Equipment/Star/EquipSubStar").GetComponent<MonoEquipSubStar>().SetupView(this.storageItem.GetSubRarity(), this.storageItem.GetMaxSubRarity() - 1);
            base.view.transform.Find("Info/Info/Content/Equipment/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(this.storageItem as WeaponDataItem, false, 0);
            this.SetupItemProtectedStatus();
            base.view.transform.Find("Desc/Content/Text").GetComponent<Text>().text = this.storageItem.GetDescription();
            AvatarDataItem item = Singleton<AvatarModule>.Instance.TryGetAvatarByID(this.storageItem.avatarID);
            base.view.transform.Find("Info/Info/Content/Equipment/BtnInEquip").gameObject.SetActive(item != null);
            if (item != null)
            {
                base.view.transform.Find("Info/Info/Content/Equipment/BtnInEquip/EquipChara").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(item.IconPath);
            }
        }

        private void SetupWeaponSkills()
        {
            List<EquipSkillDataItem> skills = (this.storageItem as WeaponDataItem).skills;
            base.view.transform.Find("Skills").gameObject.SetActive(skills.Count > 0);
            if (skills.Count > 0)
            {
                base.view.transform.Find("Skills/Info/NaturalSkills/Content").GetComponent<MonoEquipSkillPanel>().SetupView(skills, this.storageItem.level);
            }
        }

        private void ShowStigmataInfo(bool active)
        {
            base.view.transform.Find("Info/Content").gameObject.SetActive(active);
            base.view.transform.Find("Info/CloseBtn").gameObject.SetActive(active);
            base.view.transform.Find("Info/OpenBtn").gameObject.SetActive(!active);
        }
    }
}

