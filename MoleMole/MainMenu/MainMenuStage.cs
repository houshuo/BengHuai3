namespace MoleMole.MainMenu
{
    using MoleMole;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MainMenuStage : MonoBehaviour
    {
        private ConfigAtmosphere _atmosphereConfig;
        private ConfigAtmosphereSeries _atmosphereConfigSeries;
        private int _atmosphereConfigSeriesId;
        private Camera _bgCamera;
        private static readonly float _bgFarClip = 10000f;
        private static readonly string _bgLayerName = "Water";
        private MaterialPropertyBlock _bgMPB;
        private RenderTextureWrapper _bgRenderTexture;
        private CloudEmitter _cloudEmitter;
        private int _currentKey;
        private bool _isSomethingWrong;
        private bool _needUpdateAtmosphere = true;
        private int _nextKey;
        private float _remainTransitionTime;
        private MaterialPropertyBlock _skyMPB;
        private Renderer _skyRenderer;
        private float _transitionTime;
        private float _virtualDayTime = -1f;
        [HideInInspector]
        public string AtmosphereConfigSeriesPath;
        public float BackgroundDist = 6000f;
        public float BackgroundExtendAngle = 4f;
        public GameObject BackgroundQuad;
        [HideInInspector]
        public bool ForceUpdateAtmosphere = true;
        [HideInInspector]
        public bool IsInTransition;
        [HideInInspector]
        public bool IsUpdateAtmosphereAuto = true;
        [HideInInspector]
        public bool UpdateAtmosphereWithTransition = true;

        public void ChooseAtmosphereSeriesDefault()
        {
            string str = "Rendering/MainMenuAtmosphereConfig/Default";
            bool flag = Singleton<PlayerModule>.Instance.playerData.userId != 0;
            string path = !flag ? str : Singleton<MiHoYoGameData>.Instance.LocalData.CurrentWeatherConfigPath;
            ConfigAtmosphereSeries config = ConfigAtmosphereSeries.LoadFromFileAndDetach(path);
            int sceneId = !flag ? 0 : Singleton<MiHoYoGameData>.Instance.LocalData.CurrentWeatherSceneID;
            this.ChooseCloudScene(config, sceneId);
        }

        public void ChooseAtmosphereSeriesNext()
        {
            ConfigAtmosphereSeries config = ConfigAtmosphereSeries.LoadFromFileAndDetach(AtmosphereSeriesData.GetPath(AtmosphereSeriesData.GetNextId(this._atmosphereConfigSeriesId)));
            int sceneIdRandomly = config.GetSceneIdRandomly();
            this.ChooseCloudScene(config, sceneIdRandomly);
        }

        public void ChooseAtmosphereSeriesRandomly()
        {
            ConfigAtmosphereSeries config = ConfigAtmosphereSeries.LoadFromFileAndDetach(AtmosphereSeriesData.GetPathRandomly());
            int sceneIdRandomly = config.GetSceneIdRandomly();
            this.ChooseCloudScene(config, sceneIdRandomly);
        }

        public void ChooseCloudScene(ConfigAtmosphereSeries config, int sceneId)
        {
            if ((config != null) && config.IsValid())
            {
                this._atmosphereConfigSeries = config;
                this.AtmosphereConfigSeriesPath = config.Path;
                int num = this.AtmosphereConfigSeriesPath.LastIndexOf('/');
                string str = this.AtmosphereConfigSeriesPath.Substring(num + 1);
                this._atmosphereConfigSeriesId = AtmosphereSeriesData.GetId(this.AtmosphereConfigSeriesPath);
                this._atmosphereConfigSeries.SetSceneId(sceneId);
                this._currentKey = this.AtmosphereConfigSeries.KeyBeforeTime(this.DayTime);
                this._nextKey = this._currentKey;
                this._atmosphereConfig = this.AtmosphereConfigSeries.Value(this._currentKey);
                this._needUpdateAtmosphere = true;
                this.IsInTransition = false;
                this.ReleaseBackgroundRenderTexture();
                this._cloudEmitter.gameObject.SetActive(true);
                this.UpdateAtmosphere();
            }
        }

        public void ChooseCloudSceneNext()
        {
            this._atmosphereConfigSeries.Common.UpdateSceneNameNext();
            this._needUpdateAtmosphere = true;
            this.ReleaseBackgroundRenderTexture();
            this._cloudEmitter.gameObject.SetActive(true);
            this.UpdateAtmosphere();
        }

        private bool DrawBackgroundToRenderTexture()
        {
            Camera main = Camera.main;
            if (main == null)
            {
                return false;
            }
            int num = LayerMask.NameToLayer(_bgLayerName);
            GameObject gameObject = this._skyRenderer.gameObject;
            int layer = gameObject.layer;
            gameObject.layer = num;
            GameObject obj3 = this._cloudEmitter.gameObject;
            int num3 = obj3.layer;
            obj3.layer = num;
            gameObject.SetActive(true);
            obj3.SetActive(true);
            this.ReleaseBackgroundRenderTexture();
            this._bgRenderTexture = GraphicsUtils.GetRenderTexture(main.pixelWidth, main.pixelHeight, 0x10, RenderTextureFormat.ARGBHalf);
            this._bgRenderTexture.onRebindToCameraCallBack = new Action(this.ReleaseBackgroundRenderTexture);
            if (this._bgCamera == null)
            {
                System.Type[] components = new System.Type[] { typeof(Camera) };
                this._bgCamera = new GameObject("Background Camera", components) { hideFlags = HideFlags.HideAndDontSave }.GetComponent<Camera>();
                this._bgCamera.enabled = false;
                this._bgCamera.CopyFrom(main);
                this._bgCamera.farClipPlane = _bgFarClip;
                string[] layerNames = new string[] { _bgLayerName };
                this._bgCamera.cullingMask = LayerMask.GetMask(layerNames);
                this._bgCamera.fieldOfView += this.BackgroundExtendAngle * 2f;
                this._bgCamera.transform.position = main.transform.position;
                this._bgCamera.transform.rotation = main.transform.rotation;
            }
            this._bgCamera.targetTexture = (RenderTexture) this._bgRenderTexture;
            this._bgRenderTexture.BindToCamera(this._bgCamera);
            this._bgCamera.Render();
            this._bgRenderTexture.UnbindFromCamera(this._bgCamera);
            gameObject.layer = layer;
            obj3.layer = num3;
            return true;
        }

        private void EvaluateAtmosphere()
        {
            if (this.UpdateAtmosphereWithTransition)
            {
                int num = this.AtmosphereConfigSeries.KeyBeforeTime(this.DayTime);
                if (num != this._currentKey)
                {
                    if (!this.IsInTransition)
                    {
                        this._nextKey = num;
                        this.IsInTransition = true;
                        this._remainTransitionTime = this.AtmosphereConfigSeries.Common.TransitionTime;
                        base.StartCoroutine(this.TransitAtmosphere());
                    }
                    else if (num != this._nextKey)
                    {
                        this._nextKey = num;
                        this._remainTransitionTime = this.AtmosphereConfigSeries.Common.TransitionTime;
                    }
                }
            }
            else if (!this.IsInTransition)
            {
                this._needUpdateAtmosphere = true;
                this._currentKey = this._atmosphereConfigSeries.KeyBeforeTime(this.DayTime);
                this._atmosphereConfig = this._atmosphereConfigSeries.Evaluate(this.DayTime, true);
            }
        }

        private void Init()
        {
            try
            {
                this._skyRenderer = base.transform.Find("Sky").gameObject.GetComponent<Renderer>();
                this._skyMPB = new MaterialPropertyBlock();
                this._cloudEmitter = base.GetComponentInChildren<CloudEmitter>(true);
                if (GlobalDataManager.metaConfig == null)
                {
                    GlobalDataManager.Refresh();
                }
                this.IsUpdateAtmosphereAuto = true;
                this.UpdateAtmosphere();
                this._isSomethingWrong = false;
            }
            catch
            {
                this._isSomethingWrong = true;
                throw;
            }
        }

        private void OnDestroy()
        {
            this.ReleaseBackgroundRenderTexture();
        }

        private void OnEnable()
        {
            this.Init();
        }

        private void ReleaseBackgroundRenderTexture()
        {
            if (this._bgRenderTexture != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this._bgRenderTexture);
                this._bgRenderTexture = null;
            }
        }

        private void SetBackgroundQuad()
        {
            if (this._bgCamera != null)
            {
                float num = this._bgCamera.fieldOfView * 0.01745329f;
                float y = (this.BackgroundDist * Mathf.Tan(num * 0.5f)) * 2f;
                float x = y * this._bgCamera.aspect;
                Transform transform = this.BackgroundQuad.transform;
                transform.localScale = (Vector3) new Vector2(x, y);
                transform.eulerAngles = this._bgCamera.transform.eulerAngles;
                transform.position = (Vector3) (transform.forward * this.BackgroundDist);
            }
        }

        public void SetupAtmosphere(ConfigAtmosphereCommon commonConfig, ConfigAtmosphere config)
        {
            if (this._skyMPB == null)
            {
                this.Init();
            }
            this.SetupSky(commonConfig, config.Background);
            this._cloudEmitter.SetupCloudConfig(commonConfig, config.CloudStyle);
            this.SetupIndoor(config.Indoor);
        }

        private void SetupIndoor(ConfigIndoor indoor)
        {
            Shader.SetGlobalColor("_miHoYo_Indoor_Tint_Color", indoor.TintColor);
        }

        private void SetupSky(ConfigAtmosphereCommon commonConfig, ConfigBackground config)
        {
            if (commonConfig.Tex != null)
            {
                this._skyMPB.SetTexture("_MainTex", commonConfig.Tex);
                this._skyMPB.SetColor("_TexRColor", config.RColor);
                this._skyMPB.SetColor("_TexGColor", config.GColor);
                this._skyMPB.SetColor("_TexBColor", config.BColor);
                this._skyMPB.SetFloat("_TexXLocation", config.XLocation);
                this._skyMPB.SetFloat("_TexYLocation", config.YLocation);
                this._skyMPB.SetFloat("_TexHigh", config.High);
            }
            if (commonConfig.SecondTex != null)
            {
                this._skyMPB.SetTexture("_SecTexture", commonConfig.SecondTex);
                this._skyMPB.SetFloat("_SecTexXLocation", config.SecTexXLocation);
                this._skyMPB.SetFloat("_SecTexYLocation", config.SecTexYLocation);
                this._skyMPB.SetFloat("_SecTexHigh", config.SecTexHigh);
                this._skyMPB.SetFloat("_SecTexEmission", config.SecTexEmission);
            }
            this._skyMPB.SetColor("_GradBottomColor", config.GradBottomColor);
            this._skyMPB.SetColor("_GradTopColor", config.GradTopColor);
            this._skyMPB.SetFloat("_GradLocation", config.GradLocation);
            this._skyMPB.SetFloat("_GradHigh", config.GradHigh);
            this._skyMPB.SetFloat("_BloomFactor", config.BloomFactor);
            this._skyRenderer.SetPropertyBlock(this._skyMPB);
        }

        private void Start()
        {
            this.Init();
        }

        [DebuggerHidden]
        private IEnumerator TransitAtmosphere()
        {
            return new <TransitAtmosphere>c__Iterator40 { <>f__this = this };
        }

        public void Update()
        {
            if (!this._isSomethingWrong)
            {
                try
                {
                    if (!GlobalVars.STATIC_CLOUD_MODE)
                    {
                        this.UpdateAtmosphere();
                        if (this._bgRenderTexture != null)
                        {
                            GraphicsUtils.ReleaseRenderTexture(this._bgRenderTexture);
                            this._bgRenderTexture = null;
                        }
                        if (this._skyRenderer != null)
                        {
                            this._skyRenderer.gameObject.SetActive(true);
                        }
                        if (this._cloudEmitter != null)
                        {
                            this._cloudEmitter.gameObject.SetActive(true);
                        }
                        if (this.BackgroundQuad != null)
                        {
                            this.BackgroundQuad.SetActive(false);
                        }
                    }
                    else if ((this._bgRenderTexture == null) && (this._atmosphereConfigSeries != null))
                    {
                        this.DrawBackgroundToRenderTexture();
                        Renderer component = this.BackgroundQuad.GetComponent<Renderer>();
                        if (this._bgMPB == null)
                        {
                            this._bgMPB = new MaterialPropertyBlock();
                            if (component != null)
                            {
                                component.GetPropertyBlock(this._bgMPB);
                            }
                        }
                        this._bgMPB.SetTexture("_MainTex", (Texture) this._bgRenderTexture);
                        if (component != null)
                        {
                            component.SetPropertyBlock(this._bgMPB);
                        }
                        if (this._skyRenderer != null)
                        {
                            this._skyRenderer.gameObject.SetActive(false);
                        }
                        if (this._cloudEmitter != null)
                        {
                            this._cloudEmitter.gameObject.SetActive(false);
                        }
                        if (this.BackgroundQuad != null)
                        {
                            this.BackgroundQuad.SetActive(true);
                        }
                        this.SetBackgroundQuad();
                    }
                    this._isSomethingWrong = false;
                }
                catch
                {
                    this._isSomethingWrong = true;
                    throw;
                }
            }
        }

        private void UpdateAtmosphere()
        {
            if (this.AtmosphereConfigSeries != null)
            {
                this.EvaluateAtmosphere();
                if (this.ForceUpdateAtmosphere || (this._needUpdateAtmosphere && this.IsUpdateAtmosphereAuto))
                {
                    this.SetupAtmosphere(this.AtmosphereConfigSeries.Common, this._atmosphereConfig);
                }
                this.ForceUpdateAtmosphere = false;
                this._needUpdateAtmosphere = false;
            }
        }

        public ConfigAtmosphere AtmosphereConfig
        {
            get
            {
                return this._atmosphereConfig;
            }
        }

        public ConfigAtmosphereSeries AtmosphereConfigSeries
        {
            get
            {
                return this._atmosphereConfigSeries;
            }
        }

        public string CloudSceneName
        {
            get
            {
                return this._atmosphereConfigSeries.Common.ScneneName;
            }
        }

        public float DayTime
        {
            get
            {
                if (this._virtualDayTime > -0.9f)
                {
                    return this._virtualDayTime;
                }
                DateTime now = TimeUtil.Now;
                return (((now.Hour + (((float) now.Minute) / 60f)) + (((float) now.Second) / 3600f)) + (((float) now.Millisecond) / 3600000f));
            }
            set
            {
                this._virtualDayTime = value;
            }
        }

        [CompilerGenerated]
        private sealed class <TransitAtmosphere>c__Iterator40 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MainMenuStage <>f__this;
            internal float <t>__0;

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
                    case 1:
                        if (!this.<>f__this.IsInTransition)
                        {
                            break;
                        }
                        this.<>f__this._needUpdateAtmosphere = true;
                        this.<t>__0 = Time.deltaTime / this.<>f__this._remainTransitionTime;
                        this.<>f__this._remainTransitionTime -= Time.deltaTime;
                        this.<>f__this._remainTransitionTime = Mathf.Max(1E-06f, this.<>f__this._remainTransitionTime);
                        if (this.<t>__0 >= 1f)
                        {
                            break;
                        }
                        this.<>f__this._atmosphereConfig = ConfigAtmosphere.Lerp(this.<>f__this._atmosphereConfig, this.<>f__this.AtmosphereConfigSeries.Value(this.<>f__this._nextKey), this.<t>__0);
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_0176;

                    case 2:
                        this.$PC = -1;
                        goto Label_0174;

                    default:
                        goto Label_0174;
                }
                if (this.<>f__this.IsInTransition)
                {
                    this.<>f__this._currentKey = this.<>f__this._nextKey;
                    this.<>f__this._atmosphereConfig = this.<>f__this.AtmosphereConfigSeries.Value(this.<>f__this._currentKey);
                    this.<>f__this.IsInTransition = false;
                }
                this.$current = null;
                this.$PC = 2;
                goto Label_0176;
            Label_0174:
                return false;
            Label_0176:
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

