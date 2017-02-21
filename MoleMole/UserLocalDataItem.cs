namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class UserLocalDataItem
    {
        [SerializeField]
        private List<MailCacheKey> _allMailSet = new List<MailCacheKey>();
        [SerializeField]
        private Dictionary<CabinType, bool> _cabinNeedToShowLevelUpCompleteSet;
        [SerializeField]
        private Dictionary<CabinType, bool> _cabinNeedToShowNewUnlockDict;
        [SerializeField]
        private Dictionary<int, bool> _cabinTechTreeNodeVisited;
        [SerializeField]
        private string _currentWeatherConfigPath;
        [SerializeField]
        private int _currentWeatherSceneID = 0;
        [SerializeField]
        private int _difficulty = 1;
        [SerializeField]
        private List<string> _doneBehaviourList = new List<string>();
        [SerializeField]
        private bool _enableRealTimeWeather = true;
        [SerializeField]
        private DateTime _endThunderWeatherTime = TimeUtil.Now;
        [SerializeField]
        private Dictionary<int, List<ChatMsgDataItem>> _friendChatMsgMap = new Dictionary<int, List<ChatMsgDataItem>>();
        [SerializeField]
        private bool _hasShowInviteHintDialog = false;
        private bool _isDirty = true;
        [SerializeField]
        private int _lastActIndex = 0;
        [SerializeField]
        private int _lastChapterID = 1;
        [SerializeField]
        private DateTime _lastCrashReportTime;
        [SerializeField]
        private int _lastGalAvatarId = 0;
        [SerializeField]
        private GetLastEndlessRewardDataRsp _lastRewardData;
        [SerializeField]
        private DateTime _lastShowBindAccountWarningTime;
        [SerializeField]
        private DateTime _lastShowBindIdentityWarningTime;
        [SerializeField]
        private DateTime _lastShowBulletinTime;
        [SerializeField]
        private WeatherInfo _lastWeatherInfo = new WeatherInfo();
        [SerializeField]
        private uint _loginRandomNum = 0;
        [SerializeField]
        private HashSet<int> _needPlayLevelAnimationSet = new HashSet<int>();
        [SerializeField]
        private DateTime _nextRandomWeatherTime = TimeUtil.Now;
        [SerializeField]
        private HashSet<uint> _oldBulletinIDSet = new HashSet<uint>();
        [SerializeField]
        private HashSet<int> _oldFriendUIDSet = new HashSet<int>();
        [SerializeField]
        private HashSet<int> _oldRequestUIDSet = new HashSet<int>();
        [SerializeField]
        private int _payMethod;
        [SerializeField]
        private ConfigNotificationSetting _personalNotificationSetting = new ConfigNotificationSetting();
        [SerializeField]
        private byte[] _processingStageEndReq;
        [SerializeField]
        private List<MailCacheKey> _readMailIdList = new List<MailCacheKey>();
        [SerializeField]
        private string _receipt;
        [SerializeField]
        private string _storageShowTabName = "WeaponTab";
        [SerializeField]
        private Dictionary<int, SubSkillStatus> _subSkillStatusDict;
        [SerializeField]
        private Dictionary<int, int> _visitedTutorial;

        public bool EndDirtyCheck()
        {
            bool flag = this._isDirty;
            this._isDirty = true;
            return flag;
        }

        private Dictionary<int, SubSkillStatus> GetDefaultSubSkillStatusDict()
        {
            Dictionary<int, SubSkillStatus> dictionary = new Dictionary<int, SubSkillStatus>();
            foreach (AvatarDataItem item in Singleton<AvatarModule>.Instance.UserAvatarList)
            {
                foreach (AvatarSkillDataItem item2 in item.skillDataList)
                {
                    foreach (AvatarSubSkillDataItem item3 in item2.avatarSubSkillList)
                    {
                        if (item3.ShouldShowHintPoint())
                        {
                            dictionary[item3.subSkillID] = item3.Status;
                        }
                    }
                }
            }
            return dictionary;
        }

        public bool IsVisited_CabinTechTreeNode(int id)
        {
            if (this._cabinTechTreeNodeVisited == null)
            {
                this._cabinTechTreeNodeVisited = new Dictionary<int, bool>();
            }
            if (!this._cabinTechTreeNodeVisited.ContainsKey(id))
            {
                this._cabinTechTreeNodeVisited.Add(id, false);
                Singleton<MiHoYoGameData>.Instance.Save();
            }
            return this._cabinTechTreeNodeVisited[id];
        }

        public int IsVisited_Tutorial(int stepID)
        {
            if (this._visitedTutorial == null)
            {
                this._visitedTutorial = new Dictionary<int, int>();
            }
            if (!this._visitedTutorial.ContainsKey(stepID))
            {
                return -1;
            }
            return this._visitedTutorial[stepID];
        }

        public void SetDirty()
        {
            this._isDirty = true;
        }

        public void SetVisited_CabinTechTreeNode(int id)
        {
            if (!this._cabinTechTreeNodeVisited[id])
            {
                this._cabinTechTreeNodeVisited[id] = true;
                Singleton<MiHoYoGameData>.Instance.Save();
            }
        }

        public void SetVisited_Tutorial(int stepID)
        {
            if (!this._visitedTutorial.ContainsKey(stepID))
            {
                this._visitedTutorial.Add(stepID, 0);
                Singleton<MiHoYoGameData>.Instance.Save();
            }
            else if (this._visitedTutorial[stepID] == 0)
            {
                this._visitedTutorial[stepID] = 1;
                Singleton<MiHoYoGameData>.Instance.Save();
            }
        }

        public void StartDirtyCheck()
        {
            this._isDirty = false;
        }

        public Dictionary<CabinType, bool> CabinNeedToShowLevelUpCompleteSet
        {
            get
            {
                if (this._cabinNeedToShowLevelUpCompleteSet == null)
                {
                    this._cabinNeedToShowLevelUpCompleteSet = new Dictionary<CabinType, bool>();
                    this._cabinNeedToShowLevelUpCompleteSet[2] = false;
                    this._cabinNeedToShowLevelUpCompleteSet[6] = false;
                    this._cabinNeedToShowLevelUpCompleteSet[7] = false;
                    this._cabinNeedToShowLevelUpCompleteSet[3] = false;
                    this._cabinNeedToShowLevelUpCompleteSet[4] = false;
                    this._cabinNeedToShowLevelUpCompleteSet[5] = false;
                    this._cabinNeedToShowLevelUpCompleteSet[1] = false;
                }
                return this._cabinNeedToShowLevelUpCompleteSet;
            }
            set
            {
                this._cabinNeedToShowLevelUpCompleteSet = value;
            }
        }

        public Dictionary<CabinType, bool> CabinNeedToShowNewUnlockDict
        {
            get
            {
                if (this._cabinNeedToShowNewUnlockDict == null)
                {
                    this._cabinNeedToShowNewUnlockDict = new Dictionary<CabinType, bool>();
                }
                return this._cabinNeedToShowNewUnlockDict;
            }
            set
            {
                this._cabinNeedToShowNewUnlockDict = value;
            }
        }

        public string CurrentWeatherConfigPath
        {
            get
            {
                if (string.IsNullOrEmpty(this._currentWeatherConfigPath))
                {
                    this._currentWeatherConfigPath = "Rendering/MainMenuAtmosphereConfig/Lightning";
                }
                return this._currentWeatherConfigPath;
            }
            set
            {
                this._currentWeatherConfigPath = value;
            }
        }

        public int CurrentWeatherSceneID
        {
            get
            {
                return this._currentWeatherSceneID;
            }
            set
            {
                this._currentWeatherSceneID = value;
            }
        }

        public List<string> DoneBehaviourList
        {
            get
            {
                if (this._doneBehaviourList == null)
                {
                    this._doneBehaviourList = new List<string>();
                }
                return this._doneBehaviourList;
            }
            set
            {
                this._doneBehaviourList = value;
            }
        }

        public bool EnableRealTimeWeather
        {
            get
            {
                return this._enableRealTimeWeather;
            }
            set
            {
                this._enableRealTimeWeather = value;
            }
        }

        public DateTime EndThunderDateTime
        {
            get
            {
                return this._endThunderWeatherTime;
            }
            set
            {
                this._endThunderWeatherTime = value;
            }
        }

        public Dictionary<int, List<ChatMsgDataItem>> FriendChatMsgMap
        {
            get
            {
                if (this._friendChatMsgMap == null)
                {
                    this._friendChatMsgMap = new Dictionary<int, List<ChatMsgDataItem>>();
                }
                return this._friendChatMsgMap;
            }
            set
            {
                this._friendChatMsgMap = value;
            }
        }

        public bool HasShowInviteHintDialog
        {
            get
            {
                return this._hasShowInviteHintDialog;
            }
            set
            {
                this._hasShowInviteHintDialog = value;
            }
        }

        public bool isDirty
        {
            get
            {
                return this._isDirty;
            }
        }

        public int LastActIndex
        {
            get
            {
                return this._lastActIndex;
            }
            set
            {
                this._lastActIndex = value;
            }
        }

        public int LastChapterID
        {
            get
            {
                if (!Singleton<LevelModule>.Instance.HasChapter(this._lastChapterID))
                {
                    this._lastChapterID = Singleton<LevelModule>.Instance.GetOneUnlockChapterID();
                }
                return this._lastChapterID;
            }
            set
            {
                this._lastChapterID = value;
            }
        }

        public DateTime LastCrashReportTime
        {
            get
            {
                return this._lastCrashReportTime;
            }
            set
            {
                this._lastCrashReportTime = value;
            }
        }

        public LevelDiffculty LastDifficulty
        {
            get
            {
                if (!Singleton<LevelModule>.Instance.GetChapterById(this.LastChapterID).HasLevelsOfDifficulty((LevelDiffculty) this._difficulty))
                {
                    this._difficulty = 1;
                }
                return (LevelDiffculty) ((int) Enum.ToObject(typeof(LevelDiffculty), this._difficulty));
            }
            set
            {
                this._difficulty = (int) value;
            }
        }

        public int LastGalAvatarId
        {
            get
            {
                return this._lastGalAvatarId;
            }
            set
            {
                this._lastGalAvatarId = value;
            }
        }

        public GetLastEndlessRewardDataRsp LastRewardData
        {
            get
            {
                return this._lastRewardData;
            }
            set
            {
                this._lastRewardData = value;
            }
        }

        public DateTime LastShowBindAccountWarningTime
        {
            get
            {
                return this._lastShowBindAccountWarningTime;
            }
            set
            {
                this._lastShowBindAccountWarningTime = value;
            }
        }

        public DateTime LastShowBindIdentityWarningTime
        {
            get
            {
                return this._lastShowBindIdentityWarningTime;
            }
            set
            {
                this._lastShowBindIdentityWarningTime = value;
            }
        }

        public DateTime LastShowBulletinTime
        {
            get
            {
                return this._lastShowBulletinTime;
            }
            set
            {
                this._lastShowBulletinTime = value;
            }
        }

        public WeatherInfo LastWeatherInfo
        {
            get
            {
                return this._lastWeatherInfo;
            }
            set
            {
                this._lastWeatherInfo = value;
            }
        }

        public uint LoginRandomNum
        {
            get
            {
                return this._loginRandomNum;
            }
            set
            {
                this._loginRandomNum = value;
            }
        }

        public HashSet<int> NeedPlayLevelAnimationSet
        {
            get
            {
                if (this._needPlayLevelAnimationSet == null)
                {
                    this._needPlayLevelAnimationSet = new HashSet<int>();
                }
                return this._needPlayLevelAnimationSet;
            }
            set
            {
                this._needPlayLevelAnimationSet = value;
            }
        }

        public DateTime NextRandomDateTime
        {
            get
            {
                return this._nextRandomWeatherTime;
            }
            set
            {
                this._nextRandomWeatherTime = value;
            }
        }

        public HashSet<uint> OldBulletinIDSet
        {
            get
            {
                if (this._oldBulletinIDSet == null)
                {
                    this._oldBulletinIDSet = new HashSet<uint>();
                }
                return this._oldBulletinIDSet;
            }
            set
            {
                this._oldBulletinIDSet = value;
            }
        }

        public HashSet<int> OldFriendUIDSet
        {
            get
            {
                if (this._oldFriendUIDSet == null)
                {
                    this._oldFriendUIDSet = new HashSet<int>();
                }
                return this._oldFriendUIDSet;
            }
            set
            {
                this._oldFriendUIDSet = value;
            }
        }

        public List<MailCacheKey> OldMailCache
        {
            get
            {
                if (this._allMailSet == null)
                {
                    this._allMailSet = new List<MailCacheKey>();
                }
                return this._allMailSet;
            }
            set
            {
                this._allMailSet = value;
            }
        }

        public HashSet<int> OldRequestUIDSet
        {
            get
            {
                if (this._oldRequestUIDSet == null)
                {
                    this._oldRequestUIDSet = new HashSet<int>();
                }
                return this._oldRequestUIDSet;
            }
            set
            {
                this._oldRequestUIDSet = value;
            }
        }

        public int PayMethod
        {
            get
            {
                return this._payMethod;
            }
            set
            {
                this._payMethod = value;
            }
        }

        public ConfigNotificationSetting PersonalNotificationSetting
        {
            get
            {
                return this._personalNotificationSetting;
            }
            set
            {
                this._personalNotificationSetting = value;
            }
        }

        public byte[] ProcessingStageEndReq
        {
            get
            {
                return this._processingStageEndReq;
            }
            set
            {
                this._processingStageEndReq = value;
            }
        }

        public List<MailCacheKey> ReadMailIdList
        {
            get
            {
                if (this._readMailIdList == null)
                {
                    this._readMailIdList = new List<MailCacheKey>();
                }
                return this._readMailIdList;
            }
            set
            {
                this._readMailIdList = value;
            }
        }

        public string Receipt
        {
            get
            {
                if (this._receipt == null)
                {
                    return string.Empty;
                }
                return this._receipt;
            }
            set
            {
                this._receipt = value;
            }
        }

        public string StorageShowTabName
        {
            get
            {
                return this._storageShowTabName;
            }
            set
            {
                this._storageShowTabName = value;
            }
        }

        public Dictionary<int, SubSkillStatus> SubSkillStatusDict
        {
            get
            {
                if (this._subSkillStatusDict == null)
                {
                    this._subSkillStatusDict = this.GetDefaultSubSkillStatusDict();
                }
                return this._subSkillStatusDict;
            }
            set
            {
                this._subSkillStatusDict = value;
            }
        }
    }
}

