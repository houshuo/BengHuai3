namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class RemoteAvatarDetailPageContext : BasePageContext
    {
        private Transform _avatarModel;
        private MonoAvatarRotatePanel _avatarRotatePanel;
        private Transform _dialogTrans;
        private bool _fromDialog;
        private int _showingSkillId;
        private TabManager _tabManager;
        public const string LV_UP_TAB = "LvUpTab";
        public const string SKILL_TAB = "SkillTab";
        public const string STIGMATA_TAB = "StigmataTab";
        public readonly FriendDetailDataItem userData;
        public const string WEAPON_TAB = "WeaponTab";

        public RemoteAvatarDetailPageContext(FriendDetailDataItem userData, bool fromDialog = false, Transform dialogTrans = null)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "RemoteAvatarDetailPageContext",
                viewPrefabPath = "UI/Menus/Page/AvatarDetailPage"
            };
            base.config = pattern;
            base.showSpaceShip = true;
            this.userData = userData;
            this._tabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
            this._fromDialog = fromDialog;
            this._dialogTrans = dialogTrans;
        }

        public override void BackPage()
        {
            if (this._tabManager.GetShowingTabKey() == "SkillTab")
            {
                base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().OnBackPage();
            }
            else
            {
                base.BackPage();
            }
            if (this._fromDialog && (this._dialogTrans != null))
            {
                this._dialogTrans.gameObject.SetActive(true);
            }
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("TabBtns/LvUpTabBtn").GetComponent<Button>(), new UnityAction(this.OnLvUpTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/WeaponTabBtn").GetComponent<Button>(), new UnityAction(this.OnWeaponTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/StigmataTabBtn").GetComponent<Button>(), new UnityAction(this.OnStigmataTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<Button>(), new UnityAction(this.OnSkillTabBtnClick));
        }

        public override void Destroy()
        {
            base.Destroy();
            this.SetMeiHairFade(string.Empty);
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

        private bool OnDelFriendRsp(DelFriendRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
                string str = Singleton<FriendModule>.Instance.TryGetPlayerNickName((int) rsp.get_target_uid());
                object[] replaceParams = new object[] { str };
                string text = LocalizationGeneralLogic.GetText("Menu_Desc_DeleteFriend", replaceParams);
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(text, 2f), UIType.Any);
            }
            else
            {
                string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput, 2f), UIType.Any);
            }
            return false;
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            string showingTabKey = this._tabManager.GetShowingTabKey();
            UIUtil.Create3DAvatarByPage(this.userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, showingTabKey);
            if (showingTabKey == "StigmataTab")
            {
                this._avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToBack, MiscData.PageInfoKey.AvatarDetailPage, "Default");
            }
        }

        public void OnLvUpTabBtnClick()
        {
            this._tabManager.ShowTab("LvUpTab");
            UIUtil.SetCameraLookAt(this.userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, "LvUpTab");
            this._avatarRotatePanel.enableManualRotate = true;
            this._avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "LvUpTab");
            this.SetMeiHairFade("LvUpTab");
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.SelectAvtarSkillIconChange) && this.OnSelectedSkillChanged((int) ntf.body));
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x43:
                    return this.OnAddFriendRsp(pkt.getData<AddFriendRsp>());

                case 0x45:
                    return this.OnDelFriendRsp(pkt.getData<DelFriendRsp>());
            }
            return false;
        }

        private bool OnSelectedSkillChanged(int newSkillId)
        {
            this._showingSkillId = newSkillId;
            AvatarSkillDataItem selectedSkillData = (this._showingSkillId != 0) ? this.userData.leaderAvatar.GetAvatarSkillBySkillID(this._showingSkillId) : null;
            base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupView(this.userData, selectedSkillData);
            return false;
        }

        public void OnSkillTabBtnClick()
        {
            this._tabManager.ShowTab("SkillTab");
            base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupView(this.userData, null);
            UIUtil.SetCameraLookAt(this.userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, "SkillTab");
            this._avatarRotatePanel.enableManualRotate = true;
            this._avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "SkillTab");
            this.SetMeiHairFade("SkillTab");
        }

        public void OnStigmataTabBtnClick()
        {
            this._tabManager.ShowTab("StigmataTab");
            UIUtil.SetCameraLookAt(this.userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, "StigmataTab");
            this._avatarRotatePanel.enableManualRotate = false;
            this._avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "StigmataTab");
            this.SetMeiHairFade("StigmataTab");
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("TabGreen") : Color.white;
            btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.interactable = !active;
            go.SetActive(active);
        }

        public void OnWeaponTabBtnClick()
        {
            this._tabManager.ShowTab("WeaponTab");
            if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
            {
                base.view.GetComponent<MonoFadeInAnimManager>().Play("WeaponTabFadeIn", false, null);
            }
            UIUtil.SetCameraLookAt(this.userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, "WeaponTab");
            this._avatarRotatePanel.enableManualRotate = false;
            this._avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "WeaponTab");
            this.SetMeiHairFade("WeaponTab");
        }

        private void SetMeiHairFade(string tabName = "")
        {
            foreach (MonoMeiHairFadeAnimation animation in this._avatarModel.GetComponentsInChildren<MonoMeiHairFadeAnimation>())
            {
                if (tabName == "WeaponTab")
                {
                    animation.FadeForWeaponTab();
                }
                else if (tabName == "StigmataTab")
                {
                    animation.FadeForStigmataTab();
                }
                else
                {
                    animation.CancelFade();
                }
            }
        }

        private void SetupAvatarRotatePanel()
        {
            this._avatarRotatePanel = base.view.transform.Find("AvatarRotatePanel").GetComponent<MonoAvatarRotatePanel>();
            this._avatarRotatePanel.SetupView(this.userData.leaderAvatar);
        }

        private void SetupLvUpTab()
        {
            GameObject gameObject = base.view.transform.Find("LvUpTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/LvUpTabBtn").GetComponent<Button>();
            component.transform.Find("Text").GetComponent<LocalizedText>().TextID = "Menu_Detail";
            component.transform.Find("PopUp").gameObject.SetActive(false);
            this._tabManager.SetTab("LvUpTab", component, gameObject);
            gameObject.GetComponent<MonoAvatarDetailLvUpTab>().SetupView(this.userData);
            FriendBriefDataItem item = Singleton<FriendModule>.Instance.TryGetFriendBriefData(this.userData.uid);
            if (item != null)
            {
                base.view.transform.Find("AvatarDetailProfile/Info/CombatNumText").GetComponent<Text>().text = item.avatarCombat.ToString();
            }
        }

        private void SetupMeiHairFade(string tabName)
        {
            BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
            if (sceneCanvas is MonoMainCanvas)
            {
                this._avatarModel = ((MonoMainCanvas) sceneCanvas).avatar3dModelContext.GetAvatarById(this.userData.leaderAvatar.avatarID);
            }
            else
            {
                this._avatarModel = ((MonoTestUI) sceneCanvas).avatar3dModelContext.GetAvatarById(this.userData.leaderAvatar.avatarID);
            }
            this.SetMeiHairFade(tabName);
        }

        private void SetupRemotePlayerView()
        {
            base.view.transform.Find("RemotePlayerInfo/Info/NameText").GetComponent<Text>().text = this.userData.nickName;
            base.view.transform.Find("RemotePlayerInfo/Info/IDText").GetComponent<Text>().text = "ID." + this.userData.uid;
        }

        private void SetupSkillTab()
        {
            GameObject gameObject = base.view.transform.Find("SkillTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<Button>();
            this._tabManager.SetTab("SkillTab", component, gameObject);
            component.transform.Find("PopUp").gameObject.SetActive(false);
            AvatarSkillDataItem selectedSkillData = (this._showingSkillId != 0) ? this.userData.leaderAvatar.GetAvatarSkillBySkillID(this._showingSkillId) : null;
            gameObject.GetComponent<MonoAvatarDetailSkillTab>().SetupView(this.userData, selectedSkillData);
        }

        private void SetupStigmataTab()
        {
            GameObject gameObject = base.view.transform.Find("StigmataTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/StigmataTabBtn").GetComponent<Button>();
            this._tabManager.SetTab("StigmataTab", component, gameObject);
            gameObject.GetComponent<MonoAvatarDetailStigmataTab>().SetupView(this.userData);
            RectTransform transform = gameObject.transform.Find("Effect") as RectTransform;
            Vector3 anchoredPosition = (Vector3) transform.anchoredPosition;
            anchoredPosition.y = 260f;
            transform.anchoredPosition = anchoredPosition;
        }

        protected override bool SetupView()
        {
            if (this._fromDialog && (this._dialogTrans != null))
            {
                this._dialogTrans.gameObject.SetActive(false);
            }
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string tabName = !string.IsNullOrEmpty(showingTabKey) ? showingTabKey : "LvUpTab";
            base.view.transform.Find("AvatarDetailProfile").GetComponent<MonoAvatarDetailProfile>().SetupView(this.userData.leaderAvatar);
            UIUtil.Create3DAvatarByPage(this.userData.leaderAvatar, MiscData.PageInfoKey.AvatarDetailPage, tabName);
            this.SetupLvUpTab();
            this.SetupWeaponTab();
            this.SetupStigmataTab();
            this.SetupSkillTab();
            this.SetupAvatarRotatePanel();
            this.SetupMeiHairFade(tabName);
            base.view.transform.Find("RemotePlayerInfo").gameObject.SetActive(true);
            this.SetupRemotePlayerView();
            this._tabManager.ShowTab(tabName);
            if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
            {
                base.view.GetComponent<MonoFadeInAnimManager>().Play("FriendLvUpTabFadeIn", false, null);
            }
            return false;
        }

        private void SetupWeaponTab()
        {
            GameObject gameObject = base.view.transform.Find("WeaponTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/WeaponTabBtn").GetComponent<Button>();
            this._tabManager.SetTab("WeaponTab", component, gameObject);
            gameObject.GetComponent<MonoAvatarDetailWeaponTab>().SetupView(this.userData);
        }
    }
}

