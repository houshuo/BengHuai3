namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class AvatarSkillDataItem
    {
        private Dictionary<int, AvatarSubSkillDataItem> _avatarSubSkillMap;
        private AvatarSkillMetaData _metaData;
        private int avatarID;
        public List<AvatarSubSkillDataItem> avatarSubSkillList;
        private const int LEADER_SKILL_ORDER = 5;
        private const char LOGIC_ADD = '+';
        private const char LOGIC_MINUS = '-';
        private const char LOGIC_NONE = '0';
        private const char LOGIC_REPLACE = 'R';
        public int skillID;
        public bool UnLocked;

        public AvatarSkillDataItem(int skillID, int avatarID)
        {
            this.skillID = skillID;
            this.avatarID = avatarID;
            this.UnLocked = false;
            this._metaData = AvatarSkillMetaDataReader.GetAvatarSkillMetaDataByKey(skillID);
            this.SetupDefaultSubSkillList();
        }

        private void ApplySubSkillToSkillPara(float subParaValue, char logic, ref float paraValue)
        {
            char ch = logic;
            switch (ch)
            {
                case '+':
                    paraValue += subParaValue;
                    return;

                case '-':
                    paraValue -= subParaValue;
                    return;
            }
            if (ch == 'R')
            {
                paraValue = subParaValue;
            }
        }

        public AvatarSubSkillDataItem GetAvatarSubSkillBySubSkillId(int subSkillID)
        {
            return this._avatarSubSkillMap[subSkillID];
        }

        public int GetLevelSum()
        {
            int num = 0;
            foreach (AvatarSubSkillDataItem item in this.avatarSubSkillList)
            {
                num += item.level;
            }
            return num;
        }

        public int GetMaxLevelSum()
        {
            int num = 0;
            foreach (AvatarSubSkillDataItem item in this.avatarSubSkillList)
            {
                num += item.MaxLv;
            }
            return num;
        }

        public float GetParam1()
        {
            float paraValue = this._metaData.paramBase_1;
            if (((this._metaData.paramLogic_1 != '0') && this._avatarSubSkillMap.ContainsKey(this._metaData.paramSubID_1)) && this._avatarSubSkillMap[this._metaData.paramSubID_1].UnLocked)
            {
                float subParaValue = this._avatarSubSkillMap[this._metaData.paramSubID_1].GetParaValue(this._metaData.paramSubIndex_1);
                this.ApplySubSkillToSkillPara(subParaValue, this._metaData.paramLogic_1, ref paraValue);
            }
            return paraValue;
        }

        public float GetParam2()
        {
            float paraValue = this._metaData.paramBase_2;
            if (((this._metaData.paramLogic_2 != '0') && this._avatarSubSkillMap.ContainsKey(this._metaData.paramSubID_2)) && this._avatarSubSkillMap[this._metaData.paramSubID_2].UnLocked)
            {
                float subParaValue = this._avatarSubSkillMap[this._metaData.paramSubID_2].GetParaValue(this._metaData.paramSubIndex_2);
                this.ApplySubSkillToSkillPara(subParaValue, this._metaData.paramLogic_2, ref paraValue);
            }
            return paraValue;
        }

        public float GetParam3()
        {
            float paraValue = this._metaData.paramBase_3;
            if (((this._metaData.paramLogic_3 != '0') && this._avatarSubSkillMap.ContainsKey(this._metaData.paramSubID_3)) && this._avatarSubSkillMap[this._metaData.paramSubID_3].UnLocked)
            {
                float subParaValue = this._avatarSubSkillMap[this._metaData.paramSubID_3].GetParaValue(this._metaData.paramSubIndex_3);
                this.ApplySubSkillToSkillPara(subParaValue, this._metaData.paramLogic_3, ref paraValue);
            }
            return paraValue;
        }

        private void SetupDefaultSubSkillList()
        {
            this.avatarSubSkillList = new List<AvatarSubSkillDataItem>();
            this._avatarSubSkillMap = new Dictionary<int, AvatarSubSkillDataItem>();
            foreach (int num in AvatarSubSkillMetaDataReaderExtend.GetAvatarSubSkillIdList(this.skillID))
            {
                AvatarSubSkillDataItem item = new AvatarSubSkillDataItem(num, this.avatarID);
                this.avatarSubSkillList.Add(item);
                this._avatarSubSkillMap.Add(num, item);
            }
        }

        public void SetupSubSkillListFromServer(AvatarSkillDetailData skillData, int avatarLevel, int avatarStar)
        {
            this.SetupDefaultSubSkillList();
            if (this.skillID == skillData.get_skill_id())
            {
                foreach (AvatarSubSkillDetailData data in skillData.get_sub_skill_list())
                {
                    if (this._avatarSubSkillMap.ContainsKey((int) data.get_sub_skill_id()))
                    {
                        this._avatarSubSkillMap[(int) data.get_sub_skill_id()].level = (int) data.get_level();
                    }
                }
            }
            this.UnLocked = (avatarLevel >= this.UnLockLv) && (avatarStar >= this.UnLockStar);
        }

        public bool ShouldShowHintPointForSubSkill()
        {
            if (this.UnLocked)
            {
                foreach (AvatarSubSkillDataItem item in this.avatarSubSkillList)
                {
                    if (item.ShouldShowHintPoint())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string ButtonName
        {
            get
            {
                return this._metaData.buttonName;
            }
        }

        public bool CanTry
        {
            get
            {
                return (this._metaData.canTry == 1);
            }
        }

        public string IconPath
        {
            get
            {
                return this._metaData.iconPath;
            }
        }

        public string IconPathInLevel
        {
            get
            {
                return this._metaData.iconPathInLevel;
            }
        }

        public bool IsLeaderSkill
        {
            get
            {
                return (this.ShowOrder == 5);
            }
        }

        public int ShowOrder
        {
            get
            {
                return this._metaData.showOrder;
            }
        }

        public string SkillInfo
        {
            get
            {
                object[] replaceParams = new object[] { this.GetParam1(), this.GetParam2(), this.GetParam3() };
                return LocalizationGeneralLogic.GetText(this._metaData.info, replaceParams).Replace("{{", string.Empty).Replace("}}", string.Empty);
            }
        }

        public string SkillName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.name, new object[0]);
            }
        }

        public string SkillShortInfo
        {
            get
            {
                string pattern = "{{(.*)}}";
                object[] replaceParams = new object[] { this.GetParam1(), this.GetParam2(), this.GetParam3() };
                string text = LocalizationGeneralLogic.GetText(this._metaData.info, replaceParams);
                if (Regex.IsMatch(text, pattern))
                {
                    return Regex.Match(text, pattern).Groups[1].Value.ToString();
                }
                return this.SkillInfo;
            }
        }

        public string SkillStep
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.skillStep, new object[0]);
            }
        }

        public int UnLockLv
        {
            get
            {
                return this._metaData.unlockLv;
            }
        }

        public int UnLockStar
        {
            get
            {
                return this._metaData.unlockStar;
            }
        }
    }
}

