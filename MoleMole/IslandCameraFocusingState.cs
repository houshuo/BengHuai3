namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    public class IslandCameraFocusingState : IslandCameraBaseState
    {
        private MonoIslandBuilding _building;

        public IslandCameraFocusingState(MonoIslandCameraSM sm)
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
    }
}

