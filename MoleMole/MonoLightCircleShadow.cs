namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoLightCircleShadow : MonoLightShadow
    {
        public float radius = 1f;

        public override float Evaluate(Vector3 pos)
        {
            float num = Vector3.Distance(pos, base.XZPosition);
            if (num > this.radius)
            {
                return 0f;
            }
            return base.attenuateCurve.Evaluate(num / this.radius);
        }

        private void OnDrawGizmosSelected()
        {
            Color color = new Color(0.5f, 0.5f, 0.5f) {
                a = 0.3f
            };
            Gizmos.color = color;
            Gizmos.DrawSphere(base.transform.position, this.radius);
        }
    }
}

