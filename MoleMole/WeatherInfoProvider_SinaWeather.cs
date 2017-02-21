namespace MoleMole
{
    using SimpleJSON;
    using System;
    using System.Collections.Generic;

    public class WeatherInfoProvider_SinaWeather : WeatherInfoProvider
    {
        private Dictionary<string, WeatherType> _weatherInfoDict;

        public WeatherInfoProvider_SinaWeather()
        {
            Dictionary<string, WeatherType> dictionary = new Dictionary<string, WeatherType>();
            dictionary.Add("01_01", WeatherType.Sunny);
            dictionary.Add("02_08", WeatherType.Cloudy);
            dictionary.Add("03_01", WeatherType.Rainy);
            dictionary.Add("03_03", WeatherType.Rainy);
            dictionary.Add("03_06", WeatherType.Rainy);
            dictionary.Add("03_07", WeatherType.Rainy);
            dictionary.Add("03_19", WeatherType.Rainy);
            dictionary.Add("03_22", WeatherType.Rainy);
            dictionary.Add("03_26", WeatherType.Rainy);
            dictionary.Add("03_28", WeatherType.Rainy);
            dictionary.Add("03_30", WeatherType.Rainy);
            dictionary.Add("04_02", WeatherType.Snowy);
            dictionary.Add("04_07", WeatherType.Snowy);
            dictionary.Add("04_21", WeatherType.Snowy);
            dictionary.Add("04_27", WeatherType.Snowy);
            dictionary.Add("04_29", WeatherType.Snowy);
            dictionary.Add("05_13", WeatherType.Windy);
            dictionary.Add("05_20", WeatherType.Windy);
            dictionary.Add("05_23", WeatherType.Windy);
            dictionary.Add("05_31", WeatherType.Windy);
            dictionary.Add("07_25", WeatherType.HeavyCloudy);
            dictionary.Add("08_11", WeatherType.Lightning);
            dictionary.Add("09_10", WeatherType.Extreme);
            dictionary.Add("09_14", WeatherType.Extreme);
            dictionary.Add("09_15", WeatherType.Extreme);
            dictionary.Add("09_24", WeatherType.Extreme);
            this._weatherInfoDict = dictionary;
        }

        protected override string GetUrl()
        {
            return "http://weather.sina.com.cn/";
        }

        protected override WeatherInfo ParseWeatherInfo(string content)
        {
            string str = "var PAGECONF";
            int index = content.IndexOf(str);
            if (index == -1)
            {
                return null;
            }
            int startIndex = content.IndexOf("{", index);
            if (startIndex == -1)
            {
                return null;
            }
            int num3 = content.IndexOf("};", startIndex);
            if (num3 == -1)
            {
                return null;
            }
            int length = (num3 - startIndex) + 1;
            if (length <= 0)
            {
                return null;
            }
            string aJSON = content.Substring(startIndex, length);
            try
            {
                JSONNode node = JSONNode.Parse(aJSON);
                char[] trimChars = new char[] { '\'' };
                string key = node["HOSTCITY"]["weatherCode"].Value.Trim(trimChars).Substring(0, 5);
                char[] chArray2 = new char[] { '\'' };
                string s = node["HOSTCITY"]["temp"].Value.Trim(chArray2);
                WeatherInfo info = new WeatherInfo {
                    weatherType = !this._weatherInfoDict.ContainsKey(key) ? WeatherType.Sunny : this._weatherInfoDict[key]
                };
                if (!float.TryParse(s, out info.temperature))
                {
                    info.temperature = -100f;
                }
                return info;
            }
            catch
            {
                return null;
            }
        }
    }
}

