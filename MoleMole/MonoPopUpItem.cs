namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoPopUpItem : MonoBehaviour
    {
        private ClickCallBack _clickCallBack;
        public int itemIndex;
        public string itemName;

        public void OnClick()
        {
            if (this._clickCallBack != null)
            {
                this._clickCallBack(this.itemName, this.itemIndex);
            }
        }

        public void SetupView(string name, int index, ClickCallBack callBack = null)
        {
            this.itemName = name;
            this.itemIndex = index;
            this._clickCallBack = callBack;
            base.transform.Find("Text").GetComponent<Text>().text = this.itemName;
        }

        public delegate void ClickCallBack(string name, int index);
    }
}

