namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoLightShadowGroup : MonoBehaviour
    {
        private MonoLightShadow[] _probs;
        public LightProbProperties baseProperties;
        public float radius = 1f;
        public LightProbProperties shadowProperties;

        public bool Evaluate(Vector3 pos, ref LightProbProperties ret)
        {
            if (Vector3.Distance(pos, this.XZPosition) > this.radius)
            {
                return false;
            }
            ret = new LightProbProperties();
            float t = 0f;
            foreach (MonoLightShadow shadow in this._probs)
            {
                float num4 = shadow.Evaluate(pos);
                if (num4 > t)
                {
                    t = num4;
                }
            }
            ret = LightProbProperties.Lerp(this.baseProperties, this.shadowProperties, t);
            return true;
        }

        public void Init()
        {
            this._probs = base.GetComponentsInChildren<MonoLightShadow>();
        }

        private void OnDrawGizmosSelected()
        {
            Color bodyColor = this.baseProperties.bodyColor;
            bodyColor.a = 0.5f;
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

