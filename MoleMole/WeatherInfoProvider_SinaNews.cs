namespace MoleMole
{
    using System;

    public class WeatherInfoProvider_SinaNews : WeatherInfoProvider
    {
        protected override string GetUrl()
        {
            return "http://php.weather.sina.com.cn/iframe_weather.php?type=w";
        }

        protected override WeatherInfo ParseWeatherInfo(string content)
        {
            WeatherInfo info = new WeatherInfo();
            if (content.IndexOf("180_180/qing_0") != -1)
            {
                info.weatherType = WeatherType.Sunny;
            }
            else if ((content.IndexOf("180_180/duoyun_0") != -1) || (content.IndexOf("180_180/yin_0") != -1))
            {
                info.weatherType = WeatherType.Sunny;
            }
            else if ((((content.IndexOf("180_180/xiaoyu_0") != -1) || (content.IndexOf("180_180/zhongyuyu_0") != -1)) || ((content.IndexOf("180_180/dayu_0") != -1) || (content.IndexOf("180_180/baoyu_0") != -1))) || (((content.IndexOf("180_180/zhenyu_0") != -1) || (content.IndexOf("180_180/tedabaoyu_0") != -1)) || (content.IndexOf("180_180/yujiaxue_0") != -1)))
            {
                info.weatherType = WeatherType.Rainy;
            }
            else if (content.IndexOf("180_180/leizhenyu_0") != -1)
            {
                info.weatherType = WeatherType.Lightning;
            }
            else
            {
                info.weatherType = WeatherType.Sunny;
            }
            info.temperature = 30f;
            info.extraInfo = content.Length.ToString();
            return info;
        }
    }
}

