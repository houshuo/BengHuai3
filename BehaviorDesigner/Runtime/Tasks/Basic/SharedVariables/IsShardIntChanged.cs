namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Basic/SharedVariable"), TaskDescription("Returns success if the variable value is changed.")]
    public class IsShardIntChanged : Conditional
    {
        private int _originalValue;
        [Tooltip("The first variable to check")]
        public SharedInt variable;

        public override void OnAwake()
        {
            this._originalValue = this.variable.Value;
        }

        public override void OnReset()
        {
            this.variable = 0;
            this._originalValue = 0;
        }

        public override TaskStatus OnUpdate()
        {
            if (this._originalValue != this.variable.Value)
            {
                this._originalValue = this.variable.Value;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}

