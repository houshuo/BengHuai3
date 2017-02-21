namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;

    [TaskCategory("Group")]
    public class GetMinions : BehaviorDesigner.Runtime.Tasks.Action
    {
        private ConfigOverrideList _minionConfigLs;
        private Dictionary<int, BaseMonoEntity> _minions;
        public SharedString GruopAIGridType;
        [RequiredField]
        public SharedEntityDictionary MinionDict;
        private BaseMonoMonster monster;

        public override void OnAwake()
        {
            if (!string.IsNullOrEmpty(this.GruopAIGridType.Value))
            {
                this.monster = base.GetComponent<BaseMonoMonster>();
                this._minions = this.MinionDict.Value;
                if (this._minions == null)
                {
                    this._minions = new Dictionary<int, BaseMonoEntity>();
                    this.MinionDict.Value = this._minions;
                }
                this._minionConfigLs = AIData.GetGroupAIGridEntry(this.GruopAIGridType.Value).Minions;
            }
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            if (this._minionConfigLs != null)
            {
                List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
                for (int i = 0; i < this._minionConfigLs.length; i++)
                {
                    ConfigGroupAIMinion config = this._minionConfigLs.GetConfig<ConfigGroupAIMinion>(i);
                    if (this._minions.ContainsKey(i))
                    {
                        if (this._minions[i] != null)
                        {
                            continue;
                        }
                        this._minions.Remove(i);
                    }
                    for (int j = 0; j < allMonsters.Count; j++)
                    {
                        if ((allMonsters[j].MonsterName == config.MonsterName) && (allMonsters[j] != this.monster))
                        {
                            BehaviorDesigner.Runtime.BehaviorTree component = allMonsters[j].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
                            bool flag = true;
                            SharedBool variable = component.GetVariable("Group_IsMinion") as SharedBool;
                            if (!variable.Value)
                            {
                                flag = false;
                            }
                            SharedEntity entity = component.GetVariable("Group_LeaderEntity") as SharedEntity;
                            if ((entity != null) && (entity.Value != null))
                            {
                                flag = false;
                            }
                            SharedString str = component.GetVariable("GroupAIGrid") as SharedString;
                            if ((str != null) && !string.IsNullOrEmpty(str.Value))
                            {
                                flag = false;
                            }
                            if (flag && allMonsters[j].IsAIControllerActive())
                            {
                                BTreeMonsterAIController activeAIController = (BTreeMonsterAIController) allMonsters[j].GetActiveAIController();
                                if (activeAIController.IsBehaviorRunning())
                                {
                                    activeAIController.SetActive(false);
                                    activeAIController.SetActive(true);
                                    component.SetVariableValue("Group_LeaderEntity", this.monster);
                                    AIData.SetSharedVariableCompat(component, config.AIParams);
                                    this._minions.Add(i, allMonsters[j]);
                                    break;
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

