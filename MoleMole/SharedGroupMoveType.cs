namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using MoleMole.Config;
    using System;

    public class SharedGroupMoveType : SharedVariable<ConfigGroupAIMinionOld.MoveType>
    {
        public static implicit operator SharedGroupMoveType(ConfigGroupAIMinionOld.MoveType value)
        {
            return new SharedGroupMoveType { mValue = value };
        }

        public override string ToString()
        {
            return base.mValue.ToString();
        }
    }
}

