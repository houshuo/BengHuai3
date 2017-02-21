namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class LDDropEquipItem : LDDropDataItem
    {
        public int dropNum = 1;
        public int level = 1;
        public int metaId = 1;
        public int rarity = 1;

        public override void CreateDropGoods(Vector3 initPos, Vector3 initDir, bool actDropAnim = true)
        {
            for (int i = 0; i < this.dropNum; i++)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(this.metaId, 1);
                if (dummyStorageDataItem is WeaponDataItem)
                {
                    Singleton<DynamicObjectManager>.Instance.CreateEquipItem(0x21800001, this.metaId, initPos, initDir, actDropAnim, this.level);
                }
                else if (dummyStorageDataItem is StigmataDataItem)
                {
                    Singleton<DynamicObjectManager>.Instance.CreateStigmataItem(0x21800001, this.metaId, initPos, initDir, actDropAnim, this.level);
                }
                else if (dummyStorageDataItem is MaterialDataItem)
                {
                    Singleton<DynamicObjectManager>.Instance.CreateMaterialItem(0x21800001, this.metaId, initPos, initDir, actDropAnim, this.level);
                }
                else if (dummyStorageDataItem is AvatarFragmentDataItem)
                {
                    Singleton<DynamicObjectManager>.Instance.CreateAvatarFragmentItem(0x21800001, this.metaId, initPos, initDir, actDropAnim, this.level);
                }
            }
            Singleton<LevelScoreManager>.Instance.AddDropItem(this.metaId, this.level, this.dropNum);
        }
    }
}

