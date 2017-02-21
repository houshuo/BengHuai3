namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using UnityEngine;

    public abstract class BaseRenderingProperty
    {
        public string propertyName;

        protected BaseRenderingProperty()
        {
        }

        public abstract void ApplyGlobally();
        public BaseRenderingProperty Clone()
        {
            return (BaseRenderingProperty) base.MemberwiseClone();
        }

        public abstract BaseInstancedRenderingProperty CreateInstancedProperty(Material material);
        public abstract void LerpStep(float t);
        public abstract void SetupTransition(BaseRenderingProperty target);
        public abstract void SimpleApplyOnMaterial(Material material);
    }
}

