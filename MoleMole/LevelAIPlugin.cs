namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;
    using System.Collections.Generic;

    public class LevelAIPlugin : BaseActorPlugin
    {
        private List<BaseMonoMonster> _attackingMonsters;
        private LevelActor _levelActor;
        public const string AVATAR_BE_ATTACK_MAX_NUM_NAME = "AvatarBeAttackMaxNum";
        public const string AVATAR_BE_ATTACK_NUM_NAME = "AvatarBeAttackNum";

        public LevelAIPlugin(LevelActor levelActor)
        {
            this._levelActor = levelActor;
            this._attackingMonsters = new List<BaseMonoMonster>();
        }

        public void AddAttackingMonster(BaseMonoMonster monster)
        {
            if (!this._attackingMonsters.Contains(monster))
            {
                this._attackingMonsters.Add(monster);
            }
            this.RefreshAvatarBeAttackMaxNum();
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtKilled) && this.ListenKill((EvtKilled) evt));
        }

        private bool ListenKill(EvtKilled evt)
        {
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 4)
            {
                BaseMonoMonster item = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(evt.targetID);
                if ((item != null) && this._attackingMonsters.Contains(item))
                {
                    this.RemoveAttackingMonster(item);
                }
            }
            return true;
        }

        public override void OnAdded()
        {
            this.SetAvatarBeAttackMaxNum(4);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(this._levelActor.runtimeID);
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(this._levelActor.runtimeID);
        }

        public void RefreshAvatarBeAttackMaxNum()
        {
            GlobalVariables.Instance.SetVariableValue("AvatarBeAttackNum", this._attackingMonsters.Count);
        }

        public void RemoveAttackingMonster(BaseMonoMonster monster)
        {
            if (this._attackingMonsters.Contains(monster))
            {
                this._attackingMonsters.Remove(monster);
            }
            this.RefreshAvatarBeAttackMaxNum();
        }

        public void SetAvatarBeAttackMaxNum(int maxNum)
        {
            GlobalVariables.Instance.SetVariableValue("AvatarBeAttackMaxNum", maxNum);
            GlobalVariables.Instance.SetVariableValue("AvatarBeAttackNum", 0);
        }
    }
}

