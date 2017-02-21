namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class EquipSkillDataItem
    {
        private EquipSkillMetaData _metaData;
        private float _param1;
        private float _param1Add;
        private float _param2;
        private float _param2Add;
        private float _param3;
        private float _param3Add;

        public EquipSkillDataItem(int id, float param1, float param2, float param3, float param1Add, float param2Add, float param3Add)
        {
            this.ID = id;
            this._metaData = EquipSkillMetaDataReader.GetEquipSkillMetaDataByKey(id);
            if (this._metaData == null)
            {
            }
            this._param1 = param1;
            this._param1Add = param1Add;
            this._param2 = param2;
            this._param2Add = param2Add;
            this._param3 = param3;
            this._param3Add = param3Add;
        }

        public string GetSkillDisplay(int equipLevel = 1)
        {
            return LocalizationGeneralLogic.GetTextWithParamArray<float>(this._metaData.skillDisplay, MiscData.GetColor("Blue"), this.GetSkillParamArray(equipLevel));
        }

        public float GetSkillParam1(int equipLevel)
        {
            if (equipLevel < 1)
            {
                return 0f;
            }
            return (this._param1 + ((equipLevel - 1) * this._param1Add));
        }

        public float GetSkillParam2(int equipLevel)
        {
            if (equipLevel < 1)
            {
                return 0f;
            }
            return (this._param2 + ((equipLevel - 1) * this._param2Add));
        }

        public float GetSkillParam3(int equipLevel)
        {
            if (equipLevel < 1)
            {
                return 0f;
            }
            return (this._param3 + ((equipLevel - 1) * this._param3Add));
        }

        public float[] GetSkillParamArray(int equipLevel = 1)
        {
            return new float[] { this.GetSkillParam1(equipLevel), this.GetSkillParam2(equipLevel), this.GetSkillParam3(equipLevel) };
        }

        public List<float> GetSkillParamList(int equipLevel)
        {
            if (equipLevel < 1)
            {
                return null;
            }
            return new List<float> { this.GetSkillParam1(equipLevel), this.GetSkillParam2(equipLevel), this.GetSkillParam3(equipLevel) };
        }

        private string GetSkillParaToDisplay(float paraValue)
        {
            return paraValue.ToString();
        }

        public int ID { get; private set; }

        public string skillName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.skillName, new object[0]);
            }
        }
    }
}

