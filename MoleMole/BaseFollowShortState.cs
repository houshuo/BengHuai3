namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Runtime.CompilerServices;

    [fiInspectorOnly]
    public class BaseFollowShortState : State<MainCameraFollowState>
    {
        public BaseFollowShortState(MainCameraFollowState followState) : base(followState)
        {
        }

        protected void End()
        {
            base._owner.RemoveShortState();
        }

        public virtual void PostUpdate()
        {
        }

        public bool isInteruptable { get; protected set; }

        public bool isSkippingBaseState { get; protected set; }
    }
}

