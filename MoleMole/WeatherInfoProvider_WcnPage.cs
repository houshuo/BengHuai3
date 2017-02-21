namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class WeatherInfoProvider_WcnPage : WeatherInfoProvider
    {
        private Dictionary<string, WeatherType> _weatherTypeDict;

        public WeatherInfoProvider_WcnPage()
        {
            Dictionary<string, WeatherType> dictionary = new Dictionary<string, WeatherType>();
            dictionary.Add("00", WeatherType.Sunny);
            dictionary.Add("01", WeatherType.Cloudy);
            dictionary.Add("02", WeatherType.Cloudy);
            dictionary.Add("03", WeatherType.Rainy);
            dictionary.Add("04", WeatherType.Lightning);
            dictionary.Add("05", WeatherType.Lightning);
            dictionary.Add("06", WeatherType.Rainy);
            dictionary.Add("07", WeatherType.Rainy);
            dictionary.Add("08", WeatherType.Rainy);
            dictionary.Add("09", WeatherType.Rainy);
            dictionary.Add("10", WeatherType.Rainy);
            dictionary.Add("11", WeatherType.Rainy);
            dictionary.Add("12", WeatherType.Rainy);
            dictionary.Add("13", WeatherType.Sunny);
            dictionary.Add("14", WeatherType.Sunny);
            dictionary.Add("15", WeatherType.Sunny);
            dictionary.Add("16", WeatherType.Sunny);
            dictionary.Add("17", WeatherType.Sunny);
            dictionary.Add("18", WeatherType.Sunny);
            dictionary.Add("19", WeatherType.Rainy);
            dictionary.Add("20", WeatherType.Sunny);
            dictionary.Add("21", WeatherType.Rainy);
            dictionary.Add("22", WeatherType.Rainy);
            dictionary.Add("23", WeatherType.Rainy);
            dictionary.Add("24", WeatherType.Rainy);
            dictionary.Add("25", WeatherType.Rainy);
            dictionary.Add("26", WeatherType.Sunny);
            dictionary.Add("27", WeatherType.Sunny);
            dictionary.Add("28", WeatherType.Sunny);
            dictionary.Add("29", WeatherType.Sunny);
            dictionary.Add("30", WeatherType.Sunny);
            dictionary.Add("31", WeatherType.Sunny);
            dictionary.Add("53", WeatherType.Sunny);
            this._weatherTypeDict = dictionary;
        }

        protected override string GetUrl()
        {
            return "http://weather.gtimg.cn/city/01012601.js";
        }

        protected override WeatherInfo ParseWeatherInfo(string content)
        {
            int index = content.IndexOf("sk_wt", 0);
            if (index == -1)
            {
                return null;
            }
            int num2 = content.IndexOf("\"", index);
            if (num2 == -1)
            {
                return null;
            }
            int num3 = content.IndexOf("\"", (int) (num2 + 1));
            if (num3 == -1)
            {
                return null;
            }
            if (((num3 - num2) - 1) < 0)
            {
                return null;
            }
            string key = content.Substring(num2 + 1, (num3 - num2) - 1);
            if (!this._weatherTypeDict.ContainsKey(key))
            {
                return null;
            }
            return new WeatherInfo { weatherType = this._weatherTypeDict[key], temperature = 30f, extraInfo = key };
        }
    }
}

