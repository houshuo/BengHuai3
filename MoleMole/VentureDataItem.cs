namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class VentureDataItem
    {
        private VentureMetaData _metaData;
        public List<int> dispatchAvatarIdList;
        public DateTime endTime;
        public List<int> selectedAvatarList;
        public VentureStatus status;

        public VentureDataItem(int ventureMetaID)
        {
            this._metaData = VentureMetaDataReader.TryGetVentureMetaDataByKey(ventureMetaID);
            this.status = VentureStatus.None;
            this.dispatchAvatarIdList = new List<int>();
            this.selectedAvatarList = new List<int>();
        }

        public void CleanUpSelectAvatarList()
        {
            <CleanUpSelectAvatarList>c__AnonStoreyC7 yc = new <CleanUpSelectAvatarList>c__AnonStoreyC7 {
                toRemoveAvatarList = new List<int>()
            };
            foreach (int num in this.selectedAvatarList)
            {
                if (Singleton<IslandModule>.Instance.IsAvatarDispatched(num))
                {
                    yc.toRemoveAvatarList.Add(num);
                }
            }
            this.selectedAvatarList.RemoveAll(new Predicate<int>(yc.<>m__C5));
        }

        public int GetStaminaReturnOnCancel()
        {
            TimeSpan span = (TimeSpan) (this.endTime - TimeUtil.Now);
            return Mathf.FloorToInt((this.StaminaCost * ((float) span.TotalSeconds)) / ((float) this.TimeCost));
        }

        public VentureCondition GetVentureCondition(int index)
        {
            VentureCondition condition = new VentureCondition();
            switch (index)
            {
                case 0:
                    if (this._metaData.requestType1 >= 1)
                    {
                        condition.condition = (IslandVentureDispatchCond) this._metaData.requestType1;
                        condition.para1 = this._metaData.argument11;
                        condition.para2 = this._metaData.argument12;
                        this.SetConditionDesc(condition);
                        return condition;
                    }
                    return null;

                case 1:
                    if (this._metaData.requestType2 >= 1)
                    {
                        condition.condition = (IslandVentureDispatchCond) this._metaData.requestType2;
                        condition.para1 = this._metaData.argument21;
                        condition.para2 = this._metaData.argument22;
                        this.SetConditionDesc(condition);
                        return condition;
                    }
                    return null;

                case 2:
                    if (this._metaData.requestType3 >= 1)
                    {
                        condition.condition = (IslandVentureDispatchCond) this._metaData.requestType3;
                        condition.para1 = this._metaData.argument31;
                        condition.para2 = this._metaData.argument32;
                        this.SetConditionDesc(condition);
                        return condition;
                    }
                    return null;
            }
            return null;
        }

        public bool IsConditionAllMatch()
        {
            if (this.selectedAvatarList.Count < 1)
            {
                return false;
            }
            for (int i = 0; i < 3; i++)
            {
                if (!this.IsConditionMatch(this.GetVentureCondition(i)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsConditionMatch(VentureCondition condition)
        {
            if (condition == null)
            {
                return true;
            }
            if (this.selectedAvatarList.Count >= 1)
            {
                AvatarModule instance = Singleton<AvatarModule>.Instance;
                switch (condition.condition)
                {
                    case 1:
                        return this.selectedAvatarList.Contains(condition.para1);

                    case 2:
                        foreach (int num in this.selectedAvatarList)
                        {
                            if (instance.GetAvatarByID(num).level < condition.para1)
                            {
                                return false;
                            }
                        }
                        return true;

                    case 3:
                        foreach (int num2 in this.selectedAvatarList)
                        {
                            if (instance.GetAvatarByID(num2).level >= condition.para1)
                            {
                                return true;
                            }
                        }
                        return false;

                    case 4:
                    {
                        int num3 = 0;
                        foreach (int num4 in this.selectedAvatarList)
                        {
                            num3 += instance.GetAvatarByID(num4).level;
                        }
                        return (num3 >= condition.para1);
                    }
                    case 5:
                        foreach (int num5 in this.selectedAvatarList)
                        {
                            if (instance.GetAvatarByID(num5).star < condition.para1)
                            {
                                return false;
                            }
                        }
                        return true;

                    case 6:
                        foreach (int num6 in this.selectedAvatarList)
                        {
                            if (instance.GetAvatarByID(num6).star >= condition.para1)
                            {
                                return true;
                            }
                        }
                        return false;

                    case 7:
                        return (this.selectedAvatarList.Count >= condition.para1);

                    case 8:
                    {
                        int num7 = 0;
                        foreach (int num8 in this.selectedAvatarList)
                        {
                            if (instance.GetAvatarByID(num8).Attribute == condition.para1)
                            {
                                num7++;
                            }
                        }
                        return (num7 >= condition.para2);
                    }
                    case 9:
                    {
                        int num9 = 0;
                        foreach (int num10 in this.selectedAvatarList)
                        {
                            if (instance.GetAvatarByID(num10).ClassId == condition.para1)
                            {
                                num9++;
                            }
                        }
                        return (num9 >= condition.para2);
                    }
                    case 10:
                    {
                        HashSet<int> set = new HashSet<int>();
                        foreach (int num11 in this.selectedAvatarList)
                        {
                            AvatarDataItem avatarByID = instance.GetAvatarByID(num11);
                            if (!set.Contains(avatarByID.Attribute))
                            {
                                set.Add(avatarByID.Attribute);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    case 11:
                    {
                        HashSet<int> set2 = new HashSet<int>();
                        foreach (int num12 in this.selectedAvatarList)
                        {
                            AvatarDataItem item4 = instance.GetAvatarByID(num12);
                            if (!set2.Contains(item4.ClassId))
                            {
                                set2.Add(item4.ClassId);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public void SetConditionDesc(VentureCondition condition)
        {
            if (condition != null)
            {
                AvatarModule instance = Singleton<AvatarModule>.Instance;
                switch (condition.condition)
                {
                    case 1:
                    {
                        AvatarDataItem avatarByID = instance.GetAvatarByID(condition.para1);
                        object[] replaceParams = new object[] { avatarByID.FullName };
                        condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[condition.condition], replaceParams);
                        break;
                    }
                    case 2:
                    {
                        object[] objArray2 = new object[] { condition.para1 };
                        condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[condition.condition], objArray2);
                        break;
                    }
                    case 3:
                    {
                        object[] objArray3 = new object[] { condition.para1 };
                        condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[condition.condition], objArray3);
                        break;
                    }
                    case 4:
                    {
                        object[] objArray4 = new object[] { condition.para1 };
                        condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[condition.condition], objArray4);
                        break;
                    }
                    case 5:
                    {
                        object[] objArray5 = new object[] { MiscData.Config.AvatarStarName[condition.para1] };
                        condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[condition.condition], objArray5);
                        break;
                    }
                    case 6:
                    {
                        object[] objArray6 = new object[] { MiscData.Config.AvatarStarName[condition.para1] };
                        condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[condition.condition], objArray6);
                        break;
                    }
                    case 7:
                    {
                        object[] objArray7 = new object[] { condition.para1 };
                        condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[condition.condition], objArray7);
                        break;
                    }
                    case 8:
                    {
                        string text = LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.AvatarAttributeName[condition.para1], new object[0]);
                        object[] objArray8 = new object[] { text, condition.para2 };
                        condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[condition.condition], objArray8);
                        break;
                    }
                    case 9:
                    {
                        string str2 = LocalizationGeneralLogic.GetText(ClassMetaDataReader.GetClassMetaDataByKey(condition.para1).firstName, new object[0]);
                        object[] objArray9 = new object[] { str2, condition.para2 };
                        condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[condition.condition], objArray9);
                        break;
                    }
                    case 10:
                        condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[condition.condition], new object[0]);
                        break;

                    case 11:
                        condition.desc = LocalizationGeneralLogic.GetText(MiscData.Config.IslandVentureConditionText[condition.condition], new object[0]);
                        break;
                }
            }
        }

        public void SetDispatchAvatarList(List<uint> avatarIdList)
        {
            this.dispatchAvatarIdList.Clear();
            foreach (uint num in avatarIdList)
            {
                this.dispatchAvatarIdList.Add((int) num);
            }
        }

        public void SetEndTime(uint timeStamp)
        {
            if (timeStamp == 0)
            {
                this.status = VentureStatus.None;
            }
            else
            {
                this.endTime = Miscs.GetDateTimeFromTimeStamp(timeStamp);
                if (this.endTime > TimeUtil.Now)
                {
                    this.status = VentureStatus.InProgress;
                }
                else
                {
                    this.status = VentureStatus.Done;
                }
            }
        }

        public int AvatarMaxNum
        {
            get
            {
                return this._metaData.avatarMaxNum;
            }
        }

        public string Desc
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.desc, new object[0]);
            }
        }

        public int Difficulty
        {
            get
            {
                return this._metaData.difficulty;
            }
        }

        public int ExtraHcoinNum
        {
            get
            {
                return this._metaData.extraHcoinNum;
            }
        }

        public float ExtraHcoinRatio
        {
            get
            {
                return (((float) this._metaData.extraHcoinChange) / 10000f);
            }
        }

        public string IconPath
        {
            get
            {
                return this._metaData.iconPath;
            }
        }

        public int Level
        {
            get
            {
                return this._metaData.level;
            }
        }

        public int RewardExp
        {
            get
            {
                RewardData data = RewardDataReader.TryGetRewardDataByKey(this._metaData.rewardId);
                if ((data != null) && (data.RewardExp > 0))
                {
                    return data.RewardExp;
                }
                return 0;
            }
        }

        public List<int> RewardItemIDListToShow
        {
            get
            {
                return this._metaData.rewardItemShowList;
            }
        }

        public int StaminaCost
        {
            get
            {
                return this._metaData.staminaCost;
            }
        }

        public int TimeCost
        {
            get
            {
                return this._metaData.timeCost;
            }
        }

        public int VentureID
        {
            get
            {
                return this._metaData.ID;
            }
        }

        public string VentureName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.name, new object[0]);
            }
        }

        [CompilerGenerated]
        private sealed class <CleanUpSelectAvatarList>c__AnonStoreyC7
        {
            internal List<int> toRemoveAvatarList;

            internal bool <>m__C5(int x)
            {
                return this.toRemoveAvatarList.Contains(x);
            }
        }

        public enum VentureStatus
        {
            None,
            InProgress,
            Done
        }
    }
}

