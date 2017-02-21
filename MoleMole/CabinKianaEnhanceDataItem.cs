namespace MoleMole
{
    using System;

    public class CabinKianaEnhanceDataItem : CabinAvatarEnhanceDataItem
    {
        private static CabinKianaEnhanceDataItem _instance;

        private CabinKianaEnhanceDataItem()
        {
            base.cabinType = 2;
            base._techTree = new CabinTechTree(base.cabinType);
            base.level = 0;
            base.extendGrade = 1;
        }

        public static CabinKianaEnhanceDataItem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CabinKianaEnhanceDataItem();
            }
            return _instance;
        }
    }
}

