namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AvatarDataItem
    {
        private ClassMetaData _classMetaData;
        private int _leaderSkillId;
        private AvatarLevelMetaData _levelMetaData;
        private AvatarMetaData _metaData;
        private Dictionary<int, AvatarSkillDataItem> _skillDataMap;
        private AvatarStarMetaData _starMetaData;
        private int _unlockNeedFragment;
        public int avatarID;
        public static readonly EquipmentSlot[] EQUIP_SLOTS = new EquipmentSlot[] { 1, 2, 3, 4 };
        public Dictionary<EquipmentSlot, StorageDataItemBase> equipsMap;
        public int exp;
        public int fragment;
        public bool Initialized;
        public int level;
        public List<AvatarSkillDataItem> skillDataList;
        public int star;
        public bool UnLocked;

        public AvatarDataItem(int avatarID, int level = 1, int star = 0)
        {
            AvatarMetaData avatarMetaDataByKey = AvatarMetaDataReader.GetAvatarMetaDataByKey(avatarID);
            ClassMetaData classMetaDataByKey = ClassMetaDataReader.GetClassMetaDataByKey(avatarMetaDataByKey.classID);
            this.Init(avatarID, avatarMetaDataByKey, classMetaDataByKey, null, null, level, star);
        }

        public AvatarDataItem(int avatarID, AvatarMetaData metaData, ClassMetaData classMetaData, AvatarStarMetaData starMetaData, AvatarLevelMetaData levelMetaData, int level, int star)
        {
            this.Init(avatarID, metaData, classMetaData, starMetaData, levelMetaData, level, star);
        }

        private int CalculateUnlockNeedFragment()
        {
            int num = 0;
            for (int i = 0; i < this._metaData.unlockStar; i++)
            {
                num += AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(this.avatarID, i).upgradeFragment;
            }
            return num;
        }

        public AvatarDataItem Clone()
        {
            AvatarDataItem item = new AvatarDataItem(this.avatarID, this.level, this.star) {
                UnLocked = this.UnLocked,
                Initialized = this.Initialized,
                exp = this.exp,
                fragment = this.fragment
            };
            foreach (EquipmentSlot slot in EQUIP_SLOTS)
            {
                StorageDataItemBase base2 = (this.equipsMap[slot] != null) ? this.equipsMap[slot].Clone() : null;
                item.equipsMap[slot] = base2;
            }
            foreach (AvatarSkillDataItem item2 in this.skillDataList)
            {
                AvatarSkillDataItem avatarSkillBySkillID = item.GetAvatarSkillBySkillID(item2.skillID);
                avatarSkillBySkillID.UnLocked = item2.UnLocked;
                foreach (AvatarSubSkillDataItem item4 in item2.avatarSubSkillList)
                {
                    avatarSkillBySkillID.GetAvatarSubSkillBySubSkillId(item4.subSkillID).level = item4.level;
                }
            }
            return item;
        }

        public float GetAvatarCombatUsingNewEquip(StorageDataItemBase equipData)
        {
            EquipmentSlot slot;
            if (UIUtil.GetEquipmentSlot(equipData, out slot))
            {
                AvatarDataItem item = this.Clone();
                foreach (KeyValuePair<EquipmentSlot, StorageDataItemBase> pair in this.equipsMap)
                {
                    if (pair.Key == slot)
                    {
                        item.equipsMap[pair.Key] = equipData;
                    }
                }
                return item.CombatNum;
            }
            return this.CombatNum;
        }

        public AvatarFragmentDataItem GetAvatarFragmentDataItem()
        {
            return new AvatarFragmentDataItem(AvatarFragmentMetaDataReader.GetAvatarFragmentMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(this.avatarID).avatarFragmentID)) { number = this.fragment };
        }

        public AvatarSkillDataItem GetAvatarSkillBySkillID(int skillID)
        {
            return this._skillDataMap[skillID];
        }

        private float GetBaseAttack()
        {
            float num = this._starMetaData.atkBase + ((this.level - 1) * this._starMetaData.atkAdd);
            CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(this.ClassId);
            if (avatarEnhanceCabinByClass != null)
            {
                num *= 1f + avatarEnhanceCabinByClass.GetAvatarAttrEnhance(3);
            }
            return num;
        }

        private float GetBaseCritical()
        {
            float num = this._starMetaData.crtBase + ((this.level - 1) * this._starMetaData.crtAdd);
            CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(this.ClassId);
            if (avatarEnhanceCabinByClass != null)
            {
                num *= 1f + avatarEnhanceCabinByClass.GetAvatarAttrEnhance(5);
            }
            return num;
        }

        private float GetBaseDefense()
        {
            float num = this._starMetaData.dfsBase + ((this.level - 1) * this._starMetaData.dfsAdd);
            CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(this.ClassId);
            if (avatarEnhanceCabinByClass != null)
            {
                num *= 1f + avatarEnhanceCabinByClass.GetAvatarAttrEnhance(4);
            }
            return num;
        }

        private float GetBaseHp()
        {
            float num = this._starMetaData.hpBase + ((this.level - 1) * this._starMetaData.hpAdd);
            CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(this.ClassId);
            if (avatarEnhanceCabinByClass != null)
            {
                num *= 1f + avatarEnhanceCabinByClass.GetAvatarAttrEnhance(1);
            }
            return num;
        }

        private float GetBaseSP()
        {
            float num = this._starMetaData.spBase + ((this.level - 1) * this._starMetaData.spAdd);
            CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(this.ClassId);
            if (avatarEnhanceCabinByClass != null)
            {
                num *= 1f + avatarEnhanceCabinByClass.GetAvatarAttrEnhance(2);
            }
            return num;
        }

        public Sprite GetBGSprite()
        {
            switch (((EntityNature) this.Attribute))
            {
                case EntityNature.Mechanic:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrJiXie");

                case EntityNature.Biology:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrShengWu");

                case EntityNature.Psycho:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AttrYiNeng");
            }
            return null;
        }

        public int GetCost(int theStar)
        {
            int num = 0;
            Dictionary<int, int> costAddByAvatarStar = Singleton<PlayerModule>.Instance.playerData.costAddByAvatarStar;
            if (costAddByAvatarStar.ContainsKey(theStar))
            {
                num = costAddByAvatarStar[theStar];
            }
            return (this._levelMetaData.cost + num);
        }

        public int GetCurrentCost()
        {
            int num = 0;
            foreach (StorageDataItemBase base2 in this.equipsMap.Values)
            {
                if (base2 != null)
                {
                    num += base2.GetCost();
                }
            }
            return num;
        }

        public float GetEquipAttackAdd()
        {
            float num = 0f;
            foreach (StorageDataItemBase base2 in this.equipsMap.Values)
            {
                if (base2 != null)
                {
                    num += base2.GetAttackAdd();
                }
            }
            return num;
        }

        public float GetEquipAttackAddWithAffix()
        {
            float num = 0f;
            foreach (StorageDataItemBase base2 in this.equipsMap.Values)
            {
                if (base2 is WeaponDataItem)
                {
                    num += base2.GetAttackAdd();
                }
                else if (base2 is StigmataDataItem)
                {
                    num += (base2 as StigmataDataItem).GetAttackAddWithAffix(this);
                }
            }
            return num;
        }

        public float GetEquipCriticalAdd()
        {
            float num = 0f;
            foreach (StorageDataItemBase base2 in this.equipsMap.Values)
            {
                if (base2 != null)
                {
                    num += base2.GetCriticalAdd();
                }
            }
            return num;
        }

        public float GetEquipCriticalAddWithAffix()
        {
            float num = 0f;
            foreach (StorageDataItemBase base2 in this.equipsMap.Values)
            {
                if (base2 is WeaponDataItem)
                {
                    num += base2.GetCriticalAdd();
                }
                else if (base2 is StigmataDataItem)
                {
                    num += (base2 as StigmataDataItem).GetCriticalAddWithAffix(this);
                }
            }
            return num;
        }

        public float GetEquipDefenseAdd()
        {
            float num = 0f;
            foreach (StorageDataItemBase base2 in this.equipsMap.Values)
            {
                if (base2 != null)
                {
                    num += base2.GetDefenceAdd();
                }
            }
            return num;
        }

        public float GetEquipDefenseAddWithAffix()
        {
            float num = 0f;
            foreach (StorageDataItemBase base2 in this.equipsMap.Values)
            {
                if (base2 != null)
                {
                    if (base2 is WeaponDataItem)
                    {
                        num += base2.GetDefenceAdd();
                    }
                    else if (base2 is StigmataDataItem)
                    {
                        num += (base2 as StigmataDataItem).GetDefenceAddWithAffix(this);
                    }
                }
            }
            return num;
        }

        public float GetEquipHPAdd()
        {
            float num = 0f;
            foreach (StorageDataItemBase base2 in this.equipsMap.Values)
            {
                if (base2 != null)
                {
                    num += base2.GetHPAdd();
                }
            }
            return num;
        }

        public float GetEquipHPAddWithAffix()
        {
            float num = 0f;
            foreach (StorageDataItemBase base2 in this.equipsMap.Values)
            {
                if (base2 != null)
                {
                    if (base2 is WeaponDataItem)
                    {
                        num += base2.GetHPAdd();
                    }
                    else if (base2 is StigmataDataItem)
                    {
                        num += (base2 as StigmataDataItem).GetHPAddWithAffix(this);
                    }
                }
            }
            return num;
        }

        public float GetEquipSPAdd()
        {
            float num = 0f;
            foreach (StorageDataItemBase base2 in this.equipsMap.Values)
            {
                if (base2 != null)
                {
                    num += base2.GetSPAdd();
                }
            }
            return num;
        }

        public float GetEquipSPAddWithAffix()
        {
            float num = 0f;
            foreach (StorageDataItemBase base2 in this.equipsMap.Values)
            {
                if (base2 != null)
                {
                    if (base2 is WeaponDataItem)
                    {
                        num += base2.GetSPAdd();
                    }
                    else if (base2 is StigmataDataItem)
                    {
                        num += (base2 as StigmataDataItem).GetSPAddWithAffix(this);
                    }
                }
            }
            return num;
        }

        public AvatarSkillDataItem GetLeaderSkill()
        {
            return this._skillDataMap[this._leaderSkillId];
        }

        public EquipSetDataItem GetOwnEquipSetData()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            foreach (StigmataDataItem item in this.GetStigmataList())
            {
                if ((item != null) && (item.GetEquipmentSetID() != 0))
                {
                    if (dictionary.ContainsKey(item.GetEquipmentSetID()))
                    {
                        Dictionary<int, int> dictionary3;
                        int num2;
                        num2 = dictionary3[num2];
                        (dictionary3 = dictionary)[num2 = item.GetEquipmentSetID()] = num2 + 1;
                    }
                    else
                    {
                        dictionary[item.GetEquipmentSetID()] = 1;
                    }
                }
            }
            foreach (KeyValuePair<int, int> pair in dictionary)
            {
                EquipSetDataItem item2 = new EquipSetDataItem(pair.Key, pair.Value);
                if (item2.GetOwnSetSkills().Count > 0)
                {
                    return item2;
                }
            }
            return null;
        }

        public int GetSkillPointAddNum()
        {
            int num = 0;
            foreach (AvatarSkillDataItem item in this.skillDataList)
            {
                num += item.GetLevelSum();
            }
            return num;
        }

        public StigmataDataItem GetStigmata(EquipmentSlot slot)
        {
            StorageDataItemBase base2 = this.equipsMap[slot];
            return ((base2 != null) ? (base2 as StigmataDataItem) : null);
        }

        public Dictionary<EquipmentSlot, StigmataDataItem> GetStigmataDict()
        {
            Dictionary<EquipmentSlot, StigmataDataItem> dictionary = new Dictionary<EquipmentSlot, StigmataDataItem>();
            dictionary[2] = this.equipsMap[2] as StigmataDataItem;
            dictionary[3] = this.equipsMap[3] as StigmataDataItem;
            dictionary[4] = this.equipsMap[4] as StigmataDataItem;
            return dictionary;
        }

        public List<StigmataDataItem> GetStigmataList()
        {
            return new List<StigmataDataItem> { (this.equipsMap[2] as StigmataDataItem), (this.equipsMap[3] as StigmataDataItem), (this.equipsMap[4] as StigmataDataItem) };
        }

        public AvatarSkillDataItem GetUltraSkill()
        {
            return this._skillDataMap[this._metaData.ultraSkillID];
        }

        public WeaponDataItem GetWeapon()
        {
            return (this.equipsMap[1] as WeaponDataItem);
        }

        private void Init(int avatarID, AvatarMetaData metaData, ClassMetaData classMetaData, AvatarStarMetaData starMetaData, AvatarLevelMetaData levelMetaData, int level, int star)
        {
            this.avatarID = avatarID;
            this.equipsMap = new Dictionary<EquipmentSlot, StorageDataItemBase>();
            foreach (EquipmentSlot slot in EQUIP_SLOTS)
            {
                this.equipsMap.Add(slot, null);
            }
            this._metaData = metaData;
            this._classMetaData = classMetaData;
            this._starMetaData = starMetaData;
            this._levelMetaData = levelMetaData;
            this.Initialized = false;
            this.UnLocked = false;
            this.SetupDefaultSkillList();
            this.star = (star != 0) ? star : this._metaData.unlockStar;
            this.OnStarUpdate(this.star, this.star);
            this.level = level;
            this.OnLevelUpdate(this.level, this.level);
            this._unlockNeedFragment = this.CalculateUnlockNeedFragment();
        }

        public bool IsEasterner()
        {
            foreach (int num in MiscData.Config.EasternerClassIDList)
            {
                if (num == this.ClassId)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnLevelUpdate(int preValue, int newValue)
        {
            if ((this._levelMetaData == null) || (preValue != newValue))
            {
                this._levelMetaData = AvatarLevelMetaDataReader.GetAvatarLevelMetaDataByKey(newValue);
                this.UpdateSkillInfo();
            }
        }

        public void OnStarUpdate(int preValue, int newValue)
        {
            if ((this._starMetaData == null) || (preValue != newValue))
            {
                this._starMetaData = AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(this.avatarID, newValue);
                this.UpdateSkillInfo();
            }
        }

        public EquipmentSlot SearchEquipSlot(StorageDataItemBase item)
        {
            foreach (KeyValuePair<EquipmentSlot, StorageDataItemBase> pair in this.equipsMap)
            {
                if ((pair.Value != null) && (pair.Value.uid == item.uid))
                {
                    return pair.Key;
                }
            }
            return 0;
        }

        private void SetupDefaultSkillList()
        {
            this.skillDataList = new List<AvatarSkillDataItem>();
            this._skillDataMap = new Dictionary<int, AvatarSkillDataItem>();
            foreach (int num in this._metaData.skillList)
            {
                AvatarSkillDataItem item = new AvatarSkillDataItem(num, this.avatarID);
                this.skillDataList.Add(item);
                this._skillDataMap.Add(num, item);
                if (item.IsLeaderSkill)
                {
                    this._leaderSkillId = item.skillID;
                }
            }
        }

        public void UpdateSkillInfo()
        {
            foreach (AvatarSkillDataItem item in this.skillDataList)
            {
                item.UnLocked = (this.level >= item.UnLockLv) && (this.star >= item.UnLockStar);
            }
        }

        public int Attribute
        {
            get
            {
                return this._metaData.attribute;
            }
        }

        public string AttributeIconPath
        {
            get
            {
                return MiscData.Config.PrefabPath.AvatarAttrIcon[this.Attribute];
            }
        }

        public string AttributeName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.AvatarAttributeName[this.Attribute], new object[0]);
            }
        }

        public string AvatarRegistryKey
        {
            get
            {
                return this._metaData.avatarRegistryKey;
            }
        }

        public string AvatarTachie
        {
            get
            {
                return this._starMetaData.figurePath;
            }
        }

        public bool CanStarUp
        {
            get
            {
                return ((this.fragment >= this.MaxFragment) && (this.MaxFragment > 0));
            }
        }

        public bool CanUnlock
        {
            get
            {
                return (!this.UnLocked && (this.fragment > this._unlockNeedFragment));
            }
        }

        public string ClassEnFirstName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._classMetaData.enFirstName, new object[0]);
            }
        }

        public string ClassEnLastName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._classMetaData.enLastName, new object[0]);
            }
        }

        public string ClassFirstName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._classMetaData.firstName, new object[0]);
            }
        }

        public int ClassId
        {
            get
            {
                return this._metaData.classID;
            }
        }

        public string ClassLastName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._classMetaData.lastName, new object[0]);
            }
        }

        public float CombatNum
        {
            get
            {
                PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
                float num = 1f + (((this.star - 1) * playerData.avatarCombatBaseStarRate) / 10000f);
                float num2 = 1f + (((this.level - 1) * playerData.avatarCombatBaseLevelRate) / 10000f);
                float num3 = 1f + (((this._metaData.unlockStar - 1) * playerData.avatarCombatBaseUnlockStarRate) / 10000f);
                float num4 = ((playerData.avatarCombatBaseWeight * num) * num2) * num3;
                int num5 = 0;
                int num6 = 0;
                foreach (AvatarSkillDataItem item2 in this.skillDataList)
                {
                    foreach (AvatarSubSkillDataItem item3 in item2.avatarSubSkillList)
                    {
                        num6 += item3.MaxLv;
                        num5 += item3.level;
                    }
                }
                float num7 = 0f;
                if (num6 > 0)
                {
                    num7 = (((float) num5) / ((float) num6)) * playerData.avatarCombatSkillWeight;
                }
                float num8 = 0f;
                CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(this.ClassId);
                if ((avatarEnhanceCabinByClass != null) && (avatarEnhanceCabinByClass.status == CabinStatus.UnLocked))
                {
                    num8 = avatarEnhanceCabinByClass.GetTotalEnhancePoint() * playerData.avatarCombatIslandWeight;
                }
                float num9 = 0f;
                WeaponDataItem item5 = this.equipsMap[1] as WeaponDataItem;
                if (item5 != null)
                {
                    num = (1f + (((item5.rarity - 1) * playerData.avatarCombatWeaponRarityRate) / 10000f)) + ((item5.GetSubRarity() * playerData.avatarCombatWeaponSubRarityRate) / 10000f);
                    num2 = 1f + (((item5.level - 1) * playerData.avatarCombatWeaponLevelRate) / 10000f);
                    num9 = ((playerData.avatarCombatWeaponWeight * num) * num2) * item5.GetPowerUpConf();
                }
                float num10 = ((float) playerData.avatarCombatStigmataRarityRate) / 10000f;
                float num11 = ((float) playerData.avatarCombatStigmataSubRarityRate) / 10000f;
                float num12 = ((float) playerData.avatarCombatStigmataLevelRate) / 10000f;
                float num13 = 0f;
                StigmataDataItem item6 = this.equipsMap[2] as StigmataDataItem;
                if (item6 != null)
                {
                    num = (1f + ((item6.rarity - 1) * num10)) + (item6.GetSubRarity() * num11);
                    num2 = 1f + ((item6.level - 1) * num12);
                    num13 = ((playerData.avatarCombatStigmataWeight * num) * num2) * item6.GetPowerUpConf();
                }
                float num14 = 0f;
                StigmataDataItem item7 = this.equipsMap[3] as StigmataDataItem;
                if (item7 != null)
                {
                    num = (1f + ((item7.rarity - 1) * num10)) + (item7.GetSubRarity() * num11);
                    num2 = 1f + ((item7.level - 1) * num12);
                    num14 = ((playerData.avatarCombatStigmataWeight * num) * num2) * item7.GetPowerUpConf();
                }
                float num15 = 0f;
                StigmataDataItem item8 = this.equipsMap[4] as StigmataDataItem;
                if (item8 != null)
                {
                    num = (1f + ((item8.rarity - 1) * num10)) + (item8.GetSubRarity() * num11);
                    num2 = 1f + ((item8.level - 1) * num12);
                    num15 = ((playerData.avatarCombatStigmataWeight * num) * num2) * item8.GetPowerUpConf();
                }
                EquipSetDataItem ownEquipSetData = this.GetOwnEquipSetData();
                float num16 = 0f;
                if (ownEquipSetData != null)
                {
                    num16 = ((((num13 + num14) + num15) * (this.GetOwnEquipSetData().ownNum - 1)) * playerData.avatarCombatStigmataSuitNumRate) / 10000f;
                }
                return (((((((num4 + num7) + num9) + num13) + num14) + num15) + num16) + num8);
            }
        }

        public string Desc
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.desc, new object[0]);
            }
        }

        public string FigurePath
        {
            get
            {
                return this._starMetaData.figurePath;
            }
        }

        public float FinalAttack
        {
            get
            {
                return (this.GetBaseAttack() + this.GetEquipAttackAdd());
            }
        }

        public float FinalAttackUI
        {
            get
            {
                return (this.GetBaseAttack() + this.GetEquipAttackAddWithAffix());
            }
        }

        public float FinalCritical
        {
            get
            {
                return (this.GetBaseCritical() + this.GetEquipCriticalAdd());
            }
        }

        public float FinalCriticalUI
        {
            get
            {
                return (this.GetBaseCritical() + this.GetEquipCriticalAddWithAffix());
            }
        }

        public float FinalDefense
        {
            get
            {
                return (this.GetBaseDefense() + this.GetEquipDefenseAdd());
            }
        }

        public float FinalDefenseUI
        {
            get
            {
                return (this.GetBaseDefense() + this.GetEquipDefenseAddWithAffix());
            }
        }

        public float FinalHP
        {
            get
            {
                return (this.GetBaseHp() + this.GetEquipHPAdd());
            }
        }

        public float FinalHPUI
        {
            get
            {
                return (this.GetBaseHp() + this.GetEquipHPAddWithAffix());
            }
        }

        public float FinalSP
        {
            get
            {
                return (this.GetBaseSP() + this.GetEquipSPAdd());
            }
        }

        public float FinalSPUI
        {
            get
            {
                return (this.GetBaseSP() + this.GetEquipSPAddWithAffix());
            }
        }

        public string FullName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.fullName, new object[0]);
            }
        }

        public string IconPath
        {
            get
            {
                return this._starMetaData.iconPath;
            }
        }

        public string IconPathInLevel
        {
            get
            {
                return this._starMetaData.iconPathInLevel;
            }
        }

        public int InitialWeapon
        {
            get
            {
                return this._metaData.initialWeapon;
            }
        }

        public int LevelTutorialID
        {
            get
            {
                return this._metaData.levelTutorialID;
            }
        }

        public int MaxCost
        {
            get
            {
                int num = 0;
                Dictionary<int, int> costAddByAvatarStar = Singleton<PlayerModule>.Instance.playerData.costAddByAvatarStar;
                if (costAddByAvatarStar.ContainsKey(this.star))
                {
                    num = costAddByAvatarStar[this.star];
                }
                return (this._levelMetaData.cost + num);
            }
        }

        public int MaxExp
        {
            get
            {
                return this._levelMetaData.exp;
            }
        }

        public int MaxFragment
        {
            get
            {
                return (!this.UnLocked ? this._unlockNeedFragment : this._starMetaData.upgradeFragment);
            }
        }

        public string RankName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(MiscData.Config.TextMapKey.AvatarRankName[this.star], new object[0]);
            }
        }

        public string ShortName
        {
            get
            {
                return LocalizationGeneralLogic.GetText(this._metaData.shortName, new object[0]);
            }
        }

        public List<int> WeaponBaseTypeList
        {
            get
            {
                return this._metaData.weaponBaseTypeList;
            }
        }
    }
}

