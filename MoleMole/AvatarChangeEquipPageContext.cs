namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class AvatarChangeEquipPageContext : BasePageContext
    {
        private StorageDataItemBase _selectedItem;
        private List<StorageDataItemBase> _showItemList;
        public readonly AvatarDataItem avatarData;
        public readonly EquipmentSlot slot;
        public readonly StorageDataItemBase storageItem;

        public AvatarChangeEquipPageContext(AvatarDataItem avatarData, StorageDataItemBase storageItem, EquipmentSlot slot)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AvatarChangeEquipPageContext",
                viewPrefabPath = "UI/Menus/Page/AvatarChangeEquipPage"
            };
            base.config = pattern;
            base.showSpaceShip = true;
            this.avatarData = avatarData;
            this.storageItem = storageItem;
            this.slot = slot;
        }

        public override void BackToMainMenuPage()
        {
            UIUtil.SetAvatarTattooVisible(false, this.avatarData);
            base.BackToMainMenuPage();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>(), new UnityAction(this.OnOkBtnClick));
            base.BindViewCallback(base.view.transform.Find("ActionBtns/DetailBtn").GetComponent<Button>(), new UnityAction(this.OnDetailBtnClick));
        }

        private void DressItemAlreadyEquiped(bool confirm)
        {
            if (confirm)
            {
                if (this.storageItem == null)
                {
                    if (this.slot != 1)
                    {
                        AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(this._selectedItem.avatarID);
                        EquipmentSlot slot = avatarByID.SearchEquipSlot(this._selectedItem);
                        if (slot > 0)
                        {
                            Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(avatarByID.avatarID, null, slot);
                            Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(this.avatarData.avatarID, this._selectedItem, slot);
                        }
                    }
                }
                else if (this.storageItem is StigmataDataItem)
                {
                    AvatarDataItem item2 = Singleton<AvatarModule>.Instance.GetAvatarByID(this._selectedItem.avatarID);
                    EquipmentSlot slot2 = item2.SearchEquipSlot(this._selectedItem);
                    if (slot2 > 0)
                    {
                        Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(item2.avatarID, null, slot2);
                        Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(this.avatarData.avatarID, null, slot2);
                        Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(this.avatarData.avatarID, this._selectedItem, slot2);
                        Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(item2.avatarID, this.storageItem, slot2);
                    }
                }
                else if (this.storageItem is WeaponDataItem)
                {
                    Singleton<NetworkManager>.Instance.RequestExchangeAvatarWeapon(this.avatarData.avatarID, this._selectedItem.avatarID);
                }
            }
        }

        private bool Filter(StorageDataItemBase item)
        {
            bool flag = false;
            bool flag2 = false;
            switch (this.slot)
            {
                case 1:
                    flag = item.GetType() == typeof(WeaponDataItem);
                    flag2 = item.GetBaseType() == this.avatarData.WeaponBaseTypeList[0];
                    break;

                case 2:
                    flag = item.GetType() == typeof(StigmataDataItem);
                    flag2 = item.GetBaseType() == 1;
                    break;

                case 3:
                    flag = item.GetType() == typeof(StigmataDataItem);
                    flag2 = item.GetBaseType() == 2;
                    break;

                case 4:
                    flag = item.GetType() == typeof(StigmataDataItem);
                    flag2 = item.GetBaseType() == 3;
                    break;
            }
            return (flag && flag2);
        }

        private void FormatText(Text textComp, int number, Color newColor, bool bSign, string prefix, string suffix)
        {
            string str = !bSign ? string.Empty : ((number <= 0) ? string.Empty : "+");
            object[] args = new object[] { prefix, str, number, suffix };
            string str2 = string.Format("{0}{1}{2}{3}", args);
            textComp.text = str2;
            textComp.color = newColor;
        }

        private string GetCurrentTabName()
        {
            if (this.slot == 1)
            {
                return "WeaponTab";
            }
            return "StigmataTab";
        }

        private int GetEffectiveSetSkillNumberIfEquip()
        {
            if (!(this._selectedItem is StigmataDataItem))
            {
                return 0;
            }
            int num = 0;
            foreach (KeyValuePair<EquipmentSlot, StorageDataItemBase> pair in this.avatarData.equipsMap)
            {
                if (((pair.Key != 1) && (pair.Key != this.slot)) && (pair.Value != null))
                {
                    int equipmentSetID = (pair.Value as StigmataDataItem).GetEquipmentSetID();
                    if ((equipmentSetID != 0) && (equipmentSetID == (this._selectedItem as StigmataDataItem).GetEquipmentSetID()))
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        private List<StorageDataItemBase> GetFilterList()
        {
            List<StorageDataItemBase> list = Singleton<StorageModule>.Instance.UserStorageItemList.FindAll(x => this.Filter(x));
            list.Sort(new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToRarityDesc));
            if (this.storageItem != null)
            {
                list.Remove(this.storageItem);
                list.Insert(0, this.storageItem);
            }
            return list;
        }

        private int GetNewCostSum(int newCost)
        {
            int currentCost = this.avatarData.GetCurrentCost();
            int num2 = (this.storageItem != null) ? this.storageItem.GetCost() : 0;
            return ((currentCost - num2) + newCost);
        }

        private bool IsCostOver(StorageDataItemBase from, StorageDataItemBase to, AvatarDataItem avatarData)
        {
            int num = (from != null) ? from.GetCost() : 0;
            int num2 = (to != null) ? to.GetCost() : 0;
            return (((avatarData.GetCurrentCost() - num) + num2) > avatarData.MaxCost);
        }

        private void OnChange(Transform trans, int index)
        {
            StorageDataItemBase item = this._showItemList[index];
            bool isSelected = item == this._selectedItem;
            MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
            bool bShowCostOver = this.GetNewCostSum(item.GetCost()) > this.avatarData.MaxCost;
            bool bUsed = (item.avatarID > 0) && (item.avatarID != this.avatarData.avatarID);
            component.SetupView(item, MonoItemIconButton.SelectMode.SmallWhenUnSelect, isSelected, bShowCostOver, bUsed);
            component.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnItemClick));
            trans.Find("AlreadyEquip").gameObject.SetActive((this.storageItem != null) && (this.storageItem == item));
        }

        public void OnDetailBtnClick()
        {
            if (this._selectedItem != null)
            {
                StorageItemDetailPageContext context = new StorageItemDetailPageContext(this._selectedItem, true, true) {
                    uiEquipOwner = this.avatarData,
                    showIdentifyBtnOnly = true
                };
                Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            }
        }

        private bool OnDressEquipmentRsp(DressEquipmentRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if (this.slot != 1)
                {
                    bool flag = (this.storageItem != null) && (this.storageItem == this._selectedItem);
                    StorageDataItemBase base2 = !flag ? this._selectedItem : null;
                    BaseMonoUIAvatar uIAvatar = UIUtil.GetUIAvatar(this.avatarData.avatarID);
                    if (flag)
                    {
                        if (uIAvatar != null)
                        {
                            uIAvatar.StigmataFadeOut(this.slot);
                        }
                    }
                    else if ((base2 != null) && (uIAvatar != null))
                    {
                        uIAvatar.ChangeStigmata(this.storageItem as StigmataDataItem, this._selectedItem as StigmataDataItem, this.slot);
                    }
                    EquipSetDataItem ownEquipSetData = this.avatarData.GetOwnEquipSetData();
                    if ((ownEquipSetData != null) && (ownEquipSetData.ownNum == 3))
                    {
                        Singleton<WwiseAudioManager>.Instance.Post("VO_M_Con_07_OneSuite", null, null, null);
                    }
                }
                this.BackPage();
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

        private bool OnExchangeAvatarWeaponRsp(ExchangeAvatarWeaponRsp rsp)
        {
            if (rsp.get_retcode() != null)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                this.BackPage();
            }
            return false;
        }

        private void OnItemClick(StorageDataItemBase item, bool selected)
        {
            if (!selected)
            {
                this._selectedItem = item;
                this.UpdateInfo();
                base.view.transform.Find("SelectPanel/Info/Content/ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
            }
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            this.SetupList();
            this.UpdateInfo();
        }

        public void OnOkBtnClick()
        {
            if (this._selectedItem != null)
            {
                StorageDataItemBase dataItem = ((this.storageItem == null) || (this.storageItem != this._selectedItem)) ? this._selectedItem : null;
                if ((this._selectedItem != this.storageItem) && (this._selectedItem.avatarID > 0))
                {
                    string textID = !(this._selectedItem is StigmataDataItem) ? "Menu_Desc_WeaponAlreadyEquip" : "Menu_Desc_StigmataAlreadyEquip";
                    GeneralDialogContext dialogContext = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.DoubleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
                    };
                    object[] replaceParams = new object[] { Singleton<AvatarModule>.Instance.GetAvatarByID(this._selectedItem.avatarID).FullName };
                    dialogContext.desc = LocalizationGeneralLogic.GetText(textID, replaceParams);
                    dialogContext.buttonCallBack = new Action<bool>(this.DressItemAlreadyEquiped);
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
                else
                {
                    Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(this.avatarData.avatarID, dataItem, this.slot);
                }
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 40:
                    return this.OnDressEquipmentRsp(pkt.getData<DressEquipmentRsp>());

                case 0x88:
                    return this.OnExchangeAvatarWeaponRsp(pkt.getData<ExchangeAvatarWeaponRsp>());
            }
            return false;
        }

        private void SetupActionBtns()
        {
            Transform transform = base.view.transform.Find("ActionBtns/OkBtn");
            base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(false);
            string textID = string.Empty;
            bool flag = false;
            if ((this.storageItem != null) && (this.storageItem == this._selectedItem))
            {
                textID = "Menu_Action_Unequip";
                flag = !(this.storageItem is WeaponDataItem);
            }
            else
            {
                textID = "Menu_Action_Equip";
                bool flag2 = this._selectedItem.avatarID > 0;
                bool flag3 = this.IsCostOver(this.storageItem, this._selectedItem, this.avatarData);
                bool flag4 = (this._selectedItem is StigmataDataItem) && !(this._selectedItem as StigmataDataItem).IsAffixIdentify;
                flag = !flag3;
                if (flag4)
                {
                    flag = false;
                }
                base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(!flag);
                string text = string.Empty;
                if (flag2)
                {
                    base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(true);
                    bool flag5 = this.IsCostOver(this._selectedItem, this.storageItem, Singleton<AvatarModule>.Instance.GetAvatarByID(this._selectedItem.avatarID));
                    if (flag3)
                    {
                        text = LocalizationGeneralLogic.GetText("Menu_EquipWarning_CostLack", new object[0]);
                        flag = false;
                    }
                    else if (flag5)
                    {
                        text = LocalizationGeneralLogic.GetText("Menu_EquipWarning_OppositeCostOver", new object[0]);
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                else if (flag4)
                {
                    base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(true);
                    text = LocalizationGeneralLogic.GetText("Menu_AffixTab_NotIdentifyName", new object[0]);
                    flag = false;
                }
                else if (flag3)
                {
                    base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(true);
                    text = LocalizationGeneralLogic.GetText("Menu_EquipWarning_CostLack", new object[0]);
                }
                base.view.transform.Find("ActionBtns/WarnMsg/Text").GetComponent<Text>().text = text;
                base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(!flag);
            }
            transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID, new object[0]);
            transform.GetComponent<Button>().interactable = flag;
        }

        private void SetupAttr(Transform trans, float paramBefore, float paramAfter)
        {
            int num = Mathf.FloorToInt(paramBefore);
            int number = Mathf.FloorToInt(paramAfter);
            trans.gameObject.SetActive((num > 0) || (number > 0));
            if (trans.gameObject.activeSelf)
            {
                Color color;
                UIUtil.TryParseHexString("#00C3FFFF", out color);
                Color white = Color.white;
                Color red = Color.red;
                int num3 = number - num;
                trans.Find("Values/Current").GetComponent<Text>().text = num.ToString();
                Color newColor = (num3 <= 0) ? ((num3 >= 0) ? white : red) : color;
                Text component = trans.Find("Values/New").GetComponent<Text>();
                this.FormatText(component, number, newColor, false, string.Empty, string.Empty);
                Text textComp = trans.Find("Values/Delta").GetComponent<Text>();
                this.FormatText(textComp, num3, newColor, true, "[", "]");
                textComp.gameObject.SetActive(num3 != 0);
            }
        }

        private void SetupEmpty()
        {
            base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView").gameObject.SetActive(false);
            IEnumerator enumerator = base.view.transform.Find("SelectPanel/Info/Content").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    current.gameObject.SetActive(false);
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

        private void SetupList()
        {
            base.view.transform.Find("SelectPanel/Info/Content/ScrollView").GetComponent<MonoGridScroller>().Init(new MoleMole.MonoGridScroller.OnChange(this.OnChange), this._showItemList.Count, null);
        }

        private void SetupProfile()
        {
            int newCost = (this._selectedItem != null) ? this._selectedItem.GetCost() : 0;
            base.view.transform.Find("AvatarDetailProfile").GetComponent<MonoAvatarDetailProfile>().UpdateInfo(this.GetNewCostSum(newCost), this.avatarData.MaxCost, this.avatarData, this._selectedItem);
        }

        private void SetupSkill()
        {
            List<EquipSkillDataItem> skills;
            <SetupSkill>c__AnonStoreyE6 ye = new <SetupSkill>c__AnonStoreyE6();
            int num = 3;
            if (this._selectedItem is WeaponDataItem)
            {
                skills = (this._selectedItem as WeaponDataItem).skills;
                base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/SetSkills").gameObject.SetActive(false);
                base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/AffixSkills").gameObject.SetActive(false);
            }
            else
            {
                <SetupSkill>c__AnonStoreyE5 ye2 = new <SetupSkill>c__AnonStoreyE5();
                StigmataDataItem item = this._selectedItem as StigmataDataItem;
                skills = item.skills;
                ye2.setSkillsTrans = base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/SetSkills");
                SortedDictionary<int, EquipSkillDataItem> allSetSkills = item.GetAllSetSkills();
                if (allSetSkills.Count == 0)
                {
                    ye2.setSkillsTrans.gameObject.SetActive(false);
                }
                else
                {
                    ye2.setSkillsTrans.gameObject.SetActive(true);
                    ye2.setSkillsTrans.Find("Name/Text").GetComponent<Text>().text = item.GetEquipSetName();
                    Transform transform = ye2.setSkillsTrans.Find("Desc");
                    int effectiveSetSkillNumberIfEquip = this.GetEffectiveSetSkillNumberIfEquip();
                    for (int j = 0; j < ye2.setSkillsTrans.Find("Desc").childCount; j++)
                    {
                        int key = j + 2;
                        Transform child = transform.GetChild(j);
                        if (child != null)
                        {
                            EquipSkillDataItem item2;
                            allSetSkills.TryGetValue(key, out item2);
                            if (item2 == null)
                            {
                                child.gameObject.SetActive(false);
                            }
                            else
                            {
                                child.Find("Desc").GetComponent<Text>().text = item2.GetSkillDisplay(1);
                                if (j < effectiveSetSkillNumberIfEquip)
                                {
                                    child.Find("Desc").GetComponent<Text>().color = MiscData.GetColor("TotalWhite");
                                    child.Find("Label").GetComponent<Text>().color = MiscData.GetColor("Blue");
                                    child.Find("Label/EffectStatus").GetComponent<Text>().color = MiscData.GetColor("Blue");
                                    child.Find("Label/EffectStatus").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Label_SkillEffectiveIfEquip", new object[0]);
                                }
                                else
                                {
                                    Color color = MiscData.GetColor("TextGrey");
                                    child.Find("Desc").GetComponent<Text>().color = color;
                                    child.Find("Label").GetComponent<Text>().color = color;
                                    child.Find("Label/EffectStatus").GetComponent<Text>().color = color;
                                    child.Find("Label/EffectStatus").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Label_SkillIneffectIfEquip", new object[0]);
                                }
                            }
                        }
                    }
                    Toggle component = ye2.setSkillsTrans.Find("Btn").GetComponent<Toggle>();
                    this.UpdataSKillBtn(ye2.setSkillsTrans, component.isOn);
                    base.BindViewCallback(component, new UnityAction<bool>(ye2.<>m__120));
                }
                List<StigmataDataItem.AffixSkillData> affixSkillList = item.GetAffixSkillList();
                ye2.affixSkillsTrans = base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/AffixSkills");
                if (affixSkillList.Count < 1)
                {
                    ye2.affixSkillsTrans.gameObject.SetActive(false);
                }
                else
                {
                    ye2.affixSkillsTrans.gameObject.SetActive(true);
                    Transform transform3 = ye2.affixSkillsTrans.Find("Desc");
                    for (int k = 0; k < transform3.childCount; k++)
                    {
                        Transform transform4 = transform3.GetChild(k);
                        transform4.gameObject.SetActive(k < affixSkillList.Count);
                        if (k < affixSkillList.Count)
                        {
                            StigmataDataItem.AffixSkillData data = affixSkillList[k];
                            if (transform4.Find("Label") != null)
                            {
                                transform4.Find("Label").GetComponent<Text>().text = item.GetAffixName();
                            }
                            transform4.Find("Desc").GetComponent<Text>().text = !item.IsAffixIdentify ? string.Empty : data.skill.GetSkillDisplay(1);
                        }
                    }
                    Toggle toggle = ye2.affixSkillsTrans.Find("Btn").GetComponent<Toggle>();
                    this.UpdataSKillBtn(ye2.affixSkillsTrans, toggle.isOn);
                    base.BindViewCallback(toggle, new UnityAction<bool>(ye2.<>m__121));
                }
            }
            ye.setNaturalSkillsTrans = base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/NaturalSkills");
            ye.setNaturalSkillsTrans.gameObject.SetActive(skills.Count > 0);
            string text = LocalizationGeneralLogic.GetText("Menu_Title_WeaponSkill", new object[0]);
            if (this._selectedItem is StigmataDataItem)
            {
                text = LocalizationGeneralLogic.GetText("Menu_Title_StigmataSkill", new object[0]);
            }
            ye.setNaturalSkillsTrans.Find("Name/Label").GetComponent<Text>().text = text;
            for (int i = 1; i <= num; i++)
            {
                Transform trans = base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/NaturalSkills/Desc/Skill_" + i);
                trans.gameObject.SetActive(true);
                if (i > skills.Count)
                {
                    trans.gameObject.SetActive(false);
                }
                else
                {
                    EquipSkillDataItem skillData = skills[i - 1];
                    this.UpdateSkillContent(trans, skillData);
                    Toggle toggle3 = ye.setNaturalSkillsTrans.Find("Btn").GetComponent<Toggle>();
                    toggle3.isOn = true;
                    this.UpdataSKillBtn(ye.setNaturalSkillsTrans, toggle3.isOn);
                    base.BindViewCallback(toggle3, new UnityAction<bool>(ye.<>m__122));
                }
            }
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("AvatarDetailProfile").GetComponent<MonoAvatarDetailProfile>().SetupView(this.avatarData);
            UIUtil.Create3DAvatarByPage(this.avatarData, MiscData.PageInfoKey.AvatarDetailPage, this.GetCurrentTabName());
            if (this.slot == 1)
            {
                base.view.transform.Find("SelectPanel/Info/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectWeapon", new object[0]);
            }
            else
            {
                base.view.transform.Find("SelectPanel/Info/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectStigmata", new object[0]);
            }
            this._showItemList = this.GetFilterList();
            if (this._showItemList.Count > 0)
            {
                this._selectedItem = this._showItemList[0];
                this.SetupList();
                this.UpdateInfo();
            }
            else
            {
                this.SetupEmpty();
            }
            return false;
        }

        private void UpdataSKillBtn(Transform trans, bool isOn)
        {
            trans.Find("Desc").gameObject.SetActive(isOn);
            trans.Find("Btn/CloseImg").gameObject.SetActive(isOn);
            trans.Find("Btn/OpenImg").gameObject.SetActive(!isOn);
        }

        private void UpdateInfo()
        {
            base.view.transform.Find("SelectPanel/Info/Content/SelectedEquip/Name").GetComponent<Text>().text = this._selectedItem.GetDisplayTitle();
            base.view.transform.Find("SelectPanel/Info/Content/SelectedEquip/Lv").GetComponent<Text>().text = "LV." + this._selectedItem.level;
            MonoItemAttributeDiff component = base.view.transform.Find("EquipInfoPanel/AttrDiff").GetComponent<MonoItemAttributeDiff>();
            component.showKeepIcon = true;
            component.SetupView(this.avatarData, this.storageItem, this._selectedItem, new Action<Transform, float, float>(this.SetupAttr));
            this.SetupSkill();
            this.SetupActionBtns();
            this.SetupProfile();
        }

        private void UpdateSkillContent(Transform trans, EquipSkillDataItem skillData)
        {
            trans.Find("Label").GetComponent<Text>().text = skillData.skillName;
            trans.Find("Desc").GetComponent<Text>().text = skillData.GetSkillDisplay(this._selectedItem.level);
        }

        [CompilerGenerated]
        private sealed class <SetupSkill>c__AnonStoreyE5
        {
            internal Transform affixSkillsTrans;
            internal Transform setSkillsTrans;

            internal void <>m__120(bool isOn)
            {
                this.setSkillsTrans.Find("Desc").gameObject.SetActive(isOn);
                this.setSkillsTrans.Find("Btn/CloseImg").gameObject.SetActive(isOn);
                this.setSkillsTrans.Find("Btn/OpenImg").gameObject.SetActive(!isOn);
            }

            internal void <>m__121(bool isOn)
            {
                this.affixSkillsTrans.Find("Desc").gameObject.SetActive(isOn);
                this.affixSkillsTrans.Find("Btn/CloseImg").gameObject.SetActive(isOn);
                this.affixSkillsTrans.Find("Btn/OpenImg").gameObject.SetActive(!isOn);
            }
        }

        [CompilerGenerated]
        private sealed class <SetupSkill>c__AnonStoreyE6
        {
            internal Transform setNaturalSkillsTrans;

            internal void <>m__122(bool isOn)
            {
                this.setNaturalSkillsTrans.Find("Desc").gameObject.SetActive(isOn);
                this.setNaturalSkillsTrans.Find("Btn/CloseImg").gameObject.SetActive(isOn);
                this.setNaturalSkillsTrans.Find("Btn/OpenImg").gameObject.SetActive(!isOn);
            }
        }
    }
}

