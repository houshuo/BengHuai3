namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;

    [Serializable]
    public class GeneralLocalDataItem
    {
        [SerializeField]
        private AccountData _account = new AccountData();
        [SerializeField]
        private string _lastLoginAccountName;
        [SerializeField]
        private int _lastLoginUserId = 0;
        [SerializeField]
        private ConfigAudioSetting _personalAudioSetting = new ConfigAudioSetting();
        [SerializeField]
        private ConfigGraphicsPersonalSetting _personalGraphicsSetting = new ConfigGraphicsPersonalSetting();
        [SerializeField]
        private Dictionary<System.Type, int> _uiStatistics = new Dictionary<System.Type, int>();
        [SerializeField]
        private string _userLocalDataVersionId;
        [CompilerGenerated]
        private static Func<KeyValuePair<System.Type, int>, int> <>f__am$cache7;

        public void AddContextShowCount(BaseContext context)
        {
            if (((MiscData.Config != null) && MiscData.Config.CollectUIStatistics) && (((context.uiType == UIType.Page) || (context.uiType == UIType.Dialog)) || (context.uiType == UIType.SpecialDialog)))
            {
                int num = 0;
                this._uiStatistics.TryGetValue(context.GetType(), out num);
                this._uiStatistics[context.GetType()] = num + 1;
                Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
            }
        }

        public void ClearLastLoginUser()
        {
            this.LastLoginAccountName = this._account.GetAccountName();
            this._account = new AccountData();
            this._lastLoginUserId = 0;
        }

        public void ReportUIStatistics()
        {
            if (((MiscData.Config != null) && MiscData.Config.CollectUIStatistics) && ((this._uiStatistics != null) && (this._uiStatistics.Count > 0)))
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("UI Statistics:");
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = p => p.Value;
                }
                IEnumerator<KeyValuePair<System.Type, int>> enumerator = Enumerable.OrderByDescending<KeyValuePair<System.Type, int>, int>(this._uiStatistics, <>f__am$cache7).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<System.Type, int> current = enumerator.Current;
                        builder.AppendLine(string.Format("{0}={1}", current.Key.Name, current.Value));
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                Singleton<QAManager>.GetInstance().SendMessageToSever(builder.ToString(), string.Empty);
            }
            if ((this._uiStatistics != null) && (this._uiStatistics.Count > 0))
            {
                this._uiStatistics.Clear();
                Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
            }
        }

        public AccountData Account
        {
            get
            {
                if (this._account == null)
                {
                    this._account = new AccountData();
                }
                return this._account;
            }
            set
            {
                this._account = value;
            }
        }

        public string LastLoginAccountName
        {
            get
            {
                return this._lastLoginAccountName;
            }
            set
            {
                this._lastLoginAccountName = value;
            }
        }

        public int LastLoginUserId
        {
            get
            {
                return this._lastLoginUserId;
            }
            set
            {
                this._lastLoginUserId = value;
            }
        }

        public ConfigAudioSetting PersonalAudioSetting
        {
            get
            {
                return this._personalAudioSetting;
            }
            set
            {
                this._personalAudioSetting = value;
            }
        }

        public ConfigGraphicsPersonalSetting PersonalGraphicsSetting
        {
            get
            {
                return this._personalGraphicsSetting;
            }
            set
            {
                this._personalGraphicsSetting = value;
            }
        }

        public string UserLocalDataVersion
        {
            get
            {
                if (string.IsNullOrEmpty(this._userLocalDataVersionId))
                {
                    this._userLocalDataVersionId = LocalDataVersion.version;
                }
                return this._userLocalDataVersionId;
            }
            set
            {
                this._userLocalDataVersionId = value;
            }
        }

        public class AccountData
        {
            public string email;
            public string ext;
            public bool isEmailVerify;
            public bool isRealNameVerify;
            public string mobile;
            public string name;
            public string token;
            public string uid;

            public string GetAccountName()
            {
                string[] strArray = new string[] { this.name, this.email, this.mobile };
                int index = 0;
                int length = strArray.Length;
                while (index < length)
                {
                    if (!string.IsNullOrEmpty(strArray[index]))
                    {
                        return strArray[index];
                    }
                    index++;
                }
                return string.Empty;
            }
        }
    }
}

