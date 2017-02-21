namespace MoleMole
{
    using System;
    using UnityEngine;

    public class ColorRendererFader : RendererFader
    {
        private MaterialPropertyBlock _block;
        private float _origAlpha;
        private Color _origColor;

        public ColorRendererFader(Renderer renderer, string property)
        {
            base._renderer = renderer;
            base._propertyID = Shader.PropertyToID(property);
            this._origColor = renderer.sharedMaterial.GetColor(base._propertyID);
            this._origAlpha = this._origColor.a;
            this._block = new MaterialPropertyBlock();
        }

        public override void LerpAlpha(float t)
        {
            base._renderer.GetPropertyBlock(this._block);
            Vector4 vector = this._block.GetVector(base._propertyID);
            Color color = !(vector == Vector4.zero) ? vector : this._origColor;
            color.a = this._origAlpha * t;
            this._block.SetColor(base._propertyID, color);
            base._renderer.SetPropertyBlock(this._block);
        }
    }
}

