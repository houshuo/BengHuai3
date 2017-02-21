namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoBasePerpStage : MonoBehaviour
    {
        private string _currentBaseWeatherName;
        private ConfigStageRenderingData _currentRendertingData;
        private ConfigSubWeatherCollection _currentSubWeather;
        private ConfigStageRenderingData _initBaseRenderingData;
        private ConfigSubWeatherCollection _initBaseSubWeather;
        private bool _isInMiddleRendering;
        private bool _isInMiddleWeather;
        private Transform _mainCameraTransform;
        private int _middleRenderingIndex;
        private int _middleSubWeatherIndex;
        private GameObject _rainObject;
        private GameObject _rainPrefab;
        private FixedStack<RenderingDataTransition> _renderingDataStack;
        private RenderingDataState _renderingDataState;
        private EntityTimer _renderingDataTransitionTimer;
        protected static string _stageDataPath = "Stage/Data/";
        private FixedStack<SubWeatherTransition> _subWeatherStack;
        private ConfigSubWeatherCollection _transitTargetSubWeather;
        private WeatherState _weatherState;
        private EntityTimer _weatherTransitionTimer;
        private const int BASE_RENDERING_DATANAME_IX = 0;
        private const int BASE_SUB_WEATHER_IX = 0;
        public MonoLightShadowManager lightMapCorrectManager;
        public MonoLightProbManager lightProbManager;
        private readonly string RAIN_PREFAB_PATH = "Effect/Weather/Rain/Rain";
        [NonSerialized, HideInInspector]
        public RainController rainController;
        public Transform windZone;

        private void AuxTimeScaleCallback(float oldValue, int oldIx, float newValue, int newIx)
        {
            if (this.rainController != null)
            {
                if (newValue < 1f)
                {
                    this.rainController.EnterSlowMode(oldValue);
                }
                else
                {
                    this.rainController.LeaveSlowMode();
                }
            }
        }

        public virtual void Awake()
        {
            this._renderingDataStack = new FixedStack<RenderingDataTransition>(10, new Action<RenderingDataTransition, int, RenderingDataTransition, int>(this.OnRenderingDataChanged));
            this._currentRendertingData = null;
            this._renderingDataTransitionTimer = new EntityTimer(0f);
            this._renderingDataTransitionTimer.SetActive(false);
            this._renderingDataState = RenderingDataState.Idle;
            this._mainCameraTransform = Camera.main.transform;
            this._subWeatherStack = new FixedStack<SubWeatherTransition>(5, new Action<SubWeatherTransition, int, SubWeatherTransition, int>(this.OnSubWeatherChanged));
            this._rainPrefab = Miscs.LoadResource<GameObject>(this.RAIN_PREFAB_PATH, BundleType.RESOURCE_FILE);
            this._weatherState = WeatherState.Idle;
            this._weatherTransitionTimer = new EntityTimer(0f);
            this._weatherTransitionTimer.SetActive(false);
        }

        private Material[] CollectAndAssignInstancedMaterials()
        {
            Dictionary<Material, Material> dictionary = new Dictionary<Material, Material>();
            Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
            int num = 1;
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Renderer renderer = componentsInChildren[i];
                Material[] sharedMaterials = renderer.sharedMaterials;
                Material[] materialArray2 = new Material[sharedMaterials.Length];
                for (int j = 0; j < sharedMaterials.Length; j++)
                {
                    Material key = sharedMaterials[j];
                    if (!dictionary.ContainsKey(key))
                    {
                        Material material2 = new Material(key) {
                            renderQueue = key.renderQueue,
                            name = string.Format("{0} #{1}", key.name, num++)
                        };
                        dictionary.Add(key, material2);
                    }
                    materialArray2[j] = dictionary[key];
                }
                renderer.materials = materialArray2;
            }
            Material[] array = new Material[dictionary.Count];
            dictionary.Values.CopyTo(array, 0);
            return array;
        }

        private Material[] CollectSharedMaterials()
        {
            HashSet<Material> set = new HashSet<Material>();
            Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
                for (int j = 0; j < sharedMaterials.Length; j++)
                {
                    if (!set.Contains(sharedMaterials[j]))
                    {
                        set.Add(sharedMaterials[j]);
                    }
                }
            }
            Material[] array = new Material[set.Count];
            set.CopyTo(array, 0);
            return array;
        }

        private void CommonInit(StageEntry stageEntry)
        {
            this.InitWindZone();
            this.InitLightProb();
            this.InitLightMapCorrection();
            Singleton<StageManager>.Instance.SetBaseStageEffectSetting(stageEntry.StageEffectSetting);
            Singleton<StageManager>.Instance.SetBaseStageEffectSetting(this._subWeatherStack.value.weather.stageEffectSetting);
            FixedStack<float> auxTimeScaleStack = Singleton<LevelManager>.Instance.levelEntity.auxTimeScaleStack;
            auxTimeScaleStack.onChanged = (Action<float, int, float, int>) Delegate.Combine(auxTimeScaleStack.onChanged, new Action<float, int, float, int>(this.AuxTimeScaleCallback));
        }

        public ContinueWeatherDataSettings GetContinueWeatherDataSetup()
        {
            return new ContinueWeatherDataSettings { renderingDataContinueTimer = this.GetCurrentRenderingDataLeftTimer(), weatherContinueTimer = this.GetCurrentWeatherDataLeftTimer(), currentWeatherData = this.GetCurrentWeatherData(), continueWeatherData = this.GetCurrentBaseWeatherData(), continueWeatherName = this.GetCurrentBaseWeatherName() };
        }

        public ConfigStageRenderingData GetCurrentBaseRenderingData()
        {
            return this._renderingDataStack.Get(0).renderingData;
        }

        private ConfigSubWeatherCollection GetCurrentBaseSubWeatherData()
        {
            return this._subWeatherStack.Get(0).weather;
        }

        public ConfigWeatherData GetCurrentBaseWeatherData()
        {
            return new ConfigWeatherData { configRenderingData = this.GetCurrentBaseRenderingData(), configSubWeathers = this.GetCurrentBaseSubWeatherData() };
        }

        public string GetCurrentBaseWeatherName()
        {
            return this._currentBaseWeatherName;
        }

        public ConfigStageRenderingData GetCurrentRenderingData()
        {
            return (ConfigStageRenderingData) this._currentRendertingData.Clone();
        }

        public float GetCurrentRenderingDataLeftTimer()
        {
            float num = 0f;
            if (this._renderingDataState == RenderingDataState.RenderingTransit)
            {
                num = this._renderingDataTransitionTimer.timespan - this._renderingDataTransitionTimer.timer;
            }
            return num;
        }

        private ConfigSubWeatherCollection GetCurrentSubWeatherData()
        {
            if (this._weatherState == WeatherState.Idle)
            {
                return this._currentSubWeather.Copy();
            }
            if (this._weatherState == WeatherState.WeatherTransit)
            {
                return ConfigSubWeatherCollection.Lerp(this._currentSubWeather, this._transitTargetSubWeather, this._weatherTransitionTimer.GetTimingRatio());
            }
            return null;
        }

        public ConfigWeatherData GetCurrentWeatherData()
        {
            return new ConfigWeatherData { configRenderingData = this.GetCurrentRenderingData(), configSubWeathers = this.GetCurrentSubWeatherData() };
        }

        public float GetCurrentWeatherDataLeftTimer()
        {
            float num = 0f;
            if (this._weatherState == WeatherState.WeatherTransit)
            {
                num = this._weatherTransitionTimer.timespan - this._weatherTransitionTimer.timer;
            }
            return num;
        }

        private ConfigStageRenderingData GetRenderingDataByName(string name)
        {
            return (!string.IsNullOrEmpty(name) ? RenderingData.GetRenderingDataConfig<ConfigStageRenderingData>(name) : ConfigStageRenderingData.CreateStageRenderingDataFromMaterials(this.CollectSharedMaterials()));
        }

        private ConfigWeatherData GetWeatherDataByName(string name)
        {
            ConfigWeatherData data = !string.IsNullOrEmpty(name) ? WeatherData.GetWeatherDataConfig(name) : new ConfigWeatherData();
            if (data.configRenderingData == null)
            {
                if (this._currentRendertingData != null)
                {
                    data.configRenderingData = this._currentRendertingData.Clone();
                }
                else
                {
                    data.configRenderingData = ConfigStageRenderingData.CreateDefault();
                }
            }
            if (data.configSubWeathers == null)
            {
                data.configSubWeathers = new ConfigSubWeatherCollection();
            }
            return data;
        }

        public virtual void Init(StageEntry entry, ConfigWeatherData weatherData)
        {
            this._currentBaseWeatherName = null;
            this._initBaseRenderingData = weatherData.configRenderingData as ConfigStageRenderingData;
            this._renderingDataStack.Push(0, new RenderingDataTransition(this._initBaseRenderingData), true);
            this._currentRendertingData = (ConfigStageRenderingData) this._initBaseRenderingData.Clone();
            this._currentRendertingData.ApplyGlobally();
            this._initBaseSubWeather = weatherData.configSubWeathers;
            this._subWeatherStack.Push(0, new SubWeatherTransition(this._initBaseSubWeather, 0), true);
            this.SetSubWeathersImmediately(this._initBaseSubWeather);
            this.CommonInit(entry);
        }

        public virtual void Init(StageEntry entry, string weatherName)
        {
            this._currentBaseWeatherName = weatherName;
            ConfigWeatherData weatherDataByName = this.GetWeatherDataByName(weatherName);
            this._initBaseRenderingData = weatherDataByName.configRenderingData as ConfigStageRenderingData;
            this._renderingDataStack.Push(0, new RenderingDataTransition(this._initBaseRenderingData), true);
            this._currentRendertingData = (ConfigStageRenderingData) this._initBaseRenderingData.Clone();
            this._currentRendertingData.ApplyGlobally();
            this._initBaseSubWeather = weatherDataByName.configSubWeathers;
            this._subWeatherStack.Push(0, new SubWeatherTransition(this._initBaseSubWeather, 0), true);
            this.SetSubWeathersImmediately(this._initBaseSubWeather);
            this.CommonInit(entry);
        }

        public virtual void Init(StageEntry entry, ConfigWeatherData fromWeatherData, ConfigWeatherData toWeatherData, float renderingTimer, float weatherTimer)
        {
            this._currentBaseWeatherName = null;
            this._currentRendertingData = (ConfigStageRenderingData) fromWeatherData.configRenderingData.Clone();
            this._currentRendertingData.ApplyGlobally();
            this._initBaseRenderingData = toWeatherData.configRenderingData as ConfigStageRenderingData;
            this._renderingDataStack.Push(0, new RenderingDataTransition(this._initBaseRenderingData), true);
            if (renderingTimer > 0f)
            {
                this.TransitRenderingData(this._initBaseRenderingData, renderingTimer);
            }
            else
            {
                this.SetRenderingDataImmediately(this._initBaseRenderingData);
            }
            this.SetSubWeathersImmediately(fromWeatherData.configSubWeathers);
            this._initBaseSubWeather = toWeatherData.configSubWeathers;
            this._subWeatherStack.Push(0, new SubWeatherTransition(this._initBaseSubWeather, 0), true);
            if (weatherTimer > 0f)
            {
                this.TransitSubWeather(this._initBaseSubWeather, weatherTimer);
            }
            else
            {
                this.SetSubWeathersImmediately(this._initBaseSubWeather);
            }
            this.CommonInit(entry);
        }

        public virtual void Init(StageEntry entry, ConfigWeatherData fromWeatherData, string toWeatherName, float renderingTimer, float weatherTimer)
        {
            this._currentBaseWeatherName = toWeatherName;
            ConfigWeatherData weatherDataByName = this.GetWeatherDataByName(toWeatherName);
            this._currentRendertingData = (ConfigStageRenderingData) fromWeatherData.configRenderingData.Clone();
            this._currentRendertingData.ApplyGlobally();
            this._initBaseRenderingData = weatherDataByName.configRenderingData as ConfigStageRenderingData;
            this._renderingDataStack.Push(0, new RenderingDataTransition(this._initBaseRenderingData), true);
            if (renderingTimer > 0f)
            {
                this.TransitRenderingData(this._initBaseRenderingData, renderingTimer);
            }
            else
            {
                this.SetRenderingDataImmediately(this._initBaseRenderingData);
            }
            this.SetSubWeathersImmediately(fromWeatherData.configSubWeathers);
            this._initBaseSubWeather = weatherDataByName.configSubWeathers;
            this._subWeatherStack.Push(0, new SubWeatherTransition(this._initBaseSubWeather, 0), true);
            if (weatherTimer > 0f)
            {
                this.TransitSubWeather(this._initBaseSubWeather, weatherTimer);
            }
            else
            {
                this.SetSubWeathersImmediately(this._initBaseSubWeather);
            }
            this.CommonInit(entry);
        }

        public void InitLightMapCorrection()
        {
            if (this.lightMapCorrectManager != null)
            {
                this.lightMapCorrectManager.Init();
            }
        }

        public void InitLightProb()
        {
            if (this.lightProbManager != null)
            {
                this.lightProbManager.Init();
            }
        }

        public void InitWindZone()
        {
            if (this.windZone != null)
            {
                this.windZone.GetComponent<MonoWindZone>().Init();
            }
        }

        public bool IsCurrentRenderingDataBase()
        {
            return (this._renderingDataStack.GetRealTopIndex() == 0);
        }

        public bool IsCurrentSubWeatherDataBase()
        {
            return (this._subWeatherStack.GetRealTopIndex() == 0);
        }

        public void OnDestroy()
        {
        }

        private void OnRenderingDataChanged(RenderingDataTransition oldTransition, int oldIx, RenderingDataTransition newTransition, int newIx)
        {
            if (oldTransition.renderingData != newTransition.renderingData)
            {
                float transitDuration;
                if (oldIx == newIx)
                {
                    transitDuration = newTransition.transitDuration;
                }
                else if (oldIx > newIx)
                {
                    transitDuration = oldTransition.transitDuration;
                }
                else
                {
                    transitDuration = newTransition.transitDuration;
                }
                this.TransitRenderingData(newTransition.renderingData, transitDuration);
            }
        }

        private void OnSubWeatherChanged(SubWeatherTransition oldTransition, int oldIx, SubWeatherTransition newTransition, int newIx)
        {
            float transitDuration;
            if (oldIx == newIx)
            {
                transitDuration = newTransition.transitDuration;
            }
            else if (oldIx > newIx)
            {
                transitDuration = oldTransition.transitDuration;
            }
            else
            {
                transitDuration = newTransition.transitDuration;
            }
            this.TransitSubWeather(newTransition.weather, transitDuration);
            Singleton<StageManager>.Instance.SetBaseStageEffectSetting(newTransition.weather.stageEffectSetting);
        }

        public void PopRenderingData(int stackIx)
        {
            this._renderingDataStack.Pop(stackIx);
        }

        public void PopWeather(int stackIx)
        {
            int renderingIndexInStack = this._subWeatherStack.Get(stackIx).renderingIndexInStack;
            if (renderingIndexInStack != -1)
            {
                this._renderingDataStack.Pop(renderingIndexInStack);
            }
            int stageEffectSettingIndexInStack = this._subWeatherStack.Get(stackIx).stageEffectSettingIndexInStack;
            if (stageEffectSettingIndexInStack != -1)
            {
                Singleton<StageManager>.Instance.PopStageSettingData(stageEffectSettingIndexInStack);
            }
            this._subWeatherStack.Pop(stackIx);
        }

        public int PushRenderingData(ConfigStageRenderingData renderingData, float transitDuration)
        {
            this.TrySetMiddleRenderingForBase();
            return this._renderingDataStack.Push(new RenderingDataTransition(renderingData, transitDuration), false);
        }

        public int PushRenderingData(string renderingDataName, float transitDuration)
        {
            this.TrySetMiddleRenderingForBase();
            ConfigStageRenderingData renderingDataByName = this.GetRenderingDataByName(renderingDataName);
            return this._renderingDataStack.Push(new RenderingDataTransition(renderingDataByName, transitDuration), false);
        }

        private int PushSubWeatherData(ConfigSubWeatherCollection subWeather, int renderingIdx, int settingIx, float transitDuration)
        {
            this.TrySetMiddleSubWeatherForBase();
            return this._subWeatherStack.Push(new SubWeatherTransition(subWeather, renderingIdx, settingIx, transitDuration), false);
        }

        public int PushWeather(string weatherName, float transitDuration)
        {
            ConfigWeatherData weatherDataByName = this.GetWeatherDataByName(weatherName);
            int renderingIdx = this.PushRenderingData(weatherDataByName.configRenderingData as ConfigStageRenderingData, transitDuration);
            int settingIx = -1;
            if (weatherDataByName.configSubWeathers.stageEffectSetting != null)
            {
                settingIx = Singleton<StageManager>.Instance.PushStageSettingData(weatherDataByName.configSubWeathers.stageEffectSetting);
            }
            return this.PushSubWeatherData(weatherDataByName.configSubWeathers, renderingIdx, settingIx, transitDuration);
        }

        public virtual void Reset(StageEntry entry, ConfigWeatherData weatherData)
        {
            this._initBaseRenderingData = weatherData.configRenderingData as ConfigStageRenderingData;
            this._renderingDataStack.Set(0, new RenderingDataTransition(this._initBaseRenderingData), true);
            this.SetRenderingDataImmediately(this._initBaseRenderingData);
            this._currentRendertingData.ApplyGlobally();
            this._initBaseSubWeather = weatherData.configSubWeathers;
            this._subWeatherStack.Set(0, new SubWeatherTransition(this._initBaseSubWeather, 0), true);
            this.SetSubWeathersImmediately(this._initBaseSubWeather);
            this.CommonInit(entry);
        }

        public void ResetBaseRenderingData(float duration)
        {
            this._renderingDataStack.Set(0, new RenderingDataTransition(this._initBaseRenderingData, duration), false);
        }

        private void ResetBaseSubWeather(float duration)
        {
            this._subWeatherStack.Set(0, new SubWeatherTransition(this._initBaseSubWeather, 0, duration), false);
        }

        public void ResetBaseWeather(float duration)
        {
            this.ResetBaseRenderingData(duration);
            this.ResetBaseSubWeather(duration);
        }

        public void SetBaseRenderingData(ConfigStageRenderingData renderingData, float duration)
        {
            this._renderingDataStack.Set(0, new RenderingDataTransition(renderingData, duration), false);
        }

        public void SetBaseRenderingData(string renderingDataName, float duration)
        {
            ConfigStageRenderingData renderingDataByName = this.GetRenderingDataByName(renderingDataName);
            this._renderingDataStack.Set(0, new RenderingDataTransition(renderingDataByName, duration), false);
        }

        private void SetBaseSubWeather(ConfigSubWeatherCollection subWeather, float duration)
        {
            this._subWeatherStack.Set(0, new SubWeatherTransition(subWeather, 0, duration), false);
        }

        public void SetBaseWeather(string weatherName, float duration)
        {
            this._currentBaseWeatherName = weatherName;
            ConfigWeatherData weatherDataByName = this.GetWeatherDataByName(weatherName);
            this.SetBaseRenderingData(weatherDataByName.configRenderingData as ConfigStageRenderingData, duration);
            this.SetBaseSubWeather(weatherDataByName.configSubWeathers, duration);
        }

        private void SetRain(ConfigRain config)
        {
            if (Application.isPlaying)
            {
                if ((config == null) || (config.density < 0.001f))
                {
                    if (this._rainObject != null)
                    {
                        UnityEngine.Object.Destroy(this._rainObject);
                        this._rainObject = null;
                    }
                }
                else
                {
                    if (this._rainObject == null)
                    {
                        this._rainObject = UnityEngine.Object.Instantiate<GameObject>(this._rainPrefab);
                        this._rainObject.transform.SetParent(base.transform);
                        this.rainController = this._rainObject.GetComponent<RainController>();
                        this.rainController.Init();
                    }
                    this.rainController.SetRain(config);
                }
            }
        }

        public void SetRenderingDataImmediately(ConfigStageRenderingData targetRenderingData)
        {
            this._currentRendertingData = (ConfigStageRenderingData) targetRenderingData.Clone();
            if (this._renderingDataState == RenderingDataState.RenderingTransit)
            {
                this._renderingDataState = RenderingDataState.Idle;
                this._renderingDataTransitionTimer.Reset(false);
            }
        }

        private void SetSubWeathers(ConfigSubWeatherCollection subWeather)
        {
            this.SetRain(subWeather.configRain);
        }

        private void SetSubWeathersImmediately(ConfigSubWeatherCollection subWeather)
        {
            this.SetSubWeathers(subWeather);
            this._currentSubWeather = subWeather;
            if (this._weatherState == WeatherState.WeatherTransit)
            {
                this._weatherState = WeatherState.Idle;
                this._weatherTransitionTimer.Reset(false);
            }
        }

        public void SetWeahterImmediately(ConfigWeatherData config)
        {
            if (!Application.isPlaying)
            {
                if (config.configRenderingData != null)
                {
                    config.configRenderingData.ApplyGlobally();
                }
            }
            else
            {
                this.SetRenderingDataImmediately(config.configRenderingData as ConfigStageRenderingData);
                this._currentRendertingData.ApplyGlobally();
            }
            this.SetSubWeathersImmediately(config.configSubWeathers);
        }

        public virtual void Start()
        {
        }

        [DebuggerHidden]
        private IEnumerator TintIter(string renderDataName, float duration, float transitDuration)
        {
            return new <TintIter>c__Iterator41 { renderDataName = renderDataName, transitDuration = transitDuration, duration = duration, <$>renderDataName = renderDataName, <$>transitDuration = transitDuration, <$>duration = duration, <>f__this = this };
        }

        public void TransitRenderingData(ConfigStageRenderingData targetRenderingData, float duration)
        {
            this._currentRendertingData.SetupTransition(targetRenderingData);
            this._renderingDataState = RenderingDataState.RenderingTransit;
            this._renderingDataTransitionTimer.timespan = duration;
            this._renderingDataTransitionTimer.Reset(true);
        }

        private void TransitSubWeather(ConfigSubWeatherCollection subWeather, float transitDuration)
        {
            if (this._weatherState == WeatherState.WeatherTransit)
            {
                this._currentSubWeather = ConfigSubWeatherCollection.Lerp(this._currentSubWeather, this._transitTargetSubWeather, this._weatherTransitionTimer.GetTimingRatio());
                this._transitTargetSubWeather = subWeather;
            }
            else if (this._weatherState == WeatherState.Idle)
            {
                this._transitTargetSubWeather = subWeather;
                this._weatherState = WeatherState.WeatherTransit;
            }
            ConfigSubWeatherCollection.LerpPreparation(this._currentSubWeather, this._transitTargetSubWeather);
            this._weatherTransitionTimer.timespan = transitDuration;
            this._weatherTransitionTimer.Reset(true);
        }

        public void TransitWeatherData(ConfigWeatherData targetWeatherData, float renderingDataDuration, float weatherDuration)
        {
            this.TransitRenderingData(targetWeatherData.configRenderingData as ConfigStageRenderingData, renderingDataDuration);
            this.TransitSubWeather(targetWeatherData.configSubWeathers, weatherDuration);
        }

        public void TriggerTint(string renderDataName, float duration, float transitDuration)
        {
            base.StartCoroutine(this.TintIter(renderDataName, duration, transitDuration));
        }

        public void TrySetMiddleRenderingForBase()
        {
            if (this.IsCurrentRenderingDataBase() && (this._renderingDataState == RenderingDataState.RenderingTransit))
            {
                float duration = this._renderingDataTransitionTimer.timespan - this._renderingDataTransitionTimer.timer;
                this._middleRenderingIndex = this._renderingDataStack.Push(new RenderingDataTransition(this.GetCurrentRenderingData(), duration), true);
                this._isInMiddleRendering = true;
            }
        }

        public void TrySetMiddleSubWeatherForBase()
        {
            if (this.IsCurrentSubWeatherDataBase() && (this._weatherState == WeatherState.WeatherTransit))
            {
                float duration = this._weatherTransitionTimer.timespan - this._weatherTransitionTimer.timer;
                this._middleSubWeatherIndex = this._subWeatherStack.Push(new SubWeatherTransition(this.GetCurrentSubWeatherData(), this._middleRenderingIndex, duration), true);
                this._isInMiddleRendering = true;
            }
        }

        public virtual void Update()
        {
            Shader.SetGlobalVector("_miHoYo_CameraRight", this._mainCameraTransform.transform.right);
            this.UpdateRenderingData();
            this.UpdateSubWeather();
        }

        private void UpdateRenderingData()
        {
            if (this._renderingDataState == RenderingDataState.Idle)
            {
                if (this._isInMiddleRendering && (this._renderingDataStack.GetRealTopIndex() == this._middleRenderingIndex))
                {
                    this.PopRenderingData(this._middleRenderingIndex);
                    this._isInMiddleRendering = false;
                    this._middleRenderingIndex = 0;
                }
            }
            else if (this._renderingDataState == RenderingDataState.RenderingTransit)
            {
                this._renderingDataTransitionTimer.Core(1f);
                this._currentRendertingData.LerpStep(this._renderingDataTransitionTimer.GetTimingRatio());
                this._currentRendertingData.ApplyGlobally();
                if (this._renderingDataTransitionTimer.isTimeUp)
                {
                    if (this._isInMiddleRendering && (this._renderingDataStack.GetRealTopIndex() == this._middleRenderingIndex))
                    {
                        this.PopRenderingData(this._middleRenderingIndex);
                        this._isInMiddleRendering = false;
                        this._middleRenderingIndex = 0;
                    }
                    else
                    {
                        this._renderingDataTransitionTimer.Reset(false);
                        this._renderingDataState = RenderingDataState.Idle;
                    }
                }
            }
        }

        private void UpdateSubWeather()
        {
            if (((this._weatherState == WeatherState.Idle) && this._isInMiddleWeather) && (this._subWeatherStack.GetRealTopIndex() == this._middleSubWeatherIndex))
            {
                this.PopWeather(this._middleSubWeatherIndex);
                this._isInMiddleWeather = false;
                this._middleSubWeatherIndex = 0;
            }
            if (this._weatherState == WeatherState.WeatherTransit)
            {
                this._weatherTransitionTimer.Core(1f);
                ConfigSubWeatherCollection subWeather = ConfigSubWeatherCollection.Lerp(this._currentSubWeather, this._transitTargetSubWeather, this._weatherTransitionTimer.GetTimingRatio());
                this.SetSubWeathers(subWeather);
                if (this._weatherTransitionTimer.isTimeUp)
                {
                    if (this._isInMiddleWeather && (this._subWeatherStack.GetRealTopIndex() == this._middleSubWeatherIndex))
                    {
                        this.PopWeather(this._middleSubWeatherIndex);
                        this._isInMiddleWeather = false;
                        this._middleSubWeatherIndex = 0;
                    }
                    else
                    {
                        this._weatherState = WeatherState.Idle;
                        this._currentSubWeather = subWeather;
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <TintIter>c__Iterator41 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <$>duration;
            internal string <$>renderDataName;
            internal float <$>transitDuration;
            internal MonoBasePerpStage <>f__this;
            internal int <stackIx>__0;
            internal float duration;
            internal string renderDataName;
            internal float transitDuration;

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
                        this.<stackIx>__0 = this.<>f__this.PushRenderingData(this.renderDataName, this.transitDuration);
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00A0;
                }
                if (this.duration > 0f)
                {
                    this.duration -= Time.deltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<>f__this.PopRenderingData(this.<stackIx>__0);
                this.$PC = -1;
            Label_00A0:
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

        public class ContinueWeatherDataSettings
        {
            public ConfigWeatherData continueWeatherData;
            public string continueWeatherName;
            public ConfigWeatherData currentWeatherData;
            public float renderingDataContinueTimer;
            public float weatherContinueTimer;
        }

        private enum RenderingDataState
        {
            Idle,
            RenderingTransit
        }

        private class RenderingDataTransition
        {
            public ConfigStageRenderingData renderingData;
            public float transitDuration;

            public RenderingDataTransition(ConfigStageRenderingData data)
            {
                this.transitDuration = 0.5f;
                this.renderingData = data;
            }

            public RenderingDataTransition(ConfigStageRenderingData data, float duration)
            {
                this.transitDuration = 0.5f;
                this.renderingData = data;
                this.transitDuration = duration;
            }
        }

        private class SubWeatherTransition
        {
            public int renderingIndexInStack;
            public int stageEffectSettingIndexInStack;
            public float transitDuration;
            public ConfigSubWeatherCollection weather;

            public SubWeatherTransition(ConfigSubWeatherCollection data, int renderingIdx)
            {
                this.transitDuration = 0.5f;
                this.stageEffectSettingIndexInStack = -1;
                this.weather = data;
                this.renderingIndexInStack = renderingIdx;
            }

            public SubWeatherTransition(ConfigSubWeatherCollection data, int renderingIdx, float duration)
            {
                this.transitDuration = 0.5f;
                this.stageEffectSettingIndexInStack = -1;
                this.weather = data;
                this.transitDuration = duration;
                this.renderingIndexInStack = renderingIdx;
            }

            public SubWeatherTransition(ConfigSubWeatherCollection data, int renderingIdx, int settingIx, float duration)
            {
                this.transitDuration = 0.5f;
                this.stageEffectSettingIndexInStack = -1;
                this.weather = data;
                this.transitDuration = duration;
                this.renderingIndexInStack = renderingIdx;
                this.stageEffectSettingIndexInStack = settingIx;
            }
        }

        private enum WeatherState
        {
            Idle,
            WeatherTransit
        }
    }
}

