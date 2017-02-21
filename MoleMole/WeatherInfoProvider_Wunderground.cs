namespace MoleMole
{
    using SimpleJSON;
    using System;

    public class WeatherInfoProvider_Wunderground : WeatherInfoProvider
    {
        protected override string GetUrl()
        {
            return "http://api.wunderground.com/api/a48bebc6f61128cf/conditions/q/autoip.json";
        }

        protected override WeatherInfo ParseWeatherInfo(string content)
        {
            WeatherInfo info = new WeatherInfo();
            JSONNode node = JSONNode.Parse(content);
            if (node != null)
            {
                info.weatherType = WeatherType.Sunny;
                info.temperature = node["current_observation"]["temp_c"].AsFloat;
            }
            info.extraInfo = content;
            return info;
        }
    }
}

