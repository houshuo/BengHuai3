namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;

    [TaskCategory("Group")]
    public class SetSharedEntityVariable : BehaviorDesigner.Runtime.Tasks.Action
    {
        public List<SetVariable> SetVariableList;
        public SharedEntity targetEntity;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            if (this.targetEntity.Value == null)
            {
                return TaskStatus.Failure;
            }
            BehaviorDesigner.Runtime.BehaviorTree component = this.targetEntity.Value.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            for (int i = 0; i < this.SetVariableList.Count; i++)
            {
                switch (this.SetVariableList[i].variableType)
                {
                    case SetVariable.VariableType.Bool:
                        component.SetVariableValue(this.SetVariableList[i].variableName, this.SetVariableList[i].boolValue);
                        break;

                    case SetVariable.VariableType.Int:
                        component.SetVariableValue(this.SetVariableList[i].variableName, this.SetVariableList[i].intValue);
                        break;

                    case SetVariable.VariableType.Float:
                        component.SetVariableValue(this.SetVariableList[i].variableName, this.SetVariableList[i].floatValue);
                        break;
                }
            }
            return TaskStatus.Success;
        }

        public class SetVariable
        {
            public bool boolValue;
            public float floatValue;
            public int intValue;
            public string variableName;
            public VariableType variableType;

            public enum VariableType
            {
                Bool,
                Int,
                Float
            }
        }
    }
}

