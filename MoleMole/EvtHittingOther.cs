namespace MoleMole
{
    using System;
    using UnityEngine;

    public class EvtHittingOther : BaseEvent, IEvtWithOtherID, IEvtWithAttackResult, IEvtWithAnimEventID, IEvtWithHitCollision, IEvtWithRemoteID
    {
        public string animEventID;
        public AttackData attackData;
        public AttackResult.HitCollsion hitCollision;
        public uint toID;

        public EvtHittingOther()
        {
            base.requireResolve = true;
        }

        public EvtHittingOther(uint fromID, uint toID, AttackData attackData) : base(fromID, false, true)
        {
            this.toID = toID;
            this.attackData = attackData;
            this.animEventID = null;
        }

        public EvtHittingOther(uint fromID, uint toID, string animEventID) : base(fromID, false, true)
        {
            this.animEventID = animEventID;
            this.toID = toID;
        }

        public EvtHittingOther(uint fromID, uint toID, string animEventID, AttackData attackData) : base(fromID, false, true)
        {
            this.toID = toID;
            this.animEventID = animEventID;
            this.attackData = attackData;
        }

        public EvtHittingOther(uint fromID, uint toID, string animEventID, Vector3 hitPoint, Vector3 hitForward) : this(fromID, toID, animEventID)
        {
            AttackResult.HitCollsion collsion = new AttackResult.HitCollsion {
                hitPoint = hitPoint,
                hitDir = hitForward
            };
            this.hitCollision = collsion;
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
            return this.hitCollision;
        }

        public uint GetOtherID()
        {
            return this.toID;
        }

        public uint GetRemoteID()
        {
            return this.toID;
        }

        public uint GetSenderID()
        {
            return base.targetID;
        }

        public override string ToString()
        {
            object[] args = new object[] { base.GetDebugName(base.targetID), base.GetDebugName(this.toID), base.GetDebugName(this.animEventID), (this.attackData != null) ? this.attackData.damage.ToString() : "<null>" };
            return string.Format("{0} hitting {1} by skill {2}, caused damage {3}", args);
        }
    }
}

