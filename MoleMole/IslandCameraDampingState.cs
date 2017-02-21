namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class IslandCameraDampingState : IslandCameraBaseState
    {
        public IslandCameraDampingState(MonoIslandCameraSM sm)
        {
            base._sm = sm;
        }

        public override void Enter(IslandCameraBaseState lastState, object param = null)
        {
            base.Enter(lastState, param);
        }

        public override void Exit(IslandCameraBaseState nextState)
        {
        }

        public override void OnTouchStart(Gesture gesture)
        {
            base._sm.GotoState(E_IslandCameraState.Swipe, gesture);
        }

        public override void OnTouchUp(Gesture gesture)
        {
        }

        public override void Update()
        {
            base._sm.ToDampingSpeed();
            if (Miscs.IsAlmostZero(base._sm.SwipeMoveHandler().magnitude))
            {
                base._sm.SetSwipeSpeed(Vector2.zero);
                base._sm.GotoState(E_IslandCameraState.Swipe, null);
            }
        }
    }
}

