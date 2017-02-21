namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public abstract class CabinDataItemBase
    {
        public CabinTechTree _techTree;
        public CabinType cabinType;
        public int extendGrade;
        public int level;
        public DateTime levelUpEndTime;

        protected CabinDataItemBase()
        {
        }

        public bool CanExtendCabin()
        {
            return ((CabinExtendGradeMetaDataReader.TryGetCabinExtendGradeMetaDataByKey(this.cabinType, this.extendGrade + 1) != null) && (this.status != CabinStatus.Locked));
        }

        public bool CanUpLevel()
        {
            return ((this.status == CabinStatus.UnLocked) && (this.level < this.GetCabinMaxLevel()));
        }

        public int GetCabinExntendScoinCost()
        {
            return CabinExtendGradeMetaDataReader.TryGetCabinExtendGradeMetaDataByKey(this.cabinType, this.extendGrade + 1).scoinNeed;
        }

        public int GetCabinLevelUpScoinCost()
        {
            return CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey(this.cabinType, this.level + 1).scoinNeed;
        }

        public int GetCabinLevelUpTimeCost()
        {
            return CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey(this.cabinType, this.level + 1).upLevelTimeNeed;
        }

        public int GetCabinMaxLevel()
        {
            return CabinExtendGradeMetaDataReader.TryGetCabinExtendGradeMetaDataByKey(this.cabinType, this.extendGrade).cabinLevelMax;
        }

        public int GetCabinMaxLevelNextExntendGrade()
        {
            return CabinExtendGradeMetaDataReader.TryGetCabinExtendGradeMetaDataByKey(this.cabinType, this.extendGrade + 1).cabinLevelMax;
        }

        public string GetCabinName()
        {
            return LocalizationGeneralLogic.GetText(CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey(this.cabinType, 1).cabinName, new object[0]);
        }

        public List<StorageDataItemBase> GetExtendItemNeed()
        {
            CabinExtendGradeMetaData data = CabinExtendGradeMetaDataReader.TryGetCabinExtendGradeMetaDataByKey(this.cabinType, this.extendGrade + 1);
            List<StorageDataItemBase> list = new List<StorageDataItemBase>();
            foreach (CabinExtendGradeMetaData.CabinExtendNeedItem item in data.itemListNeed)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(item.itemMetaID, 1);
                dummyStorageDataItem.number = item.itemNum;
                list.Add(dummyStorageDataItem);
            }
            return list;
        }

        public List<StorageDataItemBase> GetLevelUpItemNeed()
        {
            CabinLevelMetaData data = CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey(this.cabinType, this.level + 1);
            List<StorageDataItemBase> list = new List<StorageDataItemBase>();
            foreach (CabinLevelMetaData.CabinUpLevelNeedItem item in data.itemListNeed)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(item.itemMetaID, 1);
                if (dummyStorageDataItem != null)
                {
                    dummyStorageDataItem.number = item.itemNum;
                    list.Add(dummyStorageDataItem);
                }
            }
            return list;
        }

        public int GetPlayerLevelNeedToUpLevel()
        {
            return CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey(this.cabinType, this.level + 1).playerLevelNeed;
        }

        public int GetResetScoin()
        {
            if (this._techTree == null)
            {
                return 0;
            }
            return this._techTree.GetResetScoin();
        }

        public int GetUnlockPlayerLevel()
        {
            return CabinLevelMetaDataReader.TryGetCabinLevelMetaDataByKey(this.cabinType, 1).playerLevelNeed;
        }

        public int GetUsedPower()
        {
            if (this._techTree == null)
            {
                return 0;
            }
            return this._techTree.GetPowerUsed();
        }

        public bool HasTechTree()
        {
            return (this._techTree != null);
        }

        public bool IsUpLevel()
        {
            return (this.levelUpEndTime > TimeUtil.Now);
        }

        public bool NeedToShowLevelUpComplete()
        {
            return (Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[this.cabinType] && !this.IsUpLevel());
        }

        public virtual void SetupMateData()
        {
        }

        public CabinStatus status
        {
            get
            {
                if (this.level > 0)
                {
                    return CabinStatus.UnLocked;
                }
                return CabinStatus.Locked;
            }
        }
    }
}

