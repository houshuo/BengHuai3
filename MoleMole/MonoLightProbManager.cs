namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoLightProbManager : MonoBehaviour
    {
        private MonoLightProbGroup[] _groups;
        public LightProbProperties properties;

        public bool Evaluate(Vector3 pos, ref LightProbProperties ret)
        {
            if (this._groups == null)
            {
                return false;
            }
            ret = new LightProbProperties();
            int num = 0;
            foreach (MonoLightProbGroup group in this._groups)
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
            MonoLightProbGroup[] componentsInChildren = base.GetComponentsInChildren<MonoLightProbGroup>(true);
            List<MonoLightProbGroup> list = new List<MonoLightProbGroup>();
            foreach (MonoLightProbGroup group in componentsInChildren)
            {
                if (Mathf.Abs(group.transform.position.y) < 2f)
                {
                    group.Init();
                    list.Add(group);
                    group.gameObject.SetActive(true);
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

