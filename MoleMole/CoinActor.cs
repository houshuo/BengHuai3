namespace MoleMole
{
    using System;

    public class CoinActor : BaseGoodsActor
    {
        public float scoinReward;

        public override void DoGoodsLogic(uint avatarRuntimeID)
        {
            Singleton<AvatarManager>.Instance.GetLocalAvatar().PickupCoin(base.runtimeID);
            LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
            instance.scoinInside += this.scoinReward;
            base.Kill();
        }
    }
}

