namespace MoleMole
{
    using System;
    using UnityEngine;

    public class ContextIdentifier : MonoBehaviour
    {
        public BaseContext context;

        private void OnDestroy()
        {
            this.context = null;
        }
    }
}

