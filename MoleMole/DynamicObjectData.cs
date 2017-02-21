namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class DynamicObjectData
    {
        public static Dictionary<string, string> dynamicObjectDict;

        public static ConfigDynamicObjectRegistry GetDynamicObjectRegistry(string registryPath)
        {
            string[] dynamicObjectRegistryPathes = GlobalDataManager.metaConfig.dynamicObjectRegistryPathes;
            for (int i = 0; i < dynamicObjectRegistryPathes.Length; i++)
            {
                if (dynamicObjectRegistryPathes[i] == registryPath)
                {
                    return ConfigUtil.LoadConfig<ConfigDynamicObjectRegistry>(dynamicObjectRegistryPathes[i]);
                }
            }
            return null;
        }

        public static void ReloadFromFile()
        {
            dynamicObjectDict = new Dictionary<string, string>();
            string[] dynamicObjectRegistryPathes = GlobalDataManager.metaConfig.dynamicObjectRegistryPathes;
            for (int i = 0; i < dynamicObjectRegistryPathes.Length; i++)
            {
                ConfigDynamicObjectRegistry registry = ConfigUtil.LoadConfig<ConfigDynamicObjectRegistry>(dynamicObjectRegistryPathes[i]);
                if (registry.entries != null)
                {
                    for (int j = 0; j < registry.entries.Length; j++)
                    {
                        DynamicObjectEntry entry = registry.entries[j];
                        dynamicObjectDict.Add(entry.name, entry.prefabPath);
                    }
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <ReloadFromFileAsync>c__IteratorA { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__IteratorA : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal AsyncAssetRequst <asyncRequest>__3;
            internal string[] <dynamicObjectPathes>__0;
            internal ConfigDynamicObjectRegistry <dynamicObjectRegistryConfig>__4;
            internal DynamicObjectEntry <entry>__6;
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
                        DynamicObjectData.dynamicObjectDict = new Dictionary<string, string>();
                        this.<dynamicObjectPathes>__0 = GlobalDataManager.metaConfig.dynamicObjectRegistryPathes;
                        this.<step>__1 = this.progressSpan / ((float) this.<dynamicObjectPathes>__0.Length);
                        this.<ix>__2 = 0;
                        goto Label_01C3;

                    case 1:
                        this.<dynamicObjectRegistryConfig>__4 = (ConfigDynamicObjectRegistry) this.<asyncRequest>__3.asset;
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__1);
                        }
                        SuperDebug.VeryImportantAssert(this.<dynamicObjectRegistryConfig>__4 != null, "dynamicObjectRegistryConfig is null dynamicObjectPathes :" + this.<dynamicObjectPathes>__0[this.<ix>__2]);
                        if ((this.<dynamicObjectRegistryConfig>__4 != null) && (this.<dynamicObjectRegistryConfig>__4.entries != null))
                        {
                            this.<jx>__5 = 0;
                            while (this.<jx>__5 < this.<dynamicObjectRegistryConfig>__4.entries.Length)
                            {
                                this.<entry>__6 = this.<dynamicObjectRegistryConfig>__4.entries[this.<jx>__5];
                                DynamicObjectData.dynamicObjectDict.Add(this.<entry>__6.name, this.<entry>__6.prefabPath);
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
                if (this.<ix>__2 < this.<dynamicObjectPathes>__0.Length)
                {
                    this.<asyncRequest>__3 = ConfigUtil.LoadConfigAsync(this.<dynamicObjectPathes>__0[this.<ix>__2], BundleType.RESOURCE_FILE);
                    SuperDebug.VeryImportantAssert(this.<asyncRequest>__3 != null, "patternConfig is null dynamicObjectPathes :" + this.<dynamicObjectPathes>__0[this.<ix>__2]);
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

