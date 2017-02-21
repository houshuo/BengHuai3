namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Basic/SharedVariable"), TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class LessThanSharedFloatOR : Conditional
    {
        [Tooltip("The variable to compare to")]
        public SharedFloat compareTo;
        [Tooltip("The first variable to compare")]
        public SharedFloat variable01;
        [Tooltip("The first variable to compare")]
        public SharedFloat variable02;
        [Tooltip("The first variable to compare")]
        public SharedFloat variable03;

        public override void OnReset()
        {
            this.variable01 = 0f;
            this.variable02 = 0f;
            this.variable03 = 0f;
            this.compareTo = 0f;
        }

        public override TaskStatus OnUpdate()
        {
            if (((this.variable01.Value >= this.compareTo.Value) && (this.variable02.Value >= this.compareTo.Value)) && (this.variable03.Value >= this.compareTo.Value))
            {
                return TaskStatus.Failure;
            }
            return TaskStatus.Success;
        }
    }
}

