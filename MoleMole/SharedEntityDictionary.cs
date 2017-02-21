namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;
    using System.Collections.Generic;

    public class SharedEntityDictionary : SharedVariable<Dictionary<int, BaseMonoEntity>>
    {
        public static implicit operator SharedEntityDictionary(Dictionary<int, BaseMonoEntity> value)
        {
            return new SharedEntityDictionary { mValue = value };
        }

        public override string ToString()
        {
            return ((base.mValue != null) ? (base.mValue.Count + " BaseMonoEntity") : "null");
        }
    }
}

