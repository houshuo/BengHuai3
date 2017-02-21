namespace MoleMole
{
    using System;

    public class SPMedicActor : BaseGoodsActor
    {
        public float healSP;

        public override void DoGoodsLogic(uint avatarRuntimeID)
        {
            Singleton<AvatarManager>.Instance.GetLocalAvatar().FireEffect("Ability_HealSP_Pick");
            foreach (BaseMonoAvatar avatar2 in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
            {
                ((AvatarActor) Singleton<EventManager>.Instance.GetActor(avatar2.GetRuntimeID())).HealSP(this.healSP);
            }
            base.Kill();
        }
    }
}

