namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MiscData
    {
        private static Dictionary<int, string> _currencyIconPathDict;
        private static Dictionary<int, string> _featureUnlockLevelDict;
        private static Dictionary<int, string> _gachaTicketIconPathDict;

        public static string AddColor(string key, string text)
        {
            string str = ColorUtility.ToHtmlStringRGBA(GetColor(key));
            return string.Format("<color=#{0}>{1}</color>", str, text);
        }

        public static Color GetColor(string key)
        {
            return Miscs.ParseColor(Config.Color[key].ToString());
        }

        public static string GetCurrencyIconPath(int metaID)
        {
            if (!_currencyIconPathDict.ContainsKey(metaID))
            {
                return null;
            }
            return _currencyIconPathDict[metaID];
        }

        public static int GetEquipPowerUpResultIndex(int boostRate)
        {
            for (int i = 0; i < Config.EquipPowerUpBoostRateResult.Count; i++)
            {
                if ((boostRate >= Config.EquipPowerUpBoostRateResult[i]) && ((i == (Config.EquipPowerUpBoostRateResult.Count - 1)) || (boostRate < Config.EquipPowerUpBoostRateResult[i + 1])))
                {
                    return (i + 1);
                }
            }
            return 1;
        }

        public static string GetGachaTicketIconPath(int metaID)
        {
            if (!_gachaTicketIconPathDict.ContainsKey(metaID))
            {
                return null;
            }
            return _gachaTicketIconPathDict[metaID];
        }

        public static List<string> GetNewFeatures(int levelBefore, int levelNow)
        {
            List<string> list = new List<string>();
            if (levelBefore < levelNow)
            {
                foreach (KeyValuePair<int, string> pair in _featureUnlockLevelDict)
                {
                    if ((pair.Key > levelBefore) && (pair.Key <= levelNow))
                    {
                        list.Add(pair.Value);
                    }
                }
            }
            return list;
        }

        public static ConfigPageAvatarShowInfo GetPageAvatarShowInfo(PageInfoKey pageKey)
        {
            return Config.PageAvatarShowInfo[(int) pageKey];
        }

        public static ConfigPlotAvatarCameraPosInfo GetPlotAvatarCameraPosInfo()
        {
            return Config.PlotAvatarCameraPosInfo;
        }

        public static void LoadFromFile()
        {
            Config = ConfigUtil.LoadJSONConfig<ConfigMisc>("Data/MiscData");
            _featureUnlockLevelDict = new Dictionary<int, string>();
            foreach (KeyValuePair<string, object> pair in Config.FeatureUnlockLevel)
            {
                int introduced6 = int.Parse(pair.Key);
                _featureUnlockLevelDict[introduced6] = pair.Value.ToString();
            }
            _currencyIconPathDict = new Dictionary<int, string>();
            foreach (KeyValuePair<string, object> pair2 in Config.CurrencyIconPath)
            {
                int introduced7 = int.Parse(pair2.Key);
                _currencyIconPathDict[introduced7] = pair2.Value.ToString();
            }
            _gachaTicketIconPathDict = new Dictionary<int, string>();
            foreach (KeyValuePair<string, object> pair3 in Config.GachaTicketIconPath)
            {
                int introduced8 = int.Parse(pair3.Key);
                _gachaTicketIconPathDict[introduced8] = pair3.Value.ToString();
            }
        }

        public static ConfigMisc Config
        {
            [CompilerGenerated]
            get
            {
                return <Config>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <Config>k__BackingField = value;
            }
        }

        public enum PageInfoKey
        {
            GameEntryPage,
            MainPage,
            AvatarOverviewPage,
            AvatarDetailPage
        }
    }
}

