namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Ability")]
    public class GetAnimatorInteger : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAnimatorEntity _animatorEntity;
        public string AnimatorInteger;
        public SharedInt SharedInteger;

        public override void OnAwake()
        {
            BaseMonoEntity component = base.GetComponent<BaseMonoEntity>();
            this._animatorEntity = (BaseMonoAnimatorEntity) component;
        }

        public override TaskStatus OnUpdate()
        {
            this.SharedInteger.SetValue(this._animatorEntity.GetLocomotionInteger(this.AnimatorInteger));
            return TaskStatus.Success;
        }
    }
}

