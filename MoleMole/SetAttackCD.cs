namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    public class SetAttackCD : BehaviorDesigner.Runtime.Tasks.Action
    {
        [Tooltip("CD Ratio")]
        public SharedFloat CDRatio;
        [Tooltip("The value to set the SharedFloat to")]
        public SharedFloat targetValue;
        [RequiredField, Tooltip("The SharedFloat to set")]
        public SharedFloat targetVariable;

        public override void OnReset()
        {
            this.targetValue = 0f;
            this.targetVariable = 0f;
        }

        public override TaskStatus OnUpdate()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            float num = this.targetValue.Value;
            if (this.CDRatio.Value != 0f)
            {
                num *= this.CDRatio.Value;
            }
            this.targetVariable.SetValue(num * component.GetProperty("AI_AttackCDRatio"));
            return TaskStatus.Success;
        }
    }
}

