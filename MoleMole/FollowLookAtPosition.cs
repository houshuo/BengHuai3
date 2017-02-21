namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class FollowLookAtPosition : BaseFollowShortState
    {
        private bool _mute;
        private Vector3 _targetPosition;
        private float _timer;
        private float MAX_FOLLOW_TIME;

        public FollowLookAtPosition(MainCameraFollowState followState) : base(followState)
        {
            this.MAX_FOLLOW_TIME = 2f;
            base.isSkippingBaseState = false;
        }

        public override void Enter()
        {
            if (Singleton<LevelDesignManager>.Instance.IsPointInCameraFov(this._targetPosition))
            {
                base.End();
            }
            this._timer = this.MAX_FOLLOW_TIME;
            if (base._owner.recoverState.active)
            {
                base._owner.recoverState.CancelLerpRatioRecovering();
            }
            if (this._mute)
            {
                Singleton<MainUIManager>.Instance.CurrentPageContext.SetActive(false);
                Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(true);
            }
        }

        public override void Exit()
        {
            base._owner.posLerpRatio = 1f;
            base._owner.forwardLerpRatio = 1f;
            if (this._mute)
            {
                Singleton<MainUIManager>.Instance.CurrentPageContext.SetActive(true);
                Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(false);
            }
        }

        public override void PostUpdate()
        {
            this._timer -= Time.deltaTime;
            Vector3 vector = this._targetPosition - base._owner.cameraPosition;
            float f = Miscs.AngleFromToIgnoreY(-base._owner.cameraForward, -vector);
            if ((Mathf.Abs(f) < 20f) || (this._timer < 0f))
            {
                base.End();
            }
            else
            {
                float b = base._owner.anchorPolar - f;
                base._owner.anchorPolar = Mathf.Lerp(base._owner.anchorPolar, b, (Time.deltaTime * 5f) / 5f);
                base._owner.posLerpRatio = 2f;
                base._owner.forwardLerpRatio = 2f;
                base._owner.needLerpPositionThisFrame = true;
                base._owner.needLerpForwardThisFrame = true;
            }
        }

        public void SetLookAtTarget(Vector3 targetPosition, bool mute = false)
        {
            this._targetPosition = targetPosition;
            this._mute = mute;
        }
    }
}

