namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;

    [TaskCategory("AttackTarget/Monster")]
    public class AttackTargetSelectByThreat : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoMonster _monster;
        private MonsterAIPlugin _monsterAIPlugin;
        public SharedEntity retargetEntity;

        public override void OnAwake()
        {
            this._monster = base.GetComponent<BaseMonoMonster>();
            this._monsterAIPlugin = Singleton<EventManager>.Instance.GetActor<MonsterActor>(this._monster.GetRuntimeID()).GetPlugin<MonsterAIPlugin>();
        }

        public override TaskStatus OnUpdate()
        {
            if ((this.retargetEntity.Value != null) && this.retargetEntity.Value.IsActive())
            {
                this._monster.SetAttackTarget(this.retargetEntity.Value);
                this.retargetEntity.SetValue((BaseMonoEntity) null);
                return TaskStatus.Success;
            }
            if (this._monster.AttackTarget == null)
            {
                BaseMonoAvatar newTarget = this.SelectNearestAvatar();
                if (newTarget != null)
                {
                    this._monsterAIPlugin.InitNearestAvatarThreat(newTarget.GetRuntimeID());
                    this._monster.SetAttackTarget(newTarget);
                    return TaskStatus.Success;
                }
            }
            else
            {
                uint runtimeID = this._monster.AttackTarget.GetRuntimeID();
                uint num2 = this._monsterAIPlugin.RetargetByThreat(runtimeID);
                if (runtimeID != num2)
                {
                    this._monster.SetAttackTarget(Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(num2));
                }
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }

        private BaseMonoAvatar SelectNearestAvatar()
        {
            BaseMonoAvatar avatar = null;
            float maxValue = float.MaxValue;
            List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
            for (int i = 0; i < allAvatars.Count; i++)
            {
                BaseMonoAvatar avatar2 = allAvatars[i];
                if (((avatar2 != null) && avatar2.IsActive()) && !avatar2.denySelect)
                {
                    float num3 = Miscs.DistancForVec3IgnoreY(this._monster.XZPosition, avatar2.XZPosition);
                    if (num3 < maxValue)
                    {
                        maxValue = num3;
                        avatar = avatar2;
                    }
                }
            }
            return avatar;
        }
    }
}

