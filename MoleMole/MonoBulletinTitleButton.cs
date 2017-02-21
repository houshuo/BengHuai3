namespace MoleMole
{
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoBulletinTitleButton : MonoBehaviour
    {
        private Bulletin _bulletinDataItem;
        private ShowBulletinById _onShowBulletinById;

        public void OnShowBulletinById()
        {
            if (this._onShowBulletinById != null)
            {
                this._onShowBulletinById(this._bulletinDataItem.get_id());
            }
        }

        public void SetupView(Bulletin bulletinDataItem, bool isSelected, ShowBulletinById onShowBulletinById = null)
        {
            this._bulletinDataItem = bulletinDataItem;
            this._onShowBulletinById = onShowBulletinById;
            base.transform.Find("Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(this._bulletinDataItem.get_title_button());
            base.transform.Find("NewImg").gameObject.SetActive(this._bulletinDataItem.get_mark() == 2);
            base.transform.Find("HotImg").gameObject.SetActive(this._bulletinDataItem.get_mark() == 1);
            base.transform.GetComponent<Button>().interactable = !isSelected;
        }

        private enum MarkType
        {
            None,
            Hot,
            New
        }
    }
}

