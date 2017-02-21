namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlotMetaDataReader
    {
        private static Dictionary<int, PlotMetaData> _itemDict;
        private static List<PlotMetaData> _itemList;

        public static List<PlotMetaData> GetItemList()
        {
            return _itemList;
        }

        public static PlotMetaData GetPlotMetaDataByKey(int plotID)
        {
            PlotMetaData data;
            _itemDict.TryGetValue(plotID, out data);
            if (data == null)
            {
            }
            return data;
        }

        public static void LoadFromFile()
        {
            List<string> list = new List<string>();
            TextAsset asset = Miscs.LoadResource("Data/_ExcelOutput/PlotData", BundleType.DATA_FILE) as TextAsset;
            char[] separator = new char[] { "\n"[0] };
            string[] strArray = asset.text.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].Length >= 1)
                {
                    list.Add(strArray[i]);
                }
            }
            int capacity = list.Count - 1;
            _itemDict = new Dictionary<int, PlotMetaData>();
            _itemList = new List<PlotMetaData>(capacity);
            for (int j = 1; j <= capacity; j++)
            {
                char[] chArray2 = new char[] { "\t"[0] };
                string[] strArray2 = list[j].Split(chArray2);
                PlotMetaData item = new PlotMetaData(int.Parse(strArray2[0]), int.Parse(strArray2[1]), int.Parse(strArray2[2]), int.Parse(strArray2[3]));
                _itemList.Add(item);
                _itemDict.Add(item.plotID, item);
            }
        }

        public static PlotMetaData TryGetPlotMetaDataByKey(int plotID)
        {
            PlotMetaData data;
            _itemDict.TryGetValue(plotID, out data);
            return data;
        }
    }
}

