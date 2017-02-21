namespace MoleMole
{
    using System;

    public class LDWaitVideoEnd : BaseLDEvent
    {
        public override void OnEvent(BaseEvent evt)
        {
            EvtVideoState state = evt as EvtVideoState;
            if ((state != null) && (state.VideoState == EvtVideoState.State.Finish))
            {
                base.Done();
            }
        }
    }
}

