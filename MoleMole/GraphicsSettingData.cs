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

    public static class GraphicsSettingData
    {
        private static bool _enableGyroscope;
        private static bool _hasGetGyroscope;
        private static bool _hasSettingGrade = false;
        private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();
        private static Action<string> _loadJsonConfigCallback = null;
        private static GraphicsRecommendGrade _recommendGrade;
        private static ConfigGraphicsRecommendSetting _recommendSetting;
        private static Dictionary<string, ConfigOverrideGroup> _recommendSettingGroupMap;
        private static string _recommendSettingName = "Default";
        private static ConfigOverrideGroup _recommendVolatileSettingGroup;

        public static void ApplyPersonalContrastDelta()
        {
            if (Singleton<MiHoYoGameData>.Instance != null)
            {
                GraphicsSettingUtil.SetPostFXContrast(Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.ContrastDelta);
            }
        }

        public static void ApplySettingConfig()
        {
            if (Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode)
            {
                ApplySettingConfig(GetGraphicsEcoModeConfig());
            }
            else if (Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsUserDefinedGrade || Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsUserDefinedVolatile)
            {
                ApplySettingConfig(GetGraphicsPersonalSettingConfig(Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting));
            }
            else
            {
                ApplySettingConfig(GetGraphicsRecommendCompleteConfig());
            }
            ApplyPersonalContrastDelta();
        }

        public static void ApplySettingConfig(ConfigGraphicsSetting setting)
        {
            GraphicsSettingUtil.SetTargetFrameRate(setting.TargetFrameRate);
            bool forceWhenDisable = true;
            GraphicsSettingUtil.EnablePostFX(setting.VolatileSetting.UsePostFX, forceWhenDisable);
            GraphicsSettingUtil.ApplyResolution(setting.ResolutionPercentage, setting.ResolutionQuality, setting.RecommendResolutionX, setting.RecommendResolutionY);
            GraphicsSettingUtil.SetPostEffectBufferSizeByQuality(setting.PostFxGradeBufferSize, setting.VolatileSetting.PostFXGrade);
            GraphicsSettingUtil.EnableHDR(setting.VolatileSetting.UseHDR);
            GraphicsSettingUtil.EnableDistortion(setting.VolatileSetting.UseDistortion);
            GraphicsSettingUtil.EnableReflection(setting.VolatileSetting.UseReflection);
            GraphicsSettingUtil.EnableFXAA(setting.VolatileSetting.UseFXAA);
            GraphicsSettingUtil.EnableDynamicBone(setting.VolatileSetting.UseDynamicBone);
            GraphicsSettingUtil.EnableStaticCloudMode(!setting.VolatileSetting.UseDynamicBone);
        }

        private static void CopyGraphicsVolatileConfig(ConfigGraphicsVolatileSetting from, ref ConfigGraphicsVolatileSetting to)
        {
            to.PostFXGrade = from.PostFXGrade;
            to.UsePostFX = from.UsePostFX;
            to.UseDistortion = from.UseDistortion;
            to.UseDynamicBone = from.UseDynamicBone;
            to.UseFXAA = from.UseFXAA;
            to.UseHDR = from.UseHDR;
            to.UseReflection = from.UseReflection;
        }

        public static void CopyPersonalContrastDelta(ref float contrastDelta)
        {
            contrastDelta = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.ContrastDelta;
        }

        public static void CopyPersonalGraphicsConfig(ref ConfigGraphicsPersonalSetting to)
        {
            CopyPersonalGraphicsConfig(Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode, ref to);
        }

        public static void CopyPersonalGraphicsConfig(bool isEcoMode, ref ConfigGraphicsPersonalSetting to)
        {
            ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
            if (isEcoMode)
            {
                CopyToPersonalGraphicsConfig(GetGraphicsEcoModeConfig(), ref to);
                to.IsUserDefinedGrade = personalGraphicsSetting.IsUserDefinedGrade;
                to.IsUserDefinedVolatile = personalGraphicsSetting.IsUserDefinedVolatile;
            }
            else if (personalGraphicsSetting.IsUserDefinedGrade && personalGraphicsSetting.IsUserDefinedVolatile)
            {
                UnityEngine.Debug.LogError("IsUserDefinedGrade and IsUserDefinedVolatile both true");
            }
            else if (!personalGraphicsSetting.IsUserDefinedGrade && !personalGraphicsSetting.IsUserDefinedVolatile)
            {
                CopyToPersonalGraphicsConfig(GetGraphicsRecommendCompleteConfig(), ref to);
                to.IsUserDefinedGrade = false;
                to.IsUserDefinedVolatile = false;
            }
            else if (personalGraphicsSetting.IsUserDefinedGrade)
            {
                CopyToPersonalGraphicsConfig(GetGraphicsRecommendCompleteConfig(personalGraphicsSetting.RecommendGrade), ref to);
                to.RecommendGrade = personalGraphicsSetting.RecommendGrade;
                to.IsUserDefinedGrade = true;
                to.IsUserDefinedVolatile = false;
            }
            else
            {
                ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig();
                to.PostFxGradeBufferSize = graphicsRecommendConfig.PostFxGradeBufferSize;
                to.RecommendResolutionX = graphicsRecommendConfig.RecommendResolutionX;
                to.RecommendResolutionY = graphicsRecommendConfig.RecommendResolutionY;
                to.ResolutionPercentage = graphicsRecommendConfig.ResolutionPercentage;
                to.ResolutionQuality = personalGraphicsSetting.ResolutionQuality;
                to.TargetFrameRate = personalGraphicsSetting.TargetFrameRate;
                to.ContrastDelta = 0f;
                to.VolatileSetting = new ConfigGraphicsVolatileSetting();
                CopyGraphicsVolatileConfig(personalGraphicsSetting.VolatileSetting, ref to.VolatileSetting);
                to.RecommendGrade = personalGraphicsSetting.RecommendGrade;
                to.IsUserDefinedGrade = false;
                to.IsUserDefinedVolatile = true;
            }
            to.IsEcoMode = isEcoMode;
        }

        public static void CopyToPersonalGraphicsConfig(ConfigGraphicsSetting setting, ref ConfigGraphicsPersonalSetting to)
        {
            ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig();
            to.PostFxGradeBufferSize = graphicsRecommendConfig.PostFxGradeBufferSize;
            to.RecommendResolutionX = graphicsRecommendConfig.RecommendResolutionX;
            to.RecommendResolutionY = graphicsRecommendConfig.RecommendResolutionY;
            to.ResolutionPercentage = graphicsRecommendConfig.ResolutionPercentage;
            to.ResolutionQuality = setting.ResolutionQuality;
            to.TargetFrameRate = setting.TargetFrameRate;
            to.ContrastDelta = setting.ContrastDelta;
            to.VolatileSetting = new ConfigGraphicsVolatileSetting();
            CopyGraphicsVolatileConfig(setting.VolatileSetting, ref to.VolatileSetting);
        }

        private static string GetGraphicsDeviceName()
        {
            for (int i = 0; i < SystemInfo.graphicsDeviceName.Length; i++)
            {
                if (!char.IsLetter(SystemInfo.graphicsDeviceName[i]))
                {
                    return SystemInfo.graphicsDeviceName.Substring(0, i);
                }
            }
            return SystemInfo.graphicsDeviceName;
        }

        public static ConfigGraphicsSetting GetGraphicsEcoModeConfig()
        {
            return GetGraphicsEcoModeConfig(GetTargetPlatform());
        }

        private static ConfigGraphicsSetting GetGraphicsEcoModeConfig(string platformName)
        {
            if (!_recommendSettingGroupMap.ContainsKey(platformName))
            {
                return null;
            }
            bool flag = false;
            ConfigGraphicsRecommendSetting config = _recommendSettingGroupMap[platformName].GetConfig<ConfigGraphicsRecommendSetting>("EcoMode");
            if (config == null)
            {
                flag = true;
                config = (ConfigGraphicsRecommendSetting) _recommendSettingGroupMap[platformName].Default;
            }
            ConfigGraphicsSetting setting2 = new ConfigGraphicsSetting {
                PostFxGradeBufferSize = config.PostFxGradeBufferSize,
                RecommendResolutionX = config.RecommendResolutionX,
                RecommendResolutionY = config.RecommendResolutionY,
                ResolutionPercentage = config.ResolutionPercentage,
                ResolutionQuality = !flag ? config.ResolutionQuality : ResolutionQualityGrade.Low,
                TargetFrameRate = !flag ? config.TargetFrameRate : 30,
                ContrastDelta = 0f
            };
            ConfigGraphicsVolatileSetting from = _recommendVolatileSettingGroup.GetConfig<ConfigGraphicsVolatileSetting>((!flag ? config.RecommendGrade : GraphicsRecommendGrade.Off).ToString());
            setting2.VolatileSetting = new ConfigGraphicsVolatileSetting();
            CopyGraphicsVolatileConfig(from, ref setting2.VolatileSetting);
            GraphicsRecommendGrade graphicsRecommendGrade = GetGraphicsRecommendGrade();
            setting2.VolatileSetting.UsePostFX = graphicsRecommendGrade >= GraphicsRecommendGrade.High;
            setting2.VolatileSetting.UseHDR = graphicsRecommendGrade >= GraphicsRecommendGrade.High;
            ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
            if (personalGraphicsSetting.IsUserDefinedVolatile && !personalGraphicsSetting.VolatileSetting.UsePostFX)
            {
                setting2.VolatileSetting.UsePostFX = false;
            }
            if (personalGraphicsSetting.IsUserDefinedVolatile && !personalGraphicsSetting.VolatileSetting.UseHDR)
            {
                setting2.VolatileSetting.UseHDR = false;
            }
            return setting2;
        }

        public static ConfigGraphicsSetting GetGraphicsPersonalSettingConfig(ConfigGraphicsPersonalSetting personalSetting)
        {
            if (personalSetting.IsUserDefinedGrade && personalSetting.IsUserDefinedVolatile)
            {
                UnityEngine.Debug.LogError("IsUserDefinedGrade and IsUserDefinedVolatile both true");
                return null;
            }
            if (!personalSetting.IsUserDefinedGrade && !personalSetting.IsUserDefinedVolatile)
            {
                return GetGraphicsRecommendCompleteConfig();
            }
            if (personalSetting.IsUserDefinedGrade)
            {
                return GetGraphicsRecommendCompleteConfig(personalSetting.RecommendGrade);
            }
            ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig();
            personalSetting.PostFxGradeBufferSize = graphicsRecommendConfig.PostFxGradeBufferSize;
            personalSetting.RecommendResolutionX = graphicsRecommendConfig.RecommendResolutionX;
            personalSetting.RecommendResolutionY = graphicsRecommendConfig.RecommendResolutionY;
            personalSetting.ResolutionPercentage = graphicsRecommendConfig.ResolutionPercentage;
            return personalSetting;
        }

        public static ConfigGraphicsSetting GetGraphicsRecommendCompleteConfig()
        {
            return GetGraphicsRecommendCompleteConfig(GetTargetPlatform(), SystemInfo.deviceModel);
        }

        public static ConfigGraphicsSetting GetGraphicsRecommendCompleteConfig(GraphicsRecommendGrade grade)
        {
            return GetGraphicsRecommendCompleteConfig(GetTargetPlatform(), SystemInfo.deviceModel, grade);
        }

        private static ConfigGraphicsSetting GetGraphicsRecommendCompleteConfig(string platformName, string deviceModel)
        {
            if (!_recommendSettingGroupMap.ContainsKey(platformName))
            {
                return null;
            }
            ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig(platformName, deviceModel);
            ConfigGraphicsSetting setting2 = new ConfigGraphicsSetting {
                PostFxGradeBufferSize = graphicsRecommendConfig.PostFxGradeBufferSize,
                RecommendResolutionX = graphicsRecommendConfig.RecommendResolutionX,
                RecommendResolutionY = graphicsRecommendConfig.RecommendResolutionY,
                ResolutionPercentage = graphicsRecommendConfig.ResolutionPercentage,
                ResolutionQuality = graphicsRecommendConfig.ResolutionQuality,
                TargetFrameRate = graphicsRecommendConfig.TargetFrameRate,
                ContrastDelta = 0f
            };
            ConfigGraphicsVolatileSetting config = _recommendVolatileSettingGroup.GetConfig<ConfigGraphicsVolatileSetting>(graphicsRecommendConfig.RecommendGrade.ToString());
            setting2.VolatileSetting = new ConfigGraphicsVolatileSetting();
            CopyGraphicsVolatileConfig(config, ref setting2.VolatileSetting);
            return setting2;
        }

        private static ConfigGraphicsSetting GetGraphicsRecommendCompleteConfig(string platformName, string deviceModel, GraphicsRecommendGrade grade)
        {
            if (!_recommendSettingGroupMap.ContainsKey(platformName))
            {
                return null;
            }
            ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig(platformName, deviceModel);
            ConfigGraphicsSetting setting2 = new ConfigGraphicsSetting {
                PostFxGradeBufferSize = graphicsRecommendConfig.PostFxGradeBufferSize,
                RecommendResolutionX = graphicsRecommendConfig.RecommendResolutionX,
                RecommendResolutionY = graphicsRecommendConfig.RecommendResolutionY,
                ResolutionPercentage = graphicsRecommendConfig.ResolutionPercentage,
                ResolutionQuality = graphicsRecommendConfig.ResolutionQuality,
                TargetFrameRate = graphicsRecommendConfig.TargetFrameRate,
                ContrastDelta = 0f
            };
            ConfigGraphicsVolatileSetting config = _recommendVolatileSettingGroup.GetConfig<ConfigGraphicsVolatileSetting>(grade.ToString());
            setting2.VolatileSetting = new ConfigGraphicsVolatileSetting();
            CopyGraphicsVolatileConfig(config, ref setting2.VolatileSetting);
            return setting2;
        }

        private static ConfigGraphicsRecommendSetting GetGraphicsRecommendConfig()
        {
            return GetGraphicsRecommendConfig(GetTargetPlatform(), SystemInfo.deviceModel);
        }

        private static ConfigGraphicsRecommendSetting GetGraphicsRecommendConfig(string platformName, string deviceModel)
        {
            if (!_recommendSettingGroupMap.ContainsKey(platformName))
            {
                return null;
            }
            if ((platformName == "PC") || (platformName == "IOS"))
            {
                _recommendSettingName = deviceModel;
                return _recommendSettingGroupMap[platformName].GetConfig<ConfigGraphicsRecommendSetting>(deviceModel);
            }
            if (_recommendSetting == null)
            {
                ConfigOverrideGroup group = _recommendSettingGroupMap[platformName];
                if ((group.Overrides != null) && (group.Overrides.Count > 0))
                {
                    string graphicsDeviceName = GetGraphicsDeviceName();
                    string[] names = Enum.GetNames(typeof(GraphicsRecommendGrade));
                    for (int i = names.Length - 1; i >= 0; i--)
                    {
                        string key = graphicsDeviceName + " " + names[i];
                        if (group.Overrides.ContainsKey(key))
                        {
                            ConfigGraphicsRecommendSetting setting = (ConfigGraphicsRecommendSetting) group.Overrides[key];
                            if (setting.MatchRequirements())
                            {
                                _recommendSetting = setting;
                                _recommendSettingName = key;
                                return _recommendSetting;
                            }
                        }
                    }
                    for (int j = names.Length - 1; j >= 0; j--)
                    {
                        string str3 = names[j];
                        if (group.Overrides.ContainsKey(str3))
                        {
                            ConfigGraphicsRecommendSetting setting2 = (ConfigGraphicsRecommendSetting) group.Overrides[str3];
                            if (setting2.MatchRequirements())
                            {
                                _recommendSetting = setting2;
                                _recommendSettingName = str3;
                                return _recommendSetting;
                            }
                        }
                    }
                }
                _recommendSetting = (ConfigGraphicsRecommendSetting) group.Default;
                _recommendSettingName = "Default";
            }
            return _recommendSetting;
        }

        public static GraphicsRecommendGrade GetGraphicsRecommendGrade()
        {
            if (!_hasSettingGrade)
            {
                ConfigGraphicsRecommendSetting graphicsRecommendConfig = GetGraphicsRecommendConfig();
                _hasSettingGrade = true;
                _recommendGrade = graphicsRecommendConfig.RecommendGrade;
            }
            return _recommendGrade;
        }

        public static string GetGraphicsRecommendSettingName()
        {
            return _recommendSettingName;
        }

        private static string GetTargetPlatform()
        {
            switch (SystemInfo.deviceType)
            {
                case DeviceType.Unknown:
                    UnityEngine.Debug.LogWarning("unknown device type!");
                    return string.Empty;

                case DeviceType.Handheld:
                {
                    string deviceModel = SystemInfo.deviceModel;
                    if ((!deviceModel.StartsWith("iPhone") && !deviceModel.StartsWith("iPod")) && !deviceModel.StartsWith("iPad"))
                    {
                        return "Android";
                    }
                    return "IOS";
                }
                case DeviceType.Desktop:
                    return "PC";

                case DeviceType.Console:
                    UnityEngine.Debug.LogWarning("device type is Console, we do not know how to set graphics!");
                    return string.Empty;
            }
            return string.Empty;
        }

        public static bool IsEnableGyroscope()
        {
            if (!_hasGetGyroscope)
            {
                string targetPlatform = GetTargetPlatform();
                if (!_recommendSettingGroupMap.ContainsKey(targetPlatform))
                {
                    _hasGetGyroscope = true;
                    _enableGyroscope = true;
                    return _enableGyroscope;
                }
                ConfigGraphicsRecommendSetting config = _recommendSettingGroupMap[targetPlatform].GetConfig<ConfigGraphicsRecommendSetting>("Gyroscope");
                if (config == null)
                {
                    _hasGetGyroscope = true;
                    config = (ConfigGraphicsRecommendSetting) _recommendSettingGroupMap[targetPlatform].Default;
                    return config.EnableGyroscope;
                }
                _hasGetGyroscope = true;
                List<string> excludeDeviceModels = config.ExcludeDeviceModels;
                string deviceModel = SystemInfo.deviceModel;
                foreach (string str3 in excludeDeviceModels)
                {
                    if (deviceModel.ToLower() == str3.ToLower())
                    {
                        _enableGyroscope = false;
                        return _enableGyroscope;
                    }
                }
                _enableGyroscope = true;
            }
            return _enableGyroscope;
        }

        public static bool IsEqualToPersonalConfigIgnoreContrast(ConfigGraphicsPersonalSetting to)
        {
            ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
            if (personalGraphicsSetting.IsEcoMode != to.IsEcoMode)
            {
                return false;
            }
            return ((personalGraphicsSetting.IsEcoMode && to.IsEcoMode) || ((((!personalGraphicsSetting.IsUserDefinedGrade && !to.IsUserDefinedGrade) && (!personalGraphicsSetting.IsUserDefinedVolatile && !to.IsUserDefinedVolatile)) || ((personalGraphicsSetting.IsUserDefinedGrade && to.IsUserDefinedGrade) && (personalGraphicsSetting.RecommendGrade == to.RecommendGrade))) || (((((personalGraphicsSetting.IsUserDefinedVolatile && to.IsUserDefinedVolatile) && ((personalGraphicsSetting.ResolutionQuality == to.ResolutionQuality) && (personalGraphicsSetting.TargetFrameRate == to.TargetFrameRate))) && (((personalGraphicsSetting.VolatileSetting.PostFXGrade == to.VolatileSetting.PostFXGrade) && (personalGraphicsSetting.VolatileSetting.UseDistortion == to.VolatileSetting.UseDistortion)) && ((personalGraphicsSetting.VolatileSetting.UseDynamicBone == to.VolatileSetting.UseDynamicBone) && (personalGraphicsSetting.VolatileSetting.UseFXAA == to.VolatileSetting.UseFXAA)))) && ((personalGraphicsSetting.VolatileSetting.UseHDR == to.VolatileSetting.UseHDR) && (personalGraphicsSetting.VolatileSetting.UsePostFX == to.VolatileSetting.UsePostFX))) && (personalGraphicsSetting.VolatileSetting.UseReflection == to.VolatileSetting.UseReflection))));
        }

        public static bool IsEqualToPersonalContrastDelta(float contrastDelta)
        {
            return (contrastDelta == Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.ContrastDelta);
        }

        private static void OnLoadOneJsonConfigFinish(ConfigOverrideGroup configGroup, string configPath)
        {
            char[] separator = new char[] { '/' };
            string[] strArray = configPath.Split(separator);
            string key = strArray[strArray.Length - 1];
            _recommendSettingGroupMap.Add(key, configGroup);
            _loadDataBackGroundWorker.StopBackGroundWork(false);
            _recommendSetting = null;
            _hasSettingGrade = false;
            _hasGetGyroscope = false;
            if (_loadJsonConfigCallback != null)
            {
                _loadJsonConfigCallback("GraphicsSettingData");
                _loadJsonConfigCallback = null;
            }
        }

        public static void ReloadFromFile()
        {
            _recommendVolatileSettingGroup = ConfigUtil.LoadJSONConfig<ConfigOverrideGroup>(GlobalDataManager.metaConfig.graphicsVolatileSettingRegistryPath);
            _recommendSettingGroupMap = new Dictionary<string, ConfigOverrideGroup>();
            string[] graphicsSettingRegistryPathes = GlobalDataManager.metaConfig.graphicsSettingRegistryPathes;
            string targetPlatform = GetTargetPlatform();
            if (targetPlatform != string.Empty)
            {
                foreach (string str2 in graphicsSettingRegistryPathes)
                {
                    if (str2.Contains(targetPlatform))
                    {
                        ConfigOverrideGroup group = ConfigUtil.LoadJSONConfig<ConfigOverrideGroup>(str2);
                        char[] separator = new char[] { '/' };
                        string[] strArray3 = str2.Split(separator);
                        string key = strArray3[strArray3.Length - 1];
                        _recommendSettingGroupMap.Add(key, group);
                    }
                }
                _recommendSetting = null;
                _hasSettingGrade = false;
                _hasGetGyroscope = false;
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator16 { finishCallback = finishCallback, progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>finishCallback = finishCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        public static void SavePersonalConfigIgnoreContrast(ConfigGraphicsPersonalSetting settingConfig)
        {
            if (!Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode || !settingConfig.IsEcoMode)
            {
                if (Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode != settingConfig.IsEcoMode)
                {
                    Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode = settingConfig.IsEcoMode;
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EcoModeVisible, settingConfig.IsEcoMode));
                }
                if (settingConfig.IsEcoMode)
                {
                    Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
                    ApplySettingConfig();
                }
                else if (settingConfig.IsUserDefinedVolatile)
                {
                    SavePersonalConfigIgnoreContrast((ConfigGraphicsSetting) settingConfig);
                }
                else if (settingConfig.IsUserDefinedGrade)
                {
                    SavePersonalConfigIgnoreContrast(settingConfig.RecommendGrade);
                }
                else
                {
                    Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
                    ApplySettingConfig();
                }
            }
        }

        private static void SavePersonalConfigIgnoreContrast(ConfigGraphicsSetting settingConfig)
        {
            ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
            personalGraphicsSetting.ResolutionQuality = settingConfig.ResolutionQuality;
            personalGraphicsSetting.TargetFrameRate = settingConfig.TargetFrameRate;
            personalGraphicsSetting.VolatileSetting = new ConfigGraphicsVolatileSetting();
            CopyGraphicsVolatileConfig(settingConfig.VolatileSetting, ref personalGraphicsSetting.VolatileSetting);
            personalGraphicsSetting.IsUserDefinedGrade = false;
            personalGraphicsSetting.IsUserDefinedVolatile = true;
            Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
            ApplySettingConfig();
        }

        private static void SavePersonalConfigIgnoreContrast(GraphicsRecommendGrade grade)
        {
            ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
            personalGraphicsSetting.RecommendGrade = grade;
            personalGraphicsSetting.IsUserDefinedGrade = true;
            personalGraphicsSetting.IsUserDefinedVolatile = false;
            Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
            ApplySettingConfig();
        }

        public static void SavePersonalContrastDelta(float contrastDelta)
        {
            Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.ContrastDelta = contrastDelta;
            Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
            ApplyPersonalContrastDelta();
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator16 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>finishCallback;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal string[] <$s_976>__4;
            internal int <$s_977>__5;
            internal AsyncAssetRequst <asyncRequest>__0;
            internal string <configFilePath>__6;
            internal string[] <graphicsSettingRegistryPathes>__1;
            internal float <step>__2;
            internal string <targetPlatform>__3;
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
                        GraphicsSettingData._loadJsonConfigCallback = this.finishCallback;
                        this.<asyncRequest>__0 = ConfigUtil.LoadJsonConfigAsync(GlobalDataManager.metaConfig.graphicsVolatileSettingRegistryPath, BundleType.DATA_FILE);
                        SuperDebug.VeryImportantAssert(this.<asyncRequest>__0 != null, "assetRequest is null graphicsVolatileSettingRegistryPath :" + GlobalDataManager.metaConfig.graphicsVolatileSettingRegistryPath);
                        if (this.<asyncRequest>__0 != null)
                        {
                            this.$current = this.<asyncRequest>__0.operation;
                            this.$PC = 1;
                            goto Label_0233;
                        }
                        goto Label_0231;

                    case 1:
                        GraphicsSettingData._recommendVolatileSettingGroup = ConfigUtil.LoadJSONStrConfig<ConfigOverrideGroup>(this.<asyncRequest>__0.asset.ToString());
                        GraphicsSettingData._recommendSettingGroupMap = new Dictionary<string, ConfigOverrideGroup>();
                        this.<graphicsSettingRegistryPathes>__1 = GlobalDataManager.metaConfig.graphicsSettingRegistryPathes;
                        this.<step>__2 = this.progressSpan / ((float) this.<graphicsSettingRegistryPathes>__1.Length);
                        this.<targetPlatform>__3 = GraphicsSettingData.GetTargetPlatform();
                        if (!(this.<targetPlatform>__3 == string.Empty))
                        {
                            GraphicsSettingData._loadDataBackGroundWorker.StartBackGroundWork("GraphicsSettingData");
                            this.<$s_976>__4 = this.<graphicsSettingRegistryPathes>__1;
                            this.<$s_977>__5 = 0;
                            while (this.<$s_977>__5 < this.<$s_976>__4.Length)
                            {
                                this.<configFilePath>__6 = this.<$s_976>__4[this.<$s_977>__5];
                                if (!this.<configFilePath>__6.Contains(this.<targetPlatform>__3))
                                {
                                    goto Label_0209;
                                }
                                this.<asyncRequest>__0 = ConfigUtil.LoadJsonConfigAsync(this.<configFilePath>__6, BundleType.DATA_FILE);
                                SuperDebug.VeryImportantAssert(this.<asyncRequest>__0 != null, "assetRequest is null graphicsPath :" + this.<configFilePath>__6);
                                if (this.<asyncRequest>__0 == null)
                                {
                                    goto Label_0231;
                                }
                                this.$current = this.<asyncRequest>__0.operation;
                                this.$PC = 2;
                                goto Label_0233;
                            Label_01BC:
                                if (this.moveOneStepCallback != null)
                                {
                                    this.moveOneStepCallback(this.<step>__2);
                                }
                                ConfigUtil.LoadJSONStrConfigMultiThread<ConfigOverrideGroup>(this.<asyncRequest>__0.asset.ToString(), GraphicsSettingData._loadDataBackGroundWorker, new Action<ConfigOverrideGroup, string>(GraphicsSettingData.OnLoadOneJsonConfigFinish), this.<configFilePath>__6);
                                break;
                            Label_0209:
                                this.<$s_977>__5++;
                            }
                            break;
                        }
                        goto Label_0231;

                    case 2:
                        goto Label_01BC;

                    default:
                        goto Label_0231;
                }
                this.$PC = -1;
            Label_0231:
                return false;
            Label_0233:
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
    }
}

