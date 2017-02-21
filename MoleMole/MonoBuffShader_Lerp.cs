namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoBuffShader_Lerp : MonoBuffShader_Base
    {
        public float DisableDuration = 0.5f;
        public float EnableDuration = 0.5f;
        public ShaderProperty_Base FromProperty;
        public string Keyword = string.Empty;
        public AnimationCurve LerpCurve;
        public string NewShaderName = string.Empty;
        public Texture NewTexture;
        public string TexturePropertyName = string.Empty;
        public ShaderProperty_Base ToProperty;

        public void Lerp<T>(MaterialColorModifier.Multiplier multiplier, float normalized, bool dir) where T: ShaderProperty_Base
        {
            T local = !dir ? ((T) this.ToProperty) : ((T) this.FromProperty);
            T local2 = !dir ? ((T) this.FromProperty) : ((T) this.ToProperty);
            local.LerpTo(multiplier, local2, this.LerpCurve.Evaluate(normalized));
        }

        public void Lerp<T>(Material targetMat, float normalized, bool dir) where T: ShaderProperty_Base
        {
            T local = !dir ? ((T) this.ToProperty) : ((T) this.FromProperty);
            T to = !dir ? ((T) this.FromProperty) : ((T) this.ToProperty);
            local.LerpTo(targetMat, to, this.LerpCurve.Evaluate(normalized));
        }
    }
}

