namespace MoleMole
{
    using System;

    public class MainCameraStaticState : BaseMainCameraState
    {
        public MainCameraStaticState(MonoMainCamera camera) : base(camera)
        {
        }

        public override void Enter()
        {
            base.cameraPosition = base._owner.transform.position;
            base.cameraForward = base._owner.transform.forward;
            base.cameraFOV = base._owner.originalFOV;
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
        }
    }
}

