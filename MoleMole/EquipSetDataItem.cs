namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class EquipSetDataItem
    {
        private EquipmentSetMetaData _metaData;
        public readonly int ID;
        public readonly int ownNum;

        public EquipSetDataItem(int id, int ownNum = 0)
        {
            this.ID = id;
            this.ownNum = ownNum;
            this._metaData = EquipmentSetMetaDataReader.GetEquipmentSetMetaDataByKey(id);
            this.EquipSkillDict = new SortedDictionary<int, EquipSkillDataItem>();
            if (this._metaData != null)
            {
                if (this._metaData.prop1ID != 0)
                {
                    this.EquipSkillDict.Add(this._metaData.spellEffectNum1, new EquipSkillDataItem(this._metaData.prop1ID, this._metaData.prop1Param1, this._metaData.prop1Param2, this._metaData.prop1Param3, this._metaData.prop1Param1Add, this._metaData.prop1Param2Add, this._metaData.prop1Param3Add));
                }
                if (this._metaData.prop2ID != 0)
                {
                    this.EquipSkillDict.Add(this._metaData.spellEffectNum2, new EquipSkillDataItem(this._metaData.prop2ID, this._metaData.prop2Param1, this._metaData.prop2Param2, this._metaData.prop2Param3, this._metaData.prop2Param1Add, this._metaData.prop2Param2Add, this._metaData.prop2Param3Add));
                }
                if (this._metaData.prop3ID != 0)
                {
                    this.EquipSkillDict.Add(this._metaData.spellEffectNum3, new EquipSkillDataItem(this._metaData.prop3ID, this._metaData.prop3Param1, this._metaData.prop3Param2, this._metaData.prop3Param3, this._metaData.prop3Param1Add, this._metaData.prop3Param2Add, this._metaData.prop3Param3Add));
                }
            }
        }

        public Dictionary<int, EquipSkillDataItem> GetOwnSetSkills()
        {
            Dictionary<int, EquipSkillDataItem> dictionary = new Dictionary<int, EquipSkillDataItem>();
            foreach (KeyValuePair<int, EquipSkillDataItem> pair in this.EquipSkillDict)
            {
                if (pair.Key <= this.ownNum)
                {
                    dictionary.Add(pair.Key, pair.Value);
                }
            }
            return dictionary;
        }

        public string EquipSetDesc
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.setDesc, new object[0]);
            }
        }

        public string EquipSetName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.setName, new object[0]);
            }
        }

        public SortedDictionary<int, EquipSkillDataItem> EquipSkillDict { get; private set; }
    }
}

