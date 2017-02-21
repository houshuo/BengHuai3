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

    public static class TouchPatternData
    {
        private static Dictionary<string, List<TouchPatternItem>> _touchPatternDict;

        public static List<TouchPatternItem> GetTouchPatternList(string characterName)
        {
            if (_touchPatternDict.ContainsKey(characterName))
            {
                return _touchPatternDict[characterName];
            }
            return null;
        }

        public static void ReloadFromFile()
        {
            _touchPatternDict = new Dictionary<string, List<TouchPatternItem>>();
            string[] touchPatternPathes = GlobalDataManager.metaConfig.touchPatternPathes;
            for (int i = 0; i < touchPatternPathes.Length; i++)
            {
                ConfigTouchPattern pattern = ConfigUtil.LoadConfig<ConfigTouchPattern>(touchPatternPathes[i]);
                if (_touchPatternDict.ContainsKey(pattern.name))
                {
                    UnityEngine.Debug.LogError(string.Format("duplicate touch pattern name : {0}", pattern.name));
                }
                else if (pattern.touchPatternItems != null)
                {
                    List<TouchPatternItem> list = new List<TouchPatternItem>();
                    for (int j = 0; j < pattern.touchPatternItems.Length; j++)
                    {
                        list.Add(pattern.touchPatternItems[j]);
                    }
                    _touchPatternDict[pattern.name] = list;
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator6D { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator6D : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal AsyncAssetRequst <asyncRequest>__3;
            internal int <ix>__2;
            internal int <jx>__6;
            internal ConfigTouchPattern <patternConfig>__4;
            internal List<TouchPatternItem> <patternList>__5;
            internal float <step>__1;
            internal string[] <touchPatternPathes>__0;
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
                        TouchPatternData._touchPatternDict = new Dictionary<string, List<TouchPatternItem>>();
                        this.<touchPatternPathes>__0 = GlobalDataManager.metaConfig.touchPatternPathes;
                        this.<step>__1 = this.progressSpan / ((float) this.<touchPatternPathes>__0.Length);
                        this.<ix>__2 = 0;
                        goto Label_020C;

                    case 1:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__1);
                        }
                        this.<patternConfig>__4 = (ConfigTouchPattern) this.<asyncRequest>__3.asset;
                        SuperDebug.VeryImportantAssert(this.<patternConfig>__4 != null, "assetRequest is null touchPatternPath :" + this.<touchPatternPathes>__0[this.<ix>__2]);
                        if (this.<patternConfig>__4 != null)
                        {
                            if (TouchPatternData._touchPatternDict.ContainsKey(this.<patternConfig>__4.name))
                            {
                                UnityEngine.Debug.LogError(string.Format("duplicate touch pattern name : {0}", this.<patternConfig>__4.name));
                            }
                            else if (this.<patternConfig>__4.touchPatternItems != null)
                            {
                                this.<patternList>__5 = new List<TouchPatternItem>();
                                this.<jx>__6 = 0;
                                while (this.<jx>__6 < this.<patternConfig>__4.touchPatternItems.Length)
                                {
                                    this.<patternList>__5.Add(this.<patternConfig>__4.touchPatternItems[this.<jx>__6]);
                                    this.<jx>__6++;
                                }
                                TouchPatternData._touchPatternDict[this.<patternConfig>__4.name] = this.<patternList>__5;
                            }
                        }
                        break;

                    default:
                        goto Label_0226;
                }
            Label_01FE:
                this.<ix>__2++;
            Label_020C:
                if (this.<ix>__2 < this.<touchPatternPathes>__0.Length)
                {
                    this.<asyncRequest>__3 = ConfigUtil.LoadConfigAsync(this.<touchPatternPathes>__0[this.<ix>__2], BundleType.RESOURCE_FILE);
                    SuperDebug.VeryImportantAssert(this.<asyncRequest>__3 != null, "assetRequest is null touchPatternPath :" + this.<touchPatternPathes>__0[this.<ix>__2]);
                    if (this.<asyncRequest>__3 != null)
                    {
                        this.$current = this.<asyncRequest>__3.operation;
                        this.$PC = 1;
                        return true;
                    }
                    goto Label_01FE;
                }
                this.$PC = -1;
            Label_0226:
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

