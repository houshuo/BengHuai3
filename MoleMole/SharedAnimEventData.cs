namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class SharedAnimEventData
    {
        private static List<string> _configPathList;
        private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();
        private static Action<string> _loadJsonConfigCallback = null;
        private static Dictionary<string, ConfigEntityAnimEvent> _sharedAnimEventDict;
        public const char SHARED_ANIM_EVENT_PREFIX = '#';

        public static bool IsSharedAnimEventID(string animEventID)
        {
            return (animEventID[0] == '#');
        }

        private static void OnLoadOneJsonConfigFinish(ConfigSharedAnimEventGroup animEventGroup, string configPath)
        {
            _configPathList.Remove(configPath);
            foreach (KeyValuePair<string, ConfigEntityAnimEvent> pair in animEventGroup.AnimEvents)
            {
                _sharedAnimEventDict.Add(pair.Key, pair.Value);
            }
            if (_configPathList.Count == 0)
            {
                _loadDataBackGroundWorker.StopBackGroundWork(false);
                if (_loadJsonConfigCallback != null)
                {
                    _loadJsonConfigCallback("SharedAnimEventData");
                    _loadJsonConfigCallback = null;
                }
            }
        }

        public static void ReloadFromData()
        {
            _sharedAnimEventDict = new Dictionary<string, ConfigEntityAnimEvent>();
            foreach (string str in GlobalDataManager.metaConfig.sharedAnimEventGroupPathes)
            {
                foreach (KeyValuePair<string, ConfigEntityAnimEvent> pair in ConfigUtil.LoadJSONConfig<ConfigSharedAnimEventGroup>(str).AnimEvents)
                {
                    _sharedAnimEventDict.Add(pair.Key, pair.Value);
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator12 { finishCallback = finishCallback, progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>finishCallback = finishCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        public static ConfigAvatarAnimEvent ResolveAnimEvent(ConfigAvatar config, string animEventID)
        {
            ConfigAvatarAnimEvent event2;
            if (animEventID == null)
            {
                return null;
            }
            if (animEventID[0] == '#')
            {
                return (_sharedAnimEventDict[animEventID] as ConfigAvatarAnimEvent);
            }
            config.AnimEvents.TryGetValue(animEventID, out event2);
            return event2;
        }

        public static ConfigMonsterAnimEvent ResolveAnimEvent(ConfigMonster config, string animEventID)
        {
            ConfigMonsterAnimEvent event2;
            if (animEventID == null)
            {
                return null;
            }
            if (animEventID[0] == '#')
            {
                return (_sharedAnimEventDict[animEventID] as ConfigMonsterAnimEvent);
            }
            config.AnimEvents.TryGetValue(animEventID, out event2);
            return event2;
        }

        public static ConfigPropAnimEvent ResolveAnimEvent(ConfigPropObject config, string animEventID)
        {
            ConfigPropAnimEvent event2;
            if (animEventID == null)
            {
                return null;
            }
            if (animEventID[0] == '#')
            {
                return (_sharedAnimEventDict[animEventID] as ConfigPropAnimEvent);
            }
            config.AnimEvents.TryGetValue(animEventID, out event2);
            return event2;
        }

        public static ConfigEntityAnimEvent ResolveAnimEvent(IEntityConfig config, string animEventID)
        {
            if (animEventID == null)
            {
                return null;
            }
            if (animEventID[0] == '#')
            {
                return _sharedAnimEventDict[animEventID];
            }
            return config.TryGetAnimEvent(animEventID);
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator12 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>finishCallback;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal string[] <$s_947>__1;
            internal int <$s_948>__2;
            internal string[] <$s_949>__5;
            internal int <$s_950>__6;
            internal AsyncAssetRequst <asyncRequest>__8;
            internal string <jsonPath>__3;
            internal string <jsonPath>__7;
            internal string[] <pathes>__0;
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
                        SharedAnimEventData._loadJsonConfigCallback = this.finishCallback;
                        SharedAnimEventData._configPathList = new List<string>();
                        SharedAnimEventData._sharedAnimEventDict = new Dictionary<string, ConfigEntityAnimEvent>();
                        this.<pathes>__0 = GlobalDataManager.metaConfig.sharedAnimEventGroupPathes;
                        if (this.<pathes>__0.Length != 0)
                        {
                            this.<$s_947>__1 = this.<pathes>__0;
                            this.<$s_948>__2 = 0;
                            while (this.<$s_948>__2 < this.<$s_947>__1.Length)
                            {
                                this.<jsonPath>__3 = this.<$s_947>__1[this.<$s_948>__2];
                                SharedAnimEventData._configPathList.Add(this.<jsonPath>__3);
                                this.<$s_948>__2++;
                            }
                            this.<step>__4 = this.progressSpan / ((float) this.<pathes>__0.Length);
                            SharedAnimEventData._loadDataBackGroundWorker.StartBackGroundWork("SharedAnimEventData");
                            this.<$s_949>__5 = this.<pathes>__0;
                            this.<$s_950>__6 = 0;
                            while (this.<$s_950>__6 < this.<$s_949>__5.Length)
                            {
                                this.<jsonPath>__7 = this.<$s_949>__5[this.<$s_950>__6];
                                this.<asyncRequest>__8 = ConfigUtil.LoadJsonConfigAsync(this.<jsonPath>__7, BundleType.DATA_FILE);
                                SuperDebug.VeryImportantAssert(this.<asyncRequest>__8 != null, "assetRequest is null sharedAnimEventPath :" + this.<jsonPath>__7);
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
                                ConfigUtil.LoadJSONStrConfigMultiThread<ConfigSharedAnimEventGroup>(this.<asyncRequest>__8.asset.ToString(), SharedAnimEventData._loadDataBackGroundWorker, new Action<ConfigSharedAnimEventGroup, string>(SharedAnimEventData.OnLoadOneJsonConfigFinish), this.<jsonPath>__7);
                            Label_01D5:
                                this.<$s_950>__6++;
                            }
                            this.$PC = -1;
                            break;
                        }
                        if (SharedAnimEventData._loadJsonConfigCallback != null)
                        {
                            SharedAnimEventData._loadJsonConfigCallback("SharedAnimEventData");
                            SharedAnimEventData._loadJsonConfigCallback = null;
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

