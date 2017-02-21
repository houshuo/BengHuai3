namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class LDDropCoin : LDDropDataItem
    {
        public int dropNum = 1;
        public float scoinReward;

        public override void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = true)
        {
            for (int i = 0; i < this.dropNum; i++)
            {
                Singleton<DynamicObjectManager>.Instance.CreateCoin(0x21800001, initPos, initDir, this.scoinReward, actDropAnim);
            }
        }
    }
}

