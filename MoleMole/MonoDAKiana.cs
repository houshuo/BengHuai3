namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoDAKiana : BaseMonoDarkAvatar
    {
        public Renderer LeftWeapon;
        public Renderer RightWeapon;

        private void SetWeaponVisible(int show)
        {
            bool flag = show != 0;
            if (this.LeftWeapon != null)
            {
                this.LeftWeapon.enabled = flag;
            }
            if (this.RightWeapon != null)
            {
                this.RightWeapon.enabled = flag;
            }
        }
    }
}

