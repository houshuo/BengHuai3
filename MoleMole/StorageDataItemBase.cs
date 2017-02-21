namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public abstract class StorageDataItemBase
    {
        public int avatarID = -1;
        public int exp;
        public int ID;
        public bool isProtected;
        public int level = 1;
        public int number = 1;
        public int rarity;
        public int uid;

        protected StorageDataItemBase()
        {
        }

        public abstract StorageDataItemBase Clone();
        public static int CompareToBaseTypeAsc(StorageDataItemBase lobj, StorageDataItemBase robj)
        {
            if (lobj.GetBaseType() != robj.GetBaseType())
            {
                return (lobj.GetBaseType() - robj.GetBaseType());
            }
            return CompareToRarityDesc(lobj, robj);
        }

        public static int CompareToBaseTypeDesc(StorageDataItemBase lobj, StorageDataItemBase robj)
        {
            if (lobj.GetBaseType() != robj.GetBaseType())
            {
                return (robj.GetBaseType() - lobj.GetBaseType());
            }
            return CompareToRarityDesc(lobj, robj);
        }

        public static int CompareToCostAsc(StorageDataItemBase lobj, StorageDataItemBase robj)
        {
            if (lobj.GetCost() != robj.GetCost())
            {
                return (lobj.GetCost() - robj.GetCost());
            }
            return CompareToRarityDesc(lobj, robj);
        }

        public static int CompareToCostDesc(StorageDataItemBase lobj, StorageDataItemBase robj)
        {
            if (lobj.GetCost() != robj.GetCost())
            {
                return (robj.GetCost() - lobj.GetCost());
            }
            return CompareToRarityDesc(lobj, robj);
        }

        public static int CompareToLevelAsc(StorageDataItemBase lobj, StorageDataItemBase robj)
        {
            if (lobj.level != robj.level)
            {
                return (lobj.level - robj.level);
            }
            if (lobj.exp != robj.exp)
            {
                return (robj.exp - lobj.exp);
            }
            if (lobj.rarity != robj.rarity)
            {
                return (robj.rarity - lobj.rarity);
            }
            if (lobj.GetSubRarity() != robj.GetSubRarity())
            {
                return (robj.GetSubRarity() - lobj.GetSubRarity());
            }
            if (lobj.ID != robj.ID)
            {
                return (lobj.ID - robj.ID);
            }
            return (robj.uid - lobj.uid);
        }

        public static int CompareToLevelDesc(StorageDataItemBase lobj, StorageDataItemBase robj)
        {
            if (lobj.level != robj.level)
            {
                return (robj.level - lobj.level);
            }
            if (lobj.exp != robj.exp)
            {
                return (robj.exp - lobj.exp);
            }
            if (lobj.rarity != robj.rarity)
            {
                return (robj.rarity - lobj.rarity);
            }
            if (lobj.GetSubRarity() != robj.GetSubRarity())
            {
                return (robj.GetSubRarity() - lobj.GetSubRarity());
            }
            if (lobj.ID != robj.ID)
            {
                return (lobj.ID - robj.ID);
            }
            return (robj.uid - lobj.uid);
        }

        public static int CompareToRarityAsc(StorageDataItemBase lobj, StorageDataItemBase robj)
        {
            if (lobj.rarity != robj.rarity)
            {
                return (lobj.rarity - robj.rarity);
            }
            if (lobj.GetSubRarity() != robj.GetSubRarity())
            {
                return (lobj.GetSubRarity() - robj.GetSubRarity());
            }
            if (lobj.level != robj.level)
            {
                return (robj.level - lobj.level);
            }
            if (lobj.exp != robj.exp)
            {
                return (robj.exp - lobj.exp);
            }
            if (lobj.ID != robj.ID)
            {
                return (lobj.ID - robj.ID);
            }
            return (robj.uid - lobj.uid);
        }

        public static int CompareToRarityDesc(StorageDataItemBase lobj, StorageDataItemBase robj)
        {
            if (lobj.rarity != robj.rarity)
            {
                return (robj.rarity - lobj.rarity);
            }
            if (lobj.GetSubRarity() != robj.GetSubRarity())
            {
                return (robj.GetSubRarity() - lobj.GetSubRarity());
            }
            if (lobj.level != robj.level)
            {
                return (robj.level - lobj.level);
            }
            if (lobj.exp != robj.exp)
            {
                return (robj.exp - lobj.exp);
            }
            if (lobj.ID != robj.ID)
            {
                return (lobj.ID - robj.ID);
            }
            return (robj.uid - lobj.uid);
        }

        public static int CompareToUidAsc(StorageDataItemBase lobj, StorageDataItemBase robj)
        {
            return (lobj.uid - robj.uid);
        }

        public static int CompareToUidDesc(StorageDataItemBase lobj, StorageDataItemBase robj)
        {
            return (robj.uid - lobj.uid);
        }

        public abstract float GetAttackAdd();
        public abstract int GetBaseType();
        public abstract string GetBaseTypeName();
        public abstract int GetCoinNeedToUpLevel();
        public abstract int GetCoinNeedToUpRarity();
        public abstract int GetCost();
        public abstract float GetCriticalAdd();
        public abstract float GetDefenceAdd();
        public abstract string GetDescription();
        public abstract string GetDisplayTitle();
        public abstract List<KeyValuePair<int, int>> GetEvoMaterial();
        public abstract StorageDataItemBase GetEvoStorageItem();
        public abstract int GetExpType();
        public abstract float GetGearExp();
        public abstract float GetHPAdd();
        public abstract string GetIconPath();
        public abstract int GetIdForKey();
        public abstract string GetImagePath();
        public abstract int GetMaxExp();
        public abstract int GetMaxLevel();
        public abstract int GetMaxRarity();
        public abstract int GetMaxSubRarity();
        public abstract float GetPriceForSell();
        public abstract float GetSPAdd();
        public abstract int GetSubRarity();
        public abstract void UpLevel();
        public abstract void UpRarity();
    }
}

