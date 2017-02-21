namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class StoragePowerUpPageContext : BasePageContext
    {
        private List<StorageDataItemBase> _resourceList;
        private StorageDataItemBase _storageItemBeforePowerup;
        public const int MAX_SELECT_NUM = 6;
        public readonly StorageDataItemBase storageItem;
        public AvatarDataItem uiEquipOwner;

        public StoragePowerUpPageContext(StorageDataItemBase storageItem)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "StoragePowerUpPageContext",
                viewPrefabPath = "UI/Menus/Page/Storage/WeaponPowerUpPage"
            };
            base.config = pattern;
            if (storageItem is StigmataDataItem)
            {
                base.config.viewPrefabPath = "UI/Menus/Page/Storage/StigmataPowerUpPage";
            }
            this.storageItem = storageItem;
            this._resourceList = new List<StorageDataItemBase>();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>(), new UnityAction(this.OnOkBtnClick));
            base.BindViewCallback(base.view.transform.Find("ActionBtns/ClearBtn").GetComponent<Button>(), new UnityAction(this.OnClearBtnClick));
            if (this.storageItem is StigmataDataItem)
            {
                base.BindViewCallback(base.view.transform.Find("PowerUpInfo/Equipment/LockButton").GetComponent<Button>(), new UnityAction(this.OnLockButtonClick));
            }
            else
            {
                base.BindViewCallback(base.view.transform.Find("Info/Info/Content/Equipment/LockButton").GetComponent<Button>(), new UnityAction(this.OnLockButtonClick));
            }
        }

        private void DoStartPowerup()
        {
            this._storageItemBeforePowerup = this.storageItem.Clone();
            LoadingWheelWidgetContext widget = new LoadingWheelWidgetContext(0x20, null) {
                ignoreMaxWaitTime = true
            };
            Singleton<MainUIManager>.Instance.ShowWidget(widget, UIType.Any);
            Singleton<NetworkManager>.Instance.RequestEquipmentPowerUp(this.storageItem, this._resourceList);
        }

        private string GetStorageShowPageDefaultTab()
        {
            if (this.storageItem is WeaponDataItem)
            {
                return "WeaponTab";
            }
            if (this.storageItem is StigmataDataItem)
            {
                return "StigmataTab";
            }
            if (this.storageItem is AvatarFragmentDataItem)
            {
                return "FragmentTab";
            }
            return "ItemTab";
        }

        public void OnClearBtnClick()
        {
            this._resourceList.Clear();
            this.SetupResourceListView();
            this.OnSetResourceList();
        }

        private bool OnEquipmentPowerUpRsp(EquipmentPowerUpRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new EquipPowerUpEffectDialogContext(this._storageItemBeforePowerup, this.storageItem, this._resourceList, EquipPowerUpEffectDialogContext.DialogType.PowerUp, (int) rsp.get_boost_rate()), UIType.Any);
                this._resourceList.Clear();
                this.SetupResourceListView();
                this.OnSetResourceList();
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            this.SetupView();
        }

        public void OnLockButtonClick()
        {
            Singleton<NetworkManager>.Instance.RequestChangeEquipmentProtectdStatus(this.storageItem);
        }

        public override bool OnNotify(Notify ntf)
        {
            return false;
        }

        public void OnOkBtnClick()
        {
            if (this._resourceList.Count > 0)
            {
                bool flag = false;
                foreach (StorageDataItemBase base2 in this._resourceList)
                {
                    if (((base2 is WeaponDataItem) || (base2 is StigmataDataItem)) && (base2.rarity >= 3))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    GeneralDialogContext dialogContext = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.DoubleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0]),
                        desc = LocalizationGeneralLogic.GetText("Menu_Desc_WillConsume3StarItemHint", new object[0]),
                        buttonCallBack = delegate (bool confirmed) {
                            if (confirmed)
                            {
                                this.DoStartPowerup();
                            }
                        }
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
                else
                {
                    this.DoStartPowerup();
                }
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x20:
                    return this.OnEquipmentPowerUpRsp(pkt.getData<EquipmentPowerUpRsp>());

                case 0x4b:
                    this.SetupItemProtectedStatus();
                    break;
            }
            return false;
        }

        private void OnResourceItemButtonClick(StorageDataItemBase item, bool selelcted = false)
        {
            StorageShowPageContext context = new StorageShowPageContext {
                featureType = StorageShowPageContext.FeatureType.SelectForPowerUp,
                selectedResources = this._resourceList,
                powerUpTarget = this.storageItem,
                defaultTab = this.GetStorageShowPageDefaultTab()
            };
            Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
        }

        private void OnSetResourceList()
        {
            float num;
            float num2;
            UIUtil.CalCulateExpFromItems(out num, out num2, this._resourceList, this.storageItem);
            int lvAfter = UIUtil.CalculateLvWithExp(num2, this.storageItem);
            this.SetupScoinExp(Mathf.RoundToInt(num), Mathf.RoundToInt(num2));
            this.SetupAttributesPanel(lvAfter);
            this.SetupLvInfoPanel(Mathf.RoundToInt(num2), Mathf.RoundToInt((float) lvAfter));
        }

        private void SetupAttr(Transform trans, float paramBefore, float paramAfter)
        {
            int num = Mathf.FloorToInt(paramBefore);
            int num2 = Mathf.FloorToInt(paramAfter);
            trans.gameObject.SetActive((num > 0) || (num2 > 0));
            if (trans.gameObject.activeSelf)
            {
                int num3 = num2 - num;
                Transform transform = (num3 < 0) ? trans.Find("DownNm") : trans.Find("UpNum");
                trans.Find("UpNum").gameObject.SetActive(num3 >= 0);
                trans.Find("DownNm").gameObject.SetActive(num3 < 0);
                transform.Find("Num").GetComponent<Text>().text = num.ToString();
                transform.Find("Diff/DiffNum").GetComponent<Text>().text = num3.ToString();
            }
        }

        private void SetupAttributesPanel(int lvAfter)
        {
            StorageDataItemBase itemAfter = this.storageItem.Clone();
            itemAfter.level = lvAfter;
            MonoItemAttributeDiff component = base.view.transform.Find("Attributes/InfoPanel/BasicStatus").GetComponent<MonoItemAttributeDiff>();
            component.showKeepIcon = this.storageItem.level != lvAfter;
            component.SetupView(this.uiEquipOwner, this.storageItem, itemAfter, new Action<Transform, float, float>(this.SetupAttr));
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

        private void SetupItemProtectedStatus()
        {
            Transform transform;
            if (this.storageItem is StigmataDataItem)
            {
                transform = base.view.transform.Find("PowerUpInfo/Equipment/LockButton");
            }
            else
            {
                transform = base.view.transform.Find("Info/Info/Content/Equipment/LockButton");
            }
            bool isProtected = this.storageItem.isProtected;
            transform.Find("LockedImage").gameObject.SetActive(isProtected);
            transform.Find("UnlockedImage").gameObject.SetActive(!isProtected);
        }

        private void SetupLvInfoPanel(int expGet, int lvAfter)
        {
            base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/CurrentLevelNum").GetComponent<Text>().text = this.storageItem.level.ToString();
            base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/NextLevelNum").GetComponent<Text>().text = lvAfter.ToString();
            base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/Exp/NumText").GetComponent<Text>().text = this.storageItem.exp.ToString();
            base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/Exp/MaxNumText").GetComponent<Text>().text = this.storageItem.GetMaxExp().ToString();
            base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/Exp/TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue((float) this.storageItem.exp, (float) this.storageItem.GetMaxExp(), 0f);
            base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/Exp/TiltSliderNext").gameObject.SetActive(expGet > 0);
            if (expGet > 0)
            {
                float num = (lvAfter <= this.storageItem.level) ? ((float) (this.storageItem.exp + expGet)) : ((float) this.storageItem.GetMaxExp());
                base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/Exp/TiltSliderNext").GetComponent<MonoMaskSlider>().UpdateValue(num, (float) this.storageItem.GetMaxExp(), 0f);
            }
        }

        private void SetupResourceListView()
        {
            for (int i = 1; i <= 6; i++)
            {
                StorageDataItemBase item = (i > this._resourceList.Count) ? null : this._resourceList[i - 1];
                MonoItemIconButton component = base.view.transform.Find("ResourceList/Content/" + i).GetComponent<MonoItemIconButton>();
                component.SetupView(item, MonoItemIconButton.SelectMode.None, false, false, false);
                component.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnResourceItemButtonClick));
            }
            base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>().interactable = this.storageItem.level < this.storageItem.GetMaxLevel();
        }

        private void SetupScoinExp(int scoinNeed, int expGet)
        {
            base.view.transform.Find("PowerUpInfo/ScoinExp/Content/Scoin/Num").GetComponent<Text>().text = scoinNeed.ToString();
            base.view.transform.Find("PowerUpInfo/ScoinExp/Content/Exp/Num").GetComponent<Text>().text = expGet.ToString();
        }

        private void SetupStigmata()
        {
            base.view.transform.Find("PowerUpInfo/Equipment/Title/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab((this.storageItem as StigmataDataItem).GetSmallIconPath());
            base.view.transform.Find("PowerUpInfo/Equipment/Title/Name").GetComponent<Text>().text = this.storageItem.GetDisplayTitle();
            base.view.transform.Find("PowerUpInfo/Equipment/Cost/Num").GetComponent<Text>().text = this.storageItem.GetCost().ToString();
            base.view.transform.Find("PowerUpInfo/Equipment/Star/EquipStar").GetComponent<MonoEquipSubStar>().SetupView(this.storageItem.rarity, this.storageItem.GetMaxRarity());
            base.view.transform.Find("PowerUpInfo/Equipment/Star/EquipSubStar").GetComponent<MonoEquipSubStar>().SetupView(this.storageItem.GetSubRarity(), this.storageItem.GetMaxSubRarity() - 1);
            base.view.transform.Find("Info/Figure").GetComponent<MonoStigmataFigure>().SetupView(this.storageItem as StigmataDataItem);
            this.SetupItemProtectedStatus();
        }

        protected override bool SetupView()
        {
            this.SetupInfoPanel();
            this.SetupResourceListView();
            this.OnSetResourceList();
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
            base.view.transform.Find("Info/Info/Content/Equipment/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(this.storageItem as WeaponDataItem, true, 0);
            this.SetupItemProtectedStatus();
        }
    }
}

