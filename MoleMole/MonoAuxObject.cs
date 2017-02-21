namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoAuxObject : MonoBehaviour
    {
        [NonSerialized]
        public string entryName;
        [NonSerialized]
        public uint ownerID;

        public void SetDestroy()
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }
}

