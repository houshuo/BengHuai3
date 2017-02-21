namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class AvatarCardDataItem : StorageDataItemBase
    {
        private AvatarCardMetaData _metaData;
        private bool isSplite;
        private int spliteFragmentNum;

        public AvatarCardDataItem(AvatarCardMetaData avatarCardMetaData)
        {
            base.uid = 0;
            this._metaData = avatarCardMetaData;
            base.ID = this._metaData.ID;
            base.rarity = this._metaData.rarity;
            base.level = 1;
            base.exp = 0;
            base.number = 1;
            this.isSplite = false;
            this.spliteFragmentNum = 0;
        }

        public override StorageDataItemBase Clone()
        {
            throw new NotImplementedException();
        }

        public override float GetAttackAdd()
        {
            throw new NotImplementedException();
        }

        public float GetAvatarExpProvideNum()
        {
            return this._metaData.characterExpProvide;
        }

        public override int GetBaseType()
        {
            throw new NotImplementedException();
        }

        public override string GetBaseTypeName()
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
            return this._metaData.cost;
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
            return LocalizationGeneralLogic.GetText(this._metaData.displayDescription, new object[0]);
        }

        public override string GetDisplayTitle()
        {
            return LocalizationGeneralLogic.GetText(this._metaData.displayTitle, new object[0]);
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
            return (this._metaData.gearExpProvideBase + (this._metaData.gearExpPorvideAdd * (base.level - 1)));
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
            return this._metaData.imagePath;
        }

        public override int GetMaxExp()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override float GetPriceForSell()
        {
            return (this._metaData.sellPriceBase + (this._metaData.sellPriceAdd * base.level));
        }

        public override float GetSPAdd()
        {
            throw new NotImplementedException();
        }

        public int GetSpliteFragmentNum()
        {
            return this.spliteFragmentNum;
        }

        public override int GetSubRarity()
        {
            return 0;
        }

        public bool IsSplite()
        {
            return this.isSplite;
        }

        public void SpliteToFragment(int num)
        {
            this.isSplite = true;
            this.spliteFragmentNum = num;
        }

        public override void UpLevel()
        {
        }

        public override void UpRarity()
        {
        }
    }
}

