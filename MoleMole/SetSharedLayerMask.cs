namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Basic/SharedVariable")]
    public class SetSharedLayerMask : BehaviorDesigner.Runtime.Tasks.Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The value to set the LayerMask to")]
        public SharedLayerMask targetValue;
        [RequiredField, BehaviorDesigner.Runtime.Tasks.Tooltip("The LayerMask to set")]
        public SharedLayerMask targetVariable;

        public override void OnReset()
        {
            this.targetValue = new LayerMask();
            this.targetVariable = new LayerMask();
        }

        public override TaskStatus OnUpdate()
        {
            this.targetVariable.Value = this.targetValue.Value;
            return TaskStatus.Success;
        }
    }
}

