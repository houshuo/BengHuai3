namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class PropObjectData
    {
        private static List<string> _configPathList;
        private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();
        private static Action<string> _loadJsonConfigCallback = null;
        public static Dictionary<string, ConfigPropObject> _propMap;

        public static int CalculateContentHash()
        {
            int lastHash = 0;
            foreach (ConfigPropObject obj2 in _propMap.Values)
            {
                HashUtils.TryHashObject(obj2, ref lastHash);
            }
            return lastHash;
        }

        public static ConfigPropObject GetPropObjectConfig(string propName)
        {
            return _propMap[propName];
        }

        private static void OnLoadOneJsonConfigFinish(ConfigPropObjectRegistry propList, string configPath)
        {
            _configPathList.Remove(configPath);
            foreach (ConfigPropObject obj2 in propList)
            {
                _propMap.Add(obj2.Name, obj2);
            }
            if (_configPathList.Count == 0)
            {
                _loadDataBackGroundWorker.StopBackGroundWork(false);
                if (_loadJsonConfigCallback != null)
                {
                    _loadJsonConfigCallback("PropObjectData");
                    _loadJsonConfigCallback = null;
                }
            }
        }

        public static void ReloadFromFile()
        {
            _propMap = new Dictionary<string, ConfigPropObject>();
            foreach (string str in GlobalDataManager.metaConfig.propObjectRegistryPathes)
            {
                foreach (ConfigPropObject obj2 in ConfigUtil.LoadJSONConfig<ConfigPropObjectRegistry>(str))
                {
                    _propMap.Add(obj2.Name, obj2);
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator10 { finishCallback = finishCallback, progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>finishCallback = finishCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator10 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>finishCallback;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal string[] <$s_926>__1;
            internal int <$s_927>__2;
            internal string[] <$s_928>__5;
            internal int <$s_929>__6;
            internal AsyncAssetRequst <asyncRequest>__8;
            internal string[] <pathes>__0;
            internal string <propRegistryPath>__3;
            internal string <propRegistryPath>__7;
            internal float <step>__4;
            internal Action<string> finishCallback;
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
                        PropObjectData._loadJsonConfigCallback = this.finishCallback;
                        PropObjectData._configPathList = new List<string>();
                        PropObjectData._propMap = new Dictionary<string, ConfigPropObject>();
                        this.<pathes>__0 = GlobalDataManager.metaConfig.propObjectRegistryPathes;
                        if (this.<pathes>__0.Length != 0)
                        {
                            this.<$s_926>__1 = this.<pathes>__0;
                            this.<$s_927>__2 = 0;
                            while (this.<$s_927>__2 < this.<$s_926>__1.Length)
                            {
                                this.<propRegistryPath>__3 = this.<$s_926>__1[this.<$s_927>__2];
                                PropObjectData._configPathList.Add(this.<propRegistryPath>__3);
                                this.<$s_927>__2++;
                            }
                            this.<step>__4 = this.progressSpan / ((float) this.<pathes>__0.Length);
                            PropObjectData._loadDataBackGroundWorker.StartBackGroundWork("PropObjectData");
                            this.<$s_928>__5 = this.<pathes>__0;
                            this.<$s_929>__6 = 0;
                            while (this.<$s_929>__6 < this.<$s_928>__5.Length)
                            {
                                this.<propRegistryPath>__7 = this.<$s_928>__5[this.<$s_929>__6];
                                this.<asyncRequest>__8 = ConfigUtil.LoadJsonConfigAsync(this.<propRegistryPath>__7, BundleType.DATA_FILE);
                                SuperDebug.VeryImportantAssert(this.<asyncRequest>__8 != null, "assetRequest is null propRegistryPath :" + this.<propRegistryPath>__7);
                                if (this.<asyncRequest>__8 == null)
                                {
                                    goto Label_01D5;
                                }
                                this.$current = this.<asyncRequest>__8.operation;
                                this.$PC = 1;
                                return true;
                            Label_018D:
                                if (this.moveOneStepCallback != null)
                                {
                                    this.moveOneStepCallback(this.<step>__4);
                                }
                                ConfigUtil.LoadJSONStrConfigMultiThread<ConfigPropObjectRegistry>(this.<asyncRequest>__8.asset.ToString(), PropObjectData._loadDataBackGroundWorker, new Action<ConfigPropObjectRegistry, string>(PropObjectData.OnLoadOneJsonConfigFinish), this.<propRegistryPath>__7);
                            Label_01D5:
                                this.<$s_929>__6++;
                            }
                            this.$PC = -1;
                            break;
                        }
                        if (PropObjectData._loadJsonConfigCallback != null)
                        {
                            PropObjectData._loadJsonConfigCallback("PropObjectData");
                            PropObjectData._loadJsonConfigCallback = null;
                        }
                        break;

                    case 1:
                        goto Label_018D;
                }
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

