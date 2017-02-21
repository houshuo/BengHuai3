namespace MoleMole
{
    using System;

    public class EvtChargeRelease : BaseEvent
    {
        public bool isSwitchRelease;
        public string releaseSkillID;

        public EvtChargeRelease(uint targetID, string releaseSkillID) : base(targetID)
        {
            this.releaseSkillID = releaseSkillID;
        }

        public override string ToString()
        {
            return string.Format("{0} charge {1} release, Is switch release: {2}", base.GetDebugName(base.targetID), this.releaseSkillID, this.isSwitchRelease);
        }
    }
}

