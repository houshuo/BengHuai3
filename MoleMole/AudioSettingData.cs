namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public static class AudioSettingData
    {
        private static float _volumeThreshold = 0.01f;

        public static void ApplySettingConfig()
        {
            ConfigAudioSetting personalAudioSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalAudioSetting;
            if (personalAudioSetting.IsUserDefined)
            {
                Singleton<WwiseAudioManager>.Instance.SetParam("Vol_BGM", personalAudioSetting.BGMVolume);
                Singleton<WwiseAudioManager>.Instance.SetParam("Vol_SE", personalAudioSetting.SoundEffectVolume);
                Singleton<WwiseAudioManager>.Instance.SetParam("Vol_Voice", personalAudioSetting.VoiceVolume);
                Singleton<WwiseAudioManager>.Instance.SetLanguage(personalAudioSetting.CVLanguage);
            }
        }

        public static void CopyPersonalAudioConfig(ref ConfigAudioSetting to)
        {
            ConfigAudioSetting personalAudioSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalAudioSetting;
            to.BGMVolume = personalAudioSetting.BGMVolume;
            to.SoundEffectVolume = personalAudioSetting.SoundEffectVolume;
            to.VoiceVolume = personalAudioSetting.VoiceVolume;
            to.CVLanguage = personalAudioSetting.CVLanguage;
        }

        public static bool IsValueEqualToPersonalAudioConfig(ConfigAudioSetting to)
        {
            ConfigAudioSetting personalAudioSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalAudioSetting;
            return ((((Mathf.Abs((float) (personalAudioSetting.BGMVolume - to.BGMVolume)) <= _volumeThreshold) && (Mathf.Abs((float) (personalAudioSetting.SoundEffectVolume - to.SoundEffectVolume)) <= _volumeThreshold)) && (Mathf.Abs((float) (personalAudioSetting.VoiceVolume - to.VoiceVolume)) <= _volumeThreshold)) && (personalAudioSetting.CVLanguage == to.CVLanguage));
        }

        public static void SavePersonalConfig(ConfigAudioSetting settingConfig)
        {
            Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalAudioSetting.BGMVolume = settingConfig.BGMVolume;
            Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalAudioSetting.SoundEffectVolume = settingConfig.SoundEffectVolume;
            Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalAudioSetting.VoiceVolume = settingConfig.VoiceVolume;
            Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalAudioSetting.CVLanguage = settingConfig.CVLanguage;
            Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalAudioSetting.IsUserDefined = true;
            Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
            ApplySettingConfig();
        }
    }
}

