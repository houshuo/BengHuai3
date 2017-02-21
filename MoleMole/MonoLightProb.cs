namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoLightProb : MonoLightProbEntityBase
    {
        public AnimationCurve attenuateCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        public virtual bool Evaluate(Vector3 pos, LightProbProperties defaultProperties, ref LightProbProperties ret)
        {
            float num = Vector3.Distance(pos, base.XZPosition);
            if (num > base.radius)
            {
                return false;
            }
            ret = LightProbProperties.Lerp(defaultProperties, base.properties, this.attenuateCurve.Evaluate(num / base.radius));
            return true;
        }
    }
}

