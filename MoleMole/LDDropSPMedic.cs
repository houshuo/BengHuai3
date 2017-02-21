namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class LDDropSPMedic : LDDropDataItem
    {
        public int dropNum = 1;
        public float healSP;

        public override void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = true)
        {
            for (int i = 0; i < this.dropNum; i++)
            {
                Singleton<DynamicObjectManager>.Instance.CreateSPMedic(0x21800001, initPos, initDir, this.healSP, actDropAnim);
            }
        }
    }
}

