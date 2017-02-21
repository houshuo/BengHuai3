namespace MoleMole
{
    using System;

    public class CabinMiscDataItem : CabinDataItemBase
    {
        private static CabinMiscDataItem _instance;

        private CabinMiscDataItem()
        {
            base.cabinType = 4;
            base._techTree = new CabinTechTree(base.cabinType);
            base.level = 0;
            base.extendGrade = 1;
        }

        public static CabinMiscDataItem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CabinMiscDataItem();
            }
            return _instance;
        }
    }
}

