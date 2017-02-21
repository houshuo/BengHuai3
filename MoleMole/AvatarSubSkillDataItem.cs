namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AvatarSubSkillDataItem
    {
        private int _avatarID;
        private Dictionary<int, NeedItemData> _lvUpNeedItemDict;
        public AvatarSubSkillMetaData _metaData;
        private SubSkillStatus _status;
        private Dictionary<int, NeedItemData> _unlockNeedItemDict;
        public int level;
        public int subSkillID;

        public AvatarSubSkillDataItem(int avatarSubSkillID, int avatarID)
        {
            this.subSkillID = avatarSubSkillID;
            this._avatarID = avatarID;
            this.level = 0;
            this._metaData = AvatarSubSkillMetaDataReader.GetAvatarSubSkillMetaDataByKey(avatarSubSkillID);
        }

        public bool CanLvUp(AvatarDataItem avatar)
        {
            if (this.UnLocked)
            {
                PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
                return (((((avatar.level >= this.LvUpNeedAvatarLevel) && (this.level < this.MaxLv)) && ((playerData.skillPoint >= this.LvUpPoint) && (playerData.scoin >= this.LvUpSCoin))) && this.LvUpHasEnoughItems()) && (avatar.star >= this.GetUpLevelStarNeed()));
            }
            return false;
        }

        public bool CanUnlock(AvatarDataItem avatar)
        {
            if (!this.UnLocked)
            {
                PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
                return ((((avatar.level >= this.UnlockLv) && (avatar.star >= this.UnlockStar)) && ((playerData.skillPoint >= this.UnlockPoint) && (playerData.scoin >= this.UnlockSCoin))) && this.UnlockHasEnoughItems());
            }
            return false;
        }

        public NeedItemData GetLvUpNeedItemDataByID(int metaID)
        {
            NeedItemData data;
            this._lvUpNeedItemDict.TryGetValue(metaID, out data);
            return data;
        }

        public float GetParaValue(int index)
        {
            switch (index)
            {
                case 1:
                    return this.SkillParam_1;

                case 2:
                    return this.SkillParam_2;

                case 3:
                    return this.SkillParam_3;
            }
            return 0f;
        }

        public NeedItemData GetUnlockNeedItemDataByID(int metaID)
        {
            NeedItemData data;
            this._unlockNeedItemDict.TryGetValue(metaID, out data);
            return data;
        }

        public int GetUpLevelStarNeed()
        {
            int starNeed = 0;
            foreach (AvatarSubSkillMetaData.UpLevelStarNeed need in this._metaData.upLevelStarNeedList)
            {
                if (need.level == (this.level + 1))
                {
                    starNeed = need.starNeed;
                }
            }
            return starNeed;
        }

        public bool LvUpHasEnoughItems()
        {
            bool flag = true;
            foreach (NeedItemData data in this.LvUpNeedItemList)
            {
                flag = flag && Singleton<StorageModule>.Instance.HasEnoughItem(data.itemMetaID, data.itemNum);
            }
            return flag;
        }

        public bool ShouldShowHintPoint()
        {
            return ((this.Status == SubSkillStatus.CanUnlock) || (this.Status == SubSkillStatus.CanUpLevel));
        }

        public bool UnlockHasEnoughItems()
        {
            bool flag = true;
            foreach (NeedItemData data in this.UnlockNeedItemList)
            {
                flag = flag && Singleton<StorageModule>.Instance.HasEnoughItem(data.itemMetaID, data.itemNum);
            }
            return flag;
        }

        private AvatarDataItem _avatar
        {
            get
            {
                return Singleton<AvatarModule>.Instance.GetAvatarByID(this._avatarID);
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

        public string Info
        {
            get
            {
                return LocalizationGeneralLogic.GetTextWithParamArray<float>(this._metaData.info, MiscData.GetColor("Blue"), this.SkillParamArray);
            }
        }

        public int LvUpNeedAvatarLevel
        {
            get
            {
                return Mathf.FloorToInt((float) (this._metaData.unlockLv + (this.level * this._metaData.unlockLvAdd)));
            }
        }

        public List<NeedItemData> LvUpNeedItemList
        {
            get
            {
                List<NeedItemData> list = new List<NeedItemData>();
                this._lvUpNeedItemDict = new Dictionary<int, NeedItemData>();
                if (this.level != 0)
                {
                    List<AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem> list2 = AvatarSubSkillLevelMetaDataReader.GetAvatarSubSkillLevelMetaDataByKey(this.LvUpNeedAvatarLevel).needItemList[this._metaData.lvUpItemType];
                    if (list2 == null)
                    {
                        return list;
                    }
                    foreach (AvatarSubSkillLevelMetaData.SkillUpLevelNeedItem item in list2)
                    {
                        if (item.itemMetaID > 0)
                        {
                            NeedItemData data = new NeedItemData(item.itemMetaID, item.itemNum);
                            list.Add(data);
                            this._lvUpNeedItemDict.Add(item.itemMetaID, data);
                        }
                    }
                }
                return list;
            }
        }

        public int LvUpPoint
        {
            get
            {
                return this._metaData.preLvPoint;
            }
        }

        public int LvUpSCoin
        {
            get
            {
                if (this.level == 0)
                {
                    return 0;
                }
                return AvatarSubSkillLevelMetaDataReader.GetAvatarSubSkillLevelMetaDataByKey(this.LvUpNeedAvatarLevel).needScoinList[this._metaData.lvUpScoinType];
            }
        }

        public int MaxLv
        {
            get
            {
                return this._metaData.maxLv;
            }
        }

        public string Name
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.name, new object[0]);
            }
        }

        public string NextLevelInfo
        {
            get
            {
                return LocalizationGeneralLogic.GetTextWithParamArray<float>(this._metaData.info, MiscData.GetColor("Blue"), this.NextLevelSkillParamArray);
            }
        }

        public float[] NextLevelSkillParamArray
        {
            get
            {
                return new float[] { (this._metaData.paramBase_1 + (this._metaData.paramAdd_1 * this.level)), (this._metaData.paramBase_2 + (this._metaData.paramAdd_2 * this.level)), (this._metaData.paramBase_3 + (this._metaData.paramAdd_3 * this.level)) };
            }
        }

        public int ShowOrder
        {
            get
            {
                return this._metaData.showOrder;
            }
        }

        public float SkillParam_1
        {
            get
            {
                return ((this.level != 0) ? (this._metaData.paramBase_1 + (this._metaData.paramAdd_1 * (this.level - 1))) : this._metaData.paramBase_1);
            }
        }

        public float SkillParam_2
        {
            get
            {
                return ((this.level != 0) ? (this._metaData.paramBase_2 + (this._metaData.paramAdd_2 * (this.level - 1))) : this._metaData.paramBase_2);
            }
        }

        public float SkillParam_3
        {
            get
            {
                return ((this.level != 0) ? (this._metaData.paramBase_3 + (this._metaData.paramAdd_3 * (this.level - 1))) : this._metaData.paramBase_3);
            }
        }

        public float[] SkillParamArray
        {
            get
            {
                return new float[] { this.SkillParam_1, this.SkillParam_2, this.SkillParam_3 };
            }
        }

        public SubSkillStatus Status
        {
            get
            {
                if (!this.UnLocked)
                {
                    if (((this._avatar != null) && (this._avatar.level >= this.UnlockLv)) && (this._avatar.star >= this.UnlockStar))
                    {
                        this._status = SubSkillStatus.CanUnlock;
                    }
                    else
                    {
                        this._status = SubSkillStatus.Locked;
                    }
                }
                else if (((this.level < this.MaxLv) && (this._avatar != null)) && ((this._avatar.level >= this.LvUpNeedAvatarLevel) && (this._avatar.star >= this.GetUpLevelStarNeed())))
                {
                    this._status = SubSkillStatus.CanUpLevel;
                }
                else
                {
                    this._status = SubSkillStatus.CannotUpLevel;
                }
                return this._status;
            }
        }

        public bool UnLocked
        {
            get
            {
                return (this.level > 0);
            }
        }

        public int UnlockLv
        {
            get
            {
                return Mathf.FloorToInt((float) (this._metaData.unlockLv + (this._metaData.unlockLvAdd * this.level)));
            }
        }

        public List<NeedItemData> UnlockNeedItemList
        {
            get
            {
                List<NeedItemData> list = new List<NeedItemData>();
                this._unlockNeedItemDict = new Dictionary<int, NeedItemData>();
                foreach (AvatarSubSkillMetaData.SkillUpLevelNeedItem item in this._metaData.unlockItemList)
                {
                    if (item.itemMetaID > 0)
                    {
                        NeedItemData data = new NeedItemData(item.itemMetaID, item.itemNum);
                        list.Add(data);
                        this._unlockNeedItemDict.Add(item.itemMetaID, data);
                    }
                }
                return list;
            }
        }

        public int UnlockPoint
        {
            get
            {
                return this._metaData.unlockPoint;
            }
        }

        public int UnlockSCoin
        {
            get
            {
                return this._metaData.unlockScoin;
            }
        }

        public int UnlockStar
        {
            get
            {
                return this._metaData.unlockStar;
            }
        }
    }
}

