namespace MoleMole
{
    using System;

    public class EvtAttackStart : BaseEvent
    {
        public readonly string skillID;

        public EvtAttackStart(uint targetID, string skillID) : base(targetID)
        {
            this.skillID = skillID;
        }

        public override string ToString()
        {
            return string.Format("{0} starts attack, skillID is {1}", base.GetDebugName(base.targetID), this.skillID);
        }
    }
}

