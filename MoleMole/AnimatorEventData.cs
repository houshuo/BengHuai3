namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class AnimatorEventData
    {
        private static Dictionary<string, AnimatorEventPattern> _animatorEventPatternDict;

        public static Dictionary<string, AnimatorEventPattern> GetAllAnimatorEventPatterns()
        {
            return _animatorEventPatternDict;
        }

        public static AnimatorEventPattern GetAnimatorEventPattern(string patternName)
        {
            if (!_animatorEventPatternDict.ContainsKey(patternName))
            {
                return null;
            }
            return _animatorEventPatternDict[patternName];
        }

        public static ConfigAnimatorEventPattern[] GetConfigs()
        {
            List<ConfigAnimatorEventPattern> list = new List<ConfigAnimatorEventPattern>();
            string[] animatorEventPatternPathes = GlobalDataManager.metaConfig.animatorEventPatternPathes;
            for (int i = 0; i < animatorEventPatternPathes.Length; i++)
            {
                ConfigAnimatorEventPattern item = ConfigUtil.LoadConfig<ConfigAnimatorEventPattern>(animatorEventPatternPathes[i]);
                list.Add(item);
            }
            return list.ToArray();
        }

        public static void ReloadFromFile()
        {
            _animatorEventPatternDict = new Dictionary<string, AnimatorEventPattern>();
            string[] animatorEventPatternPathes = GlobalDataManager.metaConfig.animatorEventPatternPathes;
            for (int i = 0; i < animatorEventPatternPathes.Length; i++)
            {
                ConfigAnimatorEventPattern pattern = ConfigUtil.LoadConfig<ConfigAnimatorEventPattern>(animatorEventPatternPathes[i]);
                if (pattern.patterns != null)
                {
                    for (int j = 0; j < pattern.patterns.Length; j++)
                    {
                        AnimatorEventPattern pattern2 = pattern.patterns[j];
                        _animatorEventPatternDict.Add(pattern2.name, pattern2);
                    }
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator5 { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator5 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal string[] <animatorEventPatternPathes>__0;
            internal AsyncAssetRequst <asyncRequest>__3;
            internal int <ix>__2;
            internal int <jx>__5;
            internal AnimatorEventPattern <pattern>__6;
            internal ConfigAnimatorEventPattern <patternConfig>__4;
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
                        AnimatorEventData._animatorEventPatternDict = new Dictionary<string, AnimatorEventPattern>();
                        this.<animatorEventPatternPathes>__0 = GlobalDataManager.metaConfig.animatorEventPatternPathes;
                        this.<step>__1 = this.progressSpan / ((float) this.<animatorEventPatternPathes>__0.Length);
                        this.<ix>__2 = 0;
                        goto Label_01BE;

                    case 1:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__1);
                        }
                        this.<patternConfig>__4 = (ConfigAnimatorEventPattern) this.<asyncRequest>__3.asset;
                        SuperDebug.VeryImportantAssert(this.<patternConfig>__4 != null, "patternConfig is null animatorEventPatternPath :" + this.<animatorEventPatternPathes>__0[this.<ix>__2]);
                        if ((this.<patternConfig>__4 != null) && (this.<patternConfig>__4.patterns != null))
                        {
                            this.<jx>__5 = 0;
                            while (this.<jx>__5 < this.<patternConfig>__4.patterns.Length)
                            {
                                this.<pattern>__6 = this.<patternConfig>__4.patterns[this.<jx>__5];
                                AnimatorEventData._animatorEventPatternDict.Add(this.<pattern>__6.name, this.<pattern>__6);
                                this.<jx>__5++;
                            }
                        }
                        break;

                    default:
                        goto Label_01D8;
                }
            Label_01B0:
                this.<ix>__2++;
            Label_01BE:
                if (this.<ix>__2 < this.<animatorEventPatternPathes>__0.Length)
                {
                    this.<asyncRequest>__3 = ConfigUtil.LoadConfigAsync(this.<animatorEventPatternPathes>__0[this.<ix>__2], BundleType.RESOURCE_FILE);
                    SuperDebug.VeryImportantAssert(this.<asyncRequest>__3 != null, "assetRequest is null animatorEventPatternPath :" + this.<animatorEventPatternPathes>__0[this.<ix>__2]);
                    if (this.<asyncRequest>__3 != null)
                    {
                        this.$current = this.<asyncRequest>__3.operation;
                        this.$PC = 1;
                        return true;
                    }
                    goto Label_01B0;
                }
                this.$PC = -1;
            Label_01D8:
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

