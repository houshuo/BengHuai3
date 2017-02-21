namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ItempediaData
    {
        private static List<int> _blackList;

        public static bool IsInBlacklist(int id)
        {
            int num = 0;
            int count = _blackList.Count;
            while (num < count)
            {
                if (_blackList[num] == id)
                {
                    return true;
                }
                num++;
            }
            return false;
        }

        public static void LoadFromFile()
        {
            List<string> list = new List<string>();
            TextAsset asset = Miscs.LoadResource("Data/_ExcelOutput/BlacklistData", BundleType.DATA_FILE) as TextAsset;
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
            _blackList = new List<int>(capacity);
            for (int j = 1; j <= capacity; j++)
            {
                char[] chArray2 = new char[] { "\t"[0] };
                string[] strArray2 = list[j].Split(chArray2);
                _blackList.Add(int.Parse(strArray2[0]));
            }
        }
    }
}

