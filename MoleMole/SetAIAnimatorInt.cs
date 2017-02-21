namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    public class SetAIAnimatorInt : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAnimatorEntity _entity;
        public string intName = string.Empty;
        public int value;

        public override void OnAwake()
        {
            this._entity = base.GetComponent<BaseMonoAnimatorEntity>();
        }

        public override TaskStatus OnUpdate()
        {
            if (this._entity.IsActive())
            {
                this._entity.SetLocomotionInteger(this.intName, this.value, false);
            }
            return TaskStatus.Success;
        }
    }
}

