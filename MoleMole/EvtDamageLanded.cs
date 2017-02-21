namespace MoleMole
{
    using System;

    public class EvtDamageLanded : BaseEvent
    {
        public uint attackeeID;
        public AttackResult attackResult;

        public EvtDamageLanded(uint fromID, uint attackeeID, AttackResult attackResult) : base(fromID)
        {
            this.attackeeID = attackeeID;
            this.attackResult = attackResult;
        }

        public override string ToString()
        {
            return string.Format("{0} non aattack damage Landed on {1}", base.GetDebugName(base.targetID), base.GetDebugName(this.attackeeID));
        }
    }
}

