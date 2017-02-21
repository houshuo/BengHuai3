namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoWeatherTest : MonoBehaviour
    {
        private void OnGUI()
        {
            if (GUILayout.Button("Query Weather", new GUILayoutOption[0]))
            {
            }
            WeatherInfo weatherInfo = Singleton<RealTimeWeatherManager>.Instance.GetWeatherInfo();
            if (weatherInfo != null)
            {
                GUILayout.Label("WeatherType:" + weatherInfo.weatherType.ToString(), new GUILayoutOption[0]);
                GUILayout.Label("Temperature:" + weatherInfo.temperature.ToString(), new GUILayoutOption[0]);
                GUILayout.Label((("ExtraInfo:" + weatherInfo.extraInfo) == null) ? string.Empty : weatherInfo.extraInfo, new GUILayoutOption[0]);
            }
            else
            {
                GUILayout.Label("<No Weather Info>", new GUILayoutOption[0]);
            }
        }
    }
}

