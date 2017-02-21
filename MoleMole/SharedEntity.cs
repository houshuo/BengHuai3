namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;

    public class SharedEntity : SharedVariable<BaseMonoEntity>
    {
        public static implicit operator SharedEntity(BaseMonoEntity value)
        {
            return new SharedEntity { mValue = value };
        }

        public override string ToString()
        {
            return ((base.mValue == null) ? "<null entity>" : base.mValue.gameObject.name);
        }
    }
}

