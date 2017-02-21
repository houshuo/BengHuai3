namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoSubZone2D : MonoBehaviour
    {
        public virtual bool Contain(Vector3 pos)
        {
            return false;
        }
    }
}

