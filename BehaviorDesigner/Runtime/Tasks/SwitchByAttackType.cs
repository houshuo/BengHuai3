namespace BehaviorDesigner.Runtime.Tasks
{
    using MoleMole;
    using MoleMole.Config;
    using System;

    [TaskCategory("Switch")]
    public class SwitchByAttackType : BaseSwitch
    {
        public ConfigGroupAIMinionOld.AttackType[] cases;
        public SharedGroupAttackType target;

        protected override int CalculateChildIndex()
        {
            for (int i = 0; i < this.cases.Length; i++)
            {
                if (this.cases[i] == ((ConfigGroupAIMinionOld.AttackType) this.target.Value))
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

