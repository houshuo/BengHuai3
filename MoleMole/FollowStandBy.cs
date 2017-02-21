namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FollowStandBy : BaseFollowShortState
    {
        private float _origCenterOffsetY;
        private float _origRadius;
        private State _state;
        private float _targetCenterOffsetY;
        private float _targetRadius;
        private float _timer;
        private const float RADIUS_LINEAR_LERP_ENTER_TIME = 1.5f;
        private const float RADIUS_LINEAR_LERP_EXIT_TIME = 0.3f;

        public FollowStandBy(MainCameraFollowState followState) : base(followState)
        {
            base.isSkippingBaseState = true;
        }

        public override void Enter()
        {
            if (base._owner.recoverState.active)
            {
                base._owner.recoverState.CancelPosAndForwardRecover();
            }
            this._origRadius = base._owner.anchorRadius;
            this._targetRadius = 3.84f;
            this._origCenterOffsetY = base._owner.followCenterY;
            this._targetCenterOffsetY = base._owner.followCenterY * 0.85f;
            this._timer = 0f;
            this._state = State.Entering;
            base._owner.posLerpRatio = 0.2f;
            base._owner.forwardLerpRatio = 0.2f;
        }

        public override void Exit()
        {
            base._owner.recoverState.TryRecover();
            base._owner.posLerpRatio = 1f;
            base._owner.forwardLerpRatio = 1f;
        }

        public override void Update()
        {
            if (this._state == State.Entering)
            {
                if (!base._owner.avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Stable))
                {
                    this._timer = 0f;
                    base._owner.posLerpRatio = 1f;
                    base._owner.forwardLerpRatio = 1f;
                    this._state = State.Exiting;
                }
                else
                {
                    this._timer += Time.deltaTime * base._owner.mainCamera.TimeScale;
                    if (this._timer < 1.5f)
                    {
                        float t = this._timer / 1.5f;
                        base._owner.anchorRadius = Mathf.Lerp(this._origRadius, this._targetRadius, t);
                        base._owner.followCenterY = Mathf.Lerp(this._origCenterOffsetY, this._targetCenterOffsetY, t);
                    }
                    else
                    {
                        base._owner.anchorRadius = this._targetRadius;
                        base._owner.followCenterY = this._targetCenterOffsetY;
                        this._state = State.StandBy;
                    }
                }
            }
            else if (this._state == State.StandBy)
            {
                if (!base._owner.avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Stable))
                {
                    base._owner.posLerpRatio = 1f;
                    base._owner.forwardLerpRatio = 1f;
                    this._timer = 0f;
                    this._state = State.Exiting;
                }
            }
            else if (this._state == State.Exiting)
            {
                this._timer += Time.deltaTime * base._owner.mainCamera.TimeScale;
                if (this._timer < 0.3f)
                {
                    float num2 = this._timer / 0.3f;
                    base._owner.anchorRadius = Mathf.Lerp(this._targetRadius, this._origRadius, num2);
                    base._owner.followCenterY = Mathf.Lerp(this._targetCenterOffsetY, this._origCenterOffsetY, num2);
                }
                else
                {
                    base._owner.anchorRadius = this._origRadius;
                    base._owner.followCenterY = this._origCenterOffsetY;
                    base.End();
                }
            }
        }

        private enum State
        {
            Entering,
            StandBy,
            Exiting
        }
    }
}

