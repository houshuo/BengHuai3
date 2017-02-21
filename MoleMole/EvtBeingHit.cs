namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class EvtBeingHit : BaseEvent, IEvtWithOtherID, IEvtWithAttackResult, IEvtWithAnimEventID, IEvtWithHitCollision, IEvtWithRemoteID
    {
        public string animEventID;
        public AttackData attackData;
        public BeHitEffect beHitEffect;
        public float resolvedDamage;
        public uint sourceID;

        public EvtBeingHit()
        {
            base.requireResolve = true;
        }

        public EvtBeingHit(uint toID, uint fromID, string animEventID, AttackData attackData) : base(toID, false, true)
        {
            this.animEventID = animEventID;
            this.sourceID = fromID;
            this.attackData = attackData;
        }

        public string GetAnimEventID()
        {
            return this.animEventID;
        }

        public AttackResult GetAttackResult()
        {
            return this.attackData;
        }

        public uint GetChannelID()
        {
            return base.targetID;
        }

        public AttackResult.HitCollsion GetHitCollision()
        {
            return this.attackData.hitCollision;
        }

        public uint GetOtherID()
        {
            return this.sourceID;
        }

        public uint GetRemoteID()
        {
            return base.targetID;
        }

        public uint GetSenderID()
        {
            return this.sourceID;
        }

        public override string ToString()
        {
            object[] args = new object[] { base.GetDebugName(base.targetID), base.GetDebugName(this.sourceID), this.animEventID, this.attackData.damage };
            return string.Format("{0} being hit by {1} on skill {2}, caused damage {3}", args);
        }
    }
}

