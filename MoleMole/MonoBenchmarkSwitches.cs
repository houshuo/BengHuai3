namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.SceneManagement;

    public class MonoBenchmarkSwitches : MonoBehaviour
    {
        private bool _isDisableEventSystem;
        private Texture2D[] _simpleTextures;
        private GUIStyle _style;
        protected List<BenchmarkSwitch> _switches;
        protected bool _toggled;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache10;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache11;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache12;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache13;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache14;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache15;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache8;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache9;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cacheA;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cacheB;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cacheC;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cacheD;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cacheE;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cacheF;
        private const int BUTTONS_PER_COLUMN = 8;

        [CompilerGenerated]
        private static void <MakeComponentEnabledToggler`1>m__95<T>(bool enabled) where T: Behaviour
        {
            foreach (T local in UnityEngine.Object.FindObjectsOfType<T>())
            {
                local.enabled = enabled;
            }
        }

        [CompilerGenerated]
        private static void <MakeComponentEnabledToggler`2>m__96<T1, T2>(bool enabled) where T1: Behaviour where T2: Behaviour
        {
            foreach (T1 local in UnityEngine.Object.FindObjectsOfType<T1>())
            {
                local.GetComponent<T2>().enabled = enabled;
            }
        }

        protected void AddUIPageTogglers(Dictionary<string, string[]> pageDict)
        {
            BaseMonoCanvas canvas = UnityEngine.Object.FindObjectOfType<BaseMonoCanvas>();
            if (canvas != null)
            {
                foreach (KeyValuePair<string, string[]> pair in pageDict)
                {
                    <AddUIPageTogglers>c__AnonStoreyBC ybc = new <AddUIPageTogglers>c__AnonStoreyBC {
                        trsfs = new Transform[pair.Value.Length]
                    };
                    for (int i = 0; i < ybc.trsfs.Length; i++)
                    {
                        ybc.trsfs[i] = canvas.transform.Find(pair.Value[i]);
                    }
                    this._switches.Add(new BenchmarkSwitch("UI " + pair.Key, new Action<bool>(ybc.<>m__9B)));
                }
            }
        }

        private void Awake()
        {
            this._simpleTextures = new Texture2D[4];
            Color[] colorArray = new Color[] { Color.red, Color.green, Color.blue, Color.yellow };
            for (int i = 0; i < this._simpleTextures.Length; i++)
            {
                this._simpleTextures[i] = new Texture2D(0x10, 0x10);
                Color[] colors = new Color[0x100];
                for (int j = 0; j < colors.Length; j++)
                {
                    colors[j] = colorArray[i];
                }
                this._simpleTextures[i].SetPixels(colors);
            }
        }

        private Action<bool> MakeComponentEnabledToggler<T>() where T: Behaviour
        {
            return new Action<bool>(MonoBenchmarkSwitches.<MakeComponentEnabledToggler`1>m__95<T>);
        }

        private Action<bool> MakeComponentEnabledToggler<T1, T2>() where T1: Behaviour where T2: Behaviour
        {
            return new Action<bool>(MonoBenchmarkSwitches.<MakeComponentEnabledToggler`2>m__96<T1, T2>);
        }

        protected Action<bool> MakeDamageTextToggler()
        {
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = delegate (bool enabled) {
                    GlobalVars.muteDamageText = !enabled;
                };
            }
            return <>f__am$cache13;
        }

        private Action<bool> MakeEventSystemEnabledToggler()
        {
            return delegate (bool enabled) {
                if (!enabled)
                {
                    this._isDisableEventSystem = true;
                }
                foreach (EventSystem system in UnityEngine.Object.FindObjectsOfType<EventSystem>())
                {
                    system.enabled = enabled;
                }
            };
        }

        protected Action<bool> MakeInlevelLockTogger()
        {
            if (<>f__am$cache14 == null)
            {
                <>f__am$cache14 = delegate (bool enabled) {
                    GlobalVars.muteInlevelLock = !enabled;
                };
            }
            return <>f__am$cache14;
        }

        protected Action<bool> MakeUICameraClearToggler()
        {
            if (<>f__am$cache15 == null)
            {
                <>f__am$cache15 = delegate (bool enabled) {
                    foreach (Camera camera in UnityEngine.Object.FindObjectsOfType<Camera>())
                    {
                        if (camera.gameObject.name == "UICamera")
                        {
                            camera.clearFlags = !enabled ? CameraClearFlags.Nothing : CameraClearFlags.Depth;
                        }
                    }
                    foreach (MonoInLevelUICamera camera2 in UnityEngine.Object.FindObjectsOfType<MonoInLevelUICamera>())
                    {
                        camera2.GetComponent<Camera>().clearFlags = !enabled ? CameraClearFlags.Nothing : CameraClearFlags.Depth;
                    }
                };
            }
            return <>f__am$cache15;
        }

        protected Action<bool> MakeUICameraEnabledToggler()
        {
            if (<>f__am$cache12 == null)
            {
                <>f__am$cache12 = delegate (bool enabled) {
                    foreach (Camera camera in UnityEngine.Object.FindObjectsOfType<Camera>())
                    {
                        if (camera.gameObject.name == "UICamera")
                        {
                            camera.enabled = enabled;
                        }
                    }
                    foreach (MonoInLevelUICamera camera2 in UnityEngine.Object.FindObjectsOfType<MonoInLevelUICamera>())
                    {
                        camera2.GetComponent<Camera>().enabled = enabled;
                    }
                };
            }
            return <>f__am$cache12;
        }

        private void OnDestroy()
        {
            UnityEngine.Object.Destroy(this._style.normal.background);
        }

        private void OnEnable()
        {
            this._style = new GUIStyle();
            Texture2D textured = new Texture2D(0x10, 0x10);
            Color[] colors = new Color[0x100];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.gray;
            }
            textured.SetPixels(colors);
            this._style.normal.background = textured;
        }

        private void OnGUI()
        {
            if (this._toggled)
            {
                this.WidgetSwitches();
                if (GUI.Button(new Rect(Screen.width * 0.85f, Screen.height * 0.85f, 60f, 60f), "CLOSE"))
                {
                    this._toggled = false;
                    if (!this._isDisableEventSystem)
                    {
                        this.SetEventSystemEnable(true);
                    }
                }
                if (GUI.Button(new Rect(Screen.width * 0.93f, Screen.height * 0.85f, 60f, 60f), "Return"))
                {
                    MonoDevLevel level = UnityEngine.Object.FindObjectOfType<MonoDevLevel>();
                    if (level != null)
                    {
                        level.gameObject.SetActive(false);
                    }
                    foreach (Canvas canvas in UnityEngine.Object.FindObjectsOfType<Canvas>())
                    {
                        canvas.gameObject.SetActive(false);
                    }
                    if (Singleton<AvatarManager>.Instance != null)
                    {
                        Singleton<AvatarManager>.Instance.RemoveAllAvatars();
                    }
                    if (Singleton<MonsterManager>.Instance != null)
                    {
                        Singleton<MonsterManager>.Instance.RemoveAllMonsters();
                    }
                    if (Singleton<PropObjectManager>.Instance != null)
                    {
                        Singleton<PropObjectManager>.Instance.RemoveAllPropObjects();
                    }
                    if (Singleton<DynamicObjectManager>.Instance != null)
                    {
                        Singleton<DynamicObjectManager>.Instance.RemoveAllDynamicObjects();
                    }
                    if (Singleton<CameraManager>.Instance != null)
                    {
                        Singleton<CameraManager>.Instance.RemoveAllCameras();
                    }
                    GeneralLogicManager.DestroyAll();
                    if (Singleton<LevelScoreManager>.Instance != null)
                    {
                        Singleton<LevelScoreManager>.Destroy();
                    }
                    SceneManager.LoadScene("DevLevelDeploy");
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                GUI.Label(new Rect(20f, Screen.height * 0.85f, 200f, 60f), "GraphicAPI: " + SystemInfo.graphicsDeviceType.ToString());
            }
            else if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.15f, 80f, 30f), "Benchmark"))
            {
                this._toggled = true;
                this.SetEventSystemEnable(false);
            }
        }

        protected void SetEventSystemEnable(bool enabled)
        {
            EventSystem system = UnityEngine.Object.FindObjectOfType<EventSystem>();
            if (system != null)
            {
                system.enabled = enabled;
            }
        }

        private void Start()
        {
            this._switches = new List<BenchmarkSwitch>();
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = enabled => GraphicsSettingUtil.EnablePostFX(enabled, false);
            }
            this._switches.Add(new BenchmarkSwitch("Post FX", <>f__am$cache5));
            this._switches.Add(new BenchmarkSwitch("HDR", new Action<bool>(GraphicsSettingUtil.EnableHDR)));
            this._switches.Add(new BenchmarkSwitch("FXAA", new Action<bool>(GraphicsSettingUtil.EnableFXAA)));
            this._switches.Add(new BenchmarkSwitch("Distortion", new Action<bool>(GraphicsSettingUtil.EnableDistortion)));
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = delegate (bool enabled) {
                    PostFXBase base2 = UnityEngine.Object.FindObjectOfType<PostFXBase>();
                    if (base2 != null)
                    {
                        base2.FastMode = !enabled;
                    }
                };
            }
            this._switches.Add(new BenchmarkSwitch("Not Fast Mode", <>f__am$cache6));
            this._switches.Add(new BufferSizeBenchmarkSwitch());
            this._switches.Add(new BenchmarkSwitch("ColorGrading", new Action<bool>(GraphicsSettingUtil.EnableColorGrading)));
            this._switches.Add(new BenchmarkSwitch("Reflection", new Action<bool>(GraphicsSettingUtil.EnableReflection)));
            this._switches.Add(new BenchmarkSwitch("DynBone", new Action<bool>(GraphicsSettingUtil.EnableDynamicBone)));
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = delegate (bool enabled) {
                    if (enabled)
                    {
                        Application.targetFrameRate = 60;
                    }
                    else
                    {
                        Application.targetFrameRate = 30;
                    }
                };
            }
            this._switches.Add(new BenchmarkSwitch("60FPS", <>f__am$cache7));
            this._switches.Add(new BenchmarkSwitch("UI Camera", this.MakeUICameraEnabledToggler()));
            this._switches.Add(new BenchmarkSwitch("UI Camera Clear", this.MakeUICameraClearToggler()));
            this._switches.Add(new BenchmarkSwitch("UI EventSystem", this.MakeEventSystemEnabledToggler()));
            this._switches.Add(new BenchmarkSwitch("DamageText", this.MakeDamageTextToggler()));
            this._switches.Add(new BenchmarkSwitch("InlevelLock", this.MakeInlevelLockTogger()));
            if (UnityEngine.Object.FindObjectOfType<MonoBasePerpStage>() != null)
            {
                this._switches.Add(new StageBenchmarkSwitch());
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = delegate (bool enabled) {
                    foreach (SkinnedMeshRenderer renderer in UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>())
                    {
                        renderer.enabled = enabled;
                    }
                };
            }
            this._switches.Add(new BenchmarkSwitch("SkinMeshRenderer", <>f__am$cache8));
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = delegate (bool enabled) {
                    foreach (MeshRenderer renderer in UnityEngine.Object.FindObjectsOfType<MeshRenderer>())
                    {
                        renderer.enabled = enabled;
                    }
                };
            }
            this._switches.Add(new BenchmarkSwitch("MeshRenderer", <>f__am$cache9));
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = delegate (bool enabled) {
                    foreach (ParticleSystem system in UnityEngine.Object.FindObjectsOfType<ParticleSystem>())
                    {
                        system.gameObject.SetActive(enabled);
                    }
                    if (Singleton<EffectManager>.Instance != null)
                    {
                        Singleton<EffectManager>.Instance.mute = !enabled;
                    }
                };
            }
            this._switches.Add(new BenchmarkSwitch("ParticleSystem", <>f__am$cacheA));
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = delegate (bool enabled) {
                    foreach (Collider collider in UnityEngine.Object.FindObjectsOfType<Collider>())
                    {
                        collider.enabled = enabled;
                    }
                };
            }
            this._switches.Add(new BenchmarkSwitch("Collider", <>f__am$cacheB));
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = delegate (bool enabled) {
                    foreach (Animator animator in UnityEngine.Object.FindObjectsOfType<Animator>())
                    {
                        animator.enabled = enabled;
                        if (!enabled)
                        {
                            Rigidbody component = animator.GetComponent<Rigidbody>();
                            component.velocity = Vector3.zero;
                            component.angularVelocity = Vector3.zero;
                        }
                    }
                };
            }
            this._switches.Add(new BenchmarkSwitch("Animator", <>f__am$cacheC));
            if (UnityEngine.Object.FindObjectOfType<BehaviorDesigner.Runtime.BehaviorTree>() != null)
            {
                if (<>f__am$cacheD == null)
                {
                    <>f__am$cacheD = delegate (bool enabled) {
                        if (!enabled)
                        {
                            BehaviorManager.instance.UpdateInterval = UpdateIntervalType.Manual;
                        }
                        else
                        {
                            BehaviorManager.instance.UpdateInterval = UpdateIntervalType.EveryFrame;
                        }
                    };
                }
                this._switches.Add(new BenchmarkSwitch("AI", <>f__am$cacheD));
            }
            if (Singleton<EffectManager>.Instance != null)
            {
                if (<>f__am$cacheE == null)
                {
                    <>f__am$cacheE = enabled => Singleton<EffectManager>.Instance.mute = !enabled;
                }
                this._switches.Add(new BenchmarkSwitch("Effects", <>f__am$cacheE));
            }
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = enabled => Time.fixedDeltaTime = !enabled ? 1f : 0.02f;
            }
            this._switches.Add(new BenchmarkSwitch("FixedUpdates", <>f__am$cacheF));
            this._switches.Add(new ResolutionBenchmarkSwitch());
            this._switches.Add(new DofRefSizeBenchmarkSwitch());
            this._switches.Add(new BenchmarkSwitch("Light", this.MakeComponentEnabledToggler<Light>()));
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = delegate (bool enabled) {
                    foreach (SkinnedMeshRenderer renderer in UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>())
                    {
                        if (enabled)
                        {
                            renderer.quality = SkinQuality.Auto;
                        }
                        else
                        {
                            renderer.quality = SkinQuality.Bone1;
                        }
                    }
                };
            }
            this._switches.Add(new BenchmarkSwitch("Quality Bone", <>f__am$cache10));
            this._switches.Add(new MaterialShaderBenchmarkSwitch());
            this._switches.Add(new MaterialTextureBenchmarkSwitch(this._simpleTextures));
            if (UnityEngine.Object.FindObjectOfType<PostFXWithResScale>() != null)
            {
                this._switches.Add(new ResScaleBenchmarkSwitch());
            }
            this._switches.Add(new UseResScaleFXSwitch(this._switches, this));
            this._switches.Add(new NoFPSLimitSwitch());
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = delegate (bool enabled) {
                    PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
                    if (tfx != null)
                    {
                        if (enabled)
                        {
                            tfx.DisableRadialBlur = false;
                            tfx.RadialBlurCenter = new Vector2(0.5f, 0.5f);
                            tfx.RadialBlurScatterScale = 0.3f;
                            tfx.RadialBlurStrenth = 2f;
                        }
                        else
                        {
                            tfx.RadialBlurStrenth = 0f;
                        }
                    }
                };
            }
            this._switches.Add(new BenchmarkSwitch("Radial Blur", <>f__am$cache11));
            if (UnityEngine.Object.FindObjectOfType<PostFX>() != null)
            {
                this._switches.Add(new RadialBlurResBenchmarkSwitch());
            }
            foreach (BenchmarkSwitch switch2 in this._switches)
            {
                switch2.SetEnabled();
            }
        }

        protected void WidgetSwitches()
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.gray;
            bool flag = false;
            for (int i = 0; i < this._switches.Count; i++)
            {
                if ((i % 8) == 0)
                {
                    GUILayout.BeginArea(new Rect(Mathf.Floor((float) (i / 8)) * 240f, 10f, 250f, (float) (Screen.height - 20)), this._style);
                    flag = true;
                }
                this._switches[i].DrawWidgets();
                if ((i % 8) == 7)
                {
                    GUILayout.EndArea();
                    flag = false;
                }
            }
            if (flag)
            {
                GUILayout.EndArea();
            }
        }

        [CompilerGenerated]
        private sealed class <AddUIPageTogglers>c__AnonStoreyBC
        {
            internal Transform[] trsfs;

            internal void <>m__9B(bool enabled)
            {
                foreach (Transform transform in this.trsfs)
                {
                    if (transform != null)
                    {
                        transform.gameObject.SetActive(enabled);
                    }
                }
            }
        }

        public class BenchmarkSwitch
        {
            protected bool _toggled;
            public string name;
            public Action<bool> toggler;

            public BenchmarkSwitch(string name)
            {
                this.name = name;
            }

            public BenchmarkSwitch(string name, Action<bool> toggler)
            {
                this.name = name;
                this.toggler = toggler;
            }

            public virtual void DrawWidgets()
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
                if (GUILayout.Button(string.Format("{0}: {1}", this.name, !this._toggled ? "<color=red>Disabled</color>" : "Enabled"), options))
                {
                    this._toggled = !this._toggled;
                    this.toggler(this._toggled);
                }
            }

            public virtual void SetEnabled()
            {
                this.toggler(true);
                this._toggled = true;
            }
        }

        public class BufferSizeBenchmarkSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            private PostFXBase.InternalBufferSizeEnum[] _bufferSizes;
            private int _ix;

            public BufferSizeBenchmarkSwitch() : base("PostFX Buffer Size")
            {
                this._bufferSizes = new PostFXBase.InternalBufferSizeEnum[] { PostFXBase.InternalBufferSizeEnum.SIZE_128, PostFXBase.InternalBufferSizeEnum.SIZE_256, PostFXBase.InternalBufferSizeEnum.SIZE_512 };
            }

            public override void DrawWidgets()
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
                if (GUILayout.Button("Buffer Size: " + this._bufferSizes[this._ix].ToString(), options))
                {
                    this._ix = (this._ix + 1) % this._bufferSizes.Length;
                    this.SetSize();
                }
            }

            public override void SetEnabled()
            {
                this.SetSize();
            }

            private void SetSize()
            {
                PostFXBase base2 = UnityEngine.Object.FindObjectOfType<PostFXBase>();
                if (base2 != null)
                {
                    base2.internalBufferSize = this._bufferSizes[this._ix];
                }
            }
        }

        public class DofRefSizeBenchmarkSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            private int _ix;
            private Resolution _orig;
            private int[] _sizeList;

            public DofRefSizeBenchmarkSwitch() : base("Dof Size")
            {
                this._sizeList = new int[] { 0x438, 0x3f0, 0x3a8, 0x360, 0x318, 720 };
            }

            public override void DrawWidgets()
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
                if (GUILayout.Button("Dof Size: " + this._sizeList[this._ix].ToString(), options))
                {
                    this._ix = (this._ix + 1) % this._sizeList.Length;
                    this.SetSize();
                }
            }

            public override void SetEnabled()
            {
                this.SetSize();
            }

            private void SetSize()
            {
                MainMenuCamera camera = UnityEngine.Object.FindObjectOfType<MainMenuCamera>();
                if (camera != null)
                {
                    camera.ReferencedBufferHeight = this._sizeList[this._ix];
                }
            }
        }

        public class MaterialShaderBenchmarkSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            public MaterialShaderBenchmarkSwitch() : base("Use Simple Shader")
            {
                base.toggler = new Action<bool>(this.SetSimpleShader);
            }

            public override void DrawWidgets()
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
                if (GUILayout.Button(!base._toggled ? "<color=red>Use Simple Shader</color>" : "Use Simple Shader", options))
                {
                    base.toggler(false);
                    base._toggled = false;
                }
            }

            private void SetSimpleShader(bool enabled)
            {
                if (!enabled && base._toggled)
                {
                    Shader shader = Shader.Find("Unlit/Texture");
                    foreach (Renderer renderer in UnityEngine.Object.FindObjectsOfType<Renderer>())
                    {
                        foreach (Material material in renderer.sharedMaterials)
                        {
                            material.shader = shader;
                        }
                    }
                }
            }
        }

        public class MaterialTextureBenchmarkSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            private Texture2D[] _texes;

            public MaterialTextureBenchmarkSwitch(Texture2D[] simpleTexes) : base("Use Simple Texture")
            {
                base.toggler = new Action<bool>(this.SetSimpleTexture);
                this._texes = simpleTexes;
            }

            public override void DrawWidgets()
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
                if (GUILayout.Button(!base._toggled ? "<color=red>Use Simple Shader&Tex</color>" : "Use Simple Shader&Tex", options))
                {
                    base.toggler(false);
                    base._toggled = false;
                }
            }

            private void SetSimpleTexture(bool enabled)
            {
                if (!enabled && base._toggled)
                {
                    Shader shader = Shader.Find("Unlit/Texture");
                    int num = 0;
                    foreach (Renderer renderer in UnityEngine.Object.FindObjectsOfType<Renderer>())
                    {
                        foreach (Material material in renderer.sharedMaterials)
                        {
                            material.shader = shader;
                            material.mainTexture = this._texes[num % this._texes.Length];
                            num++;
                        }
                    }
                }
            }
        }

        public class NoFPSLimitSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            public NoFPSLimitSwitch() : base("No FPS Limit")
            {
                base.toggler = new Action<bool>(this.NoLimitToggle);
            }

            public override void DrawWidgets()
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
                if (GUILayout.Button(!base._toggled ? "<color=red>No FPS Limit</color>" : "No FPS Limit", options))
                {
                    base.toggler(false);
                    base._toggled = false;
                }
            }

            private void NoLimitToggle(bool enabled)
            {
                if (!enabled && base._toggled)
                {
                    Application.targetFrameRate = 0x5f5e0ff;
                    QualitySettings.vSyncCount = 0;
                }
            }
        }

        public class RadialBlurResBenchmarkSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            private int _ix;
            private PostFXBase.SizeDownScaleEnum[] _scaleList;

            public RadialBlurResBenchmarkSwitch() : base("RB Res Scale")
            {
                this._scaleList = new PostFXBase.SizeDownScaleEnum[] { PostFXBase.SizeDownScaleEnum.Div_5, PostFXBase.SizeDownScaleEnum.Div_6, PostFXBase.SizeDownScaleEnum.Div_7, PostFXBase.SizeDownScaleEnum.Div_8, PostFXBase.SizeDownScaleEnum.Div_1, PostFXBase.SizeDownScaleEnum.Div_2, PostFXBase.SizeDownScaleEnum.Div_3, PostFXBase.SizeDownScaleEnum.Div_4 };
            }

            public override void DrawWidgets()
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
                if (GUILayout.Button("Res Scale: " + this._scaleList[this._ix].ToString(), options))
                {
                    this._ix = (this._ix + 1) % this._scaleList.Length;
                    this.SetScale();
                }
            }

            public override void SetEnabled()
            {
                this.SetScale();
            }

            private void SetScale()
            {
                PostFX tfx = UnityEngine.Object.FindObjectOfType<PostFX>();
                if (tfx != null)
                {
                    tfx.RadialBlurDownScale = this._scaleList[this._ix];
                }
            }
        }

        public class ResolutionBenchmarkSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            private int _ix;
            private Resolution _orig;
            private float[] _scaleList;

            public ResolutionBenchmarkSwitch() : base("Res Scale")
            {
                this._scaleList = new float[] { 1f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f };
                this._orig = Screen.currentResolution;
            }

            public override void DrawWidgets()
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
                if (GUILayout.Button("Resolution: " + this._scaleList[this._ix].ToString(), options))
                {
                    this._ix = (this._ix + 1) % this._scaleList.Length;
                    this.SetResolution();
                }
            }

            public override void SetEnabled()
            {
                this.SetResolution();
            }

            private void SetResolution()
            {
                float num = this._scaleList[this._ix];
                Screen.SetResolution((int) (this._orig.width * num), (int) (this._orig.height * num), Screen.fullScreen);
            }
        }

        public class ResScaleBenchmarkSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            private int _ix;
            private PostFXWithResScale.CAMERA_RES_SCALE[] _scaleList;

            public ResScaleBenchmarkSwitch() : base("Res Scale")
            {
                this._scaleList = new PostFXWithResScale.CAMERA_RES_SCALE[] { PostFXWithResScale.CAMERA_RES_SCALE.RES_100, PostFXWithResScale.CAMERA_RES_SCALE.RES_90, PostFXWithResScale.CAMERA_RES_SCALE.RES_80, PostFXWithResScale.CAMERA_RES_SCALE.RES_70, PostFXWithResScale.CAMERA_RES_SCALE.RES_60, PostFXWithResScale.CAMERA_RES_SCALE.RES_50, PostFXWithResScale.CAMERA_RES_SCALE.RES_25 };
            }

            public override void DrawWidgets()
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
                if (GUILayout.Button("Res Scale: " + this._scaleList[this._ix].ToString(), options))
                {
                    this._ix = (this._ix + 1) % this._scaleList.Length;
                    this.SetScale();
                }
            }

            public override void SetEnabled()
            {
                this.SetScale();
            }

            private void SetScale()
            {
                PostFXWithResScale scale = UnityEngine.Object.FindObjectOfType<PostFXWithResScale>();
                if (scale != null)
                {
                    scale.CameraResScale = this._scaleList[this._ix];
                }
            }
        }

        public class StageBenchmarkSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            private GameObject _stageGO;

            public StageBenchmarkSwitch() : base("Stage")
            {
                base.toggler = new Action<bool>(this.SetStageEnable);
            }

            private void SetStageEnable(bool enabled)
            {
                if (this._stageGO == null)
                {
                    this._stageGO = UnityEngine.Object.FindObjectOfType<MonoBasePerpStage>().gameObject;
                }
                this._stageGO.SetActive(enabled);
            }
        }

        public class UseResScaleFXSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            private MonoBenchmarkSwitches _bench;
            private List<MonoBenchmarkSwitches.BenchmarkSwitch> _switches;

            public UseResScaleFXSwitch(List<MonoBenchmarkSwitches.BenchmarkSwitch> switches, MonoBenchmarkSwitches bench) : base("Use Res Scale")
            {
                this._switches = switches;
                this._bench = bench;
            }

            [DebuggerHidden]
            private IEnumerator ChangePostFXIter()
            {
                return new <ChangePostFXIter>c__Iterator31 { <>f__this = this };
            }

            public override void DrawWidgets()
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
                if (GUILayout.Button(!base._toggled ? "Use Res Scale" : "<color=red>Use Res Scale</color>", options) && !base._toggled)
                {
                    base._toggled = true;
                    this._bench.StartCoroutine(this.ChangePostFXIter());
                }
            }

            public override void SetEnabled()
            {
            }

            [CompilerGenerated]
            private sealed class <ChangePostFXIter>c__Iterator31 : IEnumerator, IDisposable, IEnumerator<object>
            {
                internal object $current;
                internal int $PC;
                internal List<MonoBenchmarkSwitches.BenchmarkSwitch>.Enumerator <$s_1139>__15;
                internal MonoBenchmarkSwitches.UseResScaleFXSwitch <>f__this;
                internal Shader <bright>__8;
                internal Shader <distortionApply>__2;
                internal Shader <distortionNorm>__3;
                internal Shader <down>__7;
                internal Shader <down4x>__6;
                internal Shader <drawAlpha>__5;
                internal Shader <drawDepth>__4;
                internal Shader <gaussComp>__9;
                internal Shader <glareCompo>__10;
                internal Shader <mp1>__11;
                internal Shader <mp2>__12;
                internal Shader <mp3>__13;
                internal PostFXBase <postFX>__0;
                internal GameObject <postFXGo>__1;
                internal PostFXWithResScale <resScale>__14;
                internal MonoBenchmarkSwitches.BenchmarkSwitch <swc>__16;

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
                            this.$current = new WaitForEndOfFrame();
                            this.$PC = 1;
                            goto Label_02E9;

                        case 1:
                            this.<postFX>__0 = UnityEngine.Object.FindObjectOfType<PostFXBase>();
                            if (this.<postFX>__0 != null)
                            {
                                this.<postFXGo>__1 = this.<postFX>__0.gameObject;
                                this.<distortionApply>__2 = this.<postFX>__0.DistortionApplyShader;
                                this.<distortionNorm>__3 = this.<postFX>__0.DistortionMapNormShader;
                                this.<drawDepth>__4 = this.<postFX>__0.DrawDepthShader;
                                this.<drawAlpha>__5 = this.<postFX>__0.DrawAlphaShader;
                                this.<down4x>__6 = this.<postFX>__0.DownSample4XShader;
                                this.<down>__7 = this.<postFX>__0.DownSampleShader;
                                this.<bright>__8 = this.<postFX>__0.BrightPassExShader;
                                this.<gaussComp>__9 = this.<postFX>__0.GaussCompositionExShader;
                                this.<glareCompo>__10 = this.<postFX>__0.GlareCompositionExShader;
                                this.<mp1>__11 = this.<postFX>__0.MultipleGaussPassFilterShader_128;
                                this.<mp2>__12 = this.<postFX>__0.MultipleGaussPassFilterShader_256;
                                this.<mp3>__13 = this.<postFX>__0.MultipleGaussPassFilterShader_512;
                                UnityEngine.Object.DestroyImmediate(this.<postFX>__0);
                                this.$current = null;
                                this.$PC = 2;
                                goto Label_02E9;
                            }
                            break;

                        case 2:
                            this.<resScale>__14 = this.<postFXGo>__1.AddComponent<PostFXWithResScale>();
                            this.<resScale>__14.DistortionApplyShader = this.<distortionApply>__2;
                            this.<resScale>__14.DistortionMapNormShader = this.<distortionNorm>__3;
                            this.<resScale>__14.DrawDepthShader = this.<drawDepth>__4;
                            this.<resScale>__14.DrawAlphaShader = this.<drawAlpha>__5;
                            this.<resScale>__14.DownSample4XShader = this.<down4x>__6;
                            this.<resScale>__14.DownSampleShader = this.<down>__7;
                            this.<resScale>__14.BrightPassExShader = this.<bright>__8;
                            this.<resScale>__14.GaussCompositionExShader = this.<gaussComp>__9;
                            this.<resScale>__14.GlareCompositionExShader = this.<glareCompo>__10;
                            this.<resScale>__14.MultipleGaussPassFilterShader_128 = this.<mp1>__11;
                            this.<resScale>__14.MultipleGaussPassFilterShader_256 = this.<mp2>__12;
                            this.<resScale>__14.MultipleGaussPassFilterShader_512 = this.<mp3>__13;
                            this.<resScale>__14.enabled = false;
                            this.$current = null;
                            this.$PC = 3;
                            goto Label_02E9;

                        case 3:
                            this.<resScale>__14.enabled = true;
                            this.<$s_1139>__15 = this.<>f__this._switches.GetEnumerator();
                            try
                            {
                                while (this.<$s_1139>__15.MoveNext())
                                {
                                    this.<swc>__16 = this.<$s_1139>__15.Current;
                                    if (this.<swc>__16 is MonoBenchmarkSwitches.ResScaleBenchmarkSwitch)
                                    {
                                        break;
                                    }
                                }
                            }
                            finally
                            {
                                this.<$s_1139>__15.Dispose();
                            }
                            this.<>f__this._switches.Add(new MonoBenchmarkSwitches.ResScaleBenchmarkSwitch());
                            this.$PC = -1;
                            break;
                    }
                    return false;
                Label_02E9:
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
}

