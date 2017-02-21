namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class WeatherInfoProvider_YahooWeather : WeatherInfoProvider
    {
        private List<NameWeatherPair> _nameWeatherList;
        private const string temperatureStartPattern = "WeatherLocationAndTemperature.0.1.1.2.0";
        private const string weatherStartPattern = "WeatherLocationAndTemperature.0.1.1.0.1";

        public WeatherInfoProvider_YahooWeather()
        {
            List<NameWeatherPair> list = new List<NameWeatherPair>();
            NameWeatherPair item = new NameWeatherPair {
                pattern = "Sunny",
                weather = WeatherType.Sunny
            };
            list.Add(item);
            NameWeatherPair pair2 = new NameWeatherPair {
                pattern = "Fair",
                weather = WeatherType.Sunny
            };
            list.Add(pair2);
            NameWeatherPair pair3 = new NameWeatherPair {
                pattern = "Clear",
                weather = WeatherType.Sunny
            };
            list.Add(pair3);
            NameWeatherPair pair4 = new NameWeatherPair {
                pattern = "Mostly Cloudy",
                weather = WeatherType.HeavyCloudy
            };
            list.Add(pair4);
            NameWeatherPair pair5 = new NameWeatherPair {
                pattern = "Cloudy",
                weather = WeatherType.Cloudy
            };
            list.Add(pair5);
            NameWeatherPair pair6 = new NameWeatherPair {
                pattern = "Shower",
                weather = WeatherType.Rainy
            };
            list.Add(pair6);
            NameWeatherPair pair7 = new NameWeatherPair {
                pattern = "Rain",
                weather = WeatherType.Rainy
            };
            list.Add(pair7);
            NameWeatherPair pair8 = new NameWeatherPair {
                pattern = "Snow",
                weather = WeatherType.Snowy
            };
            list.Add(pair8);
            NameWeatherPair pair9 = new NameWeatherPair {
                pattern = "Thunder",
                weather = WeatherType.Lightning
            };
            list.Add(pair9);
            this._nameWeatherList = list;
        }

        private string FetchInnerTextByPattern(string content, string pattern)
        {
            int index = content.IndexOf(pattern);
            if (index == -1)
            {
                return null;
            }
            int startIndex = content.IndexOf(">", index);
            if (startIndex == -1)
            {
                return null;
            }
            int num3 = content.IndexOf("<", startIndex);
            if (num3 == -1)
            {
                return null;
            }
            int length = (num3 - startIndex) - 1;
            return content.Substring(startIndex + 1, length);
        }

        protected override string GetUrl()
        {
            return "https://www.yahoo.com/news/weather/";
        }

        private WeatherType GetWeatherTypeFromPattern(string content)
        {
            int num = 0;
            int count = this._nameWeatherList.Count;
            while (num < count)
            {
                NameWeatherPair pair = this._nameWeatherList[num];
                if (content.IndexOf(pair.pattern) != -1)
                {
                    NameWeatherPair pair2 = this._nameWeatherList[num];
                    return pair2.weather;
                }
                num++;
            }
            return WeatherType.Sunny;
        }

        protected override WeatherInfo ParseWeatherInfo(string content)
        {
            string str = this.FetchInnerTextByPattern(content, "WeatherLocationAndTemperature.0.1.1.0.1");
            if (str == null)
            {
                return null;
            }
            string s = this.FetchInnerTextByPattern(content, "WeatherLocationAndTemperature.0.1.1.2.0");
            if (s == null)
            {
                return null;
            }
            WeatherInfo info = new WeatherInfo {
                weatherType = this.GetWeatherTypeFromPattern(str)
            };
            if (!float.TryParse(s, out info.temperature))
            {
                info.temperature = -100f;
            }
            info.temperature = (info.temperature - 32f) / 1.8f;
            return info;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NameWeatherPair
        {
            public string pattern;
            public WeatherType weather;
        }
    }
}

