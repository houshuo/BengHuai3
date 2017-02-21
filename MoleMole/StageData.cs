namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class StageData
    {
        private static Dictionary<string, StageEntry> _stageEntryDict;
        public const float ORTH_STAGE_LIFT_RATIO = 0.06f;
        public const string STAGE_REG_FILE_PATH = "Stage/StageRegistry";

        public static Dictionary<string, StageEntry> GetAllStageEntries()
        {
            return _stageEntryDict;
        }

        public static StageEntry GetFirstStageEntryByPrefabPathAndLocatorName(string perpStagePrefabPath, string locatorName)
        {
            foreach (StageEntry entry in _stageEntryDict.Values)
            {
                if ((entry.GetPerpStagePrefabPath() == perpStagePrefabPath) && entry.LocationPointName.EndsWith(locatorName))
                {
                    return entry;
                }
            }
            return null;
        }

        public static StageEntry GetStageEntryByName(string typeName)
        {
            return _stageEntryDict[typeName];
        }

        public static void ReloadFromFile()
        {
            _stageEntryDict = new Dictionary<string, StageEntry>();
            string[] stageEntryPathes = GlobalDataManager.metaConfig.stageEntryPathes;
            for (int i = 0; i < stageEntryPathes.Length; i++)
            {
                ConfigStageRegistry registry = ConfigUtil.LoadConfig<ConfigStageRegistry>(stageEntryPathes[i]);
                if (registry.entries != null)
                {
                    for (int j = 0; j < registry.entries.Length; j++)
                    {
                        StageEntry entry = registry.entries[j];
                        _stageEntryDict.Add(entry.TypeName, entry);
                    }
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator13 { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator13 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal AsyncAssetRequst <asyncRequest>__3;
            internal int <ix>__2;
            internal int <jx>__5;
            internal StageEntry <stage>__6;
            internal string[] <stageEntryPathes>__0;
            internal ConfigStageRegistry <stageRegistryConfig>__4;
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
                        StageData._stageEntryDict = new Dictionary<string, StageEntry>();
                        this.<stageEntryPathes>__0 = GlobalDataManager.metaConfig.stageEntryPathes;
                        this.<step>__1 = this.progressSpan / ((float) this.<stageEntryPathes>__0.Length);
                        this.<ix>__2 = 0;
                        goto Label_01BE;

                    case 1:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__1);
                        }
                        this.<stageRegistryConfig>__4 = (ConfigStageRegistry) this.<asyncRequest>__3.asset;
                        SuperDebug.VeryImportantAssert(this.<stageRegistryConfig>__4 != null, "stageRegistryConfig is null stagePath :" + this.<stageEntryPathes>__0[this.<ix>__2]);
                        if ((this.<stageRegistryConfig>__4 != null) && (this.<stageRegistryConfig>__4.entries != null))
                        {
                            this.<jx>__5 = 0;
                            while (this.<jx>__5 < this.<stageRegistryConfig>__4.entries.Length)
                            {
                                this.<stage>__6 = this.<stageRegistryConfig>__4.entries[this.<jx>__5];
                                StageData._stageEntryDict.Add(this.<stage>__6.TypeName, this.<stage>__6);
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
                if (this.<ix>__2 < this.<stageEntryPathes>__0.Length)
                {
                    this.<asyncRequest>__3 = ConfigUtil.LoadConfigAsync(this.<stageEntryPathes>__0[this.<ix>__2], BundleType.RESOURCE_FILE);
                    SuperDebug.VeryImportantAssert(this.<asyncRequest>__3 != null, "assetRequest is null stagePath :" + this.<stageEntryPathes>__0[this.<ix>__2]);
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

