namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoLightProbGroup : MonoLightProbEntityBase
    {
        private MonoLightProb[] _probs;

        public bool Evaluate(Vector3 pos, ref LightProbProperties ret)
        {
            if (Vector3.Distance(pos, base.XZPosition) > base.radius)
            {
                return false;
            }
            ret = new LightProbProperties();
            int num2 = 0;
            foreach (MonoLightProb prob in this._probs)
            {
                LightProbProperties properties = new LightProbProperties();
                if (prob.Evaluate(pos, base.properties, ref properties))
                {
                    num2++;
                    ret += properties;
                }
            }
            if (num2 != 0)
            {
                ret = (LightProbProperties) (ret / ((float) num2));
            }
            else
            {
                ret = base.properties;
            }
            return true;
        }

        public void Init()
        {
            this._probs = base.GetComponentsInChildren<MonoLightProb>();
        }
    }
}

