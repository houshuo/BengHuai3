namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class RealTimeWeatherManager
    {
        private bool _available = true;
        private ConfigRealTimeWeather _config = Resources.Load<ConfigRealTimeWeather>("RealTimeWeather/ConfigRealTimeWeather");
        private int _failingCounter;
        private DateTime _lastQueryTime;
        private WeatherInfoProvider[] _weatherInfoProviders;
        private const string REALTIME_WEATHER_CONFIG_PATH = "RealTimeWeather/ConfigRealTimeWeather";

        public RealTimeWeatherManager()
        {
            this._weatherInfoProviders = WeatherInfoProvider.CreateWeatherInfoProviders(this._config.siteUsage);
            if (this._weatherInfoProviders.Length == 0)
            {
                this._available = false;
            }
            else
            {
                int index = 0;
                int length = this._weatherInfoProviders.Length;
                while (index < length)
                {
                    this._weatherInfoProviders[index].Init();
                    index++;
                }
            }
        }

        public DateTime GetLastQueryTime()
        {
            return this._lastQueryTime;
        }

        public void GetWeatherConfig(WeatherType type, out string configName, out int sceneId)
        {
            configName = this._config.defaultConfigName;
            sceneId = this._config.defaultSceneId;
            int index = 0;
            int length = this._config.weatherMatch.Length;
            while (index < length)
            {
                if (this._config.weatherMatch[index].realTimeWeatherType == type)
                {
                    float num3 = UnityEngine.Random.value;
                    int num4 = 0;
                    int num5 = this._config.weatherMatch[index].weatherPatterns.Length;
                    while (num4 < num5)
                    {
                        float num6 = num3 * num5;
                        float num7 = num4 + 1;
                        if (num6 < num7)
                        {
                            configName = this._config.weatherMatch[index].weatherPatterns[num4].configName;
                            sceneId = this._config.weatherMatch[index].weatherPatterns[num4].sceneId;
                            break;
                        }
                        num4++;
                    }
                    break;
                }
                index++;
            }
        }

        public WeatherInfo GetWeatherInfo()
        {
            return Singleton<MiHoYoGameData>.Instance.LocalData.LastWeatherInfo;
        }

        private void IncreateFail()
        {
            if (this._failingCounter == this._config.maxRetryTime)
            {
                this._available = false;
            }
            else
            {
                this._failingCounter++;
            }
        }

        public bool IsReadyToRetryQuery()
        {
            if ((this.GetWeatherInfo().weatherType != WeatherType.None) && !this.IsWeatherInfoExpired())
            {
                return false;
            }
            return (DateTime.Now.Subtract(this._lastQueryTime).TotalSeconds >= this._config.retryInterval);
        }

        public bool IsWeatherInfoExpired()
        {
            WeatherInfo weatherInfo = this.GetWeatherInfo();
            return ((weatherInfo.weatherType == WeatherType.None) || (DateTime.Now.Subtract(weatherInfo.infoTime).TotalSeconds >= this._config.weatherInfoPeriod));
        }

        public void QueryWeatherInfo(Action<WeatherInfo> callback = null)
        {
            if (this._available)
            {
                this._lastQueryTime = DateTime.Now;
                this.QueryWeatherInfoByIndexOfProvider(0, callback);
            }
        }

        private void QueryWeatherInfoByIndexOfProvider(int index, Action<WeatherInfo> callback = null)
        {
            <QueryWeatherInfoByIndexOfProvider>c__AnonStoreyDD ydd = new <QueryWeatherInfoByIndexOfProvider>c__AnonStoreyDD {
                index = index,
                callback = callback,
                <>f__this = this
            };
            if ((ydd.index >= 0) && (ydd.index < this._weatherInfoProviders.Length))
            {
                WeatherInfoProvider provider = this._weatherInfoProviders[ydd.index];
                if (provider != null)
                {
                    provider.QueryWeatherInfo(new Action<WeatherInfo>(ydd.<>m__100), new Action(ydd.<>m__101));
                }
            }
        }

        public bool Available
        {
            get
            {
                return (this._available && Singleton<MiHoYoGameData>.Instance.LocalData.EnableRealTimeWeather);
            }
        }

        [CompilerGenerated]
        private sealed class <QueryWeatherInfoByIndexOfProvider>c__AnonStoreyDD
        {
            internal RealTimeWeatherManager <>f__this;
            internal Action<WeatherInfo> callback;
            internal int index;

            internal void <>m__100(WeatherInfo info)
            {
                if (info == null)
                {
                    if ((this.index + 1) < this.<>f__this._weatherInfoProviders.Length)
                    {
                        this.<>f__this.QueryWeatherInfoByIndexOfProvider(this.index + 1, this.callback);
                    }
                    else
                    {
                        this.<>f__this.IncreateFail();
                    }
                }
                else
                {
                    info.infoTime = DateTime.Now;
                    Singleton<MiHoYoGameData>.Instance.LocalData.LastWeatherInfo = info;
                    Singleton<MiHoYoGameData>.Instance.Save();
                    if (this.callback != null)
                    {
                        this.callback(info);
                    }
                }
            }

            internal void <>m__101()
            {
                if ((this.index + 1) < this.<>f__this._weatherInfoProviders.Length)
                {
                    this.<>f__this.QueryWeatherInfoByIndexOfProvider(this.index + 1, this.callback);
                }
                else
                {
                    this.<>f__this.IncreateFail();
                }
            }
        }
    }
}

