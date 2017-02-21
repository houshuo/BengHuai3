namespace MoleMole.MainMenu
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ConfigAtmosphere
    {
        public ConfigBackground Background = new ConfigBackground();
        public ConfigCloudStyle CloudStyle;
        public float FrameTime;
        public ConfigIndoor Indoor;
        public string Name;

        public int CompareTo(ConfigAtmosphere target)
        {
            return this.FrameTime.CompareTo(target.FrameTime);
        }

        public static ConfigAtmosphere FromScriptable(ScriptableConfigAtmosphere target)
        {
            if (target == null)
            {
                return null;
            }
            return new ConfigAtmosphere { Name = target.name, FrameTime = target.FrameTime, CloudStyle = target.CloudStyle, Background = target.Background, Indoor = target.Indoor };
        }

        public void InitAfterLoad()
        {
            this.Name = this.FrameTime.ToString();
        }

        public static ConfigAtmosphere Lerp(ConfigAtmosphere config1, ConfigAtmosphere config2, float t)
        {
            ConfigAtmosphere atmosphere;
            return new ConfigAtmosphere { FrameTime = Mathf.Lerp(config1.FrameTime, config2.FrameTime, t), Name = atmosphere.FrameTime.ToString(), CloudStyle = ConfigCloudStyle.Lerp(config1.CloudStyle, config2.CloudStyle, t), Background = ConfigBackground.Lerp(config1.Background, config2.Background, t), Indoor = ConfigIndoor.Lerp(config1.Indoor, config2.Indoor, t) };
        }

        public static ScriptableConfigAtmosphere ToScriptable(ConfigAtmosphere source)
        {
            if (source == null)
            {
                return null;
            }
            ScriptableConfigAtmosphere atmosphere = ScriptableObject.CreateInstance<ScriptableConfigAtmosphere>();
            atmosphere.name = source.Name;
            atmosphere.FrameTime = source.FrameTime;
            atmosphere.CloudStyle = source.CloudStyle;
            atmosphere.Background = source.Background;
            atmosphere.Indoor = source.Indoor;
            return atmosphere;
        }
    }
}

