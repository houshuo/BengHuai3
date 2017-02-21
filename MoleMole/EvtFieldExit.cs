namespace MoleMole
{
    using System;

    public class EvtFieldExit : BaseEvent, IEvtWithOtherID
    {
        public readonly uint otherID;

        public EvtFieldExit(uint targetID, uint otherID) : base(targetID)
        {
            this.otherID = otherID;
        }

        public uint GetOtherID()
        {
            return this.otherID;
        }

        public override string ToString()
        {
            return string.Format("{0} exiting field {1}", base.GetDebugName(this.otherID), base.GetDebugName(base.targetID));
        }
    }
}

