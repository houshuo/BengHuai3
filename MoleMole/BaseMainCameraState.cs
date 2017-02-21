namespace MoleMole
{
    using FullInspector;
    using System;
    using UnityEngine;

    [fiInspectorOnly]
    public class BaseMainCameraState : State<MonoMainCamera>
    {
        public Vector3 cameraForward;
        public float cameraFOV;
        public Vector3 cameraPosition;
        public float cameraShakeRatio;
        public bool lerpDirectionalLight;
        public bool muteCameraShake;

        public BaseMainCameraState(MonoMainCamera camera) : base(camera)
        {
            this.cameraShakeRatio = 1f;
        }
    }
}

