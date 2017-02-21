namespace MoleMole
{
    using System;

    public class CabinMeiEnhanceDataItem : CabinAvatarEnhanceDataItem
    {
        private static CabinMeiEnhanceDataItem _instance;

        private CabinMeiEnhanceDataItem()
        {
            base.cabinType = 6;
            base._techTree = new CabinTechTree(base.cabinType);
            base.level = 0;
            base.extendGrade = 1;
        }

        public static CabinMeiEnhanceDataItem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CabinMeiEnhanceDataItem();
            }
            return _instance;
        }
    }
}

