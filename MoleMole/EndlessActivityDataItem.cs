namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public class EndlessActivityDataItem : ActivityDataItemBase
    {
        private static EndlessActivityDataItem _instance;

        private EndlessActivityDataItem()
        {
            base._status = ActivityDataItemBase.Status.InProgress;
        }

        public override string GetActitityTitle()
        {
            return "Endless";
        }

        public override string GetActivityDescription()
        {
            return "Endless";
        }

        public override string GetActivityEnterImgPath()
        {
            return "SpriteOutput/ChapterCover/Event/BookEventEndless";
        }

        public override int GetActivityID()
        {
            return 0x2329;
        }

        public override ActivityType GetActivityType()
        {
            return 0;
        }

        public int GetEndlessMaxProgress()
        {
            return Singleton<PlayerModule>.Instance.playerData.endlessMaxProgress;
        }

        public static EndlessActivityDataItem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new EndlessActivityDataItem();
            }
            return _instance;
        }

        public override List<int> GetLevelIDList()
        {
            return new List<int> { 1 };
        }

        public override int GetMinPlayerLevelLimit()
        {
            return Singleton<PlayerModule>.Instance.playerData.endlessMinPlayerLevel;
        }

        public override ActivityDataItemBase.Status GetStatus()
        {
            base._status = (Singleton<PlayerModule>.Instance.playerData.teamLevel >= this.GetMinPlayerLevelLimit()) ? ActivityDataItemBase.Status.InProgress : ActivityDataItemBase.Status.Locked;
            return base._status;
        }

        public override void InitStatusOnPacket()
        {
            base._status = ActivityDataItemBase.Status.WaitToStart;
        }
    }
}

