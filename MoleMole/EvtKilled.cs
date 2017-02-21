namespace MoleMole
{
    using System;

    public class EvtKilled : BaseEvent, IEvtWithOtherID
    {
        public string killerAnimEventID;
        public uint killerID;

        public EvtKilled(uint targetID) : base(targetID)
        {
            this.killerID = 0x21800001;
            this.killerAnimEventID = null;
        }

        public EvtKilled(uint targetID, uint killerID, string killerAnimEventID) : base(targetID)
        {
            this.killerID = killerID;
            this.killerAnimEventID = killerAnimEventID;
        }

        public uint GetOtherID()
        {
            return this.killerID;
        }

        public override string ToString()
        {
            return string.Format("{0} get killed", base.GetDebugName(base.targetID));
        }
    }
}

