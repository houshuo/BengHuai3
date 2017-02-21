namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    public class IsTargetAttacking : Conditional
    {
        private BaseMonoAbilityEntity _entity;

        public override void OnAwake()
        {
            this._entity = base.GetComponent<BaseMonoAbilityEntity>();
        }

        public override TaskStatus OnUpdate()
        {
            BaseMonoEntity attackTarget = this._entity.GetAttackTarget();
            if ((attackTarget != null) && attackTarget.IsActive())
            {
                if (attackTarget is BaseMonoAvatar)
                {
                    BaseMonoAvatar avatar = (BaseMonoAvatar) attackTarget;
                    if (avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackOrSkill) && !avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackWithNoTarget))
                    {
                        return TaskStatus.Success;
                    }
                    return TaskStatus.Failure;
                }
                if (attackTarget is BaseMonoMonster)
                {
                    BaseMonoMonster monster = (BaseMonoMonster) attackTarget;
                    return (!monster.isGoingToAttack(0.5f) ? TaskStatus.Failure : TaskStatus.Success);
                }
            }
            return TaskStatus.Failure;
        }
    }
}

