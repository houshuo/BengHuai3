namespace MoleMole
{
    using System;

    public class LDEvtLevelTimesUp : BaseLDEvent
    {
        public override void OnEvent(BaseEvent evt)
        {
            if (evt is EvtLevelTimesUp)
            {
                base.Done();
            }
        }
    }
}

