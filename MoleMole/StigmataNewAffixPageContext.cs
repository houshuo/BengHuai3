namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class StigmataNewAffixPageContext : BasePageContext
    {
        private StigmataDataItem _selectedItem;
        private List<StigmataDataItem> _showItemList;
        public readonly StigmataDataItem stigmata;

        public StigmataNewAffixPageContext(StigmataDataItem stigmata)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "StigmataNewAffixPage",
                viewPrefabPath = "UI/Menus/Page/Storage/StigmataNewAffixPage"
            };
            base.config = pattern;
            this.stigmata = stigmata;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>(), new UnityAction(this.OnOkBtnClick));
        }

        private void OnChange(Transform trans, int index)
        {
            StorageDataItemBase item = this._showItemList[index];
            bool isSelected = item == this._selectedItem;
            MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
            component.showProtected = true;
            component.blockSelect = item.isProtected || (item.avatarID > 0);
            component.SetupView(item, MonoItemIconButton.SelectMode.CheckWhenSelect, isSelected, false, item.avatarID > 0);
            component.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnItemClick));
        }

        public bool OnFeedStigmataAffixRsp(FeedStigmataAffixRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                List<StorageDataItemBase> materialList = new List<StorageDataItemBase> {
                    this._selectedItem
                };
                StorageDataItemBase itemDataAfter = this.stigmata.Clone();
                int num = !rsp.get_new_pre_affix_idSpecified() ? 0 : ((int) rsp.get_new_pre_affix_id());
                int num2 = !rsp.get_new_suf_affix_idSpecified() ? 0 : ((int) rsp.get_new_suf_affix_id());
                (itemDataAfter as StigmataDataItem).SetAffixSkill(true, num, num2);
                Singleton<MainUIManager>.Instance.ShowDialog(new EquipPowerUpEffectDialogContext(this.stigmata, itemDataAfter, materialList, EquipPowerUpEffectDialogContext.DialogType.NewAffix, 100), UIType.Any);
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeFail", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        private void OnItemClick(StorageDataItemBase item, bool selected)
        {
            if ((this._selectedItem != null) && (item.uid == this._selectedItem.uid))
            {
                this._selectedItem = null;
            }
            else
            {
                this._selectedItem = item as StigmataDataItem;
            }
            this.UpdateInfo();
            base.view.transform.Find("SelectPanel/List").GetComponent<MonoGridScroller>().RefreshCurrent();
        }

        public override bool OnNotify(Notify ntf)
        {
            return false;
        }

        public void OnOkBtnClick()
        {
            if (this._selectedItem != null)
            {
                if (this._selectedItem.rarity > 3)
                {
                    GeneralDialogContext dialogContext = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.DoubleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0]),
                        desc = LocalizationGeneralLogic.GetText("Menu_Desc_WillConsume3StarItemHint", new object[0]),
                        buttonCallBack = delegate (bool confirmed) {
                            if (confirmed)
                            {
                                Singleton<NetworkManager>.Instance.RequestFeedStigmataAffix(this.stigmata.uid, this._selectedItem.uid);
                            }
                        }
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
                else
                {
                    Singleton<NetworkManager>.Instance.RequestFeedStigmataAffix(this.stigmata.uid, this._selectedItem.uid);
                }
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0xc2) && this.OnFeedStigmataAffixRsp(pkt.getData<FeedStigmataAffixRsp>()));
        }

        private void SetupEmpty()
        {
            base.view.transform.Find("ResourceInfo/Equipment/Content").gameObject.SetActive(false);
            base.view.transform.Find("ResourceInfo/AffixSkills/Skills/Content").gameObject.SetActive(false);
            base.view.transform.Find("SelectPanel/List").gameObject.SetActive(false);
            base.view.transform.Find("SelectPanel/EmptyText").gameObject.SetActive(true);
        }

        private void SetupList()
        {
            base.view.transform.Find("SelectPanel/List").GetComponent<MonoGridScroller>().Init(new MoleMole.MonoGridScroller.OnChange(this.OnChange), this._showItemList.Count, null);
        }

        private void SetupLvInfoPanel()
        {
            base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Lv/CurrentLevelNum").GetComponent<Text>().text = this.stigmata.level.ToString();
            base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Lv/MaxLevelNum").GetComponent<Text>().text = this.stigmata.GetMaxLevel().ToString();
            if (this.stigmata.level == this.stigmata.GetMaxLevel())
            {
                base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Lv/MaxLevelNum").GetComponent<Text>().color = MiscData.GetColor("Yellow");
            }
            base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Exp/NumText").GetComponent<Text>().text = this.stigmata.exp.ToString();
            base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Exp/MaxNumText").GetComponent<Text>().text = this.stigmata.GetMaxExp().ToString();
            base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Exp/TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue((float) this.stigmata.exp, (float) this.stigmata.GetMaxExp(), 0f);
        }

        private void SetupStigmataInfo(StigmataDataItem m_stigmata, Transform trans)
        {
            trans.Find("Equipment/Content/Title/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(m_stigmata.GetSmallIconPath());
            string displayTitle = m_stigmata.GetDisplayTitle();
            if (m_stigmata.IsAffixIdentify)
            {
                string affixName = m_stigmata.GetAffixName();
                if (!string.IsNullOrEmpty(affixName))
                {
                    displayTitle = MiscData.AddColor("Blue", affixName) + " " + displayTitle;
                }
            }
            else
            {
                displayTitle = MiscData.AddColor("WarningRed", m_stigmata.GetAffixName()) + " " + displayTitle;
            }
            trans.Find("Equipment/Content/Title/Name").GetComponent<Text>().text = displayTitle;
            trans.Find("Equipment/Content/Star/EquipStar").GetComponent<MonoEquipSubStar>().SetupView(m_stigmata.rarity, m_stigmata.GetMaxRarity());
            trans.Find("Equipment/Content/Star/EquipSubStar").GetComponent<MonoEquipSubStar>().SetupView(m_stigmata.GetSubRarity(), m_stigmata.GetMaxSubRarity() - 1);
            trans.Find("AffixSkills/Skills/Content").GetComponent<MonoStigmataAffixSkillPanel>().SetupView(m_stigmata, m_stigmata.GetAffixSkillList());
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Figure").GetComponent<MonoStigmataFigure>().SetupView(this.stigmata);
            this.SetupLvInfoPanel();
            this.SetupStigmataInfo(this.stigmata, base.view.transform.Find("OriginInfo"));
            this._showItemList = Singleton<StorageModule>.Instance.GetStigmatasCanUseForNewAffix(this.stigmata);
            this._showItemList.Sort(new Comparison<StigmataDataItem>(StorageDataItemBase.CompareToRarityAsc));
            this._selectedItem = null;
            if (this._showItemList.Count > 0)
            {
                this.SetupList();
                this.UpdateInfo();
            }
            else
            {
                this.SetupEmpty();
            }
            return false;
        }

        private void UpdateInfo()
        {
            base.view.transform.Find("ResourceInfo/Equipment/Content").gameObject.SetActive(this._selectedItem != null);
            base.view.transform.Find("ResourceInfo/AffixSkills/Skills/Content").gameObject.SetActive(this._selectedItem != null);
            if (this._selectedItem != null)
            {
                this.SetupStigmataInfo(this._selectedItem, base.view.transform.Find("ResourceInfo"));
            }
            base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>().interactable = this._selectedItem != null;
        }
    }
}

