namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MonoAvatarStigmataSlot : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        private int _index;
        private bool _isRemoteAvatar;
        private EquipmentSlot _slot;
        private StigmataDataItem _stigmataData;

        private bool Filter(StorageDataItemBase item)
        {
            bool flag = false;
            bool flag2 = false;
            switch (this._slot)
            {
                case 1:
                    flag = item.GetType() == typeof(WeaponDataItem);
                    flag2 = item.GetBaseType() == this._avatarData.WeaponBaseTypeList[0];
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

        private List<StorageDataItemBase> GetFilterList()
        {
            List<StorageDataItemBase> list = Singleton<StorageModule>.Instance.UserStorageItemList.FindAll(x => this.Filter(x));
            list.Sort(new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToRarityDesc));
            return list;
        }

        public void OnChangeBtnCallBack()
        {
            if (!this._isRemoteAvatar)
            {
                if (this.GetFilterList().Count > 0)
                {
                    AvatarChangeEquipPageContext context = new AvatarChangeEquipPageContext(this._avatarData, this._stigmataData, this._slot);
                    Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
                }
                else
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_NoSuitableStigmata", new object[0]), 2f), UIType.Any);
                }
            }
        }

        public void OnContentClick(BaseEventData data = null)
        {
            if (this._isRemoteAvatar)
            {
                if (this._stigmataData != null)
                {
                    Singleton<MainUIManager>.Instance.ShowPage(new StorageItemDetailPageContext(this._stigmataData, true, true), UIType.Page);
                }
            }
            else if (this._stigmataData == null)
            {
                this.OnChangeBtnCallBack();
            }
            else
            {
                StorageItemDetailPageContext context = new StorageItemDetailPageContext(this._stigmataData, false, true) {
                    uiEquipOwner = this._avatarData
                };
                Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            }
        }

        private void SetStigmataImage(RectTransform imageTrans, StigmataDataItem stigmata)
        {
            imageTrans.GetComponent<MonoStigmataFigure>().SetupView(stigmata);
            imageTrans.transform.Find("PrefContainer").localScale = (Vector3) (Vector3.one * stigmata.GetScale());
            imageTrans.anchoredPosition = new Vector2(stigmata.GetOffesetX(), stigmata.GetOffesetY());
        }

        private void SetupStigmata(Transform trans, StigmataDataItem stigmata)
        {
            this.SetStigmataImage(trans.Find("MaskPanel/Figure").GetComponent<RectTransform>(), stigmata);
            trans.Find("LvText").GetComponent<Text>().text = "LV." + stigmata.level;
            trans.Find("Cost/Num").GetComponent<Text>().text = stigmata.GetCost().ToString();
            trans.Find("Star/EquipStar").GetComponent<MonoEquipSubStar>().SetupView(stigmata.rarity, stigmata.GetMaxRarity());
            trans.Find("Star/EquipSubStar").GetComponent<MonoEquipSubStar>().SetupView(stigmata.GetSubRarity(), stigmata.GetMaxSubRarity() - 1);
        }

        public void SetupView(AvatarDataItem avatarDataItem, EquipmentSlot slot, int index, bool isRemoteAvatar)
        {
            this._avatarData = avatarDataItem;
            this._slot = slot;
            this._index = index;
            this._isRemoteAvatar = isRemoteAvatar;
            this._stigmataData = this._avatarData.GetStigmata(slot);
            Transform trans = base.transform.Find("Content/Stigmata");
            if (this._stigmataData == null)
            {
                trans.gameObject.SetActive(false);
                string textID = "Menu_StigmataSlot_" + this._index;
                base.transform.Find("Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID, new object[0]);
            }
            else
            {
                trans.gameObject.SetActive(true);
                base.transform.Find("Title/Text").GetComponent<Text>().text = this._stigmataData.GetDisplayTitle();
                this.SetupStigmata(trans, this._stigmataData);
            }
            base.transform.Find("ChangeBtn").gameObject.SetActive(!this._isRemoteAvatar);
        }
    }
}

