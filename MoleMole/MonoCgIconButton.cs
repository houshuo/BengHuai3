namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoCgIconButton : MonoBehaviour
    {
        private ClickCallBack _clickCallBack;
        public CgDataItem _item;

        private bool IsLocked()
        {
            return !Singleton<CGModule>.Instance.IsCGFinished(this._item.cgID);
        }

        public void OnClick()
        {
            if ((this._clickCallBack != null) && !this.IsLocked())
            {
                this._clickCallBack(this._item);
            }
        }

        public void SetClickCallback(ClickCallBack callback)
        {
            this._clickCallBack = callback;
        }

        public void SetupView(CgDataItem item)
        {
            this._item = item;
            bool flag = this.IsLocked();
            base.transform.Find("Lock").gameObject.SetActive(flag);
            base.transform.Find("Image").gameObject.SetActive(!flag);
            if (!string.IsNullOrEmpty(this._item.cgIconPath) && !flag)
            {
                string prefabPath = string.Format("SpriteOutput/CGReplay/{0}", this._item.cgIconPath);
                base.transform.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
            }
        }

        public delegate void ClickCallBack(CgDataItem item);
    }
}

