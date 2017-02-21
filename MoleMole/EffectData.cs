namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class EffectData
    {
        private static Dictionary<string, EffectPattern[]> _effectGroupDict;
        private static Dictionary<string, EffectPattern> _effectPatternDict;
        private const string EFFECT_PREFAB_PATH = "Effect/";

        public static Dictionary<string, EffectPattern> GetAllEffectPatterns()
        {
            return _effectPatternDict;
        }

        public static EffectPattern[] GetEffectGroupPatterns(string effectGroupName)
        {
            return _effectGroupDict[effectGroupName];
        }

        public static EffectPattern GetEffectPattern(string patternName)
        {
            return _effectPatternDict[patternName];
        }

        public static string GetPrefabResPath(string effectPath)
        {
            return ("Effect/" + effectPath);
        }

        public static bool HasEffectPattern(string patternName)
        {
            return _effectGroupDict.ContainsKey(patternName);
        }

        public static void ReloadFromFile()
        {
            _effectPatternDict = new Dictionary<string, EffectPattern>();
            _effectGroupDict = new Dictionary<string, EffectPattern[]>();
            string[] effectPatternPathes = GlobalDataManager.metaConfig.effectPatternPathes;
            for (int i = 0; i < effectPatternPathes.Length; i++)
            {
                ConfigEffectPattern pattern = ConfigUtil.LoadConfig<ConfigEffectPattern>(effectPatternPathes[i]);
                if (pattern.patterns != null)
                {
                    _effectGroupDict.Add(pattern.groupName, pattern.patterns);
                    for (int j = 0; j < pattern.patterns.Length; j++)
                    {
                        EffectPattern pattern2 = pattern.patterns[j];
                        _effectPatternDict.Add(pattern2.name, pattern2);
                    }
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <ReloadFromFileAsync>c__IteratorB { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__IteratorB : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal AsyncAssetRequst <asyncRequest>__3;
            internal string[] <effectPatternPathes>__0;
            internal int <ix>__2;
            internal int <jx>__5;
            internal EffectPattern <pattern>__6;
            internal ConfigEffectPattern <patternConfig>__4;
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
                        EffectData._effectPatternDict = new Dictionary<string, EffectPattern>();
                        EffectData._effectGroupDict = new Dictionary<string, EffectPattern[]>();
                        this.<effectPatternPathes>__0 = GlobalDataManager.metaConfig.effectPatternPathes;
                        this.<step>__1 = this.progressSpan / ((float) this.<effectPatternPathes>__0.Length);
                        this.<ix>__2 = 0;
                        goto Label_01E8;

                    case 1:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__1);
                        }
                        this.<patternConfig>__4 = (ConfigEffectPattern) this.<asyncRequest>__3.asset;
                        SuperDebug.VeryImportantAssert(this.<patternConfig>__4 != null, "patternConfig is null effectPath :" + this.<effectPatternPathes>__0[this.<ix>__2]);
                        if ((this.<patternConfig>__4 != null) && (this.<patternConfig>__4.patterns != null))
                        {
                            EffectData._effectGroupDict.Add(this.<patternConfig>__4.groupName, this.<patternConfig>__4.patterns);
                            this.<jx>__5 = 0;
                            while (this.<jx>__5 < this.<patternConfig>__4.patterns.Length)
                            {
                                this.<pattern>__6 = this.<patternConfig>__4.patterns[this.<jx>__5];
                                EffectData._effectPatternDict.Add(this.<pattern>__6.name, this.<pattern>__6);
                                this.<jx>__5++;
                            }
                        }
                        break;

                    default:
                        goto Label_0202;
                }
            Label_01DA:
                this.<ix>__2++;
            Label_01E8:
                if (this.<ix>__2 < this.<effectPatternPathes>__0.Length)
                {
                    this.<asyncRequest>__3 = ConfigUtil.LoadConfigAsync(this.<effectPatternPathes>__0[this.<ix>__2], BundleType.RESOURCE_FILE);
                    SuperDebug.VeryImportantAssert(this.<asyncRequest>__3 != null, "assetRequest is null effectPath :" + this.<effectPatternPathes>__0[this.<ix>__2]);
                    if (this.<asyncRequest>__3 != null)
                    {
                        this.$current = this.<asyncRequest>__3.operation;
                        this.$PC = 1;
                        return true;
                    }
                    goto Label_01DA;
                }
                this.$PC = -1;
            Label_0202:
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

