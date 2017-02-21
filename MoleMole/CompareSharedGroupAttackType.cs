namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Basic/SharedVariable"), TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class CompareSharedGroupAttackType : Conditional
    {
        [Tooltip("The variable to compare to")]
        public SharedGroupAttackType compareTo;
        [Tooltip("The first varible to compare")]
        public SharedGroupAttackType variable;

        public override void OnReset()
        {
            this.variable = 1;
            this.compareTo = 1;
        }

        public override TaskStatus OnUpdate()
        {
            return ((this.variable.Value != this.compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success);
        }
    }
}

