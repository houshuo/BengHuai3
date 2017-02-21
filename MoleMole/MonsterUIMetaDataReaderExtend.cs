namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class MonsterUIMetaDataReaderExtend
    {
        private static Dictionary<string, MonsterUIMetaData> _itemDict;

        public static MonsterUIMetaData GetMonsterUIMetaDataByName(string name)
        {
            if (!_itemDict.ContainsKey(name))
            {
                return null;
            }
            return _itemDict[name];
        }

        public static void LoadFromFileAndBuildMap()
        {
            MonsterUIMetaDataReader.LoadFromFile();
            List<MonsterUIMetaData> itemList = MonsterUIMetaDataReader.GetItemList();
            _itemDict = new Dictionary<string, MonsterUIMetaData>();
            foreach (MonsterUIMetaData data in itemList)
            {
                _itemDict.Add(data.name, data);
            }
        }
    }
}

