namespace MoleMole
{
    using System;

    public class EvtFieldEnter : BaseEvent, IEvtWithOtherID
    {
        public readonly uint otherID;

        public EvtFieldEnter(uint targetID, uint otherID) : base(targetID)
        {
            this.otherID = otherID;
        }

        public uint GetOtherID()
        {
            return this.otherID;
        }

        public override string ToString()
        {
            return string.Format("{0} entering field {1}", base.GetDebugName(this.otherID), base.GetDebugName(base.targetID));
        }
    }
}

