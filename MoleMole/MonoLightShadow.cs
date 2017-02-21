namespace MoleMole
{
    using System;
    using UnityEngine;

    public abstract class MonoLightShadow : MonoBehaviour
    {
        public AnimationCurve attenuateCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        protected MonoLightShadow()
        {
        }

        public virtual float Evaluate(Vector3 pos)
        {
            return 0f;
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

