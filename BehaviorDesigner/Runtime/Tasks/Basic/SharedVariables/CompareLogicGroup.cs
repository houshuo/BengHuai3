namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
    using System;
    using System.Collections.Generic;

    public class CompareLogicGroup
    {
        public List<FloatCompareGroup> compareGroupList;
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

