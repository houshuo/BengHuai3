namespace MoleMole
{
    using System;

    public class CabinVentureDataItem : CabinDataItemBase
    {
        private static CabinVentureDataItem _instance;
        private CabinVentureLevelDataMetaData _levelMetaData;

        private CabinVentureDataItem()
        {
            base.cabinType = 5;
            base._techTree = new CabinTechTree(base.cabinType);
            base.level = 0;
            base.extendGrade = 1;
        }

        public static CabinVentureDataItem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CabinVentureDataItem();
            }
            return _instance;
        }

        public int GetMaxVentureNum()
        {
            int maxVentureNumBase = this._levelMetaData.maxVentureNumBase;
            foreach (CabinTechTreeNode node in base._techTree.GetActiveNodeList())
            {
                CabinTechTreeMetaData data = node._metaData;
                if (data.AbilityType == 2)
                {
                    maxVentureNumBase += data.Argument1;
                }
            }
            return maxVentureNumBase;
        }

        public int GetMaxVentureNumInProgress()
        {
            int maxVentureInProgressNumBase = this._levelMetaData.maxVentureInProgressNumBase;
            foreach (CabinTechTreeNode node in base._techTree.GetActiveNodeList())
            {
                CabinTechTreeMetaData data = node._metaData;
                if (data.AbilityType == 3)
                {
                    maxVentureInProgressNumBase += data.Argument1;
                }
            }
            return maxVentureInProgressNumBase;
        }

        public bool GetRefreshCost(int times)
        {
            CabinVentureRefreshDataMetaData data = CabinVentureRefreshMetaDataReader.TryGetCabinVentureRefreshDataMetaDataByKey(times);
            if (data == null)
            {
                return false;
            }
            if (this._levelMetaData.refreshType > data.needHcoinList.Count)
            {
                return false;
            }
            return true;
        }

        public override void SetupMateData()
        {
            this._levelMetaData = CabinVentureLevelMetaDataReader.GetCabinVentureLevelDataMetaDataByKey(base.level);
        }
    }
}

