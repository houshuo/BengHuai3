namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Basic/SharedVariable"), TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class CompareSharedLayerMask : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The variable to compare to")]
        public SharedLayerMask compareTo;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The first varible to compare")]
        public SharedLayerMask variable;

        public override void OnReset()
        {
            this.variable = new LayerMask();
            this.compareTo = new LayerMask();
        }

        public override TaskStatus OnUpdate()
        {
            return ((this.variable.Value != this.compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success);
        }
    }
}

