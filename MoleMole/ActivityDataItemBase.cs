namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public abstract class ActivityDataItemBase
    {
        protected Status _status;
        public DateTime beginTime;
        public DateTime endTime;

        protected ActivityDataItemBase()
        {
        }

        public abstract string GetActitityTitle();
        public abstract string GetActivityDescription();
        public abstract string GetActivityEnterImgPath();
        public abstract int GetActivityID();
        public abstract ActivityType GetActivityType();
        public abstract List<int> GetLevelIDList();
        public abstract int GetMinPlayerLevelLimit();
        public abstract Status GetStatus();
        public abstract void InitStatusOnPacket();

        public enum Status
        {
            Unavailable,
            Over,
            Locked,
            WaitToStart,
            InProgress
        }
    }
}

