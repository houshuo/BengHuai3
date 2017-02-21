namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FollowSuddenChange : BaseFollowShortState
    {
        private BaseMonoAvatar _nextAvatar;

        public FollowSuddenChange(MainCameraFollowState followState) : base(followState)
        {
            base.isSkippingBaseState = true;
        }

        public override void Enter()
        {
            base._owner.TransitBaseState(base._owner.followAvatarState, false);
            base._owner.SetupFollowAvatar(this._nextAvatar.GetRuntimeID());
            base._owner.followCenterXZPosition = this._nextAvatar.XZPosition;
        }

        public override void Exit()
        {
        }

        public override void PostUpdate()
        {
            Vector3 faceDirection = base._owner.avatar.FaceDirection;
            base._owner.anchorPolar = Mathf.Atan2(-faceDirection.z, -faceDirection.x) * 57.29578f;
            base._owner.needLerpForwardThisFrame = false;
            base._owner.needLerpPositionThisFrame = false;
            base._owner.needSmoothFollowCenterThisFrame = false;
            base.End();
        }

        public void SetSuddenChangeTarget(BaseMonoAvatar avatar)
        {
            this._nextAvatar = avatar;
        }
    }
}

