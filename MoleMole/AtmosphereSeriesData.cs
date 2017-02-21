namespace MoleMole
{
    using MoleMole.MainMenu;
    using System;
    using System.Collections.Generic;

    public class AtmosphereSeriesData
    {
        private static ConfigAtmosphereSeriesEntry[] _allLevelClearedItems;
        private static bool _hasCheckedAllLevelsCleared;
        private static bool _isAllLevelsCleared;
        private static ConfigAtmosphereSeriesEntry[] _items;

        public static int GetId(string path)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].Path == path)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetIdRandomly()
        {
            int[] rates = new int[Items.Length];
            for (int i = 0; i < Items.Length; i++)
            {
                rates[i] = Items[i].ChooseRate;
            }
            return ConfigAtmosphereUtil.ChooseRandomly(rates);
        }

        public static int GetNextId(int curId)
        {
            int num = curId + 1;
            if (num >= Items.Length)
            {
                num = 0;
            }
            return num;
        }

        public static string GetPath(int id)
        {
            return Items[id].Path;
        }

        public static string GetPathRandomly()
        {
            return GetPath(GetIdRandomly());
        }

        private static ChapterDataItem GetTheLastStoryChapter(List<ChapterDataItem> ls)
        {
            int chapterId = -1;
            ChapterDataItem item = null;
            foreach (ChapterDataItem item2 in ls)
            {
                if ((item2.ChapterType == ChapterDataItem.ChpaterType.MainStory) && (item2.chapterId > chapterId))
                {
                    chapterId = item2.chapterId;
                    item = item2;
                }
            }
            return item;
        }

        public static bool IsAllLevelsCleared()
        {
            if (!_hasCheckedAllLevelsCleared)
            {
                _isAllLevelsCleared = false;
                try
                {
                    ChapterDataItem theLastStoryChapter = GetTheLastStoryChapter(Singleton<LevelModule>.Instance.AllChapterList);
                    List<int> chapterLevelIdList = LevelMetaDataReaderExtend.GetChapterLevelIdList(theLastStoryChapter.chapterId);
                    chapterLevelIdList.Sort();
                    foreach (LevelDataItem item2 in theLastStoryChapter.GetAllLevelList())
                    {
                        if (item2.levelId == chapterLevelIdList[chapterLevelIdList.Count - 1])
                        {
                            _isAllLevelsCleared = item2.status != 1;
                            break;
                        }
                    }
                    _hasCheckedAllLevelsCleared = true;
                }
                catch
                {
                    return false;
                }
            }
            return _isAllLevelsCleared;
        }

        public static void ReloadFromFile()
        {
            _items = ConfigUtil.LoadConfig<ConfigAtmosphereSeriesRegistry>(GlobalDataManager.metaConfig.atmosphereRegistryPath).Items;
            _allLevelClearedItems = ConfigUtil.LoadConfig<ConfigAtmosphereSeriesRegistry>(GlobalDataManager.metaConfig.allLevelsClearedAtmosphereRegistryPath).Items;
        }

        public static ConfigAtmosphereSeriesEntry[] Items
        {
            get
            {
                return (!IsAllLevelsCleared() ? _items : _allLevelClearedItems);
            }
        }
    }
}

