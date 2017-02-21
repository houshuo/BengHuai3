namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoHelperFrameRow : MonoBehaviour
    {
        private FriendBriefDataItem _friendData;
        private bool _isFrozen;
        private DateTime _nextAvaliableTime;
        private Action<FriendBriefDataItem> _onFrameClick;
        private Action<FriendBriefDataItem> _onIconClick;

        public int GetAddFP(bool isFriend)
        {
            return (!isFriend ? MiscData.Config.BasicConfig.StrangeAddFriendPoint : MiscData.Config.BasicConfig.FriendAddFriendPoint);
        }

        public void OnFrameClick()
        {
            if (this._onFrameClick != null)
            {
                this._onFrameClick(this._friendData);
            }
        }

        private void OnFrozenTimeOutCallBack()
        {
            this._isFrozen = false;
            base.transform.Find("BG/Select").gameObject.SetActive(false);
            base.transform.Find("BG/Normal").gameObject.SetActive(true);
            base.transform.Find("Grey").gameObject.SetActive(false);
            base.transform.Find("Btns/FrameBtn").gameObject.SetActive(!this._isFrozen);
            base.transform.Find("FriendPoint").gameObject.SetActive(!this._isFrozen);
            base.transform.Find("RemainTimer").gameObject.SetActive(this._isFrozen);
        }

        public void OnIconClick()
        {
            if (this._onIconClick != null)
            {
                this._onIconClick(this._friendData);
            }
        }

        public void SetupView(FriendBriefDataItem friendData, bool selected, Action<FriendBriefDataItem> onFrameClick = null, Action<FriendBriefDataItem> onIconClick = null)
        {
            this._friendData = friendData;
            bool flag = Singleton<FriendModule>.Instance.IsMyFriend(this._friendData.uid);
            this._isFrozen = Singleton<FriendModule>.Instance.isHelperFrozen(friendData.uid);
            if (this._isFrozen)
            {
                this._nextAvaliableTime = Singleton<FriendModule>.Instance.GetHelperNextAvaliableTime(friendData.uid);
            }
            base.transform.Find("BG/Select").gameObject.SetActive(selected);
            base.transform.Find("BG/Normal").gameObject.SetActive(!selected && !this._isFrozen);
            base.transform.Find("Grey").gameObject.SetActive(this._isFrozen);
            base.transform.Find("PlayerName").GetComponent<Text>().text = friendData.nickName;
            base.transform.Find("FriendMark/Friend").gameObject.SetActive(flag);
            base.transform.Find("FriendMark/Strange").gameObject.SetActive(!flag);
            base.transform.Find("AvatarStar").GetComponent<MonoAvatarStar>().SetupView(friendData.avatarStar);
            base.transform.Find("Lv").GetComponent<Text>().text = "LV." + friendData.level;
            base.transform.Find("SelectMark").gameObject.SetActive(selected);
            base.transform.Find("PlayerIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._friendData.AvatarIconPath);
            base.transform.Find("FriendPoint/Value").GetComponent<Text>().text = this.GetAddFP(flag).ToString();
            base.transform.Find("Btns/FrameBtn").gameObject.SetActive(!this._isFrozen);
            base.transform.Find("FriendPoint").gameObject.SetActive(!this._isFrozen);
            base.transform.Find("RemainTimer").gameObject.SetActive(this._isFrozen);
            if (this._isFrozen)
            {
                base.transform.Find("RemainTimer/Time").GetComponent<MonoRemainTimer>().SetTargetTime(this._nextAvaliableTime, null, new Action(this.OnFrozenTimeOutCallBack), false);
            }
            this._onFrameClick = onFrameClick;
            this._onIconClick = onIconClick;
        }
    }
}

