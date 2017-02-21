namespace MoleMole
{
    using System;

    public class CabinEngineDataItem : CabinDataItemBase
    {
        private static CabinEngineDataItem _instance;

        private CabinEngineDataItem()
        {
            base.cabinType = 1;
            base._techTree = null;
            base.level = 0;
            base.extendGrade = 1;
        }

        public static CabinEngineDataItem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CabinEngineDataItem();
            }
            return _instance;
        }
    }
}

