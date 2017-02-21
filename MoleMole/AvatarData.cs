namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;

    public static class AvatarData
    {
        private static bool _avatarDataInited = false;
        private static ConfigAvatarRegistry _avatarRegistry;
        private static List<string> _configPathList;
        private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();
        private static Action<string> _loadJsonConfigCallback = null;
        public static int AVATAR_APPEAR_TAG;
        public static int AVATAR_ATK_TAG;
        public const float AVATAR_ATTACK_EXIT_STEER_LERPING_RATIO = 1.3f;
        public const float AVATAR_CAMERA_PULL_Z_FAR_RATIO = 1.3f;
        public const float AVATAR_CAMERA_PULL_Z_FURTHER_RATIO = 1.9f;
        public const float AVATAR_CAMERA_PUSH_Z_NEAR_RATIO = 0.8f;
        private const string AVATAR_CONFIG_PATH = "Data/AvatarConfig";
        public const float AVATAR_DEFAULT_RIGIDBODY_MASS = 1f;
        public static int AVATAR_DIE_TAG;
        public static int AVATAR_EVADE_TAG;
        public static int AVATAR_HIT_TAG;
        public static int AVATAR_IDLESUB_TAG;
        public static int AVATAR_MOVESUB_TAG;
        private const string AVATAR_PREFAB_LOW_SUFFIX = "_Low";
        private const string AVATAR_PREFAB_PATH = "Entities/Avatar/";
        private const string AVATAR_PREFAB_PREFIX = "Avatar_";
        private const string AVATAR_REGISTRY_PATH = "Data/AvatarConfig/AvatarRegistry";
        public static int AVATAR_SKL_MOVE_TAG;
        public static int AVATAR_SKL_NO_TARGET_TAG;
        public static int AVATAR_SKL_TAG;
        public static int AVATAR_SWITCH_TAG;
        public static HashSet<int>[] AVATAR_TAG_GROUPS;
        public const float AVATAR_TARGET_FADE_OFF_TIME = 0.5f;
        public static int AVATAR_THROW_TAG;
        public const float HIT_HEAVY_THRESHOLD = 0.8f;
        public static bool NO_TARGET_SKILL_CLEAR_AVATAR_TARGET = true;
        public static bool NO_TARGET_SKILL_RESET_AVATAR_TARGET_FADE_OFF_TIME = true;
        public static bool RUN_CLEAR_ATTACK_TARGET = true;
        public const int SKILL_NUM = 3;
        public const int SKILL_WEPON_INDEX = 3;
        public const float TIME_SLOW_ATTACK_TARGET_RADIUS = 2f;
        public const string WEAPON_SKILL_NAME = "SKL_WEAPON";

        private static void AddUnlockedAbility(AvatarActor avatarActor, ConfigAvatarAbilityUnlock unlockConfig, Dictionary<string, bool> defaultReplaceMap, float skillParam1 = 0, float skillParam2 = 0, float skillParam3 = 0)
        {
            ConfigAbility abilityConfig = AbilityData.GetAbilityConfig(unlockConfig.AbilityName, unlockConfig.AbilityOverride);
            Dictionary<string, object> dictionary = null;
            bool flag = false;
            for (int i = 0; i < avatarActor.appliedAbilities.Count; i++)
            {
                if (avatarActor.appliedAbilities[i].Item1.AbilityName == unlockConfig.AbilityName)
                {
                    dictionary = avatarActor.appliedAbilities[i].Item2;
                    if (avatarActor.appliedAbilities[i].Item1 != abilityConfig)
                    {
                        avatarActor.appliedAbilities[i] = Tuple.Create<ConfigAbility, Dictionary<string, object>>(abilityConfig, dictionary);
                    }
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                dictionary = avatarActor.CreateAppliedAbility(abilityConfig);
                if (unlockConfig.AbilityReplaceID != null)
                {
                    defaultReplaceMap[unlockConfig.AbilityReplaceID] = true;
                    avatarActor.abilityIDMap[unlockConfig.AbilityReplaceID] = unlockConfig.AbilityName;
                }
            }
            if (unlockConfig.ParamSpecial1 != null)
            {
                AbilityData.SetupParamSpecial(abilityConfig, dictionary, unlockConfig.ParamSpecial1, unlockConfig.ParamMethod1, skillParam1);
            }
            if (unlockConfig.ParamSpecial2 != null)
            {
                AbilityData.SetupParamSpecial(abilityConfig, dictionary, unlockConfig.ParamSpecial2, unlockConfig.ParamMethod2, skillParam2);
            }
            if (unlockConfig.ParamSpecial3 != null)
            {
                AbilityData.SetupParamSpecial(abilityConfig, dictionary, unlockConfig.ParamSpecial3, unlockConfig.ParamMethod3, skillParam3);
            }
        }

        public static int CalculateContentHash()
        {
            int lastHash = 0;
            foreach (AvatarRegistryEntry entry in _avatarRegistry.AvatarRegistry.Values)
            {
                HashUtils.TryHashObject(entry.Config, ref lastHash);
            }
            return lastHash;
        }

        private static bool CheckUnlockBySkillID(ConfigAvatarAbilityUnlock unlockConfig, AvatarDataItem avatarDataItem, bool useLeaderSkill)
        {
            for (int i = 0; i < avatarDataItem.skillDataList.Count; i++)
            {
                AvatarSkillDataItem item = avatarDataItem.skillDataList[i];
                if ((item.skillID == unlockConfig.UnlockBySkillID) && item.UnLocked)
                {
                    if (item.IsLeaderSkill)
                    {
                        return useLeaderSkill;
                    }
                    return true;
                }
            }
            return false;
        }

        private static bool CheckUnlockBySubSkillIDAndAddParam(ConfigAvatarAbilityUnlock unlockConfig, AvatarDataItem avatarDataItem, out AvatarSubSkillDataItem outSubSkillItem, bool useLeaderSkill)
        {
            for (int i = 0; i < avatarDataItem.skillDataList.Count; i++)
            {
                AvatarSkillDataItem item = avatarDataItem.skillDataList[i];
                if (!item.IsLeaderSkill || useLeaderSkill)
                {
                    for (int j = 0; j < item.avatarSubSkillList.Count; j++)
                    {
                        AvatarSubSkillDataItem item2 = item.avatarSubSkillList[j];
                        if ((item2.subSkillID == unlockConfig.UnlockBySubSkillID) && item2.UnLocked)
                        {
                            outSubSkillItem = item2;
                            return true;
                        }
                    }
                }
            }
            outSubSkillItem = null;
            return false;
        }

        public static Dictionary<string, AvatarRegistryEntry> GetAllAvatarData()
        {
            return _avatarRegistry.AvatarRegistry;
        }

        public static ConfigAvatar GetAvatarConfig(string type)
        {
            if (_avatarRegistry.AvatarRegistry.ContainsKey(type))
            {
                return _avatarRegistry.AvatarRegistry[type].Config;
            }
            return null;
        }

        public static uint GetAvatarTypeIDByName(string typeName)
        {
            return _avatarRegistry.AvatarRegistry[typeName].ID;
        }

        public static string GetPrefabResPath(string type, bool useLow = false)
        {
            AvatarRegistryEntry entry;
            _avatarRegistry.AvatarRegistry.TryGetValue(type, out entry);
            if (entry == null)
            {
                throw new Exception("Invalid Type or State!: " + type);
            }
            if (useLow && entry.Config.CommonArguments.HasLowPrefab)
            {
                string[] textArray1 = new string[] { "Entities/Avatar/", type, "/Avatar_", type, "_Low" };
                return string.Concat(textArray1);
            }
            return ("Entities/Avatar/" + type + "/Avatar_" + type);
        }

        public static void InitAvatarData()
        {
            if (!_avatarDataInited)
            {
                foreach (AvatarRegistryEntry entry in _avatarRegistry.AvatarRegistry.Values)
                {
                    entry.Config.OnLevelLoaded();
                }
                AVATAR_APPEAR_TAG = Animator.StringToHash("AVATAR_APPEAR");
                AVATAR_IDLESUB_TAG = Animator.StringToHash("AVATAR_IDLESUB");
                AVATAR_MOVESUB_TAG = Animator.StringToHash("AVATAR_MOVESUB");
                AVATAR_HIT_TAG = Animator.StringToHash("AVATAR_HITSUB");
                AVATAR_DIE_TAG = Animator.StringToHash("AVATAR_DIESUB");
                AVATAR_EVADE_TAG = Animator.StringToHash("AVATAR_EVADESUB");
                AVATAR_ATK_TAG = Animator.StringToHash("AVATAR_ATK");
                AVATAR_SKL_TAG = Animator.StringToHash("AVATAR_SKL");
                AVATAR_SKL_MOVE_TAG = Animator.StringToHash("AVATAR_SKL_MOVE");
                AVATAR_SKL_NO_TARGET_TAG = Animator.StringToHash("AVATAR_SKL_NO_TARGET");
                AVATAR_THROW_TAG = Animator.StringToHash("AVATAR_THROW");
                AVATAR_SWITCH_TAG = Animator.StringToHash("AVATAR_SWITCHSUB");
                AVATAR_TAG_GROUPS = new HashSet<int>[] { new HashSet<int> { AVATAR_HIT_TAG, AVATAR_THROW_TAG }, new HashSet<int> { AVATAR_APPEAR_TAG, AVATAR_DIE_TAG, AVATAR_SKL_TAG, AVATAR_SKL_NO_TARGET_TAG, AVATAR_HIT_TAG, AVATAR_THROW_TAG }, new HashSet<int> { AVATAR_MOVESUB_TAG, AVATAR_IDLESUB_TAG, AVATAR_SKL_MOVE_TAG }, new HashSet<int> { AVATAR_SWITCH_TAG, AVATAR_SKL_NO_TARGET_TAG, AVATAR_SKL_MOVE_TAG }, new HashSet<int> { AVATAR_SKL_TAG }, new HashSet<int> { AVATAR_ATK_TAG }, new HashSet<int> { AVATAR_IDLESUB_TAG, AVATAR_APPEAR_TAG }, new HashSet<int> { AVATAR_MOVESUB_TAG, AVATAR_IDLESUB_TAG, AVATAR_ATK_TAG, AVATAR_SKL_TAG, AVATAR_SKL_MOVE_TAG, AVATAR_SKL_NO_TARGET_TAG, AVATAR_HIT_TAG, AVATAR_THROW_TAG, AVATAR_SWITCH_TAG }, new HashSet<int> { AVATAR_MOVESUB_TAG }, new HashSet<int> { AVATAR_ATK_TAG, AVATAR_SKL_TAG, AVATAR_SKL_MOVE_TAG, AVATAR_SKL_NO_TARGET_TAG }, new HashSet<int> { AVATAR_THROW_TAG } };
            }
        }

        private static void OnLoadOneJsonConfigFinish(ConfigAvatar configAvatar, string avatarType)
        {
            string item = string.Format("{0}/{1}{2}_Config", "Data/AvatarConfig", "Avatar_", avatarType);
            _configPathList.Remove(item);
            _avatarRegistry.AvatarRegistry[avatarType].Config = configAvatar;
            if (_configPathList.Count == 0)
            {
                _loadDataBackGroundWorker.StopBackGroundWork(false);
                ReloadAvatarConfig();
                if (_loadJsonConfigCallback != null)
                {
                    _loadJsonConfigCallback("AvatarData");
                    _loadJsonConfigCallback = null;
                }
            }
        }

        private static void PrintAvatarSubSkills()
        {
            string message = string.Empty;
            foreach (string str2 in _avatarRegistry.AvatarRegistry.Keys)
            {
                ConfigAvatarAbilityUnlock[] abilitiesUnlock = _avatarRegistry.AvatarRegistry[str2].Config.AbilitiesUnlock;
                for (int i = 0; i < abilitiesUnlock.Length; i++)
                {
                    string str3 = message;
                    object[] objArray1 = new object[] { str3, abilitiesUnlock[i].UnlockBySubSkillID, "\t", abilitiesUnlock[i].AbilityName, "\n" };
                    message = string.Concat(objArray1);
                }
            }
            UnityEngine.Debug.Log(message);
        }

        private static void ReloadAvatarConfig()
        {
            foreach (AvatarMetaData data in AvatarMetaDataReader.GetItemList())
            {
                ConfigAvatar config = _avatarRegistry.AvatarRegistry[data.avatarRegistryKey].Config;
                config.CommonArguments.Nature = (EntityNature) data.attribute;
                if (config.Skills.ContainsKey("SKL01"))
                {
                    ConfigAvatarSkill skill = config.Skills["SKL01"];
                    skill.SkillCD = data.SKL01CD;
                    skill.SPCost = data.SKL01SP;
                    skill.SPNeed = data.SKL01SPNeed;
                    skill.ChargesCount = data.SKL01Charges;
                }
                if (config.Skills.ContainsKey("SKL02"))
                {
                    ConfigAvatarSkill skill2 = config.Skills["SKL02"];
                    skill2.SkillCD = data.SKL02CD;
                    skill2.SPCost = data.SKL02SP;
                    skill2.SPNeed = data.SKL02SPNeed;
                    skill2.ChargesCount = data.SKL02Charges;
                }
                if (config.Skills.ContainsKey("SKL03"))
                {
                    ConfigAvatarSkill skill3 = config.Skills["SKL03"];
                    skill3.SkillCD = data.SKL03CD;
                    skill3.SPCost = data.SKL03SP;
                    skill3.SPNeed = data.SKL03SPNeed;
                    skill3.ChargesCount = data.SKL03Charges;
                }
            }
        }

        public static void ReloadFromFile()
        {
            _avatarRegistry = ConfigUtil.LoadJSONConfig<ConfigAvatarRegistry>("Data/AvatarConfig/AvatarRegistry");
            foreach (string str in _avatarRegistry.AvatarRegistry.Keys)
            {
                _avatarRegistry.AvatarRegistry[str].Config = ConfigUtil.LoadJSONConfig<ConfigAvatar>(string.Format("{0}/{1}{2}_Config", "Data/AvatarConfig", "Avatar_", str));
            }
            ReloadAvatarConfig();
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator7 { finishCallback = finishCallback, progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>finishCallback = finishCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        public static void UnlockAvatarAbilities(AvatarDataItem avatarDataItem, AvatarActor avatarActor, bool useLeaderSkill)
        {
            Dictionary<string, bool> defaultReplaceMap = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, ConfigEntityAbilityEntry> pair in avatarActor.config.Abilities)
            {
                avatarActor.abilityIDMap.Add(pair.Key, pair.Value.AbilityName);
                defaultReplaceMap.Add(pair.Key, false);
            }
            for (int i = 0; i < avatarActor.config.AbilitiesUnlock.Length; i++)
            {
                bool flag = false;
                ConfigAvatarAbilityUnlock unlockConfig = avatarActor.config.AbilitiesUnlock[i];
                AvatarSubSkillDataItem outSubSkillItem = null;
                if (unlockConfig.IsUnlockBySkill)
                {
                    flag = CheckUnlockBySkillID(unlockConfig, avatarDataItem, useLeaderSkill);
                }
                else
                {
                    flag = CheckUnlockBySubSkillIDAndAddParam(unlockConfig, avatarDataItem, out outSubSkillItem, useLeaderSkill);
                }
                if (flag)
                {
                    if (outSubSkillItem != null)
                    {
                        AddUnlockedAbility(avatarActor, unlockConfig, defaultReplaceMap, outSubSkillItem.SkillParam_1, outSubSkillItem.SkillParam_2, outSubSkillItem.SkillParam_3);
                    }
                    else
                    {
                        AddUnlockedAbility(avatarActor, unlockConfig, defaultReplaceMap, 0f, 0f, 0f);
                    }
                }
            }
            foreach (KeyValuePair<string, ConfigEntityAbilityEntry> pair2 in avatarActor.config.Abilities)
            {
                if (!defaultReplaceMap[pair2.Key] && (pair2.Value.AbilityName != "Noop"))
                {
                    avatarActor.CreateAppliedAbility(AbilityData.GetAbilityConfig(pair2.Value.AbilityName, pair2.Value.AbilityOverride));
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator7 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>finishCallback;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal Dictionary<string, AvatarRegistryEntry>.KeyCollection.Enumerator <$s_874>__1;
            internal Dictionary<string, AvatarRegistryEntry>.KeyCollection.Enumerator <$s_875>__5;
            internal AsyncAssetRequst <asyncRequest>__0;
            internal string <avatarType>__2;
            internal string <avatarType>__6;
            internal string <path>__3;
            internal string <path>__7;
            internal float <step>__4;
            internal Action<string> finishCallback;
            internal Action<float> moveOneStepCallback;
            internal float progressSpan;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            this.<$s_875>__5.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        AvatarData._loadJsonConfigCallback = this.finishCallback;
                        AvatarData._configPathList = new List<string>();
                        this.<asyncRequest>__0 = ConfigUtil.LoadJsonConfigAsync("Data/AvatarConfig/AvatarRegistry", BundleType.DATA_FILE);
                        this.$current = this.<asyncRequest>__0.operation;
                        this.$PC = 1;
                        goto Label_02AA;

                    case 1:
                        AvatarData._avatarRegistry = ConfigUtil.LoadJSONStrConfig<ConfigAvatarRegistry>(this.<asyncRequest>__0.asset.ToString());
                        if (AvatarData._avatarRegistry.AvatarRegistry.Count != 0)
                        {
                            this.<$s_874>__1 = AvatarData._avatarRegistry.AvatarRegistry.Keys.GetEnumerator();
                            try
                            {
                                while (this.<$s_874>__1.MoveNext())
                                {
                                    this.<avatarType>__2 = this.<$s_874>__1.Current;
                                    this.<path>__3 = string.Format("{0}/{1}{2}_Config", "Data/AvatarConfig", "Avatar_", this.<avatarType>__2);
                                    AvatarData._configPathList.Add(this.<path>__3);
                                }
                            }
                            finally
                            {
                                this.<$s_874>__1.Dispose();
                            }
                            this.<step>__4 = this.progressSpan / ((float) AvatarData._avatarRegistry.AvatarRegistry.Count);
                            AvatarData._loadDataBackGroundWorker.StartBackGroundWork("AvatarData");
                            this.<$s_875>__5 = AvatarData._avatarRegistry.AvatarRegistry.Keys.GetEnumerator();
                            num = 0xfffffffd;
                            break;
                        }
                        if (AvatarData._loadJsonConfigCallback != null)
                        {
                            AvatarData._loadJsonConfigCallback("AvatarData");
                            AvatarData._loadJsonConfigCallback = null;
                        }
                        goto Label_02A8;

                    case 2:
                        break;

                    default:
                        goto Label_02A8;
                }
                try
                {
                    switch (num)
                    {
                        case 2:
                            goto Label_022F;
                    }
                    while (this.<$s_875>__5.MoveNext())
                    {
                        this.<avatarType>__6 = this.<$s_875>__5.Current;
                        this.<path>__7 = string.Format("{0}/{1}{2}_Config", "Data/AvatarConfig", "Avatar_", this.<avatarType>__6);
                        this.<asyncRequest>__0 = ConfigUtil.LoadJsonConfigAsync(this.<path>__7, BundleType.DATA_FILE);
                        SuperDebug.VeryImportantAssert(this.<asyncRequest>__0 != null, "assetRequest is null avatarPath :" + this.<path>__7);
                        if (this.<asyncRequest>__0 == null)
                        {
                            continue;
                        }
                        this.$current = this.<asyncRequest>__0.operation;
                        this.$PC = 2;
                        flag = true;
                        goto Label_02AA;
                    Label_022F:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__4);
                        }
                        ConfigUtil.LoadJSONStrConfigMultiThread<ConfigAvatar>(this.<asyncRequest>__0.asset.ToString(), AvatarData._loadDataBackGroundWorker, new Action<ConfigAvatar, string>(AvatarData.OnLoadOneJsonConfigFinish), this.<avatarType>__6);
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.<$s_875>__5.Dispose();
                }
                this.$PC = -1;
            Label_02A8:
                return false;
            Label_02AA:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public enum AvatarTagGroup
        {
            ShowHit,
            MuteJoyStickInput,
            UseJoyStickDirectionForMove,
            AttackWithNoTarget,
            AttackSteerOnEnter,
            AttackTargetLeadDirection,
            Stable,
            AllowTriggerInput,
            Movement,
            AttackOrSkill,
            Throw
        }
    }
}

