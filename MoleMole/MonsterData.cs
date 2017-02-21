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

    public static class MonsterData
    {
        private static List<string> _configPathList;
        private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();
        private static Action<string> _loadJsonConfigCallback = null;
        private static bool _monsterDataInited = false;
        public const string ELITE_MONSTER_EFFECT_PATTERN = "Monster_Elite_01";
        public const float ELITE_MONSTER_SCALE = 1.1f;
        public const string FROZEN_DIE_EFFECT_PATTERN = "Frozen_Die";
        public static int MONSTER_ATK_TAG;
        public static int MONSTER_ATKBS_TAG;
        public static int MONSTER_DIE_TAG;
        public static Shader MONSTER_ELITE_SHADER;
        public const float MONSTER_FAST_KILL_ANI_DAMAGE_RATIO = 0.9f;
        public const float MONSTER_FAST_KILL_HOLD_DURATION = 0.3f;
        public static int MONSTER_FREEZE_DIR_TAG;
        public static int MONSTER_HIT_TAG;
        public static int MONSTER_IDLESUB_TAG;
        public const string MONSTER_LEVEL_TABLE_FILE_PATH = "Entities/Monster/";
        public static int MONSTER_MOVESUB_TAG;
        public const int MONSTER_NUMBER_LENGTH = 3;
        public static Shader MONSTER_OPAQUE_SHADER;
        private const string MONSTER_PREFAB_LOW_SUFFIX = "_Low";
        private const string MONSTER_PREFAB_PATH = "Entities/Monster/";
        public static int MONSTER_SKL_TAG;
        public static HashSet<int>[] MONSTER_TAG_GROUPS;
        public const float MONSTER_THROW_FAST_KILL_WAIT_DURATION = 0.1f;
        public const float MONSTER_THROW_MASS_RATIO = 0.1f;
        public static int MONSTER_THROWSUB_TAG;
        public static Shader MONSTER_TRANSPARENT_SHADER;
        public static Dictionary<string, ConfigOverrideGroup> monsterGroupMap;
        public const float WALL_GROUND_CONTACT_DEGREE_THRESHOLD = 20f;
        public const float WALL_STEER_ANGLE_FACTOR = 0.01f;

        private static void AllMonsterConfigOnLevelLoaded()
        {
            foreach (ConfigOverrideGroup group in monsterGroupMap.Values)
            {
                ((ConfigMonster) group.Default).OnLevelLoaded();
                if (group.Overrides != null)
                {
                    foreach (object obj2 in group.Overrides.Values)
                    {
                        ((ConfigMonster) obj2).OnLevelLoaded();
                    }
                }
            }
        }

        public static int CalculateContentHash()
        {
            int lastHash = 0;
            foreach (ConfigOverrideGroup group in monsterGroupMap.Values)
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

        public static List<MonsterConfigMetaData> GetAllMonsterConfigMetaData()
        {
            return MonsterConfigMetaDataReader.GetItemList();
        }

        public static ConfigMonster GetFirstMonsterConfigBySubTypeName(string subTypeName)
        {
            foreach (MonsterConfigMetaData data in MonsterConfigMetaDataReader.GetItemList())
            {
                if (data.subTypeName == subTypeName)
                {
                    string configFile = data.configFile;
                    string configType = data.configType;
                    return monsterGroupMap[configFile].GetConfig<ConfigMonster>(configType);
                }
            }
            return null;
        }

        public static ConfigMonster GetMonsterConfig(string monsterName, string typeName, string configType = "")
        {
            MonsterConfigMetaData monsterConfigMetaDataByKey = MonsterConfigMetaDataReader.GetMonsterConfigMetaDataByKey(monsterName, typeName);
            string configFile = monsterConfigMetaDataByKey.configFile;
            string name = configType;
            if (name == string.Empty)
            {
                name = monsterConfigMetaDataByKey.configType;
            }
            return monsterGroupMap[configFile].GetConfig<ConfigMonster>(name);
        }

        public static MonsterConfigMetaData GetMonsterConfigMetaData(string monsterName, string typeName)
        {
            return MonsterConfigMetaDataReader.GetMonsterConfigMetaDataByKey(monsterName, typeName);
        }

        public static string GetPrefabResPath(string monsterName, string typeName, bool useLow = false)
        {
            MonsterConfigMetaData monsterConfigMetaDataByKey = MonsterConfigMetaDataReader.GetMonsterConfigMetaDataByKey(monsterName, typeName);
            string categoryName = monsterConfigMetaDataByKey.categoryName;
            string subTypeName = monsterConfigMetaDataByKey.subTypeName;
            ConfigMonster monster = GetMonsterConfig(monsterConfigMetaDataByKey.monsterName, monsterConfigMetaDataByKey.typeName, string.Empty);
            if (useLow && monster.CommonArguments.HasLowPrefab)
            {
                string[] textArray1 = new string[] { "Entities/Monster/", categoryName, "/", subTypeName, "/", subTypeName, "_Low" };
                return string.Concat(textArray1);
            }
            string[] textArray2 = new string[] { "Entities/Monster/", categoryName, "/", subTypeName, "/", subTypeName };
            return string.Concat(textArray2);
        }

        public static UniqueMonsterMetaData GetUniqueMonsterMetaData(uint uniqueMonsterID)
        {
            return UniqueMonsterMetaDataReader.GetUniqueMonsterMetaDataByKey(uniqueMonsterID);
        }

        public static void InitMonsterData()
        {
            if (!_monsterDataInited)
            {
                AllMonsterConfigOnLevelLoaded();
                MONSTER_FREEZE_DIR_TAG = Animator.StringToHash("MONSTER_FREEZE_DIR");
                MONSTER_IDLESUB_TAG = Animator.StringToHash("MONSTER_IDLESUB");
                MONSTER_MOVESUB_TAG = Animator.StringToHash("MONSTER_MOVESUB");
                MONSTER_HIT_TAG = Animator.StringToHash("MONSTER_HITSUB");
                MONSTER_DIE_TAG = Animator.StringToHash("MONSTER_DIESUB");
                MONSTER_ATKBS_TAG = Animator.StringToHash("MONSTER_ATKBS");
                MONSTER_ATK_TAG = Animator.StringToHash("MONSTER_ATK");
                MONSTER_SKL_TAG = Animator.StringToHash("MONSTER_SKL");
                MONSTER_THROWSUB_TAG = Animator.StringToHash("MONSTER_THROWSUB");
                MONSTER_TAG_GROUPS = new HashSet<int>[] { new HashSet<int> { MONSTER_HIT_TAG }, new HashSet<int> { MONSTER_HIT_TAG, MONSTER_ATK_TAG, MONSTER_DIE_TAG, MONSTER_THROWSUB_TAG, MONSTER_FREEZE_DIR_TAG }, new HashSet<int> { MONSTER_THROWSUB_TAG }, new HashSet<int> { MONSTER_MOVESUB_TAG }, new HashSet<int> { MONSTER_MOVESUB_TAG, MONSTER_IDLESUB_TAG }, new HashSet<int> { MONSTER_MOVESUB_TAG, MONSTER_IDLESUB_TAG, MONSTER_SKL_TAG, MONSTER_ATK_TAG, MONSTER_HIT_TAG, MONSTER_FREEZE_DIR_TAG }, new HashSet<int> { MONSTER_IDLESUB_TAG }, new HashSet<int> { MONSTER_ATK_TAG, MONSTER_ATKBS_TAG, MONSTER_SKL_TAG } };
                MONSTER_OPAQUE_SHADER = Shader.Find("miHoYo/Character/Simple_Emission_Opaque");
                MONSTER_TRANSPARENT_SHADER = Shader.Find("miHoYo/Character/Simple_Emission");
                MONSTER_ELITE_SHADER = Shader.Find("miHoYo/Character/Simple_Emission_Elite");
            }
        }

        public static void LoadAllMonsterConfig()
        {
            monsterGroupMap = new Dictionary<string, ConfigOverrideGroup>();
            List<MonsterConfigMetaData> itemList = MonsterConfigMetaDataReader.GetItemList();
            HashSet<string> set = new HashSet<string>();
            foreach (MonsterConfigMetaData data in itemList)
            {
                string configFile = data.configFile;
                set.Add(configFile);
            }
            foreach (string str2 in set)
            {
                ConfigOverrideGroup group = ConfigUtil.LoadJSONConfig<ConfigOverrideGroup>("Data/MonsterConfig/" + str2);
                monsterGroupMap.Add(str2, group);
            }
        }

        [DebuggerHidden]
        public static IEnumerator LoadAllMonsterConfigAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <LoadAllMonsterConfigAsync>c__IteratorF { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        private static void OnLoadOneJsonConfigFinish(ConfigOverrideGroup configGroup, string configFile)
        {
            string item = "Data/MonsterConfig/" + configFile;
            _configPathList.Remove(item);
            monsterGroupMap.Add(configFile, configGroup);
            if (_configPathList.Count == 0)
            {
                _loadDataBackGroundWorker.StopBackGroundWork(false);
                if (_loadJsonConfigCallback != null)
                {
                    _loadJsonConfigCallback("MonsterData");
                    _loadJsonConfigCallback = null;
                }
            }
        }

        public static void ReloadFromFile()
        {
            MonsterConfigMetaDataReader.LoadFromFile();
            UniqueMonsterMetaDataReader.LoadFromFile();
            LoadAllMonsterConfig();
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
        {
            return new <ReloadFromFileAsync>c__IteratorE { finishCallback = finishCallback, progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>finishCallback = finishCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <LoadAllMonsterConfigAsync>c__IteratorF : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal List<MonsterConfigMetaData>.Enumerator <$s_916>__2;
            internal HashSet<string>.Enumerator <$s_917>__5;
            internal HashSet<string>.Enumerator <$s_918>__9;
            internal AsyncAssetRequst <asyncRequest>__12;
            internal string <configFile>__10;
            internal string <configFile>__6;
            internal HashSet<string> <ConfigFiles>__1;
            internal string <configPath>__4;
            internal List<MonsterConfigMetaData> <itemList>__0;
            internal MonsterConfigMetaData <metaData>__3;
            internal string <path>__11;
            internal string <path>__7;
            internal float <step>__8;
            internal Action<float> moveOneStepCallback;
            internal float progressSpan;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            this.<$s_918>__9.Dispose();
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
                        MonsterData.monsterGroupMap = new Dictionary<string, ConfigOverrideGroup>();
                        this.<itemList>__0 = MonsterConfigMetaDataReader.GetItemList();
                        this.<ConfigFiles>__1 = new HashSet<string>();
                        if (this.<itemList>__0.Count != 0)
                        {
                            this.<$s_916>__2 = this.<itemList>__0.GetEnumerator();
                            try
                            {
                                while (this.<$s_916>__2.MoveNext())
                                {
                                    this.<metaData>__3 = this.<$s_916>__2.Current;
                                    this.<configPath>__4 = this.<metaData>__3.configFile;
                                    this.<ConfigFiles>__1.Add(this.<configPath>__4);
                                }
                            }
                            finally
                            {
                                this.<$s_916>__2.Dispose();
                            }
                            this.<$s_917>__5 = this.<ConfigFiles>__1.GetEnumerator();
                            try
                            {
                                while (this.<$s_917>__5.MoveNext())
                                {
                                    this.<configFile>__6 = this.<$s_917>__5.Current;
                                    this.<path>__7 = "Data/MonsterConfig/" + this.<configFile>__6;
                                    MonsterData._configPathList.Add(this.<path>__7);
                                }
                            }
                            finally
                            {
                                this.<$s_917>__5.Dispose();
                            }
                            this.<step>__8 = this.progressSpan / ((float) this.<ConfigFiles>__1.Count);
                            MonsterData._loadDataBackGroundWorker.StartBackGroundWork("MonsterData");
                            this.<$s_918>__9 = this.<ConfigFiles>__1.GetEnumerator();
                            num = 0xfffffffd;
                            break;
                        }
                        if (MonsterData._loadJsonConfigCallback != null)
                        {
                            MonsterData._loadJsonConfigCallback("MonsterData");
                            MonsterData._loadJsonConfigCallback = null;
                        }
                        goto Label_02A9;

                    case 1:
                        break;

                    default:
                        goto Label_02A9;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_0230;
                    }
                    while (this.<$s_918>__9.MoveNext())
                    {
                        this.<configFile>__10 = this.<$s_918>__9.Current;
                        this.<path>__11 = "Data/MonsterConfig/" + this.<configFile>__10;
                        this.<asyncRequest>__12 = ConfigUtil.LoadJsonConfigAsync(this.<path>__11, BundleType.DATA_FILE);
                        SuperDebug.VeryImportantAssert(this.<asyncRequest>__12 != null, "assetRequest is null monsterPath :" + this.<path>__11);
                        if (this.<asyncRequest>__12 == null)
                        {
                            continue;
                        }
                        this.$current = this.<asyncRequest>__12.operation;
                        this.$PC = 1;
                        flag = true;
                        return true;
                    Label_0230:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__8);
                        }
                        ConfigUtil.LoadJSONStrConfigMultiThread<ConfigOverrideGroup>(this.<asyncRequest>__12.asset.ToString(), MonsterData._loadDataBackGroundWorker, new Action<ConfigOverrideGroup, string>(MonsterData.OnLoadOneJsonConfigFinish), this.<configFile>__10);
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.<$s_918>__9.Dispose();
                }
                this.$PC = -1;
            Label_02A9:
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

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__IteratorE : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>finishCallback;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
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
                        MonsterData._loadJsonConfigCallback = this.finishCallback;
                        MonsterData._configPathList = new List<string>();
                        MonsterConfigMetaDataReader.LoadFromFile();
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_00A4;

                    case 1:
                        UniqueMonsterMetaDataReader.LoadFromFile();
                        this.$current = null;
                        this.$PC = 2;
                        goto Label_00A4;

                    case 2:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(MonsterData.LoadAllMonsterConfigAsync(this.progressSpan, this.moveOneStepCallback));
                        this.$PC = 3;
                        goto Label_00A4;

                    case 3:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_00A4:
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

        public enum MonsterTagGroup
        {
            ShowHit,
            FreezeDirection,
            Throw,
            Movement,
            IdleOrMovement,
            Grounded,
            Idle,
            AttackOrSkill
        }
    }
}

