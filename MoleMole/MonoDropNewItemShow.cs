namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoDropNewItemShow : MonoBehaviour
    {
        private StorageDataItemBase _itemData;
        private DropNewItemType _itemType = DropNewItemType.NormalItem;

        private void PostOpenningAudioEvent()
        {
            if (this._itemData is StigmataDataItem)
            {
                Singleton<WwiseAudioManager>.Instance.Post("UI_Item_Tattoo_PTL_Display", null, null, null);
            }
            else if (this._itemData is AvatarCardDataItem)
            {
                Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Large", null, null, null);
            }
            else
            {
                Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Small", null, null, null);
            }
        }

        public void ResetRectMaskForStigmata()
        {
            if (this._itemData is StigmataDataItem)
            {
                base.transform.Find("Item/StigmataIcon").GetComponent<RectMask>().SetGraphicDirty();
            }
        }

        private void SetupEffect()
        {
            if (this._itemType == DropNewItemType.NormalItem)
            {
                base.transform.Find("GachaStart/Green").gameObject.SetActive(false);
                base.transform.Find("GachaStart/Blue").gameObject.SetActive(false);
                base.transform.Find("GachaStart/Purple").gameObject.SetActive(false);
                base.transform.Find("GachaStart/Orange").gameObject.SetActive(false);
                base.transform.Find("GachaStart/" + MiscData.Config.RarityColor[this._itemData.rarity]).gameObject.SetActive(true);
                base.transform.Find("Effect/Green").gameObject.SetActive(false);
                base.transform.Find("Effect/Blue").gameObject.SetActive(false);
                base.transform.Find("Effect/Purple").gameObject.SetActive(false);
                base.transform.Find("Effect/Orange").gameObject.SetActive(false);
                base.transform.Find("Effect/" + MiscData.Config.RarityColor[this._itemData.rarity]).gameObject.SetActive(true);
            }
        }

        private void SetupItemInfo()
        {
            base.transform.Find("Item/TitleRare/Green").gameObject.SetActive(false);
            base.transform.Find("Item/TitleRare/Blue").gameObject.SetActive(false);
            base.transform.Find("Item/TitleRare/Purple").gameObject.SetActive(false);
            base.transform.Find("Item/TitleRare/Orange").gameObject.SetActive(false);
            base.transform.Find("Item/TitleRare/" + MiscData.Config.RarityColor[this._itemData.rarity]).gameObject.SetActive(true);
            if (this._itemType == DropNewItemType.NormalItem)
            {
                base.transform.Find("Item/Title/DescPanel/Desc").GetComponent<Text>().text = this._itemData.GetDisplayTitle();
                base.transform.Find("Item/StigmataIcon").gameObject.SetActive(false);
                base.transform.Find("Item/3dModel").gameObject.SetActive(false);
                base.transform.Find("Item/OtherIcon").gameObject.SetActive(false);
                if (this._itemData is WeaponDataItem)
                {
                    base.transform.Find("Item/3dModel").gameObject.SetActive(true);
                    base.transform.Find("Item/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(this._itemData as WeaponDataItem, false, 0);
                }
                else if (this._itemData is StigmataDataItem)
                {
                    base.transform.Find("Item/StigmataIcon").gameObject.SetActive(true);
                    base.transform.Find("Item/StigmataIcon/Image").GetComponent<MonoStigmataFigure>().SetupView(this._itemData as StigmataDataItem);
                }
                else
                {
                    string prefabPath = !(this._itemData is EndlessToolDataItem) ? this._itemData.GetImagePath() : (this._itemData as EndlessToolDataItem).GetIconPath();
                    base.transform.Find("Item/OtherIcon").gameObject.SetActive(true);
                    base.transform.Find("Item/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
                }
                string text = LocalizationGeneralLogic.GetText("Menu_Material", new object[0]);
                if (this._itemData is StigmataDataItem)
                {
                    text = LocalizationGeneralLogic.GetText("Menu_Stigmata", new object[0]);
                }
                else if (this._itemData is WeaponDataItem)
                {
                    text = LocalizationGeneralLogic.GetText("Menu_Weapon", new object[0]);
                }
                base.transform.Find("Item/Title/DescPanel/Label").GetComponent<Text>().text = text;
                Transform transform = base.transform.Find("Item/Stars");
                if (this._itemData is AvatarFragmentDataItem)
                {
                    transform.gameObject.SetActive(false);
                }
                else
                {
                    transform.gameObject.SetActive(true);
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).gameObject.SetActive(i < this._itemData.rarity);
                    }
                }
                Color color = Miscs.ParseColor(MiscData.Config.DropItemBracketColorList[this._itemData.rarity]);
                base.transform.Find("Item/Title/DescPanel/L").GetComponent<Image>().color = color;
                base.transform.Find("Item/Title/DescPanel/R").GetComponent<Image>().color = color;
            }
            else
            {
                base.transform.Find("Item/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._itemData.GetImagePath());
                base.transform.Find("Item/Title/DescPanel/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_AvatarCard", new object[0]);
                int avatarID = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(this._itemData.ID).avatarID;
                int star = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(avatarID).star;
                base.transform.Find("Item/Stars/AvatarStar/1").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarStarIcons[star]);
                base.transform.Find("Item/Title/DescPanel/Desc").GetComponent<Text>().text = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarID).FullName;
                Color color2 = Miscs.ParseColor(MiscData.Config.DropItemBracketColorList[0]);
                base.transform.Find("Item/Title/DescPanel/L").GetComponent<Image>().color = color2;
                base.transform.Find("Item/Title/DescPanel/R").GetComponent<Image>().color = color2;
            }
        }

        public void SetupView(StorageDataItemBase itemData)
        {
            this._itemData = itemData;
            if (itemData is AvatarCardDataItem)
            {
                this._itemType = DropNewItemType.AvatarCard;
            }
            this.SetupEffect();
            this.SetupItemInfo();
            this.PostOpenningAudioEvent();
        }

        public enum DropNewItemType
        {
            AvatarCard = 2,
            NormalItem = 1
        }
    }
}

