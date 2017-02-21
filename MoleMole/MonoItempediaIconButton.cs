namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoItempediaIconButton : MonoBehaviour
    {
        private ClickCallBack _clickCallBack;
        public ItempediaDataAdapter _item;
        public Color inactiveColor;

        public void OnClick()
        {
            if (this._clickCallBack != null)
            {
                this._clickCallBack(this._item);
            }
        }

        public void SetClickCallback(ClickCallBack callback)
        {
            this._clickCallBack = callback;
        }

        private void SetItemDefaultColor()
        {
            base.transform.Find("BG/Unselected/FrameComItem").GetComponent<Image>().material = null;
            base.transform.Find("BG/Unselected/FrameComItem").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
            base.transform.Find("ItemIcon").GetComponent<Image>().material = null;
            base.transform.Find("ItemIcon").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
            base.transform.Find("ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[this._item.rarity]);
            base.transform.Find("Star/1").GetComponent<Image>().material = null;
            base.transform.Find("Star/1").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
            base.transform.Find("Star/2").GetComponent<Image>().material = null;
            base.transform.Find("Star/2").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
            base.transform.Find("Star/3").GetComponent<Image>().material = null;
            base.transform.Find("Star/3").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
            base.transform.Find("Star/4").GetComponent<Image>().material = null;
            base.transform.Find("Star/4").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
            base.transform.Find("Star/5").GetComponent<Image>().material = null;
            base.transform.Find("Star/5").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
            base.transform.Find("Star/6").GetComponent<Image>().material = null;
            base.transform.Find("Star/6").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
            base.transform.Find("ItemIcon/Icon").GetComponent<Image>().material = null;
            base.transform.Find("ItemIcon/Icon").GetComponent<Image>().color = MiscData.GetColor("DropItemImageDefaultColor");
        }

        private void SetItemGrey()
        {
            base.transform.Find("BG/Unselected/FrameComItem").GetComponent<Image>().color = MiscData.GetColor("DropItemImageGrey");
            base.transform.Find("ItemIcon").GetComponent<Image>().material = null;
            base.transform.Find("ItemIcon").GetComponent<Image>().color = MiscData.GetColor("DropItemIconGrey");
            base.transform.Find("ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[this._item.rarity]);
            base.transform.Find("Star/1").GetComponent<Image>().color = MiscData.GetColor("DropItemIconGrey");
            base.transform.Find("Star/2").GetComponent<Image>().color = MiscData.GetColor("DropItemIconGrey");
            base.transform.Find("Star/3").GetComponent<Image>().color = MiscData.GetColor("DropItemIconGrey");
            base.transform.Find("Star/4").GetComponent<Image>().color = MiscData.GetColor("DropItemIconGrey");
            base.transform.Find("Star/5").GetComponent<Image>().color = MiscData.GetColor("DropItemIconGrey");
            base.transform.Find("Star/6").GetComponent<Image>().color = MiscData.GetColor("DropItemIconGrey");
            Image component = base.transform.Find("ItemIcon/Icon").GetComponent<Image>();
            if (component.material != component.defaultMaterial)
            {
                component.color = MiscData.GetColor("DropItemIconFullGrey");
            }
            else
            {
                component.color = MiscData.GetColor("DropItemIconGrey");
            }
        }

        private void SetupCostView(bool bShowCostOver)
        {
            base.transform.Find("CostOver").gameObject.SetActive(bShowCostOver);
        }

        private void SetupRarityView()
        {
            string hexString = MiscData.Config.ItemRarityColorList[this._item.rarity];
            base.transform.Find("ItemIcon").GetComponent<Image>().color = Miscs.ParseColor(hexString);
            base.transform.Find("Star").gameObject.SetActive(true);
            base.transform.Find("Star").GetComponent<MonoItemIconStar>().SetupView(this._item.rarity, this._item.maxRarity);
        }

        private void SetupStigmataTypeIcon()
        {
            base.transform.Find("StigmataType").gameObject.SetActive(false);
        }

        private void SetupUsedView(bool bUsed)
        {
            base.transform.Find("Used").gameObject.SetActive(bUsed);
        }

        public void SetupView(ItempediaDataAdapter item, bool active)
        {
            this._item = item;
            base.transform.Find("SelectedMark").gameObject.SetActive(false);
            base.transform.Find("ProtectedMark").gameObject.SetActive(false);
            base.transform.Find("NotEnough").gameObject.SetActive(false);
            base.transform.Find("Star").gameObject.SetActive(false);
            base.transform.Find("StigmataType").gameObject.SetActive(false);
            base.transform.Find("UnidentifyText").gameObject.SetActive(false);
            base.transform.Find("BG/Unselected").gameObject.SetActive(true);
            base.transform.Find("BG/Selected").gameObject.SetActive(false);
            base.transform.Find("Text").gameObject.SetActive(false);
            if (this._item == null)
            {
                base.transform.Find("ItemIcon").gameObject.SetActive(false);
            }
            else
            {
                base.transform.Find("ItemIcon").gameObject.SetActive(true);
                Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(item.iconPath);
                base.transform.Find("ItemIcon/Icon").GetComponent<Image>().sprite = spriteByPrefab;
                base.transform.Find("ItemIcon").GetComponent<Image>().color = Color.white;
                base.transform.Find("ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[item.rarity]);
                this.SetupRarityView();
                this.SetupStigmataTypeIcon();
                if (!active)
                {
                    this.SetItemGrey();
                }
                else
                {
                    this.SetItemDefaultColor();
                }
            }
        }

        public delegate void ClickCallBack(ItempediaDataAdapter item);
    }
}

