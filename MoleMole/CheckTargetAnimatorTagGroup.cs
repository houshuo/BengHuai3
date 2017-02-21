namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Monster"), TaskDescription("Check whether target in is certain animator tag group, e.g. Attack or Skill, Movement, etc.")]
    public class CheckTargetAnimatorTagGroup : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IAIEntity _aiEntity;
        public string animatorTag;

        public override void OnAwake()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            this._aiEntity = (BaseMonoMonster) component;
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            BaseMonoAnimatorEntity attackTarget = this._aiEntity.AttackTarget as BaseMonoAnimatorEntity;
            if (attackTarget != null)
            {
                if (attackTarget is BaseMonoAvatar)
                {
                    AvatarData.AvatarTagGroup tagGroup = (AvatarData.AvatarTagGroup) ((int) Enum.Parse(typeof(AvatarData.AvatarTagGroup), this.animatorTag));
                    if ((attackTarget as BaseMonoAvatar).IsAnimatorInTag(tagGroup))
                    {
                        return TaskStatus.Success;
                    }
                }
                else if (attackTarget is BaseMonoMonster)
                {
                    MonsterData.MonsterTagGroup group2 = (MonsterData.MonsterTagGroup) ((int) Enum.Parse(typeof(MonsterData.MonsterTagGroup), this.animatorTag));
                    if ((attackTarget as BaseMonoMonster).IsAnimatorInTag(group2))
                    {
                        return TaskStatus.Success;
                    }
                }
            }
            return TaskStatus.Failure;
        }
    }
}

