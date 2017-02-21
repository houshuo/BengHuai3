namespace MoleMole
{
    using FullInspector;
    using System;

    public class ConfigRealTimeWeather : BaseScriptableObject
    {
        public string defaultConfigName;
        public int defaultSceneId;
        public int maxRetryTime = 5;
        public float retryInterval = 10f;
        public ConfigWeatherSiteUsage siteUsage;
        public float weatherInfoPeriod = 3600f;
        public ConfigWeatherMatchItem[] weatherMatch;
    }
}

