namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FollowAvatarStoryState : BaseFollowBaseState
    {
        private Vector3 _targetCameraPos;

        public FollowAvatarStoryState(MainCameraFollowState followState) : base(followState)
        {
        }

        public override void Enter()
        {
            this._targetCameraPos = base._owner.cameraPosition + new Vector3(1f, 0f, 0f);
        }

        public override void Exit()
        {
            if (base.maskedShortStates.Contains(base._owner.rotateToAvatarFacingState))
            {
                base.maskedShortStates.Remove(base._owner.rotateToAvatarFacingState);
            }
            base._owner.forwardDeltaAngle = 0f;
        }

        public override void ResetState()
        {
        }

        public void SetEnteringLerpTarget(float targetPolar)
        {
            base.maskedShortStates.Add(base._owner.rotateToAvatarFacingState);
        }

        public void StopStoryState()
        {
            this.Exit();
        }

        public override void Update()
        {
            base._owner.cameraPosition = this._targetCameraPos;
        }
    }
}

