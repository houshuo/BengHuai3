namespace MoleMole
{
    using System;

    public class FollowSuddenRecover : BaseFollowShortState
    {
        public FollowSuddenRecover(MainCameraFollowState followState) : base(followState)
        {
            base.isSkippingBaseState = true;
        }

        public override void PostUpdate()
        {
            if (base._owner.recoverState.active)
            {
                base._owner.recoverState.CancelAndJumpToOriginalState();
            }
            base._owner.needLerpForwardThisFrame = false;
            base._owner.needLerpPositionThisFrame = false;
            base.End();
        }
    }
}

