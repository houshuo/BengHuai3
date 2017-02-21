namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MainCameraCinemaState : BaseMainCameraState
    {
        private ICinema _cinema;

        public MainCameraCinemaState(MonoMainCamera camera) : base(camera)
        {
        }

        public override void Enter()
        {
            base.cameraPosition = base._owner.transform.position;
            base.cameraForward = base._owner.transform.forward;
            base.cameraFOV = base._owner.originalFOV;
            if (this._cinema.GetInitCameraClipZNear() > 0f)
            {
                base._owner.cameraComponent.nearClipPlane = Mathf.Max(0.01f, this._cinema.GetInitCameraClipZNear());
            }
            if (this._cinema.GetInitCameraFOV() > 0f)
            {
                base._owner.cameraComponent.fieldOfView = this._cinema.GetInitCameraFOV();
            }
            Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(true);
        }

        public override void Exit()
        {
            base._owner.cameraComponent.nearClipPlane = base._owner.originalNearClip;
            base._owner.cameraComponent.fieldOfView = base._owner.originalFOV;
            Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(false);
        }

        public void SetCinema(ICinema cinema)
        {
            this._cinema = cinema;
        }

        public override void Update()
        {
            Transform cameraTransform = this._cinema.GetCameraTransform();
            if (cameraTransform != null)
            {
                base.cameraPosition = cameraTransform.position;
                base.cameraForward = cameraTransform.forward;
            }
        }
    }
}

