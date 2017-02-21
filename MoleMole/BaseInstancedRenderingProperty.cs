namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public abstract class BaseInstancedRenderingProperty : IDisposable
    {
        public Material material;
        public int propertyID;

        protected BaseInstancedRenderingProperty()
        {
        }

        public abstract void ApplyProperty();
        public abstract void CopyFrom(BaseRenderingProperty target);
        public abstract BaseRenderingProperty CreateBaseRenderingProperty(string name);
        public void Dispose()
        {
            UnityEngine.Object.Destroy(this.material);
        }

        public abstract void LerpStep(float t);
        public abstract void SetupTransition(BaseRenderingProperty target);
    }
}

