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

    public static class RenderingData
    {
        private static Dictionary<string, ConfigBaseRenderingData> _renderingDataDict;

        public static void ApplyRenderingData(ConfigBaseRenderingData renderingData, Material mat)
        {
            for (int i = 0; i < renderingData.properties.Length; i++)
            {
                renderingData.properties[i].SimpleApplyOnMaterial(mat);
            }
        }

        public static T GetRenderingDataConfig<T>(string name) where T: ConfigBaseRenderingData
        {
            return (T) _renderingDataDict[name];
        }

        public static void ReloadFromFile()
        {
            _renderingDataDict = new Dictionary<string, ConfigBaseRenderingData>();
            string[] renderEntryPathes = GlobalDataManager.metaConfig.renderEntryPathes;
            for (int i = 0; i < renderEntryPathes.Length; i++)
            {
                ConfigRenderingDataRegistry registry = ConfigUtil.LoadConfig<ConfigRenderingDataRegistry>(renderEntryPathes[i]);
                if (registry.entries != null)
                {
                    for (int j = 0; j < registry.entries.Length; j++)
                    {
                        RenderingDataEntry entry = registry.entries[j];
                        _renderingDataDict.Add(entry.name, Miscs.LoadResource<ConfigBaseRenderingData>(entry.GetDataPath(), BundleType.RESOURCE_FILE));
                    }
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator11 { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator11 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal AsyncAssetRequst <asyncRequest>__3;
            internal RenderingDataEntry <entry>__6;
            internal int <ix>__2;
            internal int <jx>__5;
            internal string[] <renderingDataRegistryPathes>__0;
            internal ConfigRenderingDataRegistry <renderingRegistry>__4;
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
                        RenderingData._renderingDataDict = new Dictionary<string, ConfigBaseRenderingData>();
                        this.<renderingDataRegistryPathes>__0 = GlobalDataManager.metaConfig.renderEntryPathes;
                        this.<step>__1 = this.progressSpan / ((float) this.<renderingDataRegistryPathes>__0.Length);
                        this.<ix>__2 = 0;
                        goto Label_01C9;

                    case 1:
                        this.<renderingRegistry>__4 = (ConfigRenderingDataRegistry) this.<asyncRequest>__3.asset;
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__1);
                        }
                        SuperDebug.VeryImportantAssert(this.<renderingRegistry>__4 != null, "renderingRegistry is null renderingDataRegistryPath :" + this.<renderingDataRegistryPathes>__0[this.<ix>__2]);
                        if ((this.<renderingRegistry>__4 != null) && (this.<renderingRegistry>__4.entries != null))
                        {
                            this.<jx>__5 = 0;
                            while (this.<jx>__5 < this.<renderingRegistry>__4.entries.Length)
                            {
                                this.<entry>__6 = this.<renderingRegistry>__4.entries[this.<jx>__5];
                                RenderingData._renderingDataDict.Add(this.<entry>__6.name, Miscs.LoadResource<ConfigBaseRenderingData>(this.<entry>__6.GetDataPath(), BundleType.RESOURCE_FILE));
                                this.<jx>__5++;
                            }
                        }
                        break;

                    default:
                        goto Label_01E3;
                }
            Label_01BB:
                this.<ix>__2++;
            Label_01C9:
                if (this.<ix>__2 < this.<renderingDataRegistryPathes>__0.Length)
                {
                    this.<asyncRequest>__3 = ConfigUtil.LoadConfigAsync(this.<renderingDataRegistryPathes>__0[this.<ix>__2], BundleType.RESOURCE_FILE);
                    SuperDebug.VeryImportantAssert(this.<asyncRequest>__3 != null, "assetRequest is null renderingDataRegistryPath :" + this.<renderingDataRegistryPathes>__0[this.<ix>__2]);
                    if (this.<asyncRequest>__3 != null)
                    {
                        this.$current = this.<asyncRequest>__3.operation;
                        this.$PC = 1;
                        return true;
                    }
                    goto Label_01BB;
                }
                this.$PC = -1;
            Label_01E3:
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

