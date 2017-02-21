namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class LDDropHPMedic : LDDropDataItem
    {
        public int dropNum = 1;
        public float healHP;

        public override void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = true)
        {
            for (int i = 0; i < this.dropNum; i++)
            {
                Singleton<DynamicObjectManager>.Instance.CreateHPMedic(0x21800001, initPos, initDir, this.healHP, actDropAnim);
            }
        }
    }
}

