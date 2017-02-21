namespace MoleMole
{
    using System;

    public class EvtAttackLanded : BaseEvent, IEvtWithOtherID, IEvtWithAttackResult, IEvtWithAnimEventID, IEvtWithHitCollision, IEvtWithRemoteID
    {
        public string animEventID;
        public uint attackeeID;
        public AttackResult attackResult;

        public EvtAttackLanded()
        {
        }

        public EvtAttackLanded(uint fromID, uint attackeeID, string animEventID, AttackResult attackResult) : base(fromID)
        {
            this.attackeeID = attackeeID;
            this.animEventID = animEventID;
            this.attackResult = attackResult;
        }

        public string GetAnimEventID()
        {
            return this.animEventID;
        }

        public AttackResult GetAttackResult()
        {
            return this.attackResult;
        }

        public uint GetChannelID()
        {
            return base.targetID;
        }

        public AttackResult.HitCollsion GetHitCollision()
        {
            return this.attackResult.hitCollision;
        }

        public uint GetOtherID()
        {
            return this.attackeeID;
        }

        public uint GetRemoteID()
        {
            return base.targetID;
        }

        public uint GetSenderID()
        {
            return this.attackeeID;
        }

        public override string ToString()
        {
            return string.Format("{0} attack Landed on {1}, skill {2}", base.GetDebugName(base.targetID), base.GetDebugName(this.attackeeID), this.animEventID);
        }
    }
}

