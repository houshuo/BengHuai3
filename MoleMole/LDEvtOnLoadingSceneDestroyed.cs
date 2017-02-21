namespace MoleMole
{
    using System;

    public class LDEvtOnLoadingSceneDestroyed : BaseLDEvent
    {
        public override void OnEvent(BaseEvent evt)
        {
            if (evt is EvtLoadingState)
            {
                EvtLoadingState state = evt as EvtLoadingState;
                if ((state != null) && (state.state == EvtLoadingState.State.Destroy))
                {
                    base.Done();
                }
            }
        }
    }
}

