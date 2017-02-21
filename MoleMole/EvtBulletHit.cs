namespace MoleMole
{
    using System;

    public class EvtBulletHit : BaseEvent, IEvtWithOtherID, IEvtWithHitCollision, IEvtWithRemoteID
    {
        public bool cannotBeReflected;
        public AttackResult.HitCollsion hitCollision;
        public bool hitEnvironment;
        public bool hitGround;
        public uint otherID;
        public uint ownerID;
        public bool selfExplode;

        public EvtBulletHit()
        {
        }

        public EvtBulletHit(uint targetID) : base(targetID)
        {
            this.otherID = 0x21800001;
            this.hitEnvironment = true;
        }

        public EvtBulletHit(uint targetID, uint otherID) : base(targetID)
        {
            this.otherID = otherID;
        }

        public uint GetChannelID()
        {
            return this.ownerID;
        }

        public AttackResult.HitCollsion GetHitCollision()
        {
            return this.hitCollision;
        }

        public uint GetOtherID()
        {
            return this.otherID;
        }

        public uint GetRemoteID()
        {
            return this.otherID;
        }

        public uint GetSenderID()
        {
            return this.ownerID;
        }

        public override string ToString()
        {
            return string.Format("{0} bullet hits {1}(SelfExplode : {2})", base.GetDebugName(this.otherID), base.GetDebugName(base.targetID), this.selfExplode.ToString());
        }
    }
}

