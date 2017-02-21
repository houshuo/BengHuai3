namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MaterialPropertyBlockFader
    {
        private MaterialPropertyBlock _mpb;
        private Color _originColor;
        private string _propertyName;
        private Renderer _renderer;

        public MaterialPropertyBlockFader(Renderer renderer, string name)
        {
            this._renderer = renderer;
            this._mpb = new MaterialPropertyBlock();
            this._propertyName = name;
            this._originColor = this._renderer.sharedMaterial.GetColor(name);
        }

        public void LerpAlpha(float t)
        {
            Color color = this._originColor;
            color.a *= t;
            this._renderer.GetPropertyBlock(this._mpb);
            this._mpb.SetColor(this._propertyName, color);
            this._renderer.SetPropertyBlock(this._mpb);
        }
    }
}

