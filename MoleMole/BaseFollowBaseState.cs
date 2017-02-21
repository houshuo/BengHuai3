namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public abstract class BaseFollowBaseState : State<MainCameraFollowState>
    {
        [InspectorCollapsedFoldout]
        public HashSet<BaseFollowShortState> maskedShortStates;

        public BaseFollowBaseState(MainCameraFollowState followState) : base(followState)
        {
            this.maskedShortStates = new HashSet<BaseFollowShortState>();
        }

        public abstract void ResetState();

        public bool cannotBeSkipped { get; protected set; }
    }
}

