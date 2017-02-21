namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoTriangleZone2D : MonoSubZone2D
    {
        public float nXlen = 1f;
        public float pXlen = 1f;
        public float zlen = 1f;

        public override bool Contain(Vector3 pos)
        {
            Vector3 vector = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one).inverse.MultiplyPoint3x4(pos);
            this.validateInput();
            float num = 0f;
            if (vector.z < 0f)
            {
                return false;
            }
            if (vector.x >= 0f)
            {
                num = ((this.pXlen * vector.z) + (vector.x * this.zlen)) / (this.pXlen * this.zlen);
                if (num > 1f)
                {
                    return false;
                }
            }
            else
            {
                num = ((this.nXlen * vector.z) - (vector.x * this.zlen)) / (this.nXlen * this.zlen);
                if (num > 1f)
                {
                    return false;
                }
            }
            return true;
        }

        private void validateInput()
        {
            if (this.zlen < 0f)
            {
                this.zlen = -this.zlen;
            }
            if (this.pXlen < 0f)
            {
                this.pXlen = -this.pXlen;
            }
            if (this.nXlen < 0f)
            {
                this.nXlen = -this.nXlen;
            }
        }
    }
}

