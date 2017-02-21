namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class MonoFriendSortButton : MonoBehaviour
    {
        private string _currentTabKey;
        public FriendModule.FriendSortType sortTypeAsc;
        public FriendModule.FriendSortType sortTypeDefault;
        public FriendModule.FriendSortType sortTypeDesc;

        public void OnClick()
        {
            FriendModule.FriendSortType type = Singleton<FriendModule>.Instance.sortTypeMap[this._currentTabKey];
            FriendModule.FriendSortType body = type;
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
            Singleton<FriendModule>.Instance.sortTypeMap[this._currentTabKey] = body;
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetFriendSortType, body));
        }

        public void SetupView(string currentTabKey)
        {
            this._currentTabKey = currentTabKey;
            FriendModule.FriendSortType type = Singleton<FriendModule>.Instance.sortTypeMap[this._currentTabKey];
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

