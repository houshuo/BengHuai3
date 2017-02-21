namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class EndlessToolDataItem : StorageDataItemBase
    {
        private EndlessToolMetaData _metaData;

        public EndlessToolDataItem(int metaID, int number = 1)
        {
            base.uid = 0;
            this._metaData = EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey(metaID);
            base.ID = metaID;
            base.rarity = this._metaData.rarity;
            base.level = 1;
            base.exp = 0;
            base.number = number;
        }

        public override StorageDataItemBase Clone()
        {
            return new EndlessToolDataItem(base.ID, 1) { level = base.level, exp = base.exp, number = base.number };
        }

        public override float GetAttackAdd()
        {
            throw new NotImplementedException();
        }

        public float GetAvatarExpProvideNum()
        {
            return 0f;
        }

        public override int GetBaseType()
        {
            throw new NotImplementedException();
        }

        public override string GetBaseTypeName()
        {
            throw new NotImplementedException();
        }

        public string GetBGDescription()
        {
            throw new NotImplementedException();
        }

        public override int GetCoinNeedToUpLevel()
        {
            throw new NotImplementedException();
        }

        public override int GetCoinNeedToUpRarity()
        {
            throw new NotImplementedException();
        }

        public override int GetCost()
        {
            return 0;
        }

        public override float GetCriticalAdd()
        {
            throw new NotImplementedException();
        }

        public override float GetDefenceAdd()
        {
            throw new NotImplementedException();
        }

        public override string GetDescription()
        {
            return LocalizationGeneralLogic.GetText(this._metaData.description, new object[0]);
        }

        public override string GetDisplayTitle()
        {
            return LocalizationGeneralLogic.GetText(this._metaData.name, new object[0]);
        }

        public override List<KeyValuePair<int, int>> GetEvoMaterial()
        {
            throw new NotImplementedException();
        }

        public override StorageDataItemBase GetEvoStorageItem()
        {
            return null;
        }

        public override int GetExpType()
        {
            throw new NotImplementedException();
        }

        public override float GetGearExp()
        {
            return 0f;
        }

        public override float GetHPAdd()
        {
            throw new NotImplementedException();
        }

        public override string GetIconPath()
        {
            return this._metaData.iconPath;
        }

        public override int GetIdForKey()
        {
            return base.ID;
        }

        public override string GetImagePath()
        {
            throw new NotImplementedException();
        }

        public override int GetMaxExp()
        {
            throw new NotImplementedException();
        }

        public override int GetMaxLevel()
        {
            return base.level;
        }

        public override int GetMaxRarity()
        {
            return this._metaData.rarity;
        }

        public override int GetMaxSubRarity()
        {
            throw new NotImplementedException();
        }

        public override float GetPriceForSell()
        {
            return 0f;
        }

        public string GetSmallIconPath()
        {
            return this._metaData.smallIconPath;
        }

        public override float GetSPAdd()
        {
            throw new NotImplementedException();
        }

        public override int GetSubRarity()
        {
            return 0;
        }

        public int GetTimeSpanInSeconds()
        {
            return this._metaData.paramTime;
        }

        public override void UpLevel()
        {
        }

        public override void UpRarity()
        {
        }

        public bool ApplyToSelf
        {
            get
            {
                return ((this.ToolType == 1) || (this.ToolType == 5));
            }
        }

        public string EffectPrefatPath
        {
            get
            {
                return this._metaData.effectPath;
            }
        }

        public string ParamString
        {
            get
            {
                if (string.IsNullOrEmpty(this._metaData.paramStr))
                {
                    return null;
                }
                return this._metaData.paramStr;
            }
        }

        public string ReportTextMapId
        {
            get
            {
                return this._metaData.report;
            }
        }

        public bool ShowIcon
        {
            get
            {
                return (this._metaData.showIcon != 0);
            }
        }

        public EndlessItemType ToolType
        {
            get
            {
                return (EndlessItemType) this._metaData.type;
            }
        }
    }
}

