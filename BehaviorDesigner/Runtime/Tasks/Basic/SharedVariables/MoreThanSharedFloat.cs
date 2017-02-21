namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskDescription("Returns success if the variable value is equal to the compareTo value."), TaskCategory("Basic/SharedVariable")]
    public class MoreThanSharedFloat : Conditional
    {
        [Tooltip("The variable to compare to")]
        public SharedFloat compareTo;
        [Tooltip("The first variable to compare")]
        public SharedFloat variable;

        public override void OnReset()
        {
            this.variable = 0f;
            this.compareTo = 0f;
        }

        public override TaskStatus OnUpdate()
        {
            return ((this.variable.Value <= this.compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success);
        }
    }
}

