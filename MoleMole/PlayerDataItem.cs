namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class PlayerDataItem
    {
        public CacheDataUtil _cacheDataUtil;
        private PlayerLevelMetaData _metaData;
        public int avatarCombatBaseLevelRate;
        public int avatarCombatBaseStarRate;
        public int avatarCombatBaseUnlockStarRate;
        public int avatarCombatBaseWeight;
        public int avatarCombatIslandWeight;
        public int avatarCombatSkillWeight;
        public int avatarCombatStigmataLevelRate;
        public int avatarCombatStigmataRarityRate;
        public int avatarCombatStigmataSubRarityRate;
        public int avatarCombatStigmataSuitNumRate;
        public int avatarCombatStigmataWeight;
        public int avatarCombatWeaponLevelRate;
        public int avatarCombatWeaponRarityRate;
        public int avatarCombatWeaponSubRarityRate;
        public int avatarCombatWeaponWeight;
        public Dictionary<int, int> costAddByAvatarStar;
        public int disjoin_equipment_back_exp_percent;
        public int endlessMaxProgress = 0x63;
        public int endlessMinPlayerLevel = 10;
        public int endlessUseItemCDTime;
        public int equipmentSizeLimit;
        public int friendsPoint;
        public Dictionary<int, int> gachaTicketPriceDict;
        public int hcoin;
        public bool initByGetMainDataRsp;
        public int maxFriend = 2;
        public int maxLevelToAcceptInvite;
        public int minLevelToGenerateInviteCode;
        public DateTime nextSkillPtRecoverDatetime;
        public DateTime nextStaminaRecoverDatetime;
        public string nickname;
        public int offlineFriendsPoint;
        public int powerUpScoinCostRate;
        public int reviveHcoinCost;
        public int sameTypePowerUpRataInt;
        public int scoin;
        public CacheData<PlayerScoinExchangeInfo> scoinExchangeCache;
        public string selfDesc;
        public GetSignInRewardStatusRsp signInStatus;
        public int skillPoint;
        public CacheData<PlayerSkillPointExchangeInfo> skillPointExchangeCache;
        public int skillPointLimit;
        public int skillPointRecoverConfigTime;
        public int skillPointRecoverLeftTime;
        public int stamina;
        public CacheData<PlayerStaminaExchangeInfo> staminaExchangeCache;
        public int staminaRecoverConfigTime;
        public int staminaRecoverLeftTime;
        public Dictionary<StageType, List<int>> teamDict;
        public int teamExp;
        public int teamLevel;
        public string token;
        public PlayerUITempSaveData uiTempSaveData;
        public int userId;

        public PlayerDataItem(int teamLevel = 1)
        {
            this.teamLevel = teamLevel;
            this.OnLevelChange(this.teamLevel, this.teamLevel);
            this.costAddByAvatarStar = new Dictionary<int, int>();
            this.teamDict = new Dictionary<StageType, List<int>>();
            this.gachaTicketPriceDict = new Dictionary<int, int>();
            this.scoinExchangeCache = new CacheData<PlayerScoinExchangeInfo>();
            this.staminaExchangeCache = new CacheData<PlayerStaminaExchangeInfo>();
            this.skillPointExchangeCache = new CacheData<PlayerSkillPointExchangeInfo>();
            this._cacheDataUtil = new CacheDataUtil();
            this._cacheDataUtil.CreateCacheUtil(ECacheData.Stamina, this.staminaExchangeCache, new Action(Singleton<NetworkManager>.Instance.RequestGetStaminaExchangeInfo), 0x11);
            this.uiTempSaveData = new PlayerUITempSaveData();
            this.signInStatus = null;
            this.initByGetMainDataRsp = false;
        }

        public List<int> GetMemberList(StageType levelType)
        {
            if (!this.teamDict.ContainsKey(levelType))
            {
                this.teamDict.Add(levelType, new List<int>());
            }
            return this.teamDict[levelType];
        }

        public DateTime GetSkillPointFullTime()
        {
            int num = this.skillPointRecoverLeftTime + (this.skillPointRecoverConfigTime * ((this.skillPointLimit - this.skillPoint) - 1));
            return TimeUtil.Now.AddSeconds((double) num);
        }

        public DateTime GetStaminaFullTime()
        {
            int num = this.staminaRecoverLeftTime + (this.staminaRecoverConfigTime * ((this.MaxStamina - this.stamina) - 1));
            return TimeUtil.Now.AddSeconds((double) num);
        }

        public bool HasTeamMember(StageType levelType, int num)
        {
            if (!this.teamDict.ContainsKey(levelType))
            {
                return false;
            }
            return (num <= this.teamDict[levelType].Count);
        }

        public bool IsSkillPointFull()
        {
            return (this.skillPoint >= this.skillPointLimit);
        }

        public bool IsStaminaFull()
        {
            return (this.stamina >= this.MaxStamina);
        }

        public void OnCoinChange(int preValue, int newValue)
        {
            WwiseAudioManager instance = Singleton<WwiseAudioManager>.Instance;
            if ((preValue > newValue) && (instance != null))
            {
                instance.Post("UI_Gen_Buy_Item", null, null, null);
            }
        }

        public void OnLevelChange(int preValue, int newValue)
        {
            if ((this._metaData == null) || (newValue != preValue))
            {
                this._metaData = PlayerLevelMetaDataReader.GetPlayerLevelMetaDataByKey(newValue);
            }
            if (newValue != preValue)
            {
                Singleton<IslandModule>.Instance.OnPlayerLevelChanged(newValue, preValue);
                Singleton<NetworkManager>.Instance.RequestGetInviteeFriend();
            }
        }

        public void OnSkillPointRecoverTimeChange(int preValue, int newValue)
        {
            this.nextSkillPtRecoverDatetime = TimeUtil.Now.AddSeconds((double) this.skillPointRecoverLeftTime);
        }

        public void OnStaminaRecoverTimeChange(int preValue, int newValue)
        {
            this.nextStaminaRecoverDatetime = TimeUtil.Now.AddSeconds((double) this.staminaRecoverLeftTime);
        }

        public void RemoveTeamMember(StageType levelType, int num)
        {
            this.teamDict[levelType].RemoveAt(num - 1);
        }

        public void SetTeamMember(StageType levelType, List<int> newMemberList)
        {
            this.teamDict[levelType] = newMemberList;
        }

        public void SetTeamMember(StageType levelType, int num, int avatarId)
        {
            if (num > this.teamDict[levelType].Count)
            {
                this.teamDict[levelType].Add(avatarId);
            }
            else
            {
                this.teamDict[levelType][num - 1] = avatarId;
            }
        }

        public void SwitchTeamMember(StageType levelType, int numLeft, int numRight)
        {
            if (this.teamDict.ContainsKey(levelType))
            {
                int num = this.teamDict[levelType][numLeft - 1];
                this.teamDict[levelType][numLeft - 1] = this.teamDict[levelType][numRight - 1];
                this.teamDict[levelType][numRight - 1] = num;
            }
        }

        public override string ToString()
        {
            return ("PlayerDataItem: " + this.userId);
        }

        public int AvatarLevelLimit
        {
            get
            {
                return this._metaData.avatarLevelLimit;
            }
        }

        public int maxFriendFinal
        {
            get
            {
                return (this.maxFriend + Singleton<IslandModule>.Instance.GetMaxFriendAdd());
            }
        }

        public int MaxStamina
        {
            get
            {
                return this._metaData.stamina;
            }
        }

        public string NickNameText
        {
            get
            {
                return (!string.IsNullOrEmpty(this.nickname) ? this.nickname : LocalizationGeneralLogic.GetText("Menu_DefaultNickname", new object[] { this.userId }));
            }
        }

        public int NumFriends
        {
            get
            {
                return this._metaData.numFriends;
            }
        }

        public string SelfDescText
        {
            get
            {
                return (!string.IsNullOrEmpty(this.selfDesc) ? this.selfDesc : LocalizationGeneralLogic.GetText("Menu_DefaultSelfDesc", new object[0]));
            }
        }

        public int TeamMaxExp
        {
            get
            {
                return this._metaData.exp;
            }
        }

        public int TeamNeedExp
        {
            get
            {
                return (this.TeamMaxExp - this.teamExp);
            }
        }
    }
}

