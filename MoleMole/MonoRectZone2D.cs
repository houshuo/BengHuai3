namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoRectZone2D : MonoSubZone2D
    {
        public float xlen = 1f;
        public float zlen = 1f;

        public override bool Contain(Vector3 pos)
        {
            Vector3 vector = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one).inverse.MultiplyPoint3x4(pos);
            float num = 0.5f * this.xlen;
            float num2 = 0.5f * this.zlen;
            return (((vector.x <= num) && (vector.x >= -num)) && ((vector.z <= num2) && (vector.z >= -num2)));
        }

        private void OnDrawGizmosSelected()
        {
            Color green = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
            green.a = 0.3f;
            Gizmos.color = green;
            Gizmos.DrawCube(new Vector3(0f, 0f, 0f), new Vector3(this.xlen, 0.1f, this.zlen));
        }
    }
}

