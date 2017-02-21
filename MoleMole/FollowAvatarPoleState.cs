namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FollowAvatarPoleState : BaseFollowBaseState
    {
        private bool _enteringLerp;
        private float _enteringLerpDuration;
        private float _lerpTimer;
        private float _startForwardDelta;
        private float _startPolar;
        private float _targetForwardDelta;
        private float _targetPolar;

        public FollowAvatarPoleState(MainCameraFollowState followState) : base(followState)
        {
        }

        public void CancelEnteringLerp()
        {
            this._enteringLerp = false;
        }

        public override void Enter()
        {
            if (base._owner.lastBaseState == base._owner.followAvatarAndTargetState)
            {
                this._enteringLerp = true;
                this._lerpTimer = 0f;
                this._startPolar = base._owner.anchorPolar;
                this._startForwardDelta = base._owner.forwardDeltaAngle;
                this._targetForwardDelta = 0f;
                this._enteringLerpDuration = Miscs.AbsAngleDiff(this._startPolar, this._targetPolar) * 0.1f;
                base.cannotBeSkipped = true;
            }
        }

        public override void Exit()
        {
            if (base.maskedShortStates.Contains(base._owner.rotateToAvatarFacingState))
            {
                base.maskedShortStates.Remove(base._owner.rotateToAvatarFacingState);
            }
            this._enteringLerp = false;
            base._owner.forwardDeltaAngle = 0f;
        }

        private float GetPoleFollowAnchor()
        {
            Vector3 vector = base._owner.followData.anchorPosition - base._owner.followCenterXZPosition;
            return (Mathf.Atan2(vector.z, vector.x) * 57.29578f);
        }

        public override void ResetState()
        {
            this._enteringLerp = false;
        }

        public void SetEnteringLerpTarget(float targetPolar)
        {
            this._targetPolar = targetPolar;
            base.maskedShortStates.Add(base._owner.rotateToAvatarFacingState);
        }

        public override void Update()
        {
            if (this._enteringLerp)
            {
                this._lerpTimer += Time.deltaTime * base._owner.mainCamera.TimeScale;
                if (this._lerpTimer > this._enteringLerpDuration)
                {
                    base._owner.anchorPolar = this._targetPolar;
                    base._owner.forwardDeltaAngle = this._targetForwardDelta;
                    this._enteringLerp = false;
                    base.maskedShortStates.Remove(base._owner.rotateToAvatarFacingState);
                    base.cannotBeSkipped = false;
                }
                else
                {
                    float t = this._lerpTimer / this._enteringLerpDuration;
                    base._owner.anchorPolar = Mathf.Lerp(this._startPolar, this._targetPolar, t);
                    base._owner.forwardDeltaAngle = Mathf.Lerp(this._startForwardDelta, this._targetForwardDelta, t);
                }
            }
            else
            {
                base._owner.anchorPolar = this.GetPoleFollowAnchor();
            }
        }
    }
}

