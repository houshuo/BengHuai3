namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FloatFader : MaterialFader
    {
        private float _origAlpha;

        public FloatFader(Material material, string property)
        {
            base._material = material;
            base._propertyID = Shader.PropertyToID(property);
            this._origAlpha = base._material.GetFloat(base._propertyID);
        }

        public override void LerpAlpha(float t)
        {
            base._material.SetFloat(base._propertyID, this._origAlpha * t);
        }
    }
}

