namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    public class HasAttackTarget : Conditional
    {
        private BaseMonoAbilityEntity _entity;

        public override void OnAwake()
        {
            this._entity = base.GetComponent<BaseMonoAbilityEntity>();
        }

        public override TaskStatus OnUpdate()
        {
            if (this._entity.GetAttackTarget() != null)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}

