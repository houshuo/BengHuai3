namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskDescription("The force sequence task is similar to sequence. But it will never return failure! It will run all children tasks whatever the children return"), TaskIcon("{SkinColor}SequenceIcon.png")]
    public class ForceSequence : Composite
    {
        private int currentChildIndex;
        protected TaskStatus executionStatus;

        public override bool CanExecute()
        {
            return (this.currentChildIndex < base.children.Count);
        }

        public override int CurrentChildIndex()
        {
            return this.currentChildIndex;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            this.currentChildIndex++;
            this.executionStatus = childStatus;
        }

        public override void OnConditionalAbort(int childIndex)
        {
            this.currentChildIndex = childIndex;
            this.executionStatus = TaskStatus.Inactive;
        }

        public override void OnEnd()
        {
            this.executionStatus = TaskStatus.Inactive;
            this.currentChildIndex = 0;
        }
    }
}

