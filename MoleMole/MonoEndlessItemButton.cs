namespace MoleMole
{
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoEndlessItemButton : MonoBehaviour
    {
        private EndlessPlayerData _currentSelectPlayer;
        private Action<EndlessItem> _itemClickCallback;
        private EndlessItem _itemData;
        private EndlessToolDataItem _itemDataItem;
        private Action _itemUseClickCallback;

        public void OnClick()
        {
            if (this._itemClickCallback != null)
            {
                this._itemClickCallback(this._itemData);
            }
        }

        public void OnUseClick()
        {
            if (!this._itemDataItem.ApplyToSelf && (this._itemDataItem.ToolType != 3))
            {
                base.transform.Find("VerticalLayout/UseBtn/Use").gameObject.SetActive(false);
                base.transform.Find("VerticalLayout/UseBtn/Tip").gameObject.SetActive(true);
            }
            if (this._itemUseClickCallback != null)
            {
                this._itemUseClickCallback();
            }
        }

        public void SetupView(EndlessItem itemData, bool isSelect = false, EndlessPlayerData selectPlayer = null, Action<EndlessItem> clickCallback = null, Action useClickCallback = null)
        {
            this._itemData = itemData;
            this._itemDataItem = new EndlessToolDataItem((int) this._itemData.get_item_id(), (int) this._itemData.get_num());
            this._itemClickCallback = clickCallback;
            this._itemUseClickCallback = useClickCallback;
            base.transform.Find("VerticalLayout/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._itemDataItem.GetIconPath());
            base.transform.Find("VerticalLayout/TopLine/NameRow/NameText").GetComponent<Text>().text = this._itemDataItem.GetDisplayTitle();
            base.transform.Find("VerticalLayout/TopLine/NameRow/NumText").GetComponent<Text>().text = "x" + this._itemDataItem.number;
            base.transform.Find("VerticalLayout/TopLine/Target/Self").gameObject.SetActive(this._itemDataItem.ApplyToSelf);
            base.transform.Find("VerticalLayout/TopLine/Target/Other").gameObject.SetActive(!this._itemDataItem.ApplyToSelf);
            base.transform.Find("VerticalLayout/AbstractText").GetComponent<Text>().text = this._itemDataItem.GetDescription();
            base.transform.Find("VerticalLayout/DescText").GetComponent<Text>().text = this._itemDataItem.GetDescription();
            base.transform.Find("SelectMark").gameObject.SetActive(isSelect);
            base.transform.Find("VerticalLayout/DescText").gameObject.SetActive(false);
            base.transform.Find("VerticalLayout/UseBtn").gameObject.SetActive(isSelect);
            base.transform.Find("VerticalLayout/UseBtn/Use").gameObject.SetActive(isSelect);
            base.transform.Find("VerticalLayout/UseBtn/Tip").gameObject.SetActive(false);
        }
    }
}

