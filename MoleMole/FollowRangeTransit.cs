namespace MoleMole
{
    using System;
    using UnityEngine;

    public class FollowRangeTransit : BaseFollowShortState
    {
        private float _origAnchorElevation;
        private float _origAnchorRadius;
        private MainCameraFollowState.RangeState _rangeState;
        private float _targetAnchorElevation;
        private float _targetAnchorRadius;
        private float _timer;
        private const float RANGE_TRANSIT_LINEAR_LERP_TIME = 0.7f;

        public FollowRangeTransit(MainCameraFollowState followState) : base(followState)
        {
            base.isSkippingBaseState = false;
        }

        public override void Enter()
        {
            this._timer = 0f;
            this._origAnchorRadius = base._owner.anchorRadius;
            this._origAnchorElevation = base._owner.anchorElevation;
            if (this._rangeState == MainCameraFollowState.RangeState.Near)
            {
                this._targetAnchorRadius = 6f;
                this._targetAnchorElevation = 3.5f;
            }
            else if (this._rangeState == MainCameraFollowState.RangeState.Far)
            {
                this._targetAnchorRadius = 7f;
                this._targetAnchorElevation = 7f;
            }
            else if (this._rangeState == MainCameraFollowState.RangeState.Furter)
            {
                this._targetAnchorRadius = 8.5f;
                this._targetAnchorElevation = 7f;
            }
            else if (this._rangeState == MainCameraFollowState.RangeState.High)
            {
                this._targetAnchorElevation = 10f;
            }
            else if (this._rangeState == MainCameraFollowState.RangeState.Higher)
            {
                this._targetAnchorElevation = 15f;
            }
            base._owner.recoverState.SetupRecoverRadius(this._targetAnchorRadius);
            base._owner.recoverState.SetupRecoverElevation(this._targetAnchorElevation);
        }

        public override void Exit()
        {
            base._owner.recoverState.TryRecover();
        }

        public void SetRange(MainCameraFollowState.RangeState rangeState)
        {
            this._rangeState = rangeState;
        }

        public override void Update()
        {
            this._timer += Time.deltaTime * base._owner.mainCamera.TimeScale;
            if (this._timer < 0.7f)
            {
                base._owner.anchorRadius = Mathf.Lerp(this._origAnchorRadius, this._targetAnchorRadius, this._timer / 0.7f);
                base._owner.anchorElevation = Mathf.Lerp(this._origAnchorElevation, this._targetAnchorElevation, this._timer / 0.7f);
            }
            else
            {
                base.End();
            }
        }
    }
}

