namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class DispatchAvatarDialogContext : BaseDialogContext
    {
        [CompilerGenerated]
        private static Predicate<AvatarDataItem> <>f__am$cache4;
        public int selectedAvatarID;
        private List<AvatarDataItem> showAvatarList;
        public int teamEditIndex;
        public VentureDataItem ventureData;

        public DispatchAvatarDialogContext(VentureDataItem ventureData, int index)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "DispatchAvatarDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/CabinDispatchAvatarDialog"
            };
            base.config = pattern;
            this.ventureData = ventureData;
            this.teamEditIndex = index;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ListPanel/NextBtn").GetComponent<Button>(), new UnityAction(this.OnNextBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ListPanel/PrevBtn").GetComponent<Button>(), new UnityAction(this.OnPrevBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
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

        private void OnChange(Transform trans, int index)
        {
            bool isSelected = this.showAvatarList[index].avatarID == this.selectedAvatarID;
            AvatarDataItem avatarDataItem = this.showAvatarList[index];
            trans.GetComponent<MonoAvatarIcon>().SetupView(avatarDataItem, isSelected, null);
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
            this.Destroy();
        }

        public void OnNextBtnClick()
        {
            base.view.transform.Find("Dialog/Content/ListPanel/ScrollView").GetComponent<MonoGridScroller>().ScrollToNextPage();
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.SelectAvtarIconChange) && this.UpdateSelectedAvatar((int) ntf.body));
        }

        public void OnOutVentureDispatchClick()
        {
            this.ventureData.selectedAvatarList.RemoveAt(this.teamEditIndex - 1);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DispatchAvatarChanged, null));
            this.Destroy();
        }

        public void OnPrevBtnClick()
        {
            base.view.transform.Find("Dialog/Content/ListPanel/ScrollView").GetComponent<MonoGridScroller>().ScrollToPrevPage();
        }

        private void SetupAvatarListPanel()
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = x => x.UnLocked;
            }
            this.showAvatarList = Singleton<AvatarModule>.Instance.UserAvatarList.FindAll(<>f__am$cache4);
            this.showAvatarList.Sort(new Comparison<AvatarDataItem>(this.CompareByDefault));
            base.view.transform.Find("Dialog/Content/ListPanel/ScrollView").GetComponent<MonoGridScroller>().Init(new MoleMole.MonoGridScroller.OnChange(this.OnChange), this.showAvatarList.Count, null);
            base.view.transform.Find("Dialog/Content/ListPanel/ScrollView/Content").GetComponent<Animation>().Play();
        }

        private void SetupClassName(Transform parent, AvatarDataItem avatarSelected)
        {
            parent.Find("FirstName").GetComponent<Text>().text = avatarSelected.ClassFirstName;
            parent.Find("FirstName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnFirstName;
            parent.Find("LastName").GetComponent<Text>().text = avatarSelected.ClassLastName;
            parent.Find("LastName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnLastName;
        }

        private void SetupSelectedAvatarInfo()
        {
            base.view.transform.Find("Dialog/Content/ListPanel/ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
            AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(this.selectedAvatarID);
            Transform transform = base.view.transform.Find("Dialog/Content/InfoPanel");
            this.SetupClassName(transform.Find("Info_1/ClassName"), avatarByID);
            transform.Find("Info_1/NameText").GetComponent<Text>().text = avatarByID.ShortName;
            transform.Find("Info_1/SmallIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(avatarByID.AttributeIconPath);
            transform.Find("Info_1/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(avatarByID.star);
            transform.Find("Info_2/LevelText").GetComponent<Text>().text = "LV." + avatarByID.level;
            transform.Find("Info_2/Combat/NumText").GetComponent<Text>().text = Mathf.FloorToInt(avatarByID.CombatNum).ToString();
            this.SetupVentureDispatchBtn();
        }

        private void SetupVentureDispatchBtn()
        {
            List<int> selectedAvatarList = this.ventureData.selectedAvatarList;
            bool flag = (this.selectedAvatarID != 0) && selectedAvatarList.Contains(this.selectedAvatarID);
            Button component = base.view.transform.Find("Dialog/Content/InfoPanel/Btn").GetComponent<Button>();
            if ((this.teamEditIndex <= selectedAvatarList.Count) && (this.selectedAvatarID == selectedAvatarList[this.teamEditIndex - 1]))
            {
                base.BindViewCallback(component, new UnityAction(this.OnOutVentureDispatchClick));
                component.interactable = true;
                string textID = "Menu_GetOutTeam";
                component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID, new object[0]);
            }
            else
            {
                base.BindViewCallback(component, new UnityAction(this.OnInVentureDispatchBtnClick));
                component.interactable = !flag;
                string str2 = !flag ? "Menu_Action_DispatchAvatar" : "Menu_AlreadyInTeam";
                component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(str2, new object[0]);
            }
            if (Singleton<IslandModule>.Instance.IsAvatarDispatched(this.selectedAvatarID))
            {
                component.interactable = false;
                component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_AvatarAlreadyDispatched", new object[0]);
            }
        }

        protected override bool SetupView()
        {
            if (Singleton<AvatarModule>.Instance.UserAvatarList.Count != 0)
            {
                this.SetupAvatarListPanel();
                if (this.selectedAvatarID == 0)
                {
                    <SetupView>c__AnonStoreyE1 ye = new <SetupView>c__AnonStoreyE1();
                    this.selectedAvatarID = this.showAvatarList[0].avatarID;
                    ye.memberIdList = this.ventureData.selectedAvatarList;
                    AvatarDataItem item = this.showAvatarList.Find(new Predicate<AvatarDataItem>(ye.<>m__10C));
                    if (item != null)
                    {
                        this.selectedAvatarID = item.avatarID;
                    }
                }
                this.SetupSelectedAvatarInfo();
            }
            return false;
        }

        private bool UpdateSelectedAvatar(int avatarId)
        {
            this.selectedAvatarID = avatarId;
            this.SetupSelectedAvatarInfo();
            return false;
        }

        [CompilerGenerated]
        private sealed class <SetupView>c__AnonStoreyE1
        {
            internal List<int> memberIdList;

            internal bool <>m__10C(AvatarDataItem x)
            {
                return (((x.UnLocked && !this.memberIdList.Contains(x.avatarID)) && !Singleton<IslandModule>.Instance.IsAvatarDispatched(x.avatarID)) && !MiscData.Config.AvatarClassDoNotShow.Contains(x.ClassId));
            }
        }
    }
}

