namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class LevelMetaDataReaderExtend
    {
        private static Dictionary<int, List<int>> _chapterMap;

        public static List<int> GetChapterLevelIdList(int chapterId)
        {
            if (!_chapterMap.ContainsKey(chapterId))
            {
                return new List<int>();
            }
            return _chapterMap[chapterId];
        }

        public static void LoadFromFileAndBuildMap()
        {
            LevelMetaDataReader.LoadFromFile();
            List<LevelMetaData> itemList = LevelMetaDataReader.GetItemList();
            _chapterMap = new Dictionary<int, List<int>>();
            foreach (LevelMetaData data in itemList)
            {
                if (!_chapterMap.ContainsKey(data.chapterId))
                {
                    _chapterMap.Add(data.chapterId, new List<int>());
                }
                _chapterMap[data.chapterId].Add(data.levelId);
            }
        }
    }
}

