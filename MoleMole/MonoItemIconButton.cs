namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoItemIconButton : MonoBehaviour
    {
        private ClickCallBack _clickCallBack;
        private bool _isSelected;
        public StorageDataItemBase _item;
        private Action<StorageDataItemBase> _minusBtnCallBack;
        private int _selectNum;
        public bool blockSelect;
        public SelectMode selectMode;
        public bool showProtected;
        private const float UNSELECTED_SCALE_RATIO = 0.83f;

        public void OnClick()
        {
            if (this._clickCallBack != null)
            {
                this._clickCallBack(this._item, false);
            }
        }

        private void OnDisable()
        {
        }

        public void OnMinusBtnClick()
        {
            this._selectNum--;
            base.transform.Find("SelectedMark/Num").gameObject.SetActive(this._selectNum > 0);
            base.transform.Find("MinusBtn").gameObject.SetActive(this._selectNum > 0);
            if (this._minusBtnCallBack != null)
            {
                this._minusBtnCallBack(this._item);
            }
        }

        public void SetClickCallback(ClickCallBack callback)
        {
            this._clickCallBack = callback;
        }

        public void SetMinusBtnCallBack(Action<StorageDataItemBase> minusBtnCallBack = null)
        {
            this._minusBtnCallBack = minusBtnCallBack;
        }

        private void SetupBlockSelectView()
        {
            base.transform.Find("InteractiveMask").gameObject.SetActive(this.blockSelect);
            base.transform.GetComponent<Button>().interactable = !this.blockSelect;
        }

        private void SetupCostView(bool bShowCostOver)
        {
            base.transform.Find("CostOver").gameObject.SetActive(bShowCostOver);
        }

        private void SetupProtectedView()
        {
            base.transform.Find("ProtectedMark").gameObject.SetActive(this._item.isProtected);
        }

        private void SetupRarityView()
        {
            if (!(this._item is AvatarFragmentDataItem))
            {
                base.transform.Find("Star").gameObject.SetActive(true);
                int rarity = this._item.rarity;
                if (this._item is WeaponDataItem)
                {
                    rarity = (this._item as WeaponDataItem).GetMaxRarity();
                }
                else if (this._item is StigmataDataItem)
                {
                    rarity = (this._item as StigmataDataItem).GetMaxRarity();
                }
                base.transform.Find("Star").GetComponent<MonoItemIconStar>().SetupView(this._item.rarity, rarity);
            }
        }

        private void SetupSelectedView(bool isSelected)
        {
            this._isSelected = isSelected;
            if (this.selectMode == SelectMode.SmallWhenUnSelect)
            {
                base.transform.localScale = !this._isSelected ? ((Vector3) (Vector3.one * 0.83f)) : Vector3.one;
            }
            else
            {
                base.transform.Find("SelectedMark").gameObject.SetActive(this._isSelected);
            }
            base.transform.Find("BG/Selected").gameObject.SetActive(isSelected);
            base.transform.Find("BG/Unselected").gameObject.SetActive(!isSelected);
        }

        private void SetupStigmataAffixView(bool isIdentify)
        {
            base.transform.Find("UnidentifyText").gameObject.SetActive(!isIdentify);
            base.transform.Find("Text").gameObject.SetActive(isIdentify);
            Image component = base.transform.Find("ItemIcon/Icon").GetComponent<Image>();
            if (isIdentify)
            {
                component.material = null;
                component.color = Color.white;
            }
            else
            {
                Material material = Miscs.LoadResource<Material>("Material/ImageMonoColor", BundleType.RESOURCE_FILE);
                component.material = material;
                component.color = MiscData.GetColor("DarkBlue");
            }
            base.transform.Find("QuestionMark").gameObject.SetActive(!isIdentify);
        }

        private void SetupStigmataTypeIcon()
        {
            base.transform.Find("StigmataType").gameObject.SetActive(this._item is StigmataDataItem);
            if (this._item is StigmataDataItem)
            {
                base.transform.Find("StigmataType/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.StigmataTypeIconPath[this._item.GetBaseType()]);
            }
        }

        private void SetupUsedView(bool bUsed)
        {
            base.transform.Find("Used").gameObject.SetActive(bUsed);
        }

        public void SetupView(StorageDataItemBase item, SelectMode selectMode = 0, bool isSelected = false, bool bShowCostOver = false, bool bUsed = false)
        {
            this._item = item;
            this.selectMode = selectMode;
            this._isSelected = isSelected;
            base.transform.Find("SelectedMark").gameObject.SetActive(false);
            base.transform.Find("ProtectedMark").gameObject.SetActive(false);
            base.transform.Find("InteractiveMask").gameObject.SetActive(false);
            base.transform.Find("NotEnough").gameObject.SetActive(false);
            base.transform.Find("Star").gameObject.SetActive(false);
            base.transform.Find("StigmataType").gameObject.SetActive(false);
            base.transform.Find("UnidentifyText").gameObject.SetActive(false);
            base.transform.Find("QuestionMark").gameObject.SetActive(false);
            base.transform.Find("MinusBtn").gameObject.SetActive(false);
            if (this._item == null)
            {
                base.transform.Find("ItemIcon").gameObject.SetActive(false);
                base.transform.Find("Text").gameObject.SetActive(false);
            }
            else
            {
                base.transform.Find("ItemIcon").gameObject.SetActive(true);
                base.transform.Find("Text").gameObject.SetActive(true);
                Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(item.GetIconPath());
                base.transform.Find("ItemIcon/Icon").GetComponent<Image>().sprite = spriteByPrefab;
                base.transform.Find("ItemIcon").GetComponent<Image>().color = Color.white;
                base.transform.Find("ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[item.rarity]);
                if ((this._item is WeaponDataItem) || (this._item is StigmataDataItem))
                {
                    base.transform.Find("Text").GetComponent<Text>().text = "LV." + item.level;
                }
                else if (this._item is MaterialDataItem)
                {
                    string str = "\x00d7" + item.number;
                    if (selectMode == SelectMode.ConsumeMaterial)
                    {
                        StorageDataItemBase base2 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(this._item.ID);
                        int num = (base2 != null) ? base2.number : 0;
                        if (this._item.number > num)
                        {
                            str = MiscData.AddColor("WarningRed", num + " / ") + MiscData.AddColor("TotalBlack", this._item.number.ToString());
                        }
                        else
                        {
                            str = MiscData.AddColor("TotalBlack", num + " / " + this._item.number);
                        }
                    }
                    base.transform.Find("Text").GetComponent<Text>().text = str;
                }
                else if (this._item is AvatarFragmentDataItem)
                {
                    base.transform.Find("Text").GetComponent<Text>().text = "\x00d7" + item.number;
                }
                else
                {
                    base.transform.Find("Text").gameObject.SetActive(false);
                }
                this.SetupRarityView();
                this.SetupStigmataTypeIcon();
                if (selectMode != SelectMode.None)
                {
                    this.SetupSelectedView(isSelected);
                }
                else
                {
                    base.transform.Find("BG/Unselected").gameObject.SetActive(true);
                    base.transform.Find("BG/Selected").gameObject.SetActive(false);
                }
                if (this.showProtected)
                {
                    this.SetupProtectedView();
                }
                this.SetupBlockSelectView();
                this.SetupCostView(bShowCostOver);
                this.SetupUsedView(bUsed);
                base.transform.Find("ItemIcon/Icon").GetComponent<Image>().material = null;
                base.transform.Find("ItemIcon/Icon").GetComponent<Image>().color = MiscData.GetColor("TotalWhite");
                if (this._item is StigmataDataItem)
                {
                    this.SetupStigmataAffixView((this._item as StigmataDataItem).IsAffixIdentify);
                }
            }
        }

        public void ShowSelectedNum(int num)
        {
            this._selectNum = num;
            base.transform.Find("SelectedMark/Image").gameObject.SetActive(false);
            base.transform.Find("SelectedMark/Num").gameObject.SetActive(true);
            base.transform.Find("SelectedMark/Num").GetComponent<Text>().text = "\x00d7" + num;
            base.transform.Find("MinusBtn").gameObject.SetActive((num > 0) && (this._item is MaterialDataItem));
        }

        public delegate void ClickCallBack(StorageDataItemBase item, bool selelcted = false);

        public enum SelectMode
        {
            None,
            CheckWhenSelect,
            SmallWhenUnSelect,
            ConsumeMaterial
        }
    }
}

