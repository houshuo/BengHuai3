namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("AttackTarget/Avatar")]
    public class AttackTargetSelectLockMonster : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAvatar _avatar;

        public override void OnAwake()
        {
            this._avatar = base.GetComponent<BaseMonoAvatar>();
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            BaseMonoEntity attackTarget = this.SelectTarget();
            if (attackTarget != null)
            {
                this._avatar.GetActiveAIController().TrySetAttackTarget(attackTarget);
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        private BaseMonoEntity SelectTarget()
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            if ((localAvatar.AttackTarget != null) && localAvatar.AttackTarget.IsActive())
            {
                return localAvatar.AttackTarget;
            }
            return null;
        }
    }
}

