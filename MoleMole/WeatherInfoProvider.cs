namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public abstract class WeatherInfoProvider
    {
        private static Dictionary<string, string> _staticFakeHeaders;

        static WeatherInfoProvider()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36");
            _staticFakeHeaders = dictionary;
        }

        protected WeatherInfoProvider()
        {
        }

        public static WeatherInfoProvider[] CreateWeatherInfoProviders(ConfigWeatherSiteUsage usage)
        {
            List<WeatherInfoProvider> list = new List<WeatherInfoProvider>();
            if (usage.sinaWeather)
            {
                list.Add(new WeatherInfoProvider_SinaWeather());
            }
            if (usage.yahooWeather)
            {
                list.Add(new WeatherInfoProvider_YahooWeather());
            }
            if (usage.sinaNews)
            {
                list.Add(new WeatherInfoProvider_SinaNews());
            }
            return list.ToArray();
        }

        protected abstract string GetUrl();
        public virtual void Init()
        {
        }

        protected abstract WeatherInfo ParseWeatherInfo(string content);
        public virtual void QueryWeatherInfo(Action<WeatherInfo> callbackSucc, Action callbackFail)
        {
            <QueryWeatherInfo>c__AnonStoreyDC ydc = new <QueryWeatherInfo>c__AnonStoreyDC {
                callbackSucc = callbackSucc,
                <>f__this = this
            };
            Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithTimeOut(this.GetUrl(), new Action<string>(ydc.<>m__FF), callbackFail, 5f, null, _staticFakeHeaders, true));
        }

        [CompilerGenerated]
        private sealed class <QueryWeatherInfo>c__AnonStoreyDC
        {
            internal WeatherInfoProvider <>f__this;
            internal Action<WeatherInfo> callbackSucc;

            internal void <>m__FF(string responseText)
            {
                this.callbackSucc(this.<>f__this.ParseWeatherInfo(responseText));
            }
        }
    }
}

