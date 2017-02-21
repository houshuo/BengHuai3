namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class DialogMetaDataReader
    {
        private static Dictionary<int, DialogMetaData> _itemDict;
        private static List<DialogMetaData> _itemList;

        public static int CalculateContentHash()
        {
            int lastHash = 0;
            foreach (DialogMetaData data in _itemList)
            {
                HashUtils.TryHashObject(data, ref lastHash);
            }
            return lastHash;
        }

        public static DialogMetaData GetDialogMetaDataByKey(int dialogID)
        {
            DialogMetaData data;
            _itemDict.TryGetValue(dialogID, out data);
            if (data == null)
            {
            }
            return data;
        }

        public static DialogDataItem GetFirstLeftDialogDataItem(PlotDataItem plotDataItem)
        {
            if (_itemDict.ContainsKey(plotDataItem.startDialogID) && _itemDict.ContainsKey(plotDataItem.endDialogID))
            {
                for (int i = plotDataItem.startDialogID; i < plotDataItem.endDialogID; i++)
                {
                    if (_itemDict.ContainsKey(i) && (_itemDict[i].screenSide == 0))
                    {
                        return new DialogDataItem(_itemDict[i]);
                    }
                }
            }
            return null;
        }

        public static DialogDataItem GetFirstRightDialogDataItem(PlotDataItem plotDataItem)
        {
            if (_itemDict.ContainsKey(plotDataItem.startDialogID) && _itemDict.ContainsKey(plotDataItem.endDialogID))
            {
                for (int i = plotDataItem.startDialogID; i < plotDataItem.endDialogID; i++)
                {
                    if (_itemDict.ContainsKey(i) && (_itemDict[i].screenSide == 1))
                    {
                        return new DialogDataItem(_itemDict[i]);
                    }
                }
            }
            return null;
        }

        public static List<DialogMetaData> GetItemList()
        {
            return _itemList;
        }

        public static void LoadFromFile()
        {
            List<string> list = new List<string>();
            char[] separator = new char[] { "\n"[0] };
            string[] strArray = CommonUtils.LoadTextFileToString("Data/_ExcelOutput/DialogData").Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].Length >= 1)
                {
                    list.Add(strArray[i]);
                }
            }
            int capacity = list.Count - 1;
            _itemDict = new Dictionary<int, DialogMetaData>();
            _itemList = new List<DialogMetaData>(capacity);
            for (int j = 1; j <= capacity; j++)
            {
                char[] chArray2 = new char[] { "\t"[0] };
                string[] strArray2 = list[j].Split(chArray2);
                List<DialogMetaData.PlotChatNode> content = new List<DialogMetaData.PlotChatNode>();
                foreach (string str2 in CommonUtils.GetStringListFromString(strArray2[4], null))
                {
                    content.Add(new DialogMetaData.PlotChatNode(str2));
                }
                DialogMetaData item = new DialogMetaData(int.Parse(strArray2[0]), int.Parse(strArray2[1]), int.Parse(strArray2[2]), strArray2[3].Trim(), content, strArray2[5].Trim());
                if (!_itemDict.ContainsKey(item.dialogID))
                {
                    _itemList.Add(item);
                    _itemDict.Add(item.dialogID, item);
                }
            }
        }

        public static DialogMetaData TryGetDialogMetaDataByKey(int dialogID)
        {
            DialogMetaData data;
            _itemDict.TryGetValue(dialogID, out data);
            return data;
        }
    }
}

