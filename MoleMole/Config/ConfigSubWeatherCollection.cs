namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;

    public class ConfigSubWeatherCollection
    {
        public ConfigRain configRain;
        [InspectorNullable]
        public ConfigStageEffectSetting stageEffectSetting;

        public ConfigSubWeatherCollection Copy()
        {
            return new ConfigSubWeatherCollection { configRain = (this.configRain == null) ? null : this.configRain.Clone() };
        }

        public static ConfigSubWeatherCollection CreateDefault()
        {
            return new ConfigSubWeatherCollection { configRain = ConfigRain.CreatDefault() };
        }

        public static ConfigSubWeatherCollection Lerp(ConfigSubWeatherCollection config1, ConfigSubWeatherCollection config2, float t)
        {
            return new ConfigSubWeatherCollection { configRain = ConfigRain.Lerp(config1.configRain, config2.configRain, t) };
        }

        public static void LerpPreparation(ConfigSubWeatherCollection config1, ConfigSubWeatherCollection config2)
        {
            if ((config1.configRain == null) && (config2.configRain != null))
            {
                config1.configRain = config2.configRain.GetNullLerpAble();
            }
            if ((config1.configRain != null) && (config2.configRain == null))
            {
                config2.configRain = config1.configRain.GetNullLerpAble();
            }
        }

        public static ConfigSubWeatherCollection LoadFromFile(ConfigWeather config)
        {
            ConfigSubWeatherCollection weathers = new ConfigSubWeatherCollection();
            if (!string.IsNullOrEmpty(config.rainPath))
            {
                weathers.configRain = ConfigUtil.LoadConfig<ConfigRain>(config.rainPath);
            }
            return weathers;
        }
    }
}

