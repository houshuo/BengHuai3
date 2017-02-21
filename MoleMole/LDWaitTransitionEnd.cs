namespace MoleMole
{
    using System;

    public class LDWaitTransitionEnd : BaseLDEvent
    {
        public override void OnEvent(BaseEvent evt)
        {
            if (evt is EvtLevelState)
            {
                EvtLevelState state = evt as EvtLevelState;
                if (state.state == EvtLevelState.State.ExitTransition)
                {
                    base.Done();
                }
            }
        }
    }
}

