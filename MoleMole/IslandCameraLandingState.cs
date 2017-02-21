namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    public class IslandCameraLandingState : IslandCameraBaseState
    {
        private MonoIslandBuilding _building;

        public IslandCameraLandingState(MonoIslandCameraSM sm)
        {
            base._sm = sm;
        }

        public override void Enter(IslandCameraBaseState lastState, object param = null)
        {
            base.Enter(lastState, param);
            this._building = param as MonoIslandBuilding;
            (Singleton<MainUIManager>.Instance.SceneCanvas as MonoIslandUICanvas).TriggerFullScreenBlock(false);
        }

        public override void Exit(IslandCameraBaseState nextState)
        {
            this._building = null;
        }

        public MonoIslandBuilding GetBuilding()
        {
            return this._building;
        }

        public override void OnTouchUp(Gesture gesture)
        {
            if (((gesture.pickedObject == null) || (gesture.pickedObject != this._building.gameObject)) && (Singleton<MainUIManager>.Instance.CurrentPageContext != null))
            {
                Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
            }
        }
    }
}

