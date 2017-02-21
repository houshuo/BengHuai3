namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class AbilityData
    {
        private static Dictionary<string, ConfigOverrideGroup> _abilityGroupMap;
        private static List<string> _configPathList;
        private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();
        private static Action<string> _loadJsonConfigCallback = null;
        public const AbilityState ABILITY_STATE_BUFF = (AbilityState.Undamagable | AbilityState.MaxMoveSpeed | AbilityState.Immune | AbilityState.CritUp | AbilityState.Shielded | AbilityState.PowerUp | AbilityState.AttackSpeedUp | AbilityState.MoveSpeedUp | AbilityState.Endure);
        public static AbilityState[] ABILITY_STATE_CONTROL_DEBUFFS = new AbilityState[] { AbilityState.Stun, AbilityState.Paralyze, AbilityState.Frozen };
        public static AbilityState ABILITY_STATE_CONTROL_DEBUFFS_MASK;
        public const AbilityState ABILITY_STATE_DEBUFF = (AbilityState.Tied | AbilityState.TargetLocked | AbilityState.Fragile | AbilityState.Weak | AbilityState.AttackSpeedDown | AbilityState.MoveSpeedDown | AbilityState.Frozen | AbilityState.Poisoned | AbilityState.Burn | AbilityState.Paralyze | AbilityState.Stun | AbilityState.Bleed);
        public static AbilityState[][] ABILITY_STATE_PRECEDENCE_MAP;
        public const string ACTOR_ADDED_ALLIEN_ATTACK_RATIO = "Actor_AllienAttackRatio";
        public const string ACTOR_ADDED_ALLIEN_ATTACK_TAKE_RATIO = "Actor_AllienAttackTakeRatio";
        public const string ACTOR_ADDED_ATTACK_RATIO = "Actor_AddedAttackRatio";
        public const string ACTOR_ADDED_DAMAGE_RATIO = "Actor_AddedDamageRatio";
        public const string ACTOR_ADDED_DAMAGE_TAKE_RATIO = "Actor_DamageTakeRatio";
        public const string ACTOR_ADDED_FIRE_ATTACK_RATIO = "Actor_FireAttackRatio";
        public const string ACTOR_ADDED_FIRE_ATTACK_TAKE_RATIO = "Actor_FireAttackTakeRatio";
        public const string ACTOR_ADDED_ICE_ATTACK_RATIO = "Actor_IceAttackRatio";
        public const string ACTOR_ADDED_ICE_ATTACK_TAKE_RATIO = "Actor_IceAttackTakeRatio";
        public const string ACTOR_ADDED_NORMAL_ATTACK_RATIO = "Actor_NormalAttackRatio";
        public const string ACTOR_ADDED_NORMAL_ATTACK_TAKE_RATIO = "Actor_NormalAttackTakeRatio";
        public const string ACTOR_ADDED_THUNDER_ATTACK_RATIO = "Actor_ThunderAttackRatio";
        public const string ACTOR_ADDED_THUNDER_ATTACK_TAKE_RATIO = "Actor_ThunderAttackTakeRatio";
        public const string ACTOR_ANI_DAMAGE_DELTA = "Actor_AniDamageDelta";
        public const string ACTOR_ANI_DEFENCE_DELTA = "Actor_AniDefenceDelta";
        public const string ACTOR_ATTACK_DELTA = "Actor_AttackDelta";
        public const string ACTOR_ATTACK_RATIO = "Actor_AttackRatio";
        public const string ACTOR_ATTACK_STEAL_HP_RATIO = "Actor_AttackStealHPRatio";
        public const string ACTOR_BE_RETREAT_RATIO = "Actor_BeRetreatRatio";
        public const string ACTOR_COMBO_TIMER_DELTA = "Actor_ComboTimerDelta";
        public const string ACTOR_COMBO_TIMER_RATIO = "Actor_ComboTimerRatio";
        public const string ACTOR_CRITICAL_CHANCE_DELTA = "Actor_CriticalChanceDelta";
        public const string ACTOR_CRITICAL_DAMAGE_RATIO = "Actor_CriticalDamageRatio";
        public const string ACTOR_CRITICAL_DELTA = "Actor_CriticalDelta";
        public const string ACTOR_CRITICAL_RATIO = "Actor_CriticalRatio";
        public const string ACTOR_DAMAGE_REDUCE_RATIO = "Actor_DamageReduceRatio";
        public const string ACTOR_DEBUFF_DURATION_RATIO_DELTA = "Actor_DebuffDurationRatioDelta";
        public const string ACTOR_DEFENCE_DELTA = "Actor_DefenceDelta";
        public const string ACTOR_DEFENCE_RATIO = "Actor_DefenceRatio";
        public const string ACTOR_GOODS_ATTRACT_RATIUS = "Actor_GoodsAttrackRadius";
        public const string ACTOR_MAX_HP_DELTA = "Actor_MaxHPDelta";
        public const string ACTOR_MAX_HP_RATIO = "Actor_MaxHPRatio";
        public const string ACTOR_MAX_SP_DELTA = "Actor_MaxSPDelta";
        public const string ACTOR_MAX_SP_RATIO = "Actor_MaxSPRatio";
        public const string ACTOR_RESIST_ALL_ELEMENT_ATTACK_RATIO = "Actor_ResistAllElementAttackRatio";
        public const string ACTOR_RESIST_ALLIEN_ATTACK_RATIO = "Actor_ResistAllienAttackRatio";
        public const string ACTOR_RESIST_FIRE_ATTACK_RATIO = "Actor_ResistFireAttackRatio";
        public const string ACTOR_RESIST_ICE_ATTACK_RATIO = "Actor_ResistIceAttackRatio";
        public const string ACTOR_RESIST_NORMAL_ATTACK_RATIO = "Actor_ResistNormalAttackRatio";
        public const string ACTOR_RESIST_THUNDER_ATTACK_RATIO = "Actor_ResistThunderAttackRatio";
        public const string ACTOR_RETREAT_RATIO = "Actor_RetreatRatio";
        public const string ACTOR_SHIELD_DAMAGE_DELTA = "Actor_ShieldDamageDelta";
        public const string ACTOR_SHIELD_DAMAGE_RATIO = "Actor_ShieldDamageRatio";
        public const string ACTOR_SKL01_CD_RATIO = "Actor_SKL01CDRatio";
        public const string ACTOR_SKL02_CD_RATIO = "Actor_SKL02CDRatio";
        public const string ACTOR_SP_COST_DELTA = "Actor_SkillSPCostDelta";
        public const string ACTOR_SP_COST_RATIO = "Actor_SkillSPCostRatio";
        public const string ACTOR_SP_RECOVER_RATIO = "Actor_SPRecoverRatio";
        public const string ACTOR_THROW_ANI_DEFENCE_DELTA = "Actor_ThrowAniDefenceDelta";
        public const string AI_ATTACK_CD_RATIO = "AI_AttackCDRatio";
        public const string AI_CAN_TELEPORT = "AI_CanTeleport";
        public const float AI_CAN_TELEPORT_MIN_DISTANCE = 0.5f;
        public const string AI_IGNORE_MAX_ATTACK_NUM_CHANCE = "AI_IgnoreMaxAttackNumChance";
        public const string ANIMATOR_MOVE_SPEED = "Animator_MoveSpeedRatio";
        public const string ANIMATOR_OVERALL_SPEED = "Animator_OverallSpeedRatio";
        public const string ANIMATOR_OVERALL_SPEED_MULTIPLIED = "Animator_OverallSpeedRatioMultiplied";
        public const string ANIMATOR_RIGIDBODY_VELOCITY_RATIO = "Animator_RigidBodyVelocityRatio";
        public static AbilityState[] EMPTY = new AbilityState[0];
        public static Dictionary<string, object> EMPTY_OVERRIDE_MAP = new Dictionary<string, object>();
        public const string ENTITY_ATTACK_MOVE_RATIO = "Entity_AttackMoveRatio";
        public const string ENTITY_ATTACK_SPEED_RATIO = "Entity_AttackSpeed";
        public const string ENTITY_MASS_RATIO = "Entity_MassRatio";
        public const string ENTITY_TIME_SCALE_DELTA = "Entity_TimeScaleDelta";
        public const float LONG_PARALYZE_RESUME_DURATION = 0.5f;
        public static Dictionary<string, ConfigAbilityPropertyEntry> PROPERTIES;
        public const float SHORT_PARALYZE_RESUME_DURATION = 0.35f;
        public const float WITCH_TIME_RESUME_DURATION = 0.5f;

        static AbilityData()
        {
            for (int i = 0; i < ABILITY_STATE_CONTROL_DEBUFFS.Length; i++)
            {
                ABILITY_STATE_CONTROL_DEBUFFS_MASK |= ABILITY_STATE_CONTROL_DEBUFFS[i];
            }
            AbilityState[][] stateArrayArray1 = new AbilityState[3][];
            stateArrayArray1[0] = new AbilityState[] { AbilityState.Frozen, AbilityState.Paralyze, AbilityState.Stun };
            stateArrayArray1[1] = new AbilityState[] { AbilityState.MaxMoveSpeed, AbilityState.MoveSpeedUp };
            stateArrayArray1[2] = new AbilityState[] { AbilityState.MaxMoveSpeed, AbilityState.MoveSpeedDown };
            ABILITY_STATE_PRECEDENCE_MAP = stateArrayArray1;
            PROPERTIES = new Dictionary<string, ConfigAbilityPropertyEntry>();
            DefineEntityProperty("Entity_TimeScaleDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineEntityProperty("Entity_AttackSpeed", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineEntityProperty("Entity_AttackMoveRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineEntityProperty("Entity_MassRatio", 0f, FixedFloatStack.StackMethod.Sum, 0f, 100f);
            DefineEntityProperty("Animator_MoveSpeedRatio", 0f, FixedFloatStack.StackMethod.Sum, -0.8f, 1f);
            DefineEntityProperty("Animator_OverallSpeedRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineEntityProperty("Animator_OverallSpeedRatioMultiplied", 1f, FixedFloatStack.StackMethod.Multiplied, float.MinValue, float.MaxValue);
            DefineEntityProperty("AI_AttackCDRatio", 1f, FixedFloatStack.StackMethod.Multiplied, float.MinValue, float.MaxValue);
            DefineEntityProperty("AI_IgnoreMaxAttackNumChance", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineEntityProperty("AI_CanTeleport", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineEntityProperty("Animator_RigidBodyVelocityRatio", 0f, FixedFloatStack.StackMethod.Sum, -1f, 3f);
            DefineEntityProperty("Actor_GoodsAttrackRadius", 1f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_AniDamageDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_AniDefenceDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ThrowAniDefenceDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_CriticalDamageRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_DefenceRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_DefenceDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_CriticalChanceDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_CriticalRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_CriticalDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ShieldDamageRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ShieldDamageDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_MaxHPRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_MaxHPDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_MaxSPRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_SkillSPCostRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_SkillSPCostDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_MaxSPDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_SPRecoverRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_RetreatRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ComboTimerRatio", 1f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ComboTimerDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_AttackStealHPRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_DebuffDurationRatioDelta", 0f, FixedFloatStack.StackMethod.OneMinusMultiplied, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_AttackDelta", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_AttackRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_AddedDamageRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_AddedAttackRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_NormalAttackRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_FireAttackRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ThunderAttackRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_IceAttackRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_AllienAttackRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_DamageReduceRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ResistAllElementAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ResistNormalAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ResistFireAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ResistThunderAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ResistIceAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ResistAllienAttackRatio", 1f, FixedFloatStack.StackMethod.OneMinusMultiplied, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_DamageTakeRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_NormalAttackTakeRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_FireAttackTakeRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_ThunderAttackTakeRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_IceAttackTakeRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_AllienAttackTakeRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_SKL01CDRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_SKL02CDRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
            DefineActorProperty("Actor_BeRetreatRatio", 0f, FixedFloatStack.StackMethod.Sum, float.MinValue, float.MaxValue);
        }

        public static int CalculateContentHash()
        {
            int lastHash = 0;
            foreach (ConfigOverrideGroup group in _abilityGroupMap.Values)
            {
                HashUtils.TryHashObject(group.Default, ref lastHash);
                if (group.Overrides != null)
                {
                    foreach (KeyValuePair<string, object> pair in group.Overrides)
                    {
                        HashUtils.TryHashObject(pair, ref lastHash);
                    }
                }
            }
            return lastHash;
        }

        private static void DefineActorProperty(string propertyName, float defaultValue, FixedFloatStack.StackMethod valueType, float floor = -3.402823E+38f, float ceiling = 3.402823E+38f)
        {
            ConfigAbilityPropertyEntry entry = new ConfigAbilityPropertyEntry {
                Type = ConfigAbilityPropertyEntry.PropertyType.Actor,
                Default = defaultValue,
                Stacking = valueType,
                Floor = floor,
                Ceiling = ceiling
            };
            PROPERTIES.Add(propertyName, entry);
        }

        private static void DefineEntityProperty(string propertyName, float defaultValue, FixedFloatStack.StackMethod valueType, float floor = -3.402823E+38f, float ceiling = 3.402823E+38f)
        {
            ConfigAbilityPropertyEntry entry = new ConfigAbilityPropertyEntry {
                Type = ConfigAbilityPropertyEntry.PropertyType.Entity,
                Default = defaultValue,
                Stacking = valueType,
                Floor = floor,
                Ceiling = ceiling
            };
            PROPERTIES.Add(propertyName, entry);
        }

        public static ConfigAbility GetAbilityConfig(string abilityName)
        {
            return GetAbilityConfig(abilityName, "Default");
        }

        public static ConfigAbility GetAbilityConfig(string abilityName, string overrideName)
        {
            return _abilityGroupMap[abilityName].GetConfig<ConfigAbility>(overrideName);
        }

        public static Dictionary<string, ConfigOverrideGroup> GetAbilityGroupMap()
        {
            return _abilityGroupMap;
        }

        public static string[] GetAllAbilityNames()
        {
            if (_abilityGroupMap == null)
            {
                return new string[0];
            }
            string[] array = new string[_abilityGroupMap.Count];
            _abilityGroupMap.Keys.CopyTo(array, 0);
            return array;
        }

        public static void GetStateIndiceInPrecedenceMap(AbilityState state, out AbilityState[] precedenceTrack, out int stateIx)
        {
            AbilityState[] stateArray = null;
            stateIx = 0;
            for (int i = 0; i < ABILITY_STATE_PRECEDENCE_MAP.Length; i++)
            {
                stateArray = ABILITY_STATE_PRECEDENCE_MAP[i];
                for (int j = 0; j < stateArray.Length; j++)
                {
                    if (stateArray[j] == state)
                    {
                        precedenceTrack = stateArray;
                        stateIx = j;
                        return;
                    }
                }
            }
            precedenceTrack = null;
        }

        public static bool IsModifierBuff(ConfigAbilityModifier config)
        {
            return (config.IsBuff || ((config.State & (AbilityState.Undamagable | AbilityState.MaxMoveSpeed | AbilityState.Immune | AbilityState.CritUp | AbilityState.Shielded | AbilityState.PowerUp | AbilityState.AttackSpeedUp | AbilityState.MoveSpeedUp | AbilityState.Endure)) != AbilityState.None));
        }

        public static bool IsModifierDebuff(ConfigAbilityModifier config)
        {
            return (config.IsDebuff || ((config.State & (AbilityState.Tied | AbilityState.TargetLocked | AbilityState.Fragile | AbilityState.Weak | AbilityState.AttackSpeedDown | AbilityState.MoveSpeedDown | AbilityState.Frozen | AbilityState.Poisoned | AbilityState.Burn | AbilityState.Paralyze | AbilityState.Stun | AbilityState.Bleed)) != AbilityState.None));
        }

        private static void OnLoadOneJsonConfigFinish(ConfigAbilityRegistry abilityGroupList, string configPath)
        {
            _configPathList.Remove(configPath);
            foreach (ConfigOverrideGroup group in abilityGroupList)
            {
                try
                {
                    ConfigAbility config = group.GetConfig<ConfigAbility>("Default");
                    _abilityGroupMap.Add(config.AbilityName, group);
                }
                catch
                {
                    UnityEngine.Debug.LogError("Error during loading ability file: " + configPath);
                    throw;
                }
            }
            if (_configPathList.Count == 0)
            {
                _loadDataBackGroundWorker.StopBackGroundWork(false);
                if (_loadJsonConfigCallback != null)
                {
                    _loadJsonConfigCallback("AbilityData");
                    _loadJsonConfigCallback = null;
                }
            }
        }

        public static void ReloadFromFile()
        {
            _abilityGroupMap = new Dictionary<string, ConfigOverrideGroup>();
            foreach (string str in GlobalDataManager.metaConfig.abilityRegistryPathes)
            {
                foreach (ConfigOverrideGroup group in ConfigUtil.LoadJSONConfig<ConfigAbilityRegistry>(str))
                {
                    try
                    {
                        ConfigAbility config = group.GetConfig<ConfigAbility>("Default");
                        _abilityGroupMap.Add(config.AbilityName, group);
                    }
                    catch
                    {
                        UnityEngine.Debug.LogError("Error during loading ability file: " + str);
                        throw;
                    }
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator9 { finishCallback = finishCallback, progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>finishCallback = finishCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        public static void SetupParamSpecial(ConfigAbility abilityConfig, Dictionary<string, object> overrideMap, string paramSpecial, ParamMethod paramMethod, float paramValue)
        {
            float num2;
            float num = (float) abilityConfig.AbilitySpecials[paramSpecial];
            switch (paramMethod)
            {
                case ParamMethod.Replace:
                    num2 = paramValue;
                    break;

                case ParamMethod.Add:
                    num2 = num + paramValue;
                    break;

                case ParamMethod.Minus:
                    num2 = num - paramValue;
                    break;

                case ParamMethod.OneAddMultipled:
                    num2 = num * (1f + paramValue);
                    break;

                case ParamMethod.Negative:
                    num2 = -paramValue;
                    break;

                default:
                    num2 = 0f;
                    break;
            }
            overrideMap.Add(paramSpecial, num2);
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator9 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>finishCallback;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal string[] <$s_883>__1;
            internal int <$s_884>__2;
            internal string[] <$s_885>__5;
            internal int <$s_886>__6;
            internal string <abilityRegistryPath>__3;
            internal string <abilityRegistryPath>__7;
            internal AsyncAssetRequst <asyncRequest>__8;
            internal string[] <pathes>__0;
            internal float <step>__4;
            internal Action<string> finishCallback;
            internal Action<float> moveOneStepCallback;
            internal float progressSpan;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        AbilityData._loadJsonConfigCallback = this.finishCallback;
                        AbilityData._configPathList = new List<string>();
                        AbilityData._abilityGroupMap = new Dictionary<string, ConfigOverrideGroup>();
                        this.<pathes>__0 = GlobalDataManager.metaConfig.abilityRegistryPathes;
                        if (this.<pathes>__0.Length != 0)
                        {
                            this.<$s_883>__1 = this.<pathes>__0;
                            this.<$s_884>__2 = 0;
                            while (this.<$s_884>__2 < this.<$s_883>__1.Length)
                            {
                                this.<abilityRegistryPath>__3 = this.<$s_883>__1[this.<$s_884>__2];
                                AbilityData._configPathList.Add(this.<abilityRegistryPath>__3);
                                this.<$s_884>__2++;
                            }
                            this.<step>__4 = this.progressSpan / ((float) this.<pathes>__0.Length);
                            AbilityData._loadDataBackGroundWorker.StartBackGroundWork("AbilityData");
                            this.<$s_885>__5 = this.<pathes>__0;
                            this.<$s_886>__6 = 0;
                            while (this.<$s_886>__6 < this.<$s_885>__5.Length)
                            {
                                this.<abilityRegistryPath>__7 = this.<$s_885>__5[this.<$s_886>__6];
                                this.<asyncRequest>__8 = ConfigUtil.LoadJsonConfigAsync(this.<abilityRegistryPath>__7, BundleType.DATA_FILE);
                                SuperDebug.VeryImportantAssert(this.<asyncRequest>__8 != null, "assetRequest is null abilityPath :" + this.<abilityRegistryPath>__7);
                                if (this.<asyncRequest>__8 == null)
                                {
                                    goto Label_01D5;
                                }
                                this.$current = this.<asyncRequest>__8.operation;
                                this.$PC = 1;
                                return true;
                            Label_018D:
                                if (this.moveOneStepCallback != null)
                                {
                                    this.moveOneStepCallback(this.<step>__4);
                                }
                                ConfigUtil.LoadJSONStrConfigMultiThread<ConfigAbilityRegistry>(this.<asyncRequest>__8.asset.ToString(), AbilityData._loadDataBackGroundWorker, new Action<ConfigAbilityRegistry, string>(AbilityData.OnLoadOneJsonConfigFinish), this.<abilityRegistryPath>__7);
                            Label_01D5:
                                this.<$s_886>__6++;
                            }
                            this.$PC = -1;
                            break;
                        }
                        if (AbilityData._loadJsonConfigCallback != null)
                        {
                            AbilityData._loadJsonConfigCallback("AbilityData");
                            AbilityData._loadJsonConfigCallback = null;
                        }
                        break;

                    case 1:
                        goto Label_018D;
                }
                return false;
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
    }
}

