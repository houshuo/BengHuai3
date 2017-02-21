namespace MoleMole
{
    using System;

    public class WeatherInfoProvider_LocalHost : WeatherInfoProvider
    {
        protected override string GetUrl()
        {
            return "http://localhost/";
        }

        protected override WeatherInfo ParseWeatherInfo(string content)
        {
            return null;
        }
    }
}

