namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class GalTouchData
    {
        private static List<TouchBuffItem> _touchBuffItemList;
        private static List<TouchDataItem> _touchDataItemList;
        private static List<TouchLevelItem> _touchLevelItemList;
        private static List<TouchMissionItem> _touchMissionItemList;
        public const int BODY_PART_INDEX_ARM = 7;
        public const int BODY_PART_INDEX_CHEST = 3;
        public const int BODY_PART_INDEX_CHEST_ADV = 4;
        public const int BODY_PART_INDEX_FACE = 1;
        public const int BODY_PART_INDEX_HEAD = 2;
        public const int BODY_PART_INDEX_LEG = 9;
        public const int BODY_PART_INDEX_PRIVATE = 5;
        public const int BODY_PART_INDEX_PRIVATE_ADV = 6;
        public const int BODY_PART_INDEX_STOMACH = 8;

        public static TouchBuffItem GetTouchBuffItem(int buffId)
        {
            int num = 0;
            int count = _touchBuffItemList.Count;
            while (num < count)
            {
                if (_touchBuffItemList[num].buffId == buffId)
                {
                    return _touchBuffItemList[num];
                }
                num++;
            }
            return null;
        }

        private static int GetTouchID(int avatarID, int partIndex, int heartLevel)
        {
            if (((partIndex > 0) && (partIndex <= 9)) && ((heartLevel > 0) && (heartLevel <= 4)))
            {
                return ((((avatarID / 100) * 0x2710) + (heartLevel * 100)) + partIndex);
            }
            return 0;
        }

        public static TouchLevelItem GetTouchLevelItem(int level)
        {
            int num = 0;
            int count = _touchLevelItemList.Count;
            while (num < count)
            {
                if (_touchLevelItemList[num].level == level)
                {
                    return _touchLevelItemList[num];
                }
                num++;
            }
            return null;
        }

        public static TouchMissionItem GetTouchMissionItem(int avatarId, int heartLevel)
        {
            int num = 0;
            int count = _touchMissionItemList.Count;
            while (num < count)
            {
                if ((_touchMissionItemList[num].avatarId == avatarId) && (_touchMissionItemList[num].goodFeelLevel == heartLevel))
                {
                    return _touchMissionItemList[num];
                }
                num++;
            }
            return null;
        }

        public static void LoadFromFile()
        {
            LoadTouchData();
            LoadTouchLevelData();
            LoadTouchBuffData();
            LoadTouchMissionData();
        }

        private static void LoadTouchBuffData()
        {
            List<string> list = new List<string>();
            TextAsset asset = Miscs.LoadResource("Data/_ExcelOutput/TouchBuffData", BundleType.DATA_FILE) as TextAsset;
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
            _touchBuffItemList = new List<TouchBuffItem>(capacity);
            for (int j = 1; j <= capacity; j++)
            {
                char[] chArray2 = new char[] { "\t"[0] };
                string[] strArray2 = list[j].Split(chArray2);
                TouchBuffItem item = new TouchBuffItem {
                    buffId = int.Parse(strArray2[0]),
                    effect = strArray2[1],
                    detail = strArray2[2]
                };
                float.TryParse(strArray2[3], out item.param1);
                float.TryParse(strArray2[4], out item.param2);
                float.TryParse(strArray2[5], out item.param3);
                float.TryParse(strArray2[6], out item.param1Add);
                float.TryParse(strArray2[7], out item.param2Add);
                float.TryParse(strArray2[8], out item.param3Add);
                _touchBuffItemList.Add(item);
            }
        }

        private static void LoadTouchData()
        {
            List<string> list = new List<string>();
            TextAsset asset = Miscs.LoadResource("Data/_ExcelOutput/TouchData", BundleType.DATA_FILE) as TextAsset;
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
            _touchDataItemList = new List<TouchDataItem>(capacity);
            for (int j = 1; j <= capacity; j++)
            {
                char[] chArray2 = new char[] { "\t"[0] };
                string[] strArray2 = list[j].Split(chArray2);
                TouchDataItem item = new TouchDataItem {
                    touchId = int.Parse(strArray2[0]),
                    level = int.Parse(strArray2[1]),
                    point = int.Parse(strArray2[2])
                };
                char[] chArray3 = new char[] { ',' };
                string[] strArray3 = strArray2[3].Split(chArray3);
                item.buff = new int[strArray3.Length];
                for (int k = 0; k < strArray3.Length; k++)
                {
                    item.buff[k] = int.Parse(strArray3[k]);
                }
                _touchDataItemList.Add(item);
            }
        }

        private static void LoadTouchLevelData()
        {
            List<string> list = new List<string>();
            TextAsset asset = Miscs.LoadResource("Data/_ExcelOutput/TouchLevelData", BundleType.DATA_FILE) as TextAsset;
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
            _touchLevelItemList = new List<TouchLevelItem>(capacity);
            for (int j = 1; j <= capacity; j++)
            {
                char[] chArray2 = new char[] { "\t"[0] };
                string[] strArray2 = list[j].Split(chArray2);
                TouchLevelItem item = new TouchLevelItem {
                    level = int.Parse(strArray2[0]),
                    touchExp = int.Parse(strArray2[1]),
                    prop = float.Parse(strArray2[2]),
                    rate = float.Parse(strArray2[3]),
                    battleGain = int.Parse(strArray2[4])
                };
                _touchLevelItemList.Add(item);
            }
        }

        private static void LoadTouchMissionData()
        {
            List<string> list = new List<string>();
            TextAsset asset = Miscs.LoadResource("Data/_ExcelOutput/AvatarGoodfeelData", BundleType.DATA_FILE) as TextAsset;
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
            _touchMissionItemList = new List<TouchMissionItem>(capacity);
            for (int j = 1; j <= capacity; j++)
            {
                char[] chArray2 = new char[] { "\t"[0] };
                string[] strArray2 = list[j].Split(chArray2);
                TouchMissionItem item = new TouchMissionItem {
                    avatarId = int.Parse(strArray2[0]),
                    goodFeelLevel = int.Parse(strArray2[1]),
                    missionId = int.Parse(strArray2[2])
                };
                _touchMissionItemList.Add(item);
            }
        }

        public static int QueryBattleGain(int level)
        {
            if ((level <= 0) || (level > 4))
            {
                return 0;
            }
            TouchLevelItem item = null;
            int num = 0;
            int count = _touchLevelItemList.Count;
            while (num < count)
            {
                if (_touchLevelItemList[num].level == level)
                {
                    item = _touchLevelItemList[num];
                    break;
                }
                num++;
            }
            if (item == null)
            {
                return 0;
            }
            return item.battleGain;
        }

        public static int QueryLevelUpFeelNeed(int level)
        {
            if ((level <= 0) || (level > 4))
            {
                return 0;
            }
            TouchLevelItem item = null;
            int num = 0;
            int count = _touchLevelItemList.Count;
            while (num < count)
            {
                if (_touchLevelItemList[num].level == level)
                {
                    item = _touchLevelItemList[num];
                    break;
                }
                num++;
            }
            if (item == null)
            {
                return 0;
            }
            return item.touchExp;
        }

        public static int QueryLevelUpFeelNeedBattle(int level)
        {
            if ((level <= 0) || (level > 4))
            {
                return 0;
            }
            TouchLevelItem item = null;
            int num = 0;
            int count = _touchLevelItemList.Count;
            while (num < count)
            {
                if (_touchLevelItemList[num].level == level)
                {
                    item = _touchLevelItemList[num];
                    break;
                }
                num++;
            }
            if (item == null)
            {
                return 0;
            }
            float num3 = item.touchExp * item.rate;
            return (int) num3;
        }

        public static int QueryLevelUpFeelNeedTouch(int level)
        {
            if ((level <= 0) || (level > 4))
            {
                return 0;
            }
            TouchLevelItem item = null;
            int num = 0;
            int count = _touchLevelItemList.Count;
            while (num < count)
            {
                if (_touchLevelItemList[num].level == level)
                {
                    item = _touchLevelItemList[num];
                    break;
                }
                num++;
            }
            if (item == null)
            {
                return 0;
            }
            float num3 = item.touchExp * item.rate;
            return (item.touchExp - ((int) num3));
        }

        public static int[] QueryTouchBuff(int avatarID, int partIndex, int heartLevel)
        {
            int num = GetTouchID(avatarID, partIndex, heartLevel);
            if (num == 0)
            {
                return null;
            }
            TouchDataItem item = null;
            int num2 = 0;
            int count = _touchDataItemList.Count;
            while (num2 < count)
            {
                if (_touchDataItemList[num2].touchId == num)
                {
                    item = _touchDataItemList[num2];
                    break;
                }
                num2++;
            }
            if (item == null)
            {
                return null;
            }
            return item.buff;
        }

        public static int QueryTouchFeel(int avatarID, int partIndex, int heartLevel)
        {
            int num = GetTouchID(avatarID, partIndex, heartLevel);
            if (num == 0)
            {
                return 0;
            }
            TouchDataItem item = null;
            int num2 = 0;
            int count = _touchDataItemList.Count;
            while (num2 < count)
            {
                if (_touchDataItemList[num2].touchId == num)
                {
                    item = _touchDataItemList[num2];
                    break;
                }
                num2++;
            }
            if (item == null)
            {
                return 0;
            }
            return item.point;
        }
    }
}

