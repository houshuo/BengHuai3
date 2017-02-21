namespace BehaviorDesigner.Runtime.Tasks
{
    using BehaviorDesigner.Runtime;
    using System;

    [TaskCategory("Switch")]
    public class SwitchByInt : BaseSwitch
    {
        public int[] cases;
        public SharedInt target;

        protected override int CalculateChildIndex()
        {
            for (int i = 0; i < this.cases.Length; i++)
            {
                if (this.cases[i] == this.target.Value)
                {
                    return i;
                }
            }
            return base._currentChildIndex;
        }

        public override void OnAwake()
        {
        }
    }
}

