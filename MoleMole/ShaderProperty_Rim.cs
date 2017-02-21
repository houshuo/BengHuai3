namespace MoleMole
{
    using System;
    using UnityEngine;

    public class ShaderProperty_Rim : ShaderProperty_Base
    {
        public float _RGBias;
        public float _RGBloomFactor;
        public Color _RGColor;
        public float _RGRatio;
        public float _RGScale;
        public float _RGShininess;

        public override void LerpTo(Material targetMat, ShaderProperty_Base to_, float normalized)
        {
            ShaderProperty_Rim rim = (ShaderProperty_Rim) to_;
            targetMat.SetColor("_RGColor", Color.Lerp(this._RGColor, rim._RGColor, normalized));
            targetMat.SetFloat("_RGShininess", Mathf.Lerp(this._RGShininess, rim._RGShininess, normalized));
            targetMat.SetFloat("_RGScale", Mathf.Lerp(this._RGScale, rim._RGScale, normalized));
            targetMat.SetFloat("_RGBias", Mathf.Lerp(this._RGBias, rim._RGBias, normalized));
            targetMat.SetFloat("_RGRatio", Mathf.Lerp(this._RGRatio, rim._RGRatio, normalized));
            targetMat.SetFloat("_RGBloomFactor", Mathf.Lerp(this._RGBloomFactor, rim._RGBloomFactor, normalized));
        }
    }
}

