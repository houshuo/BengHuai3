namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class AvatarOverviewPageContext : BasePageContext
    {
        [CompilerGenerated]
        private static Predicate<AvatarDataItem> <>f__am$cache7;
        [CompilerGenerated]
        private static Predicate<AvatarDataItem> <>f__am$cache8;
        public StageType levelType;
        public int selectedAvatarID;
        private List<AvatarDataItem> showAvatarList;
        public bool showAvatarRemainHP;
        private const string STIGMATA_ICON_EMPTY_PATH = "SpriteOutput/StigmataSmallIcon/Icon_add";
        public int teamEditIndex;
        public PageType type;
        public VentureDataItem ventureData;

        public AvatarOverviewPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AvatarOverviewPageContext",
                viewPrefabPath = "UI/Menus/Page/AvatarOverviewPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            base.showSpaceShip = true;
        }

        private void ApplyTempSaveData()
        {
            PlayerUITempSaveData uiTempSaveData = Singleton<PlayerModule>.Instance.playerData.uiTempSaveData;
            if (uiTempSaveData.lastSelectedAvatarID != 0)
            {
                base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().SetNormalizedPosition(uiTempSaveData.avatarOverviewPageScrollerPos);
            }
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("3dPanel").GetComponent<Button>(), new UnityAction(this.OnAvatar3DPanelClick));
            base.BindViewCallback(base.view.transform.Find("Info/Lock/Right/InfoRow_4/UnlockBtn").GetComponent<Button>(), new UnityAction(this.OnAvatarUnlockBtnClick));
            base.BindViewCallback(base.view.transform.Find("ListPanel/NextBtn").GetComponent<Button>(), new UnityAction(this.OnNextBtnClick));
            base.BindViewCallback(base.view.transform.Find("ListPanel/PrevBtn").GetComponent<Button>(), new UnityAction(this.OnPrevBtnClick));
            base.BindViewCallback(base.view.transform.Find("Info/Unlock/Left/Info_2").GetComponent<Button>(), new UnityAction(this.OnLvInfoClick));
            base.BindViewCallback(base.view.transform.Find("Info/Unlock/Right/Weapon").GetComponent<Button>(), new UnityAction(this.OnWeaponInfoClick));
            base.BindViewCallback(base.view.transform.Find("Info/Unlock/Right/Stigmata").GetComponent<Button>(), new UnityAction(this.OnStigmataInfoClick));
            base.BindViewCallback(base.view.transform.Find("Info/Unlock/Right/Skill").GetComponent<Button>(), new UnityAction(this.OnSkillInfoClick));
            base.BindViewCallback(base.view.transform.Find("Info/Unlock/Left/Info_1").GetComponent<Button>(), new UnityAction(this.OnAvatarInfoClick));
            base.BindViewCallback(base.view.transform.Find("Info/Lock/Left").GetComponent<Button>(), new UnityAction(this.OnAvatarInfoClick));
        }

        private void CheckLockByMissionTutorial()
        {
            bool flag = UnlockUIDataReaderExtend.UnLockByMission(1) && UnlockUIDataReaderExtend.UnlockByTutorial(1);
            base.view.transform.Find("Info/Unlock/Right/Skill").GetComponent<Button>().interactable = flag;
            base.view.transform.Find("Info/Unlock/Right/Skill/Lock").gameObject.SetActive(!flag);
            base.view.transform.Find("Info/Unlock/Right/Skill/SkillPoint/PointNum").gameObject.SetActive(flag);
            base.view.transform.Find("Info/Unlock/Right/Skill/SkillPoint/SkillPtLabel").gameObject.SetActive(flag);
            MonoButtonWwiseEvent component = base.view.transform.Find("Info/Unlock/Right/Skill").GetComponent<MonoButtonWwiseEvent>();
            if (component == null)
            {
                component = base.view.transform.Find("Info/Unlock/Right/Skill").gameObject.AddComponent<MonoButtonWwiseEvent>();
            }
            component.eventName = !flag ? "UI_Gen_Select_Negative" : "UI_Click";
        }

        private int CompareByDefault(AvatarDataItem lemb, AvatarDataItem remb)
        {
            if (!lemb.UnLocked && remb.UnLocked)
            {
                return 1;
            }
            if (!remb.UnLocked && lemb.UnLocked)
            {
                return -1;
            }
            return (!lemb.UnLocked ? this.CompareByFragment(lemb, remb) : this.CompareByStar(lemb, remb));
        }

        private int CompareByFragment(AvatarDataItem lemb, AvatarDataItem remb)
        {
            int num = -(lemb.fragment - remb.fragment);
            if (num != 0)
            {
                return num;
            }
            return this.CompareByID(lemb, remb);
        }

        private int CompareByID(AvatarDataItem lemb, AvatarDataItem remb)
        {
            return (lemb.avatarID - remb.avatarID);
        }

        private int CompareByStar(AvatarDataItem lemb, AvatarDataItem remb)
        {
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(1);
            int index = memberList.IndexOf(lemb.avatarID);
            int num2 = memberList.IndexOf(remb.avatarID);
            if ((index == -1) && (num2 >= 0))
            {
                return 1;
            }
            if ((num2 == -1) && (index >= 0))
            {
                return -1;
            }
            if ((index >= 0) && (num2 >= 0))
            {
                return (index - num2);
            }
            int num3 = -(lemb.star - remb.star);
            if (num3 != 0)
            {
                return num3;
            }
            return this.CompareByID(lemb, remb);
        }

        public void OnAvatar3DPanelClick()
        {
            if (this.type != PageType.GalTouchReplace)
            {
                this.ShowDetailWithTab("LvUpTab");
            }
        }

        public void OnAvatarInfoClick()
        {
            AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(this.selectedAvatarID);
            Singleton<MainUIManager>.Instance.ShowPage(new AvatarIntroPageContext(avatarByID), UIType.Page);
        }

        public bool OnAvatarStarUpRsp(AvatarStarUpRsp rsp)
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
            return false;
        }

        public void OnAvatarUnlockBtnClick()
        {
            Singleton<NetworkManager>.Instance.RequestAvatarStarUp(this.selectedAvatarID);
        }

        private void OnChange(Transform trans, int index)
        {
            bool isSelected = this.showAvatarList[index].avatarID == this.selectedAvatarID;
            AvatarDataItem avatarDataItem = this.showAvatarList[index];
            trans.GetComponent<MonoAvatarIcon>().SetupView(avatarDataItem, isSelected, !this.showAvatarRemainHP ? null : Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(avatarDataItem.avatarID));
        }

        public void OnInTeamBtnClick()
        {
            Singleton<PlayerModule>.Instance.playerData.SetTeamMember(this.levelType, this.teamEditIndex, this.selectedAvatarID);
            Singleton<NetworkManager>.Instance.NotifyUpdateAvatarTeam(this.levelType);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TeamMemberChanged, null));
            Singleton<MainUIManager>.Instance.BackPage();
        }

        public void OnInVentureDispatchBtnClick()
        {
            if (this.teamEditIndex > this.ventureData.selectedAvatarList.Count)
            {
                this.ventureData.selectedAvatarList.Add(this.selectedAvatarID);
            }
            else
            {
                this.ventureData.selectedAvatarList[this.teamEditIndex - 1] = this.selectedAvatarID;
            }
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DispatchAvatarChanged, null));
            Singleton<MainUIManager>.Instance.BackPage();
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            UIUtil.Create3DAvatarByPage(Singleton<AvatarModule>.Instance.GetAvatarByID(this.selectedAvatarID), MiscData.PageInfoKey.AvatarOverviewPage, "Default");
            base.view.transform.Find("ListPanel/ScrollView/Content").GetComponent<Animation>().Play();
        }

        public void OnLvInfoClick()
        {
            if (this.type != PageType.GalTouchReplace)
            {
                this.ShowDetailWithTab("LvUpTab");
            }
        }

        public void OnNextBtnClick()
        {
            base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().ScrollToNextPage();
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.SelectAvtarIconChange)
            {
                return this.UpdateSelectedAvatar((int) ntf.body);
            }
            if (ntf.type == NotifyTypes.UnlockAvatar)
            {
                int body = (int) ntf.body;
                Singleton<MainUIManager>.Instance.ShowDialog(new AvatarUnlockDialogContext(body, false), UIType.Any);
            }
            return false;
        }

        public void OnOutTeamBtnClick()
        {
            Singleton<PlayerModule>.Instance.playerData.RemoveTeamMember(this.levelType, this.teamEditIndex);
            Singleton<NetworkManager>.Instance.NotifyUpdateAvatarTeam(this.levelType);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TeamMemberChanged, null));
            Singleton<MainUIManager>.Instance.BackPage();
        }

        public void OnOutVentureDispatchClick()
        {
            this.ventureData.selectedAvatarList.RemoveAt(this.teamEditIndex - 1);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DispatchAvatarChanged, null));
            Singleton<MainUIManager>.Instance.BackPage();
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x19:
                    return this.SetupView();

                case 30:
                    return this.OnAvatarStarUpRsp(pkt.getData<AvatarStarUpRsp>());
            }
            return false;
        }

        public void OnPrevBtnClick()
        {
            base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().ScrollToPrevPage();
        }

        public void OnReplaceBtnClick()
        {
            AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(this.selectedAvatarID);
            if ((avatarByID != null) && avatarByID.UnLocked)
            {
                Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.lastSelectedAvatarID = this.selectedAvatarID;
                Singleton<GalTouchModule>.Instance.ChangeAvatar(this.selectedAvatarID);
                Singleton<MainUIManager>.Instance.BackPage();
            }
        }

        public void OnSkillInfoClick()
        {
            if (this.type != PageType.GalTouchReplace)
            {
                this.ShowDetailWithTab("SkillTab");
            }
        }

        public void OnStigmataInfoClick()
        {
            if (this.type != PageType.GalTouchReplace)
            {
                this.ShowDetailWithTab("StigmataTab");
            }
        }

        public void OnWeaponInfoClick()
        {
            if (this.type != PageType.GalTouchReplace)
            {
                this.ShowDetailWithTab("WeaponTab");
            }
        }

        private void PostAvatarChangeAudioPattern(int type)
        {
            string evtName = null;
            if (type == 1)
            {
                evtName = "VO_M_Kia_04_Selected";
            }
            else if (type == 2)
            {
                evtName = "VO_M_Mei_04_Selected";
            }
            else if (type == 3)
            {
                evtName = "VO_M_Bro_04_Selected";
            }
            if (evtName != null)
            {
                Singleton<WwiseAudioManager>.Instance.Post(evtName, null, null, null);
            }
        }

        private void SetUITempSaveData()
        {
            PlayerUITempSaveData uiTempSaveData = Singleton<PlayerModule>.Instance.playerData.uiTempSaveData;
            uiTempSaveData.lastSelectedAvatarID = this.selectedAvatarID;
            uiTempSaveData.avatarOverviewPageScrollerPos = base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().GetNormalizedPosition();
            base.view.transform.Find("Info/Unlock/Left/Info_2").gameObject.SetActive(true);
            base.view.transform.Find("Info/Unlock/Right/Weapon").gameObject.SetActive(true);
            base.view.transform.Find("Info/Unlock/Right/Stigmata").gameObject.SetActive(true);
            base.view.transform.Find("Info/Unlock/Right/Skill").gameObject.SetActive(true);
            base.view.transform.Find("Info/RightEdge").gameObject.SetActive(true);
            base.view.transform.Find("Info/LeftEdge").gameObject.SetActive(true);
        }

        private void SetupAvatarListPanel()
        {
            if (this.type == PageType.TeamEdit)
            {
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = x => x.UnLocked && !MiscData.Config.AvatarClassDoNotShow.Contains(x.ClassId);
                }
                this.showAvatarList = Singleton<AvatarModule>.Instance.UserAvatarList.FindAll(<>f__am$cache7);
            }
            else
            {
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = x => !MiscData.Config.AvatarClassDoNotShow.Contains(x.ClassId);
                }
                this.showAvatarList = Singleton<AvatarModule>.Instance.UserAvatarList.FindAll(<>f__am$cache8);
            }
            this.showAvatarList.Sort(new Comparison<AvatarDataItem>(this.CompareByDefault));
            base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().Init(new MoleMole.MonoGridScroller.OnChange(this.OnChange), this.showAvatarList.Count, null);
            if (this.type == PageType.Show)
            {
                this.ApplyTempSaveData();
            }
            base.view.transform.Find("ListPanel/ScrollView/Content").GetComponent<Animation>().Play();
        }

        private void SetupClassName(Transform parent, AvatarDataItem avatarSelected)
        {
            parent.Find("Dot").gameObject.SetActive(!avatarSelected.IsEasterner());
            if (avatarSelected.IsEasterner())
            {
                parent.Find("FirstName").GetComponent<Text>().text = avatarSelected.ClassLastName;
                parent.Find("FirstName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnLastName;
                parent.Find("LastName").GetComponent<Text>().text = avatarSelected.ClassFirstName;
                parent.Find("LastName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnFirstName;
            }
            else
            {
                parent.Find("FirstName").GetComponent<Text>().text = avatarSelected.ClassFirstName;
                parent.Find("FirstName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnFirstName;
                parent.Find("LastName").GetComponent<Text>().text = avatarSelected.ClassLastName;
                parent.Find("LastName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnLastName;
            }
        }

        private void SetupGalReplace()
        {
            Button component = base.view.transform.Find("GalReplaceBtn").GetComponent<Button>();
            base.BindViewCallback(component, new UnityAction(this.OnReplaceBtnClick));
            component.interactable = this.selectedAvatarID != Singleton<GalTouchModule>.Instance.GetCurrentTouchAvatarID();
            AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(this.selectedAvatarID);
            if (avatarByID != null)
            {
                component.interactable = component.interactable && avatarByID.UnLocked;
            }
            base.view.transform.Find("Info/Unlock/Left/Info_2").gameObject.SetActive(false);
            base.view.transform.Find("Info/Unlock/Right/Weapon").gameObject.SetActive(false);
            base.view.transform.Find("Info/Unlock/Right/Stigmata").gameObject.SetActive(false);
            base.view.transform.Find("Info/Unlock/Right/Skill").gameObject.SetActive(false);
            base.view.transform.Find("Info/RightEdge").gameObject.SetActive(false);
            base.view.transform.Find("Info/LeftEdge").gameObject.SetActive(false);
        }

        private void SetupSelectedAvatarInfo()
        {
            base.view.transform.Find("ListPanel/ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
            AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(this.selectedAvatarID);
            Transform transform = base.view.transform.Find("Info/Unlock");
            Transform transform2 = base.view.transform.Find("Info/Lock");
            this.CheckLockByMissionTutorial();
            if (avatarByID.UnLocked)
            {
                transform.gameObject.SetActive(true);
                transform2.gameObject.SetActive(false);
                this.SetupClassName(transform.Find("Left/Info_1/ClassName"), avatarByID);
                transform.Find("Left/Info_1/NameText").GetComponent<Text>().text = avatarByID.ShortName;
                transform.Find("Left/Info_1/SmallIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(avatarByID.AttributeIconPath);
                transform.Find("Left/Info_2/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(avatarByID.star);
                transform.Find("Left/Info_2/LevelText").GetComponent<Text>().text = "LV." + avatarByID.level;
                transform.Find("Left/Info_2/Combat/NumText").GetComponent<Text>().text = Mathf.FloorToInt(avatarByID.CombatNum).ToString();
                transform.Find("Left/Info_2/PopUp").gameObject.SetActive(avatarByID.CanStarUp);
                WeaponDataItem weapon = avatarByID.GetWeapon();
                transform.Find("Right/Weapon/Name").GetComponent<Text>().text = (weapon != null) ? weapon.GetDisplayTitle() : string.Empty;
                this.SetupStigmataSmallIcons(transform.Find("Right/Stigmata/Icons"), avatarByID);
                transform.Find("Right/Skill/SkillPoint/PointNum").GetComponent<Text>().text = "+" + avatarByID.GetSkillPointAddNum();
                if ((avatarByID.LevelTutorialID != 0) && !Singleton<LevelTutorialModule>.Instance.IsTutorialIDFinish(avatarByID.LevelTutorialID))
                {
                    Singleton<ApplicationManager>.Instance.StartCoroutine(this.ShowAvatarTutorialDialog(avatarByID));
                }
            }
            else
            {
                transform.gameObject.SetActive(false);
                transform2.gameObject.SetActive(true);
                this.SetupClassName(transform2.Find("Left/ClassName"), avatarByID);
                transform2.Find("Left/DescContent").GetComponent<Text>().text = avatarByID.Desc;
                transform2.Find("Right/InfoRow_1/NameText").GetComponent<Text>().text = avatarByID.ShortName;
                transform2.Find("Right/InfoRow_1/SmallIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(avatarByID.AttributeIconPath);
                string prefabPath = MiscData.Config.PrefabPath.WeaponBaseTypeIcon[avatarByID.WeaponBaseTypeList[0]];
                transform2.Find("Right/InfoRow_2/WeaponTypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
                transform2.Find("Right/InfoRow_3/Fragment/NumText").GetComponent<Text>().text = avatarByID.fragment + "/" + avatarByID.MaxFragment;
                transform2.Find("Right/InfoRow_3/Fragment/TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue((float) avatarByID.fragment, (float) avatarByID.MaxFragment, 0f);
                transform2.Find("Right/InfoRow_4/UnlockBtn").GetComponent<Button>().interactable = avatarByID.CanStarUp;
                MonoButtonWwiseEvent component = transform2.Find("Right/InfoRow_4/UnlockBtn").GetComponent<MonoButtonWwiseEvent>();
                if (component == null)
                {
                    component = transform2.Find("Right/InfoRow_4/UnlockBtn").gameObject.AddComponent<MonoButtonWwiseEvent>();
                }
                component.eventName = !avatarByID.CanStarUp ? "UI_Gen_Select_Negative" : "UI_Click";
                transform2.Find("Right/InfoRow_4/UnlockBtn/PopUp").gameObject.SetActive(avatarByID.CanStarUp);
            }
            UIUtil.Create3DAvatarByPage(avatarByID, MiscData.PageInfoKey.AvatarOverviewPage, "Default");
            if (this.type == PageType.TeamEdit)
            {
                this.SetupTeamEdit();
            }
            else if (this.type == PageType.Show)
            {
                this.SetUITempSaveData();
            }
            else if (this.type == PageType.GalTouchReplace)
            {
                this.SetupGalReplace();
            }
        }

        private void SetupStigmataSmallIcons(Transform parent, AvatarDataItem avatarSelected)
        {
            List<StigmataDataItem> stigmataList = avatarSelected.GetStigmataList();
            for (int i = 1; i <= stigmataList.Count; i++)
            {
                Transform transform = parent.Find(i.ToString());
                if (stigmataList[i - 1] != null)
                {
                    transform.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(stigmataList[i - 1].GetSmallIconPath());
                }
                else
                {
                    transform.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/StigmataSmallIcon/Icon_add");
                }
            }
        }

        private void SetupTeamEdit()
        {
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(this.levelType);
            bool flag = (this.selectedAvatarID != 0) && memberList.Contains(this.selectedAvatarID);
            Button component = base.view.transform.Find("TeamEditBtn").GetComponent<Button>();
            component.gameObject.SetActive(true);
            if ((this.teamEditIndex <= memberList.Count) && (this.selectedAvatarID == memberList[this.teamEditIndex - 1]))
            {
                base.BindViewCallback(component, new UnityAction(this.OnOutTeamBtnClick));
                component.interactable = memberList.Count > 1;
                string textID = "Menu_GetOutTeam";
                component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID, new object[0]);
            }
            else
            {
                base.BindViewCallback(component, new UnityAction(this.OnInTeamBtnClick));
                component.interactable = !flag;
                string str2 = !flag ? "Menu_EnterTeam" : "Menu_AlreadyInTeam";
                component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(str2, new object[0]);
            }
            if ((this.levelType == 4) && Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(this.selectedAvatarID).get_is_die())
            {
                component.interactable = false;
                component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessEnergyUseUp", new object[0]);
            }
            else if (!flag && Singleton<IslandModule>.Instance.IsAvatarDispatched(this.selectedAvatarID))
            {
                component.interactable = false;
                component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_AvatarAlreadyDispatched", new object[0]);
            }
            base.view.transform.Find("Info/Unlock/Left/Info_2").gameObject.SetActive(true);
            base.view.transform.Find("Info/Unlock/Right/Weapon").gameObject.SetActive(true);
            base.view.transform.Find("Info/Unlock/Right/Stigmata").gameObject.SetActive(true);
            base.view.transform.Find("Info/Unlock/Right/Skill").gameObject.SetActive(true);
            base.view.transform.Find("Info/RightEdge").gameObject.SetActive(true);
            base.view.transform.Find("Info/LeftEdge").gameObject.SetActive(true);
        }

        protected override bool SetupView()
        {
            if (Singleton<AvatarModule>.Instance.UserAvatarList.Count != 0)
            {
                this.SetupAvatarListPanel();
                if (this.selectedAvatarID == 0)
                {
                    this.selectedAvatarID = this.showAvatarList[0].avatarID;
                    if (this.type == PageType.TeamEdit)
                    {
                        <SetupView>c__AnonStoreyE7 ye = new <SetupView>c__AnonStoreyE7 {
                            memberIdList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(this.levelType)
                        };
                        AvatarDataItem item = this.showAvatarList.Find(new Predicate<AvatarDataItem>(ye.<>m__124));
                        if (item != null)
                        {
                            this.selectedAvatarID = item.avatarID;
                        }
                    }
                }
                this.SetupSelectedAvatarInfo();
                base.view.transform.Find("TeamEditBtn").gameObject.SetActive(PageType.TeamEdit == this.type);
                base.view.transform.Find("GalReplaceBtn").gameObject.SetActive(PageType.GalTouchReplace == this.type);
            }
            return false;
        }

        [DebuggerHidden]
        private IEnumerator ShowAvatarTutorialDialog(AvatarDataItem avatar)
        {
            return new <ShowAvatarTutorialDialog>c__Iterator60 { avatar = avatar, <$>avatar = avatar };
        }

        private void ShowDetailWithTab(string tabString)
        {
            AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(this.selectedAvatarID);
            if (avatarByID.UnLocked)
            {
                Singleton<MainUIManager>.Instance.ShowPage(new AvatarDetailPageContext(avatarByID, tabString), UIType.Page);
                UIUtil.SetAvatarTattooVisible(tabString == "StigmataTab", avatarByID);
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowPage(new AvatarIntroPageContext(avatarByID), UIType.Page);
            }
        }

        private bool UpdateSelectedAvatar(int avatarId)
        {
            this.selectedAvatarID = avatarId;
            this.SetupSelectedAvatarInfo();
            this.PostAvatarChangeAudioPattern(avatarId / 100);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PlayAvtarChangeEffect, null));
            return false;
        }

        [CompilerGenerated]
        private sealed class <SetupView>c__AnonStoreyE7
        {
            internal List<int> memberIdList;

            internal bool <>m__124(AvatarDataItem x)
            {
                return ((x.UnLocked && !this.memberIdList.Contains(x.avatarID)) && !Singleton<IslandModule>.Instance.IsAvatarDispatched(x.avatarID));
            }
        }

        [CompilerGenerated]
        private sealed class <ShowAvatarTutorialDialog>c__Iterator60 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal AvatarDataItem <$>avatar;
            internal AvatarDataItem avatar;

            internal void <>m__127(bool confirmed)
            {
                if (confirmed)
                {
                    LevelTutorialMetaData levelTutorialMetaDataByKey = LevelTutorialMetaDataReader.GetLevelTutorialMetaDataByKey(this.avatar.LevelTutorialID);
                    Singleton<LevelScoreManager>.Create();
                    Singleton<LevelScoreManager>.Instance.SetTryLevelBeginIntent(this.avatar.avatarID, levelTutorialMetaDataByKey.lua, 0, 0);
                    Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", true, true, true, null, true);
                }
            }

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
                        Singleton<LevelTutorialModule>.Instance.MarkTutorialIDFinish(this.avatar.LevelTutorialID);
                        this.$current = null;
                        this.$PC = 1;
                        return true;

                    case 1:
                    {
                        GeneralConfirmDialogContext dialogContext = new GeneralConfirmDialogContext {
                            type = GeneralConfirmDialogContext.ButtonType.DoubleButton
                        };
                        object[] replaceParams = new object[] { this.avatar.ShortName };
                        dialogContext.desc = LocalizationGeneralLogic.GetText("Menu_Desc_AvatarTutorial", replaceParams);
                        dialogContext.buttonCallBack = new Action<bool>(this.<>m__127);
                        Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                        this.$PC = -1;
                        break;
                    }
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

        public enum PageType
        {
            Show,
            TeamEdit,
            GalTouchReplace
        }
    }
}

