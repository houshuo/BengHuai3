namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class StigmataDataItem : StorageDataItemBase
    {
        private List<AffixSkillData> _affixSkillList;
        private EquipSetDataItem _equipSetData;
        private StigmataMetaData _metaData;
        public float durability;
        public const int MAX_UID_NUM = 0x9c40;
        public const int UID_BEGIN_NUM = 0x4e20;

        public StigmataDataItem(int uid, StigmataMetaData stigmataMetaData)
        {
            base.uid = uid;
            this._metaData = stigmataMetaData;
            base.ID = this._metaData.ID;
            base.rarity = this._metaData.rarity;
            base.level = 1;
            this.durability = this._metaData.durabilityMax;
            base.exp = 0;
            base.number = 1;
            if (this._metaData != null)
            {
                if (this._metaData.setID != 0)
                {
                    this._equipSetData = new EquipSetDataItem(this._metaData.setID, 0);
                }
                else
                {
                    this._equipSetData = null;
                }
                this.skills = this.GetSkills();
            }
        }

        public override StorageDataItemBase Clone()
        {
            StigmataDataItem item = new StigmataDataItem(base.uid, this._metaData) {
                level = base.level,
                exp = base.exp,
                number = base.number
            };
            int num = (this.PreAffixSkill != null) ? this.PreAffixSkill.affixID : 0;
            int num2 = (this.SufAffixSkill != null) ? this.SufAffixSkill.affixID : 0;
            item.SetAffixSkill(this.IsAffixIdentify, num, num2);
            return item;
        }

        public string GetAffixName()
        {
            string str = string.Empty;
            if (this.IsAffixIdentify)
            {
                if ((this.PreAffixSkill != null) && (this.SufAffixSkill != null))
                {
                    return (this.PreAffixSkill.NameDual + this.SufAffixSkill.NameDual);
                }
                if ((this.PreAffixSkill != null) && (this.SufAffixSkill == null))
                {
                    return this.PreAffixSkill.NameMono;
                }
                if ((this.PreAffixSkill != null) || (this.SufAffixSkill == null))
                {
                    return str;
                }
                return this.SufAffixSkill.NameMono;
            }
            return LocalizationGeneralLogic.GetText("Menu_NotIdentifyAffix", new object[0]);
        }

        public List<AffixSkillData> GetAffixSkillList()
        {
            if (this._affixSkillList == null)
            {
                return new List<AffixSkillData>();
            }
            return this._affixSkillList;
        }

        public SortedDictionary<int, EquipSkillDataItem> GetAllSetSkills()
        {
            if (this._equipSetData != null)
            {
                return this._equipSetData.EquipSkillDict;
            }
            return new SortedDictionary<int, EquipSkillDataItem>();
        }

        public override float GetAttackAdd()
        {
            return (this._metaData.attackBase + (this._metaData.attackAdd * (base.level - 1)));
        }

        public float GetAttackAddWithAffix(AvatarDataItem avatarData)
        {
            float num = this._metaData.attackBase + (this._metaData.attackAdd * (base.level - 1));
            float num2 = 0f;
            if (this._affixSkillList != null)
            {
                foreach (AffixSkillData data in this._affixSkillList)
                {
                    num2 += data.GetAttrAdd(avatarData, 3);
                }
            }
            return (num + num2);
        }

        public override int GetBaseType()
        {
            return this._metaData.baseType;
        }

        public override string GetBaseTypeName()
        {
            return LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.StigmataBaseTypeName[this._metaData.baseType], new object[0]);
        }

        public override int GetCoinNeedToUpLevel()
        {
            return EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(base.level).stigmataUpgradeCost;
        }

        public override int GetCoinNeedToUpRarity()
        {
            return EquipmentLevelMetaDataReader.GetEquipmentLevelMetaDataByKey(base.level).stigmataEvoCost;
        }

        public override int GetCost()
        {
            return this._metaData.cost;
        }

        public override float GetCriticalAdd()
        {
            return (this._metaData.criticalBase + (this._metaData.criticalAdd * (base.level - 1)));
        }

        public float GetCriticalAddWithAffix(AvatarDataItem avatarData)
        {
            float num = this._metaData.criticalBase + (this._metaData.criticalAdd * (base.level - 1));
            float num2 = 0f;
            if (this._affixSkillList != null)
            {
                foreach (AffixSkillData data in this._affixSkillList)
                {
                    num2 += data.GetAttrAdd(avatarData, 5);
                }
            }
            return (num + num2);
        }

        public override float GetDefenceAdd()
        {
            return (this._metaData.defenceBase + (this._metaData.defenceAdd * (base.level - 1)));
        }

        public float GetDefenceAddWithAffix(AvatarDataItem avatarData)
        {
            float num = this._metaData.defenceBase + (this._metaData.defenceAdd * (base.level - 1));
            float num2 = 0f;
            if (this._affixSkillList != null)
            {
                foreach (AffixSkillData data in this._affixSkillList)
                {
                    num2 += data.GetAttrAdd(avatarData, 4);
                }
            }
            return (num + num2);
        }

        public override string GetDescription()
        {
            return LocalizationGeneralLogic.GetText(this._metaData.displayDescription, new object[0]);
        }

        public override string GetDisplayTitle()
        {
            return LocalizationGeneralLogic.GetText(this._metaData.displayTitle, new object[0]);
        }

        public int GetEquipmentSetID()
        {
            return this._metaData.setID;
        }

        public string GetEquipSetName()
        {
            if (this._equipSetData != null)
            {
                return this._equipSetData.EquipSetName;
            }
            return string.Empty;
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
            StigmataDataItem item;
            if (this._metaData.evoID == 0)
            {
                return null;
            }
            return new StigmataDataItem(0, StigmataMetaDataReader.GetStigmataMetaDataByKey(this._metaData.evoID)) { level = item.GetMaxLevel() };
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

        public float GetHPAddWithAffix(AvatarDataItem avatarData)
        {
            float num = this._metaData.HPBase + (this._metaData.HPAdd * (base.level - 1));
            float num2 = 0f;
            if (this._affixSkillList != null)
            {
                foreach (AffixSkillData data in this._affixSkillList)
                {
                    num2 += data.GetAttrAdd(avatarData, 1);
                }
            }
            return (num + num2);
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

        public float GetOffesetX()
        {
            return this._metaData.offsetX;
        }

        public float GetOffesetY()
        {
            return this._metaData.offsetY;
        }

        public float GetPowerUpConf()
        {
            return PowerTypeMetaDataReader.GetPowerTypeMetaDataByKey(this._metaData.powerType).powerConf;
        }

        public override float GetPriceForSell()
        {
            return (this._metaData.sellPriceBase + (this._metaData.sellPriceAdd * (base.level - 1)));
        }

        public float GetScale()
        {
            return this._metaData.scale;
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

        public List<EquipSkillDataItem> GetSkillsWithAffix()
        {
            List<EquipSkillDataItem> list = new List<EquipSkillDataItem>();
            list.AddRange(this.skills);
            if (this.PreAffixSkill != null)
            {
                list.Add(this.PreAffixSkill.skill);
            }
            if (this.SufAffixSkill != null)
            {
                list.Add(this.SufAffixSkill.skill);
            }
            return list;
        }

        public string GetSmallIconPath()
        {
            return this._metaData.smallIcon;
        }

        public override float GetSPAdd()
        {
            return (this._metaData.SPBase + (this._metaData.SPAdd * (base.level - 1)));
        }

        public float GetSPAddWithAffix(AvatarDataItem avatarData)
        {
            float num = this._metaData.SPBase + (this._metaData.SPAdd * (base.level - 1));
            float num2 = 0f;
            if (this._affixSkillList != null)
            {
                foreach (AffixSkillData data in this._affixSkillList)
                {
                    num2 += data.GetAttrAdd(avatarData, 2);
                }
            }
            return (num + num2);
        }

        public override int GetSubRarity()
        {
            return this._metaData.subRarity;
        }

        public string GetTattooPath()
        {
            return this._metaData.tattooPath;
        }

        public void SetAffixSkill(bool is_affix_identify, int pre_affix_id, int suf_affix_id)
        {
            this.IsAffixIdentify = is_affix_identify;
            this.PreAffixSkill = (pre_affix_id != 0) ? new AffixSkillData(pre_affix_id) : null;
            this.SufAffixSkill = (suf_affix_id != 0) ? new AffixSkillData(suf_affix_id) : null;
            this._affixSkillList = new List<AffixSkillData>();
            if (this.PreAffixSkill != null)
            {
                this._affixSkillList.Add(this.PreAffixSkill);
            }
            if (this.SufAffixSkill != null)
            {
                this._affixSkillList.Add(this.SufAffixSkill);
            }
        }

        public void SetDummyAffixSkill()
        {
            this.SetAffixSkill(true, 0, 0);
        }

        public override void UpLevel()
        {
        }

        public override void UpRarity()
        {
        }

        public bool CanRefine
        {
            get
            {
                return (this._metaData.canRefine == 1);
            }
        }

        public bool IsAffixIdentify { get; private set; }

        public AffixSkillData PreAffixSkill { get; private set; }

        public List<EquipSkillDataItem> skills { get; private set; }

        public AffixSkillData SufAffixSkill { get; private set; }

        public class AffixSkillData
        {
            private StigmataAffixMetaData _metaData;
            public readonly int affixID;
            public readonly EquipSkillDataItem skill;

            public AffixSkillData(int affixID)
            {
                this.affixID = affixID;
                this._metaData = StigmataAffixMetaDataReader.GetStigmataAffixMetaDataByKey(affixID);
                this.skill = new EquipSkillDataItem(this._metaData.propID, this._metaData.PropParam1, this._metaData.PropParam2, this._metaData.PropParam3, 0f, 0f, 0f);
            }

            public float GetAttrAdd(AvatarDataItem avatarData, int attrType)
            {
                if ((this._metaData.UINature != 0) && ((avatarData == null) || (this._metaData.UINature != avatarData.Attribute)))
                {
                    return 0f;
                }
                if ((this._metaData.UIClass != 0) && ((avatarData == null) || (this._metaData.UIClass != avatarData.ClassId)))
                {
                    return 0f;
                }
                if (this._metaData.UIType != attrType)
                {
                    return 0f;
                }
                return this._metaData.UIValue;
            }

            public string NameDual
            {
                get
                {
                    return LocalizationGeneralLogic.GetText(this._metaData.nameDual, new object[0]);
                }
            }

            public string NameMono
            {
                get
                {
                    return LocalizationGeneralLogic.GetText(this._metaData.nameMono, new object[0]);
                }
            }
        }
    }
}

