namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class IslandCameraDragBackState : IslandCameraBaseState
    {
        private DragBackPoint _dragBackPoint;
        private Vector3 _startPos;
        private float _startTime;
        private Vector3 _toPos;
        private float _totalTime;

        public IslandCameraDragBackState(MonoIslandCameraSM sm)
        {
            base._sm = sm;
        }

        public override void Enter(IslandCameraBaseState lastState, object param = null)
        {
            base.Enter(lastState, param);
            this._dragBackPoint = param as DragBackPoint;
            this._startPos = base._sm.GetPivot();
            this._toPos = this._dragBackPoint._finalPos;
            this._startTime = Time.time;
            Vector3 vector = this._toPos - this._startPos;
            this._totalTime = vector.magnitude / base._sm._dragBack_speed;
        }

        public override void Exit(IslandCameraBaseState nextState)
        {
        }

        public override void OnTouchStart(Gesture gesture)
        {
            base._sm.GotoState(E_IslandCameraState.Swipe, gesture);
        }

        public override void Update()
        {
            if (this._totalTime <= 0f)
            {
                base._sm.SetPivot(this._toPos);
                base._sm.GotoState(E_IslandCameraState.Swipe, null);
            }
            float num = (Time.time - this._startTime) / this._totalTime;
            if (num >= 1f)
            {
                base._sm.SetPivot(this._toPos);
                base._sm.GotoState(E_IslandCameraState.Swipe, null);
            }
            else
            {
                float num2 = (2f * num) - (num * num);
                num2 = Mathf.Clamp(num2, 0f, 1f);
                base._sm.SetPivot(Vector3.Lerp(this._startPos, this._toPos, num2));
            }
        }
    }
}

