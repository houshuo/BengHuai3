namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using MoleMole.Config;
    using System;

    public class SharedGroupAttackType : SharedVariable<ConfigGroupAIMinionOld.AttackType>
    {
        public static implicit operator SharedGroupAttackType(ConfigGroupAIMinionOld.AttackType value)
        {
            return new SharedGroupAttackType { mValue = value };
        }

        public override string ToString()
        {
            return base.mValue.ToString();
        }
    }
}

