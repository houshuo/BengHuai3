namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class SearchedFriendDetailDialogContext : BaseDialogContext
    {
        public readonly bool hideActionBtns;
        public readonly FriendDetailDataItem playerInfo;

        public SearchedFriendDetailDialogContext(FriendDetailDataItem playerInfo, bool hideActionBtns = false)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "SearchedFriendDetailDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/SearchedFriendInfoShowDialog",
                ignoreNotify = true
            };
            base.config = pattern;
            this.playerInfo = playerInfo;
            this.hideActionBtns = hideActionBtns;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/AddFriendBtn").GetComponent<Button>(), new UnityAction(this.OnAddFriendBtnCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/CheckInfoBtn").GetComponent<Button>(), new UnityAction(this.OnDetailBtnClick));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
        }

        public void OnAddFriendBtnCallBack()
        {
            Singleton<NetworkManager>.Instance.RequestAddFriend(this.playerInfo.uid);
            object[] replaceParams = new object[] { Singleton<FriendModule>.Instance.TryGetPlayerNickName(this.playerInfo.uid) };
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_RequestAddFriend", replaceParams), 2f), UIType.Any);
            this.Close();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        private void OnDetailBtnClick()
        {
            RemoteAvatarDetailPageContext context = new RemoteAvatarDetailPageContext(this.playerInfo, false, null);
            Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            this.Close();
        }

        protected override bool SetupView()
        {
            bool flag = Singleton<FriendModule>.Instance.IsMyFriend(this.playerInfo.uid);
            base.view.transform.Find("Dialog/Content/ActionBtns/AddFriendBtn").gameObject.SetActive(!flag);
            base.view.transform.Find("Dialog/Content/IsFriendMark").gameObject.SetActive(flag);
            base.view.transform.Find("Dialog/Content/ActionBtns/AddFriendBtn").gameObject.SetActive(!Singleton<FriendModule>.Instance.IsMyFriend(this.playerInfo.uid));
            base.view.transform.Find("Dialog/Content/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this.playerInfo.leaderAvatar.IconPath);
            base.view.transform.Find("Dialog/Content/NameText").GetComponent<Text>().text = this.playerInfo.nickName;
            base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = this.playerInfo.Desc;
            return false;
        }
    }
}

