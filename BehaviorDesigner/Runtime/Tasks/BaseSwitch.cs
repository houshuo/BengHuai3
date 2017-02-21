namespace BehaviorDesigner.Runtime.Tasks
{
    using BehaviorDesigner.Runtime;
    using System;

    public abstract class BaseSwitch : Composite
    {
        protected int _currentChildIndex;
        protected TaskStatus _executionStatus;
        public bool InteruptChildOnSwitchChange;

        protected BaseSwitch()
        {
        }

        protected abstract int CalculateChildIndex();
        public override bool CanExecute()
        {
            return ((this._executionStatus != TaskStatus.Success) && (this._executionStatus != TaskStatus.Failure));
        }

        public override bool CanReevaluate()
        {
            return this.InteruptChildOnSwitchChange;
        }

        public override int CurrentChildIndex()
        {
            this._currentChildIndex = this.CalculateChildIndex();
            return this._currentChildIndex;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            this._executionStatus = childStatus;
        }

        public override void OnChildStarted()
        {
            if (this.InteruptChildOnSwitchChange)
            {
                this._executionStatus = TaskStatus.Running;
            }
        }

        public override void OnConditionalAbort(int childIndex)
        {
            this._currentChildIndex = this.CalculateChildIndex();
            this._executionStatus = TaskStatus.Inactive;
        }

        public override void OnEnd()
        {
            this._executionStatus = TaskStatus.Inactive;
        }

        public override void OnReevaluationEnded(TaskStatus status)
        {
        }

        public override bool OnReevaluationStarted()
        {
            if (this._executionStatus != TaskStatus.Inactive)
            {
                int num = this.CalculateChildIndex();
                if (num != this._currentChildIndex)
                {
                    BehaviorManager.instance.Interrupt(base.Owner, base.children[this._currentChildIndex], this);
                    this._currentChildIndex = num;
                    return true;
                }
            }
            return false;
        }
    }
}

