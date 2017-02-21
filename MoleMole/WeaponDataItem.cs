namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class WeaponDataItem : StorageDataItemBase
    {
        private WeaponMetaData _metaData;
        public float durability;
        public const int MAX_UID_NUM = 0x4e20;
        public const int UID_BEGIN_NUM = 0;

        public WeaponDataItem(int uid, WeaponMetaData weaponMetaData)
        {
            base.uid = uid;
            this._metaData = weaponMetaData;
            base.ID = this._metaData.ID;
            base.rarity = this._metaData.rarity;
            base.level = 1;
            this.durability = this._metaData.durabilityMax;
            base.exp = 0;
            base.number = 1;
            if (this._metaData != null)
            {
                this.skills = this.GetSkills();
            }
        }

        public override StorageDataItemBase Clone()
        {
            return new WeaponDataItem(base.uid, this._metaData) { level = base.level, exp = base.exp, number = base.number };
        }

        public override float GetAttackAdd()
        {
            return (this._metaData.attackBase + (this._metaData.attackAdd * (base.level - 1)));
        }

        public override int GetBaseType()
        {
            return this._metaData.baseType;
        }

        public override string GetBaseTypeName()
        {
            return LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.WeaponBaseTypeName[this._metaData.baseType], new object[0]);
        }

        public override int GetCoinNeedToUpLevel()
        {
            return EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(base.level).weaponUpgradeCost;
        }

        public override int GetCoinNeedToUpRarity()
        {
            return EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(base.level).weaponEvoCost;
        }

        public override int GetCost()
        {
            return this._metaData.cost;
        }

        public override float GetCriticalAdd()
        {
            return (this._metaData.criticalBase + (this._metaData.criticalAdd * (base.level - 1)));
        }

        public override float GetDefenceAdd()
        {
            return (this._metaData.defenceBase + (this._metaData.defenceAdd * (base.level - 1)));
        }

        public override string GetDescription()
        {
            return LocalizationGeneralLogic.GetText(this._metaData.displayDescription, new object[0]);
        }

        public override string GetDisplayTitle()
        {
            return LocalizationGeneralLogic.GetText(this._metaData.displayTitle, new object[0]);
        }

        public int GetEvoID()
        {
            return this._metaData.evoID;
        }

        public override List<KeyValuePair<int, int>> GetEvoMaterial()
        {
            List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
            foreach (string str in this._metaData.evoMaterial)
            {
                char[] separator = new char[] { ':' };
                string[] strArray = str.Split(separator);
                list.Add(new KeyValuePair<int, int>(int.Parse(strArray[0]), int.Parse(strArray[1])));
            }
            return list;
        }

        public override StorageDataItemBase GetEvoStorageItem()
        {
            WeaponDataItem item;
            if (this._metaData.evoID == 0)
            {
                return null;
            }
            return new WeaponDataItem(0, WeaponMetaDataReader.GetWeaponMetaDataByKey(this._metaData.evoID)) { level = item.GetMaxLevel() };
        }

        public override int GetExpType()
        {
            return this._metaData.expType;
        }

        public override float GetGearExp()
        {
            return (this._metaData.gearExpProvideBase + (this._metaData.gearExpPorvideAdd * (base.level - 1)));
        }

        public override float GetHPAdd()
        {
            return (this._metaData.HPBase + (this._metaData.HPAdd * (base.level - 1)));
        }

        public override string GetIconPath()
        {
            return this._metaData.iconPath;
        }

        public override int GetIdForKey()
        {
            return base.uid;
        }

        public override string GetImagePath()
        {
            return this._metaData.imagePath;
        }

        public override int GetMaxExp()
        {
            return EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(base.level).expList[this._metaData.expType];
        }

        public override int GetMaxLevel()
        {
            return this._metaData.maxLv;
        }

        public override int GetMaxRarity()
        {
            return this._metaData.maxRarity;
        }

        public override int GetMaxSubRarity()
        {
            return this._metaData.subMaxRarity;
        }

        public float GetPowerUpConf()
        {
            return PowerTypeMetaDataReader.GetPowerTypeMetaDataByKey(this._metaData.powerType).powerConf;
        }

        public string GetPrefabPath()
        {
            return this._metaData.bodyMod;
        }

        public override float GetPriceForSell()
        {
            return (this._metaData.sellPriceBase + (this._metaData.sellPriceAdd * (base.level - 1)));
        }

        private List<EquipSkillDataItem> GetSkills()
        {
            List<EquipSkillDataItem> list = new List<EquipSkillDataItem>();
            if (this._metaData != null)
            {
                if (this._metaData.prop1ID > 0)
                {
                    list.Add(new EquipSkillDataItem(this._metaData.prop1ID, this._metaData.prop1Param1, this._metaData.prop1Param2, this._metaData.prop1Param3, this._metaData.prop1Param1Add, this._metaData.prop1Param2Add, this._metaData.prop1Param3Add));
                }
                if (this._metaData.prop2ID > 0)
                {
                    list.Add(new EquipSkillDataItem(this._metaData.prop2ID, this._metaData.prop2Param1, this._metaData.prop2Param2, this._metaData.prop2Param3, this._metaData.prop2Param1Add, this._metaData.prop2Param2Add, this._metaData.prop2Param3Add));
                }
                if (this._metaData.prop3ID > 0)
                {
                    list.Add(new EquipSkillDataItem(this._metaData.prop3ID, this._metaData.prop3Param1, this._metaData.prop3Param2, this._metaData.prop3Param3, this._metaData.prop3Param1Add, this._metaData.prop3Param2Add, this._metaData.prop3Param3Add));
                }
            }
            return list;
        }

        public override float GetSPAdd()
        {
            return (this._metaData.SPBase + (this._metaData.SPAdd * (base.level - 1)));
        }

        public override int GetSubRarity()
        {
            return this._metaData.subRarity;
        }

        public override void UpLevel()
        {
        }

        public override void UpRarity()
        {
        }

        public List<EquipSkillDataItem> skills { get; private set; }
    }
}

