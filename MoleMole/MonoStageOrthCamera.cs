namespace MoleMole
{
    using System;
    using UnityEngine;

    public sealed class MonoStageOrthCamera : BaseMonoCamera
    {
        private Vector3 _origOrthCameraPosition;

        public void Init(uint runtimeID)
        {
            base.Init(3, runtimeID);
            base._cameraTrans.SetPositionY(-100f);
            this._origOrthCameraPosition = base._cameraTrans.transform.position;
        }

        public void ResumePosition()
        {
            base.transform.position = this._origOrthCameraPosition;
        }

        public void SetShake(Vector3 shakeOffset)
        {
            Vector3 vector = base.transform.TransformDirection(shakeOffset);
            base._cameraTrans.position = this._origOrthCameraPosition + ((Vector3) (vector * 0.15f));
        }

        public override void Update()
        {
            base.Update();
        }
    }
}

