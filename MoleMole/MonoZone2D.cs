namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoZone2D : MonoBehaviour
    {
        private MonoSubZone2D[] _zones;

        private void Awake()
        {
            this.Init();
        }

        public bool Contain(Vector3 pos)
        {
            for (int i = 0; i < this._zones.Length; i++)
            {
                if (this._zones[i].Contain(pos))
                {
                    return true;
                }
            }
            return false;
        }

        public void Init()
        {
            this._zones = base.GetComponentsInChildren<MonoSubZone2D>(true);
        }

        public void Reset()
        {
            this.Init();
        }
    }
}

