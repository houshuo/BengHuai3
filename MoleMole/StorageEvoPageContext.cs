namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class StorageEvoPageContext : BasePageContext
    {
        private StorageDataItemBase _beforeEvoItemData;
        private Dictionary<int, EvoItem> _resourceDict;
        private List<EvoItem> _resourceList;
        [CompilerGenerated]
        private static Func<EvoItem, bool> <>f__am$cache6;
        public const int MAX_SELECT_NUM = 5;
        public readonly StorageDataItemBase storageItem;
        public readonly StorageDataItemBase targetItem;
        public AvatarDataItem uiEquipOwner;

        public StorageEvoPageContext(StorageDataItemBase storageItem)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "StorageItemDetailPageContext",
                viewPrefabPath = "UI/Menus/Page/Storage/WeaponEvoPage"
            };
            base.config = pattern;
            if (storageItem is StigmataDataItem)
            {
                base.config.viewPrefabPath = "UI/Menus/Page/Storage/StigmataEvoPage";
            }
            this.storageItem = storageItem;
            this.targetItem = storageItem.GetEvoStorageItem();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>(), new UnityAction(this.OnOkBtnClick));
        }

        private void DoRequestEvo(List<StorageDataItemBase> resourceList)
        {
            this._beforeEvoItemData = this.storageItem.Clone();
            Singleton<NetworkManager>.Instance.RequestEquipmentEvo(resourceList, this.storageItem);
        }

        private void GetEvoResourceList(out List<EvoItem> list, out Dictionary<int, EvoItem> dict)
        {
            list = new List<EvoItem>();
            dict = new Dictionary<int, EvoItem>();
            HashSet<int> set = new HashSet<int>();
            foreach (KeyValuePair<int, int> pair in this.storageItem.GetEvoMaterial())
            {
                int key = pair.Key;
                int number = pair.Value;
                EvoItem item = new EvoItem();
                List<StorageDataItemBase> list2 = Singleton<StorageModule>.Instance.TryGetStorageDataItemByMetaId(key, number);
                if (list2.Count > 0)
                {
                    list2.Sort(new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToLevelAsc));
                    item.item = list2[0].Clone();
                    if (item.item is WeaponDataItem)
                    {
                        bool flag = false;
                        foreach (StorageDataItemBase base2 in list2)
                        {
                            if ((((base2.uid != this.storageItem.uid) && (Singleton<AvatarModule>.Instance.TryGetAvatarByID(base2.avatarID) == null)) && !base2.isProtected) && !set.Contains(base2.uid))
                            {
                                flag = true;
                                item.item = base2.Clone();
                                set.Add(base2.uid);
                                break;
                            }
                        }
                        item.enough = flag;
                        if (!flag)
                        {
                            item.item = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(key, 1);
                        }
                        item.item.number = 1;
                    }
                    else
                    {
                        item.enough = true;
                        item.item.number = number;
                    }
                }
                else
                {
                    item.item = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(key, 1);
                    item.enough = false;
                    item.item.number = number;
                }
                list.Add(item);
                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, item);
                }
            }
        }

        private bool IsAllResoursesEnough()
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = x => x.enough;
            }
            return Enumerable.All<EvoItem>(this._resourceList, <>f__am$cache6);
        }

        private bool IsAvatarCostEnough()
        {
            if (AvatarMetaDataReader.GetAvatarMetaDataByKey(this.storageItem.avatarID) == null)
            {
                return true;
            }
            AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(this.storageItem.avatarID);
            return (avatarByID.MaxCost >= ((avatarByID.GetCurrentCost() + this.targetItem.GetCost()) - this.storageItem.GetCost()));
        }

        private bool OnEquipmenEvoRsp(EquipmentEvoRsp rsp)
        {
            System.Type type;
            StorageDataItemBase base2;
            if (rsp.get_retcode() != null)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                goto Label_0129;
            }
            List<StorageDataItemBase> materialList = new List<StorageDataItemBase>();
            foreach (EvoItem item in this._resourceList)
            {
                materialList.Add(item.item);
            }
            EquipmentType type2 = rsp.get_new_item().get_type();
            if (type2 != 3)
            {
                if (type2 != 4)
                {
                    return false;
                }
            }
            else
            {
                type = typeof(WeaponDataItem);
                goto Label_0097;
            }
            type = typeof(StigmataDataItem);
        Label_0097:
            base2 = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(type, (int) rsp.get_new_item().get_id_or_unique_id());
            if (base2 == null)
            {
                return false;
            }
            Singleton<MainUIManager>.Instance.ShowDialog(new EquipPowerUpEffectDialogContext(this._beforeEvoItemData, base2, materialList, EquipPowerUpEffectDialogContext.DialogType.Evo, 100), UIType.Any);
        Label_0129:
            return false;
        }

        public override bool OnNotify(Notify ntf)
        {
            return false;
        }

        public void OnOkBtnClick()
        {
            <OnOkBtnClick>c__AnonStorey102 storey = new <OnOkBtnClick>c__AnonStorey102 {
                <>f__this = this,
                list = new List<StorageDataItemBase>()
            };
            bool flag = false;
            foreach (EvoItem item in this._resourceList)
            {
                storey.list.Add(item.item);
                if ((item.item.level > 1) || (item.item.exp > 0))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_ConsumeExpEquipmentHint", new object[0]),
                    buttonCallBack = new Action<bool>(storey.<>m__1A0)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                this.DoRequestEvo(storey.list);
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x26) && this.OnEquipmenEvoRsp(pkt.getData<EquipmentEvoRsp>()));
        }

        private void OnResourceItemButtonClick(StorageDataItemBase item, bool selelcted = false)
        {
            if (this._resourceDict[item.ID].enough)
            {
                UIUtil.ShowItemDetail(item, true, true);
            }
            else if (item is MaterialDataItem)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new DropLinkDialogContext(item as MaterialDataItem, null), UIType.Any);
            }
            else
            {
                UIUtil.ShowItemDetail(item, true, true);
            }
        }

        private void SetupInfoPanels()
        {
            if (this.storageItem is WeaponDataItem)
            {
                this.SetupWeapon(base.view.transform.Find("InfoBefore"), this.storageItem as WeaponDataItem, false);
                this.SetupWeapon(base.view.transform.Find("InfoAfter"), this.targetItem as WeaponDataItem, true);
            }
            else if (this.storageItem is StigmataDataItem)
            {
                this.SetupStigmatasView(this.storageItem);
            }
        }

        private void SetupResourceList()
        {
            for (int i = 1; i <= 5; i++)
            {
                MonoItemIconButton component = base.view.transform.Find("ResourceList/Content/" + i).GetComponent<MonoItemIconButton>();
                if (i <= this._resourceList.Count)
                {
                    component.gameObject.SetActive(true);
                    EvoItem item = this._resourceList[i - 1];
                    component.SetupView(item.item, MonoItemIconButton.SelectMode.ConsumeMaterial, false, false, false);
                    component.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnResourceItemButtonClick));
                    component.transform.Find("NotEnough").gameObject.SetActive(!item.enough);
                }
                else
                {
                    component.gameObject.SetActive(false);
                }
            }
        }

        private void SetupStigmatasView(StorageDataItemBase stigmata)
        {
            StigmataDataItem evoStorageItem = stigmata.GetEvoStorageItem() as StigmataDataItem;
            StigmataDataItem item2 = stigmata as StigmataDataItem;
            int num = (item2.PreAffixSkill != null) ? item2.PreAffixSkill.affixID : 0;
            int num2 = (item2.SufAffixSkill != null) ? item2.SufAffixSkill.affixID : 0;
            evoStorageItem.SetAffixSkill(item2.IsAffixIdentify, num, num2);
            base.view.transform.Find("InfoBefore/Attributes/InfoPanel/BasicStatus").GetComponent<MonoItemAttributeDiff>().SetupView(this.uiEquipOwner, stigmata, evoStorageItem, new Action<Transform, float, float>(this.ShowBeforeAttr));
            base.view.transform.Find("InfoAfter/Attributes/InfoPanel/BasicStatus").GetComponent<MonoItemAttributeDiff>().SetupView(this.uiEquipOwner, stigmata, evoStorageItem, new Action<Transform, float, float>(this.ShowAfterAttr));
            base.view.transform.Find("InfoAfter/Equipment/Title/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(evoStorageItem.GetSmallIconPath());
            base.view.transform.Find("InfoAfter/Equipment/Title/Name").GetComponent<Text>().text = evoStorageItem.GetDisplayTitle();
            base.view.transform.Find("InfoAfter/Equipment/Cost/Num").GetComponent<Text>().text = evoStorageItem.GetCost().ToString();
            base.view.transform.Find("InfoAfter/Equipment/Star/EquipStar").GetComponent<MonoEquipSubStar>().SetupView(evoStorageItem.rarity, evoStorageItem.GetMaxRarity());
            base.view.transform.Find("InfoAfter/Equipment/Star/EquipSubStar").GetComponent<MonoEquipSubStar>().SetupView(evoStorageItem.GetSubRarity(), evoStorageItem.GetMaxSubRarity() - 1);
            base.view.transform.Find("InfoAfter/Figure").GetComponent<MonoStigmataFigure>().SetupView(evoStorageItem);
            base.view.transform.Find("InfoAfter/Equipment/Lv/CurrentLevelNum").GetComponent<Text>().text = evoStorageItem.level.ToString();
            base.view.transform.Find("InfoAfter/Equipment/Lv/MaxLevelNum").GetComponent<Text>().text = evoStorageItem.GetMaxLevel().ToString();
        }

        protected override bool SetupView()
        {
            this.SetupInfoPanels();
            this.GetEvoResourceList(out this._resourceList, out this._resourceDict);
            this.SetupResourceList();
            int coinNeedToUpRarity = this.storageItem.GetCoinNeedToUpRarity();
            base.view.transform.Find("Scoin/Content/Scoin/Num").GetComponent<Text>().text = coinNeedToUpRarity.ToString();
            bool flag = this.storageItem.level == this.storageItem.GetMaxLevel();
            bool flag2 = Singleton<PlayerModule>.Instance.playerData.scoin >= coinNeedToUpRarity;
            bool flag3 = this.IsAllResoursesEnough();
            bool flag4 = this.IsAvatarCostEnough();
            bool flag5 = ((flag && flag2) && flag3) && flag4;
            Text component = base.view.transform.Find("Scoin/Content/Tip/Tips").GetComponent<Text>();
            component.gameObject.SetActive(!flag5);
            if (!flag)
            {
                component.text = LocalizationGeneralLogic.GetText("Menu_Desc_LvNotMax", new object[0]);
            }
            else if (!flag2)
            {
                component.text = LocalizationGeneralLogic.GetText("Menu_Desc_ScoinLack", new object[0]);
            }
            else if (!flag3)
            {
                component.text = LocalizationGeneralLogic.GetText("Menu_Desc_ResourceLack", new object[0]);
            }
            else if (!flag4)
            {
                component.text = LocalizationGeneralLogic.GetText("Menu_CostOver", new object[0]);
            }
            base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>().interactable = flag5;
            return false;
        }

        private void SetupWeapon(Transform trans, WeaponDataItem weapon, bool isEvoWeapon)
        {
            trans.Find("Content/Equipment/Info/Stars/Star/EquipStar").GetComponent<MonoEquipSubStar>().SetupView(weapon.rarity, weapon.GetMaxRarity());
            trans.Find("Content/Equipment/Info/Stars/Star/EquipSubStar").GetComponent<MonoEquipSubStar>().SetupView(weapon.GetSubRarity(), weapon.GetMaxSubRarity() - 1);
            string prefabPath = MiscData.Config.PrefabPath.WeaponBaseTypeIcon[weapon.GetBaseType()];
            trans.Find("Title/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
            trans.Find("Title/Name").GetComponent<Text>().text = weapon.GetDisplayTitle();
            trans.Find("Content/Equipment/Info/Cost/Num").GetComponent<Text>().text = weapon.GetCost().ToString();
            if (isEvoWeapon)
            {
                trans.Find("Content/Equipment/Info/BasicStatus").GetComponent<MonoItemAttributeDiff>().SetupView(this.uiEquipOwner, this.storageItem, this.targetItem, new Action<Transform, float, float>(this.ShowAfterAttr));
            }
            else
            {
                trans.Find("Content/Equipment/Info/BasicStatus").GetComponent<MonoItemAttributeDiff>().SetupView(this.uiEquipOwner, this.storageItem, this.targetItem, new Action<Transform, float, float>(this.ShowBeforeAttr));
            }
            trans.Find("Content/Equipment/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(weapon, true, 0);
            trans.Find("Content/Equipment/Lv/CurrentLevelNum").GetComponent<Text>().text = weapon.level.ToString();
            trans.Find("Content/Equipment/Lv/MaxLevelNum").GetComponent<Text>().text = weapon.GetMaxLevel().ToString();
        }

        private void ShowAfterAttr(Transform trans, float paramBefore, float paramAfter)
        {
            int num = Mathf.FloorToInt(paramBefore);
            int num2 = Mathf.FloorToInt(paramAfter);
            trans.gameObject.SetActive(num != num2);
            if (trans.gameObject.activeSelf)
            {
                int num3 = num2 - num;
                Transform transform = (num3 < 0) ? trans.Find("DownNm") : trans.Find("UpNum");
                trans.Find("UpNum").gameObject.SetActive(num3 >= 0);
                trans.Find("DownNm").gameObject.SetActive(num3 < 0);
                transform.Find("Num").GetComponent<Text>().text = num2.ToString();
                transform.Find("Diff/DiffNum").GetComponent<Text>().text = num3.ToString();
            }
        }

        private void ShowBeforeAttr(Transform trans, float paramBefore, float paramAfter)
        {
            int num = Mathf.FloorToInt(paramBefore);
            int num2 = Mathf.FloorToInt(paramAfter);
            trans.gameObject.SetActive(num != num2);
            if (trans.gameObject.activeSelf)
            {
                trans.Find("UpNum").gameObject.SetActive(true);
                trans.Find("DownNm").gameObject.SetActive(false);
                trans.Find("UpNum/Num").GetComponent<Text>().text = num.ToString();
                trans.Find("UpNum/Diff").gameObject.SetActive(false);
            }
        }

        [CompilerGenerated]
        private sealed class <OnOkBtnClick>c__AnonStorey102
        {
            internal StorageEvoPageContext <>f__this;
            internal List<StorageDataItemBase> list;

            internal void <>m__1A0(bool confirmed)
            {
                if (confirmed)
                {
                    this.<>f__this.DoRequestEvo(this.list);
                }
            }
        }
    }
}

