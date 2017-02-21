namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class IslandCameraSwipeState : IslandCameraBaseState
    {
        private bool _bSwipe = true;
        private int _fingerIndex = -2;
        private GameObject _selectObj;

        public IslandCameraSwipeState(MonoIslandCameraSM sm)
        {
            base._sm = sm;
            this._bSwipe = true;
        }

        public override void Enter(IslandCameraBaseState lastState, object param = null)
        {
            base.Enter(lastState, param);
            if (param != null)
            {
                Gesture gesture = param as Gesture;
                if (gesture != null)
                {
                    this.OnTouchStart(gesture);
                }
            }
        }

        public override void Exit(IslandCameraBaseState nextState)
        {
            this._selectObj = null;
            this._fingerIndex = -2;
        }

        public override void OnDrag(Gesture gesture)
        {
            if (this._bSwipe)
            {
                base._sm.SwipeToWorldSpeed(gesture.deltaPosition);
                base._sm.SwipeMoveHandler();
            }
        }

        public override void OnDragEnd(Gesture gesture)
        {
            if (this._bSwipe)
            {
                if (base._sm._swipeNextState == E_SwipeNextState.SwipeToDamping)
                {
                    base._sm.GotoState(E_IslandCameraState.Damping, null);
                }
                else if (base._sm._swipeNextState == E_SwipeNextState.SwipeToDragBack)
                {
                    base._sm.GotoState(E_IslandCameraState.DragBack, base._sm.GetDragBackPoint());
                }
            }
        }

        public override void OnSwipe(Gesture gesture)
        {
            if (this._bSwipe)
            {
                base._sm.SwipeToWorldSpeed(gesture.deltaPosition);
                base._sm.SwipeMoveHandler();
            }
        }

        public override void OnSwipeEnd(Gesture gesture)
        {
            if (this._bSwipe)
            {
                if (base._sm._swipeNextState == E_SwipeNextState.SwipeToDamping)
                {
                    base._sm.GotoState(E_IslandCameraState.Damping, null);
                }
                else if (base._sm._swipeNextState == E_SwipeNextState.SwipeToDragBack)
                {
                    base._sm.GotoState(E_IslandCameraState.DragBack, base._sm.GetDragBackPoint());
                }
            }
        }

        public override void OnTouchStart(Gesture gesture)
        {
            if (gesture.pickedObject != null)
            {
                this._selectObj = gesture.pickedObject;
                this._fingerIndex = gesture.fingerIndex;
            }
        }

        public override void OnTouchUp(Gesture gesture)
        {
            if (((this._selectObj != null) && (this._selectObj == gesture.pickedObject)) && (this._fingerIndex == gesture.fingerIndex))
            {
                MonoIslandBuilding component = this._selectObj.GetComponent<MonoIslandBuilding>();
                if (component != null)
                {
                    base._sm.GotoState(E_IslandCameraState.ToLanded, component);
                }
            }
        }

        public override void Update()
        {
        }
    }
}

