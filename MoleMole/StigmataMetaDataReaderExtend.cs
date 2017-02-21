namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class StigmataMetaDataReaderExtend
    {
        private static Dictionary<int, HashSet<int>> _evoMap;

        private static HashSet<int> CalculateEvoList(StigmataMetaData meta)
        {
            HashSet<int> set = new HashSet<int>();
            for (int i = meta.evoID; i > 0; i = StigmataMetaDataReader.GetStigmataMetaDataByKey(i).evoID)
            {
                set.Add(i);
            }
            return set;
        }

        public static bool CanEvo(int from, int to)
        {
            return (_evoMap.ContainsKey(from) && _evoMap[from].Contains(to));
        }

        public static bool IsEvoRelation(int id1, int id2)
        {
            return (CanEvo(id1, id2) || CanEvo(id2, id1));
        }

        public static void LoadFromFileAndBuildMap()
        {
            StigmataMetaDataReader.LoadFromFile();
            List<StigmataMetaData> itemList = StigmataMetaDataReader.GetItemList();
            _evoMap = new Dictionary<int, HashSet<int>>();
            foreach (StigmataMetaData data in itemList)
            {
                if (data.evoID > 0)
                {
                    _evoMap[data.ID] = CalculateEvoList(data);
                }
            }
        }
    }
}

