namespace MoleMole
{
    using System;

    public class ItempediaDataAdapter
    {
        private StorageDataItemBase _dummyStorageItemData;
        private object _itemData;

        public ItempediaDataAdapter(object data)
        {
            this._itemData = data;
        }

        public static int CompareToBaseTypeAsc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
        {
            if (lobj.baseType != robj.baseType)
            {
                return (lobj.baseType - robj.baseType);
            }
            return CompareToRarityAsc(lobj, robj);
        }

        public static int CompareToBaseTypeDesc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
        {
            if (lobj.baseType != robj.baseType)
            {
                return (robj.baseType - lobj.baseType);
            }
            return CompareToRarityDesc(lobj, robj);
        }

        public static int CompareToCostAsc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
        {
            if (lobj.cost != robj.cost)
            {
                return (lobj.cost - robj.cost);
            }
            return CompareToRarityAsc(lobj, robj);
        }

        public static int CompareToCostDesc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
        {
            if (lobj.cost != robj.cost)
            {
                return (robj.cost - lobj.cost);
            }
            return CompareToRarityDesc(lobj, robj);
        }

        public static int CompareToLevelAsc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
        {
            if (lobj.level != robj.level)
            {
                return (lobj.level - robj.level);
            }
            if (lobj.rarity != robj.rarity)
            {
                return (lobj.rarity - robj.rarity);
            }
            return (lobj.ID - robj.ID);
        }

        public static int CompareToLevelDesc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
        {
            if (lobj.level != robj.level)
            {
                return (robj.level - lobj.level);
            }
            if (lobj.rarity != robj.rarity)
            {
                return (robj.rarity - lobj.rarity);
            }
            return (robj.ID - lobj.ID);
        }

        public static int CompareToRarityAsc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
        {
            if (lobj.rarity != robj.rarity)
            {
                return (lobj.rarity - robj.rarity);
            }
            if (lobj.level != robj.level)
            {
                return (lobj.level - robj.level);
            }
            return (lobj.ID - robj.ID);
        }

        public static int CompareToRarityDesc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
        {
            if (lobj.rarity != robj.rarity)
            {
                return (robj.rarity - lobj.rarity);
            }
            if (lobj.level != robj.level)
            {
                return (robj.level - lobj.level);
            }
            return (robj.ID - lobj.ID);
        }

        public static int CompareToSuiteAsc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
        {
            if (lobj.suite == robj.suite)
            {
                return CompareToRarityAsc(lobj, robj);
            }
            if (lobj.suite == 0)
            {
                return 1;
            }
            if (robj.suite == 0)
            {
                return -1;
            }
            return (lobj.suite - robj.suite);
        }

        public static int CompareToSuiteDesc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
        {
            if (lobj.suite == robj.suite)
            {
                return CompareToRarityDesc(lobj, robj);
            }
            if (lobj.suite == 0)
            {
                return 1;
            }
            if (robj.suite == 0)
            {
                return -1;
            }
            return (robj.suite - lobj.suite);
        }

        public StorageDataItemBase GetDummyStorageItemData()
        {
            if (this._dummyStorageItemData == null)
            {
                if (this._itemData is WeaponMetaData)
                {
                    WeaponMetaData weaponMetaData = (WeaponMetaData) this._itemData;
                    this._dummyStorageItemData = new WeaponDataItem(0, weaponMetaData);
                    this._dummyStorageItemData.level = weaponMetaData.maxLv;
                }
                else if (this._itemData is StigmataMetaData)
                {
                    StigmataMetaData stigmataMetaData = (StigmataMetaData) this._itemData;
                    this._dummyStorageItemData = new StigmataDataItem(0, stigmataMetaData);
                    this._dummyStorageItemData.level = stigmataMetaData.maxLv;
                    ((StigmataDataItem) this._dummyStorageItemData).SetAffixSkill(true, 0, 0);
                }
                else if (this._itemData is ItemMetaData)
                {
                    this._dummyStorageItemData = new MaterialDataItem((ItemMetaData) this._itemData);
                }
            }
            return this._dummyStorageItemData;
        }

