namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class MonoStorageSortButton : MonoBehaviour
    {
        private string _currentTabKey;
        public StorageModule.StorageSortType sortTypeAsc;
        public StorageModule.StorageSortType sortTypeDefault;
        public StorageModule.StorageSortType sortTypeDesc;

        public void OnClick()
        {
            StorageModule.StorageSortType type = Singleton<StorageModule>.Instance.sortTypeMap[this._currentTabKey];
            StorageModule.StorageSortType body = type;
            if (this.sortTypeAsc == type)
            {
                body = this.sortTypeDesc;
            }
            else if (this.sortTypeDesc == type)
            {
                body = this.sortTypeAsc;
            }
            else
            {
                body = this.sortTypeDefault;
            }
            Singleton<StorageModule>.Instance.sortTypeMap[this._currentTabKey] = body;
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetStorageSortType, body));
        }

        public void SetupView(string currentTabKey)
        {
            this._currentTabKey = currentTabKey;
            StorageModule.StorageSortType type = Singleton<StorageModule>.Instance.sortTypeMap[this._currentTabKey];
            Image component = base.transform.GetComponent<Image>();
            bool flag = (this.sortTypeAsc == type) || (this.sortTypeDesc == type);
            component.enabled = flag;
            component.color = !flag ? Color.white : MiscData.GetColor("Yellow");
            base.transform.Find("Text").GetComponent<Text>().color = !flag ? Color.white : MiscData.GetColor("Black");
            base.transform.Find("Order").gameObject.SetActive(flag);
            base.transform.Find("Order/UpImg").gameObject.SetActive(this.sortTypeAsc == type);
            base.transform.Find("Order/DownImg").gameObject.SetActive(this.sortTypeDesc == type);
        }
    }
}

