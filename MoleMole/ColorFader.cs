namespace MoleMole
{
    using System;
    using UnityEngine;

    public class ColorFader : MaterialFader
    {
        private Color _origColor;

        public ColorFader(Material material, string property)
        {
            base._material = material;
            base._propertyID = Shader.PropertyToID(property);
            this._origColor = base._material.GetColor(base._propertyID);
        }

        public override void LerpAlpha(float t)
        {
            Color color = this._origColor;
            color.a *= t;
            base._material.SetColor(base._propertyID, color);
        }
    }
}

