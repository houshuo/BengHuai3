namespace MoleMole
{
    using System;
    using UnityEngine;

    public class HeadTailPaster : Paster
    {
        private float _origSize;
        [Header("Head Target Transform")]
        public Transform HeadTransform;
        [Header("Tail Target Transform")]
        public Transform TailTransform;

        protected override void Start()
        {
            base.Start();
            this._origSize = base.Size;
        }

        protected override void Update()
        {
            Vector3 vector = (Vector3) ((this.HeadTransform.position + this.TailTransform.position) * 0.5f);
            Vector3 vector3 = this.HeadTransform.position - this.TailTransform.position;
            Vector3 normalized = vector3.normalized;
            float num = (vector.y >= 1f) ? vector.y : 1f;
            base.Size = this._origSize * Mathf.Lerp(1f, 0.4f, (num - 1f) / 3f);
            float y = vector.y;
            vector.y = 0f;
            normalized.y = 0f;
            base._pasterTrsf.position = vector;
            base._pasterTrsf.forward = normalized;
            base._pasterTrsf.localScale = (Vector3) (base.transform.localScale * base.Size);
            float num3 = Mathf.Clamp01(1f - ((y - base.FalloffStartDistance) / base.FalloffEndDistance));
            base._material.SetFloat("_Falloff", num3);
        }
    }
}

