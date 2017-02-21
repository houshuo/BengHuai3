namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using UnityEngine;

    public class FloatRenderingProperty : BaseRenderingProperty
    {
        [NonSerialized]
        public float _fromValue;
        [NonSerialized]
        public float _toValue;
        public float max;
        public float min;
        public float value;

        public FloatRenderingProperty()
        {
        }

        public FloatRenderingProperty(string name, float value, float min, float max)
        {
            base.propertyName = name;
            this.value = value;
            this.min = min;
            this.max = max;
        }

        public override void ApplyGlobally()
        {
            Shader.SetGlobalFloat(base.propertyName, this.value);
        }

        public override BaseInstancedRenderingProperty CreateInstancedProperty(Material material)
        {
            return new FloatInstancedRenderingProperty { material = material, propertyID = Shader.PropertyToID(base.propertyName), value = this.value };
        }

        public override void LerpStep(float t)
        {
            this.value = Mathf.Lerp(this._fromValue, this._toValue, t);
        }

        public override void SetupTransition(BaseRenderingProperty target)
        {
            this._fromValue = this.value;
            this._toValue = (target == null) ? this.value : ((FloatRenderingProperty) target).value;
        }

        public override void SimpleApplyOnMaterial(Material material)
        {
            material.SetFloat(base.propertyName, this.value);
        }
    }
}

