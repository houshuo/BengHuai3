namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoAvatarDetailLvUpTab : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        protected bool _isRemoteAvatar;
        private FriendDetailDataItem _userData;

        private void DeleteFriendConfirmCallBack(bool isConfirm)
        {
            if (isConfirm)
            {
                Singleton<NetworkManager>.Instance.RequestDelFriend(this._userData.uid);
            }
        }

        public void OnAddFriendBtnClick()
        {
            Singleton<NetworkManager>.Instance.RequestAddFriend(this._userData.uid);
        }

        public void OnCloseInfoPopUpBtnClick()
        {
            base.transform.Find("InfoPopup").gameObject.SetActive(false);
            base.transform.Find("InfoPopUpCloseBtn").gameObject.SetActive(false);
        }

        public void OnDeleteFriendBtnClick()
        {
            GeneralDialogContext dialogContext = new GeneralDialogContext {
                type = GeneralDialogContext.ButtonType.DoubleButton,
                title = LocalizationGeneralLogic.GetText("Menu_Action_DeleteFriend", new object[0]),
                desc = LocalizationGeneralLogic.GetText("Menu_Desc_ConfirmDeleteFriend", new object[0]),
                okBtnText = LocalizationGeneralLogic.GetText("Menu_Action_Delete", new object[0]),
                buttonCallBack = new Action<bool>(this.DeleteFriendConfirmCallBack)
            };
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        public void OnLvUpBtnClick()
        {
            PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
            bool flag = Singleton<StorageModule>.Instance.GetAllAvatarExpAddMaterial().Count > 0;
            bool flag2 = (this._avatarData.level < playerData.AvatarLevelLimit) || (this._avatarData.exp < this._avatarData.MaxExp);
            if (flag && flag2)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new MaterialUseDialogContext(this._avatarData), UIType.Any);
            }
            else
            {
                string text = LocalizationGeneralLogic.GetText("Err_Unknown", new object[0]);
                if (!flag)
                {
                    text = LocalizationGeneralLogic.GetText("Err_NoLvUpItem", new object[0]);
                }
                else if (!flag2)
                {
                    object[] replaceParams = new object[] { playerData.teamLevel, playerData.AvatarLevelLimit };
                    text = LocalizationGeneralLogic.GetText("Err_AvatarLevelLimit", replaceParams);
                }
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = text
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
        }

        public void OnShowInfoPopUpBtnClick()
        {
            base.transform.Find("InfoPopup").gameObject.SetActive(true);
            base.transform.Find("InfoPopUpCloseBtn").gameObject.SetActive(true);
        }

        public void OnStarUpBtnClick()
        {
            Singleton<NetworkManager>.Instance.RequestAvatarStarUp(this._avatarData.avatarID);
        }

        private void SetupAvatarBasicStatus(Transform trans)
        {
            trans.Find("HP/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._avatarData.FinalHPUI).ToString();
            trans.Find("SP/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._avatarData.FinalSPUI).ToString();
            trans.Find("ATK/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._avatarData.FinalAttackUI).ToString();
            trans.Find("DEF/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._avatarData.FinalDefenseUI).ToString();
            trans.Find("CRT/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._avatarData.FinalCriticalUI).ToString();
        }

        private void SetupTiltSlider(Transform trans, float value, float maxValue)
        {
            float num = value;
            float num2 = maxValue;
            string str = value.ToString();
            string str2 = maxValue.ToString();
            if (maxValue == 0f)
            {
                num2 = num;
                str2 = "MAX";
            }
            trans.Find("NumText").GetComponent<Text>().text = str;
            trans.Find("MaxNumText").GetComponent<Text>().text = str2;
            trans.Find("TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue(num, num2, 0f);
        }

        public void SetupView(AvatarDataItem avatarData)
        {
            this._isRemoteAvatar = false;
            this._avatarData = avatarData;
            this.SetupAvatarBasicStatus(base.transform.Find("BasicStatus/InfoPanel/BasicStatus"));
            base.transform.Find("BasicStatus/InfoPanel/StarPanel/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(this._avatarData.star);
            base.transform.Find("Introduction").gameObject.SetActive(false);
            base.transform.Find("Star").gameObject.SetActive(true);
            this.SetupTiltSlider(base.transform.Find("Star/InfoRowFragment/Fragment"), (float) avatarData.fragment, (float) avatarData.MaxFragment);
            base.transform.Find("Star/StarUpBtn").GetComponent<Button>().interactable = avatarData.CanStarUp;
            base.transform.Find("Star/StarUpBtn/PopUp").gameObject.SetActive(avatarData.CanStarUp);
            MonoButtonWwiseEvent component = null;
            component = base.transform.Find("Star/StarUpBtn").GetComponent<MonoButtonWwiseEvent>();
            if (component == null)
            {
                component = base.transform.Find("Star/StarUpBtn").gameObject.AddComponent<MonoButtonWwiseEvent>();
            }
            component.eventName = !base.transform.Find("Star/StarUpBtn").GetComponent<Button>().interactable ? "UI_Gen_Select_Negative" : "UI_Click";
            base.transform.Find("Lv").gameObject.SetActive(true);
            this.SetupTiltSlider(base.transform.Find("Lv/InfoRowLv/Exp"), (float) avatarData.exp, (float) avatarData.MaxExp);
            PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
            base.transform.Find("Lv/LvUpBtn").GetComponent<Button>().interactable = (this._avatarData.level < playerData.AvatarLevelLimit) || (this._avatarData.exp < this._avatarData.MaxExp);
            component = base.transform.Find("Lv/LvUpBtn").GetComponent<MonoButtonWwiseEvent>();
            if (component == null)
            {
                component = base.transform.Find("Lv/LvUpBtn").gameObject.AddComponent<MonoButtonWwiseEvent>();
            }
            component.eventName = !base.transform.Find("Lv/LvUpBtn").GetComponent<Button>().interactable ? "UI_Gen_Select_Negative" : "UI_Click";
        }

        public void SetupView(FriendDetailDataItem userData)
        {
            this._isRemoteAvatar = true;
            this._userData = userData;
            this._avatarData = this._userData.leaderAvatar;
            this.SetupAvatarBasicStatus(base.transform.Find("BasicStatus/InfoPanel/BasicStatus"));
            base.transform.Find("BasicStatus/InfoPanel/StarPanel/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(this._avatarData.star);
            base.transform.Find("Star").gameObject.SetActive(false);
            base.transform.Find("Lv").gameObject.SetActive(false);
            base.transform.Find("Introduction").gameObject.SetActive(true);
            base.transform.Find("Introduction/InfoPanel/Desc").GetComponent<Text>().text = this._userData.Desc;
            bool flag = Singleton<FriendModule>.Instance.IsMyFriend(this._userData.uid);
            base.transform.Find("Introduction/AddBtn").gameObject.SetActive(!flag);
            base.transform.Find("Introduction/DeleteBtn").gameObject.SetActive(flag);
        }
    }
}

