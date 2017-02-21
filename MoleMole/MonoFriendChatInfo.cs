namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoFriendChatInfo : MonoBehaviour
    {
        private FriendBriefDataItem _friendBriefData;
        private ChangeTalkingFriend _onChangeTalkingFriendClick;
        public bool hasNewMessage;

        public FriendBriefDataItem GetFriendData()
        {
            return this._friendBriefData;
        }

        public void OnChangeTalkingFriendClick()
        {
            this._onChangeTalkingFriendClick(this._friendBriefData);
        }

        public void RefreshNickName()
        {
            Text component = base.transform.Find("FriendBtn/Name").GetComponent<Text>();
            if ((component != null) && (component.text != this._friendBriefData.nickName))
            {
                component.text = this._friendBriefData.nickName;
            }
        }

        public void SetNewMessageTipShow(bool show)
        {
            base.transform.Find("FriendBtn/Image").gameObject.SetActive(show);
            this.hasNewMessage = show;
        }

        public void SetupView(FriendBriefDataItem friendBriefData, bool hasNewMessage, ChangeTalkingFriend onChangeTalkingFriend = null)
        {
            this._friendBriefData = friendBriefData;
            this._onChangeTalkingFriendClick = onChangeTalkingFriend;
            this.hasNewMessage = hasNewMessage;
            base.transform.Find("FriendBtn/Image").gameObject.SetActive(hasNewMessage);
        }
    }
}

