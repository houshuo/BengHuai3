namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskDescription("Returns success if the variable value is equal to the compareTo value."), TaskCategory("Basic/SharedVariable")]
    public class LessThanSharedInt : Conditional
    {
        [Tooltip("The variable to compare to")]
        public SharedInt compareTo;
        [Tooltip("The first variable to compare")]
        public SharedInt variable;

        public override void OnReset()
        {
            this.variable = 0;
            this.compareTo = 0;
        }

        public override TaskStatus OnUpdate()
        {
            return ((this.variable.Value >= this.compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success);
        }
    }
}

