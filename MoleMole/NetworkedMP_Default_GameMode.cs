namespace MoleMole
{
    using System;

    public class NetworkedMP_Default_GameMode : OriginalSPGameMode
    {
        public override void HandleLocalPlayerAvatarDie(BaseMonoAvatar diedAvatar)
        {
            diedAvatar.gameObject.SetActive(false);
            Singleton<CameraManager>.Instance.GetMainCamera().TransitToStatic();
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(false, false, false);
        }

        public override bool ShouldAttackPatternSendBeingHit(uint beHitEntityID)
        {
            return !Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(beHitEntityID);
        }
    }
}

