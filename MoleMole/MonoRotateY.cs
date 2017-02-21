namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoRotateY : MonoBehaviour
    {
        private float _angleRotated;
        [Header("Loop")]
        public bool Loop;
        public float RotateYAnglePerSecond;
        [Header("Rotate Target")]
        public Transform targetTransform;
        public float TotalAngle;

        public void Awake()
        {
            if (this.targetTransform == null)
            {
                this.targetTransform = base.transform;
            }
            this._angleRotated = 0f;
        }

        public void Update()
        {
            Vector3 localEulerAngles = this.targetTransform.localEulerAngles;
            float num = this.RotateYAnglePerSecond * Time.deltaTime;
            localEulerAngles.y += num;
            this.targetTransform.localEulerAngles = localEulerAngles;
            this._angleRotated += num;
        }
    }
}

