namespace MoleMole
{
    using System;

    public class EvtSkillStart : BaseEvent
    {
        public readonly string skillID;

        public EvtSkillStart(uint targetID, string skillID) : base(targetID)
        {
            this.skillID = skillID;
        }

        public override string ToString()
        {
            return string.Format("{0} starts skill {1}", base.GetDebugName(base.targetID), this.skillID);
        }
    }
}

