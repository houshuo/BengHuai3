namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class ChapterDataItem
    {
        private Dictionary<LevelDiffculty, List<LevelDataItem>> _chapterLevelDict;
        private ChapterMetaData _metaData;
        [CompilerGenerated]
        private static Predicate<LevelChallengeDataItem> <>f__am$cache4;
        public int chapterId;
        public bool Unlocked;

        public ChapterDataItem(int chapterId)
        {
            this.chapterId = chapterId;
            this._metaData = ChapterMetaDataReader.GetChapterMetaDataByKey(chapterId);
            this.Unlocked = false;
            this._chapterLevelDict = new Dictionary<LevelDiffculty, List<LevelDataItem>>();
        }

        public List<LevelDataItem> AddLevel(LevelDataItem m_level)
        {
            if (m_level.LevelType == 1)
            {
                return this.AddStoryLevel(m_level);
            }
            return new List<LevelDataItem> { m_level };
        }

        private List<LevelDataItem> AddStoryLevel(LevelDataItem m_level)
        {
            List<LevelDataItem> list = new List<LevelDataItem>();
            foreach (int num in LevelMetaDataReaderExtend.GetChapterLevelIdList(m_level.ChapterID))
            {
                LevelDataItem item = (m_level.levelId != num) ? new LevelDataItem(num) : m_level;
                if (!this._chapterLevelDict.ContainsKey(item.Diffculty))
                {
                    this._chapterLevelDict.Add(item.Diffculty, new List<LevelDataItem>());
                }
                this._chapterLevelDict[item.Diffculty].Add(item);
                list.Add(item);
            }
            return list;
        }

        public List<LevelDataItem> GetAllLevelList()
        {
            List<LevelDataItem> list = new List<LevelDataItem>();
            foreach (LevelDiffculty diffculty in this._chapterLevelDict.Keys)
            {
                list.AddRange(this.GetLevelList(diffculty));
            }
            return list;
        }

        public List<LevelDiffculty> GetLevelDifficultyListInChapter()
        {
            List<LevelDiffculty> list = new List<LevelDiffculty>(this._chapterLevelDict.Keys);
            list.Sort();
            return list;
        }

        public List<LevelDataItem> GetLevelList(LevelDiffculty diffcult = 1)
        {
            if (this._chapterLevelDict.ContainsKey(diffcult))
            {
                return this._chapterLevelDict[diffcult];
            }
            return new List<LevelDataItem>();
        }

        public Dictionary<int, List<LevelDataItem>> GetLevelOfActs(LevelDiffculty difficulty = 1)
        {
            Dictionary<int, List<LevelDataItem>> dictionary = new Dictionary<int, List<LevelDataItem>>();
            foreach (LevelDataItem item in this.GetLevelList(difficulty))
            {
                if (!dictionary.ContainsKey(item.ActID))
                {
                    dictionary.Add(item.ActID, new List<LevelDataItem>());
                }
                dictionary[item.ActID].Add(item);
            }
            return dictionary;
        }

        public int GetTotalFinishedChanllengeNum(LevelDiffculty difficulty)
        {
            int num = 0;
            List<LevelDataItem> levelList = this.GetLevelList(difficulty);
            int num2 = 0;
            int count = levelList.Count;
            while (num2 < count)
            {
                if (levelList[num2].status != 1)
                {
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = x => x.Finished;
                    }
                    num += levelList[num2].challengeList.FindAll(<>f__am$cache4).Count;
                }
                num2++;
            }
            return num;
        }

        public bool HasLevelsOfAct(LevelDiffculty difficulty, int act)
        {
            return this.GetLevelOfActs(difficulty).ContainsKey(act);
        }

        public bool HasLevelsOfDifficulty(LevelDiffculty difficulty)
        {
            return this._chapterLevelDict.ContainsKey(difficulty);
        }

        public string BtnPointName
        {
            get
            {
                return ("BtnPoint_" + this.chapterId);
            }
        }

        public string ChapterPrefabPath
        {
            get
            {
                return ("UI/Menus/Widget/Map/Chapter/ChapterMap_" + this.chapterId);
            }
        }

        public ChpaterType ChapterType
        {
            get
            {
                return (ChpaterType) this._metaData.chapterType;
            }
        }

        public string CoverPic
        {
            get
            {
                return this._metaData.coverPicture;
            }
        }

        public List<int> MonsterIDList
        {
            get
            {
                return this._metaData.monsterList;
            }
        }

        public string Title
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.title, new object[0]);
            }
        }

        public enum ChpaterType
        {
            Event = 2,
            MainStory = 1,
            SpecialStory = 3
        }
    }
}

