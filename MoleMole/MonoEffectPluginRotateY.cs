namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginRotateY : BaseMonoEffectPlugin
    {
        private float _angleRotated;
        [Header("Loop")]
        public bool Loop;
        public float RotateYAnglePerSecond;
        [Header("Rotate Target")]
        public Transform targetTransform;
        public float TotalAngle;

        protected override void Awake()
        {
            base.Awake();
            if (this.targetTransform == null)
            {
                this.targetTransform = base.transform;
            }
            this._angleRotated = 0f;
        }

        public override bool IsToBeRemove()
        {
            return (!this.Loop && (this._angleRotated > this.TotalAngle));
        }

        public override void SetDestroy()
        {
            this._angleRotated = this.TotalAngle - (this.RotateYAnglePerSecond * 0.3f);
        }

        public override void Setup()
        {
            this._angleRotated = 0f;
        }

        public void Update()
        {
            Vector3 localEulerAngles = this.targetTransform.localEulerAngles;
            float num = (this.RotateYAnglePerSecond * Time.deltaTime) * base._effect.TimeScale;
            localEulerAngles.y += num;
            this.targetTransform.localEulerAngles = localEulerAngles;
            this._angleRotated += num;
        }
    }
}

