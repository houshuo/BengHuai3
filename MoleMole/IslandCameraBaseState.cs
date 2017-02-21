namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    public class IslandCameraBaseState
    {
        protected MonoIslandCameraSM _sm;

        public virtual void Enter(IslandCameraBaseState lastState, object param = null)
        {
        }

        public virtual void Exit(IslandCameraBaseState nextState)
        {
        }

        public virtual void OnDrag(Gesture gesture)
        {
        }

        public virtual void OnDragEnd(Gesture gesture)
        {
        }

        public virtual void OnDragStart(Gesture gesture)
        {
        }

        public virtual void OnSwipe(Gesture gesture)
        {
        }

        public virtual void OnSwipeEnd(Gesture gesture)
        {
        }

        public virtual void OnTouchStart(Gesture gesture)
        {
        }

        public virtual void OnTouchUp(Gesture gesture)
        {
        }

        public virtual void Update()
        {
        }
    }
}

