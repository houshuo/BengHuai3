namespace MoleMole
{
    using System;

    public class EvtDefendSuccess : BaseEvent, IEvtWithOtherID, IEvtWithAnimEventID
    {
        public readonly uint attackerID;
        public readonly string skillID;

        public EvtDefendSuccess(uint targetID, uint attackerID, string skillID) : base(targetID)
        {
            this.attackerID = attackerID;
            this.skillID = skillID;
        }

        public string GetAnimEventID()
        {
            return this.skillID;
        }

        public uint GetOtherID()
        {
            return this.attackerID;
        }
    }
}

