namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoLightTriangleProb : MonoLightProb
    {
        private Mesh meshDown;
        private Mesh meshUp;
        public float nXlen = 1f;
        public float pXlen = 1f;
        public float zlen = 1f;

        public override bool Evaluate(Vector3 pos, LightProbProperties defaultProperties, ref LightProbProperties ret)
        {
            Vector3 vector = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one).inverse.MultiplyPoint3x4(pos);
            this.validateInput();
            float a = 0f;
            float b = 0f;
            if (vector.z < 0f)
            {
                return false;
            }
            b = (this.zlen - (2f * vector.z)) / this.zlen;
            if (vector.x >= 0f)
            {
                a = ((this.pXlen * vector.z) + (vector.x * this.zlen)) / (this.pXlen * this.zlen);
                if (a > 1f)
                {
                    return false;
                }
            }
            else
            {
                a = ((this.nXlen * vector.z) - (vector.x * this.zlen)) / (this.nXlen * this.zlen);
                if (a > 1f)
                {
                    return false;
                }
            }
            ret = LightProbProperties.Lerp(defaultProperties, base.properties, base.attenuateCurve.Evaluate(Mathf.Max(a, b)));
            return true;
        }

        private void OnDrawGizmosSelected()
        {
            Color bodyColor = this.properties.bodyColor;
            Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
            this.UpdateMesh();
            bodyColor.a = 0.5f;
            Gizmos.color = bodyColor;
            Gizmos.DrawMesh(this.meshUp);
            Gizmos.DrawMesh(this.meshDown);
        }

        private void UpdateMesh()
        {
            if (this.meshUp == null)
            {
                this.meshUp = new Mesh();
            }
            if (this.meshDown == null)
            {
                this.meshDown = new Mesh();
            }
            this.validateInput();
            this.meshUp.vertices = new Vector3[] { new Vector3(this.pXlen, 0f, 0f), new Vector3(0f, 0f, this.zlen), new Vector3(-this.nXlen, 0f, 0f) };
            int[] numArray1 = new int[3];
            numArray1[1] = 1;
            numArray1[2] = 2;
            this.meshUp.triangles = numArray1;
            this.meshUp.normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up };
            this.meshDown.vertices = new Vector3[] { new Vector3(this.pXlen, 0f, 0f), new Vector3(0f, 0f, this.zlen), new Vector3(-this.nXlen, 0f, 0f) };
            int[] numArray2 = new int[3];
            numArray2[1] = 2;
            numArray2[2] = 1;
            this.meshDown.triangles = numArray2;
            this.meshDown.normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down };
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

