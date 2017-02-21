namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;
    using UnityEngine;

    public class SharedLayerMask : SharedVariable<LayerMask>
    {
        public static implicit operator SharedLayerMask(LayerMask value)
        {
            return new SharedLayerMask { mValue = value };
        }

        public override string ToString()
        {
            return base.mValue.ToString();
        }
    }
}

