namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoKianaWithFire : MonoKiana_C5
    {
        public GameObject fireObject;

        private void SetBackFireVisible(int show)
        {
            bool flag = show != 0;
            if (this.fireObject != null)
            {
                this.fireObject.SetActive(flag);
            }
        }
    }
}

