namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoFriendInfo : MonoBehaviour
    {
        private AvatarDataItem _friendAvatarData;
        private FriendBriefDataItem _friendBriefData;
        private FriendOverviewPageContext.FriendTab _friendTab;
        private AcceptCallBack _onAcceptBtnClick;
        private DetailCallBack _onDetailBtnClick;
        private RejectCallBack _onRejectBtnClick;
        private RequestCallBack _onRequestBtnClick;

        private Sprite GetBGSprite()
        {
            switch (((EntityNature) this._friendAvatarData.Attribute))
            {
                case EntityNature.Mechanic:
                    return Miscs.GetSpriteByPrefab(MonoAvatarIcon.bg_path_jixie);

                case EntityNature.Biology:
                    return Miscs.GetSpriteByPrefab(MonoAvatarIcon.bg_path_shengwu);

                case EntityNature.Psycho:
                    return Miscs.GetSpriteByPrefab(MonoAvatarIcon.bg_path_yineng);
            }
            return null;
        }

        public int GetFriendUID()
        {
            return this._friendBriefData.uid;
        }

        public void OnAcceptBtnClick()
        {
            if (this._onAcceptBtnClick != null)
            {
                this._onAcceptBtnClick(this._friendBriefData);
            }
        }

        public void OnDetailBtnClick()
        {
            if (this._onDetailBtnClick != null)
            {
                this._onDetailBtnClick(this._friendBriefData);
            }
        }

        public void OnRejectBtnClick()
        {
            if (this._onRejectBtnClick != null)
            {
                this._onRejectBtnClick(this._friendBriefData);
            }
        }

        public void OnRequestBtnClick()
        {
            if (this._onRequestBtnClick != null)
            {
                this._onRequestBtnClick(this._friendBriefData);
            }
        }

        public void OnTalkBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new ChatDialogContext(this._friendBriefData.uid), UIType.Any);
        }

        public void SetupView(FriendBriefDataItem friendBriefData, FriendOverviewPageContext.FriendTab friendTab, RequestCallBack onRequest = null, AcceptCallBack onAccept = null, RejectCallBack onReject = null, DetailCallBack onDetailBtnClick = null)
        {
            this._friendBriefData = friendBriefData;
            this._friendTab = friendTab;
            this._onRequestBtnClick = onRequest;
            this._onAcceptBtnClick = onAccept;
            this._onRejectBtnClick = onReject;
            this._onDetailBtnClick = onDetailBtnClick;
            this._friendAvatarData = new AvatarDataItem(friendBriefData.showAvatarID, 1, 0);
            base.transform.Find("AvatarImage/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._friendAvatarData.IconPath);
            base.transform.Find("AvatarImage/BGColor").GetComponent<Image>().sprite = this.GetBGSprite();
            base.transform.Find("Nickname").GetComponent<Text>().text = friendBriefData.nickName;
            base.transform.Find("Lv/Num").GetComponent<Text>().text = friendBriefData.level.ToString();
            base.transform.Find("AvatarInfo/Combat/Num").GetComponent<Text>().text = friendBriefData.avatarCombat.ToString();
            base.transform.Find("AvatarInfo/AvatarStar/Star").GetComponent<MonoAvatarStar>().SetupView(friendBriefData.avatarStar);
            base.transform.Find("AvatarImage/NewMark").gameObject.SetActive((friendTab == FriendOverviewPageContext.FriendTab.FriendListTab) && !Singleton<FriendModule>.Instance.IsOldFriend(friendBriefData.uid));
            bool flag = this._friendTab == FriendOverviewPageContext.FriendTab.AddFriendTab;
            bool flag2 = this._friendTab == FriendOverviewPageContext.FriendTab.RequestListTab;
            bool flag3 = this._friendTab == FriendOverviewPageContext.FriendTab.FriendListTab;
            bool flag4 = (flag || flag2) || flag3;
            base.transform.Find("ActionBtns/TalkBtn").gameObject.SetActive(false);
            base.transform.Find("ActionBtns/AddFriendBtn").gameObject.SetActive(false);
            base.transform.Find("ActionBtns/ReplyBtns").gameObject.SetActive(false);
            base.transform.Find("ActionBtns/TalkBtn").gameObject.SetActive(flag3);
            base.transform.Find("ActionBtns/AddFriendBtn").gameObject.SetActive(flag);
            base.transform.Find("ActionBtns/ReplyBtns").gameObject.SetActive(flag2);
            base.transform.Find("ActionBtns").gameObject.SetActive(flag4);
        }
    }
}

