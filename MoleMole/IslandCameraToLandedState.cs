namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class IslandCameraToLandedState : IslandCameraBaseState
    {
        private MonoIslandBuilding _building;
        private bool _fire_prelanded;
        private float _landedPitch;
        private Vector3 _landedPos;
        private float _startPitch;
        private Vector3 _startPos;
        private float _startTime;
        private float _totalTime;

        public IslandCameraToLandedState(MonoIslandCameraSM sm)
        {
            base._sm = sm;
        }

        public override void Enter(IslandCameraBaseState lastState, object param = null)
        {
            base.Enter(lastState, param);
            this._building = param as MonoIslandBuilding;
            if ((this._building != null) && (base._sm != null))
            {
                this._startPos = base._sm.GetPivot();
                this._landedPos = this._building.GetLandedPos();
                this._startTime = Time.time;
                Vector3 vector = this._landedPos - this._startPos;
                float magnitude = vector.magnitude;
                this._totalTime = magnitude / base._sm.GetLandedSpeedFinal(magnitude);
                this._fire_prelanded = false;
                this._startPitch = base._sm.GetLookAtPitch();
                this._landedPitch = this._building.GetLandedPitch();
                if ((base._sm != null) && (base._sm.GetGyroManager() != null))
                {
                    base._sm.GetGyroManager().SetEnable(false);
                }
                if (Singleton<MainUIManager>.Instance != null)
                {
                    MonoIslandUICanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas;
                    if (sceneCanvas != null)
                    {
                        sceneCanvas.TriggerFullScreenBlock(true);
                    }
                }
            }
        }

        public override void Exit(IslandCameraBaseState nextState)
        {
            this._building = null;
            if (Singleton<MainUIManager>.Instance != null)
            {
                MonoIslandUICanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas;
                if (sceneCanvas != null)
                {
                    sceneCanvas.TriggerFullScreenBlock(false);
                }
            }
        }

        public override void Update()
        {
            if (this._totalTime <= 0f)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraPreLanded, this._building));
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraLanded, this._building));
                base._sm.TriggerCover(E_AlphaLerpDir.ToLarge);
                this._building.SetRenderQueue(E_IslandRenderQueue.Front);
                this._building.TriggerHighLight(E_AlphaLerpDir.ToLarge);
                (Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas).SetBuildingEffect(this._building, false);
                base._sm.SetPivot(this._landedPos);
                base._sm.SetLookAtPitch(this._landedPitch);
                base._sm.GotoState(E_IslandCameraState.Landing, this._building);
            }
            else
            {
                float t = (Time.time - this._startTime) / this._totalTime;
                if (t >= 1f)
                {
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraLanded, this._building));
                    base._sm.SetPivot(this._landedPos);
                    base._sm.SetLookAtPitch(this._landedPitch);
                    base._sm.GotoState(E_IslandCameraState.Landing, this._building);
                }
                else
                {
                    float num2 = Mathf.Clamp(base._sm.GetNomalizedCurvePos(t), 0f, 1f);
                    base._sm.SetPivot(Vector3.Lerp(this._startPos, this._landedPos, num2));
                    base._sm.SetLookAtPitch(Mathf.Lerp(this._startPitch, this._landedPitch, num2));
                    if (!this._fire_prelanded && (((this._startTime + this._totalTime) - Time.time) < base._sm._pre_landed_time))
                    {
                        this._fire_prelanded = true;
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandCameraPreLanded, this._building));
                        base._sm.TriggerCover(E_AlphaLerpDir.ToLarge);
                        this._building.SetRenderQueue(E_IslandRenderQueue.Front);
                        this._building.TriggerHighLight(E_AlphaLerpDir.ToLarge);
                        (Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas).SetBuildingEffect(this._building, false);
                    }
                }
            }
        }
    }
}

