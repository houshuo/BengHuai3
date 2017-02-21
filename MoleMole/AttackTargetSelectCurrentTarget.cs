namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("AttackTarget/Avatar")]
    public class AttackTargetSelectCurrentTarget : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAvatar _avatar;
        public SharedBool isNewTarget;

        public override void OnAwake()
        {
            this._avatar = base.GetComponent<BaseMonoAvatar>();
        }

        public override TaskStatus OnUpdate()
        {
            if ((this._avatar.AttackTarget != null) && this._avatar.AttackTarget.IsActive())
            {
                this.isNewTarget.SetValue(false);
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}

