namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Basic/SharedVariable")]
    public class SharedFloatLogic : BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedFloat BaseValue;
        public LogicType Logic;
        public SharedFloat ParamValue;

        public override TaskStatus OnUpdate()
        {
            switch (this.Logic)
            {
                case LogicType.Add:
                    this.BaseValue.Value += this.ParamValue.Value;
                    break;

                case LogicType.Multiple:
                    this.BaseValue.Value *= this.ParamValue.Value;
                    break;
            }
            return TaskStatus.Success;
        }

        public enum LogicType
        {
            Add,
            Multiple
        }
    }
}

