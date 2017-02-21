namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
    using BehaviorDesigner.Runtime;
    using System;

    public class FloatCompareGroup
    {
        public SharedFloat compareTo;
        public CompareType compareType;
        public SharedFloat variable;

        public bool Result()
        {
            switch (this.compareType)
            {
                case CompareType.MoreThan:
                    return (this.variable.Value > this.compareTo.Value);

                case CompareType.LessThan:
                    return (this.variable.Value < this.compareTo.Value);

                case CompareType.Equal:
                    return (this.variable.Value == this.compareTo.Value);
            }
            return false;
        }

        public enum CompareType
        {
            MoreThan,
            LessThan,
            Equal
        }
    }
}

