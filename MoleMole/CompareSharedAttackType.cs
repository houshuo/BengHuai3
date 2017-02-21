namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Basic/SharedVariable"), TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class CompareSharedAttackType : Conditional
    {
        [Tooltip("The variable to compare to")]
        public SharedAttackType compareTo;
        [Tooltip("The first varible to compare")]
        public SharedAttackType variable;

        public override void OnReset()
        {
            this.variable = 0;
            this.compareTo = 0;
        }

        public override TaskStatus OnUpdate()
        {
            return ((this.variable.Value != this.compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success);
        }
    }
}

