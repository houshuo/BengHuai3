namespace MoleMole
{
    using System;
    using UnityEngine;

    public abstract class ShaderProperty_Base : MonoBehaviour
    {
        protected ShaderProperty_Base()
        {
        }

        public virtual void LerpTo(MaterialColorModifier.Multiplier multiplier, ShaderProperty_Base to_, float normalized)
        {
        }

        public abstract void LerpTo(Material targetMat, ShaderProperty_Base to, float normalized);
    }
}

