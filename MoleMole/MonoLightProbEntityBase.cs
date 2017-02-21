namespace MoleMole
{
    using System;
    using UnityEngine;

    public abstract class MonoLightProbEntityBase : MonoBehaviour
    {
        public LightProbProperties properties;
        public float radius = 1f;

        protected MonoLightProbEntityBase()
        {
        }

        private void OnDrawGizmosSelected()
        {
            Color bodyColor = this.properties.bodyColor;
            bodyColor.a = 1f;
            Gizmos.color = bodyColor;
            Gizmos.DrawSphere(base.transform.position, 0.5f);
            bodyColor.a = 0.3f;
            Gizmos.color = bodyColor;
            Gizmos.DrawSphere(base.transform.position, this.radius);
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

