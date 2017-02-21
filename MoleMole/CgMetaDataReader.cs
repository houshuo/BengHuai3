namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class CgMetaDataReader
    {
        private static Dictionary<int, CgMetaData> _itemDict;
        private static List<CgMetaData> _itemList;

        public static int CalculateContentHash()
        {
            int lastHash = 0;
            foreach (CgMetaData data in _itemList)
            {
                HashUtils.TryHashObject(data, ref lastHash);
            }
            return lastHash;
        }

        public static CgMetaData GetCgMetaDataByKey(int CgID)
        {
            CgMetaData data;
            _itemDict.TryGetValue(CgID, out data);
            if (data == null)
            {
            }
            return data;
        }

        public static List<CgMetaData> GetItemList()
        {
            return _itemList;
        }

        public static void LoadFromFile()
        {
            List<string> list = new List<string>();
            char[] separator = new char[] { "\n"[0] };
            string[] strArray = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/CgData").Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].Length >= 1)
                {
                    list.Add(strArray[i]);
                }
            }
            int capacity = list.Count - 1;
            _itemDict = new Dictionary<int, CgMetaData>();
            _itemList = new List<CgMetaData>(capacity);
            for (int j = 1; j <= capacity; j++)
            {
                char[] chArray2 = new char[] { "\t"[0] };
                string[] strArray2 = list[j].Split(chArray2);
                CgMetaData item = new CgMetaData(int.Parse(strArray2[0]), int.Parse(strArray2[1]), strArray2[2].Trim(), strArray2[3].Trim());
                if (!_itemDict.ContainsKey(item.CgID))
                {
                    _itemList.Add(item);
                    _itemDict.Add(item.CgID, item);
                }
            }
        }

        public static CgMetaData TryGetCgMetaDataByKey(int CgID)
        {
            CgMetaData data;
            _itemDict.TryGetValue(CgID, out data);
            return data;
        }
    }
}

