namespace MoleMole
{
    using System;
    using UnityEngine;

    public class ColorSharedMaterialFader : RendererFader
    {
        private float _origAlpha;

        public ColorSharedMaterialFader(Renderer renderer, string property)
        {
            base._renderer = renderer;
            base._propertyID = Shader.PropertyToID(property);
            this._origAlpha = renderer.sharedMaterial.GetColor(base._propertyID).a;
        }

        public override void LerpAlpha(float t)
        {
            Color color = base._renderer.sharedMaterial.GetColor(base._propertyID);
            color.a = this._origAlpha * t;
            base._renderer.sharedMaterial.SetColor(base._propertyID, color);
        }
    }
}

