namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class ColorInstancedRenderingProperty : BaseInstancedRenderingProperty
    {
        public Color _fromColor;
        public Color _toColor;
        public Color value;

        public override void ApplyProperty()
        {
            base.material.SetColor(base.propertyID, this.value);
        }

        public override void CopyFrom(BaseRenderingProperty target)
        {
            if (target != null)
            {
                this.value = ((ColorRenderingProperty) target).value;
            }
        }

        public override BaseRenderingProperty CreateBaseRenderingProperty(string name)
        {
            return new ColorRenderingProperty(name, this.value);
        }

        public override void LerpStep(float t)
        {
            this.value = Color.Lerp(this._fromColor, this._toColor, t);
        }

        public override void SetupTransition(BaseRenderingProperty target)
        {
            this._fromColor = this.value;
            this._toColor = (target == null) ? this.value : ((ColorRenderingProperty) target).value;
        }
    }
}

