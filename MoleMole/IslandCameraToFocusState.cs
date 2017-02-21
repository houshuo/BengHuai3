namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class IslandCameraToFocusState : IslandCameraBaseState
    {
        private MonoIslandBuilding _building;
        private bool _fire_prefocus;
        private float _focusPitch;
        private Vector3 _focusPos;
        private float _startPitch;
        private Vector3 _startPos;
        private float _startTime;
        private float _totalTime;

        public IslandCameraToFocusState(MonoIslandCameraSM sm)
        {
            base._sm = sm;
        }

        public override void Enter(IslandCameraBaseState lastState, object param = null)
        {
            base.Enter(lastState, param);
            this._building = param as MonoIslandBuilding;
            if (this._building != null)
            {
                this._startPos = this._building.GetLandedPos();
                this._focusPos = this._building.GetFocusPos();
                this._startPitch = base._sm.GetLookAtPitch();
                this._focusPitch = this._building.GetFocusPitch();
                this._startTime = Time.time;
                Vector3 vector = this._focusPos - this._startPos;
                this._totalTime = vector.magnitude / base._sm._to_focus_speed;
                this._fire_prefocus = false;
                (Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas).TriggerFullScreenBlock(true);
            }
        }

        public override void Exit(IslandCameraBaseState nextState)
        {
            (Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas).TriggerFullScreenBlock(false);
        }

        public override void Update()
        {
            if (this._totalTime <= 0f)
            {
                base._sm.SetPivot(this._focusPos);
                base._sm.SetLookAtPitch(this._focusPitch);
                base._sm.GotoState(E_IslandCameraState.Focusing, this._building);
            }
            else
            {
                float num = (Time.time - this._startTime) / this._totalTime;
                if (num >= 1f)
                {
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraFocus, this._building));
                    base._sm.SetPivot(this._focusPos);
                    base._sm.SetLookAtPitch(this._focusPitch);
                    base._sm.GotoState(E_IslandCameraState.Focusing, this._building);
                }
                else
                {
                    float num2 = (2f * num) - (num * num);
                    num2 = Mathf.Clamp(num2, 0f, 1f);
                    base._sm.SetPivot(Vector3.Lerp(this._startPos, this._focusPos, num2));
                    base._sm.SetLookAtPitch(Mathf.Lerp(this._startPitch, this._focusPitch, num2));
                    if (!this._fire_prefocus && (((this._startTime + this._totalTime) - Time.time) < base._sm._pre_focus_time))
                    {
                        this._fire_prefocus = true;
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraPreFocus, this._building));
                    }
                }
            }
        }
    }
}

