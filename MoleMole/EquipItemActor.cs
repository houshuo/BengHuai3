namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;

    public class EquipItemActor : BaseGoodsActor
    {
        public override void DoGoodsLogic(uint avatarRuntimeID)
        {
            Singleton<AvatarManager>.Instance.GetLocalAvatar().PickupEquipItem(this.rarity, base.runtimeID);
            if (base._entity.DropItemMetaID != -1)
            {
                Singleton<LevelScoreManager>.Instance.AddDropItemToShow(base._entity.DropItemMetaID, base._entity.DropItemLevel, base._entity.DropItemNum);
            }
            base.Kill();
        }

        public int rarity { get; set; }
    }
}

