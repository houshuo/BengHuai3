namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using UnityEngine;

    public class ColorRenderingProperty : BaseRenderingProperty
    {
        [NonSerialized]
        public Color _fromValue;
        [NonSerialized]
        public Color _toValue;
        public Color value;

        public ColorRenderingProperty()
        {
        }

        public ColorRenderingProperty(string name, Color value)
        {
            base.propertyName = name;
            this.value = value;
        }

        public override void ApplyGlobally()
        {
            Shader.SetGlobalColor(base.propertyName, this.value);
        }

        public override BaseInstancedRenderingProperty CreateInstancedProperty(Material material)
        {
            return new ColorInstancedRenderingProperty { material = material, propertyID = Shader.PropertyToID(base.propertyName), value = this.value };
        }

        public override void LerpStep(float t)
        {
            this.value = Color.Lerp(this._fromValue, this._toValue, t);
        }

        public override void SetupTransition(BaseRenderingProperty target)
        {
            this._fromValue = this.value;
            this._toValue = (target == null) ? this.value : ((ColorRenderingProperty) target).value;
        }

        public override void SimpleApplyOnMaterial(Material material)
        {
            material.SetColor(base.propertyName, this.value);
        }
    }
}

