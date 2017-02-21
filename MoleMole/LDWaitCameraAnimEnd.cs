namespace MoleMole
{
    using System;

    public class LDWaitCameraAnimEnd : BaseLDEvent
    {
        public override void OnEvent(BaseEvent evt)
        {
            if (evt is EvtCamearaAnimState)
            {
                EvtCamearaAnimState state = evt as EvtCamearaAnimState;
                if ((state != null) && (state.CameraAnimState == EvtCamearaAnimState.State.Finish))
                {
                    base.Done();
                }
            }
        }
    }
}

