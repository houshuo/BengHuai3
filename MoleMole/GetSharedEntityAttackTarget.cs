namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Group")]
    public class GetSharedEntityAttackTarget : BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedEntity targetEntity;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            BaseMonoEntity newTarget = null;
            if (this.targetEntity.Value != null)
            {
                newTarget = (this.targetEntity.Value as BaseMonoMonster).AttackTarget;
            }
            if (newTarget != null)
            {
                base.GetComponent<BaseMonoMonster>().SetAttackTarget(newTarget);
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}

