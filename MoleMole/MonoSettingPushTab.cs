namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class MonoSettingPushTab : MonoBehaviour
    {
        private ConfigNotificationSetting _modifiedSettingConfig;
        public Transform activitySettingBtn;
        public Transform cabinLevelUpSettingBtn;
        public Transform expeditionEndSettingBtn;
        public Transform skillPointSettingBtn;
        public Transform staminaSettingBtn;

        public bool CheckNeedSave()
        {
            return !NotificationSettingData.IsValueEqualToPersonalNotificationConfig(this._modifiedSettingConfig);
        }

        public void OnActivityNotificationClick(bool willBeOn)
        {
            this._modifiedSettingConfig.ActivityNotification = willBeOn;
            this.SetChoiceBtn(this.activitySettingBtn, this._modifiedSettingConfig.ActivityNotification);
        }

        public void OnCabinLevelUpNotificationClick(bool willBeOn)
        {
            this._modifiedSettingConfig.CabinLevelUpNotification = willBeOn;
            this.SetChoiceBtn(this.cabinLevelUpSettingBtn, this._modifiedSettingConfig.CabinLevelUpNotification);
        }

        public void OnEnergyFullNotificationClick(bool willBeOn)
        {
            this._modifiedSettingConfig.StaminaFullNotificaltion = willBeOn;
            this.SetChoiceBtn(this.staminaSettingBtn, this._modifiedSettingConfig.StaminaFullNotificaltion);
        }

        public void OnExpeditionEndNotificationClick(bool willBeOn)
        {
            this._modifiedSettingConfig.VentureDoneNotification = willBeOn;
            this.SetChoiceBtn(this.expeditionEndSettingBtn, this._modifiedSettingConfig.VentureDoneNotification);
        }

        public void OnNoSaveBtnClick()
        {
            this.RecoverOriginState();
        }

        public void OnSaveBtnClick()
        {
            NotificationSettingData.SavePersonalNotificationConfig(this._modifiedSettingConfig);
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SettingSaveSuccess", new object[0]), 2f), UIType.Any);
        }

        public void OnSkillPointFullNotificationClick(bool willBeOn)
        {
            this._modifiedSettingConfig.SkillPointFullNotification = willBeOn;
            this.SetChoiceBtn(this.skillPointSettingBtn, this._modifiedSettingConfig.SkillPointFullNotification);
        }

        private void RecoverOriginState()
        {
            NotificationSettingData.ApplyNotificationSettingConfig();
            NotificationSettingData.CopyPersonalNotificationConfig(ref this._modifiedSettingConfig);
            this.SetChoiceBtn(this.staminaSettingBtn, this._modifiedSettingConfig.StaminaFullNotificaltion);
            this.SetChoiceBtn(this.skillPointSettingBtn, this._modifiedSettingConfig.SkillPointFullNotification);
            this.SetChoiceBtn(this.expeditionEndSettingBtn, this._modifiedSettingConfig.VentureDoneNotification);
            this.SetChoiceBtn(this.cabinLevelUpSettingBtn, this._modifiedSettingConfig.CabinLevelUpNotification);
            this.SetChoiceBtn(this.activitySettingBtn, this._modifiedSettingConfig.ActivityNotification);
        }

        private void SetChoiceBtn(Transform btn, bool active)
        {
            if (btn != null)
            {
                Transform transform = btn.Find("Choice/On");
                Transform transform2 = btn.Find("Choice/Off");
                transform.gameObject.SetActive(active);
                transform2.gameObject.SetActive(!active);
            }
        }

        private void SetSettingEnable(Transform settingTrans, bool enable)
        {
            Transform transform = settingTrans.FindChild("Choice/On");
            Transform transform2 = settingTrans.FindChild("Choice/Off");
            Transform transform3 = settingTrans.FindChild("Label");
            Transform transform4 = transform.FindChild("Text");
            Transform transform5 = transform2.FindChild("Text");
            if (!enable)
            {
                transform.FindChild("Blue").gameObject.SetActive(false);
                transform.FindChild("Grey").gameObject.SetActive(false);
                transform.FindChild("Disable").gameObject.SetActive(true);
                transform2.FindChild("Grey").gameObject.SetActive(false);
                transform2.FindChild("Disable").gameObject.SetActive(true);
                transform3.GetComponent<Text>().color = MiscData.GetColor("NotificationSettingDisableText");
                transform4.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
                transform5.GetComponent<Text>().color = MiscData.GetColor("GraphicsSettingDisableText");
            }
            else
            {
                transform.FindChild("Blue").gameObject.SetActive(true);
                transform.FindChild("Grey").gameObject.SetActive(true);
                transform2.FindChild("Grey").gameObject.SetActive(true);
                transform.FindChild("Disable").gameObject.SetActive(false);
                transform2.FindChild("Disable").gameObject.SetActive(false);
                transform3.GetComponent<Text>().color = Color.white;
                transform4.GetComponent<Text>().color = Color.white;
                transform5.GetComponent<Text>().color = Color.white;
            }
            transform.GetComponent<Button>().interactable = enable;
            transform2.GetComponent<Button>().interactable = enable;
        }

        public void SetupView()
        {
            this._modifiedSettingConfig = new ConfigNotificationSetting();
            this.RecoverOriginState();
            Transform settingTrans = base.gameObject.transform.FindChild("Content/NotificationSetting/ThirdLine/ActivityNotification").transform;
            this.SetSettingEnable(settingTrans, false);
        }
    }
}

