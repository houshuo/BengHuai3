namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MiHoYoGameData
    {
        private GeneralLocalDataItem _generalData;
        private UserLocalDataItem _userLocalData = null;
        private const string GENERAL_DATA_KEY = "GENERAL_DATA";

        private MiHoYoGameData()
        {
        }

        private void CheckThreadSafe()
        {
            PlayerPrefs.HasKey("TestThread");
        }

        private void CheckVersionAndClearIfNeed()
        {
            if ((this._generalData != null) && (this._generalData.UserLocalDataVersion != LocalDataVersion.version))
            {
                DeleteAllData();
                this._generalData = new GeneralLocalDataItem();
                this._generalData.UserLocalDataVersion = LocalDataVersion.version;
                this.SaveGeneralData();
            }
        }

        public static void DeleteAllData()
        {
            PlayerPrefs.DeleteAll();
        }

        private string GetPrefsKey()
        {
            int userId = Singleton<PlayerModule>.Instance.playerData.userId;
            return ("USD_" + userId);
        }

        private void InitGeneralData()
        {
            this._generalData = new GeneralLocalDataItem();
            this.SaveGeneralData();
        }

        private void InitUserLocalData()
        {
            this._userLocalData = new UserLocalDataItem();
            this.Save();
        }

        public void Save()
        {
            this.CheckThreadSafe();
            string str = ConfigUtil.SaveJSONStrConfig<UserLocalDataItem>(this._userLocalData);
            PlayerPrefs.SetString(this.GetPrefsKey(), str);
        }

        public void SaveGeneralData()
        {
            this.CheckThreadSafe();
            string str = ConfigUtil.SaveJSONStrConfig<GeneralLocalDataItem>(this._generalData);
            PlayerPrefs.SetString("GENERAL_DATA", str);
        }

        public GeneralLocalDataItem GeneralLocalData
        {
            get
            {
                this.CheckVersionAndClearIfNeed();
                if (this._generalData == null)
                {
                    if (!PlayerPrefs.HasKey("GENERAL_DATA"))
                    {
                        this.InitGeneralData();
                    }
                    else
                    {
                        this._generalData = ConfigUtil.LoadJSONStrConfig<GeneralLocalDataItem>(PlayerPrefs.GetString("GENERAL_DATA"));
                        if (this._generalData == null)
                        {
                            this.InitGeneralData();
                        }
                    }
                }
                return this._generalData;
            }
        }

        public UserLocalDataItem LocalData
        {
            get
            {
                this.CheckVersionAndClearIfNeed();
                if (this._userLocalData == null)
                {
                    string prefsKey = this.GetPrefsKey();
                    if (!PlayerPrefs.HasKey(prefsKey))
                    {
                        this.InitUserLocalData();
                    }
                    else
                    {
                        this._userLocalData = ConfigUtil.LoadJSONStrConfig<UserLocalDataItem>(PlayerPrefs.GetString(prefsKey));
                        if (this._userLocalData == null)
                        {
                            this.InitUserLocalData();
                        }
                    }
                }
                return this._userLocalData;
            }
        }
    }
}

