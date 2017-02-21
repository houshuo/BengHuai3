namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class CabinCollectDataItem : CabinDataItemBase
    {
        private static CabinCollectDataItem _instance;
        private CabinCollectLevelDataMetaData _metaData;
        public bool canUpdateScoinLate;
        public int currentScoinAmount;
        public List<DropItem> dropItems;
        public DateTime nextScoinUpdateTime;

        private CabinCollectDataItem()
        {
            base.cabinType = 3;
            base._techTree = new CabinTechTree(base.cabinType);
            base.level = 0;
            base.extendGrade = 1;
        }

        public bool CanFetchScoin()
        {
            if (((this.currentScoinAmount <= 0) && this.canUpdateScoinLate) && (TimeUtil.Now >= this.nextScoinUpdateTime))
            {
                Singleton<NetworkManager>.Instance.RequestGetCollectCabin();
                return false;
            }
            return (this.currentScoinAmount > 0);
        }

        public static CabinCollectDataItem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CabinCollectDataItem();
            }
            return _instance;
        }

        public bool HasScoin()
        {
            return (this.currentScoinAmount > 0);
        }

        public override void SetupMateData()
        {
            this._metaData = CabinCollectLevelMetaDataReader.TryGetCabinCollectLevelDataMetaDataByKey(base.level);
        }

        public bool TimeToFetch()
        {
            return (((this.currentScoinAmount <= 0) && this.canUpdateScoinLate) && (TimeUtil.Now >= this.nextScoinUpdateTime));
        }

        public float crtExtraRatio
        {
            get
            {
                float num = this._metaData.extraScoinRatioAddBase / 100f;
                foreach (CabinTechTreeNode node in base._techTree.GetActiveNodeList())
                {
                    CabinTechTreeMetaData data = node._metaData;
                    if (data.AbilityType == 10)
                    {
                        num += ((float) data.Argument1) / 100f;
                    }
                }
                return num;
            }
        }

        public float crtRatio
        {
            get
            {
                float num = ((float) this._metaData.extraScoinRatioBase) / 100f;
                foreach (CabinTechTreeNode node in base._techTree.GetActiveNodeList())
                {
                    CabinTechTreeMetaData data = node._metaData;
                    if (data.AbilityType == 9)
                    {
                        num += ((float) data.Argument1) / 100f;
                    }
                }
                return num;
            }
        }

        public float speed
        {
            get
            {
                float scoinGrowthBase = this._metaData.scoinGrowthBase;
                foreach (CabinTechTreeNode node in base._techTree.GetActiveNodeList())
                {
                    CabinTechTreeMetaData data = node._metaData;
                    if (data.AbilityType == 7)
                    {
                        scoinGrowthBase += data.Argument1;
                    }
                }
                return scoinGrowthBase;
            }
        }

        public float topLimit
        {
            get
            {
                float scoinStorageBase = this._metaData.scoinStorageBase;
                foreach (CabinTechTreeNode node in base._techTree.GetActiveNodeList())
                {
                    CabinTechTreeMetaData data = node._metaData;
                    if (data.AbilityType == 8)
                    {
                        scoinStorageBase += data.Argument1;
                    }
                }
                return scoinStorageBase;
            }
        }
    }
}

