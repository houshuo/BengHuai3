namespace MoleMole
{
    using System;

    public class EvtTouch : BaseEvent
    {
        public readonly uint otherID;

        public EvtTouch(uint targetID, uint otherID) : base(targetID)
        {
            this.otherID = otherID;
        }

        public override string ToString()
        {
            return string.Format("{0} entering field {1}", base.GetDebugName(this.otherID), base.GetDebugName(base.targetID));
        }
    }
}

