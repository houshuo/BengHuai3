namespace MoleMole
{
    using System;

    public class HPMedicActor : BaseGoodsActor
    {
        public float healHP;

        public override void DoGoodsLogic(uint avatarRuntimeID)
        {
            Singleton<AvatarManager>.Instance.GetLocalAvatar().PickHPMedic(base.runtimeID);
            foreach (BaseMonoAvatar avatar2 in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
            {
                ((AvatarActor) Singleton<EventManager>.Instance.GetActor(avatar2.GetRuntimeID())).HealHP(this.healHP);
            }
            base.Kill();
        }
    }
}

