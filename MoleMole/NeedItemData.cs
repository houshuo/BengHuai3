namespace MoleMole
{
    using System;

    public class NeedItemData
    {
        public bool enough;
        public readonly StorageDataItemBase itemData;
        public readonly int itemMetaID;
        public readonly int itemNum;

        public NeedItemData(int itemMetaID, int itemNum)
        {
            this.itemMetaID = itemMetaID;
            this.itemNum = itemNum;
            this.itemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(itemMetaID, 1);
            this.itemData.number = itemNum;
        }
    }
}

