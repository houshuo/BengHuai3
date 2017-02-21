namespace MoleMole
{
    using System;

    public class EvtEvadeSuccess : BaseEvent, IEvtWithOtherID, IEvtWithAttackResult, IEvtWithAnimEventID, IEvtWithRemoteID
    {
        public AttackData attackData;
        public uint attackerID;
        public string skillID;

        public EvtEvadeSuccess()
        {
        }

        public EvtEvadeSuccess(uint targetID, uint attackerID, string skillID, AttackData attackData) : base(targetID)
        {
            this.attackerID = attackerID;
            this.skillID = skillID;
            this.attackData = attackData;
        }

        public string GetAnimEventID()
        {
            return this.skillID;
        }

        public AttackResult GetAttackResult()
        {
            return this.attackData;
        }

        public uint GetChannelID()
        {
            return base.targetID;
        }

        public uint GetOtherID()
        {
            return this.attackerID;
        }

        public uint GetRemoteID()
        {
            return base.targetID;
        }

        public uint GetSenderID()
        {
            return this.attackerID;
        }
    }
}

