namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    public class AddFriendDialogContext : BaseSequenceDialogContext
    {
        private FriendDetailDataItem _friendDetailData;
        private CanvasTimer _timer;
        private const float TIMER_SPAN = 1f;

        public AddFriendDialogContext(FriendDetailDataItem friendDetailData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AddFriendDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AddFriendDialog"
            };
            base.config = pattern;
            this._friendDetailData = friendDetailData;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnCloseButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/AddBtn").GetComponent<Button>(), new UnityAction(this.OnAddButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Portrait/Button").GetComponent<Button>(), new UnityAction(this.OnPortraitButtonCallBack));
        }

        private void Close()
        {
            this.Destroy();
        }

        public override void Destroy()
        {
            this._timer.Destroy();
            base.Destroy();
        }

        private void OnAddButtonCallBack()
        {
            Singleton<NetworkManager>.Instance.RequestAddFriend(this._friendDetailData.uid);
            base.view.transform.Find("Dialog/Content/AddBtn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_RequestSend", new object[0]);
            base.view.transform.Find("Dialog/Content/AddBtn").GetComponent<Button>().interactable = false;
            this._timer.StartRun(false);
        }

        private bool OnAddFriendRsp(AddFriendRsp rsp)
        {
            int targetUid = (int) rsp.get_target_uid();
            string desc = string.Empty;
            switch (rsp.get_retcode())
            {
                case 0:
                {
                    string str2 = Singleton<FriendModule>.Instance.TryGetPlayerNickName(targetUid);
                    switch (rsp.get_action())
                    {
                        case 1:
                        {
                            object[] replaceParams = new object[] { str2 };
                            desc = LocalizationGeneralLogic.GetText("Menu_Desc_RequestAddFriend", replaceParams);
                            goto Label_0144;
                        }
                        case 2:
                        {
                            object[] objArray1 = new object[] { str2 };
                            desc = LocalizationGeneralLogic.GetText("Menu_Desc_AgreeFriend", objArray1);
                            goto Label_0144;
                        }
                        case 3:
                        {
                            object[] objArray2 = new object[] { str2 };
                            desc = LocalizationGeneralLogic.GetText("Menu_Desc_RejectFriend", objArray2);
                            goto Label_0144;
                        }
                    }
                    break;
                }
                case 1:
                    desc = LocalizationGeneralLogic.GetText("Err_FailToAddFriend", new object[0]);
                    break;

                case 3:
                    desc = LocalizationGeneralLogic.GetText("Err_FriendFull", new object[0]);
                    break;

                case 4:
                    desc = LocalizationGeneralLogic.GetText("Err_TargetFriendFull", new object[0]);
                    break;

                case 5:
                    desc = LocalizationGeneralLogic.GetText("Err_IsSelf", new object[0]);
                    break;

                case 6:
                    desc = LocalizationGeneralLogic.GetText("Err_IsFriend", new object[0]);
                    break;

                case 7:
                    desc = LocalizationGeneralLogic.GetText("Err_AskTooOften", new object[0]);
                    break;
            }
        Label_0144:
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(desc, 2f), UIType.Any);
            return false;
        }

        private void OnCloseButtonCallBack()
        {
            this.Close();
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x43) && this.OnAddFriendRsp(pkt.getData<AddFriendRsp>()));
        }

        private void OnPortraitButtonCallBack()
        {
            FriendDetailDataItem userData = Singleton<FriendModule>.Instance.TryGetFriendDetailData(this._friendDetailData.uid);
            if (userData != null)
            {
                RemoteAvatarDetailPageContext context = new RemoteAvatarDetailPageContext(userData, false, null);
                Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            }
        }

        protected override bool SetupView()
        {
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(1f, 0f);
            this._timer.timeUpCallback = new Action(this.Close);
            this._timer.StopRun();
            Transform transform = base.view.transform.Find("Dialog/Content/");
            AvatarDataItem leaderAvatar = this._friendDetailData.leaderAvatar;
            transform.Find("Portrait/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(leaderAvatar.IconPath);
            Color color = UIUtil.SetupColor(MiscData.Config.AvatarAttributeColorList[leaderAvatar.Attribute]);
            transform.Find("Portrait/BG").GetComponent<Image>().color = color;
            transform.Find("Stars/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(leaderAvatar.star);
            transform.Find("NickName/Text").GetComponent<Text>().text = this._friendDetailData.nickName;
            transform.Find("Level/Text").GetComponent<Text>().text = string.Format("LV.{0}", this._friendDetailData.level);
            transform.Find("CombatValue/Text").GetComponent<Text>().text = Mathf.FloorToInt(leaderAvatar.CombatNum).ToString();
            transform.Find("SelfDesc/Text").GetComponent<Text>().text = this._friendDetailData.Desc;
            return false;
        }
    }
}

