namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public static class NotificationSettingData
    {
        public static void ApplyNotificationSettingConfig()
        {
            if (!Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.IsUserDefined)
            {
            }
        }

        public static void CopyPersonalNotificationConfig(ref ConfigNotificationSetting to)
        {
            ConfigNotificationSetting personalNotificationSetting = Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting;
            to.StaminaFullNotificaltion = personalNotificationSetting.StaminaFullNotificaltion;
            to.SkillPointFullNotification = personalNotificationSetting.SkillPointFullNotification;
            to.ActivityNotification = personalNotificationSetting.ActivityNotification;
            to.VentureDoneNotification = personalNotificationSetting.VentureDoneNotification;
            to.CabinLevelUpNotification = personalNotificationSetting.CabinLevelUpNotification;
        }

        public static bool IsValueEqualToPersonalNotificationConfig(ConfigNotificationSetting to)
        {
            ConfigNotificationSetting personalNotificationSetting = Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting;
            return ((((personalNotificationSetting.ActivityNotification == to.ActivityNotification) && (personalNotificationSetting.StaminaFullNotificaltion == to.StaminaFullNotificaltion)) && ((personalNotificationSetting.SkillPointFullNotification == to.SkillPointFullNotification) && (personalNotificationSetting.VentureDoneNotification == to.VentureDoneNotification))) && (personalNotificationSetting.CabinLevelUpNotification == to.CabinLevelUpNotification));
        }

        public static void SavePersonalNotificationConfig(ConfigNotificationSetting settingConfig)
        {
            Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.StaminaFullNotificaltion = settingConfig.StaminaFullNotificaltion;
            Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.SkillPointFullNotification = settingConfig.SkillPointFullNotification;
            Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.ActivityNotification = settingConfig.ActivityNotification;
            Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.VentureDoneNotification = settingConfig.VentureDoneNotification;
            Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.CabinLevelUpNotification = settingConfig.CabinLevelUpNotification;
            Singleton<MiHoYoGameData>.Instance.LocalData.PersonalNotificationSetting.IsUserDefined = true;
            Singleton<MiHoYoGameData>.Instance.Save();
            ApplyNotificationSettingConfig();
        }
    }
}

