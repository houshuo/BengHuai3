namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskDescription("Returns success if the variable value is equal to the compareTo value."), TaskCategory("Basic/SharedVariable")]
    public class CompareSharedGroupMoveType : Conditional
    {
        [Tooltip("The variable to compare to")]
        public SharedGroupMoveType compareTo;
        [Tooltip("The first varible to compare")]
        public SharedGroupMoveType variable;

        public override void OnReset()
        {
            this.variable = 2;
            this.compareTo = 2;
        }

        public override TaskStatus OnUpdate()
        {
            return ((this.variable.Value != this.compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success);
        }
    }
}