        public int baseType
        {
            get
            {
                if (this._itemData is WeaponMetaData)
                {
                    return ((WeaponMetaData) this._itemData).baseType;
                }
                if (this._itemData is StigmataMetaData)
                {
                    return ((StigmataMetaData) this._itemData).baseType;
                }
                if (this._itemData is ItemMetaData)
                {
                    return ((ItemMetaData) this._itemData).BaseType;
                }
                return -1;
            }
        }

        public int cost
        {
            get
            {
                if (this._itemData is WeaponMetaData)
                {
                    return ((WeaponMetaData) this._itemData).cost;
                }
                if (this._itemData is StigmataMetaData)
                {
                    return ((StigmataMetaData) this._itemData).cost;
                }
                if (this._itemData is ItemMetaData)
                {
                    return ((ItemMetaData) this._itemData).cost;
                }
                return -1;
            }
        }

        public string iconPath
        {
            get
            {
                if (this._itemData is WeaponMetaData)
                {
                    return ((WeaponMetaData) this._itemData).iconPath;
                }
                if (this._itemData is StigmataMetaData)
                {
                    return ((StigmataMetaData) this._itemData).iconPath;
                }
                if (this._itemData is ItemMetaData)
                {
                    return ((ItemMetaData) this._itemData).iconPath;
                }
                return null;
            }
        }

        public int ID
        {
            get
            {
                if (this._itemData is WeaponMetaData)
                {
                    return ((WeaponMetaData) this._itemData).ID;
                }
                if (this._itemData is StigmataMetaData)
                {
                    return ((StigmataMetaData) this._itemData).ID;
                }
                if (this._itemData is ItemMetaData)
                {
                    return ((ItemMetaData) this._itemData).ID;
                }
                return -1;
            }
        }

        public int level
        {
            get
            {
                if (this._itemData is WeaponMetaData)
                {
                    return ((WeaponMetaData) this._itemData).maxLv;
                }
                if (this._itemData is StigmataMetaData)
                {
                    return ((StigmataMetaData) this._itemData).maxLv;
                }
                if (this._itemData is ItemMetaData)
                {
                    return ((ItemMetaData) this._itemData).maxLv;
                }
                return -1;
            }
        }

        public int maxRarity
        {
            get
            {
                if (this._itemData is WeaponMetaData)
                {
                    return ((WeaponMetaData) this._itemData).maxRarity;
                }
                if (this._itemData is StigmataMetaData)
                {
                    return ((StigmataMetaData) this._itemData).maxRarity;
                }
                if (this._itemData is ItemMetaData)
                {
                    return ((ItemMetaData) this._itemData).rarity;
                }
                return -1;
            }
        }

        public string name
        {
            get
            {
                if (this._itemData is WeaponMetaData)
                {
                    return ((WeaponMetaData) this._itemData).name;
                }
                if (this._itemData is StigmataMetaData)
                {
                    return ((StigmataMetaData) this._itemData).name;
                }
                if (this._itemData is ItemMetaData)
                {
                    return ((ItemMetaData) this._itemData).name;
                }
                return null;
            }
        }

        public int rarity
        {
            get
            {
                if (this._itemData is WeaponMetaData)
                {
                    return ((WeaponMetaData) this._itemData).rarity;
                }
                if (this._itemData is StigmataMetaData)
                {
                    return ((StigmataMetaData) this._itemData).rarity;
                }
                if (this._itemData is ItemMetaData)
                {
                    return ((ItemMetaData) this._itemData).rarity;
                }
                return -1;
            }
        }

        public int suite
        {
            get
            {
                if (this._itemData is StigmataMetaData)
                {
                    return ((StigmataMetaData) this._itemData).setID;
                }
                return -1;
            }
        }
    }
}

