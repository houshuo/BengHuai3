namespace MoleMole
{
    using System;

    public class EvtLevelTimesUp : BaseEvent
    {
        public EvtLevelTimesUp(uint ownerID) : base(ownerID)
        {
        }

        public override string ToString()
        {
            return "Level Times Up";
        }
    }
}

