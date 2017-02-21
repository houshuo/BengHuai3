namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;

    public class ConfigWeather : BaseScriptableObject
    {
        public string rainPath;
        public string renderingDataPath;
        [InspectorNullable]
        public ConfigStageEffectSetting stageEffectSetting;

        public void CopyFrom(ConfigWeather source)
        {
            this.renderingDataPath = source.renderingDataPath;
            this.rainPath = source.rainPath;
        }

        public static ConfigWeather CreateDefault()
        {
            return new ConfigWeather { rainPath = "Weather/Rain/Default" };
        }
    }
}

