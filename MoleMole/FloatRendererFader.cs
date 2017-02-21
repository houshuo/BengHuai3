namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FloatRendererFader : RendererFader
    {
        private MaterialPropertyBlock _block;
        private float _origAlpha;

        public FloatRendererFader(Renderer renderer, string property)
        {
            base._renderer = renderer;
            base._propertyID = Shader.PropertyToID(property);
            this._origAlpha = base._renderer.sharedMaterial.GetFloat(base._propertyID);
            this._block = new MaterialPropertyBlock();
        }

        public override void LerpAlpha(float t)
        {
            base._renderer.GetPropertyBlock(this._block);
            this._block.SetFloat(base._propertyID, this._origAlpha * t);
            base._renderer.SetPropertyBlock(this._block);
        }
    }
}

