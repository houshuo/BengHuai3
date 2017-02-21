namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class EquipmentSkillData
    {
        private static List<string> _configPathList;
        public static Dictionary<int, ConfigEquipmentSkillEntry> _equipSkillMap;
        private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();
        private static Action<string> _loadJsonConfigCallback = null;
        [CompilerGenerated]
        private static Predicate<StigmataDataItem> <>f__am$cache4;

        public static void AddAvatarSetEquipSkillAbilities(AvatarDataItem avatarDataItem, AvatarActor avatarActor, ref List<ConfigEquipmentSkillEntry> skillEntryList)
        {
            if (avatarDataItem.GetOwnEquipSetData() != null)
            {
                foreach (EquipSkillDataItem item2 in avatarDataItem.GetOwnEquipSetData().GetOwnSetSkills().Values)
                {
                    ConfigEquipmentSkillEntry skillConfig = null;
                    ApplyEquipSkillEntry(avatarActor, item2.ID, item2.GetSkillParam1(1), item2.GetSkillParam2(1), item2.GetSkillParam3(1), out skillConfig);
                    if (skillConfig != null)
                    {
                        skillEntryList.Add(skillConfig);
                    }
                }
            }
        }

        public static void AddAvatarStigmataEquipSkillAbilities(AvatarDataItem avatarDataItem, AvatarActor avatarActor, ref List<ConfigEquipmentSkillEntry> skillEntryList)
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = x => x != null;
            }
            List<StigmataDataItem> list = avatarDataItem.GetStigmataList().FindAll(<>f__am$cache4);
            int num = 0;
            int count = list.Count;
            while (num < count)
            {
                List<EquipSkillDataItem> skillsWithAffix = list[num].GetSkillsWithAffix();
                for (int i = 0; i < skillsWithAffix.Count; i++)
                {
                    EquipSkillDataItem item = skillsWithAffix[i];
                    ConfigEquipmentSkillEntry skillConfig = null;
                    ApplyEquipSkillEntry(avatarActor, item.ID, item.GetSkillParam1(list[num].level), item.GetSkillParam2(list[num].level), item.GetSkillParam3(list[num].level), out skillConfig);
                    if (skillConfig != null)
                    {
                        skillEntryList.Add(skillConfig);
                    }
                }
                num++;
            }
        }

        public static void AddAvatarWeaponEquipSkillAbilities(AvatarDataItem avatarDataItem, AvatarActor avatarActor, ref List<ConfigEquipmentSkillEntry> skillEntryList)
        {
            WeaponDataItem weapon = avatarDataItem.GetWeapon();
            List<EquipSkillDataItem> skills = weapon.skills;
            for (int i = 0; i < skills.Count; i++)
            {
                EquipSkillDataItem item2 = skills[i];
                ConfigEquipmentSkillEntry skillConfig = null;
                ApplyEquipSkillEntry(avatarActor, item2.ID, item2.GetSkillParam1(weapon.level), item2.GetSkillParam2(weapon.level), item2.GetSkillParam3(weapon.level), out skillConfig);
                if (skillConfig != null)
                {
                    skillEntryList.Add(skillConfig);
                }
            }
            WeaponData.AddAvatarWeaponAdditionalAbilities(weapon.ID, avatarActor);
        }

        public static void ApplyEquipSkillEntry(AvatarActor avatarActor, int equipSkillID, float calculatedParam1, float calculatedParam2, float calculatedParam3, out ConfigEquipmentSkillEntry skillConfig)
        {
            skillConfig = null;
            if ((equipSkillID > 0) && _equipSkillMap.ContainsKey(equipSkillID))
            {
                skillConfig = GetEquipmentSkillConfig(equipSkillID);
                ConfigAbility abilityConfig = AbilityData.GetAbilityConfig(skillConfig.AbilityName, skillConfig.AbilityOverride);
                Dictionary<string, object> overrideMap = avatarActor.CreateAppliedAbility(abilityConfig);
                if (skillConfig.ParamSpecial1 != null)
                {
                    AbilityData.SetupParamSpecial(abilityConfig, overrideMap, skillConfig.ParamSpecial1, skillConfig.ParamMethod1, calculatedParam1);
                }
                if (skillConfig.ParamSpecial2 != null)
                {
                    AbilityData.SetupParamSpecial(abilityConfig, overrideMap, skillConfig.ParamSpecial2, skillConfig.ParamMethod2, calculatedParam2);
                }
                if (skillConfig.ParamSpecial3 != null)
                {
                    AbilityData.SetupParamSpecial(abilityConfig, overrideMap, skillConfig.ParamSpecial3, skillConfig.ParamMethod3, calculatedParam3);
                }
            }
        }

        public static ConfigEquipmentSkillEntry GetEquipmentSkillConfig(int equipSkillID)
        {
            return _equipSkillMap[equipSkillID];
        }

        private static void OnLoadOneJsonConfigFinish(ConfigEquipmentSkillRegistry weaponList, string configPath)
        {
            _configPathList.Remove(configPath);
            foreach (ConfigEquipmentSkillEntry entry in weaponList)
            {
                _equipSkillMap.Add(entry.EquipmentSkillID, entry);
            }
            if (_configPathList.Count == 0)
            {
                _loadDataBackGroundWorker.StopBackGroundWork(false);
                if (_loadJsonConfigCallback != null)
                {
                    _loadJsonConfigCallback("EquipmentSkillData");
                    _loadJsonConfigCallback = null;
                }
            }
        }

        public static void ReloadFromFile()
        {
            _equipSkillMap = new Dictionary<int, ConfigEquipmentSkillEntry>();
            foreach (string str in GlobalDataManager.metaConfig.equipmentSkillRegistryPathes)
            {
                foreach (ConfigEquipmentSkillEntry entry in ConfigUtil.LoadJSONConfig<ConfigEquipmentSkillRegistry>(str))
                {
                    _equipSkillMap.Add(entry.EquipmentSkillID, entry);
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
        {
            return new <ReloadFromFileAsync>c__IteratorC { finishCallback = finishCallback, progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>finishCallback = finishCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__IteratorC : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>finishCallback;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal string[] <$s_900>__1;
            internal int <$s_901>__2;
            internal string[] <$s_902>__5;
            internal int <$s_903>__6;
            internal AsyncAssetRequst <asyncRequest>__8;
            internal string <equipRegistryPath>__3;
            internal string <equipRegistryPath>__7;
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
                        EquipmentSkillData._loadJsonConfigCallback = this.finishCallback;
                        EquipmentSkillData._configPathList = new List<string>();
                        EquipmentSkillData._equipSkillMap = new Dictionary<int, ConfigEquipmentSkillEntry>();
                        this.<pathes>__0 = GlobalDataManager.metaConfig.equipmentSkillRegistryPathes;
                        if (this.<pathes>__0.Length != 0)
                        {
                            this.<$s_900>__1 = this.<pathes>__0;
                            this.<$s_901>__2 = 0;
                            while (this.<$s_901>__2 < this.<$s_900>__1.Length)
                            {
                                this.<equipRegistryPath>__3 = this.<$s_900>__1[this.<$s_901>__2];
                                EquipmentSkillData._configPathList.Add(this.<equipRegistryPath>__3);
                                this.<$s_901>__2++;
                            }
                            this.<step>__4 = this.progressSpan / ((float) this.<pathes>__0.Length);
                            EquipmentSkillData._loadDataBackGroundWorker.StartBackGroundWork("EquipmentSkillData");
                            this.<$s_902>__5 = this.<pathes>__0;
                            this.<$s_903>__6 = 0;
                            while (this.<$s_903>__6 < this.<$s_902>__5.Length)
                            {
                                this.<equipRegistryPath>__7 = this.<$s_902>__5[this.<$s_903>__6];
                                this.<asyncRequest>__8 = ConfigUtil.LoadJsonConfigAsync(this.<equipRegistryPath>__7, BundleType.DATA_FILE);
                                SuperDebug.VeryImportantAssert(this.<asyncRequest>__8 != null, "assetRequest is null equipRegistryPath :" + this.<equipRegistryPath>__7);
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
                                ConfigUtil.LoadJSONStrConfigMultiThread<ConfigEquipmentSkillRegistry>(this.<asyncRequest>__8.asset.ToString(), EquipmentSkillData._loadDataBackGroundWorker, new Action<ConfigEquipmentSkillRegistry, string>(EquipmentSkillData.OnLoadOneJsonConfigFinish), this.<equipRegistryPath>__7);
                            Label_01D5:
                                this.<$s_903>__6++;
                            }
                            this.$PC = -1;
                            break;
                        }
                        if (EquipmentSkillData._loadJsonConfigCallback != null)
                        {
                            EquipmentSkillData._loadJsonConfigCallback("EquipmentSkillData");
                            EquipmentSkillData._loadJsonConfigCallback = null;
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

