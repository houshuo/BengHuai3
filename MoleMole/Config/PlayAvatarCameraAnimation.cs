namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class PlayAvatarCameraAnimation : ConfigAvatarCameraAction
    {
        public string CameraAnimName;
        public MoleMole.MainCameraFollowState.EnterPolarMode EnterPolarMode;
        public bool ExitTransitionLerp = true;
    }
}

