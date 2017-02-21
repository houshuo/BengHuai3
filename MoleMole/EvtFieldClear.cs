namespace MoleMole
{
    using System;

    public class EvtFieldClear : BaseEvent
    {
        public EvtFieldClear(uint targetID) : base(targetID)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} clear field", base.GetDebugName(base.targetID));
        }
    }
}

