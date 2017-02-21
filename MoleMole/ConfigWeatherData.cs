namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class ConfigWeatherData
    {
        public ConfigBaseRenderingData configRenderingData;
        public ConfigSubWeatherCollection configSubWeathers;

        public static ConfigWeatherData CrateDefault()
        {
            return new ConfigWeatherData { configRenderingData = ConfigStageRenderingData.CreateDefault(), configSubWeathers = ConfigSubWeatherCollection.CreateDefault() };
        }

        public static ConfigWeatherData LoadFromFile(ConfigWeather config)
        {
            if (config == null)
            {
                return null;
            }
            ConfigWeatherData data = new ConfigWeatherData();
            if (!string.IsNullOrEmpty(config.renderingDataPath))
            {
                data.configRenderingData = ConfigUtil.LoadConfig<ConfigBaseRenderingData>(config.renderingDataPath);
            }
            data.configSubWeathers = ConfigSubWeatherCollection.LoadFromFile(config);
            data.configSubWeathers.stageEffectSetting = config.stageEffectSetting;
            return data;
        }
    }
}

