namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class LevelDataItem
    {
        private int _actID;
        private int _chapterID;
        private LevelMetaData _metaData;
        [CompilerGenerated]
        private static Predicate<LevelChallengeDataItem> <>f__am$cache14;
        public List<LevelChallengeDataItem> challengeList;
        public List<int> displayBonusDropList;
        public List<int> displayDropList;
        public List<int> displayFirstDropList;
        public DateTime dropActivityEndTime;
        public int dropActivityEnterTimes;
        public int dropActivityMaxEnterTimes;
        public bool dropDisplayInfoReceived;
        public int enterTimes;
        public bool Initialized;
        public bool isDoubleDrop;
        public bool isDropActivityOpen;
        public bool isNewLevel;
        public int levelId;
        public int progress;
        public int resetTimes;
        public StageStatus status;

        public LevelDataItem(int levelID) : this(levelID, LevelMetaDataReader.GetLevelMetaDataByKey(levelID))
        {
        }

        public LevelDataItem(int levelId, LevelMetaData levelMetaData)
        {
            this.levelId = levelId;
            this._metaData = levelMetaData;
            this.Initialized = false;
            this.status = 1;
            this.challengeList = new List<LevelChallengeDataItem>();
            foreach (LevelMetaData.LevelChallengeMetaNode node in this._metaData.challengeList)
            {
                this.challengeList.Add(new LevelChallengeDataItem(node.challengeId, this._metaData, node.rewardId));
            }
            this.isNewLevel = true;
            this._actID = this._metaData.actId;
            this._chapterID = this._metaData.chapterId;
            this.isDropActivityOpen = false;
            this.isDoubleDrop = false;
            this.displayDropList = new List<int>();
            this.displayFirstDropList = new List<int>();
            this.displayBonusDropList = new List<int>();
            this.dropDisplayInfoReceived = false;
        }

        public List<int> GetAllChallengeIdList()
        {
            List<int> list = new List<int>();
            foreach (LevelChallengeDataItem item in this.challengeList)
            {
                list.Add(item.challengeId);
            }
            return list;
        }

        public Color GetBattleTypeColor()
        {
            return Miscs.ParseColor(BattleTypeMetaDataReader.GetBattleTypeMetaDataByKey(this._metaData.battleType).colorCode);
        }

        public Sprite GetBattleTypeSprite()
        {
            return Miscs.GetSpriteByPrefab(BattleTypeMetaDataReader.GetBattleTypeMetaDataByKey(this._metaData.battleType).iconPath);
        }

        public Sprite GetBriefPicSprite()
        {
            return Miscs.GetSpriteByPrefab(this._metaData.briefPicPath);
        }

        public Sprite GetDetailPicSprite()
        {
            return Miscs.GetSpriteByPrefab(this._metaData.detailPicPath);
        }

        public int GetHCoinSpentToResetLevel(int resetTimes)
        {
            return LevelResetCostMetaDataReader.GetLevelResetCostMetaDataByKey(resetTimes).costList[this._metaData.resetCostType];
        }

        public int GetReviveCost(int reviveTime)
        {
            if (ReviveCostTypeMetaDataReader.GetItemList().Count < reviveTime)
            {
                reviveTime = ReviveCostTypeMetaDataReader.GetItemList().Count;
            }
            return ReviveCostTypeMetaDataReader.GetReviveCostTypeMetaDataByKey(reviveTime).costList[this._metaData.reviveCostType];
        }

        public List<int> GetTrackChallengeIdList()
        {
            if (<>f__am$cache14 == null)
            {
                <>f__am$cache14 = x => !x.Finished || x.IsSpecialChallenge();
            }
            List<LevelChallengeDataItem> list = this.challengeList.FindAll(<>f__am$cache14);
            List<int> list2 = new List<int>();
            foreach (LevelChallengeDataItem item in list)
            {
                list2.Add(item.challengeId);
            }
            return list2;
        }

        public void Init()
        {
            if (this.LevelType == 1)
            {
                this.status = 2;
            }
            else
            {
                this.status = 1;
            }
            this.Initialized = true;
        }

        public void OnProgressUpdate(int preValue, int newValue)
        {
            if (((preValue != newValue) && (this.LevelType == 1)) && (newValue == this._metaData.maxProgress))
            {
                this.status = 3;
            }
        }

        public int ActID
        {
            get
            {
                return this._actID;
            }
            set
            {
                this._actID = value;
            }
        }

        public float AvatarExpFixed
        {
            get
            {
                return (this._metaData.avatarExpReward * (1f - this._metaData.avatarExpInside));
            }
        }

        public float AvatarExpInside
        {
            get
            {
                return (this._metaData.avatarExpReward * this._metaData.avatarExpInside);
            }
        }

        public LevelActor.Mode BattleType
        {
            get
            {
                return ((this._metaData.battleType <= 100) ? ((LevelActor.Mode) (this._metaData.battleType - 1)) : LevelActor.Mode.Single);
            }
        }

        public string BattleTypePath
        {
            get
            {
                return BattleTypeMetaDataReader.GetBattleTypeMetaDataByKey(this._metaData.battleType).iconPath;
            }
        }

        public string BtnPointName
        {
            get
            {
                return ("BtnPoint_" + (this._metaData.levelId % 100));
            }
        }

        public int ChapterID
        {
            get
            {
                return this._chapterID;
            }
            set
            {
                this._chapterID = value;
            }
        }

        public string Desc
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.displayDetail, new object[0]);
            }
        }

        public LevelDiffculty Diffculty
        {
            get
            {
                return (LevelDiffculty) this._metaData.difficulty;
            }
        }

        public bool IsMultMode
        {
            get
            {
                return ((this._metaData.battleType == 2) || (this._metaData.battleType == 3));
            }
        }

        public bool IsNormalBattleType
        {
            get
            {
                return (this._metaData.battleType == 1);
            }
        }

        public StageType LevelType
        {
            get
            {
                return (StageType) this._metaData.type;
            }
        }

        public string LevelTypeName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.LevelTypeName[this._metaData.type], new object[0]);
            }
        }

        public List<string> LoseDescList
        {
            get
            {
                return this._metaData.loseDescList;
            }
        }

        public string LuaFile
        {
            get
            {
                return this._metaData.luaFile;
            }
        }

        public int MaxEnterTimes
        {
            get
            {
                return this._metaData.enterTimes;
            }
        }

        public int MaxResetTimes
        {
            get
            {
                return this._metaData.resetTimes;
            }
        }

        public int maxReviveTimes
        {
            get
            {
                return this._metaData.reviveTimes;
            }
        }

        public int MinEnterAvatarNum
        {
            get
            {
                return this._metaData.MinEnterNum;
            }
        }

        public int NPCHardLevel
        {
            get
            {
                return this._metaData.hardLevel;
            }
        }

        public int RecommandLv
        {
            get
            {
                return this._metaData.recommendPlayerLevel;
            }
        }

        public int ResetCostType
        {
            get
            {
                return this._metaData.resetCostType;
            }
        }

        public float ScoinFixed
        {
            get
            {
                return (this._metaData.scoinReward * (1f - this._metaData.scoinInside));
            }
        }

        public float ScoinInside
        {
            get
            {
                return (this._metaData.scoinReward * this._metaData.scoinInside);
            }
        }

        public int SectionId
        {
            get
            {
                return this._metaData.sectionId;
            }
        }

        public string StageName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.name, new object[0]);
            }
        }

        public int StaminaCost
        {
            get
            {
                return this._metaData.staminaCost;
            }
        }

        public string Title
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.displayTitle, new object[0]);
            }
        }

        public int UnlockChanllengeNum
        {
            get
            {
                return this._metaData.unlockStarNum;
            }
        }

        public int UnlockPlayerLevel
        {
            get
            {
                return this._metaData.unlockPlayerLevel;
            }
        }
    }
}

