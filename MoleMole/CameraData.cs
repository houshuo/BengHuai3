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

    public static class CameraData
    {
        private static Dictionary<string, AnimationCurve> _cameraCurveDict;
        private const string IN_LEVEL_UI_CAMERA_PREFAB_PATH = "Entities/Camera/InLevelUICamera";
        public const uint IN_LEVEL_UI_CAMERA_TYPE = 2;
        private const string MAIN_CAMERA_PREFAB_PATH = "Entities/Camera/MainCamera";
        public const uint MAIN_CAMERA_TYPE = 1;
        private const string STAGE_ORTH_CAMERA_PREFAB_PATH = "Entities/Camera/OrthStageCamera";
        public const uint STAGE_ORTH_CAMERA_TYPE = 3;

        public static AnimationCurve GetCameraCurveByName(string name)
        {
            if (!string.IsNullOrEmpty(name) && _cameraCurveDict.ContainsKey(name))
            {
                return _cameraCurveDict[name];
            }
            return null;
        }

        public static string GetPrefabResPath(uint type)
        {
            switch (type)
            {
                case 1:
                    return "Entities/Camera/MainCamera";

                case 2:
                    return "Entities/Camera/InLevelUICamera";

                case 3:
                    return "Entities/Camera/OrthStageCamera";
            }
            throw new Exception("Invalid Type or State!");
        }

        public static void ReloadFromFile()
        {
            _cameraCurveDict = new Dictionary<string, AnimationCurve>();
            string[] cameraCurvePatternPathes = GlobalDataManager.metaConfig.cameraCurvePatternPathes;
            for (int i = 0; i < cameraCurvePatternPathes.Length; i++)
            {
                ConfigCameraCurvePattern pattern = ConfigUtil.LoadConfig<ConfigCameraCurvePattern>(cameraCurvePatternPathes[i]);
                if (pattern != null)
                {
                    for (int j = 0; j < pattern.patterns.Length; j++)
                    {
                        CameraCurvePattern pattern2 = pattern.patterns[j];
                        _cameraCurveDict.Add(pattern2.name, pattern2.animationCurve);
                    }
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator8 { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator8 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal AsyncAssetRequst <asyncRequest>__3;
            internal ConfigCameraCurvePattern <cameraCurvePatternConfig>__4;
            internal string[] <cameraCurvePatternPathes>__0;
            internal int <ix>__2;
            internal int <jx>__5;
            internal CameraCurvePattern <pattern>__6;
            internal float <step>__1;
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
                        CameraData._cameraCurveDict = new Dictionary<string, AnimationCurve>();
                        this.<cameraCurvePatternPathes>__0 = GlobalDataManager.metaConfig.cameraCurvePatternPathes;
                        this.<step>__1 = this.progressSpan / ((float) this.<cameraCurvePatternPathes>__0.Length);
                        this.<ix>__2 = 0;
                        goto Label_01C3;

                    case 1:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__1);
                        }
                        this.<cameraCurvePatternConfig>__4 = (ConfigCameraCurvePattern) this.<asyncRequest>__3.asset;
                        SuperDebug.VeryImportantAssert(this.<cameraCurvePatternConfig>__4 != null, "cameraCurvePatternConfig is null cameraCurvePatternPath :" + this.<cameraCurvePatternPathes>__0[this.<ix>__2]);
                        if ((this.<cameraCurvePatternConfig>__4 != null) && (this.<cameraCurvePatternConfig>__4.patterns != null))
                        {
                            this.<jx>__5 = 0;
                            while (this.<jx>__5 < this.<cameraCurvePatternConfig>__4.patterns.Length)
                            {
                                this.<pattern>__6 = this.<cameraCurvePatternConfig>__4.patterns[this.<jx>__5];
                                CameraData._cameraCurveDict.Add(this.<pattern>__6.name, this.<pattern>__6.animationCurve);
                                this.<jx>__5++;
                            }
                        }
                        break;

                    default:
                        goto Label_01DD;
                }
            Label_01B5:
                this.<ix>__2++;
            Label_01C3:
                if (this.<ix>__2 < this.<cameraCurvePatternPathes>__0.Length)
                {
                    this.<asyncRequest>__3 = ConfigUtil.LoadConfigAsync(this.<cameraCurvePatternPathes>__0[this.<ix>__2], BundleType.RESOURCE_FILE);
                    SuperDebug.VeryImportantAssert(this.<asyncRequest>__3 != null, "assetRequest is null cameraCurvePatternPath :" + this.<cameraCurvePatternPathes>__0[this.<ix>__2]);
                    if (this.<asyncRequest>__3 != null)
                    {
                        this.$current = this.<asyncRequest>__3.operation;
                        this.$PC = 1;
                        return true;
                    }
                    goto Label_01B5;
                }
                this.$PC = -1;
            Label_01DD:
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

