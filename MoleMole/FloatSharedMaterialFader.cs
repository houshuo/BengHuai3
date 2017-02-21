namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FloatSharedMaterialFader : RendererFader
    {
        private MaterialPropertyBlock _block;
        private float _origAlpha;

        public FloatSharedMaterialFader(Renderer renderer, string property)
        {
            base._renderer = renderer;
            base._propertyID = Shader.PropertyToID(property);
            this._origAlpha = base._renderer.sharedMaterial.GetFloat(base._propertyID);
        }

        public override void LerpAlpha(float t)
        {
            base._renderer.sharedMaterial.SetFloat(base._propertyID, this._origAlpha * t);
        }
    }
}

