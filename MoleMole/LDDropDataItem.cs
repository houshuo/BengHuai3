namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public abstract class LDDropDataItem
    {
        protected LDDropDataItem()
        {
        }

        public LDDropDataItem Clone()
        {
            return (LDDropDataItem) base.MemberwiseClone();
        }

        public abstract void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = true);
        public static LDDropDataItem GetLDDropDataItemByName(string typeName)
        {
            if (typeName == "HPMedic")
            {
                return new LDDropHPMedic();
            }
            if (typeName == "SPMedic")
            {
                return new LDDropSPMedic();
            }
            if (typeName == "EquipItem")
            {
                return new LDDropEquipItem();
            }
            if (typeName == "Coin")
            {
                return new LDDropCoin();
            }
            if (typeName == "Boost")
            {
                return new LDDropBoostSpeed();
            }
            if (typeName == "Crit")
            {
                return new LDDropEnhanceCrit();
            }
            if (typeName == "Shielded")
            {
                return new LDDropShielded();
            }
            return null;
        }
    }
}

