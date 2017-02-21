namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public class WeekDayActivityDataItem : ActivityDataItemBase
    {
        private WeekDayActivityMetaData _metaData;
        public int enterTimes;

        public WeekDayActivityDataItem(int activityID)
        {
            this._metaData = WeekDayActivityMetaDataReader.GetWeekDayActivityMetaDataByKey(activityID);
            base._status = ActivityDataItemBase.Status.Unavailable;
        }

        public override string GetActitityTitle()
        {
            return LocalizationGeneralLogic.GetText(this._metaData.title, new object[0]);
        }

        public override string GetActivityDescription()
        {
            return LocalizationGeneralLogic.GetText(this._metaData.desc, new object[0]);
        }

        public override string GetActivityEnterImgPath()
        {
            return this._metaData.enterImgPath;
        }

        public override int GetActivityID()
        {
            return this._metaData.weekDayActivityID;
        }

        public string GetActivityLockDescription()
        {
            return LocalizationGeneralLogic.GetText(this._metaData.descLock, new object[0]);
        }

        public override ActivityType GetActivityType()
        {
            return (ActivityType) this._metaData.activityType;
        }

        public string GetBgImgPath()
        {
            return this._metaData.bgImgPath;
        }

        public override List<int> GetLevelIDList()
        {
            return this._metaData.levelIDList;
        }

        public string GetLevelPanelPath()
        {
            return this._metaData.levelPanelPath;
        }

        public override int GetMinPlayerLevelLimit()
        {
            return this._metaData.minPlayerLevel;
        }

        public int GetSeriesID()
        {
            return this._metaData.seriesID;
        }

        public string GetSmallImgPath()
        {
            return this._metaData.smallImgPath;
        }

        public override ActivityDataItemBase.Status GetStatus()
        {
            bool flag = false;
            if (base._status != ActivityDataItemBase.Status.Unavailable)
            {
                if (TimeUtil.Now < base.beginTime)
                {
                    base._status = ActivityDataItemBase.Status.WaitToStart;
                }
                else if (TimeUtil.Now > base.endTime)
                {
                    base._status = ActivityDataItemBase.Status.Over;
                }
                else
                {
                    base._status = ActivityDataItemBase.Status.InProgress;
                    flag = true;
                }
                if ((base._status != ActivityDataItemBase.Status.Unavailable) && (Singleton<PlayerModule>.Instance.playerData.teamLevel < this._metaData.minPlayerLevel))
                {
                    base._status = ActivityDataItemBase.Status.Locked;
                }
                foreach (int num in this._metaData.levelIDList)
                {
                    LevelDataItem item = Singleton<LevelModule>.Instance.TryGetLevelById(num);
                    if (item != null)
                    {
                        item.status = !flag ? ((StageStatus) 1) : ((StageStatus) 2);
                    }
                }
            }
            return base._status;
        }

        public override void InitStatusOnPacket()
        {
            base._status = ActivityDataItemBase.Status.WaitToStart;
        }

        public bool ShouldDisplayLeftTime()
        {
            return (this._metaData.displayLeftTime != 0);
        }

        public bool ShowActivityShopEntry()
        {
            return (this._metaData.showActivityShopEntry != 0);
        }

        public int maxEnterTimes
        {
            get
            {
                return this._metaData.maxEnterTimes;
            }
        }
    }
}

