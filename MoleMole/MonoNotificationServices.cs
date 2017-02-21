namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoNotificationServices : MonoBehaviour
    {
        private int _notificationId;
        private List<int> _notificationIdList = new List<int>();

        private void AddActivityNotification()
        {
        }

        private void AddCabinLevelUpNotification()
        {
            if (Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.CabinLevelUpNotification)
            {
                foreach (CabinDataItemBase base2 in Singleton<IslandModule>.Instance.GetCabinList())
                {
                    if (base2.IsUpLevel())
                    {
                        string text = LocalizationGeneralLogic.GetText("Menu_SettingCabinLevelUpNotification", new object[0]);
                        string str2 = LocalizationGeneralLogic.GetText("Notification_CabinLevelUp", new object[0]);
                        this.AddLocalNotification(base2.levelUpEndTime, text, str2);
                    }
                }
            }
        }

        private void AddGameLocalNotifications()
        {
            if (Singleton<NetworkManager>.Instance.alreadyLogin)
            {
                this.AddStaminaFullNotification();
                this.AddSkillPointFullNotification();
                this.AddActivityNotification();
                this.AddVentureDoneNotification();
                this.AddCabinLevelUpNotification();
            }
        }

        private void AddLocalNotification(DateTime time, string title, string text)
        {
            if (time >= DateTime.Now)
            {
                int item = ++this._notificationId;
                this._notificationIdList.Add(item);
                LocalNotificationPlugin.SendNotification(item, (TimeSpan) (time - DateTime.Now), title, text);
            }
        }

        private void AddSkillPointFullNotification()
        {
            bool skillPointFullNotification = Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.SkillPointFullNotification;
            if (!Singleton<PlayerModule>.Instance.playerData.IsSkillPointFull() && skillPointFullNotification)
            {
                DateTime skillPointFullTime = Singleton<PlayerModule>.Instance.playerData.GetSkillPointFullTime();
                string text = LocalizationGeneralLogic.GetText("Menu_SettingSkillPointFullNotification", new object[0]);
                string str2 = LocalizationGeneralLogic.GetText("Notification_FullSkillPoint", new object[0]);
                this.AddLocalNotification(skillPointFullTime, text, str2);
            }
        }

        private void AddStaminaFullNotification()
        {
            bool staminaFullNotificaltion = Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.StaminaFullNotificaltion;
            if (!Singleton<PlayerModule>.Instance.playerData.IsStaminaFull() && staminaFullNotificaltion)
            {
                DateTime staminaFullTime = Singleton<PlayerModule>.Instance.playerData.GetStaminaFullTime();
                string text = LocalizationGeneralLogic.GetText("Menu_SettingEnergyFullNotification", new object[0]);
                string str2 = LocalizationGeneralLogic.GetText("Notification_FullStamina", new object[0]);
                this.AddLocalNotification(staminaFullTime, text, str2);
            }
        }

        private void AddVentureDoneNotification()
        {
            if (Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.VentureDoneNotification)
            {
                foreach (VentureDataItem item in Singleton<IslandModule>.Instance.GetVentureList())
                {
                    if (item.status == VentureDataItem.VentureStatus.InProgress)
                    {
                        string text = LocalizationGeneralLogic.GetText("Menu_SettingExpeditionEndNotification", new object[0]);
                        string str2 = LocalizationGeneralLogic.GetText("Notification_VentureDone", new object[0]);
                        this.AddLocalNotification(item.endTime, text, str2);
                    }
                }
            }
        }

        private void Awake()
        {
            this.CleanNotification();
        }

        private void CleanNotification()
        {
            LocalNotificationPlugin.ClearNotifications();
            foreach (int num in this._notificationIdList)
            {
                LocalNotificationPlugin.CancelNotification(num);
            }
            this._notificationIdList.Clear();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                this.AddGameLocalNotifications();
            }
            else
            {
                this.CleanNotification();
            }
        }

        public void OnApplicationQuit()
        {
            this.CleanNotification();
            this.AddGameLocalNotifications();
        }
    }
}

