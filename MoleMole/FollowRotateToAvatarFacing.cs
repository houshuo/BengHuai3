namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FollowRotateToAvatarFacing : BaseFollowShortState
    {
        private float _polar;
        private float _targetPolar;

        public FollowRotateToAvatarFacing(MainCameraFollowState followState) : base(followState)
        {
            base.isSkippingBaseState = true;
        }

        public bool CanRotate()
        {
            return (Mathf.Abs(Miscs.AngleFromToIgnoreY(-base._owner.cameraForward, -base._owner.avatar.FaceDirection)) > 10f);
        }

        public override void Enter()
        {
            if (base._owner.followAvatarControlledRotate.active)
            {
                base._owner.followAvatarControlledRotate.SetExitingControl(false);
            }
            float f = Miscs.AngleFromToIgnoreY(-base._owner.cameraForward, -base._owner.avatar.FaceDirection);
            float num2 = Mathf.Abs(f);
            if (num2 < 10f)
            {
                this._targetPolar = base._owner.anchorPolar;
            }
            else if ((num2 > 90f) && !base._owner.avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Stable))
            {
                this._targetPolar = base._owner.anchorPolar - (Mathf.Sign(f) * 90f);
            }
            else
            {
                this._targetPolar = base._owner.anchorPolar - f;
            }
            this._polar = base._owner.anchorPolar;
        }

        public override void Exit()
        {
            if (base._owner.followAvatarControlledRotate.active)
            {
                base._owner.followAvatarControlledRotate.SetExitingControl(true);
            }
            else
            {
                base._owner.forwardLerpRatio = 8f;
                base._owner.posLerpRatio = 8f;
                base._owner.recoverState.TryRecover();
            }
        }

        public override void PostUpdate()
        {
            if (Mathf.Abs((float) (this._polar - this._targetPolar)) < 2f)
            {
                base.End();
            }
            else
            {
                this._polar = Mathf.Lerp(this._polar, this._targetPolar, Time.deltaTime * 5f);
                base._owner.anchorPolar = this._polar;
            }
            base._owner.needLerpPositionThisFrame = false;
            base._owner.needLerpForwardThisFrame = false;
        }
    }
}

