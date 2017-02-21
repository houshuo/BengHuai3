namespace MoleMole
{
    using System;

    public class LDWaitStoryEnd : BaseLDEvent
    {
        public override void OnEvent(BaseEvent evt)
        {
            EvtStoryState state = evt as EvtStoryState;
            if ((state != null) && (state.StoryState == EvtStoryState.State.Finish))
            {
                base.Done();
            }
        }
    }
}

