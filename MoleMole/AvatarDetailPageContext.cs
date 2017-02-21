namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class AvatarDetailPageContext : BasePageContext
    {
        private AvatarDataItem _avatarBeforeLevelUp;
        private Transform _avatarModel;
        private MonoAvatarRotatePanel _avatarRotatePanel;
        private bool _shouldShowSkillPointExchangeDialog;
        private int _showingSkillId;
        private bool _skillPopUpVisible;
        private TabManager _tabManager;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache9;
        public readonly AvatarDataItem avatarData;
        public readonly string defaultTab;
        public const string LV_UP_TAB = "LvUpTab";
        public const string SKILL_TAB = "SkillTab";
        public const string STIGMATA_TAB = "StigmataTab";
        public const string WEAPON_TAB = "WeaponTab";

        public AvatarDetailPageContext(AvatarDataItem avatarData, string defaultTab = "LvUpTab")
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AvatarDetailPageContext",
                viewPrefabPath = "UI/Menus/Page/AvatarDetailPage"
            };
            base.config = pattern;
            base.showSpaceShip = true;
            this.avatarData = avatarData;
            this.defaultTab = defaultTab;
            this._tabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
        }

        public override void BackPage()
        {
            if (this._tabManager.GetShowingTabKey() == "SkillTab")
            {
                base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().OnBackPage();
            }
            else
            {
                UIUtil.SetAvatarTattooVisible(false, this.avatarData);
                base.BackPage();
            }
        }

        public override void BackToMainMenuPage()
        {
            UIUtil.SetAvatarTattooVisible(false, this.avatarData);
            base.BackToMainMenuPage();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("TabBtns/LvUpTabBtn").GetComponent<Button>(), new UnityAction(this.OnLvUpTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/WeaponTabBtn").GetComponent<Button>(), new UnityAction(this.OnWeaponTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/StigmataTabBtn").GetComponent<Button>(), new UnityAction(this.OnStigmataTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<Button>(), new UnityAction(this.OnSkillTabBtnClick));
        }

        private void CheckLockByMissionTutorial()
        {
            bool flag = UnlockUIDataReaderExtend.UnLockByMission(2) && UnlockUIDataReaderExtend.UnlockByTutorial(2);
            base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<Button>().interactable = flag;
            base.view.transform.Find("TabBtns/SkillTabBtn/PopUp").gameObject.SetActive(this._skillPopUpVisible && flag);
            base.view.transform.Find("TabBtns/SkillTabBtn/Lock").gameObject.SetActive(!flag);
            MonoButtonWwiseEvent component = base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<MonoButtonWwiseEvent>();
            if (component == null)
            {
                component = base.view.transform.Find("TabBtns/SkillTabBtn").gameObject.AddComponent<MonoButtonWwiseEvent>();
            }
            component.eventName = !flag ? "UI_Gen_Select_Negative" : "UI_Click";
        }

        public override void Destroy()
        {
            base.Destroy();
            this.SetMeiHairFade(string.Empty);
        }

        private bool OnAddAvatarExpByMaterialRsp(AddAvatarExpByMaterialRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                UIUtil.UpdateAvatarSkillStatusInLocalData(this.avatarData);
                this.SetupSkillTab();
            }
            return false;
        }

        private bool OnAvatarStarUpRsp(AvatarStarUpRsp rsp)
        {
            if (rsp.get_retcode() != null)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                Singleton<ApplicationManager>.Instance.StartCoroutine(this.PostLevelUpAudioCoroutine(this.avatarData.avatarID));
                Singleton<MainUIManager>.Instance.ShowDialog(new AvatarPromotionDialogContext(this.avatarData), UIType.Any);
                UIUtil.UpdateAvatarSkillStatusInLocalData(this.avatarData);
                this.SetupSkillTab();
            }
            return false;
        }

        private bool OnAvatarSubSkillLevelUpRsp(AvatarSubSkillLevelUpRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                AvatarSkillDataItem selectedSkillData = (this._showingSkillId != 0) ? this.avatarData.GetAvatarSkillBySkillID(this._showingSkillId) : null;
                base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupView(this.avatarData, selectedSkillData);
            }
            return false;
        }

        private bool OnBeforeAvatarLevelUp(AvatarDataItem avatarData)
        {
            this._avatarBeforeLevelUp = new AvatarDataItem(avatarData.avatarID, avatarData.level, avatarData.star);
            return false;
        }

        private bool OnEquipmentPowerupRsp(EquipmentPowerUpRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.SetupLvUpTab();
                this.SetupWeaponTab();
                this.SetupStigmataTab();
            }
            return false;
        }

        private bool OnGetAvatarDataRsp(GetAvatarDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.SetupView();
                if ((rsp.get_avatar_list().Count > 0) && (this._avatarBeforeLevelUp != null))
                {
                    Avatar avatar = rsp.get_avatar_list()[0];
                    if ((this._avatarBeforeLevelUp.avatarID == avatar.get_avatar_id()) && (avatar.get_level() > this._avatarBeforeLevelUp.level))
                    {
                        Singleton<ApplicationManager>.Instance.StartCoroutine(this.PostLevelUpAudioCoroutine((int) avatar.get_avatar_id()));
                        Singleton<MainUIManager>.Instance.ShowDialog(new AvatarLevelUpDialogContext(avatar.get_level(), (uint) this._avatarBeforeLevelUp.level), UIType.Any);
                        UIUtil.UpdateAvatarSkillStatusInLocalData(this.avatarData);
                    }
                    this._avatarBeforeLevelUp = null;
                }
            }
            return false;
        }

        private bool OnGetMainDataRsp(GetMainDataRsp rsp)
        {
            if (rsp.get_skill_pointSpecified() || rsp.get_skill_point_limitSpecified())
            {
                base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupSkillPoint();
            }
            return false;
        }

        private bool OnGetSkillPointExchangeInfoRsp(GetSkillPointExchangeInfoRsp rsp)
        {
            if (this._shouldShowSkillPointExchangeDialog)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new SkillPointExchangeDialogContext(), UIType.Any);
                this._shouldShowSkillPointExchangeDialog = false;
            }
            return false;
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            foreach (LetterSpacing spacing in base.view.transform.GetComponentsInChildren<LetterSpacing>())
            {
                if (spacing.autoFixLine)
                {
                    spacing.AccommodateText();
                }
            }
            if (this._tabManager.GetShowingTabKey() == "StigmataTab")
            {
                UIUtil.SetCameraLookAt(this.avatarData, MiscData.PageInfoKey.AvatarDetailPage, "StigmataTab");
                UIUtil.SetAvatarTattooVisible(true, this.avatarData);
            }
        }

        public void OnLvUpTabBtnClick()
        {
            this._tabManager.ShowTab("LvUpTab");
            UIUtil.SetCameraLookAt(this.avatarData, MiscData.PageInfoKey.AvatarDetailPage, "LvUpTab");
            UIUtil.SetAvatarTattooVisible(false, this.avatarData);
            this._avatarRotatePanel.enableManualRotate = true;
            this._avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "LvUpTab");
            this.SetMeiHairFade("LvUpTab");
            this.CheckLockByMissionTutorial();
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.SelectAvtarSkillIconChange)
            {
                return this.OnSelectedSkillChanged((int) ntf.body);
            }
            if (ntf.type == NotifyTypes.SubSkillStatusCacheUpdate)
            {
                return this.SetupSkillTab();
            }
            if (ntf.type == NotifyTypes.SubSkillStatusCacheUpdate)
            {
                return this.RecordShouldShowSkillPointExchangeDialog();
            }
            return ((ntf.type == NotifyTypes.BeforeAvatarLevelUp) && this.OnBeforeAvatarLevelUp((AvatarDataItem) ntf.body));
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x19:
                    return this.OnGetAvatarDataRsp(pkt.getData<GetAvatarDataRsp>());

                case 30:
                    return this.OnAvatarStarUpRsp(pkt.getData<AvatarStarUpRsp>());

                case 0x24:
                    return this.OnAddAvatarExpByMaterialRsp(pkt.getData<AddAvatarExpByMaterialRsp>());

                case 11:
                    return this.OnGetMainDataRsp(pkt.getData<GetMainDataRsp>());

                case 0x37:
                    return this.OnSkillPointExchangeRsp(pkt.getData<SkillPointExchangeRsp>());

                case 0x35:
                    return this.OnGetSkillPointExchangeInfoRsp(pkt.getData<GetSkillPointExchangeInfoRsp>());

                case 0x33:
                    return this.OnAvatarSubSkillLevelUpRsp(pkt.getData<AvatarSubSkillLevelUpRsp>());

                case 0x20:
                    return this.OnEquipmentPowerupRsp(pkt.getData<EquipmentPowerUpRsp>());
            }
            return false;
        }

        private bool OnSelectedSkillChanged(int newSkillId)
        {
            this._showingSkillId = newSkillId;
            AvatarSkillDataItem selectedSkillData = (this._showingSkillId != 0) ? this.avatarData.GetAvatarSkillBySkillID(this._showingSkillId) : null;
            base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupView(this.avatarData, selectedSkillData);
            this.SetupAvatarSkillPopUp();
            return false;
        }

        private bool OnSkillPointExchangeRsp(SkillPointExchangeRsp rsp)
        {
            GeneralDialogContext context;
            base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupSkillPoint();
            if (rsp.get_retcode() == null)
            {
                context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeSucc", new object[0])
                };
                object[] replaceParams = new object[] { rsp.get_hcoin_cost(), rsp.get_skill_point_get() };
                context.desc = LocalizationGeneralLogic.GetText("Menu_Desc_SkillPtExchangeRes", replaceParams);
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (rsp.get_retcode() == 2)
            {
                context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_GoToRecharge", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_GoToRechargeDesc", new object[0]),
                    okBtnText = LocalizationGeneralLogic.GetText("Menu_GoToRecharge", new object[0]),
                    cancelBtnText = LocalizationGeneralLogic.GetText("Menu_GiveUpRecharge", new object[0])
                };
                if (<>f__am$cache9 == null)
                {
                    <>f__am$cache9 = delegate (bool confirmed) {
                        if (confirmed)
                        {
                            Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext("RechargeTab"), UIType.Page);
                        }
                    };
                }
                context.buttonCallBack = <>f__am$cache9;
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else
            {
                context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeFail", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            return false;
        }

        public void OnSkillTabBtnClick()
        {
            this._tabManager.ShowTab("SkillTab");
            base.view.transform.Find("SkillTab").GetComponent<MonoAvatarDetailSkillTab>().SetupView(this.avatarData, null);
            UIUtil.SetCameraLookAt(this.avatarData, MiscData.PageInfoKey.AvatarDetailPage, "SkillTab");
            UIUtil.SetAvatarTattooVisible(false, this.avatarData);
            this._avatarRotatePanel.enableManualRotate = true;
            this._avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "SkillTab");
            this.SetMeiHairFade("SkillTab");
        }

        public void OnStigmataTabBtnClick()
        {
            this._tabManager.ShowTab("StigmataTab");
            UIUtil.SetCameraLookAt(this.avatarData, MiscData.PageInfoKey.AvatarDetailPage, "StigmataTab");
            UIUtil.SetAvatarTattooVisible(true, this.avatarData);
            this._avatarRotatePanel.enableManualRotate = false;
            this._avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "StigmataTab");
            this.SetMeiHairFade("StigmataTab");
            this.CheckLockByMissionTutorial();
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
            btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.interactable = !active;
            go.SetActive(active);
        }

        public void OnWeaponTabBtnClick()
        {
            this._tabManager.ShowTab("WeaponTab");
            UIUtil.SetCameraLookAt(this.avatarData, MiscData.PageInfoKey.AvatarDetailPage, "WeaponTab");
            UIUtil.SetAvatarTattooVisible(false, this.avatarData);
            this._avatarRotatePanel.enableManualRotate = false;
            this._avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, "WeaponTab");
            this.SetMeiHairFade("WeaponTab");
            this.CheckLockByMissionTutorial();
        }

        [DebuggerHidden]
        private IEnumerator PostLevelUpAudioCoroutine(int id)
        {
            return new <PostLevelUpAudioCoroutine>c__Iterator5F { id = id, <$>id = id, <>f__this = this };
        }

        private void PostLevelUpAudioEvent(int id)
        {
            int num = id / 100;
            string evtName = null;
            switch (num)
            {
                case 1:
                    evtName = "VO_M_Kia_05_LevelUp";
                    break;

                case 2:
                    evtName = "VO_M_Mei_05_LevelUp";
                    break;

                case 3:
                    evtName = "VO_M_Bro_05_LevelUp";
                    break;
            }
            if (evtName != null)
            {
                Singleton<WwiseAudioManager>.Instance.Post(evtName, null, null, null);
            }
        }

        private bool RecordShouldShowSkillPointExchangeDialog()
        {
            this._shouldShowSkillPointExchangeDialog = true;
            return false;
        }

        private void SetMeiHairFade(string tabName = "")
        {
            if (this._avatarModel != null)
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
        }

        private void SetupAvatarRotatePanel(string defaultTab)
        {
            this._avatarRotatePanel = base.view.transform.Find("AvatarRotatePanel").GetComponent<MonoAvatarRotatePanel>();
            this._avatarRotatePanel.SetupView(this.avatarData);
            UIUtil.SetCameraLookAt(this.avatarData, MiscData.PageInfoKey.AvatarDetailPage, defaultTab);
            if ((defaultTab == "LvUpTab") || (defaultTab == "SkillTab"))
            {
                this._avatarRotatePanel.enableManualRotate = true;
            }
            else
            {
                this._avatarRotatePanel.enableManualRotate = false;
            }
            string tabName = defaultTab;
            this._avatarRotatePanel.StartAutoRotateModel(MonoAvatarRotatePanel.AvatarModelAutoRotateType.RotateToOrigin, MiscData.PageInfoKey.AvatarDetailPage, tabName);
        }

        private bool SetupAvatarSkillPopUp()
        {
            this._skillPopUpVisible = false;
            Dictionary<int, SubSkillStatus> subSkillStatusDict = Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict;
            foreach (AvatarSkillDataItem item in this.avatarData.skillDataList)
            {
                if (item.UnLocked)
                {
                    foreach (AvatarSubSkillDataItem item2 in item.avatarSubSkillList)
                    {
                        if (subSkillStatusDict.ContainsKey(item2.subSkillID))
                        {
                            this._skillPopUpVisible = true;
                            break;
                        }
                    }
                }
            }
            base.view.transform.Find("TabBtns/SkillTabBtn/PopUp").gameObject.SetActive(this._skillPopUpVisible && base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<Button>().interactable);
            return false;
        }

        private void SetupLvUpTab()
        {
            GameObject gameObject = base.view.transform.Find("LvUpTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/LvUpTabBtn").GetComponent<Button>();
            component.transform.Find("PopUp").gameObject.SetActive(this.avatarData.CanStarUp);
            this._tabManager.SetTab("LvUpTab", component, gameObject);
            gameObject.GetComponent<MonoAvatarDetailLvUpTab>().SetupView(this.avatarData);
        }

        private void SetupMeiHairFade(string tabName)
        {
            BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
            if (sceneCanvas is MonoMainCanvas)
            {
                this._avatarModel = ((MonoMainCanvas) sceneCanvas).avatar3dModelContext.GetAvatarById(this.avatarData.avatarID);
            }
            else
            {
                this._avatarModel = ((MonoTestUI) sceneCanvas).avatar3dModelContext.GetAvatarById(this.avatarData.avatarID);
            }
            this.SetMeiHairFade(tabName);
        }

        private bool SetupSkillTab()
        {
            GameObject gameObject = base.view.transform.Find("SkillTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/SkillTabBtn").GetComponent<Button>();
            this._tabManager.SetTab("SkillTab", component, gameObject);
            AvatarSkillDataItem selectedSkillData = (this._showingSkillId != 0) ? this.avatarData.GetAvatarSkillBySkillID(this._showingSkillId) : null;
            gameObject.GetComponent<MonoAvatarDetailSkillTab>().SetupView(this.avatarData, selectedSkillData);
            this.SetupAvatarSkillPopUp();
            return false;
        }

        private void SetupStigmataTab()
        {
            GameObject gameObject = base.view.transform.Find("StigmataTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/StigmataTabBtn").GetComponent<Button>();
            this._tabManager.SetTab("StigmataTab", component, gameObject);
            gameObject.GetComponent<MonoAvatarDetailStigmataTab>().SetupView(this.avatarData);
            RectTransform transform = gameObject.transform.Find("Effect") as RectTransform;
            Vector3 anchoredPosition = (Vector3) transform.anchoredPosition;
            anchoredPosition.y = 140f;
            transform.anchoredPosition = anchoredPosition;
        }

        protected override bool SetupView()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string tabName = !string.IsNullOrEmpty(showingTabKey) ? showingTabKey : this.defaultTab;
            base.view.transform.Find("AvatarDetailProfile").GetComponent<MonoAvatarDetailProfile>().SetupView(this.avatarData);
            UIUtil.Create3DAvatarByPage(this.avatarData, MiscData.PageInfoKey.AvatarDetailPage, tabName);
            this._tabManager.Clear();
            this.SetupLvUpTab();
            this.SetupWeaponTab();
            this.SetupStigmataTab();
            this.SetupSkillTab();
            this.SetupAvatarRotatePanel(tabName);
            this.SetupMeiHairFade(tabName);
            base.view.transform.Find("RemotePlayerInfo").gameObject.SetActive(false);
            this._tabManager.ShowTab(tabName);
            this.CheckLockByMissionTutorial();
            return false;
        }

        private void SetupWeaponTab()
        {
            GameObject gameObject = base.view.transform.Find("WeaponTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/WeaponTabBtn").GetComponent<Button>();
            this._tabManager.SetTab("WeaponTab", component, gameObject);
            gameObject.GetComponent<MonoAvatarDetailWeaponTab>().SetupView(this.avatarData);
        }

        [CompilerGenerated]
        private sealed class <PostLevelUpAudioCoroutine>c__Iterator5F : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal int <$>id;
            internal AvatarDetailPageContext <>f__this;
            internal int id;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(1f);
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.PostLevelUpAudioEvent(this.id);
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

