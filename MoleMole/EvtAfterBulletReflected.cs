namespace MoleMole
{
    using System;

    public class EvtAfterBulletReflected : BaseEvent, IEvtWithOtherID
    {
        public AttackData attackData;
        public readonly uint bulletID;
        public readonly uint launcherID;

        public EvtAfterBulletReflected(uint targetID, uint bulletID, uint launcherID, AttackData attackData) : base(targetID)
        {
            this.bulletID = bulletID;
            this.launcherID = launcherID;
            this.attackData = attackData;
        }

        public uint GetOtherID()
        {
            return this.bulletID;
        }

        public override string ToString()
        {
            return string.Format("{0} bullet hits {1}", base.GetDebugName(this.bulletID), base.GetDebugName(base.targetID));
        }
    }
}

