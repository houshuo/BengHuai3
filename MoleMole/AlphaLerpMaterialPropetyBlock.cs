namespace MoleMole
{
    using System;
    using UnityEngine;

    public class AlphaLerpMaterialPropetyBlock
    {
        private Color _color;
        private E_AlphaLerpDir _dir;
        private float _large_alpha;
        private float _little_alpha;
        private MaterialPropertyBlock _mpb;
        private string _propertyName;
        private Renderer _renderer;

        public AlphaLerpMaterialPropetyBlock(Renderer renderer, string colorPropertyName, float littleA, float largeA)
        {
            this._renderer = renderer;
            this._mpb = new MaterialPropertyBlock();
            this._propertyName = colorPropertyName;
            this._little_alpha = littleA;
            this._large_alpha = largeA;
            this._color = this._renderer.sharedMaterial.GetColor(this._propertyName);
        }

        public void LerpAlpha(float t)
        {
            float a = (this._dir != E_AlphaLerpDir.ToLarge) ? this._large_alpha : this._little_alpha;
            float b = (this._dir != E_AlphaLerpDir.ToLarge) ? this._little_alpha : this._large_alpha;
            this._color.a = Mathf.Lerp(a, b, t);
            this._renderer.GetPropertyBlock(this._mpb);
            this._mpb.SetColor(this._propertyName, this._color);
            this._renderer.SetPropertyBlock(this._mpb);
        }

        public void SetAlpha(float alpha)
        {
            this._color.a = alpha;
            this._renderer.GetPropertyBlock(this._mpb);
            this._mpb.SetColor(this._propertyName, this._color);
            this._renderer.SetPropertyBlock(this._mpb);
        }

        public void SetDir(E_AlphaLerpDir dir)
        {
            this._dir = dir;
        }
    }
}

