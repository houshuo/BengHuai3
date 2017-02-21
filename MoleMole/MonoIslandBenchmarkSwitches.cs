namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoIslandBenchmarkSwitches : MonoBenchmarkSwitches
    {
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache5;
        public GameObject[] hiddenGameObjects;

        private void OnGUI()
        {
            if (base._toggled)
            {
                base.WidgetSwitches();
                if (GUI.Button(new Rect(Screen.width * 0.85f, Screen.height * 0.85f, 60f, 60f), "CLOSE"))
                {
                    base._toggled = false;
                    EasyTouch touch = UnityEngine.Object.FindObjectOfType<EasyTouch>();
                    if (touch != null)
                    {
                        touch.enable = true;
                    }
                    base.SetEventSystemEnable(true);
                }
                GUI.Label(new Rect(20f, Screen.height * 0.85f, 200f, 60f), "GraphicAPI: " + SystemInfo.graphicsDeviceType.ToString());
            }
            else if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.15f, 80f, 30f), "Benchmark"))
            {
                base._toggled = true;
                EasyTouch touch2 = UnityEngine.Object.FindObjectOfType<EasyTouch>();
                if (touch2 != null)
                {
                    touch2.enable = false;
                }
                base.SetEventSystemEnable(false);
            }
        }

        private void Start()
        {
            base._switches = new List<MonoBenchmarkSwitches.BenchmarkSwitch>();
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = enabled => GraphicsSettingUtil.EnablePostFX(enabled, false);
            }
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("Post FX", <>f__am$cache1));
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("HDR", new Action<bool>(GraphicsSettingUtil.EnableHDR)));
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("FXAA", new Action<bool>(GraphicsSettingUtil.EnableFXAA)));
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("Distortion", new Action<bool>(GraphicsSettingUtil.EnableDistortion)));
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = enabled => UnityEngine.Object.FindObjectOfType<PostFXBase>().FastMode = !enabled;
            }
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("Not Fast Mode", <>f__am$cache2));
            base._switches.Add(new MonoBenchmarkSwitches.BufferSizeBenchmarkSwitch());
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("ColorGrading", new Action<bool>(GraphicsSettingUtil.EnableColorGrading)));
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("Reflection", new Action<bool>(GraphicsSettingUtil.EnableReflection)));
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("DynBone", new Action<bool>(GraphicsSettingUtil.EnableDynamicBone)));
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = delegate (bool enabled) {
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
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("60FPS", <>f__am$cache3));
            if (Singleton<EffectManager>.Instance != null)
            {
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = enabled => Singleton<EffectManager>.Instance.mute = !enabled;
                }
                base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("Effects", <>f__am$cache4));
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = enabled => Time.fixedDeltaTime = !enabled ? 1f : 0.02f;
            }
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("FixedUpdates", <>f__am$cache5));
            base._switches.Add(new MonoBenchmarkSwitches.ResolutionBenchmarkSwitch());
            if (UnityEngine.Object.FindObjectOfType<PostFXWithResScale>() != null)
            {
                base._switches.Add(new MonoBenchmarkSwitches.ResScaleBenchmarkSwitch());
            }
            base._switches.Add(new MonoBenchmarkSwitches.UseResScaleFXSwitch(base._switches, this));
            base._switches.Add(new MonoBenchmarkSwitches.NoFPSLimitSwitch());
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("UI Camera", base.MakeUICameraEnabledToggler()));
            base._switches.Add(new MonoBenchmarkSwitches.BenchmarkSwitch("UI Camera Clear", base.MakeUICameraClearToggler()));
            foreach (GameObject obj2 in this.hiddenGameObjects)
            {
                base._switches.Add(new HideObjectBenchmarkSwitch(obj2));
            }
            foreach (MonoBenchmarkSwitches.BenchmarkSwitch switch2 in base._switches)
            {
                if (switch2.name != "Post FX")
                {
                    switch2.SetEnabled();
                }
            }
        }

        public class HideObjectBenchmarkSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            private GameObject _obj;

            public HideObjectBenchmarkSwitch(GameObject obj) : base("Hide " + obj.name)
            {
                this._obj = obj;
                base.toggler = new Action<bool>(this.HideObject);
            }

            private void HideObject(bool isHide)
            {
                this._obj.SetActive(!isHide);
            }

            public override void SetEnabled()
            {
                base.toggler(false);
                base._toggled = false;
            }
        }

        public class HideObjectListBenchmarkSwitch : MonoBenchmarkSwitches.BenchmarkSwitch
        {
            private int _ix;
            private GameObject[] _objList;

            public HideObjectListBenchmarkSwitch(GameObject[] objList) : base("Hide Object List")
            {
                this._objList = objList;
                this._ix = objList.Length;
            }

            public override void DrawWidgets()
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
                if (GUILayout.Button("Hide Object: " + ((this._ix >= this._objList.Length) ? "None" : this._objList[this._ix].name), options))
                {
                    this._ix = (this._ix + 1) % (this._objList.Length + 1);
                    this.HideObjest();
                }
            }

            private void HideObjest()
            {
                foreach (GameObject obj2 in this._objList)
                {
                    obj2.SetActive(true);
                }
                if (this._ix < this._objList.Length)
                {
                    this._objList[this._ix].SetActive(false);
                }
            }

            public override void SetEnabled()
            {
                this.HideObjest();
            }
        }
    }
}

