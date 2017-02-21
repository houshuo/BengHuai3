namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    public class SetAIAnimatorBool : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAnimatorEntity _entity;
        public string boolName = string.Empty;
        public bool value;

        public override void OnAwake()
        {
            this._entity = base.GetComponent<BaseMonoAnimatorEntity>();
        }

        public override TaskStatus OnUpdate()
        {
            if (this._entity.IsActive())
            {
                this._entity.SetLocomotionBool(this.boolName, this.value, false);
            }
            return TaskStatus.Success;
        }
    }
}

