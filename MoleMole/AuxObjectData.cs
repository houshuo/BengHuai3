namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class AuxObjectData
    {
        private static Dictionary<string, string> _auxObjectDict;

        public static bool ContainAuxObjectPrefabPath(string auxObjectName)
        {
            return _auxObjectDict.ContainsKey(auxObjectName);
        }

        public static string GetAuxObjectPrefabPath(string auxObjectName)
        {
            return _auxObjectDict[auxObjectName];
        }

        public static void ReloadFromFile()
        {
            _auxObjectDict = new Dictionary<string, string>();
            string[] auxEntryPathes = GlobalDataManager.metaConfig.auxEntryPathes;
            for (int i = 0; i < auxEntryPathes.Length; i++)
            {
                ConfigAuxObjectRegistry registry = ConfigUtil.LoadConfig<ConfigAuxObjectRegistry>(auxEntryPathes[i]);
                if (registry.entries != null)
                {
                    for (int j = 0; j < registry.entries.Length; j++)
                    {
                        AuxObjectEntry entry = registry.entries[j];
                        _auxObjectDict.Add(entry.name, entry.GetPrefabPath());
                    }
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator6 { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator6 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal AsyncAssetRequst <asyncRequest>__3;
            internal ConfigAuxObjectRegistry <auxRegistryConfig>__4;
            internal string[] <auxRegistryPathes>__0;
            internal AuxObjectEntry <entry>__6;
            internal int <ix>__2;
            internal int <jx>__5;
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
                        AuxObjectData._auxObjectDict = new Dictionary<string, string>();
                        this.<auxRegistryPathes>__0 = GlobalDataManager.metaConfig.auxEntryPathes;
                        this.<step>__1 = this.progressSpan / ((float) this.<auxRegistryPathes>__0.Length);
                        this.<ix>__2 = 0;
                        goto Label_01C3;

                    case 1:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__1);
                        }
                        this.<auxRegistryConfig>__4 = (ConfigAuxObjectRegistry) this.<asyncRequest>__3.asset;
                        SuperDebug.VeryImportantAssert(this.<auxRegistryConfig>__4 != null, "auxRegistryConfig is null auxRegistryPath :" + this.<auxRegistryPathes>__0[this.<ix>__2]);
                        if ((this.<auxRegistryConfig>__4 != null) && (this.<auxRegistryConfig>__4.entries != null))
                        {
                            this.<jx>__5 = 0;
                            while (this.<jx>__5 < this.<auxRegistryConfig>__4.entries.Length)
                            {
                                this.<entry>__6 = this.<auxRegistryConfig>__4.entries[this.<jx>__5];
                                AuxObjectData._auxObjectDict.Add(this.<entry>__6.name, this.<entry>__6.GetPrefabPath());
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
                if (this.<ix>__2 < this.<auxRegistryPathes>__0.Length)
                {
                    this.<asyncRequest>__3 = ConfigUtil.LoadConfigAsync(this.<auxRegistryPathes>__0[this.<ix>__2], BundleType.RESOURCE_FILE);
                    SuperDebug.VeryImportantAssert(this.<asyncRequest>__3 != null, "assetRequest is null auxRegistryPath :" + this.<auxRegistryPathes>__0[this.<ix>__2]);
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

