namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class WeaponMetaDataReaderExtend
    {
        private static Dictionary<int, int> _evoPreDict;
        private static List<int> _path;

        public static List<int> GetEvoPath(int id)
        {
            _path.Clear();
            _path.Add(id);
            int key = id;
            while (_evoPreDict.ContainsKey(key))
            {
                key = _evoPreDict[key];
                _path.Insert(0, key);
            }
            return _path;
        }

        public static void LoadFromFileAndBuildMap()
        {
            WeaponMetaDataReader.LoadFromFile();
            _evoPreDict = new Dictionary<int, int>();
            _path = new List<int>();
            foreach (WeaponMetaData data in WeaponMetaDataReader.GetItemList())
            {
                if (data.evoID > 0)
                {
                    _evoPreDict[data.evoID] = data.ID;
                }
            }
        }
    }
}

