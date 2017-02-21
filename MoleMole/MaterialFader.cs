namespace MoleMole
{
    using System;
    using UnityEngine;

    public abstract class MaterialFader : IAlphaFader
    {
        protected Material _material;
        protected int _propertyID;

        protected MaterialFader()
        {
        }

        public abstract void LerpAlpha(float t);
    }
}

