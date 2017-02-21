namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoBGWeather : MonoBehaviour
    {
        private void Start()
        {
            string str;
            int hour = TimeUtil.Now.Hour;
            if (TimeUtil.Now < Singleton<MiHoYoGameData>.Instance.LocalData.EndThunderDateTime)
            {
                str = ((hour < 6) || (hour > 0x15)) ? MiscData.Config.WeatherBgPath[0x19] : MiscData.Config.WeatherBgPath[0x18];
            }
            else
            {
                str = MiscData.Config.WeatherBgPath[hour];
            }
            base.transform.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(str);
        }
    }
}

