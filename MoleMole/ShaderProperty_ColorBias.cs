namespace MoleMole
{
    using System;
    using UnityEngine;

    public class ShaderProperty_ColorBias : ShaderProperty_Base
    {
        private Color _originalColor = Color.white;
        public Color BiasColor;

        public override void LerpTo(MaterialColorModifier.Multiplier multiplier, ShaderProperty_Base to_, float normalized)
        {
            ShaderProperty_ColorBias bias = (ShaderProperty_ColorBias) to_;
            if (multiplier != null)
            {
                multiplier.mulColor = Color.Lerp(this.BiasColor, bias.BiasColor, normalized);
            }
        }

        public override void LerpTo(Material targetMat, ShaderProperty_Base to_, float normalized)
        {
            ShaderProperty_ColorBias bias = (ShaderProperty_ColorBias) to_;
            Color color = this._originalColor * Color.Lerp(this.BiasColor, bias.BiasColor, normalized);
            if (targetMat.HasProperty("_MainColor"))
            {
                targetMat.SetColor("_MainColor", color);
            }
            else
            {
                targetMat.color = color;
            }
        }

        public void SetOriginalColor(Color color)
        {
            this._originalColor = color;
        }
    }
}

