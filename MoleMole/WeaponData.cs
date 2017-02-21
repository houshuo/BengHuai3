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

    public static class WeaponData
    {
        private static List<string> _configPathList;
        private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();
        private static Action<string> _loadJsonConfigCallback = null;
        public static Dictionary<int, ConfigWeapon> _weaponIDMap;

        public static void AddAvatarWeaponAdditionalAbilities(int weaponID, AvatarActor avatar)
        {
            ConfigWeapon weaponConfig = GetWeaponConfig(weaponID);
            for (int i = 0; i < weaponConfig.AdditionalAbilities.Length; i++)
            {
                ConfigAbility abilityConfig = AbilityData.GetAbilityConfig(weaponConfig.AdditionalAbilities[i].AbilityName, weaponConfig.AdditionalAbilities[i].AbilityOverride);
                avatar.CreateAppliedAbility(abilityConfig);
                if (!string.IsNullOrEmpty(weaponConfig.AdditionalAbilities[i].AbilityReplaceID))
                {
                    avatar.abilityIDMap[weaponConfig.AdditionalAbilities[i].AbilityReplaceID] = abilityConfig.AbilityName;
                }
            }
        }

        public static Dictionary<int, ConfigWeapon> GetAllWeaponConfigs()
        {
            return _weaponIDMap;
        }

        public static int GetFirstWeaponIDForRole(EntityRoleName role)
        {
            foreach (ConfigWeapon weapon in _weaponIDMap.Values)
            {
                if (weapon.OwnerRole == role)
                {
                    return weapon.WeaponID;
                }
            }
            return 0;
        }

        public static ConfigWeapon GetWeaponConfig(int weaponID)
        {
            return _weaponIDMap[weaponID];
        }

        private static void OnLoadOneJsonConfigFinish(ConfigWeaponRegistry weaponList, string configPath)
        {
            _configPathList.Remove(configPath);
            foreach (ConfigWeapon weapon in weaponList)
            {
                _weaponIDMap.Add(weapon.WeaponID, weapon);
            }
            if (_configPathList.Count == 0)
            {
                _loadDataBackGroundWorker.StopBackGroundWork(false);
                ReloadWeaponConfig();
                if (_loadJsonConfigCallback != null)
                {
                    _loadJsonConfigCallback("WeaponData");
                    _loadJsonConfigCallback = null;
                }
            }
        }

        public static void ReloadFromFile()
        {
            _weaponIDMap = new Dictionary<int, ConfigWeapon>();
            foreach (string str in GlobalDataManager.metaConfig.weaponRegistryPathes)
            {
                foreach (ConfigWeapon weapon in ConfigUtil.LoadJSONConfig<ConfigWeaponRegistry>(str))
                {
                    _weaponIDMap.Add(weapon.WeaponID, weapon);
                }
            }
            ReloadWeaponConfig();
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator14 { finishCallback = finishCallback, progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>finishCallback = finishCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        private static void ReloadWeaponConfig()
        {
            foreach (ConfigWeapon weapon in _weaponIDMap.Values)
            {
                WeaponMetaData data = WeaponMetaDataReader.TryGetWeaponMetaDataByKey(weapon.WeaponID);
                if (data != null)
                {
                    weapon.Attach.PrefabPath = data.bodyMod;
                    weapon.Meta = data;
                }
            }
        }

        public static void WeaponModelAndEffectAttach(int weaponID, string avatarType, BaseMonoAnimatorEntity entity)
        {
            ConfigWeapon weaponConfig = GetWeaponConfig(weaponID);
            Transform weaponProtoTrans = Miscs.LoadResource<GameObject>(weaponConfig.Attach.PrefabPath, BundleType.RESOURCE_FILE).transform;
            WeaponAttach.AttachWeaponMesh(weaponConfig, entity, weaponProtoTrans, avatarType);
            MonoEffectOverrideSetting component = weaponProtoTrans.GetComponent<MonoEffectOverrideSetting>();
            if ((component != null) || (weaponConfig.EffectOverlays != null))
            {
                MonoEffectOverride @override = entity.GetComponent<MonoEffectOverride>();
                if (@override == null)
                {
                    @override = entity.gameObject.AddComponent<MonoEffectOverride>();
                }
                if (component != null)
                {
                    for (int i = 0; i < component.materialOverrides.Length; i++)
                    {
                        MaterialOverrideEntry entry = component.materialOverrides[i];
                        @override.materialOverrides.Add(entry.materialOverrideKey, entry.material);
                    }
                    for (int j = 0; j < component.colorOverrides.Length; j++)
                    {
                        ColorOverrideEntry entry2 = component.colorOverrides[j];
                        @override.colorOverrides.Add(entry2.colorOverrideKey, entry2.color);
                    }
                    for (int k = 0; k < component.floatOverrides.Length; k++)
                    {
                        FloatOverrideEntry entry3 = component.floatOverrides[k];
                        @override.floatOverrides.Add(entry3.floatOverrideKey, entry3.value);
                    }
                }
                if (weaponConfig.EffectOverlays.Length > 0)
                {
                    for (int m = 0; m < weaponConfig.EffectOverlays.Length; m++)
                    {
                        @override.effectOverlays.Add(weaponConfig.EffectOverlays[m].EffectOverrideKey, weaponConfig.EffectOverlays[m].EffectPattern);
                    }
                }
                if (weaponConfig.EffectOverrides.Length > 0)
                {
                    for (int n = 0; n < weaponConfig.EffectOverrides.Length; n++)
                    {
                        @override.effectOverrides.Add(weaponConfig.EffectOverrides[n].EffectOverrideKey, weaponConfig.EffectOverrides[n].EffectPattern);
                    }
                }
                weaponConfig.Attach.GetRuntimeWeaponAttachHandler()(weaponConfig, weaponProtoTrans, entity, avatarType);
            }
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator14 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>finishCallback;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal string[] <$s_956>__1;
            internal int <$s_957>__2;
            internal string[] <$s_958>__5;
            internal int <$s_959>__6;
            internal AsyncAssetRequst <asyncRequest>__8;
            internal string[] <pathes>__0;
            internal float <step>__4;
            internal string <weaponRegistryPath>__3;
            internal string <weaponRegistryPath>__7;
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
                        WeaponData._loadJsonConfigCallback = this.finishCallback;
                        WeaponData._configPathList = new List<string>();
                        WeaponData._weaponIDMap = new Dictionary<int, ConfigWeapon>();
                        this.<pathes>__0 = GlobalDataManager.metaConfig.weaponRegistryPathes;
                        if (this.<pathes>__0.Length != 0)
                        {
                            this.<$s_956>__1 = this.<pathes>__0;
                            this.<$s_957>__2 = 0;
                            while (this.<$s_957>__2 < this.<$s_956>__1.Length)
                            {
                                this.<weaponRegistryPath>__3 = this.<$s_956>__1[this.<$s_957>__2];
                                WeaponData._configPathList.Add(this.<weaponRegistryPath>__3);
                                this.<$s_957>__2++;
                            }
                            this.<step>__4 = this.progressSpan / ((float) this.<pathes>__0.Length);
                            WeaponData._loadDataBackGroundWorker.StartBackGroundWork("WeaponData");
                            this.<$s_958>__5 = this.<pathes>__0;
                            this.<$s_959>__6 = 0;
                            while (this.<$s_959>__6 < this.<$s_958>__5.Length)
                            {
                                this.<weaponRegistryPath>__7 = this.<$s_958>__5[this.<$s_959>__6];
                                this.<asyncRequest>__8 = ConfigUtil.LoadJsonConfigAsync(this.<weaponRegistryPath>__7, BundleType.DATA_FILE);
                                SuperDebug.VeryImportantAssert(this.<asyncRequest>__8 != null, "assetRequest is null weaponRegistryPath :" + this.<weaponRegistryPath>__7);
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
                                ConfigUtil.LoadJSONStrConfigMultiThread<ConfigWeaponRegistry>(this.<asyncRequest>__8.asset.ToString(), WeaponData._loadDataBackGroundWorker, new Action<ConfigWeaponRegistry, string>(WeaponData.OnLoadOneJsonConfigFinish), this.<weaponRegistryPath>__7);
                            Label_01D5:
                                this.<$s_959>__6++;
                            }
                            this.$PC = -1;
                            break;
                        }
                        if (WeaponData._loadJsonConfigCallback != null)
                        {
                            WeaponData._loadJsonConfigCallback("WeaponData");
                            WeaponData._loadJsonConfigCallback = null;
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

