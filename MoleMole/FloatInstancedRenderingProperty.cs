namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class FloatInstancedRenderingProperty : BaseInstancedRenderingProperty
    {
        private float _fromFloat;
        private float _toFloat;
        private const float DEFAULT_MAX = 100f;
        private const float DEFAULT_MIN = 0f;
        public float value;

        public override void ApplyProperty()
        {
            base.material.SetFloat(base.propertyID, this.value);
        }

        public override void CopyFrom(BaseRenderingProperty target)
        {
            if (target != null)
            {
                this.value = ((FloatRenderingProperty) target).value;
            }
        }

        public override BaseRenderingProperty CreateBaseRenderingProperty(string name)
        {
            return new FloatRenderingProperty(name, this.value, 0f, 100f);
        }

        public override void LerpStep(float t)
        {
            this.value = Mathf.Lerp(this._fromFloat, this._toFloat, t);
        }

        public override void SetupTransition(BaseRenderingProperty target)
        {
            this._fromFloat = this.value;
            this._toFloat = (target == null) ? this.value : ((FloatRenderingProperty) target).value;
        }
    }
}

