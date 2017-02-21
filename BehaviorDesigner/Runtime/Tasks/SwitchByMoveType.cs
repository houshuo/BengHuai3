namespace BehaviorDesigner.Runtime.Tasks
{
    using MoleMole;
    using MoleMole.Config;
    using System;

    [TaskCategory("Switch")]
    public class SwitchByMoveType : BaseSwitch
    {
        public ConfigGroupAIMinionOld.MoveType[] cases;
        public SharedGroupMoveType target;

        protected override int CalculateChildIndex()
        {
            for (int i = 0; i < this.cases.Length; i++)
            {
                if (this.cases[i] == ((ConfigGroupAIMinionOld.MoveType) this.target.Value))
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

