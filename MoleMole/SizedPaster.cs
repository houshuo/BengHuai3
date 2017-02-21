namespace MoleMole
{
    using System;
    using UnityEngine;

    public class SizedPaster : Paster
    {
        private float _origSize;
        public float MaxHeight = 4f;
        [Header("Min/Max Heights")]
        public float MinHeight = 1f;
        [Header("Use targetTransform's Y position for Y sizing")]
        public Transform TargetTransform;

        protected override void Start()
        {
            base.transform.forward = Vector3.down;
            base.Start();
            this._origSize = base.Size;
        }

        protected override void Update()
        {
            float num = (this.TargetTransform.position.y >= this.MinHeight) ? this.TargetTransform.position.y : this.MinHeight;
            base.Size = this._origSize * Mathf.Lerp(1f, 0.4f, (num - this.MinHeight) / (this.MaxHeight - this.MinHeight));
            Vector3 position = this.TargetTransform.position;
            position.y = num;
            base.transform.position = position;
            base.transform.forward = Vector3.down;
            base.Update();
        }
    }
}

