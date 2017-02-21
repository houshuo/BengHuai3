namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;
    using UnityEngine;

    public class UseCameraForward : EffectCreationOp
    {
        [NonSerialized, InspectorDisabled]
        public string type = "Use Camera Forward";

        public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
        {
            Vector3 forward = Singleton<CameraManager>.Instance.GetMainCamera().Forward;
            initDir = forward;
        }
    }
}

