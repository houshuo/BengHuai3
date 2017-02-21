namespace BehaviorDesigner.Runtime.Tasks
{
    using System;

    [TaskIcon("{SkinColor}ParallelSelectorIcon.png"), TaskDescription("Only count on the first child!! Run all Children at the same time")]
    public class ParallelAttach : ParallelSelector
    {
        public override TaskStatus OverrideStatus(TaskStatus status)
        {
            bool flag = true;
            if (base.executionStatus.Length > 0)
            {
                if (base.executionStatus[0] == TaskStatus.Running)
                {
                    flag = false;
                }
                else if (base.executionStatus[0] == TaskStatus.Success)
                {
                    return TaskStatus.Success;
                }
            }
            return (!flag ? TaskStatus.Running : TaskStatus.Failure);
        }
    }
}

