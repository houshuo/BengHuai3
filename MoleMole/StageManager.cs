namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class StageManager
    {
        private StageEntry _activeStageEntry;
        private string[] _localAvatarPredicates = Miscs.EMPTY_STRINGS;
        private MonoBasePerpStage _perpStage;
        private FixedStack<ConfigStageEffectSetting> _stageEffectSettingStack;
        private MonoStageEnv _stageEnv;
        private bool _transitOnChange;

        private StageManager()
        {
        }

        private void AddEntityEffectPredicates(BaseMonoAnimatorEntity entity, string[] predicates)
        {
            for (int i = 0; i < predicates.Length; i++)
            {
                entity.AddAnimEventPredicate(predicates[i]);
            }
        }

        public void ApplyActiveStageEffectSettingAndStartCheckingForChange()
        {
            this.ApplyStageEffectSetting(this._stageEffectSettingStack.value);
            this._transitOnChange = true;
        }

        private void ApplyStageEffectSetting(ConfigStageEffectSetting setting)
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            this.AddEntityEffectPredicates(localAvatar, setting.LocalAvatarEffectPredicates);
            this._localAvatarPredicates = setting.LocalAvatarEffectPredicates;
            if (setting.AvatarColorOverrides.Length > 0)
            {
                List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
                for (int i = 0; i < allPlayerAvatars.Count; i++)
                {
                    BaseMonoAvatar avatar2 = allPlayerAvatars[i];
                    MonoEffectOverride component = avatar2.GetComponent<MonoEffectOverride>();
                    if (component == null)
                    {
                        component = avatar2.gameObject.AddComponent<MonoEffectOverride>();
                    }
                    for (int j = 0; j < setting.AvatarColorOverrides.Length; j++)
                    {
                        ColorOverrideEntry entry = setting.AvatarColorOverrides[j];
                        component.colorOverrides.Add(entry.colorOverrideKey, entry.color);
                    }
                }
            }
        }

        private void CleanForStageTransit()
        {
            Singleton<DynamicObjectManager>.Instance.CleanWhenStageChange();
            Singleton<PropObjectManager>.Instance.CleanWhenStageChange();
            UnityEngine.Object.Destroy(this._stageEnv.gameObject);
        }

        public void Core()
        {
        }

        public void CreateStage(string typeName, List<string> avatarSpawnNameList, string baseWeatherName, bool isContinued = false)
        {
            MonoBasePerpStage component;
            StageEntry stageEntryByName = StageData.GetStageEntryByName(typeName);
            bool isBorn = this._activeStageEntry == null;
            Vector3 zero = Vector3.zero;
            MonoBasePerpStage.ContinueWeatherDataSettings continueData = null;
            if (this._activeStageEntry == null)
            {
                component = ((GameObject) UnityEngine.Object.Instantiate(Miscs.LoadResource(stageEntryByName.GetPerpStagePrefabPath(), BundleType.RESOURCE_FILE))).GetComponent<MonoBasePerpStage>();
                SetPerpstageNodeVisibility(component, stageEntryByName, false, false);
                SetPerpstageNodeVisibility(component, stageEntryByName, true, true);
            }
            else if (this._activeStageEntry.PerpStagePrefabPath != stageEntryByName.PerpStagePrefabPath)
            {
                zero = this._perpStage.transform.position;
                continueData = this._perpStage.GetContinueWeatherDataSetup();
                UnityEngine.Object.DestroyImmediate(this._perpStage.gameObject);
                Resources.UnloadUnusedAssets();
                component = ((GameObject) UnityEngine.Object.Instantiate(Miscs.LoadResource(stageEntryByName.GetPerpStagePrefabPath(), BundleType.RESOURCE_FILE))).GetComponent<MonoBasePerpStage>();
                SetPerpstageNodeVisibility(component, stageEntryByName, false, false);
                SetPerpstageNodeVisibility(component, stageEntryByName, true, true);
            }
            else
            {
                SetPerpstageNodeVisibility(this._perpStage, this._activeStageEntry, false, true);
                zero = -this._perpStage.transform.Find(this._activeStageEntry.LocationPointName).localPosition;
                continueData = this._perpStage.GetContinueWeatherDataSetup();
                component = this._perpStage;
                SetPerpstageNodeVisibility(component, stageEntryByName, false, false);
                SetPerpstageNodeVisibility(component, stageEntryByName, true, true);
            }
            Vector3 offset = this.InitAfterCreateStage(stageEntryByName, component, zero, isBorn, baseWeatherName, continueData, isContinued);
            Singleton<AvatarManager>.Instance.PreloadTeamAvatars();
            Singleton<EventManager>.Instance.FireEvent(new EvtStageCreated(avatarSpawnNameList, isBorn, offset), MPEventDispatchMode.Normal);
        }

        public StageEntry GetActiveStageEntry()
        {
            return this._activeStageEntry;
        }

        public MonoBasePerpStage GetPerpStage()
        {
            return this._perpStage;
        }

        public MonoStageEnv GetStageEnv()
        {
            return this._stageEnv;
        }

        public string GetStageTypeName()
        {
            return this._activeStageEntry.TypeName;
        }

        private Vector3 InitAfterCreateStage(StageEntry stageEntry, MonoBasePerpStage perpStage, Vector3 preStagePos, bool isBorn, string baseWeatherName, MonoBasePerpStage.ContinueWeatherDataSettings continueData, bool isContinued)
        {
            Vector3 zero = Vector3.zero;
            Vector3 vector2 = Vector3.zero;
            if (!string.IsNullOrEmpty(stageEntry.LocationPointName))
            {
                perpStage.transform.position = Vector3.zero;
                perpStage.transform.position = -perpStage.transform.Find(stageEntry.LocationPointName).localPosition;
                vector2 = perpStage.transform.position - preStagePos;
            }
            if (this._activeStageEntry == null)
            {
                perpStage.Init(stageEntry, baseWeatherName);
            }
            else
            {
                float renderingTimer = 0f;
                float weatherTimer = 0f;
                ConfigWeatherData fromWeatherData = null;
                ConfigWeatherData toWeatherData = null;
                string toWeatherName = null;
                if (continueData != null)
                {
                    renderingTimer = continueData.renderingDataContinueTimer;
                    weatherTimer = continueData.weatherContinueTimer;
                    fromWeatherData = continueData.currentWeatherData;
                    toWeatherData = continueData.continueWeatherData;
                    toWeatherName = continueData.continueWeatherName;
                }
                if (this._activeStageEntry.PerpStagePrefabPath != stageEntry.PerpStagePrefabPath)
                {
                    if (isContinued)
                    {
                        if (renderingTimer > 0f)
                        {
                            if (toWeatherName != null)
                            {
                                perpStage.Init(stageEntry, fromWeatherData, toWeatherName, renderingTimer, weatherTimer);
                            }
                            else
                            {
                                perpStage.Init(stageEntry, fromWeatherData, toWeatherData, renderingTimer, weatherTimer);
                            }
                        }
                        else if (toWeatherName != null)
                        {
                            perpStage.Init(stageEntry, toWeatherName);
                        }
                        else
                        {
                            perpStage.Init(stageEntry, toWeatherData);
                        }
                    }
                    else
                    {
                        perpStage.Init(stageEntry, baseWeatherName);
                    }
                }
                else if (isContinued)
                {
                    if (renderingTimer > 0f)
                    {
                        perpStage.TransitWeatherData(toWeatherData, renderingTimer, weatherTimer);
                    }
                    else
                    {
                        perpStage.Reset(stageEntry, toWeatherData);
                    }
                }
                else
                {
                    perpStage.Reset(stageEntry, WeatherData.GetWeatherDataConfig(baseWeatherName));
                }
                this.CleanForStageTransit();
            }
            GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(Miscs.LoadResource(stageEntry.GetEnvPrefabPath(), BundleType.RESOURCE_FILE));
            obj2.transform.position = Vector3.zero;
            obj2.transform.rotation = Quaternion.identity;
            MonoStageEnv component = obj2.GetComponent<MonoStageEnv>();
            Vector3 vector3 = new Vector3(0f, -0.05f, 0f);
            Transform transform = Miscs.FindFirstChildGivenLayerAndCollider(obj2.transform, LayerMask.NameToLayer("StageCollider"));
            if (transform != null)
            {
                transform.position += vector3;
            }
            this.RegisterStage(stageEntry, perpStage, component);
            return vector2;
        }

        public void InitAtAwake()
        {
            this.InitStageEffectSettings();
        }

        public void InitAtStart()
        {
        }

        public void InitStageEffectSettings()
        {
            AvatarManager instance = Singleton<AvatarManager>.Instance;
            instance.onLocalAvatarChanged = (Action<BaseMonoAvatar, BaseMonoAvatar>) Delegate.Combine(instance.onLocalAvatarChanged, new Action<BaseMonoAvatar, BaseMonoAvatar>(this.OnLocalAvatarChanged));
            this._stageEffectSettingStack = new FixedStack<ConfigStageEffectSetting>(7, new Action<ConfigStageEffectSetting, int, ConfigStageEffectSetting, int>(this.OnStageEffectSettingChanged));
            this._stageEffectSettingStack.Push(ConfigStageEffectSetting.EMPTY, true);
        }

        private void OnLocalAvatarChanged(BaseMonoAvatar from, BaseMonoAvatar to)
        {
            this.RemoveEntityEffectPredicates(from, this._localAvatarPredicates);
            this.AddEntityEffectPredicates(to, this._localAvatarPredicates);
        }

        public void OnStageEffectSettingChanged(ConfigStageEffectSetting fromSetting, int oldIx, ConfigStageEffectSetting toSetting, int newIx)
        {
            if (this._transitOnChange)
            {
                this.UnApplyStageEffectSettings(fromSetting);
                this.ApplyStageEffectSetting(toSetting);
            }
        }

        public void PopStageSettingData(int ix)
        {
            this._stageEffectSettingStack.Pop(ix);
        }

        public int PushStageSettingData(ConfigStageEffectSetting setting)
        {
            return this._stageEffectSettingStack.Push(setting, false);
        }

        public void RegisterStage(StageEntry stageEntry, MonoBasePerpStage perpStage, MonoStageEnv stageEnv)
        {
            this._activeStageEntry = stageEntry;
            this._perpStage = perpStage;
            this._stageEnv = stageEnv;
        }

        private void RemoveEntityEffectPredicates(BaseMonoAnimatorEntity entity, string[] predicates)
        {
            for (int i = 0; i < predicates.Length; i++)
            {
                entity.RemoveAnimEventPredicate(predicates[i]);
            }
        }

        public void SetBaseStageEffectSetting(ConfigStageEffectSetting setting)
        {
            if (setting != null)
            {
                this._stageEffectSettingStack.Set(0, setting, false);
            }
        }

        public static void SetPerpstageNodeVisibility(MonoBasePerpStage perpStage, StageEntry stageEntry, bool useShowNodes, bool visible)
        {
            if (!useShowNodes)
            {
                if (!string.IsNullOrEmpty(stageEntry.HideNodeNames))
                {
                    SetPerpstageNodeVisibilityByNode(perpStage, stageEntry.HideNodeNames, visible);
                }
                if (!string.IsNullOrEmpty(stageEntry.HideNodePrefabPaths))
                {
                    SetPerpstageNodeVisibilityByPrefab(perpStage, stageEntry.HideNodePrefabPaths, visible);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(stageEntry.ShowNodeNames))
                {
                    SetPerpstageNodeVisibilityByNode(perpStage, stageEntry.ShowNodeNames, visible);
                }
                if (!string.IsNullOrEmpty(stageEntry.ShowNodePrefabPaths))
                {
                    SetPerpstageNodeVisibilityByPrefab(perpStage, stageEntry.ShowNodePrefabPaths, visible);
                }
            }
        }

        public static void SetPerpstageNodeVisibilityByNode(MonoBasePerpStage perpStage, string names, bool visible)
        {
            if (!string.IsNullOrEmpty(names))
            {
                char[] separator = new char[] { ';' };
                string[] strArray = names.Split(separator);
                for (int i = 0; i < strArray.Length; i++)
                {
                    perpStage.transform.Find(strArray[i]).gameObject.SetActive(visible);
                }
                perpStage.transform.gameObject.SetActive(true);
            }
        }

        public static void SetPerpstageNodeVisibilityByPrefab(MonoBasePerpStage perpStage, string prefabPathes, bool visible)
        {
            if (!string.IsNullOrEmpty(prefabPathes))
            {
                char[] separator = new char[] { ';' };
                string[] strArray = prefabPathes.Split(separator);
                for (int i = 0; i < strArray.Length; i++)
                {
                    char[] chArray2 = new char[] { '/' };
                    string[] strArray2 = strArray[i].Split(chArray2);
                    string name = strArray2[strArray2.Length - 1];
                    if (!visible)
                    {
                        Transform transform = perpStage.transform.Find(name);
                        if (transform != null)
                        {
                            transform.gameObject.SetActive(visible);
                        }
                    }
                    else
                    {
                        Transform transform2 = perpStage.transform.Find(name);
                        if (transform2 != null)
                        {
                            transform2.gameObject.SetActive(visible);
                        }
                        else
                        {
                            GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(Miscs.LoadResource(strArray[i], BundleType.RESOURCE_FILE));
                            obj2.name = name;
                            obj2.transform.SetParent(perpStage.transform, false);
                        }
                    }
                }
                perpStage.transform.gameObject.SetActive(true);
            }
        }

        private void UnApplyStageEffectSettings(ConfigStageEffectSetting setting)
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            this.RemoveEntityEffectPredicates(localAvatar, setting.LocalAvatarEffectPredicates);
            this._localAvatarPredicates = Miscs.EMPTY_STRINGS;
            List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
            for (int i = 0; i < allPlayerAvatars.Count; i++)
            {
                MonoEffectOverride component = allPlayerAvatars[i].GetComponent<MonoEffectOverride>();
                if (component != null)
                {
                    for (int j = 0; j < setting.AvatarColorOverrides.Length; j++)
                    {
                        ColorOverrideEntry entry = setting.AvatarColorOverrides[j];
                        component.colorOverrides.Remove(entry.colorOverrideKey);
                    }
                }
            }
        }
    }
}

