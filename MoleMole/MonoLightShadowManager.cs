namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoLightShadowManager : MonoBehaviour
    {
        private MonoLightShadowGroup[] _groups;
        public LightProbProperties properties;

        public bool Evaluate(Vector3 pos, ref LightProbProperties ret)
        {
            if (this._groups == null)
            {
                return false;
            }
            ret = new LightProbProperties();
            int num = 0;
            foreach (MonoLightShadowGroup group in this._groups)
            {
                LightProbProperties properties = new LightProbProperties();
                if (group.Evaluate(pos, ref properties))
                {
                    num++;
                    ret += properties;
                }
            }
            if (num != 0)
            {
                ret = (LightProbProperties) (ret / ((float) num));
            }
            else
            {
                ret = this.properties;
            }
            return true;
        }

        public void Init()
        {
            MonoLightShadowGroup[] componentsInChildren = base.GetComponentsInChildren<MonoLightShadowGroup>(true);
            List<MonoLightShadowGroup> list = new List<MonoLightShadowGroup>();
            foreach (MonoLightShadowGroup group in componentsInChildren)
            {
                if (Mathf.Abs(group.transform.position.y) < 0.2f)
                {
                    group.Init();
                    list.Add(group);
                }
                else
                {
                    group.gameObject.SetActive(false);
                }
            }
            this._groups = list.ToArray();
        }

        public void Reset()
        {
            this.Init();
        }
    }
}

