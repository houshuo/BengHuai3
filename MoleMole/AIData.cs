namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class AIData
    {
        private static Dictionary<string, ConfigGroupAIGridEntry> _allGridEntries;
        private static List<string> _configPathList;
        private static BackGroundWorker _loadDataBackGroundWorker = new BackGroundWorker();
        private static Action<string> _loadJsonConfigCallback = null;
        public const int DEFAULT_LOCAL_AVATAR_BE_ATTACK_MAX_NUM = 4;
        private const string GROUP_AI_GRID_CONFIG_PATH = "Data/AI/GroupAIGrid";
        public static ConfigGroupAIGridRepository GroupAIGrid;
        public const string TARGET_ATTACK_START_AI_EVENT = "AITargetAttackStart_0";
        public const string THREAT_RETARGET_AI_EVENT = "AIThreatRetarget_1";

        public static ConfigGroupAIGridEntry GetGroupAIGridEntry(string name)
        {
            return _allGridEntries[name];
        }

        private static void OnLoadOneJsonConfigFinish(ConfigGroupAIGridRepository gridAIEntries, string configPath)
        {
            _configPathList.Remove(configPath);
            foreach (ConfigGroupAIGridEntry entry in gridAIEntries)
            {
                _allGridEntries.Add(entry.Name, entry);
            }
            if (_configPathList.Count == 0)
            {
                _loadDataBackGroundWorker.StopBackGroundWork(false);
                if (_loadJsonConfigCallback != null)
                {
                    _loadJsonConfigCallback("AIData");
                    _loadJsonConfigCallback = null;
                }
            }
        }

        public static void ReloadFromFile()
        {
            _allGridEntries = new Dictionary<string, ConfigGroupAIGridEntry>();
            foreach (string str in GlobalDataManager.metaConfig.groupAIGridPathes)
            {
                foreach (ConfigGroupAIGridEntry entry in ConfigUtil.LoadJSONConfig<ConfigGroupAIGridRepository>(str))
                {
                    _allGridEntries.Add(entry.Name, entry);
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null, Action<string> finishCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator4 { finishCallback = finishCallback, progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>finishCallback = finishCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        public static void SetSharedVariableCompat(BehaviorDesigner.Runtime.BehaviorTree btree, ConfigDynamicArguments aiParams)
        {
            foreach (KeyValuePair<string, object> pair in aiParams)
            {
                SharedVariable variable = btree.GetVariable(pair.Key);
                if ((variable is SharedFloat) || (variable is SharedSafeFloat))
                {
                    if (pair.Value is int)
                    {
                        variable.SetValue((float) ((int) pair.Value));
                    }
                    else
                    {
                        variable.SetValue((float) pair.Value);
                    }
                }
                else if ((variable is SharedInt) || (variable is SharedSafeInt))
                {
                    variable.SetValue((int) pair.Value);
                }
                else if (variable is SharedBool)
                {
                    variable.SetValue((bool) pair.Value);
                }
                else if (variable is SharedString)
                {
                    variable.SetValue((string) pair.Value);
                }
                else if (variable is SharedGroupMoveType)
                {
                    variable.SetValue(Enum.Parse(typeof(ConfigGroupAIMinionOld.MoveType), (string) pair.Value));
                }
                else if (variable is SharedGroupAttackType)
                {
                    variable.SetValue(Enum.Parse(typeof(ConfigGroupAIMinionOld.AttackType), (string) pair.Value));
                }
            }
        }

        public static void SetSharedVariableOld(BehaviorDesigner.Runtime.BehaviorTree btree, ConfigGroupAIMinionParamOld[] AIParams)
        {
            for (int i = 0; i < AIParams.Length; i++)
            {
                switch (AIParams[i].Type)
                {
                    case ConfigGroupAIMinionParamOld.ParamType.Float:
                        btree.SetVariableValue(AIParams[i].Name, AIParams[i].FloatValue);
                        break;

                    case ConfigGroupAIMinionParamOld.ParamType.Int:
                        btree.SetVariableValue(AIParams[i].Name, AIParams[i].IntValue);
                        break;

                    case ConfigGroupAIMinionParamOld.ParamType.Bool:
                        btree.SetVariableValue(AIParams[i].Name, AIParams[i].BoolValue);
                        break;

                    case ConfigGroupAIMinionParamOld.ParamType.AttackType:
                        btree.SetVariableValue(AIParams[i].Name, AIParams[i].AttackTypeValue);
                        break;

                    case ConfigGroupAIMinionParamOld.ParamType.MoveType:
                        btree.SetVariableValue(AIParams[i].Name, AIParams[i].MoveTypeValue);
                        break;

                    default:
                        if (AIParams[i].Interruption)
                        {
                            btree.SendEvent<object>("Interruption", true);
                            btree.SendEvent<object>("Interruption", false);
                            btree.SetVariableValue("Group_TriggerAttack", false);
                        }
                        if (AIParams[i].TriggerAttack)
                        {
                            if (AIParams[i].TriggerAttackDelay > 0f)
                            {
                                btree.SetVariableValue("Group_TriggerAttackDelay", AIParams[i].TriggerAttackDelay);
                            }
                            else
                            {
                                btree.SetVariableValue("Group_TriggerAttack", true);
                            }
                        }
                        break;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator4 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>finishCallback;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal string[] <$s_861>__1;
            internal int <$s_862>__2;
            internal string[] <$s_863>__5;
            internal int <$s_864>__6;
            internal AsyncAssetRequst <asyncRequest>__8;
            internal string <gridFilePath>__3;
            internal string <gridFilePath>__7;
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
                        AIData._loadJsonConfigCallback = this.finishCallback;
                        AIData._configPathList = new List<string>();
                        AIData._allGridEntries = new Dictionary<string, ConfigGroupAIGridEntry>();
                        this.<pathes>__0 = GlobalDataManager.metaConfig.groupAIGridPathes;
                        if (this.<pathes>__0.Length != 0)
                        {
                            this.<$s_861>__1 = this.<pathes>__0;
                            this.<$s_862>__2 = 0;
                            while (this.<$s_862>__2 < this.<$s_861>__1.Length)
                            {
                                this.<gridFilePath>__3 = this.<$s_861>__1[this.<$s_862>__2];
                                AIData._configPathList.Add(this.<gridFilePath>__3);
                                this.<$s_862>__2++;
                            }
                            this.<step>__4 = this.progressSpan / ((float) this.<pathes>__0.Length);
                            AIData._loadDataBackGroundWorker.StartBackGroundWork("AIData");
                            this.<$s_863>__5 = this.<pathes>__0;
                            this.<$s_864>__6 = 0;
                            while (this.<$s_864>__6 < this.<$s_863>__5.Length)
                            {
                                this.<gridFilePath>__7 = this.<$s_863>__5[this.<$s_864>__6];
                                this.<asyncRequest>__8 = ConfigUtil.LoadJsonConfigAsync(this.<gridFilePath>__7, BundleType.DATA_FILE);
                                SuperDebug.VeryImportantAssert(this.<asyncRequest>__8 != null, "assetRequest is null gridFilePath :" + this.<gridFilePath>__7);
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
                                ConfigUtil.LoadJSONStrConfigMultiThread<ConfigGroupAIGridRepository>(this.<asyncRequest>__8.asset.ToString(), AIData._loadDataBackGroundWorker, new Action<ConfigGroupAIGridRepository, string>(AIData.OnLoadOneJsonConfigFinish), this.<gridFilePath>__7);
                            Label_01D5:
                                this.<$s_864>__6++;
                            }
                            this.$PC = -1;
                            break;
                        }
                        if (AIData._loadJsonConfigCallback != null)
                        {
                            AIData._loadJsonConfigCallback("AIData");
                            AIData._loadJsonConfigCallback = null;
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

