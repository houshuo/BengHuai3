namespace MoleMole
{
    using System;

    public class CabinBronyaEnhanceDataItem : CabinAvatarEnhanceDataItem
    {
        private static CabinBronyaEnhanceDataItem _instance;

        private CabinBronyaEnhanceDataItem()
        {
            base.cabinType = 7;
            base._techTree = new CabinTechTree(base.cabinType);
            base.level = 0;
            base.extendGrade = 1;
        }

        public static CabinBronyaEnhanceDataItem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CabinBronyaEnhanceDataItem();
            }
            return _instance;
        }
    }
}

