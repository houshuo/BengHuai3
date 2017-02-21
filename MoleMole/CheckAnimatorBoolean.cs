namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Ability")]
    public class CheckAnimatorBoolean : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAnimatorEntity _animatorEntity;
        public string AnimatorBoolean;

        public override void OnAwake()
        {
            BaseMonoEntity component = base.GetComponent<BaseMonoEntity>();
            this._animatorEntity = (BaseMonoAnimatorEntity) component;
        }

        public override TaskStatus OnUpdate()
        {
            if (this._animatorEntity.GetLocomotionBool(this.AnimatorBoolean))
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}

