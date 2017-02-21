namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class EquipmentLevelMetaDataReaderExtend
    {
        private static Dictionary<int, List<int>> _accumulateExpDict;

        public static int GetAccumulateExp(int level, int type)
        {
            return _accumulateExpDict[level][type];
        }

        public static void LoadFromFileAndBuildMap()
        {
            EquipmentLevelMetaDataReader.LoadFromFile();
            _accumulateExpDict = new Dictionary<int, List<int>>();
            List<EquipmentLevelMetaData> itemList = EquipmentLevelMetaDataReader.GetItemList();
            int num = 7;
            foreach (EquipmentLevelMetaData data in itemList)
            {
                if (!_accumulateExpDict.ContainsKey(data.level))
                {
                    _accumulateExpDict.Add(data.level, new List<int>());
                }
                if (data.level == 1)
                {
                    for (int i = 0; i <= num; i++)
                    {
                        _accumulateExpDict[data.level].Add(0);
                    }
                }
                else
                {
                    for (int j = 0; j <= num; j++)
                    {
                        _accumulateExpDict[data.level].Add(_accumulateExpDict[data.level - 1][j] + EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(data.level - 1).expList[j]);
                    }
                }
            }
        }

        private static void Test(int level, int type)
        {
        }

        private static void TestAll()
        {
            for (int i = 1; i < 10; i++)
            {
                Test(i, 1);
            }
            for (int j = 1; j < 10; j++)
            {
                Test(j, 2);
            }
        }
    }
}

