namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoCircleZone2D : MonoSubZone2D
    {
        public float radius = 1f;

        public override bool Contain(Vector3 pos)
        {
            return (Vector3.Distance(pos, this.XZPosition) < this.radius);
        }

        public Vector3 XZPosition
        {
            get
            {
                return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
            }
        }
    }
}

