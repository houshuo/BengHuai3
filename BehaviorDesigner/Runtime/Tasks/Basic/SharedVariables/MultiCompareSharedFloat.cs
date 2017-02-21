namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    public class MultiCompareSharedFloat : Conditional
    {
        public LogicGroup logicGroup;

        public override void OnReset()
        {
        }

        public override TaskStatus OnUpdate()
        {
            if (this.logicGroup.Result())
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}

