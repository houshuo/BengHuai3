namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class IslandCameraBackToBaseState : IslandCameraBaseState
    {
        private MonoIslandBuilding _building;
        private float _finalPitch;
        private Vector3 _finalPos;
        private float _startPitch;
        private Vector3 _startPos;
        private float _startTime;
        private float _totalTime;

        public IslandCameraBackToBaseState(MonoIslandCameraSM sm)
        {
            base._sm = sm;
        }

        public override void Enter(IslandCameraBaseState lastState, object param = null)
        {
            base.Enter(lastState, param);
            this._startPos = base._sm.GetPivot();
            this._finalPos = base._sm.GetCameraBasePos().position;
            this._startPitch = base._sm.GetLookAtPitch();
            this._finalPitch = base._sm.GetCameraBasePos().eulerAngles.x;
            this._startTime = Time.time;
            Vector3 vector2 = this._finalPos - this._startPos;
            float magnitude = vector2.magnitude;
            this._totalTime = magnitude / base._sm.GetBackToBaseSpeed(magnitude);
            base._sm.TriggerCover(E_AlphaLerpDir.ToLittle);
            this._building = param as MonoIslandBuilding;
            if (this._building != null)
            {
                this._building.TriggerHighLight(E_AlphaLerpDir.ToLittle);
            }
        }

        public override void Exit(IslandCameraBaseState nextState)
        {
            if (this._building != null)
            {
                this._building.SetRenderQueue(E_IslandRenderQueue.Back);
            }
            if (Singleton<MainUIManager>.Instance != null)
            {
                MonoIslandUICanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas;
                if (sceneCanvas != null)
                {
                    sceneCanvas.SetBuildingEffect(this._building, true);
                }
            }
            this._building = null;
            if ((base._sm != null) && (base._sm.GetGyroManager() != null))
            {
                base._sm.GetGyroManager().SetEnable(true);
            }
        }

        public override void Update()
        {
            if (this._totalTime <= 0f)
            {
                base._sm.SetPivot(this._finalPos);
                base._sm.SetLookAtPitch(this._finalPitch);
                base._sm.GotoState(E_IslandCameraState.Swipe, null);
            }
            else
            {
                float num = (Time.time - this._startTime) / this._totalTime;
                if (num >= 1f)
                {
                    base._sm.SetPivot(this._finalPos);
                    base._sm.SetLookAtPitch(this._finalPitch);
                    base._sm.GotoState(E_IslandCameraState.Swipe, null);
                }
                else
                {
                    float num2 = (2f * num) - (num * num);
                    num2 = Mathf.Clamp(num2, 0f, 1f);
                    base._sm.SetPivot(Vector3.Lerp(this._startPos, this._finalPos, num2));
                    base._sm.SetLookAtPitch(Mathf.Lerp(this._startPitch, this._finalPitch, num2));
                }
            }
        }
    }
}

