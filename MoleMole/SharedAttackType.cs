namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;

    public class SharedAttackType : SharedVariable<AttackType>
    {
        public static implicit operator SharedAttackType(AttackType value)
        {
            return new SharedAttackType { mValue = value };
        }

        public override string ToString()
        {
            return base.mValue.ToString();
        }
    }
}

