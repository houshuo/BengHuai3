namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [TaskCategory("Group")]
    public class LeaderActionToMinion : BehaviorDesigner.Runtime.Tasks.Action
    {
        private ConfigGroupAIGridEntry _gridEntry;
        public SharedString GruopAIGridType;
        public SharedString LeaderAction;
        [RequiredField]
        public SharedEntityDictionary MinionDict;

        public override void OnAwake()
        {
            base.OnAwake();
            if (!string.IsNullOrEmpty(this.GruopAIGridType.Value))
            {
                this._gridEntry = AIData.GetGroupAIGridEntry(this.GruopAIGridType.Value);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (this._gridEntry != null)
            {
                string name = null;
                if (this._gridEntry.LeaderActions.ContainsKey(this.LeaderAction.Value))
                {
                    List<ConfigLeaderToMinionAction> list = this._gridEntry.LeaderActions[this.LeaderAction.Value];
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (UnityEngine.Random.value < list[i].Probability)
                        {
                            name = list[i].Name;
                            break;
                        }
                    }
                    if (name != null)
                    {
                        foreach (KeyValuePair<int, BaseMonoEntity> pair in this.MinionDict.Value)
                        {
                            if (pair.Value.IsActive() && (pair.Value is BaseMonoMonster))
                            {
                                BTreeMonsterAIController activeAIController = ((BaseMonoMonster) pair.Value).GetActiveAIController() as BTreeMonsterAIController;
                                if (activeAIController != null)
                                {
                                    ConfigGroupAIMinionParam[] paramArray = this._gridEntry.Minions.GetConfig<ConfigGroupAIMinion>(pair.Key).TriggerActions[name];
                                    foreach (ConfigGroupAIMinionParam param in paramArray)
                                    {
                                        if (param.Interuption)
                                        {
                                            activeAIController.btree.SendEvent<object>("Interruption", true);
                                            activeAIController.btree.SendEvent<object>("Interruption", false);
                                            activeAIController.btree.SetVariableValue("Group_TriggerAttack", false);
                                        }
                                        if (param.Delay.fixedValue == 0f)
                                        {
                                            AIData.SetSharedVariableCompat(activeAIController.btree, param.AIParams);
                                        }
                                        else
                                        {
                                            activeAIController.DelayedSetParameter(param.Delay.fixedValue, param.AIParams);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return TaskStatus.Success;
        }
    }
}

