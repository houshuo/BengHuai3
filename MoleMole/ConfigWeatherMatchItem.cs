namespace MoleMole
{
    using System;

    [Serializable]
    public class ConfigWeatherMatchItem
    {
        public WeatherType realTimeWeatherType;
        public MainMenuWeather[] weatherPatterns;
    }
}

