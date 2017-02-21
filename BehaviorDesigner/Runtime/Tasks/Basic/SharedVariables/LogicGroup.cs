namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;

    [TaskCategory("Basic/SharedVariable"), TaskDescription("Returns success if one of the compares success.")]
    public class LogicGroup
    {
        public List<CompareLogicGroup> compareGroupList;
        public LogicType logicType;

        public bool Result()
        {
            if (this.logicType == LogicType.And)
            {
                for (int i = 0; i < this.compareGroupList.Count; i++)
                {
                    if (!this.compareGroupList[i].Result())
                    {
                        return false;
                    }
                }
                return true;
            }
            if (this.logicType == LogicType.Or)
            {
                for (int j = 0; j < this.compareGroupList.Count; j++)
                {
                    if (this.compareGroupList[j].Result())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public enum LogicType
        {
            And,
            Or
        }
    }
}

