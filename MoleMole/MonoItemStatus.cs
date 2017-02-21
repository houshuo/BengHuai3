namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoItemStatus : MonoBehaviour
    {
        public bool isValid = true;

        private void Awake()
        {
            this.isValid = true;
        }
    }
}

